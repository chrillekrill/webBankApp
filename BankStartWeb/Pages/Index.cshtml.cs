using BankAppWeb.Services;
using BankStartWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankStartWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext context;
        private readonly IAccountService accountService;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context, IAccountService accountService)
        {
            _logger = logger;
            this.context = context;
            this.accountService = accountService;
        }
        public List<decimal> Accounts { get; set; }
        public List<CustomerViewModel> Customers { get; set; }
        public decimal TotalBalance { get; set; }
        public class CustomerViewModel
        {
            public int Id { get; set; }
        }
        
        public void OnGet()
        {
            
            var acc = context.Accounts;
            var cus = context.Customers;

            Accounts = acc.Select(x => x.Balance).ToList();

            TotalBalance = accountService.TotalBalance(Accounts);

            Customers = cus.Select(c => new CustomerViewModel
            {
                Id = c.Id
            }).ToList();

            
        }
    }
}