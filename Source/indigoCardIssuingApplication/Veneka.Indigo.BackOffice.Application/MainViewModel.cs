using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Veneka.Indigo.BackOffice.Application.Authentication;
using Veneka.Indigo.BackOffice.Application.Authentication.Test;
using Veneka.Indigo.BackOffice.Application.BackOfficeApiClient;
using Veneka.Indigo.BackOffice.Application.Crypto;
using Veneka.Indigo.BackOffice.Application.ViewModels;
using Veneka.Indigo.BackOffice.Application.Views;

namespace Veneka.Indigo.BackOffice.Application
{
    public class MainViewModel : ViewModel, IViewModelNavigation
    {   
        private readonly DelegateCommand _logoutCommand;
        private bool _showlogoutButton;
        // https://msdn.microsoft.com/en-us/magazine/dd419663.aspx
        // Navigation: https://stackoverflow.com/questions/19654295/wpf-mvvm-navigate-views

        public MainViewModel()
        {
            try
            {

                _showlogoutButton = IsAuthenticated;

                //NotifyPropertyChanged("IsAuthenticated");
                _logoutCommand = new DelegateCommand(Logout, CanLogout);
                AuthViewModel = new AuthenticationViewModel(this);
            }
             catch(SecurityException secEx)
            {
                
            }
        }

      
      
        public DelegateCommand LogoutCommand { get { return _logoutCommand; } }

        public bool ShowLogoutButton
        {
            get { return _showlogoutButton; }
            set
            {
                _showlogoutButton = value;
                OnPropertyChanged("ShowLogoutButton");
            }

        }
        private bool CanLogout(object parameter)
        {

            return IsAuthenticated;
        }
        private void Logout(object parameter)
        {
            BackOfficeAppPrincipal customPrincipal = Thread.CurrentPrincipal as BackOfficeAppPrincipal;
            if (customPrincipal != null)
            {
                customPrincipal.Identity = new AnonymousIdentity();
             
                _logoutCommand.RaiseCanExecuteChanged();
                ChangeViewModel(new AuthenticationViewModel());

            }
        }
        #region Navigation
        private ICommand _changePageCommand;
        private IViewModel _authViewModel;
        private IViewModel _currentPageViewModel;
        private List<IViewModel> _pageViewModels;


        public void ChangeViewModel(object viewModel)
        {
            if (viewModel is IViewModel)
            {
                CurrentPageViewModel = (IViewModel)viewModel;
            }

            throw new Exception(viewModel + " is not of Type IViewModel");
        }

        private IViewModel CheckIdentity(IViewModel viewModel)
        {
            var identity = Thread.CurrentPrincipal.Identity;

            if (identity is AnonymousIdentity)
            {
                return new AuthenticationViewModel(this);
            }

            return viewModel;
        }

        public ICommand ChangePageCommand
        {
            get
            {
                if (_changePageCommand == null)
                {
                    _changePageCommand = new DelegateCommand(ChangeViewModel);
                }

                return _changePageCommand;
            }
        }

        public List<IViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IViewModel>();

                return _pageViewModels;
            }
        }

        public IViewModel AuthViewModel
        {
            get
            {
                if (IsAuthenticated)
                {                    
                    return null;                    
                }

                return _authViewModel;
            }
            set
            {
                if (_authViewModel != value)
                {
                    _authViewModel = value;
                    //_currentPageViewModel = CheckIdentity(value);
                    OnPropertyChanged(nameof(AuthViewModel));
                }
            }
        }
        public string AuthenticatedUser
        {
            get
            {
                if (IsAuthenticated)
                    return string.Format("Signed in as {0}",
                          Thread.CurrentPrincipal.Identity.Name);

                return "Not authenticated!";
            }
        }
        public IViewModel CurrentPageViewModel
        {
            get
            {
                if (IsAuthenticated)
                {
                    AuthViewModel = null;
                    return _currentPageViewModel;
                }
                
                return null;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    //_currentPageViewModel = CheckIdentity(value);
                    OnPropertyChanged(nameof(CurrentPageViewModel));
                }
            }
        }
        #endregion
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
