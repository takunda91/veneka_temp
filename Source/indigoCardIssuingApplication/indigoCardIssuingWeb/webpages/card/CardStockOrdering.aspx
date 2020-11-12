<%@ Page Language="C#" MasterPageFile="~/IndigoCardIssuance.Master"AutoEventWireup="true" CodeBehind="CardStockOrdering.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.card.CardStockOrdering" culture="auto" meta:resourcekey="PageResource1" uiculture="auto" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblCardOrder" runat="server" Text="Production Batch List" meta:resourcekey="lblCardOrderResource1"/>
            </div>

            <asp:Label ID="lblIssuer" runat="server"  CssClass="label leftclr"  Text="<%$ Resources:CommonLabels, Issuer %>" meta:resourcekey="lblIssuerResource1"/>
            <asp:DropDownList ID="ddlIssuer" runat="server" CssClass="input rightclr" OnSelectedIndexChanged="ddlIssuer_OnSelectedIndexChanged" AutoPostBack="true" meta:resourcekey="ddlIssuerResource1" />

            <asp:Label ID="lblBranch" runat="server" Text="<%$ Resources:CommonLabels, Branch %>" CssClass="label leftclr" meta:resourcekey="lblBranchResource1"  />
            <asp:DropDownList ID="ddlBranch" runat="server" CssClass="input rightclr" AutoPostBack="True" meta:resourcekey="ddlBranchResource1"/>

            <asp:Label ID="lblProduct" runat="server"  CssClass="label leftclr"  Text="Product" meta:resourcekey="lblProductResource1"/>
            <asp:DropDownList ID="ddlProduct" runat="server" CssClass="input rightclr" meta:resourcekey="ddlProductResource1" OnSelectedIndexChanged="ddlProduct_OnSelectedIndexChanged" AutoPostBack="true" />
              
            <asp:Label ID="lblNumberOfCards" runat="server" Text="Number of cards" CssClass="label leftclr" meta:resourcekey="lblNumberOfCardsForBatchResource" />
            <asp:TextBox ID="tbNumberOfCards" runat="server" OnKeyPress="return isNumberKey(event)" CssClass="input" meta:resourcekey="tbNumberOfCardsResource1" MaxLength="6"/>
            <asp:RequiredFieldValidator ID="rfvNumberOfCards" runat="server" ControlToValidate="tbNumberOfCards"
                                        ErrorMessage="Please enter the amount of cards to order." CssClass="validation rightclr" meta:resourcekey="rfvNumberOfCardsResource1" /> 
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnCreate" runat="server" Text="<%$ Resources:CommonLabels, Create %>" OnClick="btnCreate_Click" CssClass="button" meta:resourcekey="btnCreateResource1" />
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources:CommonLabels, Confirm %>" OnClick="btnConfirm_Click" CssClass="button" Visible="False" meta:resourcekey="btnConfirmResource1" />
            <asp:Button ID="btnCancel" runat="server" Text="<%$ Resources:CommonLabels, Cancel %>" OnClick="btnCancel_Click" CssClass="button" CausesValidation="False" Visible="False" meta:resourcekey="btnCancelResource1" />
        </div>
    </div>    
</asp:Content>