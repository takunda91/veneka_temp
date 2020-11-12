<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="ExportBatchView.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.export.ExportBatchView" %>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblExportBatchDetails" runat="server" Text="Export Batch Details" />
            </div>

            <asp:Label ID="lblbatchReference" runat="server" Text="Batch reference" CssClass="label leftclr" meta:resourcekey="lblbatchReferenceResource" />
            <asp:TextBox ID="tbBatchReference" runat="server" CssClass="input rightclr" Enabled="False" />

            <asp:Label ID="lblBatchCreatedDate" runat="server" Text="Batch created date" CssClass="label leftclr" meta:resourcekey="lblBatchCreatedDateResource" />
            <asp:TextBox ID="tbBatchCreatedDate" runat="server" CssClass="input rightclr" Enabled="False" />

            <asp:Label ID="lblBatchStatus" runat="server" Text="Batch status" CssClass="label leftclr" meta:resourcekey="lblBatchStatusResource" />
            <asp:TextBox ID="tbBatchStatus" runat="server" CssClass="input rightclr" Enabled="False" />

            <asp:Label ID="lblIssuerName" runat="server" Text="Issuer name" CssClass="label leftclr" meta:resourcekey="lblIssuerResource" />
            <asp:TextBox ID="tbIssuerName" runat="server" CssClass="input rightclr" Enabled="False" />

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
            <asp:Button ID="btnRequestExport" runat="server" Text="Request Export" CssClass="button" OnClick="btnRequestExport_Click" Visible="false" />
            <asp:Button ID="btnApprove" runat="server" Text="Approve" CssClass="button" OnClick="btnApprove_Click" Visble="false" />
            <asp:Button ID="btnReject" runat="server" Text="Reject" CssClass="button" OnClick="btnReject_Click" Visible="false" />

            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" CssClass="button"
                OnClick="btnConfirm_Click" Visible="false" />

            <asp:Button ID="btnPDFReport" runat="server" Text="Generate Report" CssClass="button"
                OnClick="btnPDFReport_Click" meta:resourcekey="btnPDFReportResource" />

            <asp:Button ID="btnDisplayCards" runat="server" Text="View Cards" CssClass="button" OnClick="btnDisplayCards_Click" />

            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources: CommonLabels, Back %>" CssClass="button" OnClick="btnBack_OnClick" />
        </div>
    </div>
</asp:Content>
