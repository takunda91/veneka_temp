<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true"
    CodeBehind="RestInterfaceManagement.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.system.RestIssuerInterface"
    UICulture="auto" Culture="auto:en-US" meta:resourcekey="PageResource" %>


<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            $('#default').puitabview({
                activeIndex: document.getElementById('<%= hdnActiveTab.ClientID %>').value,
                change: function (event, ui) {
                    var hiddenSource = document.getElementById('<%= hdnActiveTab.ClientID %>');
                    hiddenSource.value = $('#default').puitabview('getActiveIndex');
                }
            });
        });
    </script>
    <asp:HiddenField ID="hdnActiveTab" runat="server" />
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="ContentPanel">
        <div class="ContentHeader">
            <asp:Label ID="lblConnectionDetailsHeader" runat="server" Text="Interface Details"
                meta:resourcekey="lblConnectionDetailsHeaderResource" />
        </div>
        <div id="default" class="indigo-tab">
            <ul>
                <li><a href="#detailsTab">Details</a></li>
                <li><a href="#additionalTab">Additional Data</a></li>
            </ul>
            <div>
                <div id="detailsTab" runat="server" style="overflow: auto">
                    <%--            <asp:Label ID="lblConnections" runat="server" Text="Interface" CssClass="label leftclr"
                meta:resourcekey="lblConnectionsResource" />
            <asp:DropDownList ID="ddlConnections" runat="server" CssClass="input rightclr" OnSelectedIndexChanged="ddlConnections_SelectedIndexChanged"
                AutoPostBack="true" CausesValidation="false" />--%>

                    <asp:Label ID="lblConnectionName" runat="server" Text="Name" CssClass="label leftclr"
                        meta:resourcekey="lblConnectionNameResource" />
                    <asp:TextBox ID="tbConnectionName" runat="server" MaxLength="100" CssClass="input rightclr" meta:resourcekey="tbConnectionNameResource1" />
                    <asp:RequiredFieldValidator ID="reqCardFileLoc" runat="server" ControlToValidate="tbConnectionName"
                        ErrorMessage="Connection name required" meta:resourcekey="reqCardFileLocResource"
                        CssClass="validation rightclr" ForeColor="Red" />

                    <asp:Label ID="lblConnectionType" runat="server" meta:resourcekey="lblConnectionType" CssClass="label leftclr" />
                    <asp:DropDownList ID="ddlConnectionType" runat="server" OnSelectedIndexChanged="ddlConnectionType_SelectedIndexChanged" AutoPostBack="True"
                        CssClass="input rightclr" meta:resourcekey="ddlConnectionTypeResource1" />

                    <asp:Label ID="lbldominname" runat="server" Text="Domain Name" CssClass="label leftclr" meta:resourcekey="lbldominnameresource" />
                    <asp:TextBox ID="tbdominname" runat="server" MaxLength="100" CssClass="input rightclr" />

                    <asp:Label ID="lblPortocol" runat="server" Text="Protocol" CssClass="label leftclr"
                        meta:resourcekey="lblPortocolResource" />
                    <asp:DropDownList ID="ddlProtocol" runat="server" CssClass="input rightclr" meta:resourcekey="ddlProtocolResource1" />

                    <asp:Label ID="lblConnectionAddress" runat="server" Text="Address" CssClass="label leftclr"
                        meta:resourcekey="lblConnectionAddressResource" />
                    <asp:TextBox ID="tbConnectionAddress" runat="server" MaxLength="150" CssClass="input rightclr" meta:resourcekey="tbConnectionAddressResource1" />
                    <asp:RequiredFieldValidator ID="rfvAddress" runat="server" ControlToValidate="tbConnectionAddress"
                        CssClass="validation rightclr" ForeColor="Red" meta:resourcekey="rfvAddressResource" />

                    <asp:Label ID="lblConnectionPort" runat="server" Text="Port" CssClass="label leftclr"
                        meta:resourcekey="lblConnectionPortResource" />
                    <asp:TextBox ID="tbConnectionPort" runat="server" CssClass="input rightclr" OnKeyPress="return isNumberKey(event)"
                        MaxLength="10" meta:resourcekey="tbConnectionPortResource1" />
                    <asp:RequiredFieldValidator ID="rfvconnectionport" runat="server" ControlToValidate="tbConnectionPort"
                        ErrorMessage="Port required" meta:resourcekey="rfvconnectionportResource" CssClass="validation rightclr"
                        ForeColor="Red" />

                    <asp:Label ID="lblRemotePort" runat="server" Text="Remote Port" CssClass="label leftclr" />
                    <asp:TextBox ID="tbRemotePort" runat="server" CssClass="input rightclr" OnKeyPress="return isNumberKey(event)"
                        MaxLength="10" meta:resourcekey="tbConnectionPortResource1" />

                    <asp:Label ID="lblPath" runat="server" Text="Path" CssClass="label leftclr" meta:resourcekey="lblPathResource" />
                    <asp:TextBox ID="tbPath" runat="server" MaxLength="200" CssClass="input rightclr" meta:resourcekey="tbPathResource1" />
                    <asp:RequiredFieldValidator ID="rfvPath" runat="server" ControlToValidate="tbPath" Enabled="false"
                        ErrorMessage="Path required" meta:resourcekey="rfvPathResource" CssClass="validation rightclr"
                        ForeColor="Red" />

                    <div id="pnlFileProperties">
                        <asp:Label ID="lblNameOfFile" runat="server" Text="File Name" CssClass="label leftclr" meta:resourcekey="lblNameOfFileResource1" />
                        <asp:TextBox ID="tbNameOfFile" runat="server" MaxLength="100" CssClass="input rightclr" meta:resourcekey="tbNameOfFileResource1" />

                        <asp:Label ID="lblDeleteCardFile" runat="server" Text="Delete File After Load" CssClass="label leftclr" meta:resourcekey="lblDeleteCardFileResource1" />
                        <asp:CheckBox ID="chkDeleteCardFile" runat="server" CssClass="input rightclr" meta:resourcekey="chkDeleteCardFileResource1" />

                        <asp:Label ID="lblDuplicateCheck" runat="server" Text="Enable File Duplicate Check" CssClass="label leftclr" meta:resourcekey="lblDuplicateCheckResource1" />
                        <asp:CheckBox ID="chkDuplicateCheck" runat="server" CssClass="input rightclr" meta:resourcekey="chkDuplicateCheckResource1" />

                        <asp:Label ID="lblFileEncryption" runat="server" Text="Enable Encrypted File Reader" CssClass="label leftclr" meta:resourcekey="lblFileEncryptionResource1" />
                        <asp:DropDownList ID="ddlFileEncryption" runat="server" CssClass="input rightclr" OnSelectedIndexChanged="ddlFileEncryption_SelectedIndexChanged"
                            AutoPostBack="True" meta:resourcekey="ddlFileEncryptionResource1" />

                        <asp:Label ID="lblPrivateKey" runat="server" Text="Private Key" CssClass="label leftclr" />
                        <asp:FileUpload ID="fuPrivateKey" runat="server" CssClass="input rightclr"></asp:FileUpload>

                        <asp:Label ID="lblPublicKey" runat="server" Text="Public Key" CssClass="label leftclr" />
                        <asp:FileUpload ID="fuPublicKey" runat="server" CssClass="input rightclr"></asp:FileUpload>
                    </div>


                    <asp:Label ID="lblHeaderLength" runat="server" Text="Header Length" CssClass="label leftclr"
                        meta:resourcekey="lblHeaderLengthResource" />
                    <asp:TextBox ID="tbHeaderLength" runat="server" CssClass="input rightclr" OnKeyPress="return isNumberKey(event)"
                        MaxLength="10" meta:resourcekey="tbHeaderLengthResource1" />

                    <asp:Label ID="lblIdentification" runat="server" Text="Identification" CssClass="label leftclr"
                        meta:resourcekey="lblIdentificationResource" />
                    <asp:TextBox ID="tbIdentification" runat="server" CssClass="input rightclr" MaxLength="50" meta:resourcekey="tbIdentificationResource1" />

                    <asp:Label ID="lblTimeout" runat="server" Text="Timeout(Milliseconds)" CssClass="label leftclr" meta:resourcekey="lblTimeoutResource1" />
                    <asp:TextBox ID="tbTimeout" runat="server" CssClass="input rightclr" OnKeyPress="return isNumberKey(event)" MaxLength="10" meta:resourcekey="tbTimeoutResource1" />

                    <asp:Label ID="lblBufferSize" runat="server" Text="Buffer Size" CssClass="label leftclr" meta:resourcekey="lblBufferSizeResource1" />
                    <asp:TextBox ID="tbBufferSize" runat="server" CssClass="input rightclr" OnKeyPress="return isNumberKey(event)" MaxLength="10" meta:resourcekey="tbBufferSizeResource1" />

                    <asp:Label ID="lblDocType" runat="server" Text="Doc Type" CssClass="label leftclr" meta:resourcekey="lblDocTypeResource1" />
                    <asp:DropDownList ID="ddlDocType" runat="server" CssClass="input rightclr" meta:resourcekey="ddlDocTypeResource1">
                        <asp:ListItem Text="A" Value="A" meta:resourcekey="ListItemResource1" />
                        <asp:ListItem Text="B" Value="B" meta:resourcekey="ListItemResource2" />
                        <asp:ListItem Text="C" Value="C" meta:resourcekey="ListItemResource3" />
                    </asp:DropDownList>

                    <asp:Label ID="lblAuthType" runat="server" Text="Authentication" CssClass="label leftclr"
                        meta:resourcekey="lblAuthTypeResource" />
                    <asp:DropDownList ID="ddlAuthType" runat="server" CssClass="input rightclr" meta:resourcekey="ddlAuthTypeResource1" />

                    <asp:Label ID="lblAuthUsername" runat="server" Text="Username" CssClass="label leftclr"
                        meta:resourcekey="lblAuthUsernameResource" />
                    <asp:TextBox ID="tbAuthUsername" runat="server" MaxLength="100" CssClass="input rightclr" meta:resourcekey="tbAuthUsernameResource1" />

                    <asp:Label ID="lblAuthPassword" runat="server" Text="Password" CssClass="label leftclr"
                        meta:resourcekey="lblAuthPasswordResource" />
                    <asp:TextBox ID="tbAuthPassword" runat="server" MaxLength="100" CssClass="input" TextMode="Password" meta:resourcekey="tbAuthPasswordResource1" />
                    <asp:CheckBox ID="chkmaskpassword" Text="Mask Password" runat="server" CssClass="input rightclr"
                        AutoPostBack="True" meta:resourcekey="chkmaskpasswordResource" Checked="True"
                        OnCheckedChanged="chkmaskpassword_CheckedChanged" />
                    <asp:Label ID="lblAuthnonce" runat="server" Text="Nonce" CssClass="label leftclr" meta:resourcekey="lblnonceResource" />
                    <asp:TextBox ID="tbAuthnonce" runat="server" CssClass="input" MaxLength="100" />

                    <asp:Label ID="lblRemoteUsername" runat="server" Text="Remote Username" CssClass="label leftclr" />
                    <asp:TextBox ID="tbRemoteUsername" runat="server" MaxLength="100" CssClass="input rightclr" />

                    <asp:Label ID="lblRemotePassword" runat="server" Text="Remote Password" CssClass="label leftclr" />
                    <asp:TextBox ID="tbRemotePassword" runat="server" MaxLength="100" CssClass="input" />

                    <asp:Label ID="lblExternalAuth" runat="server" Text="External Authentication" CssClass="label leftclr" meta:resourcekey="chkexternalauthResource" />
                    <asp:CheckBox ID="chkexternalauth" runat="server" CssClass="input rightclr" Checked="True" />

                </div>
                <div id="additionalTab" runat="server" style="overflow: auto">

                    <asp:GridView ID="GrdAdditionalData" runat="server" CellPadding="1" CssClass="bothclr" GroupingText="External System Fields" EmptyDataText="No Rows Returned" GridLines="None"
                        AutoGenerateColumns="false" Width="80%" OnRowEditing="EditRow" OnRowDataBound="GrdAdditionalData_RowDataBound" OnRowCancelingEdit="CancelEditRow" OnRowCommand="GrdAdditionalData_RowCommand" ShowFooter="true"
                        OnRowUpdating="UpdateRow" DataKeyNames="Id" OnRowDeleting="DeleteRow" AllowPaging="true"
                        OnPageIndexChanging="ChangePage">

                        <Columns>



                            <asp:TemplateField HeaderText="Key" SortExpression="Key">

                                <EditItemTemplate>

                                    <asp:TextBox ID="tbKey" Width="100px" runat="server" Text='<%# Bind("key") %>'></asp:TextBox>

                                </EditItemTemplate>

                                <FooterTemplate>

                                    <asp:TextBox ID="tbFKey" runat="server" Width="100px"></asp:TextBox>

                                </FooterTemplate>

                                <ItemTemplate>

                                    <asp:Label ID="lblKey" runat="server" Text='<%# Bind("key") %>'></asp:Label>

                                </ItemTemplate>

                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Value" SortExpression="Value">

                                <EditItemTemplate>

                                    <asp:TextBox ID="tbValue" Width="100px" runat="server" Text='<%# Bind("value") %>'></asp:TextBox>

                                </EditItemTemplate>

                                <FooterTemplate>

                                    <asp:TextBox ID="tbFValue" Width="100px" runat="server"></asp:TextBox>

                                </FooterTemplate>

                                <ItemTemplate>

                                    <asp:Label ID="lblValue" runat="server" Text='<%# Bind("value") %>'></asp:Label>

                                </ItemTemplate>

                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="Edit" ShowHeader="False">

                                <EditItemTemplate>

                                    <asp:LinkButton ID="lnkupdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update"></asp:LinkButton>

                                    <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>

                                </EditItemTemplate>

                                <FooterTemplate>

                                    <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="False" CommandName="Create" Text="Create"></asp:LinkButton>

                                </FooterTemplate>

                                <ItemTemplate>

                                    <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit"></asp:LinkButton>

                                </ItemTemplate>

                            </asp:TemplateField>

                            <asp:CommandField HeaderText="Delete" ShowDeleteButton="True" ShowHeader="True" />

                            <asp:CommandField HeaderText="Select" ShowSelectButton="True" ShowHeader="True" />



                        </Columns>

                        <AlternatingRowStyle backcolor="White" forecolor="#284775" />
                    <FooterStyle BackColor="White" Font-Bold="True" ForeColor="Black" />
                    <HeaderStyle BackColor="#0082B6" Font-Bold="True" ForeColor="Black" />
                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />

                    </asp:GridView>
                </div>

            </div>
            </div>
            <div class="InfoPanel">
                <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
            </div>
            <div class="ErrorPanel">
                <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
            </div>
            <div class="ButtonPanel">
                <asp:Button ID="btnCreate" runat="server" Text="<%$ Resources:CommonLabels, Create %>"
                    CssClass="button" OnClick="btnCreate_Click" meta:resourcekey="btnCreateResource1" />
                <asp:Button ID="btnEdit" runat="server" Text="<%$ Resources:CommonLabels, Edit %>"
                    OnClick="btnEdit_OnClick" CssClass="button" Visible="False" meta:resourcekey="btnEditResource1" />
                <asp:Button ID="btnUpdate" runat="server" Text="<%$ Resources:CommonLabels, Update %>"
                    CssClass="button" OnClick="btnUpdate_Click" Enabled="False" Visible="False" meta:resourcekey="btnUpdateResource1" />
                <asp:Button ID="btnDelete" runat="server" Text="<%$ Resources:CommonLabels, Delete %>"
                    CssClass="button" OnClick="btnDelete_Click" Visible="False" meta:resourcekey="btnDeleteResource1" />
                <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources:CommonLabels, Confirm %>"
                    CssClass="button" OnClick="btnConfirm_Click" Enabled="False" Visible="False" meta:resourcekey="btnConfirmResource1" />
                <asp:Button ID="btnCancel" runat="server" Text="<%$ Resources:CommonLabels, Back %>"
                    CssClass="button" OnClick="btnCancel_Click" Enabled="False" Visible="False" meta:resourcekey="btnCancelResource1" />
            </div>
       
    </div>
</asp:Content>
