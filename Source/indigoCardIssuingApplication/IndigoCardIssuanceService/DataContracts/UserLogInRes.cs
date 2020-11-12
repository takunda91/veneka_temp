using IndigoCardIssuanceService.bll;
using Veneka.Indigo.Common.Models;
using System.Collections.Generic;
using Veneka.Indigo.UserManagement.objects;
using System;

namespace IndigoCardIssuanceService.DataContracts
{
    public class UserLogInRes
    {        
        public string encryptedUserId { get; set; }
        public string encryptedUsername { get; set; }
        public string encryptedName { get; set; }
        public string encryptedLastname { get; set; }
        public string encryptedSessionKey { get; set; }

        public bool ChangePassword { get; set; }
        public int LanguageId { get; set; }
        public bool ldapUser { get; set; }

        public bool multifactorUser { get; set; }

        public int? authConfigurationId { get; set; }

        public DateTime PasswordExpirydate { get; set; }

        public List<RolesIssuerResult> Roles { get; set; }
        public List<StatusFlowRole> StatusFlow { get; set; }
    }
}