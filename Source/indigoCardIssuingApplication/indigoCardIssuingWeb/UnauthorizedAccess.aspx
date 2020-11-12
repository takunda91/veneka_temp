<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="UnauthorizedAccess.aspx.cs" Inherits="indigoCardIssuingWeb.UnauthorizedAccess" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" Text='<%$ Resources:DefaultExceptions, UnauthorisedPageAccessMessage %>' />            
        </div>
    </div>
</asp:Content>
