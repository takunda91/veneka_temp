using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.BackOffice.Application.Objects;

namespace Veneka.Indigo.BackOffice.Application.DAL
{
    public interface ILocalStorage
    {
        // https://www.codeproject.com/Tips/988690/WinForms-WPF-Using-SQLite-DataBase
        void Init();
        void Get(DataTable dataTable, string SelectTaskQuery, List<Parameters> lst);

        void UpdateChanges(string SelectTaskQuery, List<Parameters> lst);

        SQLiteConnection GetConnection();
    }
}
