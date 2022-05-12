using Microsoft.AspNetCore.Mvc.Rendering;

namespace BankAppWeb.Services
{
    public interface ICustomerService
    {
        List<SelectListItem> AllCountries { get; set; }
        void SetAllCountries();
    }
}
