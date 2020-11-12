using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catel.Data;

namespace IndigoToolkit2.Models
{
    public class Settings : SavableModelBase<Settings>
    {
        ///// <summary>
        ///// Gets or sets all the families.
        ///// </summary>
        //public ObservableCollection<Family> Families
        //{
        //    get { return GetValue<ObservableCollection<Family>>(FamiliesProperty); }
        //    set { SetValue(FamiliesProperty, value); }
        //}

        ///// <summary>
        ///// Register the Families property so it is known in the class.
        ///// </summary>
        //public static readonly PropertyData FamiliesProperty = RegisterProperty("Families", typeof(ObservableCollection<Family>), () => new ObservableCollection<Family>());
    }
}
