using BankAppWeb.Services;
using BankStartWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using System.ComponentModel.DataAnnotations;

namespace BankAppWeb.Pages.Accounts
{
    [Authorize(Roles = "Admin,Cashier")]
    public class DepositAndWithdrawalViewModel : PageModel
    {
        private readonly ApplicationDbContext context;
        private readonly IToastNotification toastNotification;
        private readonly ITransactionService transactionService;

        public DepositAndWithdrawalViewModel(ApplicationDbContext context,
            IToastNotification toastNotification,
            ITransactionService transactionService)
        {
            this.context = context;
            this.toastNotification = toastNotification;
            this.transactionService = transactionService;
        }
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public string Operation { get; set; }
        [Required] 
        [BindProperty]
        public decimal Amount { get; set; }
        public void OnGet(int accountId, int customerId, string operation)
        {
            AccountId = accountId;
            CustomerId = customerId;
            Operation = operation;
        }
        private void SetInformation(int accountId, int customerId, string operation)
        {
            CustomerId = customerId;
            AccountId = accountId;
            Operation = operation;
        }
        public IActionResult OnPost(int accountId, int customerId, string operation)
        {
            if (!transactionService.IsValidAmount(Amount))
            {
                ModelState.AddModelError(nameof(Amount), "Please select an amount that is more than 0.");
            }
            if (ModelState.IsValid)
            {

                ITransactionService.TransactionStatus transaction;

                if (operation == "ATM withdrawal")
                {
                    transaction = transactionService.Withdraw(accountId.ToString(), Amount, operation);
                } else
                {
                    transaction = transactionService.Deposit(accountId.ToString(), Amount);
                }
                if(transaction == ITransactionService.TransactionStatus.BalanceTooLow)
                {
                    ModelState.AddModelError(nameof(Amount), "Please withdraw an amount that is equal or less than the balance of the account");
                    SetInformation(accountId, customerId, operation);
                }
                else if(transaction == ITransactionService.TransactionStatus.Ok)
                {
                    toastNotification.AddSuccessToastMessage("Transaction complete");
                    return RedirectToPage("../Customers/CustomerView", new { id = customerId });
                }
            }
            SetInformation(accountId, customerId, operation);

            return Page();
        }
    }
}
