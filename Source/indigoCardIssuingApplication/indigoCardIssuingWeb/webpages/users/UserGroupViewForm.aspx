<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="UserGroupViewForm.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.users.UserGroupViewForm" Culture="auto:en-US" meta:resourcekey="PageResource" UICulture="auto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblHeader" runat="server" Text="User Group Details" meta:resourcekey="lblCreateUserGroupResource" />
            </div>
            <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlIssuer" runat="server" CssClass="input rightclr" AutoPostBack="true"
                OnSelectedIndexChanged="ddlIssuer_OnSelectedIndexChanged" />

            <asp:Label ID="lblRole" runat="server" Text="User Role" CssClass="label leftclr" meta:resourcekey="lblRoleResource" />
            <asp:DropDownList ID="ddlUserRoles" runat="server" CssClass="input rightclr" />

            <asp:Label ID="lblUserGroupName" runat="server" Text="User Group Name" CssClass="label leftclr" meta:resourcekey="lblUserGroupNameResource" />
            <asp:TextBox ID="tbUserGroupName" runat="server" CssClass="input" MaxLength="50" />
            <asp:RequiredFieldValidator ID="rfvUserGruopName" runat="server" ControlToValidate="tbUserGroupName" CssClass="validation rightclr"
                ErrorMessage="User Group name may not be empty." meta:resourcekey="rfvUserGruopNameResource" />

            <asp:Label ID="lblAllowCreate" runat="server" Text="Allow Create" CssClass="label leftclr" meta:resourcekey="lblAllowCreateResource" />
            <asp:CheckBox ID="chkAllowCreate" runat="server" CssClass="input rightclr" />

            <asp:Label ID="lblAllowUpdate" runat="server" Text="Allow Update" CssClass="label leftclr" meta:resourcekey="lblAllowUpdateResource" />
            <asp:CheckBox ID="chkAllowUpdate" runat="server" CssClass="input rightclr" />

            <asp:Label ID="lblMaskScreenPAN" runat="server" Text="Mask Screen PAN" CssClass="label leftclr" />
            <asp:CheckBox ID="chkMaskScreenPAN" runat="server" CssClass="input rightclr" />

            <asp:Label ID="lblMaskReportPAN" runat="server" Text="Mask Report PAN" CssClass="label leftclr" />
            <asp:CheckBox ID="chkMaskReportPAN" runat="server" CssClass="input rightclr" />

            <asp:RadioButtonList ID="rbtnlBranchAccess" runat="server" AutoPostBack="true" CssClass="bothclr"
                OnSelectedIndexChanged="rbtnlBranchAccess_OnSelectedIndexChanged">
                <asp:ListItem Text="All Branch Access" Value="1" Selected="True" meta:resourcekey="rbtnlBranchAccesslist1Resource" />
                <asp:ListItem Text="Limit Branch Access" Value="2" meta:resourcekey="rbtnlBranchAccesslist2Resource" />
            </asp:RadioButtonList>

            <asp:Panel ID="pnlBranchPanel" runat="server" GroupingText="Branches" CssClass="bothclr" Visible="false" ScrollBars="Auto" Height="150" meta:resourcekey="pnlBranchPanelResource">
                <asp:CheckBoxList ID="chkblBranchList" runat="server" CssClass="bothclr" />
            </asp:Panel>
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnCreate" runat="server" Text="Create Group" CssClass="button" OnClick="btnCreate_Click" meta:resourcekey="btnCreateUserGroupResource" />
            <asp:Button ID="btnEdit" runat="server" Text="Edit Group" CssClass="button" OnClick="btnEdit_Click" meta:resourcekey="btnEditUserGroupResource" />
            <asp:Button ID="btnUpdate" runat="server" Text="Update Group" CssClass="button" OnClick="btnUpdate_Click" meta:resourcekey="btnUpdateGroupResource" />
            <asp:Button ID="btnDelete" runat="server" Text="Delete Group" CssClass="button" OnClick="btnDelete_Click" meta:resourcekey="btnDeleteGroupResource" />

            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" CssClass="button" OnClick="btnConfirm_Click" />

            <asp:Button ID="btnRemoveUserGroup" runat="server" Text="Remove UserGroup" CssClass="button" Visible="false"
                OnClick="btnRemoveUserGroup_Click" meta:resourcekey="btnRemoveUserGroupResource" />

            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources: CommonLabels, Back %>" CssClass="button" CausesValidation="false" OnClick="btnBack_OnClick" />
        </div>
    </div>
</asp:Content>
