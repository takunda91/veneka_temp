<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="LoadFundsApprove.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.fundsload.LoadFundsApprove" %>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div class="ContentHeader">
            <asp:Label ID="lblFundsLoadHeader" runat="server" Text="Confirm Funds Load" meta:resourcekey="loadFundsApproveHeader" />
        </div>
        <div style="width: 50%; float: left;">
            <div runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label2col leftclr" />
                <asp:TextBox ID="txtIssuer" runat="server" CssClass="input2col rightclr" MaxLength="50" Enabled="false" />
            </div>
            
            <div runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="lblBranch" runat="server" Text="<%$ Resources: CommonLabels, Branch %>" CssClass="label2col leftclr" />
                <asp:TextBox ID="txtBranch" runat="server" CssClass="input2col rightclr" MaxLength="50" Enabled="false" />
            </div>

            <div id="pnlAccountNumber" runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="lblAccountNumber" runat="server" Text="Account number" CssClass="label2col leftclr" meta:resourcekey="loadFundsAccountNumber" />
                <asp:TextBox ID="txtAccountNumber" runat="server" CssClass="input2col" MaxLength="27" Enabled="false" />
            </div>

            <div id="pnlAmount" runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="lblAmount" runat="server" Text="Amount" CssClass="label2col leftclr" meta:resourcekey="loadFundsAmount" />
                <asp:TextBox ID="txtAmount" runat="server" CssClass="input2col" Enabled="false" />
            </div>

            <asp:Panel ID="pnlCustomerDetail" runat="server">
                <div id="divFirstName" runat="server">
                    <asp:Label ID="lblFirstName" runat="server" Text="First name" CssClass="label2col leftclr" meta:resourcekey="lblFirstNameResource" />
                    <asp:TextBox ID="tbFirstName" runat="server" CssClass="input2col" MaxLength="50" Enabled="false" />
                </div>
                <div id="divLastName" runat="server">
                    <asp:Label ID="lblLastName" runat="server" Text="Last name" CssClass="label2col leftclr" meta:resourcekey="lblLastNameResource" />
                    <asp:TextBox ID="tbLastName" runat="server" CssClass="input2col" MaxLength="50" Enabled="false" />
                </div>

                <div id="divContactnumber" runat="server">
                    <asp:Label ID="lblAddress" runat="server" Text="Address" CssClass="label2col leftclr" meta:resourcekey="lblcontactnumberResource" />
                    <asp:TextBox ID="txtAddress" runat="server" CssClass="input2col rightclr" MaxLength="50" Height="87px" Enabled="false" TextMode="MultiLine" />
                </div>
            </asp:Panel>
            <!--Prepaid Account Details -->
            <div id="panelPrepaid" runat="server">
                <div id="pnlPrepaidCard" runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                    <asp:Label ID="lblPrepaidCard" runat="server" Text="Prepaid Card" CssClass="label2col leftclr" meta:resourcekey="loadFundsPrepaidCard" />
                    <asp:TextBox ID="txtPrepaidCard" runat="server" CssClass="input2col" MaxLength="27" Enabled="false" />
                </div>
                <div id="div1" runat="server">
                    <asp:Label ID="lblPrepaidAccountNumber" runat="server" Text="Prepaid Account Number" CssClass="label2col leftclr" meta:resourcekey="lblPrepaidAccountNumber" />
                    <asp:TextBox ID="txtPrepaidAccountNumber" runat="server" CssClass="input2col" Enabled="false" />
                </div>
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

            <div runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="Label3" runat="server" Text="Reviewed By" CssClass="label2col leftclr" meta:resourcekey="loadFundsReviewedBy" />
                <asp:TextBox ID="txtReviewedBy" runat="server" CssClass="input2col" MaxLength="27" Enabled="false" />
            </div>

            <div runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="Label4" runat="server" Text="Reviewed Date" CssClass="label2col leftclr" meta:resourcekey="loadFundsReviewedDate" />
                <asp:TextBox ID="txtReviewedDate" runat="server" CssClass="input2col" MaxLength="27" Enabled="false" />
            </div>

            <div runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="Label5" runat="server" Text="Approved By" CssClass="label2col leftclr" meta:resourcekey="loadFundsApprovedBy" />
                <asp:TextBox ID="txtApprovedBy" runat="server" CssClass="input2col" MaxLength="27" Enabled="false" />
            </div>

            <div runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="Label6" runat="server" Text="Approved Date" CssClass="label2col leftclr" meta:resourcekey="loadFundsApprovedDate" />
                <asp:TextBox ID="txtApprovedDate" runat="server" CssClass="input2col" MaxLength="27" Enabled="false" />
            </div>

            <div runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="Label7" runat="server" Text="Loaded By" CssClass="label2col leftclr" meta:resourcekey="loadFundsLoadedBy" />
                <asp:TextBox ID="txtLoadedBy" runat="server" CssClass="input2col" MaxLength="27" Enabled="false" />
            </div>

            <div runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="Label8" runat="server" Text="Loaded Date" CssClass="label2col leftclr" meta:resourcekey="loadFundsLoadedDate" />
                <asp:TextBox ID="txtLoadedDate" runat="server" CssClass="input2col" MaxLength="27" Enabled="false" />
            </div>
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="loadFundsInfo" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="loadFundsError" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnSave" runat="server" Text="<%$ Resources: CommonLabels, Approve %>" OnClick="btnSave_Click" CssClass="button" Visible="true" />
            <asp:Button ID="btnReject" runat="server" Text="Reject" OnClick="btnReject_Click" CssClass="button" Visible="true" />

            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" OnClick="btnConfirm_Click" CssClass="button" Visible="false" />
            <asp:Button ID="btnCancel" runat="server" Text="<%$ Resources: CommonLabels, Cancel %>" OnClick="btnCancel_Click" CssClass="button" CausesValidation="false" Visible="true" />
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
