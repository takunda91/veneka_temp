using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Veneka.Indigo.Common.Models;

namespace Veneka.Indigo.CardManagement.objects
{
    public class FeeSchemeDetails : FeeSchemeResult
    {
        public FeeSchemeDetails()
        {

        }

        public FeeSchemeDetails(FeeSchemeResult feeSchemeResult)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            PropertyInfo[] properties = base.GetType().GetProperties(flags);

            foreach(var sourceProperty in feeSchemeResult.GetType().GetProperties(flags))
            {
                var propertyInfo = properties.Where(w => w.Name.Trim().ToUpper() == sourceProperty.Name.Trim().ToUpper())
                                             .First();

                if ((propertyInfo != null) && propertyInfo.CanWrite)
                {
                    if (propertyInfo.PropertyType.IsEnum)
                        propertyInfo.SetValue(this, Enum.Parse(propertyInfo.PropertyType, sourceProperty.GetValue(feeSchemeResult, null).ToString()), null);
                    else
                        propertyInfo.SetValue(this, ChangeType(sourceProperty.GetValue(feeSchemeResult, null), propertyInfo.PropertyType), null);
                }
            }
        }

        public List<FeeDetailResult> FeeDetails { get; set; }

        private static object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }
    }
}
