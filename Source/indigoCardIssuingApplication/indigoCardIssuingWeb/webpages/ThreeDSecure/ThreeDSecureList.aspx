<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="ThreeDSecureList.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.ThreeDSecure.ThreeDSecureList" culture="auto" meta:resourcekey="PageResource1" uiculture="auto" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
     <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblThreeDSecureBatchList" runat="server" Text="ThreeDSecure Batch List" meta:resourcekey="lblThreeDSecureBatchListResource" />
            </div>
            <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources:CommonLabels, Issuer %>" meta:resourcekey="lblIssuerResource1" />
            <asp:DropDownList ID="ddlIssuer" runat="server" AutoPostBack="True"
                OnSelectedIndexChanged="ddlIssuer_OnSelectedIndexChanged" meta:resourcekey="ddlIssuerResource1" />
            <br />

            <asp:DataList ID="dlBatchList" runat="server"
                OnItemCommand="dlBatchList_ItemCommand" OnItemDataBound="dlBatchList_ItemDataBound"
                Width="100%"
                HeaderStyle-Font-Bold="true"
                HeaderStyle-ForeColor="Azure" CellPadding="0" ForeColor="#333333"
                Font-Names="Verdana" Font-Size="Small" BackColor="White"
                BorderColor="#CC0099" meta:resourcekey="dlBatchListResource1">

                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle Font-Bold="True" ForeColor="White"></HeaderStyle>
                <HeaderTemplate>
                    <tr>
                       
                         <td id="tdselct" runat="server" style="font-weight: bold">
                            <asp:Label ID="lblSelect" runat="server" Text="sel" meta:resourcekey="lblSelectResource" />
                        </td>
                        <td style="font-weight: bold">
                            <asp:Label ID="lblbatchReferenceHeader" runat="server" Text="Batch Reference" meta:resourcekey="lblbatchReferenceHeaderResource" />
                        </td>
                        <td style="font-weight: bold">
                            <asp:Label ID="lblBatchStatusHeader" runat="server" Text="Batch Status" meta:resourcekey="lblBatchStatusHeaderResource" />
                        </td>
                      
                        <td style="font-weight: bold">
                            <asp:Label ID="lblNumcherOfCardsHeader" runat="server" Text="Number of Cards" meta:resourcekey="lblNumcherOfCardsHeaderResource" />
                        </td>
                                                <td style="font-weight: bold">
                            <asp:Label ID="lblBatchLoadDateHeader" runat="server" Text="Created Date" meta:resourcekey="lblBatchLoadDateHeaderResource" />
                        </td>
                        <td></td>
                    </tr>
                </HeaderTemplate>
                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <ItemTemplate>
                    <tr class="ItemSelect">
                          <td id="tdselct" runat="server">
                            <asp:CheckBox ID="chksel" runat="server" meta:resourcekey="chkselResource1" />
                        </td>
                      
                        <td>
                            <asp:Label ID="lblThreedBatchId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "threed_batch_id") %>' Visible="False" meta:resourcekey="lblThreedBatchIdResource1" />
                            <asp:LinkButton ID="lblbatchReference" CommandName="select" Text='<%# DataBinder.Eval(Container.DataItem, "batch_reference") %>' CssClass="ItemSelect" runat="server" meta:resourcekey="lblbatchReferenceResource1" />
                        </td>
                        <td>
                            <asp:Label ID="lblBatchStatus" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "batch_status") %>' meta:resourcekey="lblBatchStatusResource1" />
                        </td>
                      
                        <td>
                            <asp:Label ID="lblNumcherOfCards" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "no_cards") %>' meta:resourcekey="lblNumcherOfCardsResource1" />
                        </td>
                          <td>
                            <asp:Label ID="lblBatchLoadDate" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "date_created", DATETIME_ASPX_FORMAT) %>' meta:resourcekey="lblBatchLoadDateResource1" />
                        </td>
                         <td>
                             <%--<asp:ImageButton ImageUrl="~/images/Report.png" ID="imgreport"  runat="server" CommandName="Report" Width="50px" Height="20px" meta:resourcekey="imgreportResource1" />--%>

                         </td>
                    </tr>
                </ItemTemplate>

                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <SelectedItemTemplate>
                </SelectedItemTemplate>
            </asp:DataList>

            <div class="PaginationPanel">
                <asp:LinkButton ID="lnkFirst" runat="server" Text="<%$ Resources:CommonLabels, First %>" OnClick="lnkFirst_Click" meta:resourcekey="lnkFirstResource1"></asp:LinkButton>
                <asp:LinkButton ID="lnkPrev" runat="server" Text="<%$ Resources:CommonLabels, Prev %>" OnClick="lnkPrev_Click" meta:resourcekey="lnkPrevResource1"></asp:LinkButton>
                <asp:Label ID="lblPageIndex" runat="server" meta:resourcekey="lblPageIndexResource1" />
                <asp:LinkButton ID="lnkNext" runat="server" Text="<%$ Resources:CommonLabels, Next %>" OnClick="lnkNext_Click" meta:resourcekey="lnkNextResource1"></asp:LinkButton>
                <asp:LinkButton ID="lnkLast" runat="server" Text="<%$ Resources:CommonLabels, Last %>" OnClick="lnkLast_Click" meta:resourcekey="lnkLastResource1"></asp:LinkButton>
            </div>
        </div>
         <div id="pnlremarks" runat="server" visible="false">
              <asp:Label ID="lblStatusNote" runat="server" Text="Notes" CssClass="label leftclr" meta:resourcekey="lblStatusNoteResource" />
            <asp:TextBox ID="tbStatusNote" runat="server" MaxLength="150" CssClass="input rightclr" meta:resourcekey="tbStatusNoteResource1" />
        </div>
        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>
          <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnApprove" runat="server" Text="Approve" OnClick="btnApprove_Click" CssClass="button" meta:resourcekey="btnApproveResource1" />

            <asp:Button ID="btnBack" runat="server" Text="Back To Search" OnClick="btnBack_OnClick" CssClass="button" meta:resourcekey="btnbacktosearchResource" />
        </div>
      
    </div>
</asp:Content>

