<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="BillingReport.aspx.cs" Inherits="indigoCardIssuingWeb.Reporting.CardCenter.BillingReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Literal ID="lblBillingCardReport" Text="Billing Card Report" runat="server"  meta:resourcekey="lblBillingCardReport" />
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
                        <asp:Label ID="LabelStartDate" runat="server" Text="Month " meta:resourcekey="LabelStartDateResource" />

                    </td>
                    <td>
                        <asp:DropDownList ID="ddlMonth" runat="server" meta:resourcekey="ddlbranch" />
                    </td>
                    <td>
                        <asp:Label ID="LabelEndDate" runat="server" Text="Year " meta:resourcekey="LabelEndDateResource" />

                    </td>
                    <td>
                         <asp:DropDownList ID="ddlyear" runat="server" meta:resourcekey="ddlbranch" />
                    </td>
                    <td>
                        <asp:Button ID="btnGenerate" runat="server" Text="Generate" OnClick="btnGenerate_Click"
                            meta:resourcekey="btnApplyDateRangeResource" CssClass="button" Style="width: 100px" /></td>
                </tr>

            </table>

        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" />
        </div>
    </div>

   
</asp:Content>

