<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true"
    CodeBehind="DistBatchView.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.dist.DistBatchView" Culture="auto:en-US" meta:resourcekey="PageResource" UICulture="auto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lbldistBatchDetails" runat="server" Text="Distribution Batch Details" meta:resourcekey="lbldistBatchDetailsResource" />
            </div>

            <asp:Label ID="lblbatchReference" runat="server" Text="Batch reference" CssClass="label leftclr" meta:resourcekey="lblbatchReferenceResource" />
            <asp:TextBox ID="tbBatchReference" runat="server" CssClass="input rightclr" Enabled="False" />

            <asp:Label ID="lblIssueMethod" runat="server" Text="Issue Method" CssClass="label leftclr" />
            <asp:TextBox ID="tbIssueMethod" runat="server" CssClass="input rightclr" Enabled="False" />

            <asp:Label ID="lblBatchCreatedDate" runat="server" Text="Batch created date" CssClass="label leftclr" meta:resourcekey="lblBatchCreatedDateResource" />
            <asp:TextBox ID="tbBatchCreatedDate" runat="server" CssClass="input rightclr" Enabled="False" />

            <asp:Label ID="lblCreatedBy" runat="server" Text="Created By" CssClass="label leftclr" />
            <asp:TextBox ID="tbCreatedBy" runat="server" CssClass="input rightclr" Enabled="False" />

            <asp:Label ID="lblBatchStatus" runat="server" Text="Batch status" CssClass="label leftclr" meta:resourcekey="lblBatchStatusResource" />
            <asp:TextBox ID="tbBatchStatus" runat="server" CssClass="input rightclr" Enabled="False" />

            <asp:Label ID="lblIssuerName" runat="server" Text="Issuer name" CssClass="label leftclr" meta:resourcekey="lblIssuerResource" />
            <asp:TextBox ID="tbIssuerName" runat="server" CssClass="input rightclr" Enabled="False" />

            <asp:Label ID="lblBranchName" runat="server" Text="Branch name" CssClass="label leftclr" meta:resourcekey="lblBranchNameResource" />
            <asp:TextBox ID="tbBranchName" runat="server" CssClass="input rightclr" Enabled="False" />

            <asp:Label ID="lblProduct" runat="server" CssClass="label leftclr" Text="Product Type" meta:resourcekey="lblProductResource" />
            <asp:TextBox ID="tbProductType" runat="server" CssClass="input rightclr" Enabled="False" />

            <asp:Label ID="lblNumberOfCards" runat="server" Text="Number of cards" CssClass="label leftclr" meta:resourcekey="lblNumberOfCardsResource" />
            <asp:TextBox ID="tbNumberOfCards" runat="server" CssClass="input rightclr" Enabled="False" />

            <asp:Label ID="lblStatusNote" runat="server" Text="Notes" CssClass="label leftclr" meta:resourcekey="lblStatusNoteResource" />
            <asp:TextBox ID="tbStatusNote" runat="server" Enabled="False" MaxLength="150" CssClass="input rightclr" />
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnOther" runat="server" CssClass="button" OnClick="btnOther_Click" Visible="false" />
            <asp:Button ID="btnStatus" runat="server" CssClass="button" OnClick="btnStatus_Click" />
            <asp:Button ID="btnReject" runat="server" CssClass="button" OnClick="btnReject_Click" Visible="false" />

            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" CssClass="button"
                OnClick="btnConfirm_Click" Visible="false" />

            <asp:Button ID="btnPDFReport" runat="server" Text="Generate Report" CssClass="button"
                OnClick="btnPDFReport_Click" OnClientClick="timedRefresh(2000)" meta:resourcekey="btnPDFReportResource" />

            <asp:Button ID="btnCanel" runat="server" Text="Cancel" CssClass="button" OnClick="btnCanel_Click" />

            <asp:Button ID="btnDisplayCards" runat="server" Text="View Cards" CssClass="button" OnClick="btnDisplayCards_Click" />

            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources: CommonLabels, Back %>" CssClass="button" OnClick="btnBack_OnClick" />
        </div>
    </div>
</asp:Content>
