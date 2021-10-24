using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crono.Model
{
    public class Commessa
    {
        public string Codice { get; set; }
        public DateTime DataRegistrazione { get; set; }
        public string Cantiere { get; set; }
        public string Tecnico { get; set; }
        public bool Manutenzione { get; set; }
        public bool Chiusa { get; set; }
        public bool Intervento { get; set; }
    }
}
