using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBm;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace WebApp.CRUD;

public class ProductRepo
{
    private static ConcurrentDictionary<int, Products> productsCache;
    private PharmacyContext db;

    public ProductRepo(PharmacyContext db)
    {
        this.db = db;
        productsCache = new ConcurrentDictionary<int, Products>(db.Products.ToDictionary(p => p.Id));
    }
    private Products UpdateCache(int id, Products product)
    {
        Products old;
        if (productsCache.TryGetValue(id, out old))
        {
            if (productsCache.TryUpdate(id, product, old))
            {
                return product;
            }
        }
        return null;
    }

    public async Task<Products> CreateAsync(Products product)
    {
        EntityEntry<Products> added = await db.Products.AddAsync(product);
        int affectedRows = await db.SaveChangesAsync();
        if(affectedRows > 0)
        {
            return productsCache.AddOrUpdate(product.Id, product, UpdateCache);
        }
        return null;
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        Products? p = db.Products.Find(id);
        if(p != null)
        {
            db.Products.Remove(p);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                return productsCache.TryRemove(p.Id, out p);
            }
        }
        return null;
    }

    public Task<IEnumerable<Products>> GetAllAsync()
    {
        return Task.Run<IEnumerable<Products>>(() => productsCache.Values);
    }

    public Task<Products?> GetAsync(int id)
    {
        return Task.Run(() =>
        {
            productsCache.TryGetValue(id, out Products? p);
            return p;
        });
    }

    public async Task<Products> UpdateAsync(int id, Products product)
    {
        var old = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
        old.SerialNumber = product.SerialNumber;
        old.Name = product.Name;
        old.Price = product.Price;
        old.Number = product.Number;
        old.SupplierId = product.SupplierId;
        old.Supplier = product.Supplier;
        
        int affected = await db.SaveChangesAsync();
        if (affected == 1)
        {
            return UpdateCache(id, product);
        }
        return null;
    }
}