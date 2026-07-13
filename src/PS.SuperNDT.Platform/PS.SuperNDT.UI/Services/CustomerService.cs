using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Database;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class CustomerService
{
    public void Save(CustomerModel customer)
    {
        ArgumentNullException.ThrowIfNull(customer);

        using var db = new SuperNDTDbContext();

        var existing = db.Customers
            .FirstOrDefault(x => x.Id == customer.Id);

        if (existing == null)
        {
            db.Customers.Add(customer);
        }
        else
        {
            db.Entry(existing)
              .CurrentValues
              .SetValues(customer);
        }

        db.SaveChanges();
    }

    public CustomerModel? Get(Guid id)
    {
        using var db = new SuperNDTDbContext();

        return db.Customers
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
    }

    public List<CustomerModel> GetAll()
    {
        using var db = new SuperNDTDbContext();

        return db.Customers
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToList();
    }

    public List<CustomerModel> Search(string text)
    {
        text ??= string.Empty;

        using var db = new SuperNDTDbContext();

        return db.Customers
            .AsNoTracking()
            .Where(x =>
                x.Name.Contains(text) ||
                x.CustomerCode.Contains(text) ||
                x.ContactPerson.Contains(text) ||
                x.Mobile.Contains(text))
            .OrderBy(x => x.Name)
            .ToList();
    }

    public void Delete(Guid id)
    {
        using var db = new SuperNDTDbContext();

        var customer = db.Customers
            .FirstOrDefault(x => x.Id == id);

        if (customer == null)
            return;

        db.Customers.Remove(customer);

        db.SaveChanges();
    }
}