<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="CustomerSearch.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.card.CustomerSearch" %>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblCardSearch" runat="server" Text="Customer Search" />
            </div>

            <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlIssuer" runat="server" CssClass="input rightclr" AutoPostBack="true" OnSelectedIndexChanged="ddlIssuer_OnSelectedIndexChanged" />

            <asp:Label ID="lblBranch" runat="server" Text="<%$ Resources: CommonLabels, Branch %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlBranch" runat="server" CssClass="input rightclr" />

            <asp:Label ID="lblCardIssueMethod" runat="server" Text="Card Issue Method" CssClass="label leftclr" meta:resourcekey="lblCardIssueMethodResource"   />
            <asp:DropDownList ID="ddlCardIssueMethod" runat="server" CssClass="input rightclr"  OnSelectedIndexChanged="ddlCardIssueMethod_SelectedIndexChanged" AutoPostBack="true"/>

            <asp:Label ID="lblCardProduct" runat="server" Text="<%$ Resources: CommonLabels, Product %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlProduct" runat="server" AutoPostBack="true" CssClass="input rightclr" OnSelectedIndexChanged="ddlProduct_SelectedIndexChanged" />
            
            <asp:Label ID="lblcardrefno" runat="server" Text="Card Ref No" CssClass="label leftclr" meta:resourcekey="lblcardrefnoResource" />
            <asp:TextBox ID="tbcardrefno" runat="server" CssClass="input" MaxLength="100" />    
            
            <asp:Label ID="lblAccountNumber" runat="server" Text="Account Number" CssClass="label leftclr" />
            <asp:TextBox ID="tbAccountNumber" runat="server" CssClass="input" MaxLength="100" />  

            <asp:Label ID="lblFirstName" runat="server" Text="First Name" CssClass="label leftclr" />
            <asp:TextBox ID="tbFirstName" runat="server" CssClass="input" MaxLength="100" />  

            <asp:Label ID="lblLastName" runat="server" Text="Last Name" CssClass="label leftclr" />
            <asp:TextBox ID="tbLastName" runat="server" CssClass="input" MaxLength="100" />  

            <asp:Label ID="lblCmsId" runat="server" Text="CMS ID" CssClass="label leftclr" />
            <asp:TextBox ID="tbCMSId" runat="server" CssClass="input" MaxLength="100" />             
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
