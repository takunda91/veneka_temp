<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="RenewalLoad.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.renewal.RenewalLoad" %>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <asp:HiddenField ID="hidRenewalId" runat="server" />
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblCardRenewalDetails" runat="server" Text="Upload Renewal File" />
            </div>
            <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlIssuer" runat="server" AutoPostBack="true" CssClass="input rightclr" />

            <asp:Label ID="lblbatchReference" runat="server" Text="Select file" CssClass="label leftclr" meta:resourcekey="lblbatchReferenceResource" />
            <asp:FileUpload ID="fileRenewal" runat="server" CssClass="input rightclr" />

            <asp:Label ID="lblBranchCount" runat="server" Text="Branch count" CssClass="label leftclr" meta:resourcekey="lblBatchCreatedDateResource" />
            <asp:TextBox ID="tbBranchCount" runat="server" CssClass="input rightclr" Enabled="False" />

            <asp:Label ID="lblNumberOfCards" runat="server" Text="Number of cards" CssClass="label leftclr" meta:resourcekey="lblNumberOfCardsResource" />
            <asp:TextBox ID="tbNumberOfCards" runat="server" CssClass="input rightclr" Enabled="False" />
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnUpload" runat="server" Text="Upload" CssClass="button" OnClick="btnUpload_Click"  />
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" CssClass="button" OnClick="btnConfirm_Click" Visible="false" />
            <asp:Button ID="btnReject" runat="server" Text="Reject" CssClass="button" OnClick="btnReject_Click" Visible="false" />
        </div>
    </div>
</asp:Content>
