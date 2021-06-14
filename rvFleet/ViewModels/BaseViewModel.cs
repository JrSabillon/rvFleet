using rvFleet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using System.Web.Security;

namespace rvFleet.ViewModels
{
    public class BaseViewModel
    {
        public static usuario GetUserData()
        {
            HttpCookie userCookie = HttpContext.Current.Request.Cookies.Get(FormsAuthentication.FormsCookieName);

            if(userCookie != null)
            {
                string value = FormsAuthentication.Decrypt(userCookie.Value).UserData;
                usuario user = JsonConvert.DeserializeObject<usuario>(value);

                return user;
            }

            return new usuario();
        } 

        public static string CreateHash(string data)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}