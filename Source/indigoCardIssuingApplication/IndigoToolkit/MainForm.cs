using IndigoMigrationToolkit.Objects;
using IndigoMigrationToolkit.Utils;
using IndigoMigrationToolkit.Versions;
using IndigoToolkit.DatabaseServers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndigoToolkit
{
    public partial class MainForm : Form, IParentForm
    {
        private SqlServer _dbServer;
        private WindowLogging _loggingWindow;
        private readonly List<IndigoVersionInfo> _indigoVersions = new List<IndigoVersionInfo>
        {
            new v2000(), new v2100(), new v2110(), new v2120(), new v2130()
        };

        private const string COMMAND_TIMEOUT = "CommandTimeout";
        private const string MIGRATION_BATCHSIZE = "MigrationBatchSize";

        private readonly ComboBoxItem[] _sqlAuthTypes = new ComboBoxItem[2] 
        {
            new ComboBoxItem("Integrated", Microsoft.SqlServer.Management.Common.SqlConnectionInfo.AuthenticationMethod.ActiveDirectoryIntegrated)
            ,new ComboBoxItem("SQL Server", Microsoft.SqlServer.Management.Common.SqlConnectionInfo.AuthenticationMethod.SqlPassword)
            //,new ComboBoxItem("Active Directory", Microsoft.SqlServer.Management.Common.SqlConnectionInfo.AuthenticationMethod.ActiveDirectoryPassword)
        };


        public MainForm()
        {
            InitializeComponent();
            _loggingWindow = new WindowLogging(tbLogWindow);

            cmbSqlAuth.Items.AddRange(_sqlAuthTypes);
            cmbSqlAuth.SelectedIndex = 0;
        }

        private void btnConnectSource_Click(object sender, EventArgs e)
        {
            string sqlInstance = "localhost";

            if (!String.IsNullOrWhiteSpace(tbSourceServer.Text))
                sqlInstance = tbSourceServer.Text;
            else
                tbSourceServer.Text = sqlInstance;

            CancellationTokenSource cts = new CancellationTokenSource();
            
            Task.Factory.StartNew(() => TaskProgress(cts.Token));
            cmbSqlAuth.Enabled =
            tbUsername.Enabled =
            tbPassword.Enabled =
            ckbTrustServer.Enabled =
            ckbEncrypted.Enabled =            
            tbSourceServer.Enabled = 
            btnConnectSource.Enabled = false;

            try
            {

                if (_dbServer != null && _dbServer.Connected) //Disconnect From Instance
                {
                    _loggingWindow.WriteFormat("Disconnecting from {0}...", sqlInstance);

                    var ui = TaskScheduler.FromCurrentSynchronizationContext();

                    Task.Factory.StartNew(() => _dbServer.Disconnect())
                        .ContinueWith(task =>
                        {
                            if (task.Exception != null)
                            {
                                if(cmbSqlAuth.SelectedIndex != 0)
                                {
                                    tbUsername.Enabled =
                                    tbPassword.Enabled = true;
                                }
                                cmbSqlAuth.Enabled =
                                ckbTrustServer.Enabled =
                                ckbEncrypted.Enabled =
                                tbSourceServer.Enabled = true;
                                _loggingWindow.Write(task.Exception.Flatten().ToString());
                            }
                            else
                            {
                                _loggingWindow.WriteFormat("Disconnected from {0}.", sqlInstance);
                                btnConnectSource.Text = "Connect";
                                btnNewDB.Enabled =
                                btnMigrate.Enabled = false;

                                if (cmbSqlAuth.SelectedIndex != 0)
                                {
                                    tbUsername.Enabled =
                                    tbPassword.Enabled = true;
                                }

                                ckbTrustServer.Enabled = 
                                cmbSqlAuth.Enabled =
                                ckbEncrypted.Enabled =
                                tbSourceServer.Enabled = true;
                                tbSqlInfo.Text = _dbServer.ServerInfo;

                                if (currentForm != null)
                                {
                                    currentForm.Hide();
                                    currentForm.Dispose();
                                }
                            }
                            cts.Cancel();
                            btnConnectSource.Enabled = true;
                        }, ui);
                }
                else //Connect to instance
                {
                    if (cmbSqlAuth.SelectedIndex == 0)
                    {
                        _dbServer = new SqlServer(tbSourceServer.Text, ckbEncrypted.Checked, ckbTrustServer.Checked, CommandTimeout, _loggingWindow);
                    }
                    else
                    {
                        var sqlAuth = (Microsoft.SqlServer.Management.Common.SqlConnectionInfo.AuthenticationMethod)(cmbSqlAuth.SelectedItem as ComboBoxItem).Value;
                        _dbServer = new SqlServer(tbSourceServer.Text, tbUsername.Text, tbPassword.Text, sqlAuth, ckbEncrypted.Checked, ckbTrustServer.Checked, CommandTimeout, _loggingWindow);
                    }

                    _loggingWindow.WriteFormat("Connecting to {0}...", sqlInstance);                    

                    var ui = TaskScheduler.FromCurrentSynchronizationContext();

                    Task.Factory.StartNew(() => _dbServer.Connect())
                        .ContinueWith(task =>
                        {
                            if (task.Exception != null)
                            {
                                if (cmbSqlAuth.SelectedIndex != 0)
                                {
                                    tbUsername.Enabled =
                                    tbPassword.Enabled = true;
                                }

                                cmbSqlAuth.Enabled =
                                ckbTrustServer.Enabled =
                                ckbEncrypted.Enabled =
                                tbSourceServer.Enabled = true;
                                _loggingWindow.Write(task.Exception.Flatten().ToString());
                            }
                            else
                            {
                                _loggingWindow.WriteFormat("Connected to {0}.", sqlInstance);
                                tbSqlInfo.Text = _dbServer.ServerInfo;
                                btnConnectSource.Text = "Disconnect";
                                btnNewDB.Enabled =
                                btnMigrate.Enabled = true;
                            }
                            cts.Cancel();                            
                            btnConnectSource.Enabled = true;
                        }, ui);                    
                }
            }
            catch(Microsoft.SqlServer.Management.Common.ConnectionFailureException cfex)
            {
                if (cmbSqlAuth.SelectedIndex != 0)
                {
                    tbUsername.Enabled =
                    tbPassword.Enabled = true;
                }

                cmbSqlAuth.Enabled =
                ckbTrustServer.Enabled =
                ckbEncrypted.Enabled =
                tbSourceServer.Enabled = 
                btnConnectSource.Enabled = true;
                _loggingWindow.Write(cfex.ToString());
                cts.Cancel();
            }
            catch(AggregateException aex)
            {
                _loggingWindow.Write(aex.Flatten().ToString());
                cts.Cancel();
                progressBar.Value = 0;
            }
            catch (Exception ex)
            {
                _loggingWindow.Write(ex.ToString());
                cts.Cancel();
            }            
        }

        private void IndigoToolkit_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_dbServer != null && _dbServer.Connected)
            {
                _loggingWindow.Write("disconnecting...");
                _dbServer.Disconnect();
            }
        }

        public void TaskProgress(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                progressBar.BeginInvoke(new Action(() =>
                {
                    if (progressBar.Value == 100)                    
                        progressBar.Value = 0;                    

                    progressBar.PerformStep();
                }));

                token.WaitHandle.WaitOne(500);                
            }

            progressBar.BeginInvoke(new Action(() =>
            {
                progressBar.Value = 0;
            }));
        }

        public static int? CommandTimeout
        {
            get
            {
                if (ConfigurationManager.AppSettings[COMMAND_TIMEOUT] == null)
                    return null;

                if (!String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[COMMAND_TIMEOUT].ToString()))
                    return int.Parse(ConfigurationManager.AppSettings[COMMAND_TIMEOUT].ToString());
                else
                    return null;
            }
        }

        public static int? MigrationBatchSize
        {
            get
            {
                if (ConfigurationManager.AppSettings[MIGRATION_BATCHSIZE] == null)
                    return null;

                if (!String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[MIGRATION_BATCHSIZE].ToString()))
                    return int.Parse(ConfigurationManager.AppSettings[MIGRATION_BATCHSIZE].ToString());
                else
                    return null;
            }
        }

        private Form currentForm;

        private void button1_Click(object sender, EventArgs e)
        {
            if (_dbServer == null || !_dbServer.Connected)
            {
                MessageBox.Show("Connect to database server first.", "Not Connected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (currentForm != null)
            {
                currentForm.Hide();
                currentForm.Dispose();
                //panel1.Controls.RemoveAt(0);
            }            

            currentForm = new CreateDatabaseForm(_dbServer, _loggingWindow, this);


            currentForm.FormBorderStyle = FormBorderStyle.None;            
            currentForm.TopLevel = false;
            currentForm.AutoScroll = true;
            this.panel1.Controls.Add(currentForm);
            currentForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_dbServer == null || !_dbServer.Connected)
            {
                MessageBox.Show("Connect to database server first.", "Not Connected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (currentForm != null)
            {
                currentForm.Hide();
                currentForm.Dispose();
            }

            currentForm = new MigrationForm(_dbServer, _loggingWindow, this);

            currentForm.FormBorderStyle = FormBorderStyle.None;
            //And then, manipulates the action:
            currentForm.TopLevel = false;
            currentForm.AutoScroll = true;
            this.panel1.Controls.Add(currentForm);
            currentForm.Show();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void GetAllControl(Control c, List<Control> list)
        {
            foreach (Control control in c.Controls)
            {
                if (control.GetType() == typeof(Button))
                    list.Add(control);

                if (control.GetType() == typeof(Panel) || control.GetType() == typeof(GroupBox))
                    GetAllControl(control, list);
            }
        }

        public void Disbale()
        {
            List<Control> list = new List<Control>();

            GetAllControl(this, list);

            foreach (Control control in list)
            {
                control.Enabled = false;
            }
        }

        public void Enable()
        {
            List<Control> list = new List<Control>();

            GetAllControl(this, list);

            foreach (Control control in list)
            {
                control.Enabled = true;
            }
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbLogWindow.Clear();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrWhiteSpace(tbLogWindow.Text))
            {
                MessageBox.Show("Nothing in log window to save.", "No Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            saveLogMenu.FileName = "IndigoToolkit_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".txt";
            saveLogMenu.Filter = "Text files(*.txt) | *.txt";
            saveLogMenu.ShowDialog();
        }

        private void saveLogMenu_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Indigo Toolkit " + Application.ProductVersion);
                sb.AppendLine(tbSourceServer.Text);
                sb.AppendLine(tbLogWindow.Text);
                sb.Append("END");
                File.WriteAllText(saveLogMenu.FileName, sb.ToString());

                MessageBox.Show("File saved.", "saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Saving", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(tbLogWindow.Text))
                return;

            if(String.IsNullOrWhiteSpace(tbLogWindow.SelectedText))
                Clipboard.SetText(tbLogWindow.Text, TextDataFormat.UnicodeText);
            else
                Clipboard.SetText(tbLogWindow.SelectedText, TextDataFormat.UnicodeText);
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbLogWindow.Focus();
            tbLogWindow.SelectAll();
        }

        private void cmbSqlAuth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbSqlAuth.SelectedIndex == 0)
            {
                tbUsername.Enabled =
                tbPassword.Enabled = false;
            }
            else
            {
                tbUsername.Enabled =
                tbPassword.Enabled = true;
            }
        }
    }
}
