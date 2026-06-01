using BusinessObjects.Models;

namespace Repositories
{
    public interface IAccountRepository
    {
        AccountMember GetAccountById(string accountId);
    }
}
