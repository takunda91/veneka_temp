<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="RenewalApprove.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.renewal.RenewalApprove" %>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div class="ContentHeader">
            <asp:Label ID="lblFundsLoadHeader" runat="server" Text="Confirm Funds Load" meta:resourcekey="loadFundsApproveHeader" />
        </div>
        <div style="width: 50%; float: left;">
            <div runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="lblFileName" runat="server" Text="File Name" CssClass="label2col leftclr" />
                <asp:TextBox ID="txtFileName" runat="server" CssClass="input2col rightclr" MaxLength="50" Enabled="false" />
            </div>

            <div runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="lblUploaded" runat="server" Text="Uploaded" CssClass="label2col leftclr" />
                <asp:TextBox ID="txtUploaded" runat="server" CssClass="input2col rightclr" MaxLength="50" Enabled="false" />
            </div>

            <div runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="lblStatus" runat="server" Text="Status" CssClass="label2col leftclr" />
                <asp:TextBox ID="txtStatus" runat="server" CssClass="input2col rightclr" MaxLength="50" Enabled="false" />
            </div>
        </div>

        <div style="width: 50%; float: right">
            <div runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="Label1" runat="server" Text="Created By" CssClass="label2col leftclr" meta:resourcekey="loadFundsCreatedBy" />
                <asp:TextBox ID="txtCreatedBy" runat="server" CssClass="input2col" MaxLength="27" Enabled="false" />
            </div>

            <div runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="Label2" runat="server" Text="Created Date" CssClass="label2col leftclr" meta:resourcekey="loadFundsCreatedDate" />
                <asp:TextBox ID="txtCreatedDate" runat="server" CssClass="input2col" MaxLength="27" Enabled="false" />
            </div>
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="loadFundsInfo" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="loadFundsError" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnApprove" runat="server" Text="<%$ Resources: CommonLabels, Approve %>" OnClick="btnApprove_Click" CssClass="button" Visible="true" />
            <asp:Button ID="btnReject" runat="server" Text="Reject" OnClick="btnReject_Click" CssClass="button" Visible="true" />

            <asp:Button ID="btnPrint" runat="server" Text="<%$ Resources: CommonLabels, PrintDocument %>" OnClick="btnPrint_Click" CssClass="button" Visible="False" />

            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" OnClick="btnConfirm_Click" CssClass="button" Visible="false" />
            <asp:Button ID="btnBack" runat="server" Text="Back" OnClick="btnBack_Click" CssClass="button" CausesValidation="false" Visible="true" />
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
