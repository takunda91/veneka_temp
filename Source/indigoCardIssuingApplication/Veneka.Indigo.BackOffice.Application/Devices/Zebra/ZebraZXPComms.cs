using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZMOTIFPRINTERLib;

namespace Veneka.Indigo.BackOffice.Application.Devices.Zebra
{
    internal class ZebraZXPComms
    {
        #region ZMotif Device Connect

        // Connects to a ZMotif device
        // --------------------------------------------------------------------------------------------------

        internal static bool Connect(string deviceName, ref Job j)
        {
            bool bRet = true;


            if (j == null)
                return false;

            if (!j.IsOpen)
            {
                var alarm = j.Open(deviceName);

                //IdentifyZMotifPrinter(ref j);
            }

            return bRet;
        }

        // Disconnects from a ZMotif device
        // --------------------------------------------------------------------------------------------------

        internal static bool Disconnect(ref Job j)
        {
            bool bRet = true;

            try
            {
                if (j == null)
                    return false;

                if (j.IsOpen)
                {
                    j.Close();

                    do
                    {
                        Thread.Sleep(10);
                    } while (Marshal.FinalReleaseComObject(j) != 0);
                }
            }
            catch
            {
                bRet = false;
                throw;
            }
            finally
            {
                j = null;
                GC.Collect();
            }
            return bRet;
        }
        #endregion
    }
}
