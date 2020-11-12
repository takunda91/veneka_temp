using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Models;
using Common.Logging;
using System.Data.Objects;

namespace Veneka.Indigo.UserManagement.dal
{
    public class UserRepository
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UserRepository));

        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;

        
    }
}
