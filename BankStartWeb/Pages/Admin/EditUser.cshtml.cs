using BankStartWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;

namespace BankAppWeb.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class EditUserModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext context;
        private readonly IToastNotification toastNotification;

        public EditUserModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, IToastNotification toastNotification)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.context = context;
            this.toastNotification = toastNotification;
        }
        public List<SelectListItem> AllRoles { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Roles { get; set; }
        public string SelectedRoleToAdd { get; set; }
        [BindProperty]
        public IList<string> UserRoles { get; set; } = new List<string>();
        private void SetRoles()
        {
            AllRoles = Roles.Select(r => new SelectListItem
            {
                Text = r,
                Value = r
            }).ToList();
            AllRoles.Insert(0, new SelectListItem
            {
                Text = "Chose a role to add",
                Value = ""
            });
        }
        public void OnGet(string editUser)
        {
            var user = userManager.Users.First(x => x.Id == editUser);

            Id = user.Id;
            Name = user.UserName;

            Roles = roleManager.Roles.Select(r => r.Name).ToList();

            SetRoles();

            var roleIds = userManager.GetRolesAsync(user).Result;

            foreach (var roleId in roleIds)
            {
                UserRoles.Add(roleId);
            };
        }

        public IActionResult OnPostAddRole(string id, string selectedRoleToAdd)
        {
            if (ModelState.IsValid)
            {
                //var addRole = new IdentityUser
                //{
                //    UserName = Name,
                //    Email = Name,
                //    EmailConfirmed = true
                //};

                var user = userManager.Users.First(x => x.Id == id);

                var roleIds = userManager.GetRolesAsync(user).Result;

                foreach (var roleId in roleIds)
                {
                    UserRoles.Add(roleId);
                };

                if (UserRoles.Count > 0)
                {
                    foreach (var role in UserRoles)
                    {
                        if (role == selectedRoleToAdd)
                        {
                            toastNotification.AddErrorToastMessage("Role already exists on user");

                            Id = user.Id;
                            Name = user.UserName;

                            Roles = roleManager.Roles.Select(r => r.Name).ToList();

                            return Page();
                        }
                    }
                }
                userManager.AddToRoleAsync(user, selectedRoleToAdd).Wait();

                context.SaveChanges();

                toastNotification.AddSuccessToastMessage("Role added to user");

                return RedirectToPage("EditUser", new { editUser = id });

            }

            return Page();
        }

        public IActionResult OnPostRemoveRole(string roleName, string id)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.Users.First(x => x.Id == id);

                userManager.RemoveFromRoleAsync(user, roleName).Wait();

                context.SaveChanges();

                return RedirectToPage("EditUser", new { editUser = id });
            }
            return Page();
        }
    }
}
