using BankAppWeb.Services;
using BankStartWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankAppWeb.Pages.Customers
{
    [Authorize]
    public class CustomerViewModel : PageModel
    {
        private readonly ApplicationDbContext context;
        private readonly IAccountService accountService;
        private readonly ITransactionService transactionService;

        public int Id { get; set; }
        public string Surname { get; set; }
        public string Givenname { get; set; }
        public string Fullname => $"{Givenname} {Surname}";
        public string City { get; set; }
        public string Zipcode { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Streetaddress { get; set; }
        public string NationalId { get; set; }
        public string Telephone { get; set; }
        public int TelephoneCountryCode { get; set; }
        public List<CustomerAccount> AccountNumbers { get; set; }
        public decimal TotalBalance { get; set; }
        public bool confirmDelete { get; set; }

        public CustomerViewModel(ApplicationDbContext context, IAccountService accountService, ITransactionService transactionService)
        {
            this.context = context;
            this.accountService = accountService;
            this.transactionService = transactionService;
        }

        public class CustomerAccount
        {
            public int Id { get; set; }
            public string Accounttype { get; set; }
            public decimal Balance { get; set; }
        }

        private void RemoveCustomer(int id)
        {
            var customer = context.Customers.First(x => x.Id == id);

            context.Customers.Remove(customer);

            context.SaveChanges();
        }

        private void RemoveAccount(int id)
        {
            var account = context.Accounts.First(x => x.Id == id);

            if(account.Balance >= 0)
            {
                transactionService.Withdraw(id.ToString(), account.Balance, "Close account");
            }

            foreach(var transaction in context.Transactions) {
                foreach (var item in account.Transactions)
                {
                    if(transaction.Id == item.Id)
                    {
                        context.Transactions.Remove(transaction);
                    } 
                }
            }

            context.SaveChanges();

            context.Accounts.Remove(account);

            context.SaveChanges();
        }

        public void SetInformation(int id)
        {
            var cus = context.Customers.Include(cus => cus.Accounts).First(c => c.Id == id);

            Id = cus.Id;
            Surname = cus.Surname;
            Givenname = cus.Givenname;
            City = cus.City;
            NationalId = cus.NationalId;
            Zipcode = cus.Zipcode;
            Country = cus.Country;
            CountryCode = cus.CountryCode;
            Streetaddress = cus.Streetaddress;
            Telephone = cus.Telephone;
            TelephoneCountryCode = cus.TelephoneCountryCode;
            AccountNumbers = cus.Accounts.Select(acc => new CustomerAccount
            {
                Id = acc.Id,
                Accounttype = acc.AccountType,
                Balance = acc.Balance
            }).ToList();
            TotalBalance = accountService.TotalBalance(AccountNumbers.Select(acc => acc.Balance).ToList());
        }

        public void OnGet(int id)
        {
            SetInformation(id);
        }

        public IActionResult OnPostRemoveCustomer(int id)
        {
            if (ModelState.IsValid)
            {
                RemoveCustomer(id);

                return RedirectToPage("/Index");
            }
            SetInformation(id);

            return Page();
        }
        public IActionResult OnPostRemoveAccount(int id, int customerId)
        {
            if (ModelState.IsValid)
            {
                RemoveAccount(id);

                return RedirectToPage("/Customers/CustomerView", new {id = customerId});
            }
            SetInformation(id);

            return Page();
        }
    }
}
