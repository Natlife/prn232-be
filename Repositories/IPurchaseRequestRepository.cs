using System.Collections.Generic;
using BusinessObjects.Common;
using BusinessObjects.Models;

namespace Repositories;

public interface IPurchaseRequestRepository
{
    DepositResult CreateDeposit(DepositRequest request);
    DepositResult CreateBuyout(DepositRequest request);
    IEnumerable<PurchaseRequest> GetDepositsByCustomer(int customerId);
    IEnumerable<PurchaseRequest> GetAllPurchaseRequests();
}
