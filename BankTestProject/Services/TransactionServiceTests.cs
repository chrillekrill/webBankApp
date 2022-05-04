using System;
using System.Collections.Generic;
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
    }
}
