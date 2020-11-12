<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="CustomerCardSearch.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.card.CustomerCardSearch" Culture="auto:en-US" meta:resourcekey="PageResource" UICulture="auto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblcustomercardSearch" runat="server" Text="Customer Card Search" meta:resourcekey="lblcustomercardSearchResource" />
            </div>

            <div>
                <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr" />
                <asp:DropDownList ID="ddlIssuer" runat="server" AutoPostBack="True"
                    OnSelectedIndexChanged="ddlIssuer_OnSelectedIndexChanged"
                    CssClass="input rightclr" />

                <asp:Label ID="lblBranch" runat="server" Text="Branch" CssClass="label leftclr"
                    meta:resourcekey="lblBranchResource" />
                <asp:DropDownList ID="ddlBranch" runat="server" AutoPostBack="True"
                    CssClass="input rightclr" />


                <asp:Label ID="lblCardProduct" runat="server" Text="<%$ Resources: CommonLabels, Product %>" CssClass="label leftclr" />
                <asp:DropDownList ID="ddlProduct" runat="server" AutoPostBack="true" CssClass="input rightclr" />

                <asp:Label ID="lblPriority" runat="server" Text="Priority" CssClass="label leftclr" meta:resourcekey="lblPriorityResource" />
                <asp:DropDownList ID="ddlPriority" runat="server" CssClass="input rightclr" />

                <asp:Label ID="lblCardIssueMethod" runat="server" Text="Card Issue Method" CssClass="label leftclr" meta:resourcekey="lblCardIssueMethodResource" />
                <asp:DropDownList ID="ddlCardIssueMethod" runat="server" CssClass="input rightclr" />

                <asp:Label ID="lblaccountno" runat="server" Text="Account No" CssClass="label leftclr" meta:resourcekey="lblaccountnoResource" />
                <asp:TextBox ID="tbaccountno" runat="server" CssClass="input" MaxLength="25" />
                <asp:Label ID="lblcardrefno" runat="server" Text="Card Ref No" CssClass="label leftclr" meta:resourcekey="lblcardrefnoResource" />
                <asp:TextBox ID="tbcardrefno" runat="server" CssClass="input" MaxLength="25" />

            </div>
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>
        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnSearch" runat="server" CssClass="button" Text="<%$ Resources: CommonLabels, Search %>" OnClick="btnSearch_Click" />
        </div>


        <asp:DataList ID="dlBatchList" runat="server"
            OnItemCommand="dlBatchList_ItemCommand"
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
                    <td>
                        <asp:Label ID="lblCardnumber" runat="server" Text="Card Number" meta:resourcekey="lblCardnumberResource" />
                    </td>
                    <td>
                        <asp:Label ID="lblproduct" runat="server" Text="Product Name" meta:resourcekey="lblproductResource" />
                    </td>
                    <td>
                        <asp:Label ID="lblaccnumber" runat="server" Text="Account Number" meta:resourcekey="lblaccnumberResource" />
                    </td>
                    <td>
                        <asp:Label ID="lblfirstname" runat="server" Text="First Name" meta:resourcekey="lblfirstnameResource" />
                    </td>
                    <td>
                        <asp:Label ID="lbllastname" runat="server" Text="Last Name" meta:resourcekey="lbllastnameResource" />
                    </td>

                </tr>
            </HeaderTemplate>
            <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <ItemTemplate>
                <tr class="ItemSelect">
                    <td>
                        <asp:Label ID="lblcustomeraccountid" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "card_id") %>' Visible="false" />
                        <asp:LinkButton ID="lblbatchReference" CommandName="select" Text='<%# DataBinder.Eval(Container.DataItem, "card_number") %>' CssClass="ItemSelect" runat="server" />

                    </td>
                    <td>
                        <asp:Label ID="lblBatchCreatedDate" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "product_name") %>' />
                    </td>
                    <td>
                        <asp:Label ID="lblBatchStatus" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "account_number") %>' />
                    </td>
                    <td>
                        <asp:Label ID="lbldlIssuerName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "first_name") %>' />
                    </td>
                    <td>
                        <asp:Label ID="lbldlBranchName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "last_name") %>' />
                    </td>

                </tr>
            </ItemTemplate>

            <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <SelectedItemTemplate>
            </SelectedItemTemplate>
        </asp:DataList>

        <div class="PaginationPanel" id="divPaginationPanel" runat="server" visible="false">
            <asp:LinkButton ID="lnkFirst" runat="server" Text="<%$ Resources: CommonLabels, First %>" OnClick="lnkFirst_Click"> </asp:LinkButton>
            <asp:LinkButton ID="lnkPrev" runat="server" Text="<%$ Resources: CommonLabels, Prev %>" OnClick="lnkPrev_Click"> </asp:LinkButton>
            <asp:Label ID="lblPageIndex" runat="server" />
            <asp:LinkButton ID="lnkNext" runat="server" Text="<%$ Resources: CommonLabels, Next %>" OnClick="lnkNext_Click"> </asp:LinkButton>
            <asp:LinkButton ID="lnkLast" runat="server" Text="<%$ Resources: CommonLabels, Last %>" OnClick="lnkLast_Click"> </asp:LinkButton>
        </div>
    </div>
</asp:Content>
