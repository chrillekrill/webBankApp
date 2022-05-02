using BankAppWeb.Infrastructure.Paging;
using BankStartWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankAppWeb.Pages.Accounts
{
    [Authorize]
    public class TransactionViewModel : PageModel
    {
        private readonly ApplicationDbContext context;
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public List<Transaction> Transactions { get; set; }
        public class Transaction
        {
            public int Id { get; set; }
            public string Type { get; set; }
            public string Operation { get; set; }
            public DateTime Date { get; set; }
            public decimal Amount { get; set; }
            public decimal NewBalance { get; set; }
        }
        public TransactionViewModel(ApplicationDbContext context)
        {
            this.context = context;
        }
        public void OnGet(int accountId, int customerId)
        {
            CustomerId = customerId;
            Id = accountId;
        }

        public IActionResult OnGetFetchMore(int accountId, int pageNo)
        {
            var query = context.Accounts.Where(a => a.Id == accountId)
                .SelectMany(e => e.Transactions)
                .OrderBy(a => a.Amount);

            var r = query.GetPaged(pageNo, 5);

            var list = r.Results.Select(t => new Transaction
            {
                Id = t.Id,
                Type = t.Type,
                Operation = t.Operation,
                Date = t.Date,
                Amount = t.Amount,
                NewBalance = t.NewBalance
            }).ToList();

            bool lastPage = pageNo == r.PageCount;

            return new JsonResult(new {items = list, lastPage  = lastPage });
        }
    }
}
