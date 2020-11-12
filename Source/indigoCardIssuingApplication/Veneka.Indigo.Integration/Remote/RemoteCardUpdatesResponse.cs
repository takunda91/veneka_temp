using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Veneka.Indigo.Integration.Remote
{
    [DataContract]
    public class RemoteCardUpdatesResponse
    {
        public RemoteCardUpdatesResponse()
        {
            CardsResponse = new List<CardDetailResponse>();
        }

        [DataMember]
        public List<CardDetailResponse> CardsResponse { get; set; }
    }

    [DataContract]
    public class CardDetailResponse
    {
        [DataMember(IsRequired = true)]
        public long CardId { get; set; }

        [DataMember(IsRequired = true)]
        public DateTimeOffset TimeUpdated { get; set; }

        [DataMember(IsRequired = true)]
        public bool UpdateSuccessful { get; set; }

        [DataMember(IsRequired = true)]
        public string Detail { get; set; }
    }
}
