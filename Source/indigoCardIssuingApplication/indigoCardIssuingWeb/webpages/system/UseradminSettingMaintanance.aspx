<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="UseradminSettingMaintanance.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.users.UseradminSettingMaintanance" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="Div1" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="Label1" runat="server" Text="User Admin Settings" meta:resourcekey="lblUseradminResource" />
            </div>

         

            <asp:DataList ID="dlUseradminSettings" runat="server" CssClass="bothclr"
                OnItemCommand="dlUseradminSettings_ItemCommand" CellPadding="0"
                ForeColor="#333333" Width="100%">

                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />

                <HeaderTemplate>
                    <tr style="font-weight: bold;">
                        <td colspan="0">
                            <asp:Label ID="lbPasswordValidPeriod" runat="server" Text="Password Valid Period" meta:resourcekey="lbPasswordValidPeriodResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lbPasswordMinLength" runat="server" Text="PasswordMinLength" meta:resourcekey="lbPasswordMinLengthResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lbPasswordMaxLength" runat="server" Text="PasswordMaxLength" meta:resourcekey="lbPasswordMaxLengthResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lbPreviousPasswordsCount" runat="server" Text="PreviousPasswordsCount" meta:resourcekey="lbPreviousPasswordsCountResource" /></td>
                       
                    </tr>
                </HeaderTemplate>

                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />

                <ItemTemplate>
                    <tr class="ItemSelect">
                        <td>
                            <asp:Label ID="lbluseradminsettingsID" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "user_admin_id") %>' Visible="false" />
                            <asp:LinkButton ID="lblPasswordValidPeriod" CommandName="select" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PasswordValidPeriod") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblPasswordMinLength" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PasswordMinLength") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblPasswordMaxLength" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PasswordMaxLength") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblPreviousPasswordsCount" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PreviousPasswordsCount") %>'  />
                        </td>
                   
                    </tr>
                </ItemTemplate>
                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <SeparatorStyle />
            </asp:DataList>

            <div class="PaginationPanel" style="display:none;">
                <asp:LinkButton ID="LinkButton1" runat="server" Text="<%$ Resources: CommonLabels, First %>" OnClick="lnkFirst_Click"> </asp:LinkButton>
                <asp:LinkButton ID="LinkButton2" runat="server" Text="<%$ Resources: CommonLabels, Prev %>" OnClick="lnkPrev_Click"> </asp:LinkButton>
                <asp:Label ID="Label2" runat="server" />
                <asp:LinkButton ID="LinkButton3" runat="server" Text="<%$ Resources: CommonLabels, Next %>" OnClick="lnkNext_Click"> </asp:LinkButton>
                <asp:LinkButton ID="LinkButton4" runat="server" Text="<%$ Resources: CommonLabels, Last %>" OnClick="lnkLast_Click"> </asp:LinkButton>
            </div>
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnCreateUserAdminSettings" runat="server" Text="Create User Admin Settings"
                OnClick="btnCreateUserAdminSettings_Click" CssClass="button" Width="200px"
                meta:resourcekey="btnCreateUserGroupButtonResource" />
        </div>
    </div>
</asp:Content>

