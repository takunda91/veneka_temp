<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="ThreeDSecureView.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.ThreeDSecure.ThreeDSecureView" culture="auto" meta:resourcekey="PageResource1" uiculture="auto" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
     <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblThreeDSecureBatchDetails" runat="server" Text="ThreeDSecure Batch Details" meta:resourcekey="lblThreeDSecureBatchResource" />
            </div>

            <asp:Label ID="lblbatchReference" runat="server" Text="Batch reference" CssClass="label leftclr" meta:resourcekey="lblbatchReferenceResource" />
            <asp:TextBox ID="tbBatchReference" runat="server" Enabled="False" CssClass="input rightclr" meta:resourcekey="tbBatchReferenceResource1" />

            <asp:Label ID="lblBatchLoadDate" runat="server" Text="Batch load date" CssClass="label leftclr" meta:resourcekey="lblBatchLoadDateResource" />
            <asp:TextBox ID="tbBatchLoadDate" runat="server" Enabled="False" CssClass="input rightclr" meta:resourcekey="tbBatchLoadDateResource1" />

            <asp:Label ID="lblBatchStatus" runat="server" Text="Batch status" CssClass="label leftclr" meta:resourcekey="lblBatchStatusResource" />
            <asp:TextBox ID="tbBatchStatus" runat="server" Enabled="False" CssClass="input rightclr" meta:resourcekey="tbBatchStatusResource1" />

            <asp:Label ID="lblStatusNote" runat="server" Text="Notes" CssClass="label leftclr" meta:resourcekey="lblStatusNoteResource" />
            <asp:TextBox ID="tbStatusNote" runat="server" Enabled="False" MaxLength="150" CssClass="input rightclr" meta:resourcekey="tbStatusNoteResource1" />

            <asp:Label ID="lblNumberOfCards" runat="server" Text="Number of cards" CssClass="label leftclr" meta:resourcekey="lblNumberOfCardsResource" />
            <asp:TextBox ID="tbNumberOfCards" runat="server" Enabled="False" CssClass="input rightclr" meta:resourcekey="tbNumberOfCardsResource1" />
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnRecreateBatch" runat="server" Text="Re-Create" CssClass="button"
                OnClick="btnReCreate_Click" meta:resourcekey="btnApproveBatchResource" />

            <asp:Button ID="btnConfirm" runat="server" Text="Confirm" CssClass="button"
                OnClick="btnConfirm_Click" meta:resourcekey="btnConfirmResource" />

      <asp:Button ID="btnDisplayCards" runat="server" Text="View Cards" CssClass="button" meta:resourcekey="btnDisplayCardsResource1"  Onclick="btnDisplayCards_Click" />

            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources:CommonLabels, Back %>" OnClick="btnBack_OnClick" CssClass="button" meta:resourcekey="btnBackResource1" />
        </div>
    </div>
</asp:Content>

