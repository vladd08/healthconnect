using System;
using System.Web;
using System.Web.Mvc;
using HealthConnectWeb.Models;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace HealthConnectWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                Medic medic = new Medic();
                if (HttpContext.Request.Cookies["UsernameMedic"].Value != "")
                {
                    medic.Nume = HttpContext.Request.Cookies["NameMedic"].Value;
                    medic.Prenume = HttpContext.Request.Cookies["LastnameMedic"].Value;
                }
                return View("Medic", medic);
            }
            catch
            {
                try
                {
                    Pacient pacient = new Pacient();
                    if (HttpContext.Request.Cookies["UsernamePacient"].Value != "")
                    {
                        pacient.Nume = HttpContext.Request.Cookies["NamePacient"].Value;
                        pacient.Prenume = HttpContext.Request.Cookies["LastnamePacient"].Value;
                    }
                    return View("Pacient", pacient);
                }
                catch
                {
                    try
                    {
                        if(HttpContext.Request.Cookies["Admin"].Value != "")
                        {
                            return View("Admin");
                        }
                    }
                    catch
                    {
                        return View();
                    }

                    return View();
                }
            }

        }
        
        public ActionResult Login()
        {
            return View();
        }
        
        public ActionResult Authenticate(string username, string password, bool? role)
        {      
                HttpWebRequest request = null;

                string userCipher = Encrypt(username);
                string passwordCipher = Encrypt(password);

                if (role == true)
                {
                    request = (HttpWebRequest)WebRequest.Create(@"http://healthconnectapi.azurewebsites.net/api/Users/Medic/" + userCipher + "/" + passwordCipher);
                }
                else
                {
                    request = (HttpWebRequest)WebRequest.Create(@"http://healthconnectapi.azurewebsites.net/api/Users/Pacient/" + username + "/" + passwordCipher);
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string content = new StreamReader(response.GetResponseStream()).ReadToEnd();

                if (content == "null")
                {
                    if (username == "Admin" && password == "admin")
                    {
                        var admin = "admin";
                        HttpCookie adminCookie = new HttpCookie("Admin", admin);
                        HttpContext.Response.Cookies.Add(adminCookie);
                        return View("Admin");
                    }
                    else
                    {
                    User user = new User();
                    user.Error = "Username or password is wrong!";
                    return View("LoginError", user);
                    }
                }
                else
                {
                    if (role == true)
                    {
                        Medic medic = new Medic();
                        medic = JsonConvert.DeserializeObject<Medic>(content);
                        HttpCookie usernameCookie = new HttpCookie("UsernameMedic", username);
                        HttpCookie nameCookie = new HttpCookie("NameMedic", medic.Nume);
                        HttpCookie lastnameCookie = new HttpCookie("LastNameMedic", medic.Prenume);
                        HttpContext.Response.Cookies.Add(usernameCookie);
                        HttpContext.Response.Cookies.Add(nameCookie);
                        HttpContext.Response.Cookies.Add(lastnameCookie);
                        return View("Medic", medic);
                    }
                    else
                    {
                        Pacient pacient = new Pacient();
                        pacient = JsonConvert.DeserializeObject<Pacient>(content);
                        HttpCookie usernameCookie = new HttpCookie("UsernamePacient", username);
                        HttpCookie nameCookie = new HttpCookie("NamePacient", pacient.Nume);
                        HttpCookie lastnameCookie = new HttpCookie("LastNamePacient", pacient.Prenume);
                        HttpContext.Response.Cookies.Add(usernameCookie);
                        HttpContext.Response.Cookies.Add(nameCookie);
                        HttpContext.Response.Cookies.Add(lastnameCookie);
                        return View("Pacient", pacient);
                    }
                }
           
        }

        public ActionResult Logout()
        {
            string[] myCookies = Request.Cookies.AllKeys;
            foreach (string cookie in myCookies)
            {
                Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
            }
            return View("Index");
        }

        private string Encrypt(string source)
        {
            TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();
            byte[] byteHash;
            byte[] byteBuff;
            string key = "234fjhdf3";

            byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
            desCryptoProvider.Key = byteHash;
            desCryptoProvider.Mode = CipherMode.ECB;
            byteBuff = Encoding.UTF8.GetBytes(source);
            string cipher = Convert.ToBase64String(desCryptoProvider.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            return HttpUtility.UrlEncode(cipher);
        }
    }
}