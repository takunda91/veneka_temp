<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="LoadFunds.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.fundsload.LoadFunds" %>


<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div class="ContentHeader">
            <asp:Label ID="lblFundsLoadHeader" runat="server" Text="Create Funds Load Entry" meta:resourcekey="loadFundsHeader" />
        </div>
        <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr" />
        <asp:DropDownList ID="ddlIssuer" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlIssuer_OnSelectedIndexChanged" CssClass="input rightclr" />

        <asp:Label ID="lblBranch" runat="server" Text="<%$ Resources: CommonLabels, Branch %>" CssClass="label leftclr" />
        <asp:DropDownList ID="ddlBranch" runat="server" AutoPostBack="true" CssClass="input rightclr" />

        <asp:Label ID="lblCardProduct" runat="server" Text="<%$ Resources: CommonLabels, Product %>" CssClass="label leftclr" />
        <asp:DropDownList ID="ddlProduct" runat="server" AutoPostBack="true" CssClass="input rightclr" />

        <div id="pnlAccountNumber" runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
            <asp:Label ID="lblAccountNumber" runat="server" Text="Account number" CssClass="label leftclr" meta:resourcekey="loadFundsAccountNumber" />
            <asp:TextBox ID="tbAccountNumber" runat="server" CssClass="input" MaxLength="27" />


            <asp:RequiredFieldValidator runat="server" ID="reqAccNo" ControlToValidate="tbAccountNumber" meta:resourcekey="loadFundsAccountRequired"
                ErrorMessage="Please enter account number." CssClass="validation rightclr" />

        </div>
        <div id="pnlAmount" runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
            <asp:Label ID="lblAmount" runat="server" Text="Amount" CssClass="label leftclr" meta:resourcekey="loadFundsAmount" />
            <asp:TextBox ID="tbAmount" runat="server" CssClass="input" />

            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="tbAccountNumber" meta:resourcekey="loadFundsAmountRequired"
                ErrorMessage="Please enter account number." CssClass="validation rightclr" />

        </div>
        <div id="pnlCheck" runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
            <asp:Label ID="lblButtonSpacer" runat="server" Text="" CssClass="label leftclr" />
            <asp:Button ID="btnValidateAccount" runat="server" Text="Bank Check" meta:resourcekey="loadFundsValidateAccount"
                CssClass="button" Style="margin: 0px 0px 0px 5px !important; float: left;"
                OnClick="btnValidateAccount_Click" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblBankAccountNotFound" runat="server" meta:resourcekey="loadFundsBankAccountNotFound" />
        </div>
        <!--Account Holder Details Displayed-->

        <asp:Panel ID="pnlCustomerDetail" runat="server" Visible="false">
            <div id="divFirstName" runat="server">
                <asp:Label ID="lblFirstName" runat="server" Text="First name" CssClass="label leftclr" meta:resourcekey="lblFirstNameResource" />
                <asp:TextBox ID="tbFirstName" runat="server" CssClass="input" MaxLength="50" Enabled="false" />
            </div>
            <div id="divLastName" runat="server">
                <asp:Label ID="lblLastName" runat="server" Text="Last name" CssClass="label leftclr" meta:resourcekey="lblLastNameResource" />
                <asp:TextBox ID="tbLastName" runat="server" CssClass="input" MaxLength="50" Enabled="false" />
            </div>

            <div id="divContactnumber" runat="server">
                <asp:Label ID="lblAddress" runat="server" Text="Address" CssClass="label leftclr" meta:resourcekey="lblcontactnumberResource" />
                <asp:TextBox ID="txtAddress" runat="server" CssClass="input rightclr" MaxLength="50" Height="87px" Enabled="false" TextMode="MultiLine" />
            </div>
        </asp:Panel>

        <!--Prepaid Account Details -->
        <div id="panelPrepaid" runat="server" visible="false">
            <div id="pnlPrepaidCard" runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="lblPrepaidCard" runat="server" Text="Prepaid Card" CssClass="label leftclr" meta:resourcekey="loadFundsPrepaidCard" />
                <asp:TextBox ID="txtPrepaidCard" runat="server" CssClass="input" MaxLength="27" OnKeyPress="return isNumberKey(event)" />

                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" ControlToValidate="tbAccountNumber" meta:resourcekey="loadFundsPrepaidRequired"
                    ErrorMessage="Please enter account number." CssClass="validation rightclr" />

            </div>
            <div id="pnlPrepaidValidate" runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="lblPrepaidSpacer" runat="server" Text="" CssClass="label leftclr" />
                <asp:Button ID="btnValidatePrepaid" runat="server" Text="Prepaid Check" meta:resourcekey="loadFundsValidatePrepaid"
                    CssClass="button" Style="margin: 0px 0px 0px 5px !important; float: left;" OnClick="btnValidatePrepaid_Click" />
            </div>

            <asp:Panel ID="panelPrepaidDetails" runat="server" Visible="false">
                <div id="div1" runat="server">
                    <asp:Label ID="lblPrepaidAccountNumber" runat="server" Text="Prepaid Account Number" CssClass="label leftclr" meta:resourcekey="lblPrepaidAccountNumber" />
                    <asp:TextBox ID="txtPrepaidAccountNumber" runat="server" CssClass="input" Enabled="false" />
                </div>
            </asp:Panel>

            <div class="ErrorPanel">
                <asp:Label ID="lblPrepaidNotFound" runat="server" meta:resourcekey="loadFundsPrepaidNotFound" />
            </div>
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="loadFundsInfo" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="loadFundsError" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnSave" runat="server" Text="<%$ Resources: CommonLabels, Save %>" OnClick="btnSave_Click" CssClass="button" Visible="false" />
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" OnClick="btnConfirm_Click" CssClass="button" Visible="false" />
            <asp:Button ID="btnCancel" runat="server" Text="<%$ Resources: CommonLabels, Cancel %>" OnClick="btnCancel_Click" CssClass="button" CausesValidation="false" Visible="false" />
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

