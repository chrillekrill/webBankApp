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
        public List<int> Customers { get; set; }
        public List <int> Transactions { get; set; }
        public decimal TotalBalance { get; set; }
        public void OnGet()
        {
            
            var acc = context.Accounts;
            var cus = context.Customers;
            var tran = context.Transactions;

            Accounts = acc.Select(x => x.Balance).ToList();

            Transactions = tran.Select(x => x.Id).ToList();

            TotalBalance = accountService.TotalBalance(Accounts);

            Customers = cus.Select(c => c.Id).ToList();

            
        }
    }
}