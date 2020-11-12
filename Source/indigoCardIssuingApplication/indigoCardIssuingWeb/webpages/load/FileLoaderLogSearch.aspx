<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true"
    CodeBehind="FileLoaderLogSearch.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.load.FileLoaderLogSearch"
    UICulture="auto" Culture="auto:en-US" meta:resourcekey="PageResource" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
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
                <asp:Label ID="lblFileLoaderHistory" meta:resourcekey="lblFileLoaderHistoryResource" runat="server" />
            </div>

            <asp:Label ID="lblIssuer" runat="server" CssClass="label leftclr" Text="<%$ Resources: CommonLabels, Issuer %>" />
            <asp:DropDownList ID="ddlIssuer" runat="server" AutoPostBack="true" CssClass="input rightclr" />

            <asp:Label ID="lblKeyWord" runat="server" meta:resourcekey="lblKeyWordResource" CssClass="label leftclr" />
            <asp:TextBox ID="tbKeyWord" runat="server" CssClass="input rightclr" />

            <asp:Label ID="lblDateFrom" runat="server" meta:resourcekey="lblDateLoadedResource" CssClass="label leftclr" />
            <asp:TextBox ID="tbDateFrom" runat="server" CssClass="input " />
            <asp:RequiredFieldValidator ID="rfvdatefrom" runat="server" ControlToValidate="tbDateFrom" CssClass="validation rightclr"
                ErrorMessage="Date Required." ForeColor="Red" meta:resourcekey="rfvdatefromResource" />

            <asp:Label ID="lblDateTo" runat="server" meta:resourcekey="lblDateToResource" CssClass="label leftclr" />
            <asp:TextBox ID="tbDateTo" runat="server" CssClass="input " />
            <asp:RequiredFieldValidator ID="rfvdateto" runat="server" ControlToValidate="tbDateTo" CssClass="validation rightclr"
                ErrorMessage="Date Required." meta:resourcekey="rfvdatetoResource" />
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" CssClass="button" Text="<%$ Resources: CommonLabels, Search %>" />
        </div>
    </div>
</asp:Content>
