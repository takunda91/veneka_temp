using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Models;

namespace Veneka.Indigo.CardManagement.objects
{
    public sealed class ExternalSystemFieldResult
    {
        public ExternalSystemsResult ExternalSystem { get; set; }
        public List<ExternalSystemsResult> ExternalSystems { get; set; }

        public List<ExternalSystemFieldsResult> ExternalSystemFields { get; set; }
    }
}
