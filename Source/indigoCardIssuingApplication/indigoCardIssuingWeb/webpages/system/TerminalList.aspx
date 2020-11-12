<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="TerminalList.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.system.TerminalList" UICulture="auto" meta:resourcekey="PageResource" Culture="auto:en-US" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblTerminalListHeading" runat="server" Text="Terminal List" meta:resourcekey="lblTermnialListHeadingResource" />
            </div>

            <asp:Label runat="server" ID="lblIssuer" Text="Issuer" meta:resourcekey="lblIssuerResource" CssClass="label leftclr"></asp:Label>
            <asp:DropDownList runat="server" ID="ddlIssuer" CssClass="input rightclr" AutoPostBack="true" OnSelectedIndexChanged="ddlIssuer_SelectedIndexChanged" meta:resourcekey="ddlIssuerResource1"></asp:DropDownList>

            <asp:Label ID="lblBranch" runat="server" Text="<%$ Resources: CommonLabels, Branch %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlBranch" runat="server" CssClass="input rightclr" OnSelectedIndexChanged="ddlBranch_SelectedIndexChanged" AutoPostBack="true" />

            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlIssuer" ErrorMessage="Issuer Required"
                meta:resourcekey="rfvIssuerResource" CssClass="validation rightclr" ForeColor="Red"></asp:RequiredFieldValidator>

            <asp:DataList ID="dlTerminalList" runat="server"
                HeaderStyle-Font-Bold="true"
                Width="100%"
                OnItemCommand="dlTerminalList_ItemCommand"
                CellPadding="0" ForeColor="#333333"
                meta:resourcekey="dlTerminalListResource1">

                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="White" />

                <HeaderTemplate>
                    <tr style="font-weight: bold;">
                        <td colspan="0">
                            <asp:Label ID="lblTerminalName" runat="server" Text="Terminal Name" meta:resourcekey="lblTerminalNameResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblTerminalModel" runat="server" Text="Terminal Model" meta:resourcekey="lblTerminalNameResource" /></td>

                        <td colspan="0">
                            <asp:Label ID="lblissuer" runat="server" Text="Issuer" meta:resourcekey="lblResourcePathResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblBranch" runat="server" Text="Branch" meta:resourcekey="lblTerminalNameResource" /></td>

                    </tr>
                </HeaderTemplate>

                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />

                <ItemTemplate>
                    <tr class="ItemSelect">
                        <td colspan="0">
                            <asp:Label ID="lblTerminalId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "terminal_id") %>' Visible="False" meta:resourcekey="lblTerminalIdResource1" />
                            <asp:LinkButton ID="lnkTerminalName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "terminal_name") %>' CssClass="ItemSelect" CommandName="Select" meta:resourcekey="lnkTerminalNameResource1" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblterminalmodel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "terminal_model") %>' CommandName="Select" meta:resourcekey="lbltResourcePathResource1" />
                        </td>
                        <td colspan="0">
                            <asp:Label ID="lbltResourcePath" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "issuer_name") %>' CommandName="Select" meta:resourcekey="lbltResourcePathResource1" />
                        </td>
                        <td colspan="0">
                            <asp:Label ID="lblbranchname" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "branch_name") %>' CommandName="Select" meta:resourcekey="lbltResourcePathResource1" />
                        </td>

                    </tr>
                </ItemTemplate>
                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            </asp:DataList>

            <div class="PaginationPanel">
                <asp:LinkButton ID="lnkFirst" runat="server" Text="<%$ Resources:CommonLabels, First %>" OnClick="lnkFirst_Click" meta:resourcekey="lnkFirstResource1"></asp:LinkButton>
                <asp:LinkButton ID="lnkPrev" runat="server" Text="<%$ Resources:CommonLabels, Prev %>" OnClick="lnkPrev_Click" meta:resourcekey="lnkPrevResource1"></asp:LinkButton>
                <asp:Label ID="lblPageIndex" runat="server" meta:resourcekey="lblPageIndexResource1" />
                <asp:LinkButton ID="lnkNext" runat="server" Text="<%$ Resources:CommonLabels, Next %>" OnClick="lnkNext_Click" meta:resourcekey="lnkNextResource1"></asp:LinkButton>
                <asp:LinkButton ID="lnkLast" runat="server" Text="<%$ Resources:CommonLabels, Last %>" OnClick="lnkLast_Click" meta:resourcekey="lnkLastResource1"></asp:LinkButton>
            </div>
        </div>
        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>
        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources: CommonLabels, Back %>" CssClass="button" OnClick="btnBack_Click" Visible="False" />
        </div>
    </div>
</asp:Content>
