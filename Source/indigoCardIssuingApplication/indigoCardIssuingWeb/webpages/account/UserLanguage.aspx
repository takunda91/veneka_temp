<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="UserLanguage.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.account.UserLanguage" culture="auto:en-US" meta:resourcekey="PageResource" uiculture="auto" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphLogin" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script language="javascript" type="text/javascript" >
<!--
    function Refresh() {


        document.forms[0].submit();
        
    }
// -->
</script>
    <div class="ContentPanel">

        <div class="ContentHeader">
            <asp:Label ID="lblChangeLanguage" runat="server" Text="Change Language" meta:resourcekey="lblChangeLanguageResource"  />
        </div>
    
        <asp:Label ID="lblLanguage" runat="server" Text="Select Language:" CssClass="label leftclr" meta:resourcekey="lblLanguageResource" />      
        <asp:DropDownList ID="ddlMasterLangueges" CssClass="input" runat="server" ClientIDMode="Static" />


        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource"/>
        </div>
        
        <div class="ButtonPanel">
            <asp:Button ID="btnChangeLang" runat="server" CssClass="button"
                        Text="Change Language" OnClick="btnChangeLang_Click" meta:resourcekey="btnChangeLangResource" />
            <asp:Button ID="btnConfirm" runat="server" CssClass="button" OnClick="btnConfirm_Click" Visible="False" Text="<%$ Resources: CommonLabels, Confirm %>" />
            <asp:Button ID="btnCancel" runat="server" CssClass="button" OnClick="btnCancel_Click" Visible="False" Text="<%$ Resources: CommonLabels, Cancel %>"/>
        </div>
    </div>
</asp:Content>
