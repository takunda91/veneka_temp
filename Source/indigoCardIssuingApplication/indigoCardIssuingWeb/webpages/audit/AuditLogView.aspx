<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true"
    CodeBehind="AuditLogView.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.audit.AuditLogView" UICulture="auto" meta:resourcekey="PageResource" Culture="auto:en-US" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader" style="align: center">
                <asp:Literal ID="lblAuditLogResults" runat="server" meta:resourcekey="lblAuditLogResultsResource" />
            </div>
            <div>
                <asp:DataList ID="dlAuditList" runat="server" HeaderStyle-Font-Bold="true" Width="100%"
                    CellPadding="0" ForeColor="#333333">
                    <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="White" />
                    <HeaderTemplate>
                        <tr style="font-weight: bold;">
                            <td colspan="0">
                                <asp:Label ID="lblhdAuditDate" runat="server" Text="Date" meta:resourcekey="lblhdAuditDateResource" />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblhdUserName" runat="server" Text="User Name" meta:resourcekey="lblhdUserNameResource" />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblhWorkStationAddress" runat="server" Text="Work Station" meta:resourcekey="lblhWorkStationAddressResource" />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblhAction" runat="server" Text="User Action" meta:resourcekey="lblhActionResource" />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblhDescription" runat="server" Text="Description" meta:resourcekey="lblhDescriptionResource" />
                            </td>
                        </tr>
                    </HeaderTemplate>
                    <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                    <ItemTemplate>
                        <tr class="ItemSelect">
                            <td colspan="0">
                                <asp:Label ID="lblAuditDate" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "audit_date") %>' />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblUserName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "UserName") %>' />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblWorkstation" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "workstation_address") %>' />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblUserAction" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "audit_action_name") %>' />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblActionDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "action_description") %>' />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                </asp:DataList>
            </div>

            <div class="PaginationPanel" id="pnlpage" runat="server">
                <asp:LinkButton ID="lnkFirst" runat="server" Text="<%$ Resources: CommonLabels, First %>" OnClick="lnkFirst_Click"> </asp:LinkButton>
                <asp:LinkButton ID="lnkPrev" runat="server" Text="<%$ Resources: CommonLabels, Prev %>" OnClick="lnkPrev_Click"> </asp:LinkButton>
                <asp:Label ID="lblPageIndex" runat="server" />
                <asp:LinkButton ID="lnkNext" runat="server" Text="<%$ Resources: CommonLabels, Next %>" OnClick="lnkNext_Click"> </asp:LinkButton>
                <asp:LinkButton ID="lnkLast" runat="server" Text="<%$ Resources: CommonLabels, Last %>" OnClick="lnkLast_Click"> </asp:LinkButton>
            </div>
        </div>
    </div>

    <div class="InfoPanel">
        <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
    </div>
    <div class="ErrorPanel">
        <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
    </div>

    <div id="pnlButtons" class="ButtonPanel" runat="server">
        <asp:Button ID="btnExportToCSV" runat="server" Text="Export To CSV" OnClick="btnExportToCSV_Click" CssClass="button" meta:resourcekey="btnExportToCSVResource" />
        <asp:Button ID="btnExportToPdf" runat="server" Text="Export To PDF" OnClick="btnExportToPdf_Click" CssClass="button" meta:resourcekey="btnExportToPdfResource" />
        <asp:Button ID="btnBack" runat="server" Text="<%$ Resources: CommonLabels, Back %>" CssClass="button" OnClick="btnBack_Click"
            CausesValidation="False" />
    </div>
</asp:Content>
