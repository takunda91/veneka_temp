using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.External;
using Veneka.Indigo.Integration.Remote;

namespace IndigoAppTesting.MockIntegration
{
    [IntegrationExport(Name, GUID, typeof(IRemoteCMS))]
    public class MockRemoteCMS : IRemoteCMS
    {
        public const string GUID = "830ED210-BB84-4F8C-9305-A410F1F93264";
        public const string Name = "MockRemoteCMS";

        public DirectoryInfo ApplicationDirectory
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public List<CardDetailResponse> UpdateCards(List<CardDetail> cards, ExternalSystemFields externalFields, IConfig config)
        {
            List<CardDetailResponse> updated = new List<CardDetailResponse>();

            foreach (var card in cards)
            {
                if (card.product_id == 99)
                    throw new Exception("Some exception");

                updated.Add(new CardDetailResponse
                {
                    CardId = card.card_id,
                    Detail = "Success",
                    TimeUpdated = DateTime.Now,
                    UpdateSuccessful = true
                });
            }

            return updated;
        }
    }    
}
