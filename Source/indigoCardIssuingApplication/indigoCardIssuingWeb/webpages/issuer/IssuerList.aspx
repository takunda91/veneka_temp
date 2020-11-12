<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="IssuerList.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.issuer.IssuerList" UICulture="auto" Culture="auto:en-US" meta:resourcekey="PageResource" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="Literal1" meta:resourcekey="lblIssuersResource" runat="server" />
            </div>

            <asp:DataList ID="dlIssuersList" runat="server" DataKeyField="issuer_id"
                OnItemCommand="dlIssuersList_ItemCommand" CellPadding="0"
                ForeColor="#333333" Width="400px" CaptionAlign="Left"
                Style="text-align: left">

                <AlternatingItemStyle ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle Font-Bold="True" ForeColor="White" HorizontalAlign="Left" />

                <HeaderTemplate>
                    <tr style="width: 800px; font-weight: bold;">
                        <td style="padding-right: 2px;">
                            <asp:Literal ID="lblIssuerCode" runat="server" meta:resourcekey="lblIssuerCodeResource" /></td>
                        <td style="padding-right: 2px;">
                            <asp:Literal ID="lblIssuerName" runat="server" meta:resourcekey="lblIssuerNameResource" /></td>
                        <td style="padding-right: 2px;">
                            <asp:Literal ID="lblIssuerStatus" runat="server" meta:resourcekey="lblIssuerStatusResource" /></td>
                        <td></td>
                    </tr>
                </HeaderTemplate>

                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />

                <ItemTemplate>
                    <tr class="ItemSelect">
                        <td>
                            <asp:Label ID="IssuerCode" CommandName="select" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "issuer_code") %>' />
                        </td>
                        <td>
                            <asp:LinkButton ID="lbtnIssuerName" CommandName="select" Text='<%# DataBinder.Eval(Container.DataItem, "issuer_name") %>' runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblIssuerstatus" CommandName="select" Text='<%# DataBinder.Eval(Container.DataItem, "issuer_status") %>' runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblIssuerID" runat="server" Visible="False" Text='<%# DataBinder.Eval(Container.DataItem, "issuer_id") %>' />
                        </td>
                    </tr>
                </ItemTemplate>

                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <SelectedItemTemplate>
                </SelectedItemTemplate>
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
    </div>
</asp:Content>
