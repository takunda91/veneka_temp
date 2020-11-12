// Decompiled with JetBrains decompiler
// Type: GPT.PINPadAPI
// Assembly: PINPadAPI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8D48DA08-B04F-4D45-ADBA-8943595A7A52
// Assembly location: C:\VenekaOne\OneDrive - Veneka 2\Suppliers\GPT PAX\VenekaPPTest\VenekaPPTest\PinPadTest\bin\Debug\PINPadAPI.dll

using System;
using System.Text;

namespace GPT
{
    public class PINPadAPI
    {
        public static string PINPadAPI_CardType_ICC = "2";
        public static string PINPadAPI_CardType_MAG = "1";
        public static string PINPadAPI_CardType_MAGICC = "3";
        public static int PINPadAPI_Major_Version = 1;
        public static int PINPadAPI_Minor_Version = 0;
        public static string PINPadAPI_BluidNumber = "0101";
        public static string PINPadAPI_BluidDate = "18/11/2014";
        private spdh PINPadAPI_SPDHLayer;
        private string PINPadAPI_TerminalID;
        private bool PINPadAPI_TraceOn;

        private void SetTracingState(bool State)
        {
            this.PINPadAPI_TraceOn = State;
        }

        private int GetLastError(string[] Message, string[] File, string[] Line, string[] Pos, string[] ErrorVal)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.Prepare("123456", (byte)65, (byte)79, 19, false);
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.Send();
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.Receive(false);
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)66, (byte)97, Message);
                        break;
                    case 5:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)66, (byte)98, File);
                        break;
                    case 6:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)66, (byte)99, Pos);
                        break;
                    case 7:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)66, (byte)100, ErrorVal);
                        break;
                    case 8:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)66, (byte)101, Line);
                        break;
                    case 9:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        private void DisplayError()
        {
            int num1 = 0;
            bool flag = false;
            string[] Message = new string[1];
            string[] File = new string[1];
            string[] Line = new string[1];
            string[] Pos = new string[1];
            string[] ErrorVal = new string[1];
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.GetLastError(Message, File, Line, Pos, ErrorVal);
                        break;
                    case 1:
                        Console.Out.WriteLine("LAST ERROR DETAILS");
                        Console.Out.WriteLine("   Message  :" + Message[0]);
                        Console.Out.WriteLine("   File     :" + File[0]);
                        Console.Out.WriteLine("   Line     :" + Line[0]);
                        Console.Out.WriteLine("   Pos      :" + Pos[0]);
                        Console.Out.WriteLine("   ErrorVal :" + ErrorVal[0]);
                        break;
                    case 2:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
        }

        private void DisplayTran(byte TranType, byte TranMode, int TranID)
        {
            if (!this.PINPadAPI_TraceOn)
                return;
            StringBuilder[] stringBuilderArray = new StringBuilder[1];
            stringBuilderArray[0].Append((char)TranType);
            stringBuilderArray[0].Append((char)TranMode);
            stringBuilderArray[0].Append(string.Concat((object)TranID));
            string str = stringBuilderArray[0].ToString();
            this.SendDebugData(this.PINPadAPI_TerminalID + ">> Transaction = " + (!(str == "AO01") ? (!(str == "AO02") ? (!(str == "AO03") ? (!(str == "DO01") ? (!(str == "DO03") ? (!(str == "DO02") ? (!(str == "DO11") ? (!(str == "DO04") ? (!(str == "AO99") ? (!(str == "AO96") ? (!(str == "AO12") ? (!(str == "AO19") ? (!(str == "AO20") ? (!(str == "AO05") ? (!(str == "FO01") ? (!(str == "FO02") ? (!(str == "FO03") ? (!(str == "AO04") ? (!(str == "DO05") ? (!(str == "DO06") ? (!(str == "DO07") ? (!(str == "DO09") ? (!(str == "DO12") ? (!(str == "DO13") ? (!(str == "DO14") ? (!(str == "DO15") ? (!(str == "DO16") ? (!(str == "DO17") ? (!(str == "AO98") ? (!(str == "AO97") ? "Unknown Transaction" : "SetTestDataState") : "SetTestData") : "MaskInput") : "KeyCode") : "NumericEntry") : "SelectMenu") : "AddMenu") : "ClearMenu") : "SendToPrinter") : "SetDebugPort") : "SetDebugState OFF") : "SetDebugState ON") : "GetSerialNumber") : "PollCard") : "GetTrackData") : "WaitForCard") : "GetAndSetVersion") : "ChangeDeviceComms") : "GetLastError") : "GetSessionKey") : "RebootTerminal") : "InjectMasterKey") : "GetAmount") : "Beep") : "Display") : "StopScreen") : "StartScreen") : "GetPIN") : "SetSessionKey") : "InitialiseDUKPT") + " " + str);
        }

        private int Transact(byte TranType, byte TranMode, int TranID, bool LastTransaction)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        this.DisplayTran(TranType, TranMode, TranID);
                        num1 = this.PINPadAPI_SPDHLayer.Prepare(this.PINPadAPI_TerminalID, TranType, TranMode, TranID, LastTransaction);
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.Send();
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.Receive(false);
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.GetLastRespCode();
                        if (num1 != 0)
                        {
                            this.DisplayError();
                            break;
                        }
                        break;
                    case 4:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int GetAPIVersionNumber(out string BuildNumber, out string BuildDate)
        {            
            BuildNumber = PINPadAPI.PINPadAPI_BluidNumber;
            BuildDate = PINPadAPI.PINPadAPI_BluidDate;
            return PINPadAPI.PINPadAPI_Major_Version * 100 + PINPadAPI.PINPadAPI_Minor_Version;
        }

        public int StartScreen()
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.Transact((byte)68, (byte)79, 1, false);
                        break;
                    case 2:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int ClearMenu()
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.Transact((byte)68, (byte)79, 12, false);
                        break;
                    case 2:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int AddMenu(string MenuText, int EntryID)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)99, MenuText);
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)106, EntryID.ToString());
                        break;
                    case 3:
                        num1 = this.Transact((byte)68, (byte)79, 13, false);
                        break;
                    case 4:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int SelectMenu(string TitleText, int Timeout, string[] OptionSelected)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)99, TitleText);
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)116, Timeout.ToString());
                        break;
                    case 3:
                        num1 = this.Transact((byte)68, (byte)79, 14, false);
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)50, (byte)101, OptionSelected);
                        break;
                    case 5:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int StopScreen()
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.Transact((byte)68, (byte)79, 3, false);
                        break;
                    case 2:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int ChangeDeviceComms()
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.Transact((byte)65, (byte)79, 20, false);
                        break;
                    case 2:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int Display(int Row, int Col, string DisplayText)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)97, Row.ToString());
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)98, Col.ToString());
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)99, DisplayText);
                        break;
                    case 4:
                        num1 = this.Transact((byte)68, (byte)79, 2, false);
                        break;
                    case 5:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int Display(int Row, int Col, string DisplayText, int Font)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)97, Row.ToString());
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)98, Col.ToString());
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)108, Font.ToString());
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)99, DisplayText);
                        break;
                    case 5:
                        num1 = this.Transact((byte)68, (byte)79, 2, false);
                        break;
                    case 6:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int Display(int Row, int Col, string DisplayID, int Font, int Flags)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)97, Row.ToString());
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)98, Col.ToString());
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)108, Font.ToString());
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)109, Flags.ToString());
                        break;
                    case 5:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)107, DisplayID);
                        break;
                    case 6:
                        num1 = this.Transact((byte)68, (byte)79, 2, false);
                        break;
                    case 7:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int Display(int Row, int Col, byte[] DisplayText, int Font, int Flags)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)97, Row.ToString());
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)98, Col.ToString());
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)108, Font.ToString());
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)109, Flags.ToString());
                        break;
                    case 5:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)99, Encoding.ASCII.GetString(DisplayText));
                        break;
                    case 6:
                        num1 = this.Transact((byte)68, (byte)79, 2, false);
                        break;
                    case 7:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int NumericEntry(int Row, int Col, int Length, int Timeout, string[] Entered)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)97, Row.ToString());
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)98, Col.ToString());
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)102, Length.ToString());
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)116, Timeout.ToString());
                        break;
                    case 5:
                        num1 = this.Transact((byte)68, (byte)79, 15, false);
                        break;
                    case 6:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)50, (byte)97, Entered);
                        break;
                    case 7:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int KeyCode(int Row, int Col, int MinLen, int MaxLen, int Timeout, string[] Entered)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)97, Row.ToString());
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)98, Col.ToString());
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)116, Timeout.ToString());
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)102, MinLen.ToString());
                        break;
                    case 5:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)103, MaxLen.ToString());
                        break;
                    case 6:
                        num1 = this.Transact((byte)68, (byte)79, 16, false);
                        break;
                    case 7:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)57, (byte)101, Entered);
                        break;
                    case 8:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int MaskInput(int Row, int Col, string Mask, string Template, int Timeout, string[] RawString, string[] Formatted)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)97, Row.ToString());
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)98, Col.ToString());
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)111, Mask);
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)112, Template);
                        break;
                    case 5:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)116, Timeout.ToString());
                        break;
                    case 6:
                        num1 = this.Transact((byte)68, (byte)79, 17, false);
                        break;
                    case 7:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)50, (byte)102, Formatted);
                        break;
                    case 8:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)50, (byte)99, RawString);
                        break;
                    case 9:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int GetYesNoCancel(int Row, int Col, string DisplayText, int Timeout, string[] KeySelected)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)97, Row.ToString());
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)98, Col.ToString());
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)99, DisplayText);
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)116, Timeout.ToString());
                        break;
                    case 5:
                        num1 = this.Transact((byte)68, (byte)79, 10, false);
                        break;
                    case 6:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)50, (byte)100, KeySelected);
                        break;
                    case 7:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int BeepTerminal(int BeepType)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)105, BeepType.ToString());
                        break;
                    case 2:
                        num1 = this.Transact((byte)68, (byte)79, 11, false);
                        break;
                    case 3:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int GetAmount(int Row, int Col, string[] Amount, int TimeOut)
        {
            int num1 = 0;
            bool flag = false;
            string[] Data = new string[1];
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        Amount[0] = "0.00";
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)97, Row.ToString());
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)98, Col.ToString());
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)116, TimeOut.ToString());
                        break;
                    case 4:
                        num1 = this.Transact((byte)68, (byte)79, 4, false);
                        break;
                    case 5:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)50, (byte)97, Data);
                        break;
                    case 6:
                        Amount[0] = Data[0];
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)50, (byte)98, Data);
                        break;
                    case 7:
                        Amount[0] = Amount[0] + (object)'.' + Data[0];
                        break;
                    case 8:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int GetAmount(int Row, int Col, string[] Amount)
        {
            int num1 = 0;
            bool flag = false;
            string[] Data = new string[1];
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        Amount[0] = "0.00";
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)97, Row.ToString());
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)98, Col.ToString());
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)116, "10");
                        break;
                    case 4:
                        num1 = this.Transact((byte)68, (byte)79, 4, false);
                        break;
                    case 5:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)50, (byte)97, Data);
                        break;
                    case 6:
                        Amount[0] = Data[0];
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)50, (byte)98, Data);
                        break;
                    case 7:
                        Amount[0] = Amount[0] + (object)'.' + Data[0];
                        break;
                    case 8:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int GetAmount(int Row, int Col, string[] Amount, string Symbol, int TimeOut)
        {
            int num1 = 0;
            bool flag = false;
            string[] Data = new string[1];
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        Amount[0] = "0.00";
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)97, Row.ToString());
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)98, Col.ToString());
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)116, TimeOut.ToString());
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)110, Symbol);
                        break;
                    case 5:
                        num1 = this.Transact((byte)68, (byte)79, 4, false);
                        break;
                    case 6:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)50, (byte)97, Data);
                        break;
                    case 7:
                        Amount[0] = Data[0];
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)50, (byte)98, Data);
                        break;
                    case 8:
                        Amount[0] = Amount[0] + (object)'.' + Data[0];
                        break;
                    case 9:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int GetAmount(int Row, int Col, string[] Amount, string Symbol)
        {
            int num1 = 0;
            bool flag = false;
            string[] Data = new string[1];
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        Amount[0] = "0.00";
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)97, Row.ToString());
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)98, Col.ToString());
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)116, "10");
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)110, Symbol);
                        break;
                    case 5:
                        num1 = this.Transact((byte)68, (byte)79, 4, false);
                        break;
                    case 6:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)50, (byte)97, Data);
                        break;
                    case 7:
                        Amount[0] = Data[0];
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)50, (byte)98, Data);
                        break;
                    case 8:
                        Amount[0] = Amount[0] + (object)'.' + Data[0];
                        break;
                    case 9:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int InjectMasterKey(string MasterKey, string[] CheckVal)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)57, (byte)97, MasterKey);
                        break;
                    case 2:
                        num1 = this.Transact((byte)65, (byte)79, 99, false);
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)57, (byte)99, CheckVal);
                        break;
                    case 4:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int InjectMasterKey(string MasterKey, string[] CheckVal, int KeyType)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)57, (byte)97, MasterKey);
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)67, (byte)97, KeyType.ToString());
                        break;
                    case 3:
                        num1 = this.Transact((byte)65, (byte)79, 99, false);
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)57, (byte)99, CheckVal);
                        break;
                    case 5:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int InitialiseDUKPT(string iKSN, string iPEK)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)57, (byte)97, iPEK);
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)57, (byte)102, iKSN);
                        break;
                    case 3:
                        num1 = this.Transact((byte)65, (byte)79, 1, false);
                        break;
                    case 4:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int InitialiseDUKPT(string iKSN, string iPEK, out string CheckVal)
        {
            int num1 = 0;
            bool flag = false;
            CheckVal = "";
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)57, (byte)97, iPEK);
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)57, (byte)102, iKSN);
                        break;
                    case 3:
                        num1 = this.Transact((byte)65, (byte)79, 1, false);
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)57, (byte)99, out CheckVal);
                        break;
                    case 5:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int SetSessionKey(string SessionKey, out string CheckVal)
        {
            int num1 = 0;
            bool flag = false;
            CheckVal = "";
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)57, (byte)98, SessionKey);
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)57, (byte)113, "0800");
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)57, (byte)112, "070501");
                        break;
                    case 4:
                        num1 = this.Transact((byte)65, (byte)79, 2, false);
                        break;
                    case 5:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)57, (byte)99, out CheckVal);
                        break;
                    case 6:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int GetSessionKey(string[] CheckVal, string[] DateVal, string[] TimeVal)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.Transact((byte)65, (byte)79, 11, false);
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)57, (byte)99, CheckVal);
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)57, (byte)112, DateVal);
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)57, (byte)113, TimeVal);
                        break;
                    case 5:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int GetAndSetVersion(string LocalVersion, string[] TerminalVersion)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)57, (byte)109, LocalVersion);
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)116, "3");
                        break;
                    case 3:
                        num1 = this.Transact((byte)65, (byte)79, 5, false);
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)57, (byte)109, TerminalVersion);
                        break;
                    case 5:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int WaitForCard(string Prompt, int Timeout, string EventTypes, out string EventType)
        {
            int num1 = 0;
            bool flag = false;
            EventType = "";
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)99, Prompt);
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)116, Timeout.ToString());
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)51, (byte)97, EventTypes);
                        break;
                    case 4:
                        num1 = this.Transact((byte)70, (byte)79, 1, false);
                        break;
                    case 5:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)51, (byte)97, out EventType);
                        break;
                    case 6:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int RemoveCard(string Prompt, int Timeout)
        {
            int num1 = 0;
            bool flag = false;
            //Status = "";
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        goto case 4;
                    case 1:
                        if (Prompt == null || Prompt.Length > 0)
                        {
                            num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)67, (byte)107, Prompt);
                            goto case 4;
                        }
                        else
                            goto case 4;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)84, (byte)49, Timeout.ToString());
                        goto case 4;
                    case 3:
                        num1 = this.Transact((byte)65, (byte)79, 36, false);
                        goto case 4;
                    case 4:
                        ++num2;
                        continue;
                    case 5:
                        flag = true;
                        goto case 4;
                    default:
                        num1 = 1;
                        goto case 4;
                }
            }
            return num1;
        }

        public int ReadCardATR(out string ATR, out string status)
        {
            int num1 = 0;
            bool flag = false;
            ATR = "";
            status = "";
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.Transact((byte)65, (byte)79, 32, false);
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)65, (byte)49, out ATR);
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)67, (byte)104, out status);
                        break;
                    case 4:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int SendISOCommand(string ADPUCmd, string DataIn, out string DataOut, out string status, int le)
        {
            int num1 = 0;
            bool flag = false;
            DataOut = "";
            status = "";
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)73, (byte)50, ADPUCmd);
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)73, (byte)51, DataIn);
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)73, (byte)52, le.ToString());
                        break;
                    case 4:
                        num1 = this.Transact((byte)65, (byte)79, 46, false);
                        break;
                    case 5:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)73, (byte)51, out DataOut);
                        break;
                    case 6:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)73, (byte)52, out status);
                        break;
                    case 7:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int PollCard(string Prompt, string EventTypes, string[] EventType)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)99, Prompt);
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)51, (byte)97, EventTypes);
                        break;
                    case 3:
                        num1 = this.Transact((byte)70, (byte)79, 3, false);
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)51, (byte)97, EventType);
                        break;
                    case 5:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }



        public int GetTrackData(int TrackNumber, bool DebugInfo, string[] TrackData, string[] Status, string[] ReverseFilter, string[] SpikeFilter, string[] DirectionOfSwipe, string[] RoughJitterIndicator, string[] PercentageOfJitter, string[] RoughSpeedOfCardSwipe)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)104, TrackNumber.ToString());
                        break;
                    case 2:
                        if (DebugInfo)
                        {
                            num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)100, "1");
                            break;
                        }
                        break;
                    case 3:
                        num1 = this.Transact((byte)70, (byte)79, 2, false);
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)51, (byte)98, TrackData);
                        break;
                    case 5:
                        if (!DebugInfo)
                        {
                            flag = true;
                            break;
                        }
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)51, (byte)99, Status);
                        break;
                    case 6:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)51, (byte)100, ReverseFilter);
                        break;
                    case 7:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)51, (byte)101, SpikeFilter);
                        break;
                    case 8:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)51, (byte)102, DirectionOfSwipe);
                        break;
                    case 9:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)51, (byte)103, RoughJitterIndicator);
                        break;
                    case 10:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)51, (byte)104, PercentageOfJitter);
                        break;
                    case 11:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)51, (byte)105, RoughSpeedOfCardSwipe);
                        break;
                    case 12:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int GetTrackData(int TrackNumber, string[] TrackData, int Notation)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)104, TrackNumber.ToString());
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)57, (byte)121, Notation.ToString());
                        break;
                    case 3:
                        num1 = this.Transact((byte)70, (byte)79, 2, false);
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)51, (byte)98, TrackData);
                        break;
                    case 5:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        //public int GetTrackData(int TrackNumber, string[] TrackData, string EncryptSessionKey)
        //{
        //    int num1 = 0;
        //    bool flag = false;
        //    int num2 = 0;
        //    while (num1 == 0 && !flag)
        //    {
        //        switch (num2)
        //        {
        //            case 0:
        //                num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
        //                break;
        //            case 1:
        //                num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)104, TrackNumber.ToString());
        //                break;
        //            case 2:
        //                num1 = PINPadAPI_SPDHLayer.SetFIDData((byte)'9', (byte)'b', EncryptSessionKey);
        //                break;
        //            case 3:
        //                num1 = this.Transact((byte)70, (byte)79, 2, false);
        //                break;
        //            case 4:
        //                num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)51, (byte)98, TrackData);
        //                break;
        //            case 5:
        //                flag = true;
        //                break;
        //            default:
        //                num1 = 1;
        //                break;
        //        }
        //        ++num2;
        //    }
        //    return num1;
        //}

        public int GetTrackData(int TrackNumber, string[] TrackData, string EncryptSessionKey)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)104, TrackNumber.ToString());
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)57, (byte)98, EncryptSessionKey);
                        break;
                    case 3:
                        num1 = this.Transact((byte)70, (byte)79, 2, false);
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)51, (byte)98, TrackData);
                        break;
                    case 5:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        //public int GetTrackData(int TrackNumber, string[] BIN, string[] TrackData, string EncryptSessionKey)
        //{
        //    int num1 = 0;
        //    bool flag = false;
        //    int num2 = 0;
        //    while (num1 == 0 && !flag)
        //    {
        //        switch (num2)
        //        {
        //            case 0:
        //                num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
        //                break;
        //            case 1:
        //                num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)104, TrackNumber.ToString());
        //                break;
        //            case 2:
        //                num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)57, (byte)98, EncryptSessionKey);
        //                break;
        //            case 3:
        //                num1 = this.Transact((byte)70, (byte)79, 2, false);
        //                break;
        //            case 4:
        //                num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)51, (byte)98, TrackData);
        //                break;
        //            case 5:
        //                num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)51, (byte)106, BIN);
        //                break;
        //            case 6:
        //                flag = true;
        //                break;
        //            default:
        //                num1 = 1;
        //                break;
        //        }
        //        ++num2;
        //    }
        //    return num1;
        //}

        public int GetSerialNumber(out string SerialNumber)
        {
            int num1 = 0;
            bool flag = false;
            SerialNumber = "";
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.Transact((byte)65, (byte)79, 4, false);
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)57, (byte)102, out SerialNumber);
                        break;
                    case 3:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int SetDebugState(bool State)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.Transact((byte)68, (byte)79, !State ? 6 : 5, false);
                        break;
                    case 2:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int RebootTerminal()
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.Transact((byte)65, (byte)79, 96, false);
                        break;
                    case 2:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int SetDebugPort(string DebugPort)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)50, (byte)97, DebugPort);
                        break;
                    case 2:
                        num1 = this.Transact((byte)68, (byte)79, 7, false);
                        break;
                    case 3:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int SendDebugData(string DebugString)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)99, DebugString);
                        break;
                    case 2:
                        num1 = this.Transact((byte)68, (byte)79, 8, false);
                        break;
                    case 3:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int SendToPrinter(string StringToPrint)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)99, StringToPrint);
                        break;
                    case 2:
                        num1 = this.Transact((byte)68, (byte)79, 9, false);
                        break;
                    case 3:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int PrintLogo(string LogoName)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)99, LogoName);
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)108, "99");
                        break;
                    case 3:
                        num1 = this.Transact((byte)68, (byte)79, 9, false);
                        break;
                    case 4:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int SetTestData(string Track2, string PINBlock1, string PINBlock2, string PINBlock3, string CardType, string SessionKeyCheckValue)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)65, (byte)97, Track2);
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)65, (byte)98, PINBlock1);
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)65, (byte)99, PINBlock2);
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)65, (byte)100, PINBlock3);
                        break;
                    case 5:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)51, (byte)97, CardType);
                        break;
                    case 6:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)57, (byte)99, SessionKeyCheckValue);
                        break;
                    case 7:
                        num1 = this.Transact((byte)65, (byte)79, 98, false);
                        break;
                    case 8:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int SetTestDataState(bool TestingEnabled)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = !TestingEnabled ? this.PINPadAPI_SPDHLayer.SetFIDData((byte)65, (byte)101, "0") : this.PINPadAPI_SPDHLayer.SetFIDData((byte)65, (byte)101, "1");
                        break;
                    case 2:
                        num1 = this.Transact((byte)65, (byte)79, 97, false);
                        break;
                    case 3:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int GetPIN(out string PINBlock, string Prompt, int MinLen, int MaxLen, int Timeout)
        {
            int num1 = 0;
            bool flag = false;
            PINBlock = "";
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        //num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)57, (byte)100, PAN);
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)67, (byte)97, "0");
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)99, Prompt);
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)102, MinLen.ToString());
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)103, MaxLen.ToString());
                        break;
                    case 5:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)116, Timeout.ToString());
                        break;
                    case 6:
                        num1 = this.Transact((byte)65, (byte)79, 3, false);
                        break;
                    case 7:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)57, (byte)101, out PINBlock);
                        break;
                    case 8:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int GetPIN(out string PINBlock, string Prompt, string PAN, out string KSN, int MinLen, int MaxLen, int Timeout, bool DUKPT)
        {
            int num1 = 0;
            bool flag = false;
            PINBlock = "";
            KSN = "";
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)57, (byte)100, PAN);
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)99, Prompt);
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)102, MinLen.ToString());
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)103, MaxLen.ToString());
                        break;
                    case 5:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)49, (byte)116, Timeout.ToString());
                        break;
                    case 6:
                        num1 = !DUKPT ? this.PINPadAPI_SPDHLayer.SetFIDData((byte)67, (byte)97, "0") : this.PINPadAPI_SPDHLayer.SetFIDData((byte)67, (byte)97, "2");
                        break;
                    case 7:
                        num1 = this.Transact((byte)65, (byte)79, 3, false);
                        break;
                    case 8:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)57, (byte)101, out PINBlock);
                        break;
                    case 9:
                        if (DUKPT)
                        {
                            num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)67, (byte)99, out KSN);
                            break;
                        }
                        break;
                    case 10:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int GetEMVVersionInformation(int Type, out string CoreVersion, out string KernelVersion)
        {
            int num1 = 0;
            bool flag = false;
            CoreVersion = "";
            KernelVersion = "";
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)48, Type.ToString());
                        break;
                    case 2:
                        num1 = this.Transact((byte)69, (byte)79, 1, false);
                        break;
                    case 3:
                        this.PINPadAPI_SPDHLayer.GetFIDData((byte)69, (byte)49, out CoreVersion);
                        break;
                    case 4:
                        this.PINPadAPI_SPDHLayer.GetFIDData((byte)69, (byte)50, out KernelVersion);
                        break;
                    case 5:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int GetEMVParameters(out string MerchantName, out string MerchantCatalogCode, out string MerchantId, out string TerminalId, out string TerminalType, out string TerminalCapabilities, out string TerminalExtendedCapabilities, out string TransactionCurrencyExponent, out string ReferenceCurrencyExponent, out string ReferenceCurrencyCode, out string TerminalCountryCode, out string TransactionCurrencyCode, out string ConversionQuotient, out string TransactionType, out string ForceOnline, out string ReadPINRetryCounter, out string SupportPSESelection)
        {
            int num1 = 0;
            bool flag = false;
            MerchantName = "";
            MerchantCatalogCode = "";
            MerchantId = "";
            TerminalId = "";
            TerminalType = "";
            TerminalCapabilities = "";
            TerminalExtendedCapabilities = "";
            TransactionCurrencyExponent = "";
            ReferenceCurrencyExponent = "";
            ReferenceCurrencyCode = "";
            TerminalCountryCode = "";
            TransactionCurrencyCode = "";
            ConversionQuotient = "";
            TransactionType = "";
            ForceOnline = "";
            ReadPINRetryCounter = "";
            SupportPSESelection = "";
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.Transact((byte)69, (byte)79, 2, false);
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)69, (byte)97, out MerchantName);
                        break;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)69, (byte)98, out MerchantCatalogCode);
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)69, (byte)99, out MerchantId);
                        break;
                    case 5:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)69, (byte)100, out TerminalId);
                        break;
                    case 6:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)69, (byte)101, out TerminalType);
                        break;
                    case 7:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)69, (byte)102, out TerminalCapabilities);
                        break;
                    case 8:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)69, (byte)103, out TerminalExtendedCapabilities);
                        break;
                    case 9:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)69, (byte)104, out TransactionCurrencyExponent);
                        break;
                    case 10:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)69, (byte)105, out ReferenceCurrencyExponent);
                        break;
                    case 11:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)69, (byte)106, out ReferenceCurrencyCode);
                        break;
                    case 12:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)69, (byte)107, out TerminalCountryCode);
                        break;
                    case 13:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)69, (byte)108, out TransactionCurrencyCode);
                        break;
                    case 14:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)69, (byte)109, out ConversionQuotient);
                        break;
                    case 15:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)69, (byte)110, out TransactionType);
                        break;
                    case 16:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)69, (byte)111, out ForceOnline);
                        break;
                    case 17:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)69, (byte)112, out ReadPINRetryCounter);
                        break;
                    case 18:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)69, (byte)113, out SupportPSESelection);
                        break;
                    case 19:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int SetEMVParameters(string MerchantName, string MerchantCatalogCode, string MerchantId, string TerminalId, string TerminalType, string TerminalCapabilities, string TerminalExtendedCapabilities, string TransactionCurrencyExponent, string ReferenceCurrencyExponent, string ReferenceCurrencyCode, string TerminalCountryCode, string TransactionCurrencyCode, string ConversionQuotient, string TransactionType, string ForceOnline, string ReadPINRetryCounter, string SupportPSESelection)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        goto case 1;
                    case 1:
                        ++num2;
                        continue;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)97, MerchantName);
                        goto case 1;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)98, MerchantCatalogCode);
                        goto case 1;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)99, MerchantId);
                        goto case 1;
                    case 5:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)100, TerminalId);
                        goto case 1;
                    case 6:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)101, TerminalType);
                        goto case 1;
                    case 7:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)102, TerminalCapabilities);
                        goto case 1;
                    case 8:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)103, TerminalExtendedCapabilities);
                        goto case 1;
                    case 9:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)104, TransactionCurrencyExponent);
                        goto case 1;
                    case 10:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)105, ReferenceCurrencyExponent);
                        goto case 1;
                    case 11:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)106, ReferenceCurrencyCode);
                        goto case 1;
                    case 12:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)107, TerminalCountryCode);
                        goto case 1;
                    case 13:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)108, TransactionCurrencyCode);
                        goto case 1;
                    case 14:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)109, ConversionQuotient);
                        goto case 1;
                    case 15:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)110, TransactionType);
                        goto case 1;
                    case 16:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)111, ForceOnline);
                        goto case 1;
                    case 17:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)112, ReadPINRetryCounter);
                        goto case 1;
                    case 18:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)113, SupportPSESelection);
                        goto case 1;
                    case 19:
                        num1 = this.Transact((byte)69, (byte)79, 3, false);
                        goto case 1;
                    case 20:
                        flag = true;
                        goto case 1;
                    default:
                        num1 = 1;
                        goto case 1;
                }
            }
            return num1;
        }

        public int AddCAPK(string RID, string KeyIndex, string Module, string Exponent, string ExpiryDate, string KeyCheckSum)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        goto case 1;
                    case 1:
                        ++num2;
                        continue;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)116, RID);
                        goto case 1;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)117, KeyIndex);
                        goto case 1;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)120, (Module.Length / 2).ToString());
                        goto case 1;
                    case 5:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)121, Module);
                        goto case 1;
                    case 6:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)122, (Exponent.Length / 2).ToString());
                        goto case 1;
                    case 7:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)65, Exponent);
                        goto case 1;
                    case 8:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)66, ExpiryDate);
                        goto case 1;
                    case 9:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)67, KeyCheckSum);
                        goto case 1;
                    case 10:
                        num1 = this.Transact((byte)69, (byte)79, 6, false);
                        goto case 1;
                    case 11:
                        flag = true;
                        goto case 1;
                    default:
                        num1 = 1;
                        goto case 1;
                }
            }
            return num1;
        }

        public int AddApplication(string ApplicationName, string ApplicationId, string SelectionFlag, string TargetPercent, string MaximumTargetPercent, string FloorLimitCheck, string RandomOnlineCheck, string VelocityCheck, string FloorLimit, string Threshold, string TACDenial, string TACOnline, string TACDefault, string AcquirerId, string DDOL, string TDOL, string RiskManagamentData)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        goto case 1;
                    case 1:
                        ++num2;
                        continue;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)68, ApplicationName);
                        goto case 1;
                    case 3:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)69, ApplicationId);
                        goto case 1;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)70, SelectionFlag);
                        goto case 1;
                    case 5:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)71, TargetPercent);
                        goto case 1;
                    case 6:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)72, MaximumTargetPercent);
                        goto case 1;
                    case 7:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)73, FloorLimitCheck);
                        goto case 1;
                    case 8:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)74, RandomOnlineCheck);
                        goto case 1;
                    case 9:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)75, VelocityCheck);
                        goto case 1;
                    case 10:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)76, FloorLimit);
                        goto case 1;
                    case 11:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)77, Threshold);
                        goto case 1;
                    case 12:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)78, TACDenial);
                        goto case 1;
                    case 13:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)79, TACOnline);
                        goto case 1;
                    case 14:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)80, TACDefault);
                        goto case 1;
                    case 15:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)81, AcquirerId);
                        goto case 1;
                    case 16:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)82, DDOL);
                        goto case 1;
                    case 17:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)83, TDOL);
                        goto case 1;
                    case 18:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)84, RiskManagamentData);
                        goto case 1;
                    case 19:
                        num1 = this.Transact((byte)69, (byte)79, 9, false);
                        goto case 1;
                    case 20:
                        flag = true;
                        goto case 1;
                    default:
                        num1 = 1;
                        goto case 1;
                }
            }
            return num1;
        }

        public int InitialiseTLVData()
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.Transact((byte)69, (byte)79, 14, false);
                        break;
                    case 2:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int SelectApplication(int SequenceNumber)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)89, SequenceNumber.ToString());
                        break;
                    case 2:
                        num1 = this.Transact((byte)69, (byte)79, 15, false);
                        break;
                    case 3:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int ReadApplicationData()
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.Transact((byte)69, (byte)79, 16, false);
                        break;
                    case 2:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int AuthenticateCard()
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.Transact((byte)69, (byte)79, 19, false);
                        break;
                    case 2:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int StartTransaction(long Amount, long Cashback, out string ACType)
        {
            int num1 = 0;
            bool flag = false;
            ACType = "";
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)51, Amount.ToString());
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)52, Cashback.ToString());
                        break;
                    case 3:
                        num1 = this.Transact((byte)69, (byte)79, 20, false);
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)69, (byte)90, out ACType);
                        break;
                    case 5:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int CompleteTransaction(int OnlineStatus, string IssuerScript, out string ACType)
        {
            int num1 = 0;
            bool flag = false;
            ACType = "";
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)53, OnlineStatus.ToString());
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)54, IssuerScript);
                        break;
                    case 3:
                        num1 = this.Transact((byte)69, (byte)79, 21, false);
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)69, (byte)90, out ACType);
                        break;
                    case 5:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int GetScriptResult(out string Result)
        {
            int num1 = 0;
            bool flag = false;
            Result = "";
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.Transact((byte)69, (byte)79, 22, false);
                        break;
                    case 2:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)69, (byte)55, out Result);
                        break;
                    case 3:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int GetTLVData(string Tag, string EncryptSessionKey, out string Data)
        {
            int num1 = 0;
            bool flag = false;
            Data = "";
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        num1 = this.PINPadAPI_SPDHLayer.ClearFIDData();
                        break;
                    case 1:
                        num1 = this.PINPadAPI_SPDHLayer.SetFIDData((byte)69, (byte)114, Tag);
                        break;
                    case 2:
                        num1 = PINPadAPI_SPDHLayer.SetFIDData((byte)'9', (byte)'b', EncryptSessionKey);
                        break;
                    case 3:
                        num1 = this.Transact((byte)69, (byte)79, 4, false);
                        break;
                    case 4:
                        num1 = this.PINPadAPI_SPDHLayer.GetFIDData((byte)69, (byte)115, out Data);
                        break;
                    case 5:
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public int SystemTest()
        {
            return new spdh().SystemTest();
        }

        public int Init(string Port, string TerminalID)
        {
            int num1 = 0;
            bool flag = false;
            int num2 = 0;
            while (num1 == 0 && !flag)
            {
                switch (num2)
                {
                    case 0:
                        this.SetTracingState(false);
                        this.PINPadAPI_SPDHLayer = new spdh();
                        num1 = this.PINPadAPI_SPDHLayer.Initialise(Port);
                        break;
                    case 1:
                        this.PINPadAPI_SPDHLayer.SetSPDHRecvTimeout(60);
                        this.PINPadAPI_TerminalID = TerminalID;
                        break;
                    case 2:
                        this.DisplayError();
                        flag = true;
                        break;
                    default:
                        num1 = 1;
                        break;
                }
                ++num2;
            }
            return num1;
        }

        public void Term()
        {
            this.PINPadAPI_SPDHLayer.Terminate();
        }

        public spdh ReturnSPDHLayer()
        {
            return this.PINPadAPI_SPDHLayer;
        }
    }
}
