using BankStartWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankStartWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext context;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            this.context = context;
        }
        public List<AccountsViewModel> Accounts { get; set; }
        public List<CustomerViewModel> Customers { get; set; }
        public class CustomerViewModel
        {
            public int Id { get; set; }
        }
        public class AccountsViewModel
        {
            public int Id { get; set; }
            public decimal Balance { get; set; }
        }
        public decimal TotalBalance()
        {
            decimal total = 0;
            foreach (var account in Accounts)
            {
                total += account.Balance;
            }

            return total;
        }
        public int NumberOfAccounts()
        {
            int total = 0;
            foreach (var account in Accounts)
            {
                total += 1;
            }
            return total;
        }

        public int NumberOfCustomers()
        {
            int total = 0;
            foreach (var customer in Customers)
            {
                total += 1;
            }
            return total;
        }
        public void OnGet()
        {
            var acc = context.Accounts;
            var cus = context.Customers;

            Accounts = acc.Select(c => new AccountsViewModel
            {
                Id = c.Id,
                Balance = c.Balance
            }).ToList();

            Customers = cus.Select(c => new CustomerViewModel
            {
                Id = c.Id
            }).ToList();
        }
    }
}