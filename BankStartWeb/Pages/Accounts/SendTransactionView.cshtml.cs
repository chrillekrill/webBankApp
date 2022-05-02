using BankAppWeb.Services;
using BankStartWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAppWeb.Pages.Accounts
{
    [Authorize]
    [BindProperties]
    public class SendTransactionViewModel : PageModel
    {
        private readonly ApplicationDbContext context;
        private readonly ITransactionService transactionService;

        public SendTransactionViewModel(ApplicationDbContext context, ITransactionService transactionService)
        {
            this.context = context;
            this.transactionService = transactionService;
        }
        [Required]
        [Column(TypeName="decmial(18,2)")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "Please choose an account")]
        public string SenderAccount { get; set; }
        [Required]
        public string ReceivingCustomer { get; set; }
        [Required]
        public string ReceivingAccount { get; set; }
        public int Id { get; set; }
        public List<Customer> Customers { get; set; }
        public List<Account> CustomerAccounts { get; set; }
        public List<Account> ReceiverAccounts { get; set; }
        public List<SelectListItem> AllCustomerAccounts { get; set; }
        public List<SelectListItem> AllReceiverCustomers { get; set; }
        public List<SelectListItem> AllReceiverAccounts { get; set; }
        private List<SelectListItem> SetAllAccounts(List<SelectListItem> acc, List<Account> allAcc)
        {
            acc = allAcc.Select(account => new SelectListItem
            {
                Text = account.id.ToString() + " " + account.balance + " sek",
                Value = account.id.ToString()
            }).ToList();
            acc.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "Please select an account"
            });

            return acc;
        }
        private void SetAllCustomers()
        {
            var excludeCustomerFromList = Customers.First(c => c.id == Id);

            Customers.Remove(excludeCustomerFromList);

            AllReceiverCustomers = Customers.Select(c => new SelectListItem
            {
                Text = c.Fullname,
                Value = c.id.ToString()
            }).ToList();
            AllReceiverCustomers.Insert(0, new SelectListItem
            {
                Value = Id.ToString(),
                Text = "Own account"
            });
        }

        public class Account
        {
            public int id { get; set; }
            public decimal balance { get; set; }
        }

        public class Customer
        {
            public int id { get; set; }
            public string Givenname { get; set; }
            public string Surname { get; set; }
            public string Fullname => $"{Givenname} {Surname}";
            public List<Account> customerAccounts { get; set; }
        }

        public void OnGet(int customerId )
        {
            Id = customerId;

            SetInformation();

        }

        public void SetInformation()
        {
            var findCustomer = context.Customers.Include(c => c.Accounts).First(customer => customer.Id == Id);

            CustomerAccounts = findCustomer.Accounts.Select(a => new Account
            {
                id = a.Id,
                balance = a.Balance
            }).ToList();

            Customers = context.Customers.Include(a => a.Accounts).OrderBy(a => a.Givenname).Select(c =>

            new Customer
            {
                id = c.Id,
                Givenname = c.Givenname,
                Surname = c.Surname,
                customerAccounts = c.Accounts.Select(a => new Account
                {
                    id = a.Id,
                    balance = a.Balance
                }).ToList()
            }).ToList();

            AllCustomerAccounts = SetAllAccounts(AllCustomerAccounts, CustomerAccounts);

            SetAllCustomers();
        }
        

        public IActionResult OnPost(int id)
        {
            Id = id;
            if(!transactionService.IsValidAmount(Amount))
            {
                ModelState.AddModelError(nameof(Amount), "Please choose an amount that is greater than 0.");
            } 
            if (ModelState.IsValid)
            {
                var transaction = transactionService.Transfer(SenderAccount, ReceivingAccount, Amount, "Debit", "Transfer");

                if(transaction == ITransactionService.TransactionStatus.Ok)
                {
                    return RedirectToPage("../Customers/CustomerView", new { id = id });
                } else if(transaction == ITransactionService.TransactionStatus.BalanceTooLow)
                {
                    ModelState.AddModelError(nameof(Amount), "Balance on account is lower than the amount trying to be sent.");

                    SetInformation();

                    return Page();
                } else if(transaction == ITransactionService.TransactionStatus.SameAccount)
                {
                    ModelState.AddModelError(nameof(ReceivingAccount), "You can't transfer money to the same account.");

                    SetInformation();

                    return Page();
                }
            }

            SetInformation();

            return Page();
        }

        public IActionResult OnGetFetchAccounts(int customerId)
        {
            var accounts = context.Customers.Where(c => c.Id == customerId)
                .SelectMany(e => e.Accounts)
                .OrderBy(e => e.Id);

            var list = accounts.Select(a => new Account
            {
                id = a.Id,
                balance = a.Balance
            }).ToList();

            ReceiverAccounts = list;

            AllReceiverAccounts = SetAllAccounts(AllReceiverAccounts, ReceiverAccounts);

            return new JsonResult(new { items = list });
        }
    }
}
