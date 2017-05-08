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
    public class MediciController : ApiController
    {
        private string connectionString = @"Server=tcp:softmed.database.windows.net,1433;Initial Catalog=SoftmedTest;Persist Security Info=False;User ID=vlad.orbulescu;Password=Par0la01;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        [HttpPost]
        [Route("api/Medici/Medic")]
        public async System.Threading.Tasks.Task<HttpStatusCode> AddMedic([FromBody]Medic medic)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = System.Data.CommandType.Text;
            sqlCmd.Connection = sqlConnection;
            sqlCmd.Parameters.AddWithValue("@Id", medic.Id);
            sqlCmd.Parameters.AddWithValue("@Nume", medic.Nume);
            sqlCmd.Parameters.AddWithValue("@Prenume", medic.Prenume);
            sqlCmd.Parameters.AddWithValue("@Username", medic.Username);
            sqlCmd.Parameters.AddWithValue("@Password", medic.Password);
            sqlCmd.CommandText = "INSERT INTO Medici (Id, Nume, Prenume, Username, Password) VALUES(@Id, @Nume, @Prenume, @Username, @Password)";
            sqlCmd.Connection = sqlConnection;
            sqlConnection.Open();
            var check = await sqlCmd.ExecuteNonQueryAsync();
            if (check != 0)
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
