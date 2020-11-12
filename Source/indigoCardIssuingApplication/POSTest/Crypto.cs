using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace POSTest
{
    public partial class Crypto : Form
    {
        private const string CHECK_ZEROS = "0000000000000000";
        public Crypto()
        {
            InitializeComponent();
        }
        private void Crypto_Load(object sender, EventArgs e)
        {
            toolTipPinBlockEncode.SetToolTip(btnEncodePINBlock, "Complete PIN and PAN to encode PIN Block");
            toolTipDecodePINBlock.SetToolTip(btnDecodePINBlock, "Complete PAN and PIN Block to decode PIN");
        }

        private void btnXOR_Click(object sender, EventArgs e)
        {
            try
            {
                string xorResult = String.Empty;
                string[] componenets = new string[tbComponents.Lines.Length];

                for (int i = 0; i < tbComponents.Lines.Length; i++)
                {
                    if (!String.IsNullOrWhiteSpace(tbComponents.Lines[i]))
                    {
                        int ssLength = tbComponents.Lines[i].IndexOf("-");
                        ssLength = ssLength > 0 ? ssLength : tbComponents.Lines[i].Length;

                        if (String.IsNullOrWhiteSpace(xorResult))
                        {
                            xorResult =
                            componenets[i] =
                            tbComponents.Lines[i].Substring(0, ssLength).Replace(" ", "");
                        }
                        else
                        {
                            componenets[i] = tbComponents.Lines[i].Substring(0, ssLength).Replace(" ", "");
                            xorResult = Veneka.Indigo.Integration.Cryptography.Utility.XORHexStringsFull(componenets[i], xorResult);
                        }

                        componenets[i] += " -" + Veneka.Indigo.Integration.Cryptography.TripleDes.EncryptTripleDES(componenets[i], CHECK_ZEROS).Substring(0, 6);
                    }
                }

                tbComponents.Lines = componenets;
                tbXORResult.Text = xorResult + " -" + Veneka.Indigo.Integration.Cryptography.TripleDes.EncryptTripleDES(xorResult, CHECK_ZEROS).Substring(0, 6);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbComponents_TextChanged(object sender, EventArgs e)
        {
            if (tbComponents.Lines.Length > 0 && tbComponents.Lines.Length < 8)
                if (!String.IsNullOrWhiteSpace(tbComponents.Lines[tbComponents.Lines.Length - 1]))
                {
                    string[] newLines = new string[tbComponents.Lines.Length + 1];
                    Array.Copy(tbComponents.Lines, newLines, tbComponents.Lines.Length);
                    tbComponents.Lines = newLines;                    
                    tbComponents.SelectionStart = tbComponents.TextLength;
                    tbComponents.ScrollToCaret();
                }
        }

        private void btn3DESEncrypt_Click(object sender, EventArgs e)
        {
            try
            {
                string key = tbKey.Text = tbKey.Text.Replace(" ", "");
                string data = tbData.Text = tbData.Text.Replace(" ", "");                

                tbResult.Text = Veneka.Indigo.Integration.Cryptography.TripleDes.EncryptTripleDES(key, data);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            try
            {
                string key = tbKey.Text = tbKey.Text.Replace(" ", "");
                string data = tbData.Text = tbData.Text.Replace(" ", "");

                tbResult.Text = Veneka.Indigo.Integration.Cryptography.TripleDes.DecryptTripleDES(key, data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEncodePINBlock_Click(object sender, EventArgs e)
        {
            try
            {
                tbClearPinBlock.Text = EncodePinBlock(tbPIN.Text.Replace(" ", ""), tbPAN.Text.Replace(" ", ""));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDecodePINBlock_Click(object sender, EventArgs e)
        {
            try
            {
                tbPIN.Text = DecodePinBlock(tbClearPinBlock.Text.Replace(" ", ""), tbPAN.Text.Replace(" ", ""));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string EncodePinBlock(string PIN, string PAN)
        {
            if (PIN.Length < 4 || PIN.Length > 12)
                throw new Exception("PIn Length must be between 4 and 12 digits in length");

            string PinString = "0" + PIN.Length.ToString("X") + PIN + "FFFFFFFFFF";
            string panString = "0000" + PAN.Substring(PAN.Length - 13, 12);

            return Veneka.Indigo.Integration.Cryptography.Utility.XORHexStringsFull(PinString.Substring(0, 16), panString);           
        }

        private string DecodePinBlock(string PIN, string PAN)
        {
            string panString = String.Empty;

            if (PAN.Length == 12)
                panString = "0000" + PAN;
            else
                panString = "0000" + PAN.Substring(PAN.Length - 13, 12);

            var result = Veneka.Indigo.Integration.Cryptography.Utility.XORHexStringsFull(PIN, panString);

            return result.Substring(2, int.Parse(result[1].ToString(), System.Globalization.NumberStyles.HexNumber));
        }
    }
}
