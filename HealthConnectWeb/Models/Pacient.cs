using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthConnectWeb.Models
{
    public class Pacient
    {
        public string Cnp { get; set; }
        public string Nume { get; set; }
        public string Prenume { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Varsta { get; set; }
        public string Localitate { get; set; }
        public bool AddError { get; set; }
        public string Strada { get; set; }
        public int NrStrada { get; set; }
        public string NrTelefon { get; set; }
        public string Email { get; set; }
        public string Profesie { get; set; }
        public string LocMunca { get; set; }

    }
}