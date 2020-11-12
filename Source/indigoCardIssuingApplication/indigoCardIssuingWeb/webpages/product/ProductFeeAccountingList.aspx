<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="ProductFeeAccountingList.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.product.ProductFeeAccountingList" %>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblProductFeeListHEader" runat="server" Text="Manage Fee Schemes" />
            </div>

            <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr " />
            <asp:DropDownList ID="ddlIssuer" runat="server" AutoPostBack="true" CssClass="input rightclr"
                              OnSelectedIndexChanged="ddlIssuer_SelectedIndexChanged"  />  
            
            <asp:DataList ID="dlFeeAccountingList" runat="server" 
                            OnItemCommand="dlFeeAccountingList_ItemCommand" 
                            Width="100%"
                            HeaderStyle-Font-Bold="true"
                            HeaderStyle-ForeColor="Azure" 
                            CellPadding="0" 
                            ForeColor="#333333" >


                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle Font-Bold="True" ForeColor="White" BackColor="White"></HeaderStyle>
                <HeaderTemplate>
                    <tr style="font-weight:bold;">
                        <td >
                            <asp:Label ID="lblFeeAccountingNameH" runat="server" Text="Fee Accounting"/>
                        </td>                                      
                        <%--<td >
                            <asp:Label ID="lblIssuerNameH" runat="server"    Text="<%$ Resources: CommonLabels, Issuer %>"  />
                        </td> --%>                   
                    </tr>  
                </HeaderTemplate>
                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <ItemTemplate>
                    <tr class="ItemSelect">                    
                        <td >
                            <asp:Label ID="lblFeeAccountingId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "fee_accounting_id") %>' Visible="false" />
                            <asp:LinkButton ID="lnkFeeAccountingName"  CommandName="select" Text='<%# DataBinder.Eval(Container.DataItem, "fee_accounting_name") %>' CssClass="ItemSelect" runat="server"  />
                        </td>
                        <%--<td >
                            <asp:Label ID="lblIssuerName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "issuer_name") %>'  />
                        </td>     --%>               
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
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource"/>
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource"/>
        </div> 
    </div>
</asp:Content>
