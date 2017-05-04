using HealthconnectAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HealthconnectAPI.Controllers
{
    public class PacientiController : ApiController
    {
        private string connectionString = @"Server=tcp:softmed.database.windows.net,1433;Initial Catalog=SoftmedTest;Persist Security Info=False;User ID=vlad.orbulescu;Password=Par0la01;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        [HttpGet]
        [Route("api/Medic/{IdMedic}/Pacienti")]
        public List<Pacient> GetPacienti(int IdMedic)
        {
            List<Pacient> listaPacienti = new List<Pacient>();
            SqlDataReader reader = null;
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = System.Data.CommandType.Text;
            sqlCmd.Connection = sqlConnection;
            sqlCmd.Parameters.AddWithValue("@Id", IdMedic);
            sqlCmd.CommandText = "SELECT * FROM Pacienti WHERE IdMedic = @Id";
            sqlCmd.Connection = sqlConnection;
            sqlConnection.Open();
            reader = sqlCmd.ExecuteReader();

            Pacient pacient = null;
            while(reader.Read())
            {
                pacient = new Pacient();
                pacient.Cnp = reader.GetValue(0).ToString();
                pacient.Nume = reader.GetValue(2).ToString();
                pacient.Prenume = reader.GetValue(3).ToString();
                pacient.Varsta = Convert.ToInt32(reader.GetValue(6));
                pacient.Localitate = reader.GetValue(7).ToString();
                pacient.Strada = reader.GetValue(8).ToString();
                pacient.NrStrada = Convert.ToInt32(reader.GetValue(9));
                pacient.NrTelefon = reader.GetValue(10).ToString();
                pacient.Email = reader.GetValue(11).ToString();
                pacient.Profesie = reader.GetValue(12).ToString();
                pacient.LocMunca = reader.GetValue(13).ToString();
                listaPacienti.Add(pacient);
            }
            return listaPacienti;
        }

        [HttpGet]
        [Route("api/Medic/{IdMedic}/Pacient/{Cnp}")]
        public Pacient GetPacient(int IdMedic, string Cnp)
        {
            SqlDataReader reader = null;
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = System.Data.CommandType.Text;
            sqlCmd.Connection = sqlConnection;
            sqlCmd.Parameters.AddWithValue("@Cnp", Cnp);
            sqlCmd.Parameters.AddWithValue("@Id", IdMedic);
            sqlCmd.CommandText = "SELECT * FROM Pacienti WHERE Cnp = @Cnp AND IdMedic = @Id";
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
                pacient.Varsta = Convert.ToInt32(reader.GetValue(6));
                pacient.Localitate = reader.GetValue(7).ToString();
                pacient.Strada = reader.GetValue(8).ToString();
                pacient.NrStrada = Convert.ToInt32(reader.GetValue(9));
                pacient.NrTelefon = reader.GetValue(10).ToString();
                pacient.Email = reader.GetValue(11).ToString();
                pacient.Profesie = reader.GetValue(12).ToString();
                pacient.LocMunca = reader.GetValue(13).ToString();
            }
            return pacient;
        }

        [HttpPost]
        [Route("api/Medic/{IdMedic}")]
        public async System.Threading.Tasks.Task<HttpStatusCode> AuthenticatePacient([FromBody]Pacient pacient)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = System.Data.CommandType.Text;
            sqlCmd.Connection = sqlConnection;
            sqlCmd.Parameters.AddWithValue("@Cnp", pacient.Cnp);
            sqlCmd.Parameters.AddWithValue("@IdMedic", pacient.IdMedic);
            sqlCmd.Parameters.AddWithValue("@Nume", pacient.Nume);
            sqlCmd.Parameters.AddWithValue("@Prenume", pacient.Prenume);
            sqlCmd.Parameters.AddWithValue("@Username", pacient.Username);
            sqlCmd.Parameters.AddWithValue("@Password", pacient.Password);
            sqlCmd.Parameters.AddWithValue("@Varsta", pacient.Varsta);
            sqlCmd.Parameters.AddWithValue("@Localitate", pacient.Localitate);
            sqlCmd.Parameters.AddWithValue("@Strada", pacient.Strada);
            sqlCmd.Parameters.AddWithValue("@NrStrada", pacient.NrStrada);
            sqlCmd.Parameters.AddWithValue("@Telefon", pacient.NrTelefon);
            sqlCmd.Parameters.AddWithValue("@Email", pacient.Email);
            sqlCmd.Parameters.AddWithValue("@Profesie", pacient.Profesie);
            sqlCmd.Parameters.AddWithValue("@LocMunca", pacient.LocMunca);
            sqlCmd.CommandText = "INSERT INTO Pacienti (IdMedic,Cnp,Nume,Prenume,Username,Password,Varsta,Localitate,Strada,NrStrada,NrTelefon,Email,Profesie,LocMunca) VALUES(@IdMedic,@Cnp,@Nume,@Prenume,@Username,@Password,@Varsta,@Localitate,@Strada,@NrStrada,@Telefon,@Email,@Profesie,@LocMunca)";
            sqlCmd.Connection = sqlConnection;
            sqlConnection.Open();
            var check = await sqlCmd.ExecuteNonQueryAsync();
            if(check != 0)
            {
                return HttpStatusCode.Created;
            }
            else
            {
                return HttpStatusCode.NotModified;
            }
        }
    }
}
