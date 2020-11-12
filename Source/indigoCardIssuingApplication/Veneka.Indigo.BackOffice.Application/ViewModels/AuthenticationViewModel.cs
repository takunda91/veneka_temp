using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Security;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.BackOffice.Application.Authentication;
using System.Threading;
using Veneka.Indigo.BackOffice.Application.Views;
using Veneka.Indigo.BackOffice.Application.Authentication.Test;
using Veneka.Indigo.BackOffice.Application.BackOfficeApiClient;
using System.Windows;
using System.Configuration;
using Veneka.Indigo.BackOffice.Application.Properties;
using Common.Logging;

namespace Veneka.Indigo.BackOffice.Application.ViewModels
{
    public class AuthenticationViewModel: ViewModel
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly DelegateCommand _loginCommand;
        private readonly DelegateCommand _logoutCommand;
        private readonly DelegateCommand _showViewCommand;
        private string _username;
        private string _status;
        private IViewModelNavigation _navigation;
        private static readonly ILog log = LogManager.GetLogger(typeof(AuthenticationViewModel));
        public AuthenticationViewModel()
        {
           
            _authenticationService = new BasicAuthService(new Uri(Uri));
            _loginCommand = new DelegateCommand(Login, CanLogin);
            _logoutCommand = new DelegateCommand(Logout, CanLogout);
            _showViewCommand = new DelegateCommand(ShowView, null);
        }

        public AuthenticationViewModel(IViewModelNavigation navigation) : this()
        {
            _navigation = navigation;
        }

        #region Properties
        public string Username
        {
            get { return _username; }
            set { _username = value; NotifyPropertyChanged("Username"); }
        }

        public string AuthenticatedUser
        {
            get
            {
                if (IsAuthenticated)
                    return string.Format("Signed in as {0}. {1}",
                          Thread.CurrentPrincipal.Identity.Name,
                          Thread.CurrentPrincipal.IsInRole("Administrators") ? "You are an administrator!"
                              : "You are NOT a member of the administrators group.");

                return "Not authenticated!";
            }
        }

        public string Status
        {
            get { return _status; }
            set { _status = value; NotifyPropertyChanged("Status"); }
        }
        #endregion

        #region Commands
        public DelegateCommand LoginCommand { get { return _loginCommand; } }

        public DelegateCommand LogoutCommand { get { return _logoutCommand; } }

        public DelegateCommand ShowViewCommand { get { return _showViewCommand; } }
        #endregion

        private void Login(object parameter)
        {
            PasswordBox passwordBox = parameter as PasswordBox;
            string clearTextPassword = passwordBox.Password;
            try
            {
                //Validate credentials through the authentication service
                BackOfficeAppUser user = _authenticationService.AuthenticateUser(Username, clearTextPassword);

                if(user.Token==null)
                    throw new ArgumentException("Login failed! Please provide some valid credentials.");

                //Get the current principal object
                BackOfficeAppPrincipal customPrincipal = Thread.CurrentPrincipal as BackOfficeAppPrincipal;
                if (customPrincipal == null)
                    throw new ArgumentException("The application's default thread principal must be set to a CustomPrincipal object on startup.");

              
                //Authenticate the user
                customPrincipal.Identity = new BackOfficeAppIdentity(user.Username, user.Email, user.Roles,user.Token);

                //Update UI
                NotifyPropertyChanged("AuthenticatedUser");
                NotifyPropertyChanged("IsAuthenticated");
                _loginCommand.RaiseCanExecuteChanged();
                _logoutCommand.RaiseCanExecuteChanged();
                Username = string.Empty; //reset
                passwordBox.Password = string.Empty; //reset
                Status = string.Empty;

                _navigation.ChangeViewModel(new BulkPrintingViewModel());               
            }
            catch (UnauthorizedAccessException ex)
            {
                log.Debug(ex.Message);
                Status = "Login failed! Please provide some valid credentials.";

                MessageBox.Show(ex.Message, "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message);
                Status = string.Format("ERROR: {0}", ex.Message);
                MessageBox.Show(Status, "Alert", MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }

        private bool CanLogin(object parameter)
        {
            return !IsAuthenticated;
        }

        private void Logout(object parameter)
        {
            BackOfficeAppPrincipal customPrincipal = Thread.CurrentPrincipal as BackOfficeAppPrincipal;
            if (customPrincipal != null)
            {
                customPrincipal.Identity = new AnonymousIdentity();
                NotifyPropertyChanged("AuthenticatedUser");
                NotifyPropertyChanged("IsAuthenticated");
                _loginCommand.RaiseCanExecuteChanged();
                _logoutCommand.RaiseCanExecuteChanged();
                Status = string.Empty;
            }
        }

        private bool CanLogout(object parameter)
        {
            return IsAuthenticated;
        }



        private void ShowView(object parameter)
        {
            try
            {
                Status = string.Empty;
                IView view;
                view = new AuthenticationView();
                //if (parameter == null)
                //    view = new SecretWindow();
                //else
                //    view = new AdminWindow();

                //view.Show();
            }
            catch (SecurityException)
            {
                Status = "You are not authorized!";
            }
        }

        public string Uri { get { return Settings.Default.Uri; } }
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
