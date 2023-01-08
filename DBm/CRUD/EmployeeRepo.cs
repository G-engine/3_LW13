using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBm;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace WebApp.CRUD;

public class EmployeeRepo
{
    private static ConcurrentDictionary<int, Employees> customersCache;
    private PharmacyContext db;

    public EmployeeRepo(PharmacyContext db)
    {
        this.db = db;
        customersCache = new ConcurrentDictionary<int, Employees>(db.Employees.ToDictionary(e=>e.Id));
    }
    private Employees UpdateCache(int id, Employees employee)
    {
        Employees old;
        if (customersCache.TryGetValue(id, out old))
        {
            if (customersCache.TryUpdate(id, employee, old))
            {
                return employee;
            }
        }
        return null;
    }

    public async Task<Employees> CreateAsync(Employees employee)
    {
        EntityEntry<Employees> added = await db.Employees.AddAsync(employee);
        int affectedRows = await db.SaveChangesAsync();
        if(affectedRows > 0)
        {
            return customersCache.AddOrUpdate(employee.Id, employee, UpdateCache);
        }
        return null;
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        Employees? e = db.Employees.Find(id);
        if(e != null)
        {
            db.Employees.Remove(e);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                return customersCache.TryRemove(id, out e);
            }
        }
        return null;
    }

    public Task<IEnumerable<Employees>> GetAllAsync()
    {
        return Task.Run<IEnumerable<Employees>>(() => customersCache.Values);
    }

    public Task<Employees?> GetAsync(int id)
    {
        return Task.Run(() =>
        {
            customersCache.TryGetValue(id, out Employees? e);
            return e;
        });
    }

    public async Task<Employees> UpdateAsync(int id, Employees employee)
    {
        var old = await db.Employees.FirstOrDefaultAsync(e => e.Id == id);
        
        old.Name = employee.Name;
        old.Job = employee.Job;
        old.Salary = employee.Salary;
        old.Schedule = employee.Schedule;
        old.InVacation = employee.InVacation;
        int affected = await db.SaveChangesAsync();
        if (affected > 0)
        {
            return UpdateCache(id, employee);
        }
        return null;
    }
}