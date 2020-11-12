using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Integration.ProductPrinting;

namespace Veneka.Indigo.CardManagement.objects
{
 public   class RequestDetails  : RequestDetailResult
    {
        public RequestDetails() { }
        

        public RequestDetails(RequestDetailResult cardDetailResult)
        {

        }

        public List<ProductField> ProductFields { get; set; }


        public static TConvert ConvertTo<TConvert>(object entity) where TConvert : new()
        {
            var convertProperties = TypeDescriptor.GetProperties(typeof(TConvert)).Cast<PropertyDescriptor>();
            var entityProperties = TypeDescriptor.GetProperties(entity).Cast<PropertyDescriptor>();

            var convert = new TConvert();

            foreach (var entityProperty in entityProperties)
            {
                var property = entityProperty;
                var convertProperty = convertProperties.FirstOrDefault(prop => prop.Name == property.Name);
                if (convertProperty != null)
                {
                    //convertProperty.SetValue(convert, Convert.ChangeType(entityProperty.GetValue(entity), convertProperty.PropertyType));
                    convertProperty.SetValue(convert, entityProperty.GetValue(entity));
                }
            }

            return convert;
        }
    }
}
