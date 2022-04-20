using BankStartWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BankAppWeb.Pages.Accounts
{
    public class SendTransactionViewModel : PageModel
    {
        private readonly ApplicationDbContext context;

        public SendTransactionViewModel(ApplicationDbContext context)
        {
            this.context = context;
        }
        [Required]
        public decimal amount { get; set; }
        [Required]
        public string account { get; set; }
        public int id { get; set; }
        public List<Account> accounts { get; set; }
        public List<SelectListItem> AllAccounts { get; set; }
        private void SetAllAccounts()
        {
            AllAccounts = accounts.Select(account => new SelectListItem
            {
                Text = account.id.ToString() + " " + account.balance + " sek",
                Value = account.id.ToString()
            }).ToList();
            AllAccounts.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "Please select an account"
            });
        }


        public class Account
        {
            public int id { get; set; }
            public decimal balance { get; set; }
        }

        public void OnGet(int customerId)
        {
            id = customerId;

            var findCustomer = context.Customers.Include(c => c.Accounts).First(customer => customer.Id == customerId);

            accounts = findCustomer.Accounts.Select(a => new Account
            {
                id = a.Id,
                balance = a.Balance
            }).ToList();

            SetAllAccounts();
        }
    }
}
