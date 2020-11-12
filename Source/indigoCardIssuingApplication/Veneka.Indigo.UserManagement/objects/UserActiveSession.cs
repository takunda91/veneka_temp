using System;

namespace Veneka.Indigo.UserManagement.objects
{
    public class UserActiveSession
    {
        private int _online;
        private string _workstation;

        public UserActiveSession(int online, string workstation)
        {
            this._online = online;
            this._workstation = workstation;
        }

        public int Online
        {
            get { return this._online; }
        }

        public string Workstation
        {
            get { return this._workstation; }
        }
    }
}