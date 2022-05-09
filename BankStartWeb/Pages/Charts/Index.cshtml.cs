using BankAppWeb.Services;
using BankStartWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankAppWeb.Pages.Charts
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext context;
        private readonly IAccountService accountService;

        public IndexModel(ApplicationDbContext context, IAccountService accountService)
        {
            this.context = context;
            this.accountService = accountService;
        }

        public List<decimal> AccountsBalance { get; set; }
        public decimal TotalBalance { get; set; }
        public List<int> TotalCustomers { get; set; }
        public List<int> BarChart { get; set; } = new List<int>();
        public List<int> ChartBalance { get; set; } = new List<int>();

        private void SetAreaChartInfo()
        {
            var random = new Random();

            var randomMinus = 0;

            decimal totalBalanceTimes = 0.01M;

            for (int i = 0; i <= 12; i++)
            {
                var toAdd = (int)(TotalBalance * totalBalanceTimes) - random.Next(0, randomMinus);

                ChartBalance.Add(toAdd);

                totalBalanceTimes += 0.01M;

                randomMinus += 10000;
            }
        }

        private void SetBarChartInfo()
        {
            var random = new Random();

            var toCheck = 0;

            for(int i = 0; i <= 12; i++)
            {
                BarChart.Add(TotalCustomers[i+toCheck] += random.Next(0, 500));

                toCheck++;
            }
        }

        public void OnGet()
        {
            var acc = context.Accounts;

            var customers = context.Customers;

            AccountsBalance = acc.Select(x => x.Balance).ToList();

            TotalCustomers = acc.Select(x => x.Id).ToList();

            TotalBalance = accountService.TotalBalance(AccountsBalance);

            SetAreaChartInfo();

            SetBarChartInfo();

        }
    }   
}
