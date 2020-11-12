using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using Common.Logging;
using System.Threading;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Security;
using System.Security.Permissions;
using System.Data;
using System.Reflection;

namespace indigoCardIssuingWeb.webpages.system
{
    public partial class IssuerInterface : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(IssuerInterface));
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ADMINISTRATOR };
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();
        private bool LoadDataEmpty = false;
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                hdnActiveTab.Value = "0";
                LoadPageData();
            }

            tbAuthPassword.Attributes.Add("value", Password);
        }

        #region Helper Methods
        private void LoadPageData()
        {
            try
            {
                bool canRead;
                bool canUpdate;
                bool canCreate;

                //Check users roles to make sure he didnt try and get to the page by typing in the address of this page
                if (PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage))
                {
                    //this.ddlDocType.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));

                    //this.ddlConnectionType.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                    foreach (var item in _issuerMan.LangLookupConnectionParameterType())
                    {
                        this.ddlConnectionType.Items.Add(new ListItem(item.language_text, item.lookup_id.ToString()));
                    }

                    //this.ddlAuthType.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                    foreach (AuthenticationTypes item in Enum.GetValues(typeof(AuthenticationTypes)))
                    {
                        this.ddlAuthType.Items.Add(new ListItem(item.ToString(), ((int)item).ToString()));
                    }

                    //this.ddlProtocol.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                    foreach (ConnectionProtocol item in Enum.GetValues(typeof(ConnectionProtocol)))
                    {
                        this.ddlProtocol.Items.Add(new ListItem(item.ToString(), ((int)item).ToString()));
                    }

                    //this.ddlFileEncryption.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "0"));
                    //foreach (FileEncryptionType item in Enum.GetValues(typeof(FileEncryptionType)))
                    //{
                    //    this.ddlFileEncryption.Items.Add(new ListItem(item.ToString(), ((int)item + 1).ToString()));
                    //}

                    foreach (var item in _issuerMan.LangLookupFileEncryptionTypes())
                    {
                        this.ddlFileEncryption.Items.Add(new ListItem(item.language_text, item.lookup_id.ToString()));
                    }

                    LoadConnectionParams();
                    TypeParameters();

                    if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.ADMINISTRATOR, out canRead, out canUpdate, out canCreate))
                    {
                        if (ConnectionParameterId > 0)
                        {
                            if (canUpdate)
                            {
                                this.btnCreate.Visible = this.btnCreate.Enabled = false;
                                this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                                this.btnEdit.Visible = this.btnEdit.Enabled = true;
                                this.btnDelete.Visible = this.btnDelete.Enabled = true;
                                this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                                this.btnCancel.Visible = this.btnCancel.Enabled = false;

                            }
                            else
                            {
                                this.btnCreate.Visible = this.btnCreate.Enabled = false;
                                this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                                this.btnEdit.Visible = this.btnEdit.Enabled = false;
                                this.btnDelete.Visible = this.btnDelete.Enabled = false;
                                this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                                this.btnCancel.Visible = this.btnCancel.Enabled = false;
                            }
                            pageLayout = PageLayout.UPDATE;
                        }
                        else
                        {
                            if (canCreate)
                            {
                                this.btnCreate.Visible = this.btnCreate.Enabled = true;
                                this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                                this.btnDelete.Visible = this.btnDelete.Enabled = false;
                                this.btnEdit.Visible = this.btnEdit.Enabled = false;
                                this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                                this.btnCancel.Visible = this.btnCancel.Enabled = false;
                            }
                            else
                            {
                                this.btnCreate.Visible = this.btnCreate.Enabled = false;
                                this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                                this.btnEdit.Visible = this.btnEdit.Enabled = false;
                                this.btnDelete.Visible = this.btnDelete.Enabled = false;
                                this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                                this.btnCancel.Visible = this.btnCancel.Enabled = false;
                            }
                            pageLayout = PageLayout.CREATE;
                        }
                    }
                }
                else
                {
                    FormElements(false, true);

                    if (!log.IsTraceEnabled)
                    {
                        log.Warn("A user tried to access a page that he/she does not have access to.");
                    }

                    log.Trace(m => m(String.Format("User {0} tried to access a page he/she does not have access to.", User.Identity.Name)));
                    lblErrorMessage.Text = Resources.DefaultExceptions.UnauthorisedPageAccessMessage;
                    return;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.Message;
                }
            }
        }

        private void LoadConnectionParams()
        {
            //var results = _issuerMan.GetConnectionParameters();
            //ConnectionParams = results;

            if (SessionWrapper.InterfaceConnectionID.HasValue)
            {
                ConnectionParameterId = SessionWrapper.InterfaceConnectionID.Value;
                SessionWrapper.InterfaceConnectionID = null;
            }
            else
            {
                ConnectionParameterId = null;
            }

            LoadConnectionDetails();
        }

        private ConnectionParametersResult ConstructConnectionObject()
        {
            ConnectionParametersResult result = new ConnectionParametersResult();

            ConnectionParamsResult rtnConn = new ConnectionParamsResult();
            if (ConnectionParameterId != null && ConnectionParameterId != 0)
            {
                rtnConn.connection_parameter_id = ConnectionParameterId.Value;
            }

            //set defaults
            rtnConn.protocol = ((int)ConnectionProtocol.TCPIP);
            rtnConn.auth_type = (int)AuthenticationTypes.None;
            rtnConn.port = 0;
            rtnConn.is_external_auth = false;

            rtnConn.connection_parameter_type_id = int.Parse(this.ddlConnectionType.SelectedValue);
            rtnConn.address = this.tbConnectionAddress.Text.Trim();
            rtnConn.connection_name = this.tbConnectionName.Text.Trim();
            rtnConn.path = this.tbPath.Text;
            rtnConn.identification = tbIdentification.Text;

            rtnConn.name_of_file = this.tbNameOfFile.Text;

            rtnConn.duplicate_file_check_YN = this.chkDuplicateCheck.Checked;
            rtnConn.file_delete_YN = this.chkDeleteCardFile.Checked;

            rtnConn.username = this.tbAuthUsername.Text.Trim();
            rtnConn.password = Password;
            rtnConn.nonce = this.tbAuthnonce.Text;
            rtnConn.remote_username = this.tbRemoteUsername.Text.Trim();
            rtnConn.remote_password = this.tbRemotePassword.Text.Trim();

            rtnConn.file_encryption_type_id = int.Parse(this.ddlFileEncryption.SelectedValue);

            if (rtnConn.file_encryption_type_id > 0)
            {
                if (SessionWrapper.FilePrivateKey != null)
                {
                    rtnConn.private_key = System.Text.Encoding.UTF8.GetString(SessionWrapper.FilePrivateKey);
                    SessionWrapper.FilePrivateKey = null;
                }

                if (SessionWrapper.FilePublicKey != null)
                {
                    rtnConn.public_key = System.Text.Encoding.UTF8.GetString(SessionWrapper.FilePublicKey);
                    SessionWrapper.FilePublicKey = null;
                }
            }

            if (int.Parse(this.ddlConnectionType.SelectedValue) == 0)
            {
                rtnConn.port = int.Parse(this.tbConnectionPort.Text.Trim());
                rtnConn.auth_type = int.Parse(this.ddlAuthType.SelectedValue);
                rtnConn.protocol = int.Parse(this.ddlProtocol.SelectedValue);

                rtnConn.is_external_auth = chkexternalauth.Checked;

                if (!String.IsNullOrWhiteSpace(tbRemotePort.Text))
                    rtnConn.remote_port = int.Parse(tbRemotePort.Text);

                ////if (rtnConn.auth_type > 0)
                ////{
                //    rtnConn.password = Password;
                //    rtnConn.username = this.tbAuthUsername.Text.Trim();
                ////}
            }
            else if (int.Parse(this.ddlConnectionType.SelectedValue) == 2)
            {
                rtnConn.port = int.Parse(this.tbConnectionPort.Text.Trim());
                rtnConn.header_length = int.Parse(tbHeaderLength.Text);

                rtnConn.timeout_milli = int.Parse(this.tbTimeout.Text);

                if (!String.IsNullOrWhiteSpace(this.tbBufferSize.Text))
                    rtnConn.buffer_size = int.Parse(this.tbBufferSize.Text);

                rtnConn.doc_type = this.ddlDocType.SelectedValue;
            }
            else if (int.Parse(this.ddlConnectionType.SelectedValue) == 3)
            {
                rtnConn.port = int.Parse(this.tbConnectionPort.Text.Trim());
                rtnConn.timeout_milli = int.Parse(this.tbTimeout.Text);
            }
            else if (int.Parse(this.ddlConnectionType.SelectedValue) == 4)
            {
                rtnConn.port = int.Parse(this.tbConnectionPort.Text.Trim());
                rtnConn.domain_name = tbdominname.Text;
                rtnConn.is_external_auth = true;
            }
            TypeParameters();

            result.ConnectionParams = rtnConn;
            result.additionaldata = PopulateAdditionalData();

            return result;
        }
        private ConnectionParamAdditionalDataResult[] PopulateAdditionalData()
        {
            List<ConnectionParamAdditionalDataResult> _additionaldata = new List<ConnectionParamAdditionalDataResult>();
            foreach (DataRow row in AdditionalData.Rows)
            {

                ConnectionParamAdditionalDataResult additionaldata = new ConnectionParamAdditionalDataResult();

                additionaldata.key = row["key"].ToString();
                additionaldata.value = row["value"].ToString();


                _additionaldata.Add(additionaldata);
            }


            return _additionaldata.ToArray();
        }

        private void FormElements(bool enable, bool hide)
        {
            this.tbAuthPassword.Enabled =
                this.tbAuthUsername.Enabled =
                this.tbAuthnonce.Enabled =
                this.tbRemoteUsername.Enabled =
                this.tbRemotePassword.Enabled =
                this.tbConnectionAddress.Enabled =
                this.tbConnectionName.Enabled =
                this.GrdAdditionalData.Enabled =
                this.tbConnectionPort.Enabled =
                this.tbRemotePort.Enabled =
                this.tbPath.Enabled =
                this.chkexternalauth.Enabled =
                this.tbNameOfFile.Enabled =
                this.chkDeleteCardFile.Enabled =
                this.ddlFileEncryption.Enabled =
                this.chkDuplicateCheck.Enabled =
                this.ddlAuthType.Enabled =
                this.tbHeaderLength.Enabled =
                this.tbIdentification.Enabled =
                //this.ddlConnections.Enabled =
                this.ddlProtocol.Enabled =
                this.ddlConnectionType.Enabled =
                this.ddlDocType.Enabled =
                this.tbBufferSize.Enabled =
                this.tbTimeout.Enabled =
                this.fuPrivateKey.Enabled =
                this.fuPublicKey.Enabled =
                this.chkexternalauth.Enabled =
                this.tbdominname.Enabled = enable;

            this.detailsTab.Visible = hide ? false : true;
            this.additionalTab.Visible = hide ? false : true;

            this.btnCancel.Visible = this.btnCancel.Enabled =
                this.btnConfirm.Visible = this.btnConfirm.Enabled =
                this.btnCreate.Visible = this.btnCreate.Enabled =
                this.btnUpdate.Visible = this.btnUpdate.Enabled =
                this.btnEdit.Visible = this.btnEdit.Enabled =
                this.btnDelete.Visible = this.btnDelete.Enabled = hide ? false : true;
            tbAuthPassword.Attributes.Add("value", Password);
        }

        private void TypeParameters()
        {
            this.lblAuthPassword.Visible =
                this.tbAuthPassword.Visible =
                this.lblAuthUsername.Visible =
                this.tbAuthUsername.Visible =
                this.tbAuthnonce.Visible =
                this.lblRemotePassword.Visible =
                this.lblRemotePort.Visible =
                this.lblRemoteUsername.Visible =
                this.tbRemoteUsername.Visible =
                this.tbRemotePassword.Visible =
                this.lblConnectionAddress.Visible =
                this.tbConnectionAddress.Visible =
                this.lblConnectionPort.Visible =
                this.tbConnectionPort.Visible =
                this.lblRemotePort.Visible =
                this.tbRemotePort.Visible =
                this.lblAuthType.Visible =
                this.ddlAuthType.Visible =
                this.lblPortocol.Visible =
                this.ddlProtocol.Visible =
                this.lbldominname.Visible =
                this.tbdominname.Visible =
                this.chkexternalauth.Visible =
                this.chkmaskpassword.Visible =
                this.reqCardFileLoc.Enabled =
                this.rfvAddress.Enabled =
                this.rfvconnectionport.Enabled =
                //this.rfvPath.Enabled =
                this.lblHeaderLength.Visible =
                this.tbHeaderLength.Visible =
                this.lblIdentification.Visible =
                this.tbIdentification.Visible =
                this.lblPath.Visible =
                this.tbPath.Visible =
                this.lblNameOfFile.Visible =
                this.tbNameOfFile.Visible =
                this.lblDeleteCardFile.Visible =
                this.chkDeleteCardFile.Visible =
                this.lblFileEncryption.Visible =
                this.ddlFileEncryption.Visible =
                this.lblDuplicateCheck.Visible =
                this.chkDuplicateCheck.Visible =
                this.lblTimeout.Visible =
                this.tbTimeout.Visible =
                this.lblBufferSize.Visible =
                this.tbBufferSize.Visible =
                this.lblDocType.Visible =
                this.ddlDocType.Visible =
                this.lblPrivateKey.Visible =
                this.fuPrivateKey.Visible =
                this.lblPublicKey.Visible =
                this.fuPublicKey.Visible =
                this.lbldominname.Visible =
                this.lblExternalAuth.Visible =
                this.chkexternalauth.Visible =
                this.tbdominname.Visible = true;

            if (int.Parse(this.ddlConnectionType.SelectedValue) == 0)
            {
                this.lblNameOfFile.Visible =
                this.tbNameOfFile.Visible =
                this.lblDeleteCardFile.Visible =
                this.chkDeleteCardFile.Visible =
                this.lblFileEncryption.Visible =
                this.ddlFileEncryption.Visible =
                this.lblDuplicateCheck.Visible =
                this.chkDuplicateCheck.Visible =
                this.lblHeaderLength.Visible =
                this.tbHeaderLength.Visible =
                this.lblTimeout.Visible =
                this.tbTimeout.Visible =
                this.lblBufferSize.Visible =
                this.tbBufferSize.Visible =
                this.lblDocType.Visible =
                this.ddlDocType.Visible =
                this.lblPrivateKey.Visible =
                this.fuPrivateKey.Visible =
                this.lblPublicKey.Visible =
                this.fuPublicKey.Visible =
                this.lbldominname.Visible =
                this.tbdominname.Visible = false;
            }
            else
            if (int.Parse(this.ddlConnectionType.SelectedValue) == 5)
            {
                this.lblNameOfFile.Visible =
                this.tbNameOfFile.Visible =
                this.lblDeleteCardFile.Visible =
                this.chkDeleteCardFile.Visible =
                this.lblFileEncryption.Visible =
                this.ddlFileEncryption.Visible =
                this.lblDuplicateCheck.Visible =
                this.chkDuplicateCheck.Visible =
                this.lblHeaderLength.Visible =
                this.tbHeaderLength.Visible =
                this.lblTimeout.Visible =
                this.tbTimeout.Visible =
                this.lblBufferSize.Visible =
                this.tbBufferSize.Visible =
                this.lblDocType.Visible =
                this.ddlDocType.Visible =
                this.lblPrivateKey.Visible =
                this.fuPrivateKey.Visible =
                this.lblPublicKey.Visible =
                this.fuPublicKey.Visible =
                this.lbldominname.Visible =
                 this.lblRemotePassword.Visible =
                this.lblRemotePort.Visible =
                this.lblRemoteUsername.Visible =
                this.lblRemotePassword.Visible =
                this.tbRemotePassword.Visible =
                this.lblRemoteUsername.Visible =
                this.tbRemoteUsername.Visible =
                this.lblRemotePort.Visible =
                this.tbRemotePort.Visible =
                this.lblIdentification.Visible =
                this.tbIdentification.Visible =
                this.chkexternalauth.Visible =
                this.lblExternalAuth.Visible =
                this.lblAuthnonce.Visible =
                this.tbAuthnonce.Visible =
                this.tbdominname.Visible = false;
            }
            else if (int.Parse(this.ddlConnectionType.SelectedValue) == 2)
            {
                this.lblNameOfFile.Visible =
                this.tbNameOfFile.Visible =
                this.lblDeleteCardFile.Visible =
                this.chkDeleteCardFile.Visible =
                this.lblFileEncryption.Visible =
                this.ddlFileEncryption.Visible =
                this.lblDuplicateCheck.Visible =
                this.chkDuplicateCheck.Visible =
                this.lblPath.Visible =
                this.tbPath.Visible =
                 this.lblRemotePassword.Visible =
                this.lblRemotePort.Visible =
                this.lblRemoteUsername.Visible =
                this.lblAuthPassword.Visible =
                this.tbAuthPassword.Visible =
                this.lblAuthUsername.Visible =
                this.tbAuthUsername.Visible =
                this.tbAuthnonce.Visible =
                this.tbRemoteUsername.Visible =
                this.tbRemotePassword.Visible =
                this.lblAuthType.Visible =
                this.ddlAuthType.Visible =
                this.lblPortocol.Visible =
                this.ddlProtocol.Visible =
                this.chkmaskpassword.Visible =
                this.reqCardFileLoc.Enabled =
                this.lblPrivateKey.Visible =
                this.fuPrivateKey.Visible =
                this.lblPublicKey.Visible =
                this.fuPublicKey.Visible =
                this.lbldominname.Visible =
                this.lblExternalAuth.Visible =
                this.chkexternalauth.Visible =
                this.tbdominname.Visible =
                this.lblRemotePort.Visible =
                this.tbRemotePort.Visible =
                //this.rfvPath.Enabled = 
                false;

            }
            else if (int.Parse(this.ddlConnectionType.SelectedValue) == 3)
            {
                this.lblNameOfFile.Visible =
                this.tbNameOfFile.Visible =
                this.lblDeleteCardFile.Visible =
                this.chkDeleteCardFile.Visible =
                this.lblFileEncryption.Visible =
                this.ddlFileEncryption.Visible =
                this.lblDuplicateCheck.Visible =
                this.chkDuplicateCheck.Visible =
                this.lblPath.Visible =
                this.tbPath.Visible =
                this.lblPortocol.Visible =
                this.ddlProtocol.Visible =
                this.reqCardFileLoc.Enabled =
                this.lblPrivateKey.Visible =
                this.fuPrivateKey.Visible =
                this.lblPublicKey.Visible =
                this.fuPublicKey.Visible =
                this.lbldominname.Visible =
                this.lblExternalAuth.Visible =
                this.chkexternalauth.Visible =
                this.tbdominname.Visible =
                this.lblHeaderLength.Visible =
                this.tbHeaderLength.Visible =
                this.lblIdentification.Visible =
                this.tbIdentification.Visible =
                this.lblDocType.Visible =
                this.ddlDocType.Visible =
                this.lblBufferSize.Visible =
                this.tbBufferSize.Visible =
                this.lblRemotePort.Visible =
                this.tbRemotePort.Visible =
                this.tbAuthnonce.Visible =
                this.tbRemoteUsername.Visible =
                this.tbRemotePassword.Visible =
                false;

            }
            else if (int.Parse(this.ddlConnectionType.SelectedValue) == 4)
            {
                this.lblNameOfFile.Visible =
               this.tbNameOfFile.Visible =
               this.lblDeleteCardFile.Visible =
               this.chkDeleteCardFile.Visible =
               this.lblFileEncryption.Visible =
               this.ddlFileEncryption.Visible =
               this.lblDuplicateCheck.Visible =
               this.chkDuplicateCheck.Visible =
               this.lblHeaderLength.Visible =
               this.tbHeaderLength.Visible =
               this.lblTimeout.Visible =
               this.tbTimeout.Visible =
               this.lblBufferSize.Visible =
               this.tbBufferSize.Visible =
               this.lblDocType.Visible =
               this.ddlDocType.Visible =
               this.lblPrivateKey.Visible =
               this.fuPrivateKey.Visible =
               this.lblPublicKey.Visible =
               this.fuPublicKey.Visible =
               this.lblPortocol.Visible =
               this.ddlProtocol.Visible =
               this.chkexternalauth.Visible =
                this.lblRemotePassword.Visible =
                this.lblRemotePort.Visible =
                this.lblRemoteUsername.Visible =
               this.tbRemotePort.Visible =
               this.lblRemotePort.Visible =
                 this.tbAuthnonce.Visible =
                this.tbRemoteUsername.Visible =
                this.tbRemotePassword.Visible =
               this.lblExternalAuth.Visible = false;
            }
            else
            {
                //this.lblAuthPassword.Visible =
                //this.tbAuthPassword.Visible =
                this.lblRemotePassword.Visible =
               this.lblRemotePort.Visible =
               this.lblRemoteUsername.Visible =
               this.lblAuthUsername.Visible =
                this.tbAuthUsername.Visible =
                this.tbAuthnonce.Visible =
                this.tbRemoteUsername.Visible =
                this.tbRemotePassword.Visible =
                this.lblConnectionAddress.Visible =
                this.tbConnectionAddress.Visible =
                this.lblConnectionPort.Visible =
                this.tbConnectionPort.Visible =
                this.lblRemotePort.Visible =
                this.tbRemotePort.Visible =
                this.lblAuthType.Visible =
                this.ddlAuthType.Visible =
                this.lblPortocol.Visible =
                this.ddlProtocol.Visible =
                //this.chkmaskpassword.Visible =
                this.reqCardFileLoc.Enabled =
                this.rfvAddress.Enabled =
                this.rfvconnectionport.Enabled =
                //this.rfvPath.Enabled =
                this.lblHeaderLength.Visible =
                this.tbHeaderLength.Visible =
                this.lblIdentification.Visible =
                this.tbIdentification.Visible =
                this.lblTimeout.Visible =
                this.tbTimeout.Visible =
                this.lblBufferSize.Visible =
                this.tbBufferSize.Visible =
                this.lblDocType.Visible =
                this.ddlDocType.Visible =
                this.lbldominname.Visible =
                this.lblExternalAuth.Visible =
                this.chkexternalauth.Visible =
                this.tbdominname.Visible = false;


                EncryptionFields();
            }
        }

        private bool FileNameHasInvalidChars(string fileName, string[] filePathReferences)
        {
            return (!string.IsNullOrEmpty(fileName) && RemoveFilePathReferences(fileName, filePathReferences).IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0);
        }

        private bool PathHasInvalidChars(string path, string[] filePathReferences)
        {
            if (int.Parse(this.ddlConnectionType.SelectedValue) == 1 && Regex.Match(path.Substring(0, 2), "[a-zA-Z]:").Success)
                path = path.Substring(2);

            return (!string.IsNullOrEmpty(path) && RemoveFilePathReferences(path, filePathReferences).IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0);
        }

        private string RemoveFilePathReferences(string fileOrPath, string[] filePathReferences)
        {
            string rtnValue = fileOrPath;
            foreach (var filePathRef in filePathReferences)
            {
                Match matches = System.Text.RegularExpressions.Regex.Match(fileOrPath, filePathRef);

                while (matches.Success)
                {
                    foreach (Capture capture in matches.Captures)
                    {
                        if (fileOrPath.Length > capture.Index + capture.Length && fileOrPath[capture.Index + capture.Length] == '\\')
                            rtnValue = rtnValue.Replace(capture.Value + "\\", "");
                        else
                            rtnValue = rtnValue.Replace(capture.Value, "");
                    }

                    matches = matches.NextMatch();
                }
            }

            return rtnValue;
        }

        protected void ClearControls()
        {
            this.tbAuthPassword.Text =
                 this.tbAuthUsername.Text =
                 this.tbAuthnonce.Text =
                 this.tbRemoteUsername.Text =
                 this.tbRemotePassword.Text =
                 this.tbConnectionAddress.Text =
                 this.tbConnectionName.Text =
                 this.tbConnectionPort.Text =
                 this.tbRemotePort.Text =
                 this.tbNameOfFile.Text =
                 this.tbPath.Text = "";

            this.chkDeleteCardFile.Checked =
                this.chkDuplicateCheck.Checked = false;

            this.ddlAuthType.SelectedIndex =
            //this.ddlConnections.SelectedIndex =
            this.ddlProtocol.SelectedIndex =
            this.ddlFileEncryption.SelectedIndex = 0;
        }
        #endregion

        #region "GRID EVENT"

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        protected void GrdAdditionalData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && LoadDataEmpty)
            {
                e.Row.Visible = false;
            }
        }
        private void BindGridView(List<ConnectionParamAdditionalDataResult> additionalData)
        {
            if (additionalData == null)
            {
                additionalData = new List<ConnectionParamAdditionalDataResult>();

            }

            //Declare a datatable for the gridview
            DataTable dt = ToDataTable<ConnectionParamAdditionalDataResult>(additionalData);
            dt.Columns.Add("Id", typeof(int));
            foreach (DataRow row in dt.Rows)
            {
                row["Id"] = 1;

            }
            if (dt.Rows.Count == 0)
            {
                dt.Rows.Add(dt.NewRow());
                LoadDataEmpty = true;
            }

            GrdAdditionalData.DataSource = dt;
            GrdAdditionalData.DataBind();
            //Now hide the extra row of the grid view
            //GrdAdditionalData.Rows[0].Visible = false;
            //Delete row 0 from the datatable

            //View the datatable to the viewstate
            AdditionalData = dt;
        }
        protected void EditRow(object sender, GridViewEditEventArgs e)
        {

            GrdAdditionalData.EditIndex = e.NewEditIndex;
            //Now bind the gridview
            GrdAdditionalData.DataSource = AdditionalData;
            GrdAdditionalData.DataBind();
        }

        public DataTable AdditionalData
        {
            get
            {
                if (ViewState["AdditionalData"] == null)
                    return null;
                else
                    return (DataTable)(ViewState["AdditionalData"]);
            }
            set
            {
                ViewState["AdditionalData"] = value;
            }
        }

        protected void DeleteRow(object sender, GridViewDeleteEventArgs e)
        {
            AdditionalData.Rows[e.RowIndex].Delete();
            AdditionalData.AcceptChanges();

            if (AdditionalData.Rows.Count > 0)
            {

                GrdAdditionalData.DataSource = AdditionalData;
                GrdAdditionalData.DataBind();
            }


        }

        protected void GrdAdditionalData_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.ToUpper() == "CREATE")
            {
                GridViewRow row = GrdAdditionalData.FooterRow as GridViewRow;
                TextBox tbfield_name = row.FindControl("tbFKey") as TextBox;
                TextBox tbFValue = row.FindControl("tbFKey") as TextBox;

                if (tbfield_name.Text != string.Empty)
                {


                    Random random = new Random();
                    int rowid = random.Next(10000000, 99999999);

                    DataRow dr = AdditionalData.NewRow();
                    dr["Id"] = rowid;
                    dr["key"] = tbfield_name.Text;
                    dr["value"] = tbFValue.Text;
                    AdditionalData.Rows.Add(dr);
                    foreach (DataRow dtrow in AdditionalData.Rows)
                    {
                        if (dtrow["Id"] == null || dtrow["Id"].ToString() == string.Empty)
                        {
                            dtrow.Delete();
                            break;
                        }
                    }
                    AdditionalData.AcceptChanges();

                    GrdAdditionalData.DataSource = AdditionalData;
                    GrdAdditionalData.DataBind();
                }
                else
                {
                    lblErrorMessage.Text = "Field Name Required.";
                }
            }

        }

        protected void CancelEditRow(object sender, GridViewCancelEditEventArgs e)
        {

            GrdAdditionalData.EditIndex = -1;
            GrdAdditionalData.DataSource = AdditionalData;
            GrdAdditionalData.DataBind();

        }

        protected void UpdateRow(object sendedr, GridViewUpdateEventArgs e)
        {

            var external_system_field_id = GrdAdditionalData.DataKeys[e.RowIndex].Value;

            GridViewRow row = GrdAdditionalData.Rows[e.RowIndex] as GridViewRow;

            TextBox tbKey = row.FindControl("tbKey") as TextBox;
            TextBox tbValue = row.FindControl("tbValue") as TextBox;

            if (tbKey.Text != string.Empty)
            {

                DataRow dr = AdditionalData.Rows[e.RowIndex];

                dr.BeginEdit();
                dr["key"] = tbKey.Text;
                dr["value"] = tbValue.Text;

                dr.EndEdit();
                dr.AcceptChanges();
                GrdAdditionalData.EditIndex = -1;
                //Now bind the datatable to the gridview
                GrdAdditionalData.DataSource = AdditionalData;
                GrdAdditionalData.DataBind();
            }
            else
            {
                lblErrorMessage.Text = "Field Name Required.";
            }


        }
        protected void ChangePage(object sender, GridViewPageEventArgs e)
        {

            GrdAdditionalData.EditIndex = -1;
            GrdAdditionalData.DataSource = AdditionalData;
            GrdAdditionalData.DataBind();

        }
        #endregion

        #region Page Events
        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                CheckFileKeys();

                var filePathRefs = _issuerMan.GetFilePathReferences();

                if (FileNameHasInvalidChars(this.tbNameOfFile.Text, filePathRefs))
                {
                    this.lblErrorMessage.Text = "File name constains illegal characters.";
                    return;
                }

                if (PathHasInvalidChars(this.tbPath.Text, filePathRefs))
                {
                    this.lblErrorMessage.Text = "Path name constains illegal characters.";
                    return;
                }

                this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmUpdate").ToString();
                Password = tbAuthPassword.Text;
                FormElements(false, false);
                pageLayout = PageLayout.UPDATE;

                this.btnCreate.Visible = this.btnCreate.Enabled = false;
                this.btnDelete.Visible = this.btnDelete.Enabled = false;
                this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                this.btnEdit.Visible = this.btnEdit.Enabled = false;
                this.btnConfirm.Visible = this.btnConfirm.Enabled = true;
                this.btnCancel.Visible = this.btnCancel.Enabled = true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.Message;
                }
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                if (pageLayout != null)
                {
                    if (pageLayout == PageLayout.CREATE)
                    {
                        ConnectionParametersResult connParms = _issuerMan.CreateConnectionParam(ConstructConnectionObject());
                        ConnectionParameterId = connParms.ConnectionParams.connection_parameter_id;

                        this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmSuccessCreate").ToString();
                        FormElements(false, false);
                        pageLayout = PageLayout.UPDATE;
                        //ddlConnections.Enabled = true;
                        this.btnCreate.Visible = this.btnCreate.Enabled = false;
                        this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                        this.btnDelete.Visible = this.btnDelete.Enabled = true;
                        this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                        this.btnEdit.Visible = this.btnEdit.Enabled = true;
                        this.btnCancel.Visible = this.btnCancel.Enabled = false;
                    }
                    else if (pageLayout == PageLayout.UPDATE)
                    {
                        ConnectionParametersResult connParms = _issuerMan.UpdateConnectionParam(ConstructConnectionObject());
                        this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmSuccessUpdate").ToString();
                        pageLayout = PageLayout.UPDATE;
                        FormElements(false, false);
                        this.btnCreate.Visible = this.btnCreate.Enabled = false;
                        this.btnEdit.Visible = this.btnEdit.Enabled = true;
                        this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                        this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                        this.btnCancel.Visible = this.btnCancel.Enabled = false;
                    }
                    else if (pageLayout == PageLayout.DELETE)
                    {
                        var productsLinkedToConnection = _issuerMan.GetProductInterfaces((int)ConnectionParameterId);
                        var issuersLinkedToConnection = _issuerMan.GetIssuerConnectionParams((int)ConnectionParameterId);

                        if (productsLinkedToConnection.Count == 0 && issuersLinkedToConnection.Count == 0)
                        {
                            string response = _issuerMan.DeleteConnectionParam((int)ConnectionParameterId);
                            //ddlConnections.Items.Clear();
                            ConnectionParameterId = 0;
                            this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmSuccessDelete").ToString();
                            pageLayout = PageLayout.READ;
                            LoadConnectionParams();
                            ClearControls();
                            Password = null;
                            FormElements(true, false);
                            this.btnDelete.Visible = this.btnDelete.Enabled = false;
                            this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                            this.btnEdit.Visible = this.btnEdit.Enabled = false;
                            this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                            this.btnCancel.Visible = this.btnCancel.Enabled = false;
                        }
                        else
                        {
                            this.lblErrorMessage.Text = GetLocalResourceObject("IssuerProductInterfaceError").ToString();

                            FormElements(false, false);
                            pageLayout = PageLayout.DELETE;

                            this.btnCreate.Visible = this.btnCreate.Enabled = false;
                            this.btnDelete.Visible = this.btnDelete.Enabled = false;
                            this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                            this.btnEdit.Visible = this.btnEdit.Enabled = false;
                            this.btnConfirm.Visible = this.btnConfirm.Enabled = true;
                            this.btnCancel.Visible = this.btnCancel.Enabled = true;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.Message;
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                FormElements(true, false);
                this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                this.btnCancel.Visible = this.btnCancel.Enabled = false;

                if (pageLayout == PageLayout.CREATE)
                {
                    this.btnCreate.Visible = this.btnCreate.Enabled = true;
                    this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                    this.btnDelete.Visible = this.btnDelete.Enabled = false;
                    this.btnEdit.Visible = this.btnEdit.Enabled = false;
                }
                else if (pageLayout == PageLayout.UPDATE || pageLayout == PageLayout.DELETE)
                {
                    FormElements(false, false);
                    //ddlConnections.Enabled = true;
                    this.btnCreate.Visible = this.btnCreate.Enabled = false;
                    this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                    this.btnDelete.Visible = this.btnDelete.Enabled = true;
                    this.btnEdit.Visible = this.btnEdit.Enabled = true;
                    this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                    this.btnCancel.Visible = this.btnCancel.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.Message;
                }
            }
        }

        protected void chkmaskpassword_CheckedChanged(object sender, EventArgs e)
        {
            if (Password == null)
            {
                Password = tbAuthPassword.Text;
            }
            if (chkmaskpassword.Checked)
            {
                tbAuthPassword.TextMode = TextBoxMode.Password;
            }
            else
            {
                tbAuthPassword.TextMode = TextBoxMode.SingleLine;
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
        protected void btnEdit_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            FormElements(true, false);
            //ddlConnections.Enabled = false;
            this.btnCreate.Enabled = this.btnCreate.Visible = false;
            this.btnEdit.Enabled = this.btnEdit.Visible = false;
            this.btnDelete.Visible = this.btnDelete.Enabled = false;
            this.btnUpdate.Enabled = this.btnUpdate.Visible = true;
            this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmDeleteMessage").ToString();
                FormElements(false, false);
                pageLayout = PageLayout.DELETE;

                this.btnCreate.Visible = this.btnCreate.Enabled = false;
                this.btnDelete.Visible = this.btnDelete.Enabled = false;
                this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                this.btnEdit.Visible = this.btnEdit.Enabled = false;
                this.btnConfirm.Visible = this.btnConfirm.Enabled = true;
                this.btnCancel.Visible = this.btnCancel.Enabled = true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.Message;
                }
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
        protected void btnCreate_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                CheckFileKeys();

                var filePathRefs = _issuerMan.GetFilePathReferences();

                if (FileNameHasInvalidChars(this.tbNameOfFile.Text, filePathRefs))
                {
                    this.lblErrorMessage.Text = "File name constains illegal characters.";
                    return;
                }

                if (PathHasInvalidChars(this.tbPath.Text, filePathRefs))
                {
                    this.lblErrorMessage.Text = "Path name constains illegal characters.";
                    return;
                }

                //if (ddlConnections.Items.IndexOf(ddlConnections.Items.FindByText(tbConnectionName.Text)) > 0)
                //{
                //    this.lblErrorMessage.Text = GetLocalResourceObject("ConfirmConnectionUnique").ToString();
                //}

                else
                {
                    this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmUpdate").ToString();
                    Password = tbAuthPassword.Text;
                    FormElements(false, false);
                    pageLayout = PageLayout.CREATE;

                    this.btnCreate.Visible = this.btnCreate.Enabled = false;
                    this.btnDelete.Visible = this.btnDelete.Enabled = false;
                    this.btnEdit.Visible = this.btnEdit.Enabled = false;
                    this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                    this.btnConfirm.Visible = this.btnConfirm.Enabled = true;
                    this.btnCancel.Visible = this.btnCancel.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.Message;
                }
            }
        }

        private void CheckFileKeys()
        {
            if (fuPrivateKey.HasFile)
                SessionWrapper.FilePrivateKey = fuPrivateKey.FileBytes;

            if (fuPublicKey.HasFile)
                SessionWrapper.FilePublicKey = fuPublicKey.FileBytes;
        }

        protected void LoadConnectionDetails()
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";
            try
            {
                this.tbConnectionName.Text =
                this.tbPath.Text =
                this.tbNameOfFile.Text =
                this.tbConnectionAddress.Text =
                this.tbConnectionPort.Text =
                this.tbRemotePort.Text =
                this.tbHeaderLength.Text =
                this.tbIdentification.Text =
                this.tbTimeout.Text =
                this.tbBufferSize.Text = String.Empty;

                this.chkDeleteCardFile.Checked =
                this.chkDuplicateCheck.Checked = false;

                this.ddlFileEncryption.SelectedIndex = 0;

                this.ddlAuthType.SelectedIndex = 0;
                this.ddlProtocol.SelectedIndex = 0;
                this.ddlConnectionType.SelectedIndex = 0;
                this.ddlDocType.SelectedIndex = 0;
                this.tbAuthUsername.Text = "";
                this.tbAuthnonce.Text = "";
                this.tbAuthPassword.Text = "";
                this.tbRemoteUsername.Text = "";
                this.tbRemotePassword.Text = "";
                this.tbdominname.Text = "";
                chkexternalauth.Checked = false;


                this.btnCreate.Visible = this.btnCreate.Enabled = false;
                this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                this.btnDelete.Visible = this.btnDelete.Enabled = false;

                //ConnectionParameterId = null; // Clear out the connection ID. this is important for creating new connection.

                bool canRead;
                bool canUpdate;
                bool canCreate;
                if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.ADMINISTRATOR, out canRead, out canUpdate, out canCreate))
                {
                    if (ConnectionParameterId != null && ConnectionParameterId > 0) // this.ddlConnections.SelectedValue
                    {
                        ConnectionParametersResult result = _issuerMan.GetConnectionParameter(ConnectionParameterId.Value);

                        ConnectionParamsResult conn = result.ConnectionParams;
                        this.tbConnectionName.Text = conn.connection_name;
                        this.ddlConnectionType.SelectedValue = conn.connection_parameter_type_id.ToString();
                        this.tbPath.Text = conn.path;
                        this.tbNameOfFile.Text = conn.name_of_file ?? String.Empty;

                        this.chkDuplicateCheck.Checked = conn.duplicate_file_check_YN ?? false;
                        this.chkDeleteCardFile.Checked = conn.file_delete_YN ?? false;

                        this.ddlFileEncryption.SelectedValue = conn.file_encryption_type_id.GetValueOrDefault(0).ToString();

                        this.tbConnectionAddress.Text = conn.address;
                        this.tbConnectionPort.Text = conn.port.ToString();

                        if (conn.remote_port != null)
                            this.tbRemotePort.Text = conn.remote_port.ToString();

                        if (conn.header_length != null)
                            this.tbHeaderLength.Text = conn.header_length.Value.ToString();

                        this.tbIdentification.Text = conn.identification;

                        if (conn.timeout_milli != null)
                            this.tbTimeout.Text = conn.timeout_milli.Value.ToString();

                        if (conn.buffer_size != null)
                            this.tbBufferSize.Text = conn.buffer_size.Value.ToString();

                        if (!string.IsNullOrWhiteSpace(conn.doc_type))
                            this.ddlDocType.SelectedValue = conn.doc_type;

                        this.ddlAuthType.SelectedValue = conn.auth_type.ToString();
                        this.ddlProtocol.SelectedValue = conn.protocol.ToString();
                        this.tbAuthUsername.Text = conn.username;
                        this.tbAuthnonce.Text = conn.nonce;
                        this.tbAuthPassword.Text = conn.password;
                        this.tbRemoteUsername.Text = conn.remote_username;
                        this.tbRemotePassword.Text = conn.remote_password;
                        this.chkexternalauth.Checked = (bool)conn.is_external_auth;
                        this.tbdominname.Text = conn.domain_name;
                        Password = conn.password;
                        FormElements(false, false);
                        TypeParameters();
                        //ddlConnections.Enabled = true;
                        if (canUpdate)
                        {
                            this.btnCreate.Visible = this.btnCreate.Enabled = false;
                            this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                            this.btnEdit.Visible = this.btnEdit.Enabled = true;
                            this.btnDelete.Visible = this.btnDelete.Enabled = true;
                            this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                            this.btnCancel.Visible = this.btnCancel.Enabled = false;

                        }
                        else
                        {
                            this.btnCreate.Visible = this.btnCreate.Enabled = false;
                            this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                            this.btnEdit.Visible = this.btnEdit.Enabled = false;
                            this.btnDelete.Visible = this.btnDelete.Enabled = false;
                            this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                            this.btnCancel.Visible = this.btnCancel.Enabled = false;
                        }
                        pageLayout = PageLayout.UPDATE;
                        BindGridView(result.additionaldata.ToList());
                        ConnectionParameterId = conn.connection_parameter_id;
                    }
                    else
                    {
                        Password = null;
                        FormElements(true, false);
                        TypeParameters();
                        BindGridView(null);
                        ConnectionParameterId = 0;
                        ClearControls();
                        if (canCreate)
                        {
                            this.btnCreate.Visible = this.btnCreate.Enabled = true;
                            this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                            this.btnDelete.Visible = this.btnDelete.Enabled = false;
                            this.btnEdit.Visible = this.btnEdit.Enabled = false;
                            this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                            this.btnCancel.Visible = this.btnCancel.Enabled = false;
                        }
                        else
                        {
                            this.btnCreate.Visible = this.btnCreate.Enabled = false;
                            this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                            this.btnEdit.Visible = this.btnEdit.Enabled = false;
                            this.btnDelete.Visible = this.btnDelete.Enabled = false;
                            this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                            this.btnCancel.Visible = this.btnCancel.Enabled = false;
                        }
                        pageLayout = PageLayout.CREATE;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.Message;
                }
            }
        }

        protected void ddlConnectionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            TypeParameters();
        }

        protected void ddlFileEncryption_SelectedIndexChanged(object sender, EventArgs e)
        {
            EncryptionFields();
        }

        private void EncryptionFields()
        {
            bool visible = false;

            if (int.Parse(this.ddlFileEncryption.SelectedValue) > 0)
                visible = true;

            this.lblPrivateKey.Visible =
            this.fuPrivateKey.Visible =
            this.lblPublicKey.Visible =
            this.fuPublicKey.Visible = visible;
        }
        #endregion

        #region Page Properties
        //private List<ConnectionParamsResult> ConnectionParams
        //{
        //    get
        //    {
        //        if (ViewState["ConnectionParams"] != null)
        //        {
        //            return (List<ConnectionParamsResult>)ViewState["ConnectionParams"];
        //        }
        //        return null;
        //    }
        //    set
        //    {
        //        ViewState["ConnectionParams"] = value;
        //    }
        //}

        private int? ConnectionParameterId
        {
            get
            {
                if (ViewState["ConnectionParameterId"] != null)
                {
                    return (int)ViewState["ConnectionParameterId"];
                }
                return null;
            }
            set
            {
                ViewState["ConnectionParameterId"] = value;
            }
        }

        private PageLayout? pageLayout
        {
            get
            {
                if (ViewState["pageLayout"] != null)
                {
                    return (PageLayout)ViewState["pageLayout"];
                }
                return null;
            }
            set
            {
                ViewState["pageLayout"] = value;
            }
        }

        private string Password
        {
            get
            {
                if (ViewState["Password"] != null)
                {
                    return ViewState["Password"].ToString();
                }
                return null;
            }
            set
            {
                ViewState["Password"] = value;
            }
        }
        #endregion

    }
}

