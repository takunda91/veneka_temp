using System.Collections.Generic;

namespace Veneka.Indigo.Renewal.Integration.Send
{
    public interface IRenewalRequest
    {
        List<string> BuildFile(List<Entities.RenewalDetailListModel> details, string destinationFolder, string newFileName);
    }
}
