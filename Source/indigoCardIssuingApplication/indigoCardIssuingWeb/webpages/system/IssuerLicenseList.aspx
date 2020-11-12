<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="IssuerLicenseList.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.system.IssuerLicenseList"
    UICulture="auto" meta:resourcekey="PageResource" Culture="auto:en-US" %>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblIssuerLicenseList" runat="server" Text="Issuer License List" meta:resourcekey="lblIssuerLicenseListResource" />
            </div>

            <br />

            <asp:DataList ID="dlLicenselist" runat="server"
                HeaderStyle-Font-Bold="true"
                Width="100%"
                OnItemCommand="dlLicenselist_ItemCommand" CellPadding="0"
                ForeColor="#333333" OnItemDataBound="dlLicenselist_ItemDataBound">

                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="White" />

                <HeaderTemplate>
                    <tr style="font-weight: bold;">
                        <td colspan="0">
                            <asp:Label ID="lblHeaderIssuerName" runat="server" Text="Issuer Name" meta:resourcekey="lblProductNameResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblHeaderProductCode" runat="server" Text="Issuer Code" meta:resourcekey="lblProductCodeResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblHeaderExpiry" runat="server" Text="License Expiry" meta:resourcekey="lblProductBincodeResource" /></td>
                    </tr>
                </HeaderTemplate>

                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />

                <ItemTemplate>
                    <tr class="ItemSelect">
                        <td colspan="0">
                            <asp:Label ID="lblIssuerName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ID") %>' Visible="false" />
                            <asp:LinkButton ID="lnkIssuerName" CommandName="Select" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "IssuerName") %>' CssClass="ItemSelect" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblProductCode" CommandName="Select" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "IssuerCode") %>' />
                        </td>
                        <td colspan="0">
                            <asp:Label ID="lblExpiry" CommandName="Select" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ExpiryDate") %>' />
                        </td>
                    </tr>
                </ItemTemplate>
                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            </asp:DataList>
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>
    </div>
</asp:Content>
