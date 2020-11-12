<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="UserPendingList.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.users.UserPendingList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblUserList" runat="server" meta:resourcekey="lblUserListResource" />
            </div>
            <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" />
            <asp:DropDownList ID="ddlIssuer" runat="server" AutoPostBack="true" Width="200px" Style="margin: 3px;"
                OnSelectedIndexChanged="ddlIssuer_SelectedIndexChanged" />
            <asp:Label ID="lblBranch" runat="server" Text="<%$ Resources: CommonLabels, Branch %>" />
            <asp:DropDownList ID="ddlBranch" runat="server" AutoPostBack="true" Width="200px" Style="margin: 3px;"
                OnSelectedIndexChanged="ddlBranch_SelectedIndexChanged" />

            <br />
            <br />

            <asp:DataList ID="dlUserList" runat="server"
                HeaderStyle-Font-Bold="true"
                Width="100%" OnItemDataBound="dlUserList_ItemDataBound"
                OnItemCommand="dlUserList_ItemCommand" CellPadding="0" ForeColor="#333333"
                meta:resourcekey="dlUserListResource2">

                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="White" />

                <HeaderTemplate>
                    <tr style="font-weight: bold;">
                        <td colspan="0">
                            <asp:Label ID="lblUsername" runat="server" Text="Username" meta:resourcekey="lblUsernameResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblFirstName" runat="server" Text="First Name" meta:resourcekey="lblFirstNameResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblLastName" runat="server" Text="Last Name" meta:resourcekey="lblLastNameResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblUserStatus" runat="server" Text="User Status" meta:resourcekey="lblUserStatusResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblWorkstation" runat="server" Text="Workstation" meta:resourcekey="lblWorkstationResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblAuthenticationType" runat="server" Text="Logged In" meta:resourcekey="lblAuthenticationTypeResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblLoggedOn" runat="server" Text="Logged In" meta:resourcekey="lblLoggedOnResource" /></td>

                    </tr>
                </HeaderTemplate>

                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />

                <ItemTemplate>
                    <tr class="ItemSelect">
                        <td colspan="0">
                            <asp:Label ID="lblUserId" CommandName="Select" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "user_id") %>' Visible="false" />
                            <asp:LinkButton ID="lbtnUserName" CommandName="Select" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "username") %>' /></td>
                        <td colspan="0">
                            <asp:Label ID="lblFirstName" CommandName="Select" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "first_name") %>' />
                        </td>
                        <td colspan="0">
                            <asp:Label ID="lblLastanme" CommandName="Select" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "last_name") %>' />
                        </td>
                        <td colspan="0">
                            <asp:Label ID="lblUserStatus" CommandName="Select" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "user_status_text") %>' /></td>
                        <td colspan="0">
                            <asp:Label ID="LabelWorkstation" CommandName="Select" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "workstation") %>' /></td>
                        <td colspan="0">
                            <asp:Label ID="Label1" CommandName="Select" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "AUTHENTICATION_TYPE") %>' /></td>
                        <td colspan="0">
                            <asp:CheckBox ID="CheckBoxUserLoggedIn" Enabled="False" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "online") %>' /></td>
                    </tr>
                </ItemTemplate>
                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            </asp:DataList>

            <div class="PaginationPanel">
                <asp:LinkButton ID="lnkFirst" runat="server" OnClick="lnkFirst_Click" Text="<%$ Resources: CommonLabels, First %>"> </asp:LinkButton>
                <asp:LinkButton ID="lnkPrev" runat="server" OnClick="lnkPrev_Click" Text="<%$ Resources: CommonLabels, Prev %>"> </asp:LinkButton>
                <asp:Label ID="lblPageIndex" runat="server" />
                <asp:LinkButton ID="lnkNext" runat="server" OnClick="lnkNext_Click" Text="<%$ Resources: CommonLabels, Next %>"> </asp:LinkButton>
                <asp:LinkButton ID="lnkLast" runat="server" OnClick="lnkLast_Click" Text="<%$ Resources: CommonLabels, Last %>"> </asp:LinkButton>
            </div>
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnBack" runat="server" OnClick="btnBack_Click" CssClass="button" Text="<%$ Resources: CommonLabels, Back %>" />
        </div>
    </div>
</asp:Content>
