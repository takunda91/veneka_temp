using System.Collections.Generic;
using indigoCardIssuingWeb.Old_App_Code.CCO.objects;

namespace indigoCardIssuingWeb.CCO.objects
{
    public class CardSearchResults
    {
        public List<IssueCard> CardsList { get; set; }

        public IssueCard Batch { get; set; }
    }
}