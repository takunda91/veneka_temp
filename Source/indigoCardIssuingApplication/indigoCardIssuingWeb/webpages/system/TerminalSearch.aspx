<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="TerminalSearch.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.system.TerminalSearch" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblTerminalSearch" meta:resourcekey="lblTerminalsearchScreen" runat="server" Text="Terminal Search" />
            </div>

            <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlIssuer" runat="server" CssClass="input rightclr" AutoPostBack="true" OnSelectedIndexChanged="ddlIssuer_SelectedIndexChanged" />

            <asp:Label ID="lblBranch" runat="server" Text="<%$ Resources: CommonLabels, Branch %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlBranch" runat="server" CssClass="input rightclr" />

            <asp:Label ID="lblterminalname" runat="server" Text="Terminal Name" CssClass="label leftclr" />
            <asp:TextBox ID="tbterminalname" runat="server" CssClass="input rightclr" />

            <asp:Label ID="lblmodelname" runat="server" Text="Model Name" CssClass="label leftclr" />
            <asp:TextBox ID="tbmodelname" runat="server" CssClass="input rightclr" />


            <asp:Label ID="lbldevicenumber" runat="server" Text="Serial Number" CssClass="label leftclr" />
            <asp:TextBox ID="tbdevicenumber" runat="server" CssClass="input rightclr" />

        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnSearch" runat="server" Text="<%$ Resources: CommonLabels, Search %>" CssClass="button"
                OnClick="btnSearch_Click" />

            <asp:Button ID="btnCancel" runat="server" Text="<%$ Resources: CommonLabels, Cancel %>" CssClass="button" OnClick="btnCancel_Click" />
        </div>
    </div>
</asp:Content>
