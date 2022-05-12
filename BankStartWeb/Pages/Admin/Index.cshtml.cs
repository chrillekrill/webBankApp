using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankAppWeb.Pages.Admin
{
    [Authorize(Roles ="Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;
        public IndexModel(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }
        public List<IdentityUser> Users { get; set; }
        public void OnGet()
        {
            Users = userManager.Users.ToList();
        }
    }
}
