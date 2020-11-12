<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="PinMailerList.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.pin.PinMailerList" Culture="auto:en-US" meta:resourcekey="PageResource" UICulture="auto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblCardList" Text="Pin File Upload List" runat="server" meta:resourcekey="lblCardListResource" />
            </div>

             <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr " />
            <asp:DropDownList ID="ddlIssuer" runat="server" AutoPostBack="true" CssClass="input rightclr"
                OnSelectedIndexChanged="ddlIssuer_SelectedIndexChanged" />

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
                            <asp:Label ID="lblListNumber" runat="server" Text="Order Number" meta:resourcekey="lblOrderNumberResource" />
                        </td>
                        <td>
                            <asp:Label ID="lblBatchReference" runat="server" Text="Batch Reference" meta:resourcekey="lblBatchReferenceResource" />
                        </td>
                        <td>
                            <asp:Label ID="lblNumberOfCards" runat="server" Text="Number Of Cards" meta:resourcekey="lblNumberOfCardsResource" />
                        </td>
                      
                        <td>
                            <asp:Label ID="lblCreatedDate" runat="server" Text="Upload Date" meta:resourcekey="lblCreatedDateResource" />
                        </td>
                    </tr>

                </HeaderTemplate>
                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <ItemTemplate>

                    <tr class="ItemSelect">
                          <td>
                            <asp:Label ID="lblListNumberID" runat="server" CommandName="select" Text='<%# DataBinder.Eval(Container.DataItem, "header_pin_file_batch_id") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblHeaderId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "header_pin_file_batch_id") %>' Visible="false" />
                            <asp:LinkButton ID="lblReference" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "header_batch_reference") %>' CssClass="ItemSelect" />
                             <%--<asp:LinkButton ID="lbtnPan" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "product_bin_code")%>' CssClass="ItemSelect" />--%>
                        </td>
                      
                        <td>
                            <asp:Label ID="lblNumberOfCardsRequest" runat="server" CommandName="select" Text='<%# DataBinder.Eval(Container.DataItem, "header_number_of_cards_on_request") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblLastUpdated" runat="server" CommandName="select" Text='<%# DataBinder.Eval(Container.DataItem, "header_upload_date", DATETIME_ASPX_FORMAT)%>' />
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