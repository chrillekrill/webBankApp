using BankStartWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;
using System.ComponentModel.DataAnnotations;

namespace BankAppWeb.Pages.Accounts
{
    [Authorize(Roles = "Admin")]
    public class CreateAccountModel : PageModel
    {
        private readonly ApplicationDbContext context;
        private readonly IToastNotification toastNotification;

        [BindProperty]
        public int Id { get; set; }
        public List<SelectListItem> AllAccountTypes { get; set; }
        [BindProperty]
        [Required]
        [MaxLength(10)]
        public string AccountType { get; set; }
        public DateTime Created { get; set; }
        public CreateAccountModel(ApplicationDbContext context, IToastNotification toastNotification)
        {
            this.context = context;
            this.toastNotification = toastNotification;
        }
        private void setAllTypes()
        {
            List<string> types = new List<string>() { "Personal", "Checking", "Savings" };

            AllAccountTypes = types.Select(x => new SelectListItem
            {
                Text = x,
                Value = x
            }).ToList();

            AllAccountTypes.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "Please select an account type"
            });
        }
        public void OnGet(int customerId)
        {
            Id = customerId;

            setAllTypes();
        }

        public IActionResult OnPost(int id)
        {
            if(ModelState.IsValid)
            {
                Created = DateTime.Now;

                var account = new Account()
                {
                    AccountType = AccountType,
                    Created = Created,
                    Balance = 0,
                    Transactions = new List<Transaction>() { }
                };

                context.Customers.First(x => x.Id == id).Accounts.Add(account);

                context.SaveChanges();

                toastNotification.AddSuccessToastMessage("Account created");

                return RedirectToPage("../Customers/CustomerView", new { id });
            }

            Id = id;

            toastNotification.AddErrorToastMessage("Something went wrong");

            return Page();
        }
    }
}
