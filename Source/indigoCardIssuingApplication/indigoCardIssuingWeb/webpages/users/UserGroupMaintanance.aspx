<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="UserGroupMaintanance.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.users.UserGroupCreation" Culture="auto:en-US" meta:resourcekey="PageResource" UICulture="auto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblUserGroups" runat="server" Text="Available User Groups" meta:resourcekey="lblUserGroupsResource" />
            </div>

            <asp:DropDownList ID="ddlIssuer" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlIssuer_SelectedIndexChanged"
                CssClass="input leftclr" />

            <asp:DropDownList ID="ddlRole" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlRole_SelectedIndexChanged"
                CssClass="input rightclr" />
            <br />

            <asp:DataList ID="dlUserGroupsList" runat="server" CssClass="bothclr"
                OnItemCommand="dlUserGroupsList_ItemCommand" CellPadding="0"
                ForeColor="#333333" Width="100%">

                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />

                <HeaderTemplate>
                    <tr style="font-weight: bold;">
                        <td colspan="0">
                            <asp:Label ID="lblGroupName" runat="server" Text="Group Name" meta:resourcekey="lblGroupNameResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblRole" runat="server" Text="Role" meta:resourcekey="lblRoleResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblCreate" runat="server" Text="Create" meta:resourcekey="lblCreateResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblUpdate" runat="server" Text="Update" meta:resourcekey="lblUpdateResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblRead" runat="server" Text="Read" meta:resourcekey="lblReadResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblMaskScreen" runat="server" Text="Mask Screen" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblMaskReport" runat="server" Text="Mask Report" /></td>
                    </tr>
                </HeaderTemplate>

                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />

                <ItemTemplate>
                    <tr class="ItemSelect">
                        <td>
                            <asp:Label ID="lblUserGroupId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "user_group_id") %>' Visible="false" />
                            <asp:LinkButton ID="lbtnUserGroupList" CommandName="select" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "user_group_name") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblUserRole" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "user_role") %>' />
                        </td>
                        <td>
                            <asp:CheckBox ID="lblCreateAccess" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "can_create") %>' Enabled="false" />
                        </td>
                        <td>
                            <asp:CheckBox ID="lblUpdateAccess" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "can_update") %>' Enabled="false" />
                        </td>
                        <td>
                            <asp:CheckBox ID="lblReadAccess" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "can_read") %>' Enabled="false" />
                        </td>
                        <td>
                            <asp:CheckBox ID="lblMaskScreenPAN" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "mask_screen_pan") %>' Enabled="false" />
                        </td>
                        <td>
                            <asp:CheckBox ID="lblMaskReportPAN" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "mask_report_pan") %>' Enabled="false" />
                        </td>
                    </tr>
                </ItemTemplate>
                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <SeparatorStyle />
            </asp:DataList>

            <div class="PaginationPanel">
                <asp:LinkButton ID="lnkFirst" runat="server" Text="<%$ Resources: CommonLabels, First %>" OnClick="lnkFirst_Click"> </asp:LinkButton>
                <asp:LinkButton ID="lnkPrev" runat="server" Text="<%$ Resources: CommonLabels, Prev %>" OnClick="lnkPrev_Click"> </asp:LinkButton>
                <asp:Label ID="lblPageIndex" runat="server" />
                <asp:LinkButton ID="lnkNext" runat="server" Text="<%$ Resources: CommonLabels, Next %>" OnClick="lnkNext_Click"> </asp:LinkButton>
                <asp:LinkButton ID="lnkLast" runat="server" Text="<%$ Resources: CommonLabels, Last %>" OnClick="lnkLast_Click"> </asp:LinkButton>
            </div>
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnCreateUserGroupButton" runat="server" Text="Create User Group"
                OnClick="btnCreateUserGroupButton_Click" CssClass="button"
                meta:resourcekey="btnCreateUserGroupButtonResource" />
        </div>
    </div>
</asp:Content>
