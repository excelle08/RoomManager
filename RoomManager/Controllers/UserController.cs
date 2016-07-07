using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using RoomManager.Model;
using SqlConnection = MySql.Data.MySqlClient.MySqlConnection;


namespace RoomManager.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        static SqlConnection conn = DBConnector.GetConnection();
        static DataHelper<User> DHUser = new DataHelper<User>(conn);
        // GET /api/user (Get current user)
        [HttpGet]
        public IActionResult Get()
        {
            var usr = UserHelper.CurrentUser(HttpContext);
            if (usr == null) {
                return Forbid();
            }

            usr.Password = "";
            return new ObjectResult(usr);
        }

        // GET /api/user/all (Get all users, for admin)
        [HttpGetAttribute("all")]
        public IActionResult GetAll() {
            if (UserHelper.IsAdmin(HttpContext)) {

                var results = DHUser.SelectAll("");
                return new ObjectResult(results);
            }
            return Forbid();
        }

        // GET /api/user/{id}, (Get user of id, for admin)
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (UserHelper.IsAdmin(HttpContext)) {

                var result = DHUser.SelectOne(String.Format("id = {0}", id));
                if (result == null) {
                    return NotFound();
                }
                return new ObjectResult(result);
            }

            return Forbid();
        }

        // POST /api/user, (Login)
        [HttpPost]
        public IActionResult Post([FromBody] UserCredential cred) {
            string username = cred.username, password = cred.password;
            bool remember = cred.remember;

            ClaimsPrincipal principal = UserHelper.UserLogin(HttpContext, username, password);
            
            if (principal != null) {
                HttpContext.Authentication.SignInAsync("RMCookieMiddlewareInstance", principal);
                return Ok(new {status = "ok"});
            }
            return new ObjectResult(new {error = "login", message = "Login failed"});
        }

        // PUT /api/user (Create a new user, for admin)
        [HttpPut]
        public IActionResult Put([FromBody]User user) {
            if (UserHelper.IsAdmin(HttpContext)) {
                user.UserName = user.UserName.Trim();
                user.Password = user.Password.Trim();
                if (user.Password.Length == 0 || user.Password.Length == 0) {
                    return new ObjectResult(new {error = "user", message = "Username or password is empty"});
                }

                User existingUser = DHUser.SelectOne(String.Format("username = '{0}'", user.UserName));
                
                if (existingUser != null) {
                    return new ObjectResult(new {error = "user", message = "Username exists"});
                }
                user.Password = UserHelper.MD5Hash(user.Password);

                User result = DHUser.Insert(user);

                return new ObjectResult(result);
            }
            return Forbid();
        }

        // DELETE /api/user/id (Delete user, for admin)
        [HttpDelete("{id}")]
        public IActionResult Delete(int id) {
            if (UserHelper.IsAdmin(HttpContext)) {

                User user = DHUser.SelectOne(String.Format("id = {0}", id));
                if (user == null) {
                    return NotFound();
                } 

                int deleteRes = DHUser.Delete(user);
                if (deleteRes == 0) {
                    return new ObjectResult(new {error = "delete", message = "Failed to delete user"});
                } else {
                    return new ObjectResult(user);
                }
            }

            return Forbid();
        }

        [HttpDeleteAttribute]
        public IActionResult Logout() {
            HttpContext.Authentication.SignOutAsync("RMCookieMiddlewareInstance");
            HttpContext.Session.Remove("username");
            HttpContext.Session.Remove("password");
            HttpContext.Session.Remove("privilege");

            return Redirect("/");
        }

        // PATCH /api/user/{id}/password
        [HttpPatchAttribute("{id}/password")]
        public IActionResult UpdatePassword(int id, [FromBody] string password) {
            if (UserHelper.IsAdmin(HttpContext)) {

                password = password.Trim();
                if (password.Length == 0) {
                    return BadRequest(new {error = "passwd", message = "Empty password"});
                }

                User user = DHUser.SelectOne(String.Format("id = {0}", id));
                if (user == null) {
                    return NotFound();
                }

                user.Password = UserHelper.MD5Hash(password);
                int updateRes = DHUser.Update(user);

                if (updateRes == 0) {
                    return new ObjectResult(new {error = "passwd", message = "Error updating password"});
                }

                return new ObjectResult(user);
            }

            return Forbid();
        }

    }
}
