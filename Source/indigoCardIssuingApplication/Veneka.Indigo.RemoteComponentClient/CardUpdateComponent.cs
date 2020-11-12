using Common.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.Remote;
using Veneka.Indigo.RemoteComponentClient.BLL;
using Veneka.Indigo.RemoteComponentClient.Configuration;

namespace Veneka.Indigo.RemoteComponentClient
{
    public class CardUpdateComponent : IDisposable
    {
        private const string INDIGO_ENDPOINT = "IndigoRemoteComponentEndPoint";

        private static readonly ILog _log = LogManager.GetLogger(typeof(CardUpdateComponent));
        private readonly IRemoteComponent _proxy;
        private readonly IntegrationController _integration;

        private bool _allowUntrustedSSL = false;

        public CardUpdateComponent()
        {
            ChannelFactory<IRemoteComponent> factory = new ChannelFactory<IRemoteComponent>(INDIGO_ENDPOINT);
            _proxy = factory.CreateChannel();
            _integration = new IntegrationController(new System.IO.DirectoryInfo(Path.Combine(ConfigReader.ApplicationConfigPath.FullName, "integration")));

            _allowUntrustedSSL = ConfigReader.AllowUntrustedSSL;
        }

        public CardUpdateComponent(IRemoteComponent remoteComponent, ComposablePartCatalog catalog)
        {
            _proxy = remoteComponent;
            _integration = new IntegrationController(catalog);
        }

        public void ProcessCardUpdates()
        {
            try
            {
                var tokens = ConfigReader.GetRemoteTokens();

                if (tokens == null || tokens.Count == 0)
                {
                    _log.InfoFormat("No remote tokens to process card updates for. Please check config file has been configured correctly.");
                    return;
                }

                _log.InfoFormat("Processing card updates for {0} remote tokens.", tokens.Count);


                for (int i = 0; i < tokens.Count; i++)
                {
                    _log.InfoFormat("Processing Card Updates for token at position {0}", i);
                    _log.Debug(d => d("Position " + i + " token is " + tokens[i]));

                    if (_allowUntrustedSSL)
                        IgnoreUntrustedSSL();

                    var cardUpdates = _proxy.GetPendingCardUpdates(tokens[i]);

                    if (cardUpdates.Response.ResponseCode == 0)
                    {
                        List<CardDetailResponse> responseCards = new List<CardDetailResponse>();

                        //Loop through each remote cms product setting and call integration to do the card updates
                        foreach (var productSetting in cardUpdates.ProductSettings.Where(w => w.IntegrationTypeId == 9) ?? new List<Settings>())
                        {
                            try
                            {
                                bool doUpdate = true;
                                string responseMessage = String.Empty;
                                //fetch any details for the primary CMS for the same product
                                var primaryCms = cardUpdates.ProductSettings.Where(w => w.ProductId == productSetting.ProductId && w.IntegrationTypeId == 1).FirstOrDefault();
                                if (primaryCms != null)
                                {
                                    //Cards that werent updated successfuly in the integration layer
                                    List<CardDetailResponse> failedCards;

                                    if (!_integration
                                        .CardManagementSystem(primaryCms.IntegrationGuid)
                                        .RemoteFetchDetails(cardUpdates.Cards.Where(w => w.product_id == productSetting.ProductId).ToList(),
                                                                primaryCms.ExternalFields, ExtractConfig(primaryCms), out failedCards, out responseMessage))
                                    {
                                        _log.Error(responseMessage);
                                        doUpdate = false;
                                    }
                                    else if (failedCards != null && failedCards.Count > 0)
                                    {
                                        _log.Trace(t => t("Failed cards count after EMP call: " + failedCards.Count));
                                        //add the failed cards to response and remove them from cards to be updated
                                        responseCards.AddRange(failedCards);
                                    }
                                }

                                if (doUpdate)
                                {
                                    //Now update the remote CMS
                                    responseCards.AddRange(_integration
                                                            .RemoteCardManagementSystem(productSetting.IntegrationGuid)
                                                            .UpdateCards(cardUpdates.Cards.Where(w => w.product_id == productSetting.ProductId && !responseCards.Any(a => a.CardId == w.card_id)).ToList(),
                                                                         productSetting.ExternalFields,
                                                                         ExtractConfig(productSetting)));
                                }
                                else
                                {
                                    foreach (var failedCard in cardUpdates.Cards.Where(w => w.product_id == productSetting.ProductId).ToList())
                                    {
                                        responseCards.Add(new CardDetailResponse
                                        {
                                            CardId = failedCard.card_id,
                                            Detail = responseMessage,
                                            TimeUpdated = DateTimeOffset.Now,
                                            UpdateSuccessful = false
                                        });
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _log.Error(ex);

                                foreach (var failedCard in cardUpdates.Cards.Where(w => w.product_id == productSetting.ProductId).ToList())
                                {
                                    responseCards.Add(new CardDetailResponse
                                    {
                                        CardId = failedCard.card_id,
                                        Detail = ex.Message,
                                        TimeUpdated = DateTimeOffset.Now,
                                        UpdateSuccessful = false
                                    });
                                }
                            }
                        }

                        //check to see if we've updated all the cards. If not then the product settings for the card is missing in the request
                        foreach (var missedCard in cardUpdates.Cards.Where(p => !responseCards.Any(p2 => p2.CardId == p.card_id)))
                        {
                            responseCards.Add(new CardDetailResponse
                            {
                                CardId = missedCard.card_id,
                                Detail = "Product settings missing for card.",
                                TimeUpdated = DateTimeOffset.Now,
                                UpdateSuccessful = false
                            });
                        }


                        //Tell Indigo how the card updates faired
                        var updateResponse = _proxy.CardUpdateResults(tokens[i], new RemoteCardUpdatesResponse { CardsResponse = responseCards });

                        if (updateResponse.ResponseCode != 0)
                        {
                            _log.ErrorFormat("Failed to update cards, response received {0} - {1}", updateResponse.ResponseCode, updateResponse.ResponseMessage);
                        }
                        else
                            _log.Trace(t => t("Cards updated successfully to Indigo."));
                    }
                    else
                    {
                        _log.ErrorFormat("Failed to fetch update cards, response received {0} - {1}", cardUpdates.Response.ResponseCode, cardUpdates.Response.ResponseMessage);
                    }

                    _log.InfoFormat("Done processing Card Updates for token at position {0}", i);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        private static Integration.Config.IConfig ExtractConfig(Settings productSetting)
        {
            Integration.Config.IConfig config;

            switch ((Integration.Config.ConfigFactory.ConfigType)productSetting.ConfigType)
            {
                case Integration.Config.ConfigFactory.ConfigType.WebService: config = productSetting.WebServiceSettings; break;
                case Integration.Config.ConfigFactory.ConfigType.FileSystem: config = productSetting.FileSystemSettings; break;
                case Integration.Config.ConfigFactory.ConfigType.Socket: config = productSetting.SocketSettings; break;
                default: throw new Exception("Unknown or Unsported config type = " + productSetting.ConfigType);
            }

            

            return config;
        }

        private void IgnoreUntrustedSSL()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)3072 | SecurityProtocolType.Ssl3;

            ServicePointManager.ServerCertificateValidationCallback = (object s, X509Certificate certificate,
                                                                        X509Chain chain,
                                                                        SslPolicyErrors sslPolicyErrors) => true;
        }

        public void Dispose()
        {
            _integration.Dispose();
        }

        //private void setHeader()
        //{
        //    // create channel factory / proxy ...
        //    using (OperationContextScope scope = new OperationContextScope(_proxy))
        //    {
        //        OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = new HttpRequestMessageProperty()
        //        {
        //            Headers =
        //{
        //    { "MyCustomHeader", Environment.UserName },
        //    { HttpRequestHeader.UserAgent, "My Custom Agent"}
        //}
        //        };
        //        // perform proxy operations... 
        //    }
        //}
    }
}
