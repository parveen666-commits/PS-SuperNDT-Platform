using System;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportCustomerService
{
    public ReportCustomerModel Create(
        Guid reportId,
        string customerName,
        string projectName,
        string siteLocation,
        string contactPerson,
        string contactNumber,
        string email,
        string purchaseOrderNumber)
    {
        return new ReportCustomerModel
        {
            ReportId = reportId,
            CustomerName = customerName,
            ProjectName = projectName,
            SiteLocation = siteLocation,
            ContactPerson = contactPerson,
            ContactNumber = contactNumber,
            Email = email,
            PurchaseOrderNumber = purchaseOrderNumber
        };
    }

    public bool Validate(
        ReportCustomerModel customer)
    {
        if (customer == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(customer.CustomerName))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(customer.ProjectName))
        {
            return false;
        }

        return true;
    }
}