using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Models;

namespace Veneka.Indigo.UserManagement.objects
{
    public sealed class StatusFlowRole
    {
        public short RoleId { get; set; }
        public short DistBatchStatusId { get; set; }
        public short FlowDistBatchStatusId { get; set; }
        public short DistBatchTypeId { get; set; }
        public short CardIssueMethodId { get; set; }
        public short MainMenuId { get; set; }
        public short SubMenuId { get; set; }
        public short OrderId { get; set; }
        //public string MenuName { get; set; }
    }

    public sealed class UserRolesAndFlows
    {
        public List<RolesIssuerResult> Roles { get; set; }
        public List<StatusFlowRole> StatusFlows { get; set; }
    }

}
