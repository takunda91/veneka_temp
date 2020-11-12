using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration
{
    [InheritedExport(typeof(IFeeScheme))]
    public interface IFeeScheme: ICommon
    {
    }
}
