<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" EnableEventValidation="false"
    CodeBehind="InventoryReportPage.aspx.cs" Inherits="indigoCardIssuingWeb.Reporting.CardCenter.InventoryReportPage" culture="auto:en-US" meta:resourcekey="PageResource" uiculture="auto"  %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Literal ID="lblInventorySummaryReport" Text="Inventory Summary Report" runat="server" meta:resourcekey="lblInventorySummaryReportResource" />
            </div>  

         
    
         <table width="60%">
             
                <tr>
                    <td colspan="8" style="height: 5px;"></td>
                </tr>
               
                <tr>
                    <td colspan="8">
                          <asp:Label ID="lblvalue" runat="server" Text="Filter By:" meta:resourcekey="lblvalueResource" />
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
                    <td colspan="2">
                        <asp:DropDownList ID="ddlbranch" runat="server" meta:resourcekey="ddlbranch" />
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
              <div style="overflow:auto">
                <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt" ShowPrintButton="true"
                    WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" SizeToReportContent="true">
                  
                </rsweb:ReportViewer>
            </div>

          <asp:ScriptManager ID="ScriptManager2" runat="server">
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
