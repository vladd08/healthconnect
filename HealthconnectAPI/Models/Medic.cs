﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthconnectAPI.Models
{
    public class Medic
    {
        public int Id { get; set; }
        public string Nume { get; set; }
        public string Prenume { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}