namespace Veneka.Indigo.Common.Models
{
    public class CurrencyResult
    {
        public int currency_id { get; set; }
        public string currency_code { get; set; }
        public string iso_4217_numeric_code { get; set; }
        public int? iso_4217_minor_unit { get; set; }
        public string currency_desc { get; set; }
        public bool active_YN { get; set; }
    }
}
