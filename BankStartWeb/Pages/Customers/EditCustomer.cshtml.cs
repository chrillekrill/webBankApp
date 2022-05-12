using BankAppWeb.Services;
using BankStartWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;
using System.ComponentModel.DataAnnotations;

namespace BankAppWeb.Pages.Customers
{
    public class EditCustomerModel : PageModel
    {
        private readonly ApplicationDbContext context;
        private readonly IToastNotification toastNotification;
        private readonly ICustomerService customerService;

        public EditCustomerModel(ApplicationDbContext context, IToastNotification toastNotification, ICustomerService customerService)
        {
            this.context = context;
            this.toastNotification = toastNotification;
            this.customerService = customerService;
        }
        public List<SelectListItem> AllCountries { get; set; }
        [BindProperty]
        public int Id { get; set; }
        [BindProperty]
        [MaxLength(50)] public string Givenname { get; set; }
        [BindProperty]
        [MaxLength(50)] public string Surname { get; set; }
        [BindProperty]
        [MaxLength(50)] public string Streetaddress { get; set; }
        [BindProperty]
        [MaxLength(50)] public string City { get; set; }
        [BindProperty]
        [MaxLength(10)] public string Zipcode { get; set; }
        [BindProperty]
        [MaxLength(30)] public string Country { get; set; }
        [MaxLength(2)] public string CountryCode { get; set; }
        [BindProperty]
        [MaxLength(20)] public string NationalId { get; set; }
        [Range(0, 9999)]
        public int TelephoneCountryCode { get; set; }
        [BindProperty]
        public string Telephone { get; set; }
        [BindProperty]
        [MaxLength(50)]
        public string EmailAddress { get; set; }
        [BindProperty]
        public DateTime Birthday { get; set; }

        private void SetInformation(int id)
        {
            customerService.SetAllCountries();

            AllCountries = customerService.AllCountries;

            var customer = context.Customers.FirstOrDefault(c => c.Id == id);

            Id = customer.Id;
            Givenname = customer.Givenname;
            Surname = customer.Surname;
            Streetaddress = customer.Streetaddress;
            City = customer.City;
            Zipcode = customer.Zipcode;
            Country = customer.Country;
            CountryCode = customer.CountryCode;
            NationalId = customer.NationalId;
            TelephoneCountryCode = customer.TelephoneCountryCode;
            Telephone = customer.Telephone;
            EmailAddress = customer.EmailAddress;
            Birthday = customer.Birthday;
        }
        public void OnGet(int id)
        {
            SetInformation(id);
        }

        public IActionResult OnPost()
        {
            if(ModelState.IsValid)
            {
                string CountryCode = this.CountryCode;
                int TelephoneCountryCode = this.TelephoneCountryCode;

                if (Country == "Sverige")
                {
                    CountryCode = "SE";
                    TelephoneCountryCode = 46;

                }
                else if (Country == "Norge")
                {
                    CountryCode = "NO";
                    TelephoneCountryCode = 47;
                }
                else
                {
                    CountryCode = "FI";
                    TelephoneCountryCode = 48;
                }

                var customer = context.Customers.FirstOrDefault(c => c.Id == Id);

                customer.Givenname = Givenname;
                customer.Surname = Surname;
                customer.Streetaddress = Streetaddress;
                customer.City = City;
                customer.Zipcode = Zipcode;
                customer.Country = Country;
                customer.CountryCode = CountryCode;
                customer.NationalId = NationalId;
                customer.TelephoneCountryCode = TelephoneCountryCode;
                customer.Telephone = Telephone;
                customer.EmailAddress = EmailAddress;
                customer.Birthday = Birthday;

                context.SaveChanges();

                toastNotification.AddSuccessToastMessage("Customer edited OK");

                return RedirectToPage("Index");
            }
            SetInformation(Id);

            toastNotification.AddErrorToastMessage("Something went wrong");

            return Page();
        }
    }
}
