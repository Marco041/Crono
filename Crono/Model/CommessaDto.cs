using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crono.Model
{
    public class CommessaDto
    {
        public Commessa Comm { get; set; }
        public bool ReadOnlyMode { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public CommessaDto(Commessa c, bool readOnlyMode, DateTime? from=null, DateTime? to=null)
        {
            Comm = c;
            ReadOnlyMode = readOnlyMode;
            From = from!=null? from.Value : DateTime.Now;
            To = to!=null? to.Value: DateTime.Now;
        }
    }
}
