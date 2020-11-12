namespace IndigoToolkit2.ViewModels
{
    using Catel;
    using Catel.Data;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.Services;
    using Catel.Collections;
    using IndigoToolkit2.Models;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Linq;
    using Microsoft.SqlServer.Dac;
    using System;
    using System.Windows.Controls;
    using System.Threading;

    public class MainWindowViewModel : ViewModelBase
    {       
        private IMessageService _messageService;
        private IOpenFileService _openFileService;
        private ISaveFileService _saveFileService;

        private CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
        private bool _taskInProgress = false;

        public MainWindowViewModel(IMessageService messageService, IOpenFileService openFileService, ISaveFileService saveFileService)
        {
            Argument.IsNotNull(() => messageService);
            Argument.IsNotNull(() => openFileService);

            _messageService = messageService;
            _openFileService = openFileService;
            _saveFileService = saveFileService;

            // Add commands
            DriftReportCmd = new TaskCommand(OnDriftReportCmdExecuteAsync, OnDriftReportCmdCanExecute);
            DeployReportCmd = new TaskCommand(OnDeployReportCmdExecuteAsync, OnDeployReportCmdCanExecute);
            DeployCmd = new TaskCommand(OnDeployCmdExecuteAsync, OnDeployCmdCanExecute);
            DacPacSelectCmd = new TaskCommand(OnDacPacSelectCmdExecuteAsync);
            ProfileSelectCmd = new TaskCommand(OnProfileSelectCmdExecuteAsync);
            SaveMessageLogCmd = new TaskCommand(OnSaveMessageLogCmdExecuteAsync, OnSaveMessageLogCmdCanExecute);
            CancelCmd = new TaskCommand(OnCancelCmdExecuteAsync, OnCancelCmdCanExecute);

            // Create new config details and populate with some default values
            IndigoDatabase = new IndigoDatabase
            {
                ConnectionString = "Server=<ServerName>;Trusted_Connection=True;",
                TargetDatabaseName = "IndigoDatabase"
            };

            DacNotifications = new DacNotifications();
            DacVariables = new DacVariables();
            DacProfileOptions = new DacProfileOptions();
        }

        public override string Title { get { return "Indigo Toolkit v2"; } }

        #region Model Stuff
        /// <summary>
        /// Gets or sets the IndigoDatabase.
        /// </summary>
        [Model]
        public IndigoDatabase IndigoDatabase
        {
            get { return GetValue<IndigoDatabase>(IndigoDatabaseProperty); }
            set { SetValue(IndigoDatabaseProperty, value); }
        }

        /// <summary>
        /// Register the IndigoDatabaseProperty so it is known in the class.
        /// </summary>
        public static readonly PropertyData IndigoDatabaseProperty = RegisterProperty("IndigoDatabase", typeof(IndigoDatabase), null);


        [ViewModelToModel("IndigoDatabase")]
        public string ConnectionString
        {
            get { return GetValue<string>(ConnectionStringProperty); }
            set { SetValue(ConnectionStringProperty, value); }
        }

        public static readonly PropertyData ConnectionStringProperty = RegisterProperty("ConnectionString", typeof(string));

        [ViewModelToModel("IndigoDatabase")]
        public string TargetDatabaseName
        {
            get { return GetValue<string>(TargetDatabaseNameProperty); }
            set { SetValue(TargetDatabaseNameProperty, value); }
        }

        public static readonly PropertyData TargetDatabaseNameProperty = RegisterProperty("TargetDatabaseName", typeof(string));

        [ViewModelToModel("IndigoDatabase")]
        public string DacPacPath
        {
            get { return GetValue<string>(DacPacPathProperty); }
            set { SetValue(DacPacPathProperty, value); }
        }

        public static readonly PropertyData DacPacPathProperty = RegisterProperty("DacPacPath", typeof(string));

        [ViewModelToModel("IndigoDatabase")]
        public string PublishProfilePath
        {
            get { return GetValue<string>(PublishProfilePathProperty); }
            set { SetValue(PublishProfilePathProperty, value); }
        }

        public static readonly PropertyData PublishProfilePathProperty = RegisterProperty("PublishProfilePath", typeof(string));
        #endregion

        #region Notifcation Stuff
        public DacNotifications DacNotifications
        {
            get { return GetValue<DacNotifications>(DacNotificationsProperty); }
            set { SetValue(DacNotificationsProperty, value); }
        }

        public static readonly PropertyData DacNotificationsProperty = RegisterProperty("DacNotifications", typeof(DacNotifications));
        #endregion

        #region Deployment Options Stuff
        public DacProfileOptions DacProfileOptions
        {
            get { return GetValue<DacProfileOptions>(DacProfileOptionsProperty); }
            set { SetValue(DacProfileOptionsProperty, value); }
        }

        public static readonly PropertyData DacProfileOptionsProperty = RegisterProperty("DacProfileOptions", typeof(DacProfileOptions));
        #endregion

        #region SQLCMD Var Stuff
        public DacVariables DacVariables
        {
            get { return GetValue<DacVariables>(DacVariablesProperty); }
            set { SetValue(DacVariablesProperty, value); }
        }

        public static readonly PropertyData DacVariablesProperty = RegisterProperty("DacVariables", typeof(DacVariables));
        #endregion


        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

        #region Initialise & Close
        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            // TODO: subscribe to events here
        }

        protected override async Task CloseAsync()
        {
            // TODO: unsubscribe from events here

            _cancelTokenSource.Dispose();

            await base.CloseAsync();
        }
        #endregion

        #region Helpful Methods
        private bool CheckFieldsAndTaskRunning()
        {
            if (IndigoDatabase.CheckAllFieldPopulated() && !_taskInProgress)
            {
                return true;
            }

            return false;
        }

        private void StartTask()
        {
            _taskInProgress = true;
            ViewModelCommandManager.InvalidateCommands(true);
        }

        private void EndTask()
        {
            _taskInProgress = false;
            ViewModelCommandManager.InvalidateCommands(true);
        }

        private void SubScribeProgress(DacServices dacService)
        {
            dacService.ProgressChanged +=
              new EventHandler<DacProgressEventArgs>((s, e) => DacNotifications.AddProgress(e.Status + " " + e.Message));
            dacService.Message +=
              new EventHandler<DacMessageEventArgs>((s, e) =>
              {
                  DacNotifications.AddProgress(e.Message.MessageType + " " + e.Message.Number + " " + e.Message.Prefix + " " + e.Message.Message);
              });
        }
        #endregion

        #region DacPacSelectCmd
        public TaskCommand DacPacSelectCmd { get; private set; }

        private async Task OnDacPacSelectCmdExecuteAsync()
        {
            _openFileService.Filter = "DACPAC files|*.dacpac";
            _openFileService.IsMultiSelect = false;
            _openFileService.Title = "Select DACPAC file";

            var result = await _openFileService.DetermineFileAsync();

            if(result)
            {
                IndigoDatabase.DacPacPath = _openFileService.FileName;
            }
        }
        #endregion

        #region ProfileSelectCmd
        public TaskCommand ProfileSelectCmd { get; private set; }

        private async Task OnProfileSelectCmdExecuteAsync()
        {
            _openFileService.FileName = "";
            _openFileService.Filter = "Profile files|*.publish.xml";
            _openFileService.IsMultiSelect = false;
            _openFileService.Title = "Select Profile file";

            try
            {
                var result = await _openFileService.DetermineFileAsync();

                if (result)
                {
                    IndigoDatabase.PublishProfilePath = _openFileService.FileName;

                    // Now try import into DacProfile
                    var profile = DacProfile.Load(IndigoDatabase.PublishProfilePath);
                    
                    // Getting variables for UI
                    DacVariables.SetCollection(profile.DeployOptions.SqlCommandVariableValues);

                    // Getting options for UI
                    DacProfileOptions.SetCollection(profile.DeployOptions);
                }
            }
            catch(Exception ex)
            {
                _messageService.ShowErrorAsync(ex);
            }
        }
        #endregion

        #region DriftReportCmd
        public bool OnDriftReportCmdCanExecute()
        {
            return CheckFieldsAndTaskRunning();
        }

        /// <summary>
        /// Gets the DriftReportCmd command.
        /// </summary>
        public TaskCommand DriftReportCmd { get; private set; }

        /// <summary>
        /// Method to invoke when the DriftReportCmd command is executed.
        /// </summary>
        /// <param name="parameter">The parameter of the command.</param>
        private async Task OnDriftReportCmdExecuteAsync()
        {
            StartTask();

            await Task.Run(() => GetDrift());
            
            // Show message box
            await _messageService.ShowInformationAsync("DriftReportCmd complete.");

            EndTask();
        }

        private void GetDrift()
        {
            var dacService = new DacServices(IndigoDatabase.ConnectionString);

            SubScribeProgress(dacService);

            try
            {
                using (DacPackage dacpac = DacPackage.Load(IndigoDatabase.DacPacFile.FullName))
                {
                    
                    DacNotifications.AddProgress(dacService.GenerateDriftReport(IndigoDatabase.TargetDatabaseName));                    
                }
            }
            catch (DacServicesException dsEx)
            {
                DacNotifications.AddProgress(dsEx.Flatten());
                _messageService.ShowErrorAsync(dsEx.Flatten());
            }
            catch (Exception ex)
            {
                DacNotifications.AddProgress(ex.ToString());
                _messageService.ShowErrorAsync(ex);
            }
        }
        #endregion

        #region DeployReportCmd
        public bool OnDeployReportCmdCanExecute()
        {
            return CheckFieldsAndTaskRunning();
        }

        /// <summary>
        /// Gets the DriftReportCmd command.
        /// </summary>
        public TaskCommand DeployReportCmd { get; private set; }

        /// <summary>
        /// Method to invoke when the DriftReportCmd command is executed.
        /// </summary>
        /// <param name="parameter">The parameter of the command.</param>
        private async Task OnDeployReportCmdExecuteAsync()
        {
            StartTask();

            await Task.Run(() => GetDeployReport());

            // Show message box
            await _messageService.ShowInformationAsync("Deploy Report Complete");

            EndTask();
        }

        private void GetDeployReport()
        {
           
            _cancelTokenSource = new CancellationTokenSource();            

            var dacService = new DacServices(IndigoDatabase.ConnectionString);
            SubScribeProgress(dacService);

            try
            {
                using (DacPackage dacpac = DacPackage.Load(IndigoDatabase.DacPacFile.FullName))
                {
                    // Grab the deploy options from the pubilish profile
                    var profile = DacProfile.Load(IndigoDatabase.PublishProfilePath);

                    // Set the SQLCMD variables back to the profile
                    DacVariables.SetDictionary(profile.DeployOptions.SqlCommandVariableValues);                    

                    // Generate the report
                    var deployReport = dacService.GenerateDeployReport(dacpac, IndigoDatabase.TargetDatabaseName, options: profile.DeployOptions, cancellationToken: _cancelTokenSource.Token);
                                        
                    _saveFileService.Filter = "XML File|*.xml";                    
                    
                    var result = _saveFileService.DetermineFileAsync();

                    if (result.Result)
                    {
                        System.IO.File.WriteAllText(_saveFileService.FileName, deployReport);
                    }
                }
            }
            catch (DacServicesException dsEx)
            {
                DacNotifications.AddProgress(dsEx.Flatten());
                _messageService.ShowErrorAsync(dsEx.Flatten());
            }
            catch (Exception ex)
            {
                DacNotifications.AddProgress(ex.ToString());
                _messageService.ShowErrorAsync(ex);
            }
        }
        #endregion

        #region DeployCmd
        public bool OnDeployCmdCanExecute()
        {
            return CheckFieldsAndTaskRunning();
        }

        /// <summary>
        /// Gets the PublishCmd command.
        /// </summary>
        public TaskCommand DeployCmd { get; private set; }

        /// <summary>
        /// Method to invoke when the PublishCmd command is executed.
        /// </summary>
        /// <param name="parameter">The parameter of the command.</param>
        private async Task OnDeployCmdExecuteAsync()
        {
            StartTask();

            await Task.Run(() => DoDeploy());

            // Show message box
            await _messageService.ShowInformationAsync("Deploy Completed");

            EndTask();
        }              

        private void DoDeploy()
        {
            var dacService = new DacServices(IndigoDatabase.ConnectionString);

            SubScribeProgress(dacService);

            try
            {
                _cancelTokenSource = new CancellationTokenSource();

                using (DacPackage dacpac = DacPackage.Load(IndigoDatabase.DacPacFile.FullName))
                {
                    DacNotifications.AddProgress(dacpac.Version.ToString());                      

                    // Grab the deploy options from the pubilish profile
                    var profile = DacProfile.Load(IndigoDatabase.PublishProfilePath);

                    // Set the SQLCMD variables back to the profile
                    DacVariables.SetDictionary(profile.DeployOptions.SqlCommandVariableValues);

                    // hit the deploy
                    dacService.Deploy(dacpac, 
                                        IndigoDatabase.TargetDatabaseName,
                                        upgradeExisting: true,
                                        options: profile.DeployOptions, cancellationToken: _cancelTokenSource.Token);
                }
            }
            catch (DacServicesException dsEx)
            {
                DacNotifications.AddProgress(dsEx.Flatten());
                _messageService.ShowErrorAsync(dsEx.Flatten());
            }
            catch (Exception ex)
            {
                DacNotifications.AddProgress(ex.ToString());
                _messageService.ShowErrorAsync(ex);
            }
        }
        #endregion

        #region SaveMessageLogCmd
        public bool OnSaveMessageLogCmdCanExecute()
        {
            return !String.IsNullOrWhiteSpace(DacNotifications.Progress);
        }

        /// <summary>
        /// Gets the SaveMessageLogCmd command.
        /// </summary>
        public TaskCommand SaveMessageLogCmd { get; private set; }

        /// <summary>
        /// Method to invoke when the SaveMessageLogCmd command is executed.
        /// </summary>
        /// <param name="parameter">The parameter of the command.</param>
        private async Task OnSaveMessageLogCmdExecuteAsync()
        {
            await Task.Run(() => SaveMessageLog());

            // Show message box
            //await _messageService.ShowInformationAsync("Deploy Report Complete" + System.Environment.NewLine + IndigoDatabase.ToString());
        }
        private void SaveMessageLog()
        {
            try
            {
                _saveFileService.Filter = "TXT File|*.txt";

                var result = _saveFileService.DetermineFileAsync();

                System.IO.File.WriteAllText(_saveFileService.FileName, DacNotifications.Progress);
            }
            catch (Exception ex)
            {
                _messageService.ShowErrorAsync(ex);
            }
        }
        #endregion

        #region CancelCmd
        public bool OnCancelCmdCanExecute()
        {
            return _taskInProgress;
            //return IndigoDatabase.CheckAllFieldPopulated();
        }

        /// <summary>
        /// Gets the SaveMessageLogCmd command.
        /// </summary>
        public TaskCommand CancelCmd { get; private set; }

        /// <summary>
        /// Method to invoke when the SaveMessageLogCmd command is executed.
        /// </summary>
        /// <param name="parameter">The parameter of the command.</param>
        private async Task OnCancelCmdExecuteAsync()
        {
            await Task.Run(() =>
            {
                DacNotifications.AddProgress("Task cancelation requested. Please wait.");
                _cancelTokenSource.Cancel(true);
            });

            // Show message box
            //await _messageService.ShowInformationAsync("Deploy Report Complete" + System.Environment.NewLine + IndigoDatabase.ToString());
        }
        #endregion

        
    }
}
