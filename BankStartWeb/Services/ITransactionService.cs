namespace BankAppWeb.Services
{
    public interface ITransactionService
    {
        TransactionStatus createTransaction(string sender, string receiver, decimal amount, string type, string operation);

        public enum TransactionStatus
        {
            Ok,
            BalanceTooLow,
            AmountIsNegativeOrZero,
            SameAccount
        }
    }

}
