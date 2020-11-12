namespace Veneka.Indigo.UserManagement.objects
{
    public class UserGroup
    {
        private readonly int _issuerID;
        private readonly string _userGroupName;
        private int[] _userRoles;

        public UserGroup(string userGroupName, int[] userRoles, int issuerID)
        {
            UserRoles = userRoles;
            _userGroupName = userGroupName;
            _issuerID = issuerID;
        }

        public int[] UserRoles
        {
            get { return _userRoles; }
            private set
            {
                if (value == null || value.Length == 0)
                {
                    _userRoles = null;
                }
                else
                {
                    _userRoles = value;
                }
            }
        }

        public string[] UserRolesList
        {
            get
            {
                var roleList = new string[_userRoles.Length];

                for (int i = 0; i < _userRoles.Length; i++)
                {
                    roleList[i] = _userRoles[i].ToString();
                    ;
                }
                return roleList;
            }
        }

        public string UserGroupName
        {
            get { return _userGroupName; }
        }

        public int IssuerID
        {
            get { return _issuerID; }
        }
    }
}