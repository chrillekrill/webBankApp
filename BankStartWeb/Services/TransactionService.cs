using BankStartWeb.Data;

namespace BankAppWeb.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext context;

        public TransactionService(ApplicationDbContext context)
        {
            this.context = context;
        }
        public ITransactionService.TransactionStatus createTransaction(string sender, string receiver, decimal amount, string type, string operation)
        {
            var sendingAccount = context.Accounts.First(a => a.Id.ToString() == sender);

            var check = CheckTransactionStatus(sendingAccount, amount, receiver);

            if(check == ITransactionService.TransactionStatus.BalanceTooLow)
            {
                return ITransactionService.TransactionStatus.BalanceTooLow;
            } else if(check == ITransactionService.TransactionStatus.AmountIsNegativeOrZero)
            {
                return ITransactionService.TransactionStatus.AmountIsNegativeOrZero;
            } else if(check == ITransactionService.TransactionStatus.SameAccount)
            {
                return ITransactionService.TransactionStatus.SameAccount;
            }
                
            sendingAccount.Balance -= amount;
            context.Accounts.First(a => a.Id.ToString() == receiver).Balance += amount;

            var SenderTransaction = new Transaction
            {
                Type = type,
                Operation = operation,
                Date = DateTime.Now,
                Amount = amount,
                NewBalance = context.Accounts.First(a => a.Id.ToString() == sender).Balance
            };

            var ReceiverTransaction = new Transaction
            {
                Type = type,
                Operation = operation,
                Date = DateTime.Now,
                Amount = amount,
                NewBalance = context.Accounts.First(a => a.Id.ToString() == receiver).Balance
            };

            context.Accounts.First(a => a.Id.ToString() == sender).Transactions.Add(SenderTransaction);

            context.Accounts.First(a => a.Id.ToString() == receiver).Transactions.Add(ReceiverTransaction);

            context.SaveChanges();

            return ITransactionService.TransactionStatus.Ok;
        }

        private ITransactionService.TransactionStatus CheckTransactionStatus(Account acc, decimal amount, string receiver)
        {
            if (acc.Balance < amount)
            {
                return ITransactionService.TransactionStatus.BalanceTooLow;
            } else if(amount <= 0)
            {
                return ITransactionService.TransactionStatus.AmountIsNegativeOrZero;
            } else if(acc.Id.ToString() == receiver)
            {
                return ITransactionService.TransactionStatus.SameAccount;
            }
            return ITransactionService.TransactionStatus.Ok;
        }
    }
}
