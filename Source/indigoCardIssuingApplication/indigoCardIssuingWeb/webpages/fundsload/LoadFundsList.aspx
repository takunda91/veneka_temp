<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="LoadFundsList.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.fundsload.LoadFundsList" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblFundsLoadList" runat="server" Text="Funds Upload List " meta:resourcekey="lblDistributionBatchListResource" />
            </div>

            <asp:DataList ID="dlFundsLoadList" runat="server"
                OnItemCommand="dlFundsLoadList_ItemCommand" OnItemDataBound="dlFundsLoadList_ItemDataBound"
                Width="100%"
                HeaderStyle-Font-Bold="true"
                HeaderStyle-ForeColor="Azure"
                OnSelectedIndexChanged="dlFundsLoadList_SelectedIndexChanged"
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
                            <asp:Label ID="lblBranch" runat="server" Text="Branch" meta:resourcekey="lblBankAccountNoResource" />
                        </td>
                        <td>
                            <asp:Label ID="lblBankAccountNo" runat="server" Text="Bank Account" meta:resourcekey="lblBankAccountNoResource" />
                        </td>
                        <td>
                            <asp:Label ID="lblPrepaidCardNo" runat="server" Text="Prepaid Card" meta:resourcekey="lblPrepaidCardNoResource" />
                        </td>
                        <td>
                            <asp:Label ID="lblAmount" runat="server" Text="Amount" meta:resourcekey="lblAmountResource" />
                        </td>
                        <td>
                            <asp:Label ID="lblStatus" runat="server" Text="Status" />
                        </td>
                        <td>
                            <asp:Label ID="lblCreateDate" runat="server" Text="Created" />
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <ItemTemplate>
                    <tr class="ItemSelect">
                        <td id="tdselct" runat="server">
                            <asp:CheckBox ID="chksel" runat="server" />
                            <asp:Label ID="lblFundsLoadId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Id") %>' Visible="false" />
                        </td>
                        <td>
                            <asp:Label ID="lblBranchDetail" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "BranchName") %>' />
                        </td>
                        <td>
                            <asp:LinkButton ID="lblBankAccountDetail" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "BankAccountNo") %>' CssClass="ItemSelect" />
                        </td>
                        <td>
                            <asp:LinkButton ID="lblPrepaidCardNoDetail" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PrepaidCardNo") %>' CssClass="ItemSelect" />
                        </td>
                        <td>
                            <asp:Label ID="lblAmountDetail" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Amount") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblStatusDetail" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Status") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblCreatedDetail" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Created", DATETIME_ASPX_FORMAT) %>' />
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
        <div id="pnlremarks" runat="server" visible="true">
            <asp:Label ID="lblStatusNote" runat="server" Text="Notes" CssClass="label leftclr" meta:resourcekey="lblStatusNoteResource" />
            <asp:TextBox ID="tbStatusNote" runat="server" MaxLength="150" CssClass="input rightclr" />
        </div>
        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnApprove" runat="server" Text="Approve" OnClick="btnApprove_Click" CssClass="button" />

            <asp:Button ID="btnReject" runat="server" Text="Reject" OnClick="btnReject_Click" CssClass="button" />

            <asp:Button ID="btnConfirm" runat="server" Text="Confirm" CssClass="button" OnClick="btnConfirm_Click" Visible="False" />

            <asp:Button ID="btnBack" runat="server" Text="Back To Search" OnClick="btnBack_OnClick" CssClass="button" meta:resourcekey="btnBackResource" />
        </div>
    </div>

</asp:Content>
