<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="ApproveSingle.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.renewal.ApproveSingle" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblCardAndCustomerDetails" runat="server" Text="Card Renewal Details" meta:resourcekey="lblCardAndCustomerDetailsResource" />
            </div>
            <asp:HiddenField ID="currencyId" runat="server" />
            <asp:HiddenField ID="cbsAccountTypeId" runat="server" />
            <asp:HiddenField ID="cmsAccountTypeId" runat="server" />

            <asp:Label ID="Label14" runat="server" Text="Branch" CssClass="label leftclr" />
            <asp:TextBox ID="txtBranch" runat="server" CssClass="input" ReadOnly="true" />

            <asp:Label ID="lblBranch" runat="server" Text="Delivery Branch" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlBranch" runat="server" AutoPostBack="true" CssClass="input rightclr" />

            <asp:Label ID="lblCardProduct" runat="server" Text="Product" CssClass="label leftclr" />
            <asp:TextBox ID="txtProduct" runat="server" CssClass="input" ReadOnly="true" />

            <asp:Label ID="Label10" runat="server" Text="Customer Name" CssClass="label leftclr" />
            <asp:TextBox ID="txtCustomerName" runat="server" CssClass="input" ReadOnly="true" />

            <asp:Label ID="Label11" runat="server" Text="Email Address" CssClass="label leftclr" />
            <asp:TextBox ID="txtEmailAddress" runat="server" CssClass="input" ReadOnly="true" />

            <asp:Label ID="Label12" runat="server" Text="Mobile Number" CssClass="label leftclr" />
            <asp:TextBox ID="txtMobileNumber" runat="server" CssClass="input" ReadOnly="true" />

            <asp:Label ID="Label1" runat="server" Text="Card Number" CssClass="label leftclr" />
            <asp:TextBox ID="txtCardNumber" runat="server" CssClass="input" ReadOnly="true" />

            <asp:Label ID="Label2" runat="server" Text="Expiry Date" CssClass="label leftclr" />
            <asp:TextBox ID="txtExpiryDate" runat="server" CssClass="input" ReadOnly="true" />

            <asp:Label ID="Label3" runat="server" Text="Renewal Date" CssClass="label leftclr" />
            <asp:TextBox ID="txtRenewalDate" runat="server" CssClass="input" ReadOnly="true" />

            <asp:Label ID="Label4" runat="server" Text="Currency Code" CssClass="label leftclr" />
            <asp:TextBox ID="txtCurrencyCode" runat="server" CssClass="input" ReadOnly="true" />

            <asp:Label ID="Label5" runat="server" Text="Limit Balance" CssClass="label leftclr" />
            <asp:TextBox ID="txtLimitBalance" runat="server" CssClass="input" ReadOnly="true" />

            <asp:Label ID="Label6" runat="server" Text="Embossing Name" CssClass="label leftclr" />
            <asp:TextBox ID="txtEmbossingName" runat="server" CssClass="input" ReadOnly="true" />

            <asp:Label ID="Label7" runat="server" Text="Identity/Passport Number" CssClass="label leftclr" />
            <asp:TextBox ID="txtClientID" runat="server" CssClass="input" ReadOnly="true" />

            <asp:Label ID="Label8" runat="server" Text="Internal Account" CssClass="label leftclr" />
            <asp:TextBox ID="txtInternalAccountNumber" runat="server" CssClass="input" ReadOnly="true" />

            <asp:Label ID="Label9" runat="server" Text="External Account" CssClass="label leftclr" />
            <asp:TextBox ID="txtExternalAccountNumber" runat="server" CssClass="input" ReadOnly="true" />

            <asp:Label ID="Label13" runat="server" Text="Renewal Status" CssClass="label leftclr" />
            <asp:TextBox ID="txtRenewalStatus" runat="server" CssClass="input" ReadOnly="true" />

            <asp:Label ID="lblComment" runat="server" Text="Comment" CssClass="label leftclr"></asp:Label>
            <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" CssClass="input" Height="50px"></asp:TextBox>
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server" visible="true">
            <asp:Button ID="btnConfirm" runat="server" Text="Confirm" CssClass="button" Visible="false" OnClick="btnConfirm_Click" />
            <asp:Button ID="btnReject" runat="server" Text="Reject" CssClass="button" Visible="true" OnClick="btnReject_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Back" CssClass="button" CausesValidation="false" Visible="true" OnClick="btnCancel_Click" />
        </div>
    </div>

</asp:Content>
