using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EProcess.Indigo._2._0._0.Migration.Common.Models
{
    public class Groups
    {
        public int user_group_id { get; set; }
        public string user_group_name { get; set; }
        public bool can_create { get; set; }
        public bool can_read { get; set; }
        public bool can_update { get; set; }
        public bool can_delete { get; set; }
        public bool all_branch_access { get; set; }
    }
}
