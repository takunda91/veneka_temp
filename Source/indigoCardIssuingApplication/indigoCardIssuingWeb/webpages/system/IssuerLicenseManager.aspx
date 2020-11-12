<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="IssuerLicenseManager.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.system.IssuerLicenseManager" culture="auto:en-US" meta:resourcekey="PageResource" uiculture="auto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblIssuerLicenseHeader" runat="server" Text="Issuer Licence Details" meta:resourcekey="lblIssuerLicenseHeaderResource"/>           
            </div>

            <asp:Label ID="lblMachineId" runat="server" Text="Machine ID" CssClass="label leftclr" meta:resourcekey="lblMachineIdResource" />
            <asp:TextBox ID="tbMachineId" runat="server" ReadOnly="true" CssClass="input rightclr" />
            
            <asp:Label ID="lblUploadLicense" runat="server" Text="Upload  License" CssClass="label leftclr" meta:resourcekey="lblUploadLicenseResource"/>
            <asp:FileUpload ID="fuUploadLicense" runat="server" CssClass="input rightclr"  />
        

            <asp:Panel ID="pnlLicenseInfo" runat="server" GroupingText="License Information" CssClass="bothclr" meta:resourcekey="pnlLicenseInfoResource">
                <asp:Label ID="lblIssuerName" runat="server" Text="Issuer Name" CssClass="label leftclr"   meta:resourcekey="lblIssuerNameResource"/>
                <asp:TextBox ID="tbIssuerName" runat="server" Enabled="false" CssClass="input rightclr" />

                <asp:Label ID="lblIssuerCode" runat="server" Text="Issuer Code" CssClass="label leftclr" meta:resourcekey="lblIssuerCodeResource"/>
                <asp:TextBox ID="tbIssuerCode" runat="server" Enabled="false" CssClass="input rightclr" />

                <asp:Label ID="lblIssueDate" runat="server" Text="Issue Date" CssClass="label leftclr"  meta:resourcekey="lblIssueDateResource"/>
                <asp:TextBox ID="tbIssueDate" runat="server" Enabled="false" CssClass="input rightclr" />

                <asp:Label ID="lblExpiryDate" runat="server" Text="Expiry Date" CssClass="label leftclr" meta:resourcekey="lblExpiryDateResource"/>
                <asp:TextBox ID="tbExpiryDate" runat="server" Enabled="false" CssClass="input rightclr" />

                <asp:Label ID="lblLicensedBins" runat="server" Text="BIN Codes" CssClass="label leftclr" meta:resourcekey="lblLicensedBinsResource"/>
                <asp:ListBox ID="lbLicensedBins" runat="server" CssClass="input rightclr" />
            </asp:Panel>
        </div>


        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource"/>
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnUpdate" runat="server" Text="Upload License" CssClass="button" OnClick="btnUpdate_Click"  meta:resourcekey="btnUpdateResource" />            
        </div>
    </div>
</asp:Content>
