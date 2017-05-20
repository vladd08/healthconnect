using System;
using System.Web;
using System.Web.Mvc;
using HealthConnectWeb.Models;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

namespace HealthConnectWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                Medic medic = new Medic();
                if (HttpContext.Request.Cookies["IdMedic"].Value != "")
                {
                    medic.Id = Convert.ToInt32(HttpContext.Request.Cookies["IdMedic"].Value);
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
                    if (HttpContext.Request.Cookies["CnpPacient"].Value != "")
                    {
                        pacient.Cnp = HttpContext.Request.Cookies["CnpPacient"].Value;
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
                    request = (HttpWebRequest)WebRequest.Create(@"https://healthconnectapi.azurewebsites.net/api/Users/Medic/" + userCipher + "/" + passwordCipher);
                }
                else
                {
                    request = (HttpWebRequest)WebRequest.Create(@"https://healthconnectapi.azurewebsites.net/api/Users/Pacient/" + username + "/" + passwordCipher);
                }
            try
            {

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
                        HttpCookie idCookie = new HttpCookie("IdMedic", medic.Id.ToString());
                        HttpCookie nameCookie = new HttpCookie("NameMedic", medic.Nume);
                        HttpCookie lastnameCookie = new HttpCookie("LastNameMedic", medic.Prenume);
                        HttpContext.Response.Cookies.Add(idCookie);
                        HttpContext.Response.Cookies.Add(nameCookie);
                        HttpContext.Response.Cookies.Add(lastnameCookie);
                        return View("Medic", medic);
                    }
                    else
                    {
                        Pacient pacient = new Pacient();
                        pacient = JsonConvert.DeserializeObject<Pacient>(content);
                        HttpCookie cnpCookie = new HttpCookie("CnpPacient", pacient.Cnp);
                        HttpCookie nameCookie = new HttpCookie("NamePacient", pacient.Nume);
                        HttpCookie lastnameCookie = new HttpCookie("LastNamePacient", pacient.Prenume);
                        HttpContext.Response.Cookies.Add(cnpCookie);
                        HttpContext.Response.Cookies.Add(nameCookie);
                        HttpContext.Response.Cookies.Add(lastnameCookie);
                        return View("Pacient", pacient);
                    }
                }
            }
            catch
            {
                User user = new User();
                user.Error = "Username or password is wrong!";
                return View("LoginError", user);
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

        public PartialViewResult ShowAllPacients()
        {
            List<Pacient> pacienti = new List<Pacient>();
            HttpWebRequest request;
            request = (HttpWebRequest)WebRequest.Create(@"https://healthconnectapi.azurewebsites.net/api/Medic/" + HttpContext.Request.Cookies["IdMedic"].Value + "/Pacienti");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string content = new StreamReader(response.GetResponseStream()).ReadToEnd();
            pacienti = JsonConvert.DeserializeObject<List<Pacient>>(content);
            return PartialView("PacientiTable", pacienti);
        }

        public PartialViewResult SetPacient()
        {
            Pacient pacient = new Pacient();
            return PartialView("AddPacient", pacient);
        }

        public PartialViewResult SetShowPacient()
        {
            Pacient pacient = new Pacient();
            return PartialView("SetSearch", pacient);
        }

        public PartialViewResult ShowPacient(string cnp)
        {
            if(cnp == "")
            {
                return PartialView("RecomandareError", "Failed");
            } else
            {
                List<Pacient> listaPacienti = new List<Pacient>();
                Pacient pacient = new Pacient();
                HttpWebRequest request;
                request = (HttpWebRequest)WebRequest.Create(@"https://healthconnectapi.azurewebsites.net/api/Medic/" + HttpContext.Request.Cookies["IdMedic"].Value + "/Pacient/" + cnp);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string content = new StreamReader(response.GetResponseStream()).ReadToEnd();
                if(content != "null")
                {
                    pacient = JsonConvert.DeserializeObject<Pacient>(content);
                    listaPacienti.Add(pacient);
                }
                return PartialView("PacientiTable", listaPacienti);
            }
        }

        public PartialViewResult SetEditPacient(string cnp)
        {
            List<Pacient> pacienti = new List<Pacient>();
            Pacient pacient = new Pacient();
            HttpWebRequest request;
            request = (HttpWebRequest)WebRequest.Create(@"https://healthconnectapi.azurewebsites.net/api/Medic/" + HttpContext.Request.Cookies["IdMedic"].Value + "/Pacienti");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string content = new StreamReader(response.GetResponseStream()).ReadToEnd();
            pacienti = JsonConvert.DeserializeObject<List<Pacient>>(content);
            foreach(Pacient d in pacienti)
            {
                if(d.Cnp == cnp)
                {
                    pacient = d;
                }
            }
            return PartialView("EditPacient", pacient);
        }

        public PartialViewResult SetAddRecomandare(string cnp)
        {
            List<Pacient> pacienti = new List<Pacient>();
            Pacient pacient = new Pacient();
            HttpWebRequest request;
            request = (HttpWebRequest)WebRequest.Create(@"https://healthconnectapi.azurewebsites.net/api/Medic/" + HttpContext.Request.Cookies["IdMedic"].Value + "/Pacienti");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string content = new StreamReader(response.GetResponseStream()).ReadToEnd();
            pacienti = JsonConvert.DeserializeObject<List<Pacient>>(content);
            foreach (Pacient d in pacienti)
            {
                if (d.Cnp == cnp)
                {
                    pacient = d;
                }
            }
            return PartialView("Recomandare", pacient);
        }

        public PartialViewResult AddPacient(Pacient pacient)
        {
            Pacient mPacient = new Pacient();
            mPacient = pacient;
            HttpWebRequest request;
            request = (HttpWebRequest)WebRequest.Create(@"https://healthconnectapi.azurewebsites.net/api/Medic/" + HttpContext.Request.Cookies["IdMedic"].Value);
            request.Method = "POST";
            request.ContentType = "application/json";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(mPacient);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string content = new StreamReader(response.GetResponseStream()).ReadToEnd();
            if(content == "201")
            {
                pacient.AddError = false;
                return PartialView("AddError", pacient);
            }
            else
            {
                pacient.AddError = true;
                return PartialView("AddError", pacient);
            }
        }

        public PartialViewResult DeletePacient(string cnp)
        {
            HttpWebRequest request;
            request = (HttpWebRequest)WebRequest.Create(@"https://healthconnectapi.azurewebsites.net/api/Delete/" + HttpContext.Request.Cookies["IdMedic"].Value + "/" + cnp);
            request.Method = "DELETE";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string content = new StreamReader(response.GetResponseStream()).ReadToEnd();

            Pacient pacient = new Pacient();
            if (content == "201")
            {
                pacient.AddError = false;
                return PartialView("DeleteError", pacient);
            }
            else
            {
                pacient.AddError = true;
                return PartialView("DeleteError", pacient);
            }
        }

        public PartialViewResult EditPacient(string cnp, Pacient pac)
        {
            List<Pacient> pacienti = new List<Pacient>();
            Pacient mPacient = new Pacient();
            HttpWebRequest request;
            request = (HttpWebRequest)WebRequest.Create(@"https://healthconnectapi.azurewebsites.net/api/Medic/" + HttpContext.Request.Cookies["IdMedic"].Value + "/Pacienti");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string content = new StreamReader(response.GetResponseStream()).ReadToEnd();
            pacienti = JsonConvert.DeserializeObject<List<Pacient>>(content);

            foreach (Pacient p in pacienti)
            {
                if(p.Cnp == cnp)
                {
                    mPacient = p;
                }
            }

            mPacient.Cnp = pac.Cnp;
            mPacient.Varsta = pac.Varsta;
            mPacient.IdMedic = pac.IdMedic;
            mPacient.Email = pac.Email;
            mPacient.Localitate = pac.Localitate;
            mPacient.Strada = pac.Strada;
            mPacient.NrStrada = pac.NrStrada;
            mPacient.NrTelefon = pac.NrTelefon;
            mPacient.Profesie = pac.Profesie;
            mPacient.LocMunca = pac.LocMunca;

            HttpWebRequest mRequest;
            mRequest = (HttpWebRequest)WebRequest.Create(@"https://healthconnectapi.azurewebsites.net/api/Pacient/" + cnp);
            mRequest.Method = "PUT";
            mRequest.ContentType = "application/json";
            using (var streamWriter = new StreamWriter(mRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(mPacient);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            HttpWebResponse mResponse = (HttpWebResponse)mRequest.GetResponse();
            string mContent = new StreamReader(mResponse.GetResponseStream()).ReadToEnd();

            Pacient pacient = new Pacient();
            if (mContent == "201")
            {
                pacient.AddError = false;
                return PartialView("EditError", pacient);
            }
            else
            {
                pacient.AddError = true;
                return PartialView("EditError", pacient);
            }
        }

        public PartialViewResult AddMedic(Medic medic)
        {
            Medic mMedic = new Medic();
            mMedic = medic;
            HttpWebRequest request;
            request = (HttpWebRequest)WebRequest.Create(@"https://healthconnectapi.azurewebsites.net/api/Medici/Medic");
            request.Method = "POST";
            request.ContentType = "application/json";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(mMedic);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string content = new StreamReader(response.GetResponseStream()).ReadToEnd();
            if (content == "201")
            {
                medic.AddError = false;
                return PartialView("AddMedicError", medic);
            }
            else
            {
                medic.AddError = true;
                return PartialView("AddMedicError", medic);
            }
        }

        public PartialViewResult AdaugaRecomandare(string cnp, Recomandare recomandare)
        {
            if(recomandare.TipActivitate == null || recomandare.Mesaj == null)
            {
                return PartialView("RecomandareError", "Failed");
            } else
            {
                Recomandare rm = recomandare;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"https://healthconnectapi.azurewebsites.net/api/Recomandare/" + cnp);
                request.Method = "POST";
                request.ContentType = "application/json";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(rm);

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string content = new StreamReader(response.GetResponseStream()).ReadToEnd();
                if (content == "201")
                {
                    return PartialView("RecomandareError", "Succes");
                }
                else
                {
                    return PartialView("RecomandareError", "Failed");
                }
            }
        }

        public PartialViewResult VizualizeazaRecomandari(string cnp)
        {
            List<Recomandare> listaRecomandari = new List<Recomandare>();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"https://healthconnectapi.azurewebsites.net/api/Recomandare/" + cnp);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string content = new StreamReader(response.GetResponseStream()).ReadToEnd();
            listaRecomandari = JsonConvert.DeserializeObject<List<Recomandare>>(content);
            return PartialView("RecomandariTable", listaRecomandari);
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