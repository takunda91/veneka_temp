<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true"
    CodeBehind="AuditLogSelectionForm.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.audit.AuditLogSelectionForm"
    UICulture="auto" meta:resourcekey="PageResource" Culture="auto:en-US" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $(function () {
                $("#<%= txbDateFrom.ClientID %>").datepicker({
                    dateFormat: 'dd/mm/yy',
                    changeMonth: true,
                    changeYear: true,
                    maxDate: 1,
                    onClose: function (selectedDate) {
                        $("#<%= txbDateTo.ClientID %>").datepicker("option", "minDate", selectedDate);
                    }
                });
                $("#<%= txbDateTo.ClientID %>").datepicker({
                    dateFormat: 'dd/mm/yy',                    
                    changeMonth: true,
                    changeYear: true,
                    maxDate: 1,
                    onClose: function (selectedDate) {
                        $("#<%= txbDateFrom.ClientID %>").datepicker("option", "maxDate", selectedDate);
                    }
                });
            });
        });
    </script>
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Literal ID="lblUserAuditLogs" Text="User Audit Logs" runat="server" meta:resourcekey="lblUserAuditLogsResource" />
            </div>
            
            <asp:Label ID="UserNameSearch" runat="server"  CssClass="label leftclr" meta:resourcekey="lblUserNameSearchResource" />
            <asp:TextBox ID="tbUsernameSearch" runat="server" MaxLength="30" CssClass="input rightclr" />
            <asp:Button ID="btnSearchUser" runat="server" CssClass="button  rightclr" OnClick="btnSearchUser_Click" meta:resourcekey="btnSearchUserResource" />
            <asp:Label ID="UserName" runat="server" Text="User Name" CssClass="label leftclr"
                meta:resourcekey="UserNameResource" />
            <asp:TextBox ID="tbUsername" runat="server" CssClass="input rightclr" Enabled="false"  />
              <asp:Label ID="lblIssuer" runat="server" meta:resourcekey="lblIssuerResource" CssClass="label leftclr"
                Text="Issuer" />
            <asp:DropDownList ID="ddlIssuer" runat="server"  CssClass="input rightclr" />
            <asp:Label ID="lblUserAction" runat="server" Text="User Action" CssClass="label leftclr"
                meta:resourcekey="lblUserActionResource" />
            <asp:DropDownList ID="ddlUserAction" runat="server" CssClass="input rightclr" />
          
            <asp:Label ID="lblRoles" runat="server" Text="Roles" CssClass="label leftclr"  meta:resourcekey="lblRolesResource"/>
            <asp:DropDownList ID="ddlRoles" runat="server" CssClass="input rightclr" />
            <asp:Label ID="lblDateFrom" runat="server" Text="Date From" CssClass="label leftclr"
                meta:resourcekey="lblDateFromResource" />
            <asp:TextBox ID="txbDateFrom" runat="server" CssClass="input " />
            <asp:RequiredFieldValidator ID="rfvdatefrom" runat="server" ControlToValidate="txbDateFrom"
                CssClass="validation rightclr" ErrorMessage="Date From Required." meta:resourcekey="rfvdatefromResource" />
            <asp:Label ID="lblDateTo" runat="server" Text="Date To" CssClass="label leftclr"
                meta:resourcekey="lblDateToResource" />
            <asp:TextBox ID="txbDateTo" runat="server" 
                CssClass="input rightclr" />
            <asp:RequiredFieldValidator ID="rfvdateto" runat="server" ControlToValidate="txbDateTo"
                ErrorMessage="Date To Required." meta:resourcekey="rfvdatetoResource" CssClass="validation rightclr" />
        </div>
        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource"  />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource"  />
        </div>
        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnGenerateReport" runat="server" OnClick="btnGenerateReport_Click"
                CssClass="button" meta:resourcekey="btnGenerateReportResource" />
        </div>
    </div>
</asp:Content>
