using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Data.Hexadecimal;
using Veneka.Indigo.UX.NativeAppAPI;

namespace Veneka.Indigo.DesktopApp.Device.PINPad
{
    public interface IPINPad : IDevice
    {
        /// <summary>
        /// Initialises the device. First method called
        /// </summary>
        void InitialisePinPad();

        /// <summary>
        /// After successful initialisation the Terminal PIN Key is normally set.
        /// </summary>
        /// <param name="tpk"></param>
        /// <returns></returns>
        PINPadResponse<string> SetTPK(HexString tpk);

        /// <summary>
        /// Implements functionality to ask the user to swipe or dip their card and return track2 data.
        /// </summary>
        /// <returns></returns>
        PINPadResponse<CardData> PresentCard(string displayText);

        /// <summary>
        /// Implements functionality to ask the user to enter a new PIN.
        /// </summary>
        /// <param name="cardData"></param>
        /// <param name="minPINLength"></param>
        /// <param name="maxPINLength"></param>
        /// <param name="displayMessage"></param>
        /// <returns></returns>
        PINPadResponse<CardData> GetNewPIN(CardData cardData, short minPINLength, short maxPINLength, string displayMessage);

        /// <summary>
        /// Implements functionality to ask the user to re-enter their new PIN to perform validation that both entered PIN are the same.
        /// </summary>
        /// <param name="cardData"></param>
        /// <param name="minPINLength"></param>
        /// <param name="maxPINLength"></param>
        /// <param name="displayMessage"></param>
        /// <returns></returns>
        PINPadResponse<CardData> GetNewPINConfirmation(CardData cardData, short minPINLength, short maxPINLength, string displayMessage);
        
        PINPadResponse<string> RemoveCard(string displayText);

        void DisplayText(string text);

        void DisplayHome();

        // Event for when thing happen?
    }
}
