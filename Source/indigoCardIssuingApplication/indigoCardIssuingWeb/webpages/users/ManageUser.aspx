<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true"
    CodeBehind="ManageUser.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.users.ManageUser"
    Culture="auto:en-US" meta:resourcekey="PageResource" UICulture="auto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblUserDetails" runat="server" Text="User Details" meta:resourcekey="lblUserDetailsResource" />
            </div>
            <div>
                <asp:Label ID="lblauthConfig" runat="server" Text="Authentication Config" CssClass="label leftclr"
                    meta:resourcekey="lblLDAPuserResource" />
                <asp:DropDownList ID="ddlAuthConfig" runat="server" CssClass="input rightclr" OnSelectedIndexChanged="ddlAuth_OnSelectedIndexChanged"  
                    AutoPostBack="true"  />
              <%--  <div id="divInterface" style="display: none;" runat="server">
                    <asp:Label ID="lblInterface" runat="server" Text="External Interface" CssClass="label leftclr" meta:resourcekey="lblInterfaceResource" />
                    <asp:DropDownList ID="ddlInterface" runat="server" CssClass="input" />
                </div>--%>
                <asp:Label ID="lblUserName" runat="server" Text="Username" CssClass="label leftclr"
                    meta:resourcekey="lblUserNameResource" />
                <asp:TextBox ID="tbUserName" runat="server" CssClass="input" Enabled="False" MaxLength="50" />
                <asp:Button ID="btnLookupUser" runat="server" Text="Lookup" Visible="false" CausesValidation="false"
                    meta:resourcekey="btnLookupUserResource" CssClass="button" Style="margin: 3px 0px 0px 5px !important; float: left;"
                    OnClick="btnLookupUser_Click" />
                <asp:RequiredFieldValidator ID="rfvUserName" runat="server" ControlToValidate="tbUserName"
                    CssClass="validation rightclr" ErrorMessage="Username Required" meta:resourcekey="rfvUserNameResource"
                    ForeColor="Red" />
                <asp:Label ID="lblFirstName" runat="server" CssClass="label leftclr" meta:resourcekey="lblFirstNameResource" />
                <asp:TextBox ID="tbFirstName" runat="server" CssClass="input" Enabled="False" MaxLength="50" />
                <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="tbFirstName"
                    CssClass="validation rightclr" ErrorMessage="First Name Required" meta:resourcekey="rfvFirstNameResource"
                    ForeColor="Red" />
                <asp:Label ID="lblLastName" runat="server" CssClass="label leftclr" meta:resourcekey="lblLastNameResource" />
                <asp:TextBox ID="tbLastName" runat="server" CssClass="input" Enabled="False" MaxLength="50" />
                <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="tbLastName"
                    CssClass="validation rightclr" ErrorMessage="Last Name Required" meta:resourcekey="rfvLastNameResource"
                    ForeColor="Red" />
                <asp:Label ID="lblEmail" runat="server" Text="Email Address" CssClass="label leftclr"
                    meta:resourcekey="lblEmailResource" />
                <asp:TextBox ID="tbEmail" runat="server" CssClass="input" Enabled="False" MaxLength="100" />
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="tbEmail"
                    ErrorMessage="Email Address Required" ForeColor="Red" CssClass="validation" meta:resourcekey="rfvEmailResource" />
                <asp:RegularExpressionValidator ID="revEmail" runat="server" CssClass="validation rightclr"
                    ControlToValidate="tbEmail" ErrorMessage="Please enter correct email address"
                    ForeColor="Red" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                    meta:resourcekey="revEmailResource"></asp:RegularExpressionValidator>
                <asp:Label ID="lblUserStatus" runat="server" Text="User Status" CssClass="label leftclr"
                    meta:resourcekey="lblUserStatusResource" />
                <asp:DropDownList ID="ddlUserStatus" runat="server" CssClass="input" Enabled="False" />
                <asp:Label ID="lblDDLvalidation" runat="server" CssClass="validation rightclr" meta:resourcekey="lblDDLvalidationResource" />
                <asp:Label ID="lblUserLanguage" runat="server" Text="User Language" CssClass="label leftclr"
                    meta:resourcekey="lblUserLanguageResource" />
                <asp:DropDownList ID="ddlUserLanguage" runat="server" CssClass="input rightclr" Enabled="False" Height="16px" />

                <asp:Label ID="lblTimeZone" runat="server" Text="Time Zone" CssClass="label leftclr" meta:resourcekey="lblTimeZoneResource" />
                <asp:DropDownList ID="ddlTimeZone" runat="server" CssClass="input" />

                <asp:Label ID="lblPassword" runat="server" Text="Password" CssClass="label leftclr"
                    Visible="False" meta:resourcekey="lblPasswordResource" />
                <asp:TextBox ID="tbPassword" runat="server" TextMode="Password" CssClass="input"
                    Visible="False" MaxLength="15" />
                <asp:RegularExpressionValidator ID="revPassword" CssClass="validation rightclr" ForeColor="Red"
                    runat="server" ControlToValidate="tbPassword" ErrorMessage="Password not strong enough"
                    ValidationExpression="(?=^.{8,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$"
                    meta:resourcekey="revPasswordResource" />
                <asp:Label ID="lblConfirmPassword" runat="server" Text="Confirm Password" CssClass="label leftclr"
                    Visible="False" meta:resourcekey="lblConfirmPasswordResource" />
                <asp:TextBox ID="tbConfirmPassword" runat="server" TextMode="Password" Visible="False"
                    MaxLength="15" CssClass="input" />
                <asp:CompareValidator ID="cvPasswordsComparator" runat="server" CssClass="validation rightclr"
                    ForeColor="Red" ControlToCompare="tbConfirmPassword" ControlToValidate="tbPassword"
                    ErrorMessage="Passwords must be the same" meta:resourcekey="cvPasswordsComparatorResource" />
            </div>
            <asp:Panel ID="pnlViewUserGroups" runat="server" CssClass="bothclr" GroupingText="Groups"
                meta:resourcekey="pnlUserGroupsResource">
                <asp:DataList ID="dlGroupList" runat="server" HeaderStyle-Font-Bold="true" Width="100%"
                    CellPadding="0" ForeColor="#333333">
                    <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="White" />
                    <HeaderTemplate>
                        <tr style="font-weight: bold;">
                            <td colspan="0">
                                <asp:Label ID="lblUserGroup" runat="server" Text="User Group" meta:resourcekey="lblUserGroupResource" />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblRole" runat="server" Text="Role" meta:resourcekey="lblRoleResource" />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblGroupIssuer" runat="server" Text="Issuer" meta:resourcekey="lblGroupIssuerResource" />
                            </td>
                        </tr>
                    </HeaderTemplate>
                    <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                    <ItemTemplate>
                        <tr class="ItemSelect">
                            <td colspan="0">
                                <asp:Label ID="lblUserGroupName" CommandName="Select" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "user_group_name") %>' />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblRoleName" CommandName="Select" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "user_role") %>' />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblIssuerCode" CommandName="Select" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "issuer_code") %>' />
                                <asp:Label ID="lblIssuerName" CommandName="Select" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "issuer_name") %>' />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                </asp:DataList>
            </asp:Panel>
            <asp:Panel ID="pnlUserGroups" runat="server" CssClass="bothclr" GroupingText="Groups"
                meta:resourcekey="pnlUserGroupsResource">
                <asp:Label ID="lblIssuer" runat="server" CssClass="label leftclr" Text="<%$ Resources: CommonLabels, Issuer %>" />
                <asp:DropDownList ID="ddlIssuer" runat="server" CssClass="input rightclr" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlIssuer_OnSelectedIndexChanged" />
                <asp:Label ID="lblRoleType" runat="server" Text="Role" CssClass="label leftclr" meta:resourcekey="lblRoleTypeResource" />
                <asp:DropDownList ID="ddlRole" runat="server" CssClass="input rightclr" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlRole_OnSelectedIndexChanged" />
                <asp:Label ID="lblBranch" runat="server" Text="Branch" CssClass="label leftclr" meta:resourcekey="lblBranchResource" />
                <asp:DropDownList ID="ddlBranch" runat="server" CssClass="input rightclr" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlBranch_OnSelectedIndexChanged" />
                <asp:Label ID="lblGroups" runat="server" Text="Groups" CssClass="label leftclr" meta:resourcekey="lblGroupsResource" />
                <div class="input rightclr" style="height: 75px; overflow: auto">
                    <asp:CheckBoxList ID="cblGroups" runat="server" />
                </div>

            </asp:Panel>
        </div>
        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>
        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnCreate" runat="server" Text="Create User" CssClass="button" meta:resourcekey="btnCreateResource"
                OnClick="btnCreate_Click" />
            <asp:Button ID="btnApprove" runat="server" Text="Approve" CssClass="button" 
                OnClick="btnApprove_Click" />
              <asp:Button ID="btnReject" runat="server" Text="Reject" CssClass="button" 
                OnClick="btnReject_Click" />
            <asp:Button ID="btnEdit" runat="server" Text="<%$ Resources: CommonLabels, Edit %>"
                CssClass="button" OnClick="btnEdit_Click" />
            <asp:Button ID="btnUpdate" runat="server" Text="Update User" CssClass="button" meta:resourcekey="btnUpdateResource"
                OnClick="btnUpdate_Click" />
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>"
                CssClass="button" OnClick="btnConfirm_Click" />
            <asp:Button ID="btnResetUserLogin" runat="server" Text="Reset Login Status" CssClass="button"
                OnClick="btnResetUserLogin_Click" meta:resourcekey="btnResetUserLoginResource" />
            <asp:Button ID="btnResetPassword" runat="server" Text="Reset Password" CssClass="button"
                CausesValidation="False" OnClick="btnResetPassword_Click" meta:resourcekey="btnResetPasswordResource" />
            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources: CommonLabels, Back %>"
                CssClass="button" OnClick="btnBack_OnClick" Visible="False" CausesValidation="False" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" runat="server" contentplaceholderid="cphCustom">
        <script type="text/javascript">           

            javascript: window.history.forward();
            var t;
            window.onload = resetTimer;
            document.onkeypress = resetTimer;
            document.onclick = resetTimer;

            function logout() {
                var root = location.protocol + "//" + location.host;
                var applicationPath = "<%=Request.ApplicationPath%>";
                if (applicationPath == "/") {
                    location.href = root + applicationPath + 'Logout.aspx'
                }
                else {
                    location.href = root + applicationPath + '/Logout.aspx'
                }
            }

            function resetTimer() {
                clearTimeout(t);
                t = setTimeout(logout, 590000); //600000 log out in 10 min1800000
            }
        </script>
        <style type="text/css">
            .rightclr {}
        </style>
</asp:Content>

