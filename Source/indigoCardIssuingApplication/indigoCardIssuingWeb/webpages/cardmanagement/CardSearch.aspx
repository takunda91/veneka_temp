<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="CardSearch.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.cardmanagement.CardSearch" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">


    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblCardSearch" runat="server" Text="Card Search" meta:resourcekey="lblCardSearchResource" />
            </div>


            <asp:Label ID="lblCardNumber" runat="server" Text="Card Number" CssClass="label leftclr" meta:resourcekey="lblCardNumberResource" />
            <asp:TextBox ID="tbCardNumber" runat="server" MaxLength="19" CssClass="input rightclr" />

            <asp:Label ID="lblcardrefno" runat="server" Text="Account Number" CssClass="label leftclr" meta:resourcekey="lblIdNumberResource" />
            <asp:TextBox ID="tbAccountNumber" runat="server" CssClass="input" MaxLength="25" />    

        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnSearch" runat="server" Text="<%$ Resources: CommonLabels, Search %>" CssClass="button"
                OnClick="btnSearch_Click" />

            <asp:Button ID="btnCancel" runat="server" Text="<%$ Resources: CommonLabels, Cancel %>" CssClass="button" OnClick="btnCancel_OnClick" />
        </div>
    </div>
</asp:Content>
