using IndigoDesktopApp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace IndigoDesktopApp
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            // Add available pages
            //PageViewModels.Add(new HomeViewModel());
            //PageViewModels.Add(new PinSelectViewModel());

            // Set starting page
            if (StartupProperties.Action == null)
            {
                CurrentPageViewModel = new HomeViewModel();//PageViewModels[0];
            }
            else
            {
                switch(StartupProperties.Action)
                {
                    case StartupProperties.Actions.PINSelect:
                        CurrentPageViewModel = new PinSelectViewModel();
                        break;
                    case StartupProperties.Actions.PrintCard:
                        CurrentPageViewModel = new PrintViewModel();
                        break;
                    default: break;
                }
            }
        }


        private ICommand _changePageCommand;

        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;

        #region Properties / Commands

        public ICommand ChangePageCommand
        {
            get
            {
                if (_changePageCommand == null)
                {
                    _changePageCommand = new RelayCommand(ChangeViewModel);
                    //    p => ChangeViewModel((IPageViewModel)p),
                    //    p => p is IPageViewModel);
                }

                return _changePageCommand;
            }
        }

        public List<IPageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IPageViewModel>();

                return _pageViewModels;
            }
        }

        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged("CurrentPageViewModel");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Methods

        private void ChangeViewModel(object obj)
        {
            // if (!PageViewModels.Contains(viewModel))
            //     PageViewModels.Add(viewModel);

            //CurrentPageViewModel = PageViewModels
            //    .FirstOrDefault(vm => vm == viewModel);

            switch (obj.ToString().ToUpper())
            {
                case "HOME": CurrentPageViewModel = new HomeViewModel(); break;
                case "PRINT": CurrentPageViewModel = new PrintViewModel(); break;
                case "PINSET": CurrentPageViewModel = new PinSelectViewModel(); break;
                case "DEVICES": CurrentPageViewModel = new DevicesViewModel(); break;
                default:
                    break;
            }
            //CurrentPageViewModel
        }

        #endregion
    }
}
