using System.Collections.Generic;
using System.Linq;

namespace indigoCardIssuingWeb.CCO.objects
{
    public class DisplayMenu
    {
        private readonly string _menuName;
        private readonly string _menuValue;
        private readonly string _navigateUrl;
        private readonly bool _selecteable = true;
        private readonly Dictionary<string, DisplayMenu> submenus = new Dictionary<string, DisplayMenu>();
        
        //private FormState _menuFormState;
        private readonly int? _issueMethodId = null;

        public DisplayMenu(string menuName, string navigateUrl, string menuValue)
        {
            _menuName = menuName;
            _navigateUrl = navigateUrl;
            _menuValue = menuValue;
            // _menuFormState = formState;
        }

        public DisplayMenu(string menuName, string navigateUrl, string menuValue, bool selectable)
        {
            _menuName = menuName;
            _navigateUrl = navigateUrl;
            _menuValue = menuValue;
            _selecteable = selectable;
        }

        public DisplayMenu(string menuName, string navigateUrl, string menuValue, int issueMethodId)
        {
            _menuName = menuName;
            _navigateUrl = navigateUrl;
            _menuValue = menuValue;
            // _menuFormState = formState;
            _issueMethodId = issueMethodId;
        }

        public string MenuName
        {
            get { return _menuName; }
        }

        public string MenuValue
        {
            get { return _menuValue; }
        }

        public string NavigateUrl
        {
            get { return _navigateUrl; }
        }

        public bool Selectable
        {
            get { return _selecteable; }
        }

        //public FormState MenuFormState
        //{
        //    get { return _menuFormState; }
        //}

        public int? IssueMethodId
        {
            get { return _issueMethodId; }
        }

        public bool AddSubmenu(DisplayMenu menu)
        {
            if (submenus.ContainsKey(menu.MenuName))
                return false;

            submenus.Add(menu.MenuName, menu);
            return true;
        }

        public bool DeleteSubmenu(DisplayMenu menu)
        {
            if (!submenus.ContainsKey(menu.MenuName))
                return false;

            submenus.Remove(menu.MenuName);
            return true;
        }

        public bool DeleteAllSubmenus()
        {
            if (submenus.Count == 0)
                return false;

            submenus.Clear();
            return true;
        }

        public DisplayMenu[] GetSubmenus()
        {
            return submenus.Values.ToArray();
        }

        public bool HasSubmenus()
        {
            return submenus.Count > 0;
        }
    }
}