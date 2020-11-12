<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="PINReset.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.pin.PINReset" %>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">

        $(document).ready(function () {
            $('#dlgPinAuth').puidialog({
                minimizable: false,
                maximizable: false,
                closable: false,
                closeOnEscape: false,
                width: 500,
                height: 200,
                minHeight: 500,
                minHeight: 200,
                modal: true
            });
        });

        function showPinAuth() {
            $(document).ready(function () {
                $('#dlgPinAuth').puidialog('show');
            });
        }

        function hidePinAuth() {
            $(document).ready(function () {
                $('#dlgPinAuth').puidialog('hide');
            });
        }                
    </script>

    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <asp:HiddenField ID="hdnGuid" ClientIDMode="Static" runat="server" />

            <div id="dlgPinAuth" title="Pin Authorisation"> 
               <div>
                   <asp:Label ID="lblPinAuthUser" runat="server" Text="Username" CssClass="label leftclr" />
                   <asp:TextBox ID="tbPinAuthUser" runat="server" CssClass="input" />                   

                    <asp:Label ID="lblPinAuthPasscode" runat="server" Text="Passcode" CssClass="label leftclr" />
                    <asp:TextBox ID="tbPinAuthPasscode" runat="server" CssClass="input" TextMode="Password" />
                    
                    <asp:Label ID="lblComments" runat="server" Text="Comments" CssClass="label leftclr" />
                    <asp:TextBox ID="tbComments" runat="server" CssClass="input" />
                </div>
                <div class="ErrorPanel">
                    <asp:Label ID="lblAuthError" runat="server" />
                </div>

                <div class="InfoPanel">
                    <asp:Label ID="lblAuthInfo" runat="server" />
                </div>

                <div class="ButtonPanel">
                    <%--<asp:Button ID="btnSubmitPasscode" runat="server" Text="Submit" OnClick="btnSubmitPasscode_Click" CssClass="button" />--%>
                    
                    <asp:Button ID="btnApprove" runat="server" Text="Approve" OnClick="btnApprove_Click" CssClass="button" />
                    <asp:Button ID="btnReject" runat="server" Text="Reject" OnClick="btnReject_Click" CssClass="button" />   
                    <asp:Button ID="btnPasscodeClose" runat="server" Text="Close" OnClick="btnPasscodeClose_Click" CssClass="button" />             
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
            <asp:Button ID="btnPinCapture" runat="server" Text="Capture PIN" CssClass="button" OnClick="btnPinCapture_OnClick" meta:resourcekey="btnPinCaptureResource" Visible="false" />
            <%--<asp:Button ID="btnContinue" runat="server" Text="Continue" CssClass="button" OnClick="btnContinue_Click" Visible="false" />--%>
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" CssClass="button" OnClick="btnConfirm_OnClick" Visible="false" />
        </div>
    </div>
</asp:Content>
