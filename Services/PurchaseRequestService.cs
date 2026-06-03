using BusinessObjects.Common;
using BusinessObjects.Models;
using Repositories;
using System.Collections.Generic;

namespace Services;

public class PurchaseRequestService : IPurchaseRequestService
{
    private readonly IPurchaseRequestRepository _repo;

    public PurchaseRequestService(IPurchaseRequestRepository repo)
    {
        _repo = repo;
    }

    public DepositResult CreateDeposit(DepositRequest request)
    {
        return _repo.CreateDeposit(request);
    }

    public DepositResult CreateBuyout(DepositRequest request)
    {
        return _repo.CreateBuyout(request);
    }

    public IEnumerable<PurchaseRequest> GetDepositsByCustomer(int customerId)
    {
        return _repo.GetDepositsByCustomer(customerId);
    }

    public IEnumerable<PurchaseRequest> GetAllPurchaseRequests()
    {
        return _repo.GetAllPurchaseRequests();
    }
}
