using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Common.Logging;

namespace Veneka.Indigo.Integration.M20Printing
{
    public class PrinterIntgerationController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PrinterIntgerationController));
        protected string EmbossName;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct LPSTEMBOSSERLINEDATA
        {
            public ushort posX;
            public ushort posY;
            public byte fontID;
            public byte spacingMode;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
            public byte[] lineData;
        };


        [DllImport("CPSE.dll")]
        public static extern byte CPSC1900EmbosserDownloadLineData(ref LPSTEMBOSSERLINEDATA stembosserlinedata);


        [DllImport("CPSE.dll", EntryPoint = "_CPSC1900Connect@8", CallingConvention = CallingConvention.StdCall)]
        public static extern byte CPSC1900Connect(byte port);

        [DllImport("CPSE.dll", EntryPoint = "_CPSC1900GetDLLRelease@0")]
        public static extern float CPSC1900GetDLLRelease();

        [DllImport("CPSE.dll", EntryPoint = "_CPSC1900EmbosserCleanLineData@0")]
        public static extern byte CPSC1900EmbosserCleanLineData();

        [DllImport("CPSE.dll", EntryPoint = "_CPSC1900DiaRunWithoutCard@4")]
        public static extern byte CPSC1900DiaRunWithoutCard(bool enabler);

        [DllImport("CPSE.dll", EntryPoint = "_CPSC1900FeedCard@0")]
        public static extern byte CPSC1900FeedCard();

        [DllImport("CPSE.dll", EntryPoint = "_CPSC1900EmbosserEmbossLines@4")]
        public static extern byte CPSC1900EmbosserEmbossLines(bool postToper);

        [DllImport("CPSE.dll", EntryPoint = "_CPSC1900TopperPressCard@8")]
        public static extern byte CPSC1900TopperPressCard(bool waitTempReady, byte waiting_time);

        [DllImport("CPSE.dll", EntryPoint = "_CPSC1900EjectCard@4")]
        public static extern byte CPSC1900EjectCard(byte ejectTo);



        [DllImport("CPSE.dll", EntryPoint = "_CPSC1900MakeDemoCard@4")]
        public static extern byte CPSC1900MakeDemoCard(bool MagStrip);

        public PrinterIntgerationController(string embossName)
        {
            this.EmbossName = embossName;
        }
        public bool print_M20()
        {
            byte port = 0x30;
            var createConnection = CPSC1900Connect(port);
           //   var createConnection = 0x00;
            try
            {
                if (createConnection == 0x00)
                {
                    //connected 
                    var currentVersion = CPSC1900GetDLLRelease();
                    //  log.Info("Welcome to Indigo M20 Print for Version {0} of CPS120 Printer SDK..", currentVersion);

                    log.Info("M20 :: Welcome to Indigo M20 Print for Version of CPS120 Printer SDK..Version .." + currentVersion);
                    log.Info("M20 :: Clearing printer...For Customer Details : " + this.EmbossName);

                    var clearPrinter = CPSC1900EmbosserCleanLineData();

                    if (clearPrinter == 0x00)
                    {
                        var nameToEmboss = this.EmbossName;
                        var embossLineThree = this.EmbossLineData(401, 332, 0x32, 0x30, nameToEmboss);
                        log.Info("M20 :: Name " + nameToEmboss + ", Embosser status is " + embossLineThree);

                        //run without card

                        var runWithoutCard = CPSC1900DiaRunWithoutCard(false);
                        log.Info("M20 :: Trial run without card with response set to false as : " + runWithoutCard);

                        var feedCard = CPSC1900FeedCard();
                        log.Info("M20 :: Card Feed, if in tray card will feed it self. Card feed response : " + feedCard);

                        //Embossing lines to card now - actual print - topping = true
                        log.Info("M20 :: Embossing lines to card now - actual print - topping = true");
                        var topper = true;
                        var embossFinalCard = CPSC1900EmbosserEmbossLines(topper);
                        log.Info("M20 :: Emboss line response : " + embossFinalCard);

                        var waitTemp = true;
                        var waitTimeInSec = (byte)20;
                        var pressCard = CPSC1900TopperPressCard(waitTemp, waitTimeInSec);
                        log.Info("M20 :: Card press after " + waitTimeInSec + " sec with press response : " + pressCard);

                        //eject card
                        var ejectCard = CPSC1900EjectCard(0);
                        log.Info("M20 :: Eject card with status : " + ejectCard);
                        return true;
                    }

                    else
                    {
                        log.Error("M20 :: Failed to clear printer with status " + clearPrinter);
                        return false;
                    }
                }
                else
                {

                    log.Error("M20 :: Failed to connect with status " + createConnection);
                    return false;
                }
            }
            catch (Exception e)
            {
                log.Error("M20 Exception " + e.ToString());
                return false;
            }



        }


        public byte EmbossLineData(UInt16 aPosX, UInt16 aPosY, byte aFontID, byte aSpacingMode, string emboss_string)
        {

            LPSTEMBOSSERLINEDATA stembosserlinedataOne = new LPSTEMBOSSERLINEDATA();
            var byteArray = Encoding.ASCII.GetBytes(emboss_string);


            stembosserlinedataOne.lineData = new byte[40];

            stembosserlinedataOne.posX = aPosX;
            stembosserlinedataOne.posY = aPosY;
            stembosserlinedataOne.fontID = aFontID; //#define CPSC1900_EMBOSSER_FONT_FARRINGTON7B		0x31	//  7 xIn Front
            stembosserlinedataOne.spacingMode = aSpacingMode;//#define CPSC1900_EMBOSSER_SPACE_STD				0x30

            Buffer.BlockCopy(byteArray, 0, stembosserlinedataOne.lineData, 0, byteArray.Length);

            return CPSC1900EmbosserDownloadLineData(ref stembosserlinedataOne);
        }
    }
}
