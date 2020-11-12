using IndigoMigrationToolkit.Objects;
using IndigoMigrationToolkit.Utils;
using IndigoMigrationToolkit.Versions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndigoMigrationToolkit
{
    public partial class IndigoToolkit : Form
    {
        private Source.SourceDAL _source;
        private WindowLogging _loggingWindow;
        private readonly List<IndigoVersionInfo> _indigoVersions = new List<IndigoVersionInfo>
        {
            new v2000(), new v2100(), new v2110(), new v2120(), new v2130()
        };

        private const string COMMAND_TIMEOUT = "CommandTimeout";
        private const string MIGRATION_BATCHSIZE = "MigrationBatchSize";

        public IndigoToolkit()
        {
            InitializeComponent();
            _loggingWindow = new WindowLogging(tbLogWindow);
            
            foreach(var version in _indigoVersions)
            {
                cmbSourceVersion.Items.Add(new ComboBoxItem(version.Name, version));
                cmbTargetVersion.Items.Add(new ComboBoxItem(version.Name, version));
            }

            cmbTargetVersion.SelectedIndex = cmbTargetVersion.Items.Count - 1;
        }

        private void btnConnectSource_Click(object sender, EventArgs e)
        {
            cmbTargetDatabase.Items.Clear();
            cmbSourceDbs.Items.Clear();
            cmbTargetCollation.Items.Clear();

            string sqlInstance = "local";

            if (!String.IsNullOrWhiteSpace(tbSourceServer.Text))
                sqlInstance = tbSourceServer.Text;

            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Factory.StartNew(() => TaskProgress(cts.Token));
            tbSourceServer.Enabled = btnConnectSource.Enabled = false;

            try
            {

                if (_source != null && _source.Connected) //Disconnect From Instance
                {
                    _loggingWindow.WriteFormat("Disconnecting from {0}...", sqlInstance);

                    var ui = TaskScheduler.FromCurrentSynchronizationContext();

                    Task.Factory.StartNew(() => _source.Disconnect())
                        .ContinueWith(task =>
                        {
                            if (task.Exception != null)
                            {
                                tbSourceServer.Enabled = true;
                                _loggingWindow.Write(task.Exception.Flatten().ToString());
                            }
                            else
                            {
                                _loggingWindow.WriteFormat("Disconnected from {0}.", sqlInstance);
                                btnConnectSource.Text = "Connect";

                                gbCreateDB.Enabled = gbMigrationOptions.Enabled = false;
                                tbSourceServer.Enabled = true;
                                tbSqlInfo.Text = _source.ServerInfo;
                            }
                            cts.Cancel();
                            btnConnectSource.Enabled = true;
                        }, ui);
                }
                else //Connect to instance
                {
                    _source = new Source.SourceDAL(tbSourceServer.Text, CommandTimeout, _loggingWindow);

                    _loggingWindow.WriteFormat("Connecting to {0}...", sqlInstance);                    

                    var ui = TaskScheduler.FromCurrentSynchronizationContext();

                    Task.Factory.StartNew(() => _source.Connect())
                        .ContinueWith(task =>
                        {
                            if (task.Exception != null)
                            {
                                tbSourceServer.Enabled = true;
                                _loggingWindow.Write(task.Exception.Flatten().ToString());
                            }
                            else
                            {
                                _loggingWindow.WriteFormat("Connected to {0}.", sqlInstance);

                                var dbs = _source.DatabaseList().ToArray();
                                cmbTargetDatabase.Items.AddRange(dbs);
                                cmbSourceDbs.Items.AddRange(dbs);

                                cmbTargetCollation.Items.AddRange(_source.ServerCollations.ToArray());
                                cmbTargetCollation.SelectedText = _source.DefaultServerCollation;

                                tbSqlInfo.Text = _source.ServerInfo;

                                btnConnectSource.Text = "Disconnect";

                                gbCreateDB.Enabled = gbMigrationOptions.Enabled = true;
                            }
                            cts.Cancel();                            
                            btnConnectSource.Enabled = true;
                        }, ui);                    
                }
            }
            catch(AggregateException aex)
            {
                tbLogWindow.Text += aex.Flatten().ToString();
                cts.Cancel();
                progressBar.Value = 0;
            }
            catch (Exception ex)
            {
                tbLogWindow.Text += ex;
                cts.Cancel();
            }            
        }

        private void IndigoToolkit_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_source != null && _source.Connected)
            {
                _loggingWindow.Write("disconnecting...");
                _source.Disconnect();
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

        private void ckbExportKeys_CheckedChanged(object sender, EventArgs e)
        {
            tbKeyExportPath.Enabled = tbExportKey.Enabled = tbExportKeyConfirm.Enabled = ckbExportKeys.Checked;
        }

        private void btnCreateDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                if (_source.Exists(cmbTargetDatabase.Text))
                {
                    _loggingWindow.WriteFormat("{0} already exists on server.", cmbTargetDatabase.Text);

                    MessageBox.Show("Database already exists on server!",
                                    "Database Exists",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Stop);
                    return;
                }

                var targetIndigo = new IndigoInfo(((ComboBoxItem)cmbTargetVersion.SelectedItem).Value as IndigoVersionInfo,
                                              cmbTargetDatabase.Text,
                                              cmbTargetCollation.Text,
                                              tbMasterKey.Text,
                                              new System.IO.DirectoryInfo(tbExportPath.Text),
                                              ckbExportKeys.Checked,
                                              new System.IO.DirectoryInfo(tbKeyExportPath.Text),
                                              tbExportKey.Text,
                                              null,
                                              chkCreateEnterprise.Checked);

                var confirmResult = MessageBox.Show(String.Format("This will create new database {0} on {1}, are you sure?", targetIndigo.DatabaseName, tbSourceServer.Text),
                                         "Confirm Create Database",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

                if (confirmResult == DialogResult.No)
                    return;                

                gbMigrationOptions.Enabled = gbSourceServer.Enabled = gbCreateDB.Enabled = false;
                //btnStartMigration.Enabled = false;
                var ui = TaskScheduler.FromCurrentSynchronizationContext();

                CancellationTokenSource cts = new CancellationTokenSource();
                Task.Factory.StartNew(() => TaskProgress(cts.Token));

                Task.Factory.StartNew(() => _source.CreateNewDatabase(targetIndigo))
                    .ContinueWith(task =>
                    {
                        gbMigrationOptions.Enabled = gbSourceServer.Enabled = gbCreateDB.Enabled = true;
                        cts.Cancel();
                        progressBar.Value = 0;
                        
                        cmbTargetDatabase.Items.Clear();
                        cmbSourceDbs.Items.Clear();

                        var dbs = _source.DatabaseList().ToArray();
                        cmbTargetDatabase.Items.AddRange(dbs);
                        cmbSourceDbs.Items.AddRange(dbs);
                    }, ui);
            }
            catch (Exception ex)
            {
                tbLogWindow.Text = ex.ToString();
            }
        }

        private void btnStartMigration_Click(object sender, EventArgs e)
        {
            //_source.Bulk();
            try
            {
                if (!_source.Exists(cmbTargetDatabase.Text))
                {
                    _loggingWindow.WriteFormat("{0} does not exists on server.", cmbTargetDatabase.Text);

                    MessageBox.Show("Database doesnt exist on server!",
                                    "Database doesnt exist",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Stop);
                    return;
                }


                var targetIndigo = new IndigoInfo(((ComboBoxItem)cmbTargetVersion.SelectedItem).Value as IndigoVersionInfo,
                                              cmbTargetDatabase.Text,
                                              _source.DatabaseCollation(cmbTargetDatabase.Text),
                                              tbMasterKey.Text,
                                              new System.IO.DirectoryInfo(tbExportPath.Text),
                                              ckbExportKeys.Checked,
                                              new System.IO.DirectoryInfo(tbKeyExportPath.Text),
                                              tbExportKey.Text,
                                              new System.IO.DirectoryInfo(tbPostScripts.Text),
                                              false);

                var sourceIndigo = new IndigoInfo(((ComboBoxItem)cmbSourceVersion.SelectedItem).Value as IndigoVersionInfo,
                                                  cmbSourceDbs.Text,
                                                  tbSourceCollation.Text,
                                                  String.Empty,
                                                  null,
                                                  false,
                                                  null,
                                                  String.Empty,
                                                  null,
                                                  false);

                var confirmResult = MessageBox.Show(String.Format("This will migrate database {0} to {1} on {2}, are you sure?", sourceIndigo.DatabaseName, targetIndigo.DatabaseName, tbSourceServer.Text),
                                         "Confirm Migration",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

                if (confirmResult == DialogResult.No)
                    return;

                gbMigrationOptions.Enabled = gbSourceServer.Enabled = gbCreateDB.Enabled = false;
                //btnStartMigration.Enabled = false;
                var ui = TaskScheduler.FromCurrentSynchronizationContext();

                CancellationTokenSource cts = new CancellationTokenSource();
                Task.Factory.StartNew(() => TaskProgress(cts.Token));

                Task.Factory.StartNew(() => _source.MigrateData(targetIndigo, sourceIndigo, CommandTimeout, MigrationBatchSize))
                    .ContinueWith(task =>
                    {
                        gbMigrationOptions.Enabled = gbSourceServer.Enabled = gbCreateDB.Enabled = true;
                        cts.Cancel();                      
                    }, ui);
            }
            catch (Exception ex)
            {
                tbLogWindow.Text = ex.ToString();
            }
        }

        private void cmbSourceDbs_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbSourceCollation.Text = _source.DatabaseCollation(cmbSourceDbs.Text);
        }

        private void cmbTargetDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbTargetCollation.Text = String.Empty;
            cmbTargetCollation.SelectedText = _source.DatabaseCollation(cmbTargetDatabase.Text);
        }

        private void btnSelectPost_Click(object sender, EventArgs e)
        {
            // Display the openFile dialog.
            DialogResult result = folderBrowserDialog1.ShowDialog();

            // OK button was pressed.
            if (result == DialogResult.OK)
            {
                tbPostScripts.Text =folderBrowserDialog1.SelectedPath;
            }
        }

        private int? CommandTimeout
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

        private int? MigrationBatchSize
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
    }
}
