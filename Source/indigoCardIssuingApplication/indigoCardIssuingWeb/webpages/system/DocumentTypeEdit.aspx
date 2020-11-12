<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="DocumentTypeEdit.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.system.DocumentTypeEdit" %>
<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div class="ContentHeader">
            <asp:Label ID="lblDocumentHeader" runat="server" Text="Create Document" meta:resourcekey="loadFundsHeader" />
        </div>
        <asp:HiddenField ID="documentTypeId" runat="server" />
        <div id="pnlName" runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
            <asp:Label ID="lblName" runat="server" Text="Name" CssClass="label leftclr" meta:resourcekey="lblName" />
            <asp:TextBox ID="txtName" runat="server" CssClass="input" MaxLength="100" />

            <asp:RequiredFieldValidator runat="server" ID="reqAccNo" ControlToValidate="txtName" meta:resourcekey="documentNameRequired"
                ErrorMessage="Please enter name for document." CssClass="validation rightclr" />

        </div>
        <div id="Div1" runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
            <asp:Label ID="lblDescription" runat="server" Text="Description" CssClass="label leftclr" meta:resourcekey="lblDescription" />
            <asp:TextBox ID="txtDescription" runat="server" CssClass="input" MaxLength="500" />

            <asp:RequiredFieldValidator runat="server" ID="reqDescription" ControlToValidate="txtDescription" meta:resourcekey="documentDescriptionRequired"
                ErrorMessage="Please enter description for document." CssClass="validation rightclr" />

        </div>
        <div id="pnlAmount" runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
            <asp:Label ID="lblIsActive" runat="server" Text="Active" CssClass="label leftclr" meta:resourcekey="loadFundsAmount" />
            <asp:CheckBox ID="chkIsActive" runat="server" CssClass="input" />
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="loadFundsInfo" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="loadFundsError" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnSave" runat="server" Text="<%$ Resources: CommonLabels, Save %>" OnClick="btnSave_Click" CssClass="button" />
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" OnClick="btnConfirm_Click" CssClass="button" Visible="false" />
            <asp:Button ID="btnCancel" runat="server" Text="<%$ Resources: CommonLabels, Cancel %>" OnClick="btnCancel_Click" CssClass="button" CausesValidation="false" />
        </div>

    </div>
</asp:Content>
<asp:Content ID="Content6" runat="server" ContentPlaceHolderID="cphCustom">
    <script type="text/javascript">           

        javascript: window.history.forward();
        var t;
        window.onload = resetTimer;
        document.onkeypress = resetTimer;
        document.onclick = resetTimer;

        function logout() {
            var root = location.protocol + "//" + location.host;
            var applicationPath = "<%=Request.ApplicationPath%>";
            if (applicationPath == "/") {
                location.href = root + applicationPath + 'Logout.aspx'
            }
            else {
                location.href = root + applicationPath + '/Logout.aspx'
            }
        }

        function resetTimer() {
            clearTimeout(t);
            t = setTimeout(logout, 590000); //600000 log out in 10 min1800000
        }
    </script>
    <style type="text/css">
        .rightclr {
        }
    </style>
</asp:Content>