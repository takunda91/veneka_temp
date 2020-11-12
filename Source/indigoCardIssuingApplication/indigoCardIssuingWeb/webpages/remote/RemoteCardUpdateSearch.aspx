<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="RemoteCardUpdateSearch.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.remote.RemoteCardUpdateSearch" culture="auto" meta:resourcekey="PageResource1" uiculture="auto" %>
<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
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
                <asp:Label ID="lblSearchExportBatch" runat="server" Text="Export Batch Search" meta:resourcekey="lblSearchExportBatchResource1" />
            </div>

            <asp:Label ID="lblIssuer" Text="<%$ Resources:CommonLabels, Issuer %>" runat="server" CssClass="label leftclr" meta:resourcekey="lblIssuerResource1" />
            <asp:DropDownList ID="ddlIssuer" runat="server" CssClass="input rightclr" OnSelectedIndexChanged="ddlIssuer_SelectedIndexChanged" AutoPostBack="True" meta:resourcekey="ddlIssuerResource1" />

            <asp:Label ID="lblBranch" Text="<%$ Resources:CommonLabels, Branch %>" runat="server" CssClass="label leftclr" meta:resourcekey="lblBranchResource1" />
            <asp:DropDownList ID="ddlBranch" runat="server" CssClass="input rightclr" OnSelectedIndexChanged="ddlBranch_SelectedIndexChanged" AutoPostBack="True" meta:resourcekey="ddlBranchResource1" />

            <asp:Label ID="lblRemoteStatus" runat="server" Text="Remote Card Status" CssClass="label leftclr" meta:resourcekey="lblRemoteStatusResource1" />
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="input rightclr" meta:resourcekey="ddlStatusResource1" />

            <asp:Label ID="lblRemoteAddress" runat="server" Text="Remote Address" CssClass="label leftclr" meta:resourcekey="lblRemoteAddressResource1" />
            <asp:TextBox ID="tbRemoteAddress" runat="server" CssClass="input rightclr" meta:resourcekey="tbRemoteAddressResource1" />

            <asp:Label ID="lblDateFrom" runat="server" Text="Date Between" CssClass="label leftclr" meta:resourcekey="lblDateFromResource" />
            <asp:TextBox ID="tbDateFrom" runat="server" CssClass="input rightclr" meta:resourcekey="tbDateFromResource1" />

            <asp:Label ID="lblDateTo" runat="server" Text="And" CssClass="label leftclr" meta:resourcekey="lblDateToResource" />
            <asp:TextBox ID="tbDateTo" runat="server" CssClass="input rightclr" meta:resourcekey="tbDateToResource1" />
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div class="ButtonPanel" id="pnlButtons" runat="server">
            <asp:Button ID="btnSearch" runat="server" Text="<%$ Resources:CommonLabels, Search %>" OnClick="btnSearch_Click" CssClass="button" meta:resourcekey="btnSearchResource1" />
            <asp:Button ID="btnCancel" runat="server" Text="<%$ Resources:CommonLabels, Clear %>" CssClass="button" OnClick="btnCancel_Click" meta:resourcekey="btnCancelResource1" />
        </div>
    </div>
</asp:Content>
