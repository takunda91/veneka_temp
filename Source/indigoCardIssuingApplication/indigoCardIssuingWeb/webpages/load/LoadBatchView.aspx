<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="LoadBatchView.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.load.LoadBatchDetails" Culture="auto:en-US" meta:resourcekey="PageResource" UICulture="auto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblLoadBatchDetails" runat="server" Text="Load Batch Details" meta:resourcekey="lblLoadBatchDetailsResource" />
            </div>

            <asp:Label ID="lblbatchReference" runat="server" Text="Batch reference" CssClass="label leftclr" meta:resourcekey="lblbatchReferenceResource" />
            <asp:TextBox ID="tbBatchReference" runat="server" Enabled="False" CssClass="input rightclr" />

            <asp:Label ID="lblBatchLoadDate" runat="server" Text="Batch load date" CssClass="label leftclr" meta:resourcekey="lblBatchLoadDateResource" />
            <asp:TextBox ID="tbBatchLoadDate" runat="server" Enabled="False" CssClass="input rightclr" />

            <asp:Label ID="lblBatchStatus" runat="server" Text="Batch status" CssClass="label leftclr" meta:resourcekey="lblBatchStatusResource" />
            <asp:TextBox ID="tbBatchStatus" runat="server" Enabled="False" CssClass="input rightclr" />

            <asp:Label ID="lblStatusNote" runat="server" Text="Notes" CssClass="label leftclr" meta:resourcekey="lblStatusNoteResource" />
            <asp:TextBox ID="tbStatusNote" runat="server" Enabled="False" MaxLength="150" CssClass="input rightclr" />

            <asp:Label ID="lblNumberOfCards" runat="server" Text="Number of cards" CssClass="label leftclr" meta:resourcekey="lblNumberOfCardsResource" />
            <asp:TextBox ID="tbNumberOfCards" runat="server" Enabled="False" CssClass="input rightclr" />
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnApproveBatch" runat="server" Text="Approve Batch" CssClass="button"
                OnClick="btnApproveBatch_Click" meta:resourcekey="btnApproveBatchResource" />

            <asp:Button ID="btnRejectBatch" runat="server" Text="Reject Batch" CssClass="button"
                OnClick="btnRejectBatch_Click" meta:resourcekey="btnRejectBatchResource" />

            <asp:Button ID="btnConfirm" runat="server" Text="Confirm" CssClass="button"
                OnClick="btnConfirm_Click" meta:resourcekey="btnConfirmResource" />

            <asp:Button ID="btnPDFReport" runat="server" Text="Generate Report" CssClass="button"
                OnClick="btnPDFReport_Click" meta:resourcekey="btnPDFReportResource" />

            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources: CommonLabels, Back %>" OnClick="btnBack_OnClick" CssClass="button" />
        </div>
    </div>
</asp:Content>
