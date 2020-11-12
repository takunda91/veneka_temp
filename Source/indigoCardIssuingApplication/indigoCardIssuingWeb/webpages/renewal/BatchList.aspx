<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="BatchList.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.renewal.BatchList" %>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblCardRenewalList" runat="server" Text="Card Renewal Batches" meta:resourcekey="lblDistributionBatchListResource" />
            </div>

            <asp:DataList ID="dlCardRenewalList" runat="server"
                OnItemCommand="dlCardRenewalList_ItemCommand"
                Width="100%"
                HeaderStyle-Font-Bold="true"
                HeaderStyle-ForeColor="Azure"
                CellPadding="0"
                ForeColor="#333333">


                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle Font-Bold="True" ForeColor="White" BackColor="White"></HeaderStyle>
                <HeaderTemplate>
                    <tr style="font-weight: bold;">
                        <td id="tdselct" runat="server">
                            <asp:Label ID="lblSelect" runat="server" Text="sel" meta:resourcekey="lblSelectResource" />
                        </td>
                        <td>
                            <asp:Label ID="lblProductCount" runat="server" Text="Product" meta:resourcekey="lblProductCount" />
                        </td>
                        <td>
                            <asp:Label ID="lblFileName" runat="server" Text="Batch Number" meta:resourcekey="lblFileName" />
                        </td>
                        <td>
                            <asp:Label ID="lblUploadDate" runat="server" Text="Created" meta:resourcekey="lblUploadDate" />
                        </td>
                        <td>
                            <asp:Label ID="lblBranchCount" runat="server" Text="Branches" meta:resourcekey="lblBranchCount" />
                        </td>

                        <td>
                            <asp:Label ID="lblCardCount" runat="server" Text="Cards" meta:resourcekey="lblCardCount" />
                        </td>
                        <td>
                            <asp:Label ID="lblStatus" runat="server" Text="Status" />
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <ItemTemplate>
                    <tr class="ItemSelect">
                        <td id="tdselct" runat="server">
                            <asp:CheckBox ID="chksel" runat="server" />
                        </td>
                        <td>
                            <asp:LinkButton ID="Label1" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ProductName") %>' CssClass="ItemSelect" />
                        </td>
                        <td>
                            <asp:Label ID="lblRenewalBatchId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "RenewalBatchId") %>' Visible="false" />
                            <asp:LinkButton ID="lblBranchDetail" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CalculatedBatchNumber") %>' CssClass="ItemSelect" />
                        </td>
                        <td>
                            <asp:LinkButton ID="lblBankAccountDetail" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "RenewalDate", DATE_ASPX_FORMAT) %>' CssClass="ItemSelect" />
                        </td>
                        <td>
                            <asp:Label ID="lblPrepaidCardNoDetail" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "BranchCount") %>' CssClass="ItemSelect" />
                        </td>

                        <td>
                            <asp:Label ID="Label2" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CardCount") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblStatusDetail" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "RenewalBatchStatus") %>' />
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
            <asp:Button ID="btnApprove" runat="server" Text="Approve" OnClick="btnApprove_Click" CssClass="button" Visible="true" />
            <asp:Button ID="btnReject" runat="server" Text="Reject" OnClick="btnReject_Click" CssClass="button" Visible="true" />

            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" OnClick="btnConfirm_Click" CssClass="button" Visible="false" />
            <asp:Button ID="btnBack" runat="server" Text="Back" OnClick="btnBack_Click" CssClass="button" CausesValidation="false" Visible="true" />
        </div>
    </div>
</asp:Content>
