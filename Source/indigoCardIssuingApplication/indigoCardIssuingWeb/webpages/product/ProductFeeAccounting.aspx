<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="ProductFeeAccounting.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.product.ProductFeeAccounting" %>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblProductscreenheading" runat="server" Text="Fee Accounting Admin" />
            </div>

            <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlIssuer" runat="server" CssClass="input rightclr" />

            <asp:Label ID="lblAccountingName" runat="server" Text="Name" CssClass="label leftclr" />
            <asp:TextBox ID="tbAccountingName" runat="server" CssClass="input" MaxLength="100" />
            <asp:RequiredFieldValidator ID="rfvAccountingName" runat="server" ControlToValidate="tbAccountingName"
                ErrorMessage="Name Required." ForeColor="Red" CssClass="validation rightclr" />

            <asp:Panel ID="pnlFeeRevenue" runat="server" GroupingText="Fee Revenue Account Details" CssClass="bothclr">
                <asp:Label ID="lblRevenueAccountNo" runat="server" Text="Account Number" CssClass="label leftclr" />
                <asp:TextBox ID="tbRevenueAccountNo" runat="server" CssClass="input" MaxLength="100" />

                <asp:Label ID="lblRevenueBranchCode" runat="server" Text="Branch Code" CssClass="label leftclr" />
                <asp:TextBox ID="tbRevenueBranchCode" runat="server" CssClass="input" MaxLength="100" />

                <asp:Label ID="lblRevenueAccountType" runat="server" Text="Account Type" CssClass="label leftclr" />
                <asp:DropDownList ID="ddlRevenueAccountType" runat="server" CssClass="input rightclr" />

                <asp:Label ID="lblRevenueNarrtionEn" runat="server" Text="Narration (Engligh)" CssClass="label leftclr" />
                <asp:TextBox ID="tbRevenueNarrationEn" runat="server" CssClass="input" MaxLength="100" />

                <asp:Label ID="lblRevenueNarrationFr" runat="server" Text="Narration (French)" CssClass="label leftclr" />
                <asp:TextBox ID="tbRevenueNarrationFr" runat="server" CssClass="input" MaxLength="100" />

                <asp:Label ID="lblRevenueNarrationPt" runat="server" Text="Narration (Portuguese)" CssClass="label leftclr" />
                <asp:TextBox ID="tbRevenueNarrationPt" runat="server" CssClass="input" MaxLength="100" />

                <asp:Label ID="lblRevenueNarrationEs" runat="server" Text="Narration (Spanish)" CssClass="label leftclr" />
                <asp:TextBox ID="tbRevenueNarrationEs" runat="server" CssClass="input" MaxLength="100" />
            </asp:Panel>

            <asp:Panel ID="pnlVatAccount" runat="server" GroupingText="VAT Account Details" CssClass="bothclr">
                <asp:Label ID="lblVatAccountNo" runat="server" Text="Account Number" CssClass="label leftclr" />
                <asp:TextBox ID="tbVatAccountNo" runat="server" CssClass="input" MaxLength="100" />

                <asp:Label ID="lblVatBranchCode" runat="server" Text="Branch Code" CssClass="label leftclr" />
                <asp:TextBox ID="tbVatBranchCode" runat="server" CssClass="input" MaxLength="100" />

                <asp:Label ID="lblVatAccountType" runat="server" Text="Account Type" CssClass="label leftclr" />
                <asp:DropDownList ID="ddlVatAccountType" runat="server" CssClass="input rightclr" />

                <asp:Label ID="lblVatNarrtionEn" runat="server" Text="Narration (Engligh)" CssClass="label leftclr" />
                <asp:TextBox ID="tbVatNarrationEn" runat="server" CssClass="input" MaxLength="100" />

                <asp:Label ID="lblVatNarrtionFr" runat="server" Text="Narration (French)" CssClass="label leftclr" />
                <asp:TextBox ID="tbVatNarrationFr" runat="server" CssClass="input" MaxLength="100" />

                <asp:Label ID="lblVatNarrtionPt" runat="server" Text="Narration (Portuguese)" CssClass="label leftclr" />
                <asp:TextBox ID="tbVatNarrationPt" runat="server" CssClass="input" MaxLength="100" />

                <asp:Label ID="lblVatNarrtionEs" runat="server" Text="Narration (Spanish)" CssClass="label leftclr" />
                <asp:TextBox ID="tbVatNarrationEs" runat="server" CssClass="input" MaxLength="100" />
            </asp:Panel>

        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnCreate" runat="server" Text="Create Accounting" CssClass="button"
                meta:resourcekey="btnCreateResource" OnClick="btnCreate_Click" />

            <asp:Button ID="btnEdit" runat="server" Text="<%$ Resources: CommonLabels, Edit %>"
                CssClass="button" OnClick="btnEdit_Click" />

            <asp:Button ID="btnUpdate" runat="server" Text="Update Accounting" CssClass="button"
                meta:resourcekey="btnUpdateResource" OnClick="btnUpdate_Click" />

            <asp:Button ID="btnDelete" runat="server" Text="<%$ Resources: CommonLabels, Delete %>"
                CssClass="button" OnClick="btnDelete_Click" />

            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>"
                CssClass="button" OnClick="btnConfirm_Click" />

            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources: CommonLabels, Back %>"
                CssClass="button" OnClick="btnBack_OnClick" Visible="False" CausesValidation="False" />
        </div>
    </div>
</asp:Content>
