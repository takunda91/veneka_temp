<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="InstantPinAuthorisation.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.users.InstantPinAuthorisation" %>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel" id="pnlPinAuthorisation" runat="server">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblCustodianAuthorisation" runat="server" Text="Authorise Card Issue" meta:resourcekey="lblCustodianAuthorisationResource" />
            </div>

            <asp:Label runat="server" ID="lbUsername" Text="Custodian Username"></asp:Label>
            <asp:TextBox runat="server" TextMode="SingleLine" ID="tbCustodianUsername"></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ID="rfvUsername" ControlToValidate="tbCustodianUsername" ErrorMessage="This field is required." CssClass="validation rightclr"></asp:RequiredFieldValidator>

            <asp:Label runat="server" ID="lblCustodianAuthPin" Text="Custodian Authorisation Pin"></asp:Label>
            <asp:TextBox runat="server" TextMode="Password" ID="tbCustodianAuthPin"></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ID="rfvCustodianAuthPin" ControlToValidate="tbCustodianAuthPin" ErrorMessage="This field is required." CssClass="validation rightclr"></asp:RequiredFieldValidator>
        </div>
        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnSubmitPin" runat="server" CssClass="button"
                OnClick="btnSubmitPin_Click" meta:resourcekey="btnSubmitPinResource" Text="Submit" />
        </div>
    </div>
</asp:Content>
