<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="ExportBatchList.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.export.ExportBatchList" %>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">

    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblExportBatchList" runat="server" Text="Export Batch List " />
            </div>

            <asp:DataList ID="dlBatchList" runat="server"
                OnItemCommand="dlBatchList_ItemCommand"
                Width="100%"
                HeaderStyle-Font-Bold="true"
                HeaderStyle-ForeColor="Azure"
                OnSelectedIndexChanged="dlBatchList_SelectedIndexChanged"
                CellPadding="0"
                ForeColor="#333333">


                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle Font-Bold="True" ForeColor="White" BackColor="White"></HeaderStyle>
                <HeaderTemplate>
                    <tr style="font-weight: bold;">
                        <td>
                            <asp:Label ID="lblBatchCreatedDateHeader" runat="server" Text="Date Created" meta:resourcekey="lblBatchCreatedDateHeaderResource" />
                        </td>
                        <td>
                            <asp:Label ID="lblbatchReferenceHeader" runat="server" Text="Batch Reference" meta:resourcekey="lblbatchReferenceHeaderResource" />
                        </td>
                        <td>
                            <asp:Label ID="lblBatchStatusHeader" runat="server" Text="Batch Status" meta:resourcekey="lblBatchStatusHeaderResource" />
                        </td>
                        <td>
                            <asp:Label ID="lblIssuerName" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" />
                        </td>
                        <td>
                            <asp:Label ID="lblNumcherOfCardsHeader" runat="server" Text="Number of Cards" meta:resourcekey="lblNumcherOfCardsHeaderResource" />
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <ItemTemplate>
                    <tr class="ItemSelect">
                        <td>
                            <asp:Label ID="lblBatchCreatedDate" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "date_created", DATETIME_ASPX_FORMAT) %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblExportBatchId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "export_batch_id") %>' Visible="false" />
                            <asp:LinkButton ID="lblbatchReference" CommandName="select" Text='<%# DataBinder.Eval(Container.DataItem, "batch_reference") %>' CssClass="ItemSelect" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblBatchStatus" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "export_batch_statuses_name") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lbldlIssuerName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "issuer_name") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblNumcherOfCards" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "no_cards") %>' />
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

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnBack" runat="server" Text="Back To Search" OnClick="btnBack_OnClick" CssClass="button" meta:resourcekey="btnBackResource" />
        </div>
    </div>

</asp:Content>
