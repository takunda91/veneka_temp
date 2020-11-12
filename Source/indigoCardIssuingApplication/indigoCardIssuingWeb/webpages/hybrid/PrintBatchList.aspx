<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="PrintBatchList.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.hybrid.PrintBatchList" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblPrintBatchList" runat="server" Text="Print Batch List" meta:resourcekey="lblPrintBatchListResource" />
            </div>

            <asp:DataList ID="dlPrintList" runat="server" OnItemCommand="dlPrintList_ItemCommand" OnItemDataBound="dlPrintList_ItemDataBound"
                            Width="100%" HeaderStyle-Font-Bold="true" HeaderStyle-ForeColor="Azure" 
                            CellPadding="0" ForeColor="#333333" Font-Names="Verdana" Font-Size="Small" 
                            style="margin-right: 83px">
                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle Font-Bold="True" ForeColor="White" BackColor="White" />
                <HeaderTemplate>
                    <tr style="font-weight:bold;">
                          <td id="tdselct" runat="server">
                            <asp:Label ID="lblSelect" runat="server" Text="sel" meta:resourcekey="lblSelectResource" />
                        </td>
                        <td>
                            <asp:Label ID="lblBatchCreatedDateHeader" runat="server" Text="Created Date" meta:resourcekey="lblBatchCreatedDateHeaderResource" />
                        </td>
                        <td>
                            <asp:Label ID="lblbatchReferenceHeader" runat="server" Text="Batch Reference" meta:resourcekey="lblbatchReferenceHeaderResource" />
                        </td>
                        <td>
                            <asp:Label ID="lblBatchStatusHeader" runat="server" Text="Batch Status" meta:resourcekey="lblBatchStatusHeaderResource" />
                        </td>
                        <td>
                            <asp:Label ID="lblNumcherOfreqeustsHeader" runat="server" Text="Requests in batch" meta:resourcekey="lblNumcherOfreqeustsHeaderResource" />
                        </td>
                        <td></td>
                    </tr>
                </HeaderTemplate>
                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <ItemTemplate>
                    <tr>
                        <td id="tdselct" runat="server">
                                <asp:CheckBox ID="chksel" runat="server" />
                            <asp:Label ID="lblPrintBatchStatusId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "print_batch_statuses_id") %>' Visible="false" />

                            <%--<asp:Label ID="lblFlowPinBatchId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "flow_pin_batch_statuses_id") %>' Visible="false" />--%>

                         </td>
                        <td>
                            <asp:Label ID="lblBatchCreatedDate" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "date_created") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblPrintBatchId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "print_batch_id") %>' Visible="false" />
                            <asp:LinkButton ID="lblbatchReference" runat="server" CommandName="select" Text='<%# DataBinder.Eval(Container.DataItem, "print_batch_reference") %>' />       
                        </td>
                        <td>
                            <asp:Label ID="lblBatchStatus" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "print_batch_status_name") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblNumcherOfrequests" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "no_cards") %>' />
                        </td>
                         <td><asp:ImageButton ImageUrl="~/images/Report.png" runat="server" CommandName="Report" Width="50px" Height="20px" /> </td>

                    </tr>
                </ItemTemplate>
                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <SelectedItemTemplate>
                </SelectedItemTemplate>
            </asp:DataList>

            <div class="PaginationPanel">
                <asp:LinkButton ID="lnkFirst" runat="server" Text="<%$ Resources: CommonLabels, First %>"  OnClick="lnkFirst_Click"> </asp:LinkButton>
                <asp:LinkButton ID="lnkPrev" runat="server" Text="<%$ Resources: CommonLabels, Prev %>"  OnClick="lnkPrev_Click"> </asp:LinkButton>
                <asp:Label ID="lblPageIndex" runat="server" />
                <asp:LinkButton ID="lnkNext" runat="server" Text="<%$ Resources: CommonLabels, Next %>"  OnClick="lnkNext_Click"> </asp:LinkButton>            
                <asp:LinkButton ID="lnkLast" runat="server" Text="<%$ Resources: CommonLabels, Last %>"  OnClick="lnkLast_Click"> </asp:LinkButton>
            </div>
        </div>
          <div id="pnlremarks" runat="server" style="display:none">
              <asp:Label ID="lblStatusNote" runat="server" Text="Notes" CssClass="label leftclr" meta:resourcekey="lblStatusNoteResource" />
            <asp:TextBox ID="tbStatusNote" runat="server" MaxLength="150" CssClass="input rightclr" />
        </div>
        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource"/>
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource"/>
        </div>
          <div id="pnlButtons" class="ButtonPanel" runat="server"  >
           <%-- <asp:Button ID="btnApprove" runat="server" Text="Approve" OnClick="btnApprove_Click" CssClass="button" />--%>

            <asp:Button ID="btnBack" runat="server" Text="Back To Search" OnClick="btnBack_OnClick" CssClass="button" meta:resourcekey="btnBackResource" />
        </div>  
    </div>
</asp:Content>
