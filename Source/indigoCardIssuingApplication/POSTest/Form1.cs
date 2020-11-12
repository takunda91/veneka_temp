using POSTest.IndigoWebService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using Veneka.Module.ISO8583;
using Veneka.Module.ISO8583.Client;
using Veneka.Module.ISO8583.WCF;

namespace POSTest
{
    public partial class Form1 : Form
    {
        private readonly ISO8583Client client = new ISO8583Client();
        private int messageId = 0;
        private bool isFirstLine = true;

        public Form1()
        {
            InitializeComponent();
            cbChannel.SelectedIndex = 0;
        }

        #region Events
        private void cbChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbChannel.SelectedIndex == 1)
                lblServerAddress.Visible = tbServerAddress.Visible = true;
            else
                lblServerAddress.Visible = tbServerAddress.Visible = false;
        }

        private void btnHandShake_Click(object sender, EventArgs e)
        {
            //tbHandshakeOut.Text = 
            tbTPK.Text = String.Empty;

            if (String.IsNullOrWhiteSpace(tbDeviceID.Text))
            {
                LogToTexbox(tbHandshakeOut, "Device ID empty.");
                return;
            }

            messageId++;

            try
            {
                if (cbChannel.SelectedIndex == 0)
                    DoIndigo0800();
                else
                {
                    if (String.IsNullOrWhiteSpace(tbServerAddress.Text))
                    {
                        LogToTexbox(tbHandshakeOut, "Server address is empty.");
                        return;
                    }
                    DoLuminus0800();
                }
            }
            catch(Exception ex)
            {
                LogToTexbox(tbHandshakeOut, ex.ToString());
            }                     
        }

        private void btnPIN_Click(object sender, EventArgs e)
        {
            //tbPinOutput.Text = String.Empty;

            if (String.IsNullOrWhiteSpace(tbDeviceID.Text))
                LogToTexbox(tbPinOutput, "Device ID empty.");

            if (String.IsNullOrWhiteSpace(tbTPK.Text))
                LogToTexbox(tbPinOutput, "Do Handshake first.");

            if (String.IsNullOrWhiteSpace(tbTMK.Text) || tbTMK.Text.Length != 32)
                LogToTexbox(tbPinOutput, "TMK empty or not 32 Hex.");

            if (String.IsNullOrWhiteSpace(tbOperatorCode.Text) || tbOperatorCode.Text.Length != 4)
                LogToTexbox(tbPinOutput, "Operator code empty or not 4 numeric.");

            if (String.IsNullOrWhiteSpace(tbPIN.Text) || tbPIN.Text.Length != 4)
                LogToTexbox(tbPinOutput, "PIN empty or not 4 numeric.");

            if (String.IsNullOrWhiteSpace(tbPAN.Text) || tbPAN.Text.Length != 16)
                LogToTexbox(tbPinOutput, "PAN empty or not 16 numeric.");

            if (!String.IsNullOrWhiteSpace(tbPinOutput.Text))
                return;

            try
            {
                string tpk = Veneka.Indigo.Integration.Cryptography
                            .TripleDes.DecryptTripleDES(tbTMK.Text, tbTPK.Text);

                tbClearTPK.Text = tpk;
                LogToTexbox(tbPinOutput, "Clear TPK-\t" + tpk);

                //Operator Code
                string clearOpBlock = EncodePinBlock(tbOperatorCode.Text, tbPAN.Text);
                string opCode = EncryptPinBlock(tpk, clearOpBlock);

                LogToTexbox(tbPinOutput, "Clear Operator-\t" + clearOpBlock);
                LogToTexbox(tbPinOutput, "Encrypted Operator-\t " + opCode);

                //PIN
                string clearPinBlock = EncodePinBlock(tbPIN.Text, tbPAN.Text);
                string pinBlock = EncryptPinBlock(tpk, clearPinBlock);

                LogToTexbox(tbPinOutput, "Clear PIN Block-\t" + clearPinBlock);
                LogToTexbox(tbPinOutput, "Encrypted PIN Block-\t" + pinBlock);

                messageId++;

                if (cbChannel.SelectedIndex == 0)
                    DoIndigo0200(opCode, pinBlock);
                else
                {
                    if (String.IsNullOrWhiteSpace(tbServerAddress.Text))
                    {
                        LogToTexbox(tbHandshakeOut, "Server address is empty.");
                        return;
                    }
                    DoLuminus0200(opCode, pinBlock);
                }                
            }
            catch(Exception ex)
            {
                LogToTexbox(tbPinOutput, ex.ToString());
            }
        }

        #endregion

        #region Helpers
        private void LogToTexbox(TextBox tb, string message)
        {
            // if (!String.IsNullOrWhiteSpace(tb.Text))
            //    tb.Text += Environment.NewLine;

            string start = Environment.NewLine;

            if (isFirstLine)
            {
                start = String.Empty;
                isFirstLine = false;
            }

            tb.BeginInvoke(new Action(() =>
            {
                tb.AppendText(String.Format("{0}{1} - {2}", start, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), message));
            }));

            //tb.Text += DateTime.Now.ToString() + ": " + message;
        }
        #endregion

        #region Indigo Messages
        private void DoIndigo0800()
        {
            ISOMessage msg = new ISOMessage();
            msg.MessageClass = "08";
            msg.MessageFunction = "00";
            List<Field> fields = new List<Field>();
            
            fields.Add(new Field (3, null, "920000" ));
            fields.Add(new Field (11, null, messageId.ToString("000000")));
            fields.Add(new Field (41, null, tbDeviceID.Text ));
            fields.Add(new Field (42, null, "BANKPOS" ));
            msg.Fields = fields;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)3072 | SecurityProtocolType.Ssl3;

            ServicePointManager.ServerCertificateValidationCallback = (object s, X509Certificate certificate,
                                                                        X509Chain chain,
                                                                        SslPolicyErrors sslPolicyErrors) => true;

            var respmsg = client.Send(msg);

            if (respmsg.Fields.Single(s => s.Fieldid == 39).Value == "00")
            {
                LogToTexbox(tbHandshakeOut, "Successful Handshake");
                tbTPK.Text = respmsg.Fields.Single(s => s.Fieldid == 62).Value;
            }
            else
                LogToTexbox(tbHandshakeOut, "Unsuccessful Handshake [" + respmsg.Fields.Single(s => s.Fieldid == 39).Value + "]");
        }

        private void DoIndigo0200(string opCode, string pinBlock)
        {
            ISOMessage msg0200 = new ISOMessage();
            msg0200.MessageClass = "02";
            msg0200.MessageFunction = "00";
            List<Field> fields = new List<Field>();
            fields.Add(new Field (3, null, "700000"));
            fields.Add(new Field (11, null, messageId.ToString("000000")));
            fields.Add(new Field (35, null, tbPAN.Text + "D44122011003400000481"));
            fields.Add(new Field (41, null, tbDeviceID.Text));
            fields.Add(new Field (42, null, "BANKPOS"));

            fields.Add(new Field (52, null, opCode));
            fields.Add(new Field (57, null,  pinBlock));

            msg0200.Fields = fields;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)3072 | SecurityProtocolType.Ssl3;

            ServicePointManager.ServerCertificateValidationCallback = (object s, X509Certificate certificate,
                                                                        X509Chain chain,
                                                                        SslPolicyErrors sslPolicyErrors) => true;

            var respmsg0200 = client.Send(msg0200);

            if (respmsg0200.Fields.Single(s => s.Fieldid == 39).Value == "00")
            {
                LogToTexbox(tbPinOutput, "Successful PIN Reset");
            }
            else
                LogToTexbox(tbPinOutput, "Unsuccessful PIN Reset [" + respmsg0200.Fields.Single(s => s.Fieldid == 39).Value + "]");
        }
        #endregion

        #region Luminus Messages
        private void DoLuminus0800()
        {
            var msg = new ISO8583("0800", Ingenico.IngenicoDataList.Create());
            msg.SetField(3, "920000");
            msg.SetField(11, messageId.ToString("000000"));
            msg.SetField(41, tbDeviceID.Text);
            msg.SetField(42, "BANKPOS");

            ISO8583 response;

            var splits = tbServerAddress.Text.Split(':');

            if(splits.Length != 2)
            {
                LogToTexbox(tbHandshakeOut, "Server address not in correct format, must be \"IP:Port\"");
                return;
            }

            using (SocketClient client = new SocketClient(splits[0], int.Parse(splits[1])))
            {
                client.RaiseDataSentEvent += HandleDataSentEvent;
                client.RaiseDataReceivedEvent += HandleDataReceivedEvent;

                client.Connect();
                response = client.SendReceive(msg);
                client.Disconnect();
            }

            if (response[39] == "00")
            {
                LogToTexbox(tbHandshakeOut, "Successful Handshake");
                tbTPK.Text = BitConverter.ToString(response.GetField(62)).Replace("-", "");
            }
            else
                LogToTexbox(tbHandshakeOut, "Unsuccessful Handshake [" + response[39] + "]");
        }

        private void DoLuminus0200(string opCode, string pinBlock)
        {
            var msg = new ISO8583("0200", Ingenico.IngenicoDataList.Create());

            msg.SetField(3, "700000");
            msg.SetField(11, messageId.ToString("000000"));
            msg.SetField(35, tbPAN.Text + "D44122011003400000481");
            msg.SetField(41, tbDeviceID.Text);
            msg.SetField(42, "BANKPOS");

            msg.SetField(52, HexadecimalStringToByteArray(opCode));
            msg.SetField(57, HexadecimalStringToByteArray(pinBlock));

            ISO8583 response;

            var splits = tbServerAddress.Text.Split(':');

            if (splits.Length != 2)
            {
                LogToTexbox(tbPinOutput, "Server address not in correct format, must be \"IP:Port\"");
                return;
            }

            using (SocketClient client = new SocketClient(splits[0], int.Parse(splits[1])))
            {
                client.RaiseDataSentEvent += HandleDataSentEvent;
                client.RaiseDataReceivedEvent += HandleDataReceivedEvent;

                client.Connect();
                response = client.SendReceive(msg);
                client.Disconnect();
            }

            if (response[39] == "00")
            {
                LogToTexbox(tbHandshakeOut, "Successful Handshake");
                tbTPK.Text = BitConverter.ToString(response.GetField(62)).Replace("-", "");
            }
            else
                LogToTexbox(tbHandshakeOut, "Unsuccessful Handshake [" + response[39] + "]");
        }
        #endregion

        #region Events
        void HandleDataSentEvent(object sender, DataEventArgs e)
        {
            LogToTexbox(tbHandshakeOut, "Data Sent(" + e.Data.Length + " bytes)" + Environment.NewLine + FormatBytes(e.Data));
        }

        void HandleDataReceivedEvent(object sender, DataEventArgs e)
        {            
            LogToTexbox(tbHandshakeOut, "Data Received(" + e.Data.Length + " bytes)" + Environment.NewLine + FormatBytes(e.Data));
        }

        private string FormatBytes(byte[] data)
        {
            //BitConverter.ToString
            StringBuilder sb = new StringBuilder();
            string ss = string.Empty;           
            
            for(int i = 0; i <= data.Length / 16; i++)
            {
                int length = (i+1) * 16 >= data.Length ? data.Length % 16 : 16;

                //var digitStr = new String(data.Where(w => w ).ToArray());
                //new ArraySegment<byte>(data, i * 16, length).Array

                sb.AppendFormat("{0,-48} ", BitConverter.ToString(data, i * 16, length).Replace('-', ' '));

                //if(length < 16)
                //    sb.Append()

                //sb.Append(" ");

                foreach (byte b in data.Skip(i * 16).Take(length))
                {
                    if (b >= 33 && b <= 126)
                        sb.Append(Convert.ToChar(b));
                    else
                        sb.Append('.');
                }
                sb.AppendLine();

                //ss += //BitConverter.ToString(data, i * 16, length).Replace('-', ' ') + " " +
                //                            new String(Encoding.ASCII.GetString(data, i * 16, length).Where(Char.IsLetterOrDigit | Char.IsWhiteSpace | Char.IsSymbol);
                                            //+ Environment.NewLine;

                //sb.AppendFormat("{0} | {1}", BitConverter.ToString(data, i * 16, length).Replace('-', ' '), 
                //                            @System.Text.Encoding.UTF8.GetString(data, i * 16, length));
                //sb.AppendLine();

                //sb.AppendLine(BitConverter.ToString(data, i * 16, length).Replace('-', ' '));
            }

            return sb.ToString().Trim();
        }
        #endregion


        #region Crypto Functions
        private string EncodePinBlock(string PIN, string PAN)
        {
            if (PIN.Length < 4 || PIN.Length > 12)
                throw new Exception("PIn Length must be between 4 and 12 digits in length");

            string PinString = "0" + PIN.Length.ToString("X") + PIN + "FFFFFFFFFF";
            string panString = "0000" + PAN.Substring(PAN.Length - 13, 12);

            return Veneka.Indigo.Integration.Cryptography.Utility.XORHexStringsFull(PinString.Substring(0, 16), panString);
        }

        private string EncryptPinBlock(string tpk, string clearPinBlock)
        {
            string check;
            string pinB = Veneka.Indigo.Integration.Cryptography
                        .TripleDes.EncryptTripleDES(tpk, clearPinBlock, out check);

            return pinB;
        }               

        private byte[] HexadecimalStringToByteArray(string input)
        {
            var outputLength = input.Length / 2;
            var output = new byte[outputLength];
            for (var i = 0; i < outputLength; i++)
                output[i] = Convert.ToByte(input.Substring(i * 2, 2), 16);
            return output;
        }
        #endregion

        private void btnCrypto_Click(object sender, EventArgs e)
        {
            Crypto cryptoFrm = new Crypto();
            cryptoFrm.Show();
        }
    }
}
