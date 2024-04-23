using BooksSpring2024.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Extensions;

namespace BooksSpring2024.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles ="Admin")]
    public class UserController : Controller
    {

        private BooksDBContext _dbContext; //database variable 

        //create a user manager variable to use to manage user's roles further down
        //has previously been used within the register class to add roles 
        //needs to be injected within the constructor 
        private UserManager<IdentityUser> _userManager;

        //constructor 
        public UserController(BooksDBContext dbContext, UserManager<IdentityUser> userManager) //dependency injection
        {
              _dbContext = dbContext;
            _userManager = userManager;
        }


        //list all the users within index method
        public IActionResult Index()
        {
            //var listOfUsers = _dbContext.ApplicationUsers.ToList(); //application users refers to the AspNetUsers table 
            List<ApplicationUser> userList = _dbContext.ApplicationUsers.ToList();

            //when referring to aspNet table, don't include "aspNet"
            var allRoles = _dbContext.Roles.ToList(); //fetches all the different roles

            var userRoles = _dbContext.UserRoles.ToList(); //fetches the specific roles for each user 

            foreach(var user in userList)
            {
                var roleID = userRoles.Find(ur => ur.UserId == user.Id).RoleId; //fetches the role ID for the current user 

                var roleName = allRoles.Find(r => r.Id == roleID).Name; //fetches the name of the role that the current user belongs to 

                //save the above information within user
                user.RoleName = roleName; //atribute in user assigned value retrieved above, attribute is not stored within database 

            }

            return View(userList);
        }

        public IActionResult LockUnlock(string ID)
        {
            var userFromDB = _dbContext.ApplicationUsers.Find(ID);

            if(userFromDB.LockoutEnd != null && userFromDB.LockoutEnd > DateTime.Now)
            {
                //user account is already locked, unlock account 
                userFromDB.LockoutEnd = DateTime.Now;
            }
            else
            {
                //user account is unlocked, lock account 
                userFromDB.LockoutEnd = DateTime.Now.AddYears(10);
            }
            _dbContext.SaveChanges();

            return RedirectToAction("Idex");
                    
        }

        public IActionResult EditUserRole(string id)
        {
            var currentUserRole = _dbContext.UserRoles.FirstOrDefault(ur => ur.UserId == id); //fetches the userID and the roles ID from the userRoles aspnet table 

            IEnumerable<SelectListItem> listOfRoles = _dbContext.Roles.ToList().Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id.ToString()
            });

            //show more than just userID
            ViewBag.ListOfRoles = listOfRoles;

            ViewBag.UserInfo = _dbContext.ApplicationUsers.Find(id);

            return View(currentUserRole);
        }

        [HttpPost]
        public IActionResult EditUserRole(Microsoft.AspNetCore.Identity.IdentityUserRole<string> updatedRole)
        {
            //fetch the user from updated role
            ApplicationUser applicationUser = _dbContext.ApplicationUsers.Find(updatedRole.UserId);

            //find new role name
            string newRoleName = _dbContext.Roles.Find(updatedRole.RoleId).Name;

            //find old role ID
            string oldRoleID = _dbContext.UserRoles.FirstOrDefault(u => u.UserId == applicationUser.Id).RoleId;

            //find old role name, command works with name of the role instead of the roleID
            //this is why we are retrieving the role names in addition to the IDS
            string oldRoleName = _dbContext.Roles.Find(oldRoleID).Name;

            //two parameters - user and current role
            //removes the old record 
            _userManager.RemoveFromRoleAsync(applicationUser, oldRoleName).GetAwaiter().GetResult();

            //add new role with new value
            _userManager.AddToRoleAsync(applicationUser, newRoleName).GetAwaiter().GetResult();


            return RedirectToAction("Index");
        }

    }

   
}
