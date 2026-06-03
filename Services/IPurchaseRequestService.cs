using BusinessObjects.Common;
using BusinessObjects.Models;
using System.Collections.Generic;

namespace Services;

public interface IPurchaseRequestService
{
    DepositResult CreateDeposit(DepositRequest request);
    IEnumerable<PurchaseRequest> GetDepositsByCustomer(int customerId);
}
