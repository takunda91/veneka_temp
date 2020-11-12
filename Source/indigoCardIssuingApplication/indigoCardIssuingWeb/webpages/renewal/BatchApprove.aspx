<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="BatchApprove.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.renewal.BatchApprove" %>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div class="ContentHeader">
            <asp:Label ID="lblVerifyBatchHeader" runat="server" Text="Finalize Renewal Batch" meta:resourcekey="renewalVerifyHeader" />
        </div>
        <div style="width: 50%; float: left;">
            <div runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="lblFileName" runat="server" Text="Batch" CssClass="label2col leftclr" />
                <asp:TextBox ID="txtBatchNumber" runat="server" CssClass="input2col rightclr" MaxLength="50" Enabled="false" />
            </div>

            <div runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="lblUploaded" runat="server" Text="Uploaded" CssClass="label2col leftclr" />
                <asp:TextBox ID="txtCreated" runat="server" CssClass="input2col rightclr" MaxLength="50" Enabled="false" />
            </div>

            <div runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="lblStatus" runat="server" Text="Status" CssClass="label2col leftclr" />
                <asp:TextBox ID="txtStatus" runat="server" CssClass="input2col rightclr" MaxLength="50" Enabled="false" />
            </div>
        </div>

        <div style="width: 50%; float: right">
            <div runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="Label3" runat="server" Text="Branches" CssClass="label2col leftclr" meta:resourcekey="loadFundsCreatedBy" />
                <asp:TextBox ID="txtBranchCount" runat="server" CssClass="input2col" MaxLength="27" Enabled="false" />
            </div>

            <div runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="Label2" runat="server" Text="Product" CssClass="label2col leftclr" meta:resourcekey="loadFundsCreatedDate" />
                <asp:TextBox ID="txtProductCount" runat="server" CssClass="input2col" MaxLength="27" Enabled="false" />
            </div>

            <div runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="Label1" runat="server" Text="Cards" CssClass="label2col leftclr" meta:resourcekey="loadFundsCreatedBy" />
                <asp:TextBox ID="txtCardCount" runat="server" CssClass="input2col" MaxLength="27" Enabled="false" />
            </div>
        </div>

        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblCardRenewalList" runat="server" Text="Card Renewal Batches" meta:resourcekey="lblDistributionBatchListResource" />
            </div>

            <asp:DataList ID="dlCardRenewalList" runat="server"
                Width="100%"
                HeaderStyle-Font-Bold="true"
                HeaderStyle-ForeColor="Azure"
                CellPadding="0"
                ForeColor="#333333" OnItemCommand="dlCardRenewalList_ItemCommand" OnItemDataBound="dlCardRenewalList_ItemDataBound" OnSelectedIndexChanged="dlCardRenewalList_SelectedIndexChanged">


                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle Font-Bold="True" ForeColor="White" BackColor="White"></HeaderStyle>
                <HeaderTemplate>
                    <tr style="font-weight: bold;">
                        <td>
                            <asp:Label ID="lblCardNumber" runat="server" Text="Card Number" meta:resourcekey="lblCardNumber" />
                        </td>
                        <td>
                            <asp:Label ID="lblExpiryDate" runat="server" Text="Expiry Date" meta:resourcekey="lblExpiryDate" />
                        </td>
                        <td>
                            <asp:Label ID="lblCurrencyCode" runat="server" Text="Currency" meta:resourcekey="lblCurrencyCode" />
                        </td>
                        <td>
                            <asp:Label ID="lblCustomerName" runat="server" Text="Customer" meta:resourcekey="lblCustomerName" />
                        </td>
                        <td>
                            <asp:Label ID="lblEmbossingName" runat="server" Text="Embossing Name" meta:resourcekey="lblEmbossingName" />
                        </td>
                        <td>
                            <asp:Label ID="lblPassportIDNumber" runat="server" Text="ID Number" />
                        </td>
                        <td>
                            <asp:Label ID="lblStatus" runat="server" Text="Status" />
                        </td>
                        <td>
                            <asp:Label ID="lblRenewalStatus" runat="server" Text="Renewal Status" meta:resourcekey="lblRenewalStatus" />
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <ItemTemplate>
                    <tr class="ItemSelect">
                        <td>
                            <asp:Label ID="lblLineRenewalDetailId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "RenewalDetailId") %>' Visible="false" />
                            <asp:LinkButton ID="lblLineCardNumber" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CardNumber") %>' CssClass="ItemSelect" />
                        </td>
                        <td>
                            <asp:LinkButton ID="lblLineExpiryDate" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ExpiryDate", DATE_ASPX_FORMAT) %>' CssClass="ItemSelect" />
                        </td>
                        <td>
                            <asp:Label ID="lblLineCurrencyCode" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CurrencyCode") %>' />
                        </td>
                        <td>
                            <asp:LinkButton ID="lblLineCustomerName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CustomerName") %>' CssClass="ItemSelect" />
                        </td>
                        <td>
                            <asp:LinkButton ID="lblLineEmbossingName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "EmbossingName") %>' CssClass="ItemSelect" />
                        </td>
                        <td>
                            <asp:Label ID="lblLinePassportIDNumber" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PassportIDNumber") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblLineStatus" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Status") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblLineRenewalStatus" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "RenewalStatus") %>' />
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
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="loadFundsInfo" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="loadFundsError" />
        </div>
        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnApprove" runat="server" Text="Approve" OnClick="btnApprove_Click" CssClass="button" Visible="true" />
            <asp:Button ID="btnReject" runat="server" Text="Reject" OnClick="btnReject_Click" CssClass="button" Visible="true" />
            
            <asp:Button ID="btnPrint" runat="server" Text="<%$ Resources: CommonLabels, PrintDocument %>" OnClick="btnPrint_Click" CssClass="button" Visible="False" />
            
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" OnClick="btnConfirm_Click" CssClass="button" Visible="false" />
            <asp:Button ID="btnBack" runat="server" Text="Back" OnClick="btnBack_Click" CssClass="button" CausesValidation="false" Visible="true" />
        </div>

    </div>

</asp:Content>
