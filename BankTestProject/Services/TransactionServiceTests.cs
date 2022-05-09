using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using BankAppWeb.Services;
using BankStartWeb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BankTestProject.Services
{
    [TestClass]
    public class TransactionServiceTests
    {
        private readonly TransactionService _sut;
        private readonly ApplicationDbContext _context;

        public TransactionServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .Options;
            _context = new ApplicationDbContext(options);
            _sut = new TransactionService(_context);
        }

        public void CreateCustomer(string name, int number)
        {
            _context.Customers.Add(new Customer
            {
                Surname = name + name,
                Givenname = name,
                Streetaddress = name + "gatan " + number,
                City = name + "köping",
                Zipcode = (number * 100).ToString(),
                Country = name + "republiken",
                CountryCode = number.ToString(),
                NationalId = (00000000 + number).ToString(),
                TelephoneCountryCode = number,
                Telephone = (number + 35).ToString(),
                EmailAddress = name + number.ToString() + "@mail.com",
                Birthday = DateTime.Today,
                Accounts = new List<Account>()

            });
            _context.SaveChanges();
        }
        public void CreateAccount(decimal balance, string name)
        {
            CreateCustomer(name, 500);

            _context.Customers.First().Accounts.Add(new Account
            {
                AccountType = "Savings",
                Created = DateTime.Now,
                Balance = balance,
                Transactions = new List<Transaction>() { }
            });

            _context.SaveChanges();
        }

        [TestMethod]
        public void When_depositing_negative_amount_return_AmountIsNegativeOrZero()
        {
            _context.Database.EnsureDeleted();

            CreateAccount(0, "test");

            var testCustomerAccount = _context.Accounts.First();

            var result =  _sut.Deposit(testCustomerAccount.Id.ToString(), -500);

            Assert.AreEqual(result, ITransactionService.TransactionStatus.AmountIsNegativeOrZero);
        }

        [TestMethod]
        public void When_withdrawing_negative_amount_return_AmountIsNegativeOrZero()
        {
            _context.Database.EnsureDeleted();

            CreateAccount(0, "test");

            var testCustomerAccount = _context.Accounts.First();

            var result = _sut.Withdraw(testCustomerAccount.Id.ToString(), -500, "ATM withdrawal");

            Assert.AreEqual(result, ITransactionService.TransactionStatus.AmountIsNegativeOrZero);
        }

        [TestMethod]
        public void When_depositing_positive_amount_return_Ok()
        {
            _context.Database.EnsureDeleted();

            CreateAccount(0, "test");

            var testCustomerAccount = _context.Accounts.First();

            var result = _sut.Deposit(testCustomerAccount.Id.ToString(), 500);

            Assert.AreEqual(result, ITransactionService.TransactionStatus.Ok);
        }

        [TestMethod]
        public void When_withdrawing_positive_amount_return_Ok()
        {
            _context.Database.EnsureDeleted();

            CreateAccount(500, "test");

            var testCustomerAccount = _context.Accounts.First();

            var result = _sut.Withdraw(testCustomerAccount.Id.ToString(), 500, "ATM withdrawal");

            Assert.AreEqual(result, ITransactionService.TransactionStatus.Ok);
        }
        [TestMethod]
        public void When_transfering_negative_amount_return_AmountIsNegativeOrZero()
        {
            _context.Database.EnsureDeleted();

            CreateAccount(0, "test");
            CreateAccount(0, "test2");

            var testCustomerAccount = _context.Accounts.First(a => a.Id == 1);
            var test2CustomerAccount = _context.Accounts.First(a => a.Id == 2);

            var result = _sut.Transfer(testCustomerAccount.Id.ToString(), test2CustomerAccount.Id.ToString(), -500, "Debit", "Transfer");

            Assert.AreEqual(result, ITransactionService.TransactionStatus.AmountIsNegativeOrZero);
        }

        [TestMethod]
        public void When_transfering_postive_amount_return_Ok()
        {
            _context.Database.EnsureDeleted();

            CreateAccount(500, "test");
            CreateAccount(0, "test2");

            var testCustomerAccount = _context.Accounts.First(a => a.Id == 1);
            var test2CustomerAccount = _context.Accounts.First(a => a.Id == 2);

            var result = _sut.Transfer(testCustomerAccount.Id.ToString(), test2CustomerAccount.Id.ToString(), 500, "Debit", "Transfer");

            Assert.AreEqual(result, ITransactionService.TransactionStatus.Ok);
        }

        [TestMethod]
        public void When_transfering_amount_that_is_lower_than_account_balance_return_BalanceTooLow()
        {
            _context.Database.EnsureDeleted();

            CreateAccount(500, "test");
            CreateAccount(0, "test2");

            var testCustomerAccount = _context.Accounts.First(a => a.Id == 1);
            var test2CustomerAccount = _context.Accounts.First(a => a.Id == 2);

            var result = _sut.Transfer(testCustomerAccount.Id.ToString(), test2CustomerAccount.Id.ToString(), 1000, "Debit", "Transfer");

            Assert.AreEqual(result, ITransactionService.TransactionStatus.BalanceTooLow);
        }
    }
}
