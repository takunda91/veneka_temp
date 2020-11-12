using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Common.Models.IssuerManagement
{
	[Serializable]
	public class RestWebservicesHandler
	{
		public int issuer_id { get; set; }
		public string webservices_type { get; set; }
		public string rest_url { get; set; }
		public string param_one_name { get; set; }
		public string param_one_value { get; set; }
		public string param_two_name { get; set; }
		public string param_two_value { get; set; }
		public string param_three_name { get; set; }
		public string param_three_value { get; set; }
		public string param_four_name { get; set; }
		public string param_four_value { get; set; }
		public string param_five_name { get; set; }
		public string param_five_value { get; set; }
		public string param_six_name { get; set; }
		public string param_six_value { get; set; }
		public string param_seven_name { get; set; }
		public string param_seven_value { get; set; }
		public string param_eight_name { get; set; }
		public string param_eight_value { get; set; }
		public string param_nine_name { get; set; }
		public string param_nine_value { get; set; }
		public string param_ten_name { get; set; }
		public string param_ten_value { get; set; }
		public int webservice_params_id {get; set;}
    }
}
