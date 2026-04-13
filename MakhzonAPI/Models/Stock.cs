namespace MakhzonAPI.Models
{
    public class Stock
    {
        public string REF { get; set; }
        public string INTITULE { get; set; }
        public string INTITULE2 { get; set; }
        public string INTITULE3 { get; set; }
        public string FAMILLE { get; set; }
        public double QTE { get; set; }
        public int en_Stock { get; set; }
        public double PAMP { get; set; }
        public string CASIER { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
