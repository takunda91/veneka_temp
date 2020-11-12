﻿<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true"
    CodeBehind="IssuedCardsReportPage.aspx.cs" Culture="auto:en-US" meta:resourcekey="PageResource"
    UICulture="auto" Inherits="indigoCardIssuingWeb.Reporting.Branches.IssuedCardsReportPage" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="MainContent">
    <script type="text/javascript">
        $(function () {
            $("#<%= datepickerFrom.ClientID %>").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    maxDate: 1,
                    dateFormat: 'dd/mm/yy',
                    onClose: function (selectedDate) {
                        $("#<%= datepickerTo.ClientID %>").datepicker("option", "minDate", selectedDate);
                    }
                });
                $("#<%= datepickerTo.ClientID %>").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    maxDate: 1,
                    dateFormat: 'dd/mm/yy',
                    onClose: function (selectedDate) {
                        $("#<%= datepickerFrom.ClientID %>").datepicker("option", "maxDate", selectedDate);
                    }
                });

            });
    </script>
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblheading" runat="server" Text="Issued Cards Report" meta:resourcekey="lblheadingResource" />
            </div>
            <table width="60%">
                   <tr>
                    <td colspan="8" style="height: 5px;"></td>
                </tr>
                <tr>
                    <td colspan="8">
                        <asp:Label ID="lbldateheading" runat="server" Text="Select Date Range" meta:resourcekey="lbldateheadingResource" />

                    </td>
                </tr>

             
                <tr>
                    <td>
                        <asp:Label ID="LabelStartDate" runat="server" Text="From " meta:resourcekey="LabelStartDateResource" />

                    </td>
                    <td>
                        <asp:TextBox ID="datepickerFrom" runat="server" />
                    </td>
                    <td>
                        <asp:Label ID="LabelEndDate" runat="server" Text=" To " meta:resourcekey="LabelEndDateResource" />

                    </td>
                    <td>
                        <asp:TextBox ID="datepickerTo" runat="server" />
                    </td>
                     <td>
                        <asp:Label ID="lbllifecycle" runat="server" Text="Card Lifecycle :"  Width="120px" />

                    </td>
                    <td>
                        <asp:DropDownList ID="ddllifecycle" runat="server" AutoPostBack="true" meta:resourcekey="ddllifecycle"
                           >
                            <asp:ListItem  Selected="true" Value="0">Issued Card Life Cycle</asp:ListItem>
                            <asp:ListItem Value="1">Full Life Cycle</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td colspan="2">
                        <asp:Button ID="btnApplyDateRange" runat="server" Text="Apply" OnClick="btnApplyDateRange_Click"
                            meta:resourcekey="btnApplyDateRangeResource" CssClass="button" Style="width: 100px" /></td>
                </tr>
               
                <tr>
                    <td colspan="8">
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
                        <asp:DropDownList ID="ddlbranch" runat="server" meta:resourcekey="ddlbranch" />
                    </td>
                </tr>
               
                <tr>
                    <td>
                        <asp:Label ID="lblUser" runat="server" Text="Operator" meta:resourcekey="lblUserResource" />

                    </td>
                    <td>
                        <asp:DropDownList ID="ddluser" runat="server" meta:resourcekey="ddluser" />

                    </td>
                    <td>
                        <asp:Label ID="lblCardProduct" runat="server" Text="<%$ Resources: CommonLabels, Product %>" />
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlProduct" runat="server" />
                    </td>
                    <td colspan="2">
                        <asp:Button ID="btnApplySecondLevelFilter" runat="server" Text="Apply" CssClass="button" Style="width: 100px"
                            meta:resourcekey="btnApplyDateRangeResource" OnClick="btnApplySecondLevelFilter_Click" />
                    </td>
                </tr>
            </table>

            <div style="overflow: auto">
                <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt" ShowPrintButton="true" ShowExportControls="true"
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