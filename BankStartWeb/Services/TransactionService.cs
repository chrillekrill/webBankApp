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
        public ITransactionService.TransactionStatus Transfer(string sender, string receiver, decimal amount, string type, string operation)
        {
            var sendingAccount = context.Accounts.First(a => a.Id.ToString() == sender);

            var check = CheckTransactionStatus(sendingAccount, amount, receiver, operation);

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
            context.SaveChanges();

            CreateTransferTransaction(sender, receiver, amount, type, operation);

            return ITransactionService.TransactionStatus.Ok;
        }

        public ITransactionService.TransactionStatus Deposit(string account, decimal amount)
        {
            var depositAccount = context.Accounts.First(a => a.Id.ToString() == account);

            var check = CheckDepositOrWithdrawStatus(depositAccount, amount, "Deposit cash");

            if(check == ITransactionService.TransactionStatus.AmountIsNegativeOrZero)
            {
                return ITransactionService.TransactionStatus.AmountIsNegativeOrZero;
            }

            depositAccount.Balance += amount;

            context.SaveChanges();

            CreateDepositOrWithdrawalTransaction(account, amount, "Debit", "Deposit cash");

            return ITransactionService.TransactionStatus.Ok;
        }

        public ITransactionService.TransactionStatus Withdraw(string account, decimal amount, string operation)
        {
            var withdrawalAccount = context.Accounts.First(a => a.Id.ToString() == account);

            var check = CheckDepositOrWithdrawStatus(withdrawalAccount, amount, operation);

            if (check == ITransactionService.TransactionStatus.BalanceTooLow)
            {
                return ITransactionService.TransactionStatus.BalanceTooLow;
            }
            else if (check == ITransactionService.TransactionStatus.AmountIsNegativeOrZero)
            {
                return ITransactionService.TransactionStatus.AmountIsNegativeOrZero;
            }

            withdrawalAccount.Balance -= amount;

            context.SaveChanges();

            CreateDepositOrWithdrawalTransaction(account, amount, "Credit", operation);

            return ITransactionService.TransactionStatus.Ok;
        }

        public bool IsValidAmount(decimal amount)
        {
            if (amount <= 0)
            {
                return false;
            }

            return true;
        }

        private void CreateTransferTransaction(string sender, string receiver, decimal amount, string type, string operation)
        {
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
        }
        private void CreateDepositOrWithdrawalTransaction(string account, decimal amount, string type, string operation)
        {
            var transaction = new Transaction
            {
                Type = type,
                Operation = operation,
                Date = DateTime.Now,
                Amount = amount,
                NewBalance = context.Accounts.First(a => a.Id.ToString() == account).Balance
            };

            context.Accounts.First(a => a.Id.ToString() == account).Transactions.Add(transaction);

            context.SaveChanges();
        }

        private ITransactionService.TransactionStatus CheckTransactionStatus(Account acc, decimal amount, string receiver, string operation)
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

        private ITransactionService.TransactionStatus CheckDepositOrWithdrawStatus(Account acc, decimal amount, string operation)
        {
            if(amount <= 0)
            {
                return ITransactionService.TransactionStatus.AmountIsNegativeOrZero;
            } else if(operation == "ATM withdrawal" && acc.Balance < amount)
            {
                return ITransactionService.TransactionStatus.BalanceTooLow;
            } else if(operation == "Close account")
            {
                return ITransactionService.TransactionStatus.Ok;
            }
            return ITransactionService.TransactionStatus.Ok;
        }
    }
}
