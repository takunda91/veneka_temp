<%@ Page Title="Product List" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" meta:resourcekey="PageResource" UICulture="auto" Culture="auto:en-US"
    CodeBehind="ProductList.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.product.ProductList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblProductList" runat="server" meta:resourcekey="lblProductListResource" />
            </div>

            <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr " />
            <asp:DropDownList ID="ddlIssuer" runat="server" AutoPostBack="true" CssClass="input rightclr"
                OnSelectedIndexChanged="ddlIssuer_SelectedIndexChanged" />
            
            <asp:DropDownList ID="ddlProductStatus" runat="server" AutoPostBack="true" CssClass="input rightclr"
                OnSelectedIndexChanged="ddlProductStatus_SelectedIndexChanged">
                <asp:ListItem Selected="True" Value="0" Text="All"></asp:ListItem>
                <asp:ListItem Selected="False" Value="1" Text="Active"></asp:ListItem>
                <asp:ListItem Selected="False" Value="2" Text="Inactive"></asp:ListItem>
            </asp:DropDownList>

            <br />

            <asp:DataList ID="dlproductlist" runat="server"
                HeaderStyle-Font-Bold="true"
                Width="100%"
                OnItemCommand="dlproductlist_ItemCommand" CellPadding="0" ForeColor="#333333">

                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="White" />

                <HeaderTemplate>
                    <tr style="font-weight: bold;">
                        <td colspan="0">
                            <asp:Label ID="lblProductName" runat="server" Text="Product Name" meta:resourcekey="lblProductNameResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblProductCode" runat="server" Text="Product Code" meta:resourcekey="lblProductCodeResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblProductBincode" runat="server" Text="Product Bin Code" meta:resourcekey="lblProductBincodeResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblProductStatus" runat="server" Text="Status" /></td>
                    </tr>
                </HeaderTemplate>

                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />

                <ItemTemplate>
                    <tr class="ItemSelect">
                        <td colspan="0">
                            <asp:Label ID="lblProductid" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "product_id") %>' Visible="false" />
                            <asp:LinkButton ID="lnkProductName" CommandName="Select" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "product_name") %>' CssClass="ItemSelect" /></td>
                        <td colspan="0">
                            <asp:Label ID="lbltProductCode" CommandName="Select" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "product_code") %>' />
                        </td>
                        <td colspan="0">
                            <asp:Label ID="lbltProductBincode" CommandName="Select" runat="server" Text='<%# String.Format("{0}{1}{2}", DataBinder.Eval(Container.DataItem, "product_bin_code"), 
                                        DataBinder.Eval(Container.DataItem, "sub_product_code") != null && !String.IsNullOrWhiteSpace(DataBinder.Eval(Container.DataItem, "sub_product_code").ToString()) ? "-" : "", 
                                        DataBinder.Eval(Container.DataItem, "sub_product_code")) %>' />
                        </td>
                        <td colspan="0">
                            <asp:Label ID="lbltProductStatus" CommandName="Select" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "DeletedYN").ToString().ToUpper() == "FALSE" ? "ACTIVE" : "DELETED" %>' /></td>
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
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
    </div>
</asp:Content>
