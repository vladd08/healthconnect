using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HealthconnectAPI.Models;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace HealthconnectAPI.Controllers
{
    public class UsersController : ApiController
    {
        private string connectionString = @"Server=tcp:softmed.database.windows.net,1433;Initial Catalog=SoftmedTest;Persist Security Info=False;User ID=vlad.orbulescu;Password=Par0la01;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        [HttpGet]
        [Route("api/Users/Medic/{userCipher}/{passwordCipher}")]
        public Medic AuthenticateMedic(string userCipher, string passwordCipher)
        {

            string username = Decrypt(userCipher);
            string password = Decrypt(passwordCipher);

            SqlDataReader reader = null;
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = System.Data.CommandType.Text;
            sqlCmd.Parameters.AddWithValue("@un", username);
            sqlCmd.Parameters.AddWithValue("@pw", password);
            sqlCmd.CommandText = "SELECT * FROM Medici WHERE Username = @un AND Password = @pw";
            sqlCmd.Connection = sqlConnection;
            sqlConnection.Open();
            reader = sqlCmd.ExecuteReader();
            Medic medic = null;
            while (reader.Read())
            {
                medic = new Medic();
                medic.Id = Convert.ToInt32(reader.GetValue(0));
                medic.Nume = reader.GetValue(1).ToString();
                medic.Prenume = reader.GetValue(2).ToString();
            }
            return medic;
        }

        [HttpGet]
        [Route("api/Users/Pacient/{userCipher}/{passwordCipher}")]
        public Pacient AuthenticatePacient(string userCipher, string passwordCipher)
        {
            string password = Decrypt(passwordCipher);

            SqlDataReader reader = null;
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = System.Data.CommandType.Text;
            sqlCmd.Parameters.AddWithValue("@un", userCipher);
            sqlCmd.Parameters.AddWithValue("@pw", password);
            sqlCmd.CommandText = "SELECT * FROM Pacienti WHERE Username = @un AND Password = @pw";
            sqlCmd.Connection = sqlConnection;
            sqlConnection.Open();
            reader = sqlCmd.ExecuteReader();
            Pacient pacient = null;
            while (reader.Read())
            {
                pacient = new Pacient();
                pacient.Cnp = reader.GetValue(0).ToString();
                pacient.Nume = reader.GetValue(2).ToString();
                pacient.Prenume = reader.GetValue(3).ToString();
            }
            return pacient;
        }

        private string Decrypt(string source)
        {
            TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();

            byte[] byteHash;
            byte[] byteBuff;
            string key = "234fjhdf3";

            byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
            desCryptoProvider.Key = byteHash;
            desCryptoProvider.Mode = CipherMode.ECB;
            source = HttpUtility.UrlDecode(source);
            source = source.Replace(' ', '+');
            byteBuff = Convert.FromBase64String(source);

            string plaintext = Encoding.UTF8.GetString(desCryptoProvider.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            return plaintext;
        }
    }
}
