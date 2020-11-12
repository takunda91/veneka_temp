using System.Collections.Generic;

namespace Veneka.Indigo.Renewal.Integration.Receive
{
    public interface IReceiveResponse
    {
        List<Entities.RenewalResponseDetail> ExtractFile(string sourceFolder);
    }
}
