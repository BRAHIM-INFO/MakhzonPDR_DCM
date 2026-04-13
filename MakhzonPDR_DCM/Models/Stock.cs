using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakhzonPDR_DCM.Models
{
    public class Stock
    {
        public string REF { get; set; }
        public string INTITULE { get; set; }
        public string INTITULE2 { get; set; }
        public string INTITULE3 { get; set; }
        public double QTE { get; set; }
        public int en_Stock { get; set; }        
        public decimal PAMP { get; set; }
        public string CASIER { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
