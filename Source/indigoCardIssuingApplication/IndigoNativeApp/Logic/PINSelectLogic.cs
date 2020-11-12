using IndigoDesktopApp;
using IndigoDesktopApp.NativeAppAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.DesktopApp.Device.PINPad;
using Veneka.Indigo.UX.NativeAppAPI;

namespace Veneka.Indigo.DesktopApp.Logic
{
    public delegate void UiUpdateEventHandler(object sender, string message, bool isError, bool append, EventArgs e);

    public class PINSelectLogic
    {
        private readonly IPINOperations _pinOperations;
        private readonly IPINPad _pinPad;

        public event UiUpdateEventHandler UiUpdate;        

        public PINSelectLogic(IPINOperations pinOperations, IPINPad pinPad)
        {
            _pinOperations = pinOperations;
            _pinPad = pinPad;
        }

        protected virtual void OnUiUpdate(string message, bool isError, bool append, EventArgs e)
        {
            UiUpdate?.Invoke(this, message, isError, append, e);
        }


        public void DoPINSelect()
        {
            // COMP1 DA52 75B0 3808 D031 E5D5 8394 86BF 5489
            // COMP2 8664 2349 0B62 8F4C 64E0 0725 577C 9EEA
            // COMP3 4338 E5B5 AB40 92CE 5D91 0E4C 4662 2A13

            //TEST TPK: X6A3FDB0309D54300E4D15526A1547454    
            //          UD019BA0D484B9968CB186F183B843620
            //          TMK (clear): U5D3757F8326B5E7C803485B0D0C2CB62
            //          New key (clear): 31BAAEA801CD0EADB9DAFED0CDF240E6
            //          New key(TMK): X6A3FDB0309D54300E4D15526A1547454
            //           New key(LMK): UD019BA0D484B9968CB186F183B843620

            try
            {
                OnUiUpdate("Initialise Device", false, true, EventArgs.Empty);                

                _pinPad.InitialisePinPad();

                var token = new Token
                {
                    DeviceID = _pinPad.DeviceId,
                    Session = StartupProperties.Token,
                    Workstation = Environment.MachineName
                };

                // *** Get & Set Working Keys ***
                if (!SetWorkingKeys(token))
                    return;

                // *** Get Card Data ***
                if (!GetCardData(token, out CardData cardData, out ProductSettings productSettings))
                    return;

               
                bool complete = false;
                bool success = true;
                string additionalInfo = String.Empty;
                PINPadResponse<CardData> newPinData = null;
                PINPadResponse<CardData> confirmPinData = null;

                for(int step = 0; complete == false && success ==  true; step++)
                {
                    switch (step)
                    {
                        case 0: // First pin entry
                            OnUiUpdate("Enter PIN", false, true, EventArgs.Empty);
                            newPinData = _pinPad.GetNewPIN(cardData, Convert.ToInt16(productSettings.MinPINLength), Convert.ToInt16(productSettings.MaxPINLength), "Enter PIN");
                            success = newPinData.Success;
                            additionalInfo = newPinData.AdditionalInfo;
                            break;
                        case 1: // Confirmation pin entry
                            OnUiUpdate("Confirm PIN", false, true, EventArgs.Empty);
                            confirmPinData = _pinPad.GetNewPIN(cardData, Convert.ToInt16(productSettings.MinPINLength), Convert.ToInt16(productSettings.MaxPINLength), "Confirm PIN");
                            success = confirmPinData.Success;
                            additionalInfo = confirmPinData.AdditionalInfo;
                            break;
                        case 2: // validate pin entered
                            if(newPinData.Value.PINBlock.Equals(confirmPinData.Value.PINBlock, StringComparison.InvariantCultureIgnoreCase) &&
                                newPinData.Value.PINBlockFormat == confirmPinData.Value.PINBlockFormat)
                            {
                                // update the original cardData object
                                cardData.PINBlock = newPinData.Value.PINBlock;
                                cardData.PINBlockFormat = newPinData.Value.PINBlockFormat;
                                success = true;
                            }
                            else
                            {
                                success = false;                                
                                additionalInfo = "PIN and Confirmation PIN do not match";
                            }
                            break;
                        default: complete = true; break;
                    }
                }

                if (cardData.CardInterface == CardInterfaces.Chip_EMV)
                {
                    OnUiUpdate("Remove Card", false, true, EventArgs.Empty);
                    var removeResp = _pinPad.RemoveCard("Remove Card");
                }

                if (success)
                {
                    var completeResp = _pinOperations.Complete(cardData, token);

                    if (!completeResp.Success)
                    {
                        OnUiUpdate(completeResp.AdditionalInfo, false, true, EventArgs.Empty);
                    }
                    else
                    {
                        _pinPad.DisplayText("Complete");
                        OnUiUpdate("PIN Select Complete", false, true, EventArgs.Empty);
                    }
                }
                else
                {
                    _pinPad.DisplayText(additionalInfo);
                    OnUiUpdate(additionalInfo, false, true, EventArgs.Empty);
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException enfex)
            {
                OnUiUpdate(enfex.Message, false, true, EventArgs.Empty);
                //Console.WriteLine(enfex);
            }
            catch 
            {
                throw;
                //OnUiUpdate(String.Format("{0}{1}{2}{3}{4}", _pinPad.DllPath, Environment.NewLine,ex.Message, Environment.NewLine, ex.ToString()), EventArgs.Empty);
                //Console.WriteLine(ex);
            }
            finally
            {
                _pinPad.DisplayHome();
                _pinPad.Dispose();
            }
        }    


        private bool SetWorkingKeys(Token token)
        {
            OnUiUpdate("Setting working keys for Device", false, true, EventArgs.Empty);
            var workingKeysResp = _pinOperations.GetWorkingKey(token);

            if (!workingKeysResp.Success)
            {
                OnUiUpdate(workingKeysResp.AdditionalInfo, false, true, EventArgs.Empty);
                return false;
            }

            //if(workingKeysResp.Value[0].StartsWith())

            var resp = _pinPad.SetTPK(workingKeysResp.Value[0].TrimStart('X', 'U', 'T', 'Y'));

            if (!resp.Success)
            {
                OnUiUpdate(resp.AdditionalInfo, false, true, EventArgs.Empty);
                return false;
            }

            return true;
        }

        private bool GetCardData(Token token, out CardData cardData, out ProductSettings productSettings)
        {
            productSettings = null;
            cardData = null;

            OnUiUpdate("Present Card", false, true, EventArgs.Empty);

            // Call PIN Pad for card data
            var cardDataResp = _pinPad.PresentCard("Present Card");

            if (!cardDataResp.Success)
            {
                OnUiUpdate(cardDataResp.AdditionalInfo, false, true, EventArgs.Empty);
                return false;
            }

            OnUiUpdate("Checking card product", false, true, EventArgs.Empty);
            // Call Indigo to validate the product and get any specific settings for the product
            var productConfigResp = _pinOperations.GetProductConfig(cardDataResp.Value, token);

            if (!productConfigResp.Success)
            {
                OnUiUpdate(productConfigResp.AdditionalInfo, false, true, EventArgs.Empty);
                return false;
            }

            cardData = cardDataResp.Value;

            productSettings = productConfigResp.Value;
            cardData.ProductId = productSettings.ProductId;

            return true;
        }
    }
}
