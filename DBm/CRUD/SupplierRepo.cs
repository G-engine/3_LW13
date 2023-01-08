using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBm;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace WebApp.CRUD;

public class SupplierRepo
{
    private static ConcurrentDictionary<int, Suppliers> suppliersCache;
    private PharmacyContext db;

    public SupplierRepo(PharmacyContext db)
    {
        this.db = db;
        suppliersCache = new ConcurrentDictionary<int, Suppliers>(db.Suppliers.ToDictionary(s=>s.Id));
    }
    private Suppliers UpdateCache(int id, Suppliers supplier)
    {
        Suppliers old;
        if (suppliersCache.TryGetValue(id, out old))
        {
            if (suppliersCache.TryUpdate(id, supplier, old))
            {
                return supplier;
            }
        }
        return null;
    }

    public async Task<Suppliers> CreateAsync(Suppliers supplier)
    {
        EntityEntry<Suppliers> added = await db.Suppliers.AddAsync(supplier);
        int affectedRows = await db.SaveChangesAsync();
        if(affectedRows > 0)
        {
            return suppliersCache.AddOrUpdate(supplier.Id, supplier, UpdateCache);
        }
        return null;
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        Suppliers? s = db.Suppliers.Find(id);
        if(s != null)
        {
            db.Suppliers.Remove(s);
            
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                return suppliersCache.TryRemove(id, out s);
            }
        }
        return null;
    }

    public Task<IEnumerable<Suppliers>> GetAllAsync()
    {
        return Task.Run<IEnumerable<Suppliers>>(() => suppliersCache.Values);
    }

    public Task<Suppliers?> GetAsync(int id)
    {
        return Task.Run(() =>
        {
            suppliersCache.TryGetValue(id, out Suppliers? s);
            return s;
        });
    }

    public async Task<Suppliers> UpdateAsync(int id, Suppliers supplier)
    {
        var old = await db.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
        old.Name = supplier.Name;
        old.Products = supplier.Products;
        int affected = await db.SaveChangesAsync();
        if (affected > 0)
        {
            return UpdateCache(id, supplier);
        }
        return null;
    }
    
    public async Task<Suppliers> UpdateNameAsync(int id, Suppliers supplier)
    {
        var old = await db.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
        old.Name = supplier.Name;
        int affected = await db.SaveChangesAsync();
        if (affected > 0)
        {
            return UpdateCache(id, supplier);
        }
        return null;
    }
}