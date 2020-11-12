using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Veneka.Indigo.BackOffice.Application.Authentication;
using Veneka.Indigo.BackOffice.Application.ViewModels;
using Veneka.Indigo.BackOffice.Application.Views;

namespace Veneka.Indigo.BackOffice.Application
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //Create a custom principal with an anonymous identity at startup
            BackOfficeAppPrincipal customPrincipal = new BackOfficeAppPrincipal();
            AppDomain.CurrentDomain.SetThreadPrincipal(customPrincipal);

            base.OnStartup(e);            
        }
    }
}
