using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjects
{
    public class AccountDAO
    {
        public static AccountMember GetAccountById(string acountID)
        {
            using var db = new MyStoreContext();
            return db.AccountMembers.FirstOrDefault(c => c.MemberId.Equals(acountID));
        }
    }
}
