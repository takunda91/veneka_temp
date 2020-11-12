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
    public partial class CreateDatabaseForm : Form
    {
        private SqlServer _dbServer;
        private WindowLogging _loggingWindow;
        private IParentForm _parentForm;

        public CreateDatabaseForm(SqlServer dbServer, WindowLogging loggingWindow, IParentForm parentForm)
        {
            InitializeComponent();

            _dbServer = dbServer;
            _loggingWindow = loggingWindow;
            _parentForm = parentForm;

            if (_dbServer.Connected)
            {
                this.gbCreateDB.Enabled = true;

                var dbs = _dbServer.DatabaseList().ToArray();
                cmbTargetDatabase.Items.AddRange(dbs);                

                cmbTargetCollation.Items.AddRange(_dbServer.ServerCollations.ToArray());
            }
        }

        private void btnCreateDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                if (_dbServer.Exists(cmbTargetDatabase.Text))
                {
                    _loggingWindow.WriteFormat("{0} already exists on server.", cmbTargetDatabase.Text);

                    MessageBox.Show("Database already exists on server!",
                                    "Database Exists",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Stop);
                    return;
                }

                var targetIndigo = new IndigoInfo(
                                              cmbTargetDatabase.Text,
                                              cmbTargetCollation.Text,
                                              tbMasterKey.Text,                                              
                                              new System.IO.DirectoryInfo(tbExportPath.Text),
                                              ckbExportKeys.Checked,
                                              new System.IO.DirectoryInfo(tbKeyExportPath.Text),
                                              tbExportKey.Text,                                              
                                              true);

                var confirmResult = MessageBox.Show(String.Format("This will create new database {0} on {1}, are you sure?", targetIndigo.DatabaseName, _dbServer.ServerName),
                                         "Confirm Create Database",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

                if (confirmResult == DialogResult.No)
                    return;

                _parentForm.Disbale();
                var ui = TaskScheduler.FromCurrentSynchronizationContext();

                CancellationTokenSource cts = new CancellationTokenSource();
                Task.Factory.StartNew(() => TaskProgress(cts.Token));

                Task.Factory.StartNew(() => _dbServer.CreateNewDatabase(targetIndigo))
                    .ContinueWith(task =>
                    {
                        _parentForm.Enable();
                        cts.Cancel();
                        progressBar.Value = 0;
                        cmbTargetDatabase.Items.Clear();
                        var dbs = _dbServer.DatabaseList().ToArray();
                        cmbTargetDatabase.Items.AddRange(dbs);                        
                    }, ui);
            }
            catch (Exception ex)
            {
                _loggingWindow.Write(ex.ToString());                
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

        private void CreateDatabaseForm_Load(object sender, EventArgs e)
        {
            cmbTargetCollation.SelectedText = _dbServer.DefaultServerCollation;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _parentForm.Disbale();
        }
    }
}
