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
        public List<CustomerViewModel> Customers { get; set; }
        public class CustomerViewModel
        {
            public int Id { get; set; }
            public string Surname { get; set; }
            public string City { get; set; }
            public List<Account>? Accounts { get; set; }
        }
        public void OnGet()
        {
            var cus = context.Customers;

            Customers = cus.Select(c => new CustomerViewModel
            {
                Id = c.Id,
                Surname = c.Surname,
                City = c.City,
                Accounts = c.Accounts
            }).ToList();
        }
    }
}