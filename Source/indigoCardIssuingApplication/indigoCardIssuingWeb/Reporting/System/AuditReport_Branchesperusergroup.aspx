<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="AuditReport_Branchesperusergroup.aspx.cs" Inherits="indigoCardIssuingWeb.Reporting.System.AuditReport_Branchesperusergroup" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
 <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Literal ID="lblAuditReportUsergroup" Text="Audit Report – Branches Per User Group" runat="server" meta:resourcekey="lblInventorySummaryReportResource" />
            </div>

            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>

            <asp:Label ID="lblvalue" runat="server" Text="Filter By:" meta:resourcekey="lblvalueResource" />

            <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" />
            <asp:DropDownList ID="ddlIssuer" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlissuerlist_SelectedIndexChanged" />

             <asp:Label ID="lblBranch" runat="server" Text="<%$ Resources: CommonLabels, Branch %>" />
            <asp:DropDownList ID="ddlbranch" runat="server" Width="150px"  OnSelectedIndexChanged="ddlbranch_SelectedIndexChanged" AutoPostBack="true" />
            <br />
            <asp:Label ID="lbluserrole" runat="server" Text="User Role" />
            <asp:DropDownList ID="ddlRole" runat="server" OnSelectedIndexChanged="ddlRole_SelectedIndexChanged"  AutoPostBack="true"/>

            <asp:Label ID="lblUsergroup" runat="server" Text="User Group" />
            <asp:DropDownList ID="ddlusergroup" runat="server" Width="100px" />



            <asp:Button ID="btnApplySecondLevelFilter" runat="server" Text="Apply" CssClass="button" Style="width: 100px"
                meta:resourcekey="btnApplySecondLevelFilterResource" OnClick="btnApplySecondLevelFilter_Click" />

            <div style="overflow: auto">
                <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana"
                    Font-Size="8pt" WaitMessageFont-Names="Verdana" ShowPrintButton="true" AsyncRendering="true"
                    WaitMessageFont-Size="14pt" SizeToReportContent="True">
                 
                </rsweb:ReportViewer>
            </div>

         
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" />
        </div>
    </div>
</asp:Content>
