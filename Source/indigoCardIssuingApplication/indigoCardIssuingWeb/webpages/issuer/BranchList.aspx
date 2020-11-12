<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="BranchList.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.issuer.BranchList" Culture="auto:en-US" meta:resourcekey="PageResource" UICulture="auto" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="Description" runat="server" Text="Branches For Issuer" meta:resourcekey="lblBranchesForIssuerResource" />
            </div>

            <asp:DropDownList ID="ddlIssuer" runat="server" AutoPostBack="true" CssClass="input leftclr"
                OnSelectedIndexChanged="ddlIssuer_SelectedIndexChanged" />
            <asp:DropDownList ID="ddlBranchStatus" runat="server" AutoPostBack="true" CssClass="input rightclr"
                OnSelectedIndexChanged="ddlBranchStatus_SelectedIndexChanged" />

            <br />

            <asp:DataList ID="dlBranchList" runat="server"
                OnItemCommand="dlBranchList_ItemCommand" CellPadding="0" Font-Names="Verdana"
                Font-Size="Small" ForeColor="#333333" Width="100%"
                meta:resourcekey="dlBranchListResource1">
                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="White" />
                <HeaderTemplate>
                    <tr>
                        <td style="font-weight: bold;">
                            <asp:Literal Text="Branch Code" runat="server" meta:resourcekey="lblBranchCodeResource" />
                        </td>
                        <td style="font-weight: bold;">
                            <asp:Literal Text="Branch Name" runat="server" meta:resourcekey="lblBranchNameResource" />
                        </td>
                        <td style="font-weight: bold;">
                            <asp:Literal Text="Branch Status" runat="server" meta:resourcekey="lblBranchStatusResource" />
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <ItemTemplate>
                    <tr class="ItemSelect">
                        <td>
                            <asp:Label ID="lblBranchCode" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "branch_code") %>' />
                        </td>
                        <td>
                            <asp:LinkButton ID="lblBranchName" CommandName="select" runat="server" CssClass="ItemSelect" Text='<%# DataBinder.Eval(Container.DataItem, "branch_name") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblBranchStatus" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "branch_status") %>' />
                        </td>
                        <asp:Label ID="lblBranchID" Visible="False" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "branch_id") %>' />
                    </tr>
                </ItemTemplate>
                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            </asp:DataList>
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>
    </div>
</asp:Content>
