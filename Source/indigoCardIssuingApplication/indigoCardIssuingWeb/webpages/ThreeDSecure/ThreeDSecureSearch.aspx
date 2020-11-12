<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="ThreeDSecureSearch.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.ThreeDSecure.ThreeDSecureSearch" culture="auto" meta:resourcekey="PageResource1" uiculture="auto" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
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
                dateFormat: '<%=DATEPICKER_FORMAT%>',//'dd-mm-yy',
                onClose: function (selectedDate) {
                    $("#<%= tbDateFrom.ClientID %>").datepicker("option", "maxDate", selectedDate);
                }
            });
        });
    </script>
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblThreeDSecureSearch" runat="server" Text="Search ThreeDSecure Load Batch" meta:resourcekey="lblThreeDSecureResource" />
            </div>

            <asp:Label ID="lblIssuer" runat="server" CssClass="label leftclr" Text="<%$ Resources:CommonLabels, Issuer %>" meta:resourcekey="lblIssuerResource1" />
            <asp:DropDownList ID="ddlIssuer" runat="server" CssClass="input rightclr" meta:resourcekey="ddlIssuerResource1" />

            <asp:Label ID="lblbatchReference" runat="server" Text="Batch reference:" CssClass="label leftclr" meta:resourcekey="lblbatchReferenceResource" />
            <asp:TextBox ID="tbBatchReference" runat="server" CssClass="input rightclr" meta:resourcekey="tbBatchReferenceResource1" />

            <asp:Panel ID="pnlDate" runat="server" Visible="False" CssClass="bothclr" meta:resourcekey="pnlDateResource1">
                <asp:Calendar ID="clCalenderDate" runat="server" meta:resourcekey="clCalenderDateResource1" />
            </asp:Panel>

            <asp:Label ID="lblBatchStatus" runat="server" Text="Batch status:" CssClass="label leftclr" meta:resourcekey="lblCardNumberResource" />
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="input rightclr" meta:resourcekey="ddlStatusResource1" />

            <asp:Label ID="lblDateFrom" runat="server" Text="Batch loaded between:" CssClass="label leftclr" meta:resourcekey="lblDateFromResource" />
            <asp:TextBox ID="tbDateFrom" runat="server" CssClass="input rightclr" meta:resourcekey="tbDateFromResource1" />

            <asp:Label ID="lblDateTo" runat="server" Text="and:" CssClass="label leftclr" meta:resourcekey="lblDateToResource" />
            <asp:TextBox ID="tbDateTo" runat="server" CssClass="input rightclr" meta:resourcekey="tbDateToResource1" />
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div class="ButtonPanel" id="pnlButtons" runat="server">
            <asp:Button ID="btnSearch" runat="server" Text="<%$ Resources:CommonLabels, Search %>" CssClass="button"
                OnClick="btnSearch_Click" meta:resourcekey="btnSearchResource1" />
        </div>
    </div>
</asp:Content>

