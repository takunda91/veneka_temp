using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Common.Objects
{
    /// <summary>
    /// This class is used to communicate results back to the calling code.
    /// </summary>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class ResultObject<C, T>
    {
        
        public C Code { get; set; }
        public string Message { get; set; }
        public T Tag { get; set; }

        public ResultObject()
        {
            //Code = 100;
            Message = "[NOT SET]";
        }// end ctor ResultObject()

    }// end class ResultObject
}// end namespace IndigoCardIssuenceCommon.Models
