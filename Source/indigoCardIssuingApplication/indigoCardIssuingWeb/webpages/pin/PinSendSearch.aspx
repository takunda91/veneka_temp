<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="PinSendSearch.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.pin.PinSendSearch" Culture="auto:en-US" meta:resourcekey="PageResource" UICulture="auto" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
     
        });
    </script>

    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblCardSearch" runat="server" Text="Pin Retrieval Search | Center Manager Approved " meta:resourcekey="lblCardSearchResource" />
            </div>


            <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlIssuer" runat="server" CssClass="input rightclr" AutoPostBack="true" OnSelectedIndexChanged="ddlIssuer_OnSelectedIndexChanged" />

            <asp:Label ID="lblBranch" runat="server" Text="<%$ Resources: CommonLabels, Branch %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlBranch" runat="server" CssClass="input rightclr" />

            <asp:Label ID="lblCardProduct" runat="server" Text="<%$ Resources: CommonLabels, Product %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlProduct" runat="server" AutoPostBack="true" CssClass="input rightclr" OnSelectedIndexChanged="ddlProduct_SelectedIndexChanged" />

            <asp:Label ID="lblCardNumber" runat="server" Text="Product Bin Code" CssClass="label leftclr" meta:resourcekey="lblCardNumberResource" />
            <asp:TextBox ID="tbCardNumber" runat="server" MaxLength="19" CssClass="input rightclr" />

            <asp:Label ID="lblLastFourDigits" runat="server" Text="Last Four Digits (PAN)" CssClass="label leftclr" meta:resourcekey="lblLastFourDigitsResource" />
            <asp:TextBox ID="tbLastFourDigits" runat="server" MaxLength="19" CssClass="input rightclr" />

            <asp:Label ID="lblrequestrefno" runat="server" Text="Request Reference No" CssClass="label leftclr" meta:resourcekey="lblrequestrefResource" />
            <asp:TextBox ID="tbrequestrefno" runat="server" CssClass="input" MaxLength="25" ToolTip="This is reference number shared with the customer via notifications channel" />    

            <asp:Label ID="lblCustomerAccount" Text="Customer Account" CssClass="label leftclr" runat="server" meta:resourcekey="lbllCustomerAccountResource" />
            <asp:TextBox ID="tbCustomerAccount" runat="server" MaxLength="100" CssClass="input rightclr" meta:resourcekey="tblCustomerAccount" />
         
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