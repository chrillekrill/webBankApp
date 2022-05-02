namespace BankAppWeb.Services
{
    public interface ITransactionService
    {
        TransactionStatus Transfer(string sender, string receiver, decimal amount, string type, string operation);

        TransactionStatus Deposit(string account, decimal amount);

        TransactionStatus Withdraw(string account, decimal amount, string operation);

        bool IsValidAmount(decimal amount);
        public enum TransactionStatus
        {
            Ok,
            BalanceTooLow,
            AmountIsNegativeOrZero,
            SameAccount
        }
    }

}
