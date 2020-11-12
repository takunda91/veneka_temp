<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="PinResetList.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.pin.PinResetList" %>
<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblLoadResetList" runat="server" Text="Pin Reset List" />
            </div>

            <asp:DataList ID="dlPinResetList" runat="server" OnItemCommand="dlPinResetList_ItemCommand"
                            Width="100%" HeaderStyle-Font-Bold="true" HeaderStyle-ForeColor="Azure" OnSelectedIndexChanged="dlPinResetList_SelectedIndexChanged"
                            CellPadding="0" ForeColor="#333333" Font-Names="Verdana" Font-Size="Small" 
                            style="margin-right: 83px">
                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle Font-Bold="True" ForeColor="White" BackColor="White" />
                <HeaderTemplate>
                    <tr style="font-weight:bold;">
                        <td>
                            <asp:Label ID="lblPinReissueDateHeader" runat="server" Text="Request Date" />
                        </td>
                        <td>
                            <asp:Label ID="lblPinReissuePANHeader" runat="server" Text="Card Number"/>
                        </td>
                        <td>
                            <asp:Label ID="lblPinReissueStatusHeader" runat="server" Text="Status" />
                        </td>
                        <td>
                            <asp:Label ID="lblOperatorHeader" runat="server" Text="Operator" />
                        </td>
                        <td>
                            <asp:Label ID="lblRequestExpiry" runat="server" Text="Request Expiry" />
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:Label ID="lblPinReissueRequestDate" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "reissue_date", DATETIME_ASPX_FORMAT) %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblPinReissueId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "pin_reissue_id") %>' Visible="false" />
                            <asp:LinkButton ID="lblCardNumber" runat="server" CommandName="select" Text='<%# DataBinder.Eval(Container.DataItem, "card_number") %>' />       
                        </td>
                        <td>
                            <asp:Label ID="lblPinReissueStatus" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "pin_reissue_statuses_name") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblOperator" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "operator_usename") %>' />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "request_expiry", DATETIME_ASPX_FORMAT) %>' />
                        </td>
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

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource"/>
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource"/>
        </div>
          <div id="pnlButtons" class="ButtonPanel" runat="server"  >
            <asp:Button ID="btnBack" runat="server" Text="Back To Search" OnClick="btnBack_OnClick" CssClass="button" meta:resourcekey="btnBackResource" />
        </div>  
    </div>
</asp:Content>
