using BankStartWeb.Data;

namespace BankAppWeb.Services
{
    public class AccountService : IAccountService
    {
        public decimal TotalBalance(List<decimal> accounts)
        {
            decimal balance = 0;

            foreach(var amount in accounts)
            {
                balance += amount;
            }

            return balance;
        }
    }
}
