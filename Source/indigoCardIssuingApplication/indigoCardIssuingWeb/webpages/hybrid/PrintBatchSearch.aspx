﻿<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="PrintBatchSearch.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.hybrid.PrintBatchSearch" %>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
     <script type="text/javascript">
         $(function () {
             $("#<%= tbDateFrom.ClientID %>").datepicker({
                changeMonth: true,
                changeYear: true,
                maxDate: 1,
                dateFormat: '<%=DATEPICKER_FORMAT%>',
                onClose: function (selectedDate) {
                    $("#<%= tbDateTo.ClientID %>").datepicker("option", "minDate", selectedDate);
                }
            });
            $("#<%= tbDateTo.ClientID %>").datepicker({
                changeMonth: true,
                changeYear: true,
                maxDate: 1,
                dateFormat: '<%=DATEPICKER_FORMAT%>',
                onClose: function (selectedDate) {
                    $("#<%= tbDateFrom.ClientID %>").datepicker("option", "maxDate", selectedDate);
                }
            });
        });
    </script>

    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblSearchPrintBatch" runat="server" Text="Print Batch Search" />
            </div>  
        
            <asp:Label ID="lblIssuer" Text="<%$ Resources: CommonLabels, Issuer %>"  runat="server" CssClass="label leftclr"  />
            <asp:DropDownList ID="ddlIssuer" runat="server" CssClass="input rightclr"   AutoPostBack="true"/>
           
            <asp:Label ID="lblBranchAllocated" Text="Branch Allocated" runat="server" CssClass="label leftclr" meta:resourcekey="lblBranchAllocatedResource"  Visible="false" />
            <asp:DropDownList ID="ddlBranch" runat="server" CssClass="input rightclr"  Visible="false"/> 

            <asp:Label ID="lblIssueMethod" Text="Issue Method" runat="server" CssClass="label leftclr"  />
            <asp:DropDownList ID="ddlIssueMethod" runat="server" CssClass="input rightclr"  /> 
                  
       
            <asp:Label ID="lblbatchReference" runat="server" Text="Batch Reference" CssClass="label leftclr" meta:resourcekey="lblbatchReferenceResource" />
            <asp:TextBox ID="tbBatchReference" runat="server" CssClass="input rightclr"  />
            
            <asp:Label ID="lblBatchStatus" runat="server" Text="Batch Status" CssClass="label leftclr" meta:resourcekey="lblbranchstatusResource" />
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="input rightclr"  />

            <asp:Label ID="lblDateFrom" runat="server" Text="Date Between" CssClass="label leftclr" meta:resourcekey="lblDateFromResource" />
            <asp:TextBox ID="tbDateFrom" runat="server" CssClass="input rightclr"  />

            <asp:Label ID="lblDateTo" runat="server" Text="And" CssClass="label leftclr" meta:resourcekey="lblDateToResource" />
            <asp:TextBox ID="tbDateTo" runat="server" CssClass="input rightclr" />
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource"/>
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource"/>
        </div>

        <div class="ButtonPanel" id="pnlButtons" runat="server">
            <asp:Button ID="btnSearch" runat="server" Text="<%$ Resources: CommonLabels, Search %>"  OnClick="btnSearch_Click"
                        CssClass="button" />

            <asp:Button ID="btnCancel" runat="server" Text="<%$ Resources: CommonLabels, Clear %>"  CssClass="button"  
                        OnClick="btnCancel_Click" />
        </div>
    </div>
</asp:Content>