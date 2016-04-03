using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using RestaurantPortal.Models;

namespace RestaurantPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        // GET: Users
        private ApplicationUserManager _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private ApplicationDbContext _context;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationDbContext Context
        {
            get
            {
                return _context ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            }
            set
            {
                _context = value;
            }
        }

        public RoleManager<IdentityRole> RoleManager
        {
            get
            {
                if (_roleManager == null)
                    _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(Context));
                return _roleManager;
            }
            set
            {
                _roleManager = value;
            }
        }

        public UsersController()
        {
        }

        public ActionResult Index()
        {
            var users = UserManager.Users.ToList();
            ViewBag.Roles = RoleManager.Roles.ToList();
            return View(users);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(string id, UserEditViewModel userEditModel)
        {
            
            ViewBag.AllRoles = GetAllRoles();

            var user = await UserManager.FindByIdAsync(id);
            user.UserName = userEditModel.UserName;
            user.Email = userEditModel.Email;

            if (!string.IsNullOrWhiteSpace(userEditModel.Password))
            {
                await UserManager.RemovePasswordAsync(id);
                await UserManager.UpdateAsync(user);

                var result = await UserManager.AddPasswordAsync(id, userEditModel.Password);
                await UserManager.UpdateAsync(user);
            }

            if (userEditModel.Roles == null)
                userEditModel.Roles = new List<string>();

            var rolesForUser = await UserManager.GetRolesAsync(id);
            var rolesToAdd = userEditModel.Roles.Except(rolesForUser);
            var rolesToRemove = rolesForUser.Except(userEditModel.Roles);
            
            foreach (var item in rolesToRemove)
            {
                var result = await UserManager.RemoveFromRoleAsync(id, item);
            }


            foreach (var roleName in rolesToAdd)
            {
                var role = await RoleManager.FindByNameAsync(roleName);
                var result = await UserManager.AddToRoleAsync(id, role.Name);
            }
            await UserManager.UpdateAsync(user);
            return RedirectToAction("Index");   
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            var user = UserManager.Users.FirstOrDefault(_ => _.Id == id);

            if (user == null)
            {
                throw new ApplicationException($"No User With Id {id}");
            }

            var selectedRoles = GetUserRoles(user);
            ViewBag.RoleId = new MultiSelectList(RoleManager.Roles, "Id", "Name", selectedRoles);
            ViewBag.Roles = selectedRoles;

            ViewBag.AllRoles = GetAllRoles();
            ViewBag.SelectedRoles = selectedRoles;
            return View(new UserEditViewModel() {Id = user.Id, UserName = user.UserName, Email = user.Email, Roles = selectedRoles});

        }

        private List<string> GetAllRoles()
        {
            return RoleManager.Roles.Select(_ => _.Name).ToList();
        }

        private List<string> GetUserRoles(ApplicationUser user)
        {
            var selectedRoles = user.Roles.Select(role => RoleManager.FindById(role.RoleId).Name).OrderBy(_ => _).ToList();
            return selectedRoles;
        }

        [HttpGet]
        public ActionResult New()
        {

            ViewBag.AllRoles = GetAllRoles();
            return View(new UserEditViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> New(UserEditViewModel userEditModel)
        {

            var user = new ApplicationUser();
            user.UserName = userEditModel.UserName;
            user.Email = userEditModel.Email;
            await UserManager.CreateAsync(user, userEditModel.Password);

            if (userEditModel.Roles == null)
                userEditModel.Roles = new List<string>();

            foreach (var roleName in userEditModel.Roles)
            {
                var role = await RoleManager.FindByNameAsync(roleName);
                var result = await UserManager.AddToRoleAsync(user.Id, role.Name);
            }
            return RedirectToAction("Index");
        }
    }

    public class UserEditViewModel
    {
 
        [Key]
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public List<String> Roles { get; set; }
    }
}