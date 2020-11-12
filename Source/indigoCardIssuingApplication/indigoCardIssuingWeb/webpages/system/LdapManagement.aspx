<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="LdapManagement.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.system.LdapDetail" UICulture="auto" Culture="auto:en-US" meta:resourcekey="PageResource" %>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div class="ContentHeader">
            <asp:Label ID="lblHeader" runat="server" Text="LDAP Detail"  meta:resourcekey="lblHeaderResource"/>
        </div>

        <div id="divInputs" runat="server">
<%--            <asp:Label ID="lblLdapList" runat="server" Text="LDAP Settings" CssClass="label leftclr"  meta:resourcekey="lblLdapListResource"/>
            <asp:DropDownList ID="ddlLdapSettings" runat="server" CssClass="input rightclr" AutoPostBack="true"
                              OnSelectedIndexChanged="ddlLdapSettings_SelectedIndexChanged" CausesValidation="false" />--%>
              <asp:Label ID="lbltype" runat="server" Text="Authentication Type" CssClass="label leftclr"  meta:resourcekey="lbltypeResource"/>
            <asp:DropDownList ID="ddltype" runat="server" CssClass="input rightclr" AutoPostBack="true"
                              OnSelectedIndexChanged="ddltype_SelectedIndexChanged" CausesValidation="false" />
            <asp:Label ID="lblLdapName" runat="server" Text="Connection name" CssClass="label leftclr" meta:resourcekey="lblLdapNameResource"  />
            <asp:TextBox ID="tbLdapName" runat="server" CssClass="input rightclr" MaxLength="100"  />
            <asp:RequiredFieldValidator ID="rfvldapname" runat="server" ControlToValidate="tbLdapName" ErrorMessage="Name required"
                                        CssClass="validation rightclr" ForeColor="red" meta:resourcekey="rfvldapnameResource"  />

            <asp:Label ID="lblDomain" runat="server" Text="Domain Name" CssClass="label leftclr"  meta:resourcekey="lblDomainResource" />
            <asp:TextBox ID="tbDomain" runat="server" CssClass="input rightclr" MaxLength="200"  />
            <asp:RequiredFieldValidator ID="rfvDomain" runat="server" ControlToValidate="tbDomain" ErrorMessage="Domain name required"
                                        CssClass="validation rightclr" ForeColor="red" meta:resourcekey="rfvdomainresource"  />

            <asp:Label ID="lblHostOrIp" runat="server" Text="Hostname/IP" CssClass="label leftclr"  meta:resourcekey="lblHostOrIpResource" />
            <asp:TextBox ID="tbHostnameOrIp" runat="server" CssClass="input rightclr" MaxLength="200"   />
            <asp:RequiredFieldValidator ID="rfvhost" runat="server" ControlToValidate="tbHostnameOrIp" ErrorMessage="Hostname or IP address required"
                                        CssClass="validation rightclr" ForeColor="red"  meta:resourcekey="rfvhostResource"  />

            <asp:Label ID="lblPath" runat="server" Text="Path" CssClass="label leftclr"  meta:resourcekey="lblPathResource"/>
            <asp:TextBox ID="tbPath" runat="server" CssClass="input rightclr" MaxLength="200"   />
            <asp:RequiredFieldValidator ID="rfvpath" runat="server" ControlToValidate="tbPath" ErrorMessage="Path required"
                                        CssClass="validation rightclr" ForeColor="red" meta:resourcekey="rfvpathResource" />

            <asp:Label ID="lblUsername" runat="server" Text="Username" CssClass="label leftclr"  meta:resourcekey="lblUsernameResource"/>
            <asp:TextBox ID="tbUsername" runat="server" CssClass="input rightclr" MaxLength="100"  />

            <asp:Label ID="lblPassword" runat="server" Text="Password" CssClass="label leftclr"  meta:resourcekey="lblPasswordResource"/>
            <asp:TextBox ID="tbPassword" runat="server" CssClass="input" MaxLength="100" TextMode="Password" />

          <asp:CheckBox ID="chkmaskpassword" Text="Mask Password" runat="server" 
                CssClass="input rightclr"    Checked="true"
                       AutoPostBack="true" meta:resourcekey="chkmaskpasswordResource" 
                oncheckedchanged="chkmaskpassword_CheckedChanged" />

            

            <div id="divInterface" style="display:none;" runat="server">
            <asp:Label ID="lblInterface" runat="server" Text="External Interface" CssClass="label leftclr"  meta:resourcekey="lblInterfaceResource"/>
           <asp:DropDownList ID="ddlInterface" runat="server" CssClass="input"  />
           </div>
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server"  meta:resourcekey="lblInfoMessageResource" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server"  meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div class="ButtonPanel">
            <asp:Button ID="btnCreate" runat="server" Text="<%$ Resources: CommonLabels, Create %>"  CssClass="button" onclick="btnCreate_Click" />

            <asp:Button ID="btnEdit" runat="server" Text="<%$ Resources: CommonLabels, Edit %>" CssClass="button" OnClick="btnEdit_Click" Visible="false" />
            <asp:Button ID="btnUpdate" runat="server" Text="<%$ Resources: CommonLabels, Update %>"  CssClass="button" onclick="btnUpdate_Click" 
                        Enabled="false" Visible="false"/>
                                    <asp:Button ID="btnDelete" runat="server" Text="<%$ Resources: CommonLabels, Delete %>"  CssClass="button" onclick="btnDelete_Click"   Visible="false" />
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" CssClass="button" onclick="btnConfirm_Click"  
                        Enabled="false" Visible="false" />
            <asp:Button ID="btnCancel" runat="server" Text="<%$ Resources: CommonLabels, Back %>" CssClass="button" onclick="btnCancel_Click"    
                        Enabled="false" Visible="false"/>
        </div>
       
    </div>
</asp:Content>
