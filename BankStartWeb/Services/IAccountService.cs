using BankStartWeb.Data;

namespace BankAppWeb.Services
{
    public interface IAccountService
    {
        public decimal TotalBalance(List<decimal> accounts);
    }
}
