using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthConnectWeb.Models
{
    public class Medic : User
    {
        public int Id { get; set; }
        public string Nume { get; set; }
        public string Prenume { get; set; }
        public bool AddError { get; set; }
    }
}