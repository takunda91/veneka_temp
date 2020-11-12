<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true"
    CodeBehind="FontList.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.system.FontList" UICulture="auto" meta:resourcekey="PageResource" Culture="auto:en-US" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblFontlistHeading" runat="server" Text="Font List" meta:resourcekey="lblFontlistHeadingResource" />
            </div>
            <asp:DataList ID="dlfontlist" runat="server"
                HeaderStyle-Font-Bold="true"
                Width="100%"
                OnItemCommand="dlfontlist_ItemCommand" CellPadding="0" ForeColor="#333333">

                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="White" />

                <HeaderTemplate>
                    <tr style="font-weight: bold;">
                        <td colspan="0">
                            <asp:Label ID="lblFontName" runat="server" Text="Font Name" meta:resourcekey="lblFontNameResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblResourcePath" runat="server" Text="Resource Path" meta:resourcekey="lblResourcePathResource" /></td>
                    </tr>
                </HeaderTemplate>

                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />

                <ItemTemplate>
                    <tr class="ItemSelect">
                        <td colspan="0">
                            <asp:Label ID="lblfontid" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "font_id") %>' Visible="false" />
                            <asp:LinkButton ID="lnkFontName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "font_name") %>' CssClass="ItemSelect" CommandName="Select" /></td>
                        <td colspan="0">
                            <asp:Label ID="lbltResourcePath" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "resource_path") %>' CommandName="Select" />
                        </td>


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
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>
    </div>
</asp:Content>
