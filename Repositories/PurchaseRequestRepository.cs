using BusinessObjects.Common;
using BusinessObjects.Models;
using DataAccessObjects;
using System.Collections.Generic;

namespace Repositories;

public class PurchaseRequestRepository : IPurchaseRequestRepository
{
    public DepositResult CreateDeposit(DepositRequest request)
    {
        return PurchaseRequestDAO.CreateDeposit(request);
    }

    public IEnumerable<PurchaseRequest> GetDepositsByCustomer(int customerId)
    {
        return PurchaseRequestDAO.GetDepositsByCustomer(customerId);
    }
}
