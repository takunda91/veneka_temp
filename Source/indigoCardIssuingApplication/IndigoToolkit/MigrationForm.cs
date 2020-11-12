using IndigoMigrationToolkit.Objects;
using IndigoMigrationToolkit.Utils;
using IndigoToolkit.DatabaseServers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndigoToolkit
{
    public partial class MigrationForm : Form
    {
        private SqlServer _dbServer;
        private WindowLogging _loggingWindow;
        private IParentForm _parentForm;

        public MigrationForm(SqlServer dbServer, WindowLogging loggingWindow, IParentForm parentForm)
        {
            InitializeComponent();
            _dbServer = dbServer;
            _loggingWindow = loggingWindow;
            _parentForm = parentForm;

            foreach (var version in Enum.GetValues(typeof(PreviousIndigoVersion)).Cast<PreviousIndigoVersion>())
                cmbSourceVersion.Items.Add(new ComboBoxItem(version.ToString(), version));


            if (_dbServer.Connected)
            {
                this.gbMigrationOptions.Enabled = true;

                var dbs = _dbServer.DatabaseList().ToArray();
                cmbTargetDatabase.Items.AddRange(dbs);
                cmbSourceDbs.Items.AddRange(dbs);
            }
        }



        private void btnStartMigration_Click(object sender, EventArgs e)
        {
            try
            {
                bool targetExists = false;

                #region validations
                if (String.IsNullOrWhiteSpace(cmbSourceDbs.Text))
                {
                    MessageBox.Show("Source database name is empty. Please select a database to migrate data from.",
                                        "Source database empty",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Stop);
                    return;
                }

                if (!_dbServer.Exists(cmbSourceDbs.Text))
                {
                    MessageBox.Show("Source database does not exist! Please select a source database you wish to migrate from.",
                                        "Source Database does not exist",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Stop);
                    return;
                }

                if (String.IsNullOrWhiteSpace(cmbTargetDatabase.Text))
                {
                    MessageBox.Show("Target database name is empty. Please enter a new database to migrate data to.",
                                        "Target database empty",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Stop);
                    return;
                }

                if (_dbServer.Exists(cmbTargetDatabase.Text))
                {
                    var diagResult = MessageBox.Show("Database already exists! Press OK to attempt migration on this DB. For this to work correctly the DB should not contain any data.",
                                        "Target Database Exists",
                                        MessageBoxButtons.OKCancel,
                                        MessageBoxIcon.Stop);
                    if (diagResult == DialogResult.Cancel)
                        return;
                    else
                        targetExists = true;
                }
                #endregion


                var confirmResult = MessageBox.Show(String.Format("This will migrate database {0} to {1} on {2}, are you sure?", cmbSourceDbs.Text, cmbTargetDatabase.Text, _dbServer.ServerName),
                                         "Confirm Migration",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

                if (confirmResult == DialogResult.No)
                    return;

                var targetIndigo = new IndigoInfo(cmbTargetDatabase.Text,
                                                  tbSourceCollation.Text,
                                                  tbMasterKey.Text,
                                                  new System.IO.DirectoryInfo(tbExportPath.Text),
                                                  ckbExportKeys.Checked,
                                                  new System.IO.DirectoryInfo(tbKeyExportPath.Text),
                                                  tbExportKey.Text,
                                                  cmbSourceDbs.Text,
                                                  (PreviousIndigoVersion)((IndigoMigrationToolkit.Objects.ComboBoxItem)cmbSourceVersion.SelectedItem).Value,
                                                  new System.IO.DirectoryInfo(tbPostScripts.Text));


                _parentForm.Disbale();
                gbMigrationOptions.Enabled = false;
                var ui = TaskScheduler.FromCurrentSynchronizationContext();

                CancellationTokenSource cts = new CancellationTokenSource();
                Task.Factory.StartNew(() => TaskProgress(cts.Token));
                bool createSuccess = false;

                if (targetExists)
                {
                    Task.Factory.StartNew(() => _dbServer.MigrateData(targetIndigo, MainForm.CommandTimeout, MainForm.MigrationBatchSize))
                        .ContinueWith(task =>
                        {
                            _parentForm.Enable();
                            gbMigrationOptions.Enabled = true;
                            cts.Cancel();
                            progressBar.Value = 0;
                        }, ui);
                }
                else
                {
                    //Create database
                    Task.Factory.StartNew(() => createSuccess = _dbServer.CreateNewDatabase(targetIndigo))
                        .ContinueWith(task =>
                        {
                            cmbTargetDatabase.Items.Clear();
                            var dbs = _dbServer.DatabaseList().ToArray();
                            cmbTargetDatabase.Items.AddRange(dbs);

                            if (!createSuccess)
                            {
                                _parentForm.Enable();
                                gbMigrationOptions.Enabled = true;
                                cts.Cancel();
                                progressBar.Value = 0;
                            }

                        }, ui)
                        .ContinueWith(task => _dbServer.MigrateData(targetIndigo, MainForm.CommandTimeout, MainForm.MigrationBatchSize))
                        .ContinueWith(task =>
                        {
                            _parentForm.Enable();
                            gbMigrationOptions.Enabled = true;
                            cts.Cancel();
                            progressBar.Value = 0;
                        }, ui);
                }
            }
            catch (Exception ex)
            {
                _loggingWindow.Write(ex.ToString());
            }
        }

        private void cmbSourceDbs_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbSourceCollation.Text = String.Empty;
            tbSourceCollation.Text = _dbServer.DatabaseCollation(cmbSourceDbs.Text);
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

        private void btnValidation_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show(String.Format("This will validate migrated data on database {0} to {1} on {2}, are you sure?", cmbSourceDbs.Text, cmbTargetDatabase.Text, _dbServer.ServerName),
                                         "Confirm Migration",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

            if (confirmResult == DialogResult.No)
                return;

            var targetIndigo = new IndigoInfo(cmbTargetDatabase.Text,
                                              tbSourceCollation.Text,
                                              tbMasterKey.Text,
                                              new System.IO.DirectoryInfo(tbExportPath.Text),
                                              ckbExportKeys.Checked,
                                              new System.IO.DirectoryInfo(tbKeyExportPath.Text),
                                              tbExportKey.Text,
                                              cmbSourceDbs.Text,
                                              (PreviousIndigoVersion)((IndigoMigrationToolkit.Objects.ComboBoxItem)cmbSourceVersion.SelectedItem).Value,
                                              new System.IO.DirectoryInfo(tbPostScripts.Text));


            _parentForm.Disbale();
            gbMigrationOptions.Enabled = false;
            var ui = TaskScheduler.FromCurrentSynchronizationContext();

            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Factory.StartNew(() => TaskProgress(cts.Token));

            try
            {
                Task.Factory.StartNew(() => _dbServer.ValidateMigrateData(targetIndigo, MainForm.CommandTimeout, MainForm.MigrationBatchSize))
                    .ContinueWith(task =>
                    {
                        _parentForm.Enable();
                        gbMigrationOptions.Enabled = true;
                        cts.Cancel();
                        progressBar.Value = 0;
                    }, ui);

            }
            catch (Exception ex)
            {
                _loggingWindow.Write(ex.ToString());
            }
        }
    }
}
