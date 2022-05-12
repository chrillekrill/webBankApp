using Microsoft.AspNetCore.Mvc.Rendering;

namespace BankAppWeb.Services
{
    public class CustomerService : ICustomerService
    {
        public List<SelectListItem> AllCountries { get; set; }
        public void SetAllCountries()
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
    }
}
