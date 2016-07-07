using db = RoomManager.Model.DBConnector;
using SqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Text;
using System.Security.Claims;
using System;

namespace RoomManager.Model
{
    public class UserHelper
    {
        public static SqlConnection conn = db.GetConnection();
        public static DataHelper<User> DHUser = new DataHelper<User>(ref conn);
        public static ClaimsPrincipal UserLogin(HttpContext context, string username, string password) {
            string passhash = MD5Hash(password);
            User user = DHUser.SelectOne(String.Format("username = '{0}' AND password = '{1}'", 
                username, passhash));

            if (user == null) {
                return null; 
            }

            // Set session
            context.Session.SetString("username", username);
            context.Session.SetString("password", passhash);
            context.Session.SetInt32("privilege", (int)user.Privilege);

            List<Claim> claim = new List<Claim>();
            claim.Add(new Claim(user.Privilege.ToString(), user.Password, "MD5", user.UserName));
            ClaimsIdentity identity = new ClaimsIdentity(claim);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);            
            return principal;
        }

        public static User CurrentUser(HttpContext context) {
            byte [] bName, bPass;
            string username, passhash;
            bool coden = context.Session.TryGetValue("username", out bName);
            bool codep = context.Session.TryGetValue("password", out bPass);
            if ( !coden || !codep ) {
                return null;
            }

            username = Encoding.UTF8.GetString(bName);
            passhash = Encoding.UTF8.GetString(bPass);

            User user = DHUser.SelectOne(String.Format("username = '{0}' AND password = '{1}'", username, passhash));
            return user;
        } 

        public static bool IsAdmin(HttpContext context) {
            User usr = CurrentUser(context);
            if (usr == null || usr.Privilege != UserPrivilege.Admin) {
                return false;
            }
            return true;
        }

        public static string MD5Hash(string input) {
            MD5 md5hasher = MD5.Create();
            StringBuilder sBuilder = new StringBuilder();
            byte[] data = md5hasher.ComputeHash(Encoding.UTF8.GetBytes(input));

            foreach(byte b in data) {
                sBuilder.Append(b.ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}