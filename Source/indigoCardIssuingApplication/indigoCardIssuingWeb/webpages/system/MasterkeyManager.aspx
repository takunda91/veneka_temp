<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master"
    AutoEventWireup="true" CodeBehind="MasterkeyManager.aspx.cs"
    Inherits="indigoCardIssuingWeb.webpages.system.MasterkeyManager"
    meta:resourcekey="PageResource" UICulture="auto" Culture="auto:en-US" %>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblMasterKeyDetails" runat="server" Text="Master Key Details" meta:resourcekey="lblMasterKeyDetailsResource" />
            </div>
            <div>
                <asp:Label runat="server" ID="lblIssuer" Text="Issuer" meta:resourcekey="lblIssuerResource" CssClass="label leftclr"></asp:Label>
                <asp:DropDownList runat="server" ID="ddlIssuer" CssClass="input rightclr" OnSelectedIndexChanged="ddlIssuer_SelectedIndexChanged" meta:resourcekey="ddlIssuerResource1"></asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlIssuer" ErrorMessage="Issuer Required"
                    meta:resourcekey="rfvIssuerResource" CssClass="validation rightclr" ForeColor="Red"></asp:RequiredFieldValidator>

                <asp:Label runat="server" ID="lblMasterkeyName" Text="MasterKey Name" CssClass="label leftclr"
                    meta:resourcekey="lblMasterkeyNameResource"></asp:Label>
                <asp:TextBox runat="server" ID="txtMasterkeyName" CssClass="input" meta:resourcekey="txtMasterkeyNameResource1"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMasterkeyName" ID="rfvMasterkeyName"
                    meta:resourcekey="rfvMasterkeyNameResource" ErrorMessage="Masterkey Name Required." ForeColor="Red" CssClass="validation rightclr"></asp:RequiredFieldValidator>

                <asp:Label runat="server" ID="lblMasterKey" Text="MasterKey" CssClass="label leftclr"
                    meta:resourcekey="lblMasterkeyResource"></asp:Label>
                <asp:TextBox runat="server" ID="txtMasterkey" CssClass="input" meta:resourcekey="txtMasterkeyResource1"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMasterkey" ID="rfvMasterkey"
                    meta:resourcekey="rfvMasterkeyResource" ErrorMessage="MasterKey Required." ForeColor="Red" CssClass="validation rightclr"></asp:RequiredFieldValidator>

            </div>
        </div>
        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>
        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnCreate" runat="server" Text="<%$ Resources:CommonLabels, Create %>" CssClass="button" meta:resourcekey="btnCreateResource"
                OnClick="btnCreate_Click" />
            <asp:Button ID="btnEdit" runat="server" Text="<%$ Resources:CommonLabels, Edit %>"
                CssClass="button" OnClick="btnEdit_Click" meta:resourcekey="btnEditResource1" />
            <asp:Button ID="btnUpdate" runat="server" Text="<%$ Resources:CommonLabels, Update %>" CssClass="button" meta:resourcekey="btnUpdateResource"
                OnClick="btnUpdate_Click" />
            <asp:Button ID="btnDelete" runat="server" Text="<%$ Resources: CommonLabels, Delete %>"
                CssClass="button" OnClick="btnDelete_Click" />
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources:CommonLabels, Confirm %>"
                CssClass="button" OnClick="btnConfirm_Click" meta:resourcekey="btnConfirmResource1" />
            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources:CommonLabels, Back %>"
                CssClass="button" OnClick="btnBack_Click" Visible="False" CausesValidation="False" meta:resourcekey="btnBackResource1" />
        </div>
    </div>
</asp:Content>
