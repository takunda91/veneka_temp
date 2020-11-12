<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="PINResetPOS.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.pin.PINResetPOS" %>
<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {
            $('#dlgPinSession').puidialog({
                minimizable: false,
                maximizable: false,
                closable: false,
                closeOnEscape: false,
                width: 400,
                height: 150,
                minHeight: 400,
                minHeight: 150,
                modal: true
            });
        });

        function showPinAuth() {
            $(document).ready(function () {
                $('#dlgPinSession').puidialog('show');
            });
        }

        function hidePinAuth() {
            $(document).ready(function () {
                $('#dlgPinSession').puidialog('hide');
            });
        }                
    </script>

    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">

            <div id="dlgPinSession" title="Pin Session Key"> 
               <div>
                   <asp:Label ID="lblPinSessionKey" runat="server" Text="Session Key" CssClass="label leftclr" />
                   <asp:TextBox ID="tbPinSessionKey" runat="server" Enabled="false" CssClass="input" />
                </div>
                <div class="ErrorPanel">
                    <asp:Label ID="lblAuthError" runat="server" />
                </div>

                <div class="InfoPanel">
                    <asp:Label ID="lblAuthInfo" runat="server" />
                </div>

                <div class="ButtonPanel">
                    <asp:Button ID="btnSubmitPasscode" runat="server" Text="Close" OnClientClick="hidePinAuth();" CssClass="button" />             
                </div>
            </div>


            <div class="ContentHeader">
                <asp:Label ID="lblPinReissue" Text="PIN Reissue" runat="server" />
            </div>            

            <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlIssuer" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlIssuer_OnSelectedIndexChanged" CssClass="input rightclr" />

            <asp:Label ID="lblBranch" runat="server" Text="<%$ Resources: CommonLabels, Branch %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlBranch" runat="server" CssClass="input rightclr" />
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnGenPinSessionKey" runat="server" Text="Generate Session Key" CssClass="button" OnClick="btnGenPinSessionKey_OnClick" Visible="false" />
        </div>
    </div>
</asp:Content>
