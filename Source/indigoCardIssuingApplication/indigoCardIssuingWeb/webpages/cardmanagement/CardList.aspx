<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="CardList.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.cardmanagement.CardList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblCardList" Text="Card List" runat="server" meta:resourcekey="lblCardListResource" />
            </div>

            <asp:DataList ID="dlCardList" runat="server"
                OnItemCommand="dlCardList_ItemCommand"
                Width="100%"
                HeaderStyle-Font-Bold="true"
                HeaderStyle-ForeColor="Azure" Style="margin-top: 0px" CellPadding="0"
                ForeColor="#333333" Font-Names="Verdana" Font-Size="Small">

                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle Font-Bold="True" ForeColor="White"></HeaderStyle>

                <HeaderTemplate>
                    <tr style="font-weight: bold">
                        <td>
                            <asp:Label ID="lblCardNumber" runat="server" Text="Card Number" meta:resourcekey="lblCardNumberResource" />
                        </td>
                        <td>
                            <asp:Label ID="lblCardReferenceNumber" runat="server" Text="Card Reference Number" meta:resourcekey="lblCardReferenceNumberResource" />
                        </td>
                        <td>
                            <asp:Label ID="lblcardStatus" runat="server" Text="Card Status" meta:resourcekey="lblcardStatusResource" />
                        </td>
                        <td>
                            <asp:Label ID="lbllastupdated" runat="server" Text="Last Updated" meta:resourcekey="lbllastupdatedResource" />
                        </td>
                        <td>
                            <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" />
                        </td>
                        <td>
                            <asp:Label ID="lblBranch" runat="server" Text="<%$ Resources: CommonLabels, Branch %>" />
                        </td>
                    </tr>

                </HeaderTemplate>
                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <ItemTemplate>

                    <tr class="ItemSelect">
                        <td>
                            <asp:Label ID="lblCardId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "card_id") %>' Visible="false" />
                            <asp:LinkButton ID="lbtnCardNumber" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "card_number") %>' CssClass="ItemSelect" />
                        </td>
                        <td>
                            <asp:Label ID="lblCardReferenceNumber" runat="server" CommandName="select" Text='<%# DataBinder.Eval(Container.DataItem, "card_request_reference") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblCardStatus" runat="server" CommandName="select" Text='<%# DataBinder.Eval(Container.DataItem, "current_card_status") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblLastUpdated" runat="server" CommandName="select" Text='<%# DataBinder.Eval(Container.DataItem, "status_date", DATETIME_ASPX_FORMAT)%>' />
                        </td>
                        <td>
                            <asp:Label ID="lbldlIssuerName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "issuer_name") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lbldlBranchName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "branch_name") %>' />
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

            <div class="PaginationPanel">
                <asp:Label runat="server" Text="Total Records: " meta:resourcekey="lblTotalRecordsResource" ID="lblTotalRecords" />
                <asp:Label ID="lblTotalRecords2" runat="server" Text="0" />
            </div>
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources: CommonLabels, Back %>" CssClass="button" OnClick="btnBack_OnClick" Visible="False" />
        </div>
    </div>
</asp:Content>
