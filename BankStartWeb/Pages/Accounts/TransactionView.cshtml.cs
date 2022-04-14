using BankStartWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankAppWeb.Pages.Accounts
{
    public class TransactionViewModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public int id { get; set; }
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
            _context = context;
        }
        public void OnGet(int id)
        {
            var account = _context.Accounts.Include(a => a.Transactions).First(c => c.Id == id);

            Transactions = account.Transactions.Select(t => new Transaction
            {
                Id = t.Id,
                Type = t.Type,
                Operation = t.Operation,
                Date = t.Date,
                Amount = t.Amount,
                NewBalance = t.NewBalance
            }).ToList();
        }
    }
}
