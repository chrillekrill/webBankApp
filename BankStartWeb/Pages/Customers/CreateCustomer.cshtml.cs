using BankStartWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BankAppWeb.Pages.Customers
{
    [Authorize(Roles = "Admin")]
    [BindProperties]
    public class CreateCustomerModel : PageModel
    {
        private readonly ApplicationDbContext context;
        [Required]
        [MaxLength(50)]public string Givenname { get; set; }
        [Required]
        [MaxLength(50)] public string Surname { get; set; }
        [Required]
        [MaxLength(50)] public string Streetaddress { get; set; }
        [Required]
        [MaxLength(50)] public string City { get; set; }
        [Required]
        [MaxLength(10)] public string Zipcode { get; set; }
        [Required]
        [MaxLength(30)] public string Country { get; set; }
        [Required]
        [MaxLength(20)] public string NationalId { get; set; }
        [Required]
        public string Telephone { get; set; }
        [Required]
        [MaxLength(50)]
        public string EmailAddress { get; set; }
        [Required]
        public DateTime Birthday { get; set; }

        public List<SelectListItem> AllCountries { get; set; }
        public CreateCustomerModel(ApplicationDbContext context)
        {
            this.context = context;
        }
        public void OnGet()
        {
            SetAllCountries();
        }

        private void SetAllCountries()
        {
            List<string> c = new List<string>()
            {
                "Sverige",
                "Norge",
                "Finland"
            };

            AllCountries = c.Select(c => new SelectListItem
            {
                Text = c,
                Value = c
            }).ToList();

            AllCountries.Insert(0, new SelectListItem
            {
                Text = "Please select a country",
                Value = ""
            });
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                string CountryCode;
                int TelephoneCountryCode;

                if (Country == "Sverige")
                {
                    CountryCode = "SE";
                    TelephoneCountryCode = 46;

                } else if (Country == "Norge")
                {
                    CountryCode = "NO";
                    TelephoneCountryCode = 47;
                } else
                {
                    CountryCode = "FI";
                    TelephoneCountryCode = 48;
                }

                var customer = new Customer
                {
                Givenname = this.Givenname,
                Surname = this.Surname,
                Streetaddress = this.Streetaddress,
                City = this.City,
                Zipcode = this.Zipcode,
                Country = this.Country,
                CountryCode = CountryCode,
                NationalId = this.NationalId,
                TelephoneCountryCode = TelephoneCountryCode,
                Telephone = this.Telephone,
                EmailAddress = this.EmailAddress,
                Birthday = this.Birthday
                };

                context.Customers.Add(customer);

                context.SaveChanges();

                var cus = context.Customers.First(x => x.Id == customer.Id);

                return RedirectToPage("CustomerView", new { id = cus.Id});
            }

            SetAllCountries();

            return Page();
        }
    }
}
