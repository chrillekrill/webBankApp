using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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

        //public IActionResult OnPost(IdentityUser editUser)
        //{
        //    if(ModelState.IsValid)
        //    {
                

        //        return Page();
        //    }

        //    return Page();
        //}

        //private void CreateUserIfNotExists(string email, string password, string role)
        //{
        //    if (_userManager.FindByEmailAsync(email).Result != null) return;

        //    var user = new IdentityUser
        //    {
        //        UserName = email,
        //        Email = email,
        //        EmailConfirmed = true
        //    };
        //    _userManager.CreateAsync(user, password).Wait();
        //    _userManager.AddToRoleAsync(user, role).Wait();

        //}
    }
}
