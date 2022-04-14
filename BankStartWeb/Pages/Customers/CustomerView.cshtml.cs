using BankAppWeb.Services;
using BankStartWeb.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace BankAppWeb.Pages.Customers
{
    public class CustomerViewModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountService _accountService;

        public int Id { get; set; }
        public string Surname { get; set; }
        public string Givenname { get; set; }
        public string Fullname => $"{Givenname} {Surname}";
        public string City { get; set; }
        public string NationalId { get; set; }
        public List<CustomerAccount> AccountNumbers { get; set; }
        public decimal TotalBalance { get; set; }

        public CustomerViewModel(ApplicationDbContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        public class CustomerAccount
        {
            public int Id { get; set; }
            public string Accounttype { get; set; }
            public decimal Balance { get; set; }
        }

        public void OnGet(int id)
        {
            var cus = _context.Customers.Include(cus => cus.Accounts).First(c => c.Id == id);

            Id = cus.Id;
            Surname = cus.Surname;
            Givenname = cus.Givenname;
            City = cus.City;
            NationalId = cus.NationalId;
            AccountNumbers = cus.Accounts.Select(acc => new CustomerAccount
            {
                Id = acc.Id,
                Accounttype = acc.AccountType,
                Balance = acc.Balance
            }).ToList();
            TotalBalance = _accountService.TotalBalance(AccountNumbers.Select(acc => acc.Balance).ToList());
        }
    }
}
