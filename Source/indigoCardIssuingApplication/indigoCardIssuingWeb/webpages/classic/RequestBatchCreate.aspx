<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="RequestBatchCreate.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.classic.RequestBatchCreate" Culture="auto:en-US" meta:resourcekey="PageResource" UICulture="auto" %>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblCreateBatch" runat="server" Text="Create Batch" meta:resourcekey="lblCreateBatchResource" />
            </div>

            <div id="pnlRequestTable" runat="server">

                <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr" />
                <asp:DropDownList ID="ddlIssuer" runat="server" CssClass="input rightclr" OnSelectedIndexChanged="ddlIssuer_SelectedIndexChanged" AutoPostBack="true" />

                <asp:Label ID="lblBranch" runat="server" Text="<%$ Resources: CommonLabels, Branch %>" CssClass="label leftclr" Visible="false" />
                <asp:DropDownList ID="ddlBranch" runat="server" CssClass="input rightclr" Visible="false" OnSelectedIndexChanged="ddlBranch_SelectedIndexChanged"  AutoPostBack="true" />

                <div id="divstock"   style="display:none;" runat="server">
                        <asp:Label ID="lblstockinbranch" runat="server" Text="Stock in Branch:" CssClass="label leftclr" />
                        <asp:Label ID="lblstockvalue" runat="server"  CssClass="label leftclr" Text="0" />
                
                </div>

                <asp:DataList ID="dlBatchList" runat="server"
                    Width="100%"
                    HeaderStyle-Font-Bold="true"
                    HeaderStyle-ForeColor="Azure"
                    CellPadding="0"
                    ForeColor="#333333" OnItemCommand="dlBatchList_ItemCommand">


                    <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle Font-Bold="True" ForeColor="White" BackColor="White"></HeaderStyle>
                    <HeaderTemplate>
                        <tr style="font-weight: bold;">
                            <td style="text-align:left">
                                <asp:Label ID="lblProductHeader" runat="server" Text="Product" meta:resourcekey="lblProductHeaderResource" />
                            </td>
                            <td>
                                <asp:Label ID="lblPriorityHeader" runat="server" Text="Priority" meta:resourcekey="lblPriorityHeaderResource" />
                            </td>
                            <td>
                                <asp:Label ID="lblnoofcardsHeader" runat="server" Text="No of Cards" meta:resourcekey="lblnoofcardsHeaderResource" />
                            </td>
                            <td></td>
                        </tr>
                    </HeaderTemplate>
                    <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                    <ItemTemplate>
                        <tr class="ItemSelect">
                            <td>
                                <asp:Label ID="lblProduct" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "product_name") %>' />
                            </td>
                            <td>
                                <asp:Label ID="lblcard_priority_id" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "card_priority_id") %>' Visible="false" />
                                <asp:Label ID="lblproduct_id" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "product_id") %>' Visible="false" />
                                <asp:Label ID="lblPriority" CommandName="select" Text='<%# DataBinder.Eval(Container.DataItem, "card_priority_name") %>' CssClass="ItemSelect" runat="server" />
                            </td>
                            <td>
                                <asp:Label ID="lblnoofcards" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "cardscount") %>' />
                            </td>
                            <td style="height: 25px;">
                                <asp:Button ID="btnCreate" runat="server" Text="Create" CommandName="Create" />
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
                <br />
                <br />
            </div>


            <div id="pnlBatchCreate" runat="server" visible="false">
                <asp:Label ID="lblProductName" runat="server" Text="ProductName" CssClass="label leftclr" />
                <asp:TextBox ID="tbProductName" runat="server" CssClass="input rightclr" Enabled="false" />

                <asp:Label ID="lblPriority" runat="server" Text="Priority" CssClass="label leftclr" />
                <asp:TextBox ID="tbPriority" runat="server" CssClass="input rightclr" Enabled="false" />

                <asp:Label ID="lblNumberOfCards" runat="server" Text="Cards" CssClass="label leftclr" />
                <asp:TextBox ID="tbNumberOfCards" runat="server" CssClass="input rightclr" Enabled="false" />
                <br />
                <br />
            </div>
        </div>
        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server" visible="false">
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" CssClass="button" OnClick="btnConfirm_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="<%$ Resources: CommonLabels, Cancel %>" OnClick="btnCancel_Click" CssClass="button" CausesValidation="false" />
        </div>
    </div>
</asp:Content>
