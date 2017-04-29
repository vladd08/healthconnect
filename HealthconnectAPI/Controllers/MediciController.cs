using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HealthconnectAPI.Models;
using System.Data.SqlClient;

namespace HealthconnectAPI.Controllers
{
    public class MediciController : ApiController
    {
        [HttpGet]
        [Route("api/Medic/{id}")]
        public Medic GetMedic(int id)
        {
            SqlDataReader reader = null;
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = @"Server=tcp:softmed.database.windows.net,1433;Initial Catalog=SoftmedTest;Persist Security Info=False;User ID=vlad.orbulescu;Password=Par0la01;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = System.Data.CommandType.Text;
            sqlCmd.Parameters.AddWithValue("@Id", id);
            sqlCmd.CommandText = "SELECT * FROM Medici WHERE Id = @Id";
            sqlCmd.Connection = sqlConnection;
            sqlConnection.Open();
            reader = sqlCmd.ExecuteReader();
            Medic medic = new Medic();
            while (reader.Read())
            {
                medic.Id = Convert.ToInt32(reader.GetValue(0));
                medic.Nume = reader.GetValue(1).ToString();
                medic.Prenume = reader.GetValue(2).ToString();
                medic.Username = reader.GetValue(3).ToString();
                medic.Password = reader.GetValue(4).ToString();
            }

            return medic;
        }
    }
}
