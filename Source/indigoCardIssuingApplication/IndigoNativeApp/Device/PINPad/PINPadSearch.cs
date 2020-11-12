using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.DesktopApp.Device.PINPad
{
    public class PINPadSearch
    {
        public PINPadSearch()
        {
            //Add PIN Pad's to be included in the search
            pinPadSearches = new FindPINPads[]
            {
                PAXS300.PaxS300.ListConnectedDevices
            };
        }

        public delegate IPINPad[] FindPINPads();

        FindPINPads[] pinPadSearches;

        public IPINPad[] SearchForConnectedPINPads()
        {
            List<IPINPad> pinPads = new List<IPINPad>();

            foreach (FindPINPads search in pinPadSearches)
            {
                try
                {
                    pinPads.AddRange(search());
                }
                catch(Exception ex)
                {
                    //TODO : log exeption
                }
            }

            return pinPads.ToArray();
        }
    }
}

