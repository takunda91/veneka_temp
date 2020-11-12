<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" Culture="auto:en-US" meta:resourcekey="PageResource" UICulture="auto"
    CodeBehind="UserSearchSelection.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.users.UserSearchSelection" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblUserList" runat="server" meta:resourcekey="lblUserListResource" />
            </div>
            <div style="display: none;">
                <asp:DropDownList ID="ddlIssuer" runat="server" AutoPostBack="true" CssClass="input leftclr" Enabled="false"
                    OnSelectedIndexChanged="ddlIssuer_SelectedIndexChanged" />
            </div>
            <div>
                <asp:DataList ID="dlUserList" runat="server" DataKeyField="username" HeaderStyle-Font-Bold="true"
                    Width="100%" CellPadding="0" ForeColor="#333333" meta:resourcekey="dlUserListResource2">
                    <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="White" />
                    <HeaderTemplate>
                        <tr style="font-weight: bold;">
                            <td colspan="0">
                                <asp:Label ID="Label2" runat="server" Text="Select" meta:resourcekey="lblselectResource" />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblUsername" runat="server" Text="UserName" meta:resourcekey="lblUsernameResource" />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblFirstName" runat="server" Text="First Name" meta:resourcekey="lblFirstNameResource" />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblLastName" runat="server" Text="Last Name" meta:resourcekey="lblLastNameResource" />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblUserStatus" runat="server" Text="User Status" meta:resourcekey="lblUserStatusResource" />
                            </td>
                        </tr>
                    </HeaderTemplate>
                    <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                    <ItemTemplate>
                        <tr class="ItemSelect">
                            <td colspan="0">
                                <asp:CheckBox runat="server" CommandName="Select" ID='CheckBoxUserName' />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblUserId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "user_id") %>'
                                    Visible="false" />
                                <asp:Label ID="lbtnUserName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "username") %>'
                                    CssClass="ItemSelect" />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblFirstName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "first_name") %>' />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblLastanme" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "last_name") %>' />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblUserStatus" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "user_status_text") %>' />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                </asp:DataList>
                <div class="PaginationPanel">
                    <asp:LinkButton ID="lnkFirst" runat="server" Text="<%$ Resources: CommonLabels, First %>" OnClick="lnkFirst_Click"> </asp:LinkButton>
                    <asp:LinkButton ID="lnkPrev" runat="server" Text="<%$ Resources: CommonLabels, Prev %>" OnClick="lnkPrev_Click"> </asp:LinkButton>
                    <asp:Label ID="lblPageIndex" runat="server" />
                    <asp:LinkButton ID="lnkNext" runat="server" Text="<%$ Resources: CommonLabels, Next %>" OnClick="lnkNext_Click"> </asp:LinkButton>
                    <asp:LinkButton ID="lnkLast" runat="server" Text="<%$ Resources: CommonLabels, Last %>" OnClick="lnkLast_Click"> </asp:LinkButton>
                </div>
            </div>
        </div>
        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>
        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btndone" runat="server" Text="Select" OnClick="ButtonDone_Click"
                BorderColor="#CCCCCC" BorderStyle="Solid" Width="140px" meta:resourcekey="btndoneResource" />
            &nbsp;<asp:Button ID="btncancel" runat="server" OnClick="ButtonCancel_Click"
                BorderColor="#CCCCCC" BorderStyle="Solid" Width="140px" Text="<%$ Resources: CommonLabels, Cancel %>" />
        </div>
    </div>
</asp:Content>
