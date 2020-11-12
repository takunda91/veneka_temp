<%@ Page Title="BranchCardStockMangementReport" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="BranchCardStockMangementReportPage.aspx.cs" Inherits="indigoCardIssuingWeb.Reporting.Branches.BranchCardStockMangementReportPage" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="MainContent">

    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblheading" runat="server" Text="Branch CardStock Management Report" meta:resourcekey="lblheadingResource" />
            </div>

            <table width="60%">
                <tr>
                    <td colspan="8" style="height: 5px;"></td>
                </tr>
                <tr>
                    <td colspan="6">
                        <asp:Label ID="LabelUserDynamicContextOption" runat="server" meta:resourcekey="LabelUserDynamicContextOption" />

                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" />

                    </td>
                    <td>
                        <asp:DropDownList ID="ddlissuerlist" runat="server" AutoPostBack="true" meta:resourcekey="ddlissuerlist"
                            OnSelectedIndexChanged="ddlissuerlist_SelectedIndexChanged" />
                    </td>
                    <td>
                        <asp:Label ID="lblBranch" runat="server" Text="<%$ Resources: CommonLabels, Branch %>" />

                    </td>
                    <td>
                        <asp:DropDownList ID="ddlbranch" runat="server" AutoPostBack="true" meta:resourcekey="ddlbranch" />
                    </td>
                    <td>
                        <asp:Label ID="lblCardProduct" runat="server" Text="<%$ Resources: CommonLabels, Product %>" />

                    </td>
                    <td class="leftclr">
                        <asp:DropDownList ID="ddlProduct" runat="server"   />

                    </td>
                </tr>
                <tr>

                    <td colspan="6" align="center">
                        <asp:Button ID="btnApplySecondLevelFilter" runat="server" Text="Apply" CssClass="button" Style="width: 100px"
                            meta:resourcekey="btnApplyDateRangeResource" OnClick="btnApplySecondLevelFilter_Click" /></td>
                </tr>
            </table>







            <div style="overflow: auto">
                <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt" ShowPrintButton="true"
                    WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" SizeToReportContent="true">
                </rsweb:ReportViewer>
            </div>


            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" />
        </div>
    </div>
</asp:Content>
