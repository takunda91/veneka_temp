<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="RemoteCardUpdateView.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.remote.RemoteCardUpdateView" culture="auto" meta:resourcekey="PageResource1" uiculture="auto" %>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblRemoteCardUpdateDetails" runat="server" Text="Remote Card Details" meta:resourcekey="lblRemoteCardUpdateDetailsResource1" />
            </div>

            <asp:Label ID="lblPan" runat="server" Text="PAN" CssClass="label leftclr" meta:resourcekey="lblPanResource1" />
            <asp:TextBox ID="tbPAN" runat="server" CssClass="input rightclr" Enabled="False" meta:resourcekey="tbPANResource1" />

            <asp:Label ID="lblCardReference" runat="server" Text="Card Reference" CssClass="label leftclr" meta:resourcekey="lblCardReferenceResource1" />
            <asp:TextBox ID="tbCardReference" runat="server" CssClass="input rightclr" Enabled="False" meta:resourcekey="tbCardReferenceResource1" />

            <asp:Label ID="lblRemoteStatus" runat="server" Text="Remote Status" CssClass="label leftclr" meta:resourcekey="lblRemoteStatusResource1" />
            <asp:TextBox ID="tbRemoteStatus" runat="server" CssClass="input rightclr" Enabled="False" meta:resourcekey="tbRemoteStatusResource1" />

            <asp:Label ID="lblStatusDate" runat="server" Text="Status Date" CssClass="label leftclr" meta:resourcekey="lblStatusDateResource1" />
            <asp:TextBox ID="tbStatusDate" runat="server" CssClass="input rightclr" Enabled="False" meta:resourcekey="tbStatusDateResource1" />

            <asp:Label ID="lblIssuerName" runat="server" Text="Issuer name" CssClass="label leftclr" meta:resourcekey="lblIssuerResource" />
            <asp:TextBox ID="tbIssuerName" runat="server" CssClass="input rightclr" Enabled="False" meta:resourcekey="tbIssuerNameResource1" />

            <asp:Label ID="lblBranchName" runat="server" Text="Branch name" CssClass="label leftclr" meta:resourcekey="lblNumberOfCardsResource" />
            <asp:TextBox ID="tbBranchName" runat="server" CssClass="input rightclr" Enabled="False" meta:resourcekey="tbBranchNameResource1" />

            <asp:Label ID="lblRemoteAddress" runat="server" Text="Remote Address" CssClass="label leftclr" meta:resourcekey="lblNumberOfCardsResource" />
            <asp:TextBox ID="tbRemoteAddress" runat="server" CssClass="input rightclr" Enabled="False" meta:resourcekey="tbRemoteAddressResource1" />

            <asp:Label ID="lblRemoteUpdateTime" runat="server" Text="Remote Update Time" CssClass="label leftclr" meta:resourcekey="lblNumberOfCardsResource" />
            <asp:TextBox ID="tbRemoteUpdateTime" runat="server" CssClass="input rightclr" Enabled="False" meta:resourcekey="tbRemoteUpdateTimeResource1" />

            <asp:Label ID="lblStatusNote" runat="server" Text="Notes" CssClass="label leftclr" meta:resourcekey="lblStatusNoteResource" />
            <asp:TextBox ID="tbStatusNote" runat="server" Enabled="False" MaxLength="150" CssClass="input rightclr" meta:resourcekey="tbStatusNoteResource1" />
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnResend" runat="server" Text="Resend" CssClass="button" OnClick="btnResend_Click" Visible="False" meta:resourcekey="btnResendResource1" />
            <asp:Button ID="btnComplete" runat="server" Text="Complete" CssClass="button" OnClick="btnComplete_Click" Visble="false" meta:resourcekey="btnCompleteResource1" />
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources:CommonLabels, Confirm %>" CssClass="button" OnClick="btnConfirm_Click" Visible="False" meta:resourcekey="btnConfirmResource1" />
            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources:CommonLabels, Back %>" CssClass="button" OnClick="btnBack_OnClick" meta:resourcekey="btnBackResource1" />
        </div>
    </div>
</asp:Content>
