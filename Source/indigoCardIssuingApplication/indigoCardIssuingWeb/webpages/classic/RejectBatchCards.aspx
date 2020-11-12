<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="RejectBatchCards.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.classic.RejectBatchCards" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">  
        $('.rejectbtn').puibutton({  
            icon: 'ui-icon-close-thick'
        });
        $('.removebtn').puibutton({
            icon: 'ui-icon-trash'
        });
    </script>


    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblHeader" runat="server" Text="Reject Production Batch" />
            </div>

            <asp:Panel ID="pnlCardsInBatch" runat="server" GroupingText="Cards In Batch" >
                <asp:DataList ID="dlCardRequests" runat="server" 
                            OnItemCommand="dlCardRequests_ItemCommand" CellPadding="0" Font-Names="Verdana" 
                            Font-Size="Small" ForeColor="#333333" Width="100%" >
                    <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="White" />
                    <HeaderTemplate>       
                        <tr>
                            <td style="font-weight: bold;">
                                <asp:Literal Text="Card Reference" runat="server"/>
                            </td>
                            <td style="font-weight: bold;">
                                <asp:Literal Text="Customer Acc No" runat="server" />
                            </td>  
                            <td style="font-weight: bold;">
                                <asp:Literal Text="Customer FID" runat="server" />
                            </td>    
                            <td style="font-weight: bold;">
                                <asp:Literal Text="Reject Reason" runat="server" />
                            </td>  
                            <td style="font-weight: bold;">
                                <asp:Literal Text="Reject" runat="server" />
                            </td>   
                        </tr>
                    </HeaderTemplate>
                    <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                    <ItemTemplate>
                        <tr class="ItemSelect">
                            <td> 
                                <asp:Label ID="lblCardRefNo" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CardReferenceNumber") %>' />
                            </td>           
                            <td>
                                <asp:Label ID="lblCustAccNo" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CustomerAccountNumber") %>' />
                            </td> 
                            <td>
                                <asp:Label ID="lblCustId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CustomerId") %>' />
                            </td> 
                            <td>
                                <asp:Textbox ID="tbReason" runat="server" />
                            </td> 
                            <td>
                                <asp:Button ID="btnRejectCard" runat="server" OnClick="btnRejectCard_OnClick" CssClass="rejectbtn" />
                            </td>
                            <asp:Label ID="lblCardId" Visible="False" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CardId") %>' />
                        </tr>
                    </ItemTemplate>
                    <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                </asp:DataList>

                <div class="PaginationPanel">
                    <asp:LinkButton ID="lnkFirst" runat="server" Text="<%$ Resources: CommonLabels, First %>" OnClick="lnkFirst_Click"> </asp:LinkButton>
                    <asp:LinkButton ID="lnkPrev" runat="server" Text="<%$ Resources: CommonLabels, Prev %>" OnClick="lnkPrev_Click"> </asp:LinkButton>
                    <asp:Label ID="lblPageIndex" runat="server" />
                    <asp:LinkButton ID="lnkNext" runat="server" Text="<%$ Resources: CommonLabels, Next %>" OnClick="lnkNext_Click"> </asp:LinkButton>            
                    <asp:LinkButton ID="lnkLast" runat="server" Text="<%$ Resources: CommonLabels, Last %>" OnClick="lnkLast_Click"> </asp:LinkButton>
                </div>

                <div class="PaginationPanel">
                    <asp:Label runat="server" Text="Total Records: " meta:resourcekey="lblTotalRecordsResource" ID="lblTotalRecords"/>
                    <asp:Label ID="lblTotalRecords2" runat="server" Text="0" />
                </div>
            </asp:Panel>
            <br />

            <asp:Panel ID="pnlRejectedCards" runat="server" GroupingText="Rejected Cards">
                <asp:DataList ID="dlRejectedCardRequests" runat="server" 
                            OnItemCommand="dlCardRequests_ItemCommand" CellPadding="0" Font-Names="Verdana" 
                            Font-Size="Small" ForeColor="#333333" Width="100%" >
                    <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="White" />
                    <HeaderTemplate>       
                        <tr>
                            <td style="font-weight: bold;">
                                <asp:Literal Text="Card Reference" runat="server"/>
                            </td>
                            <td style="font-weight: bold;">
                                <asp:Literal Text="Reason" runat="server" />                            
                            </td>   
                            <td style="font-weight: bold;">
                                <asp:Literal Text="Remove" runat="server" />
                            </td>    
                        </tr>
                    </HeaderTemplate>
                    <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                    <ItemTemplate>
                        <tr class="ItemSelect">
                            <td> 
                                <asp:Label ID="lblRejectCardRefNo" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CardReferenceNumber") %>' />
                            </td>           
                            <td>                             
                                <asp:Label ID="lblRejectReason" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Comment") %>' />
                            </td> 
                            <td>
                                <asp:Button ID="btnRemoveRejectedCard" runat="server" OnClick="btnRemoveRejectedCard_OnClick" CssClass="removebtn" />
                            </td>

                            <asp:Label ID="lblRejectCardId" Visible="False" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CardId") %>' />
                        </tr>
                    </ItemTemplate>
                    <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                </asp:DataList>    
            </asp:Panel>          
        </div>

        <div class="InfoPanel"> 
            <asp:Label ID="lblInfoMessage" runat="server" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" />
        </div>
                
        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnReject" runat="server" Text="<%$ Resources: CommonLabels, Reject %>"   OnClick="btnReject_Click" CssClass="button" />
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" Visible="false"  OnClick="btnConfirm_Click" CssClass="button" />            
            <asp:Button ID="btnCancel" runat="server" Text="<%$ Resources: CommonLabels, Cancel %>"   OnClick="btnCancel_Click" Visible="False" CssClass="button" />

            <asp:Button ID="btnBack" runat="server" OnClick="btnBack_Click" Text="<%$ Resources: CommonLabels, Back %>" CssClass="button" />
        </div>
    </div>
</asp:Content>
