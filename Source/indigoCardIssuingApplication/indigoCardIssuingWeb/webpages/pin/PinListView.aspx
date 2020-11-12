<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/IndigoCardIssuance.Master" CodeBehind="PinListView.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.pin.PinListView" Culture="auto:en-US" meta:resourcekey="PageResource" UICulture="auto" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {


              $(".puidropdown").puidropdown({
                effectSpeed: 1
            });

            $('input[type="text"]').puiinputtext();


            //Checks if the control is read only, if it is make sure that backspace doesnt do 
            // a back operation.
            $('input[type="text"]').keydown(function () {
                if ($(this).prop('readOnly')) {
                    preventBackspace();
                }
            });

            $('.DialogCardList').puidialog({
                width: 700,
                height: 300,
                showEffect: 'fade',
                hideEffect: 'fade',
                minimizable: false,
                maximizable: false,
                modal: true,
                appendTo: 'form',
                open: function (type, data) { $(this).parent().appendTo("form"); }
            });


        });

        function showCardDialog() {
            $(document).ready(function () {
                $('.DialogCardList').puidialog('show');
            });
        }

        function downloadDocument(documentURL) {
            window.location.href = documentURL;
        }
    </script>

    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">

            <div class="ContentHeader">
                <asp:Label ID="lblCardAndCustomerDetails" runat="server" Text="Pin Request Details" meta:resourcekey="lblPinRequestDetails" />
            </div>
                    <asp:Panel ID="pnlCardDetails" runat="server" GroupingText="Pin Request Details" CssClass="bothclr" meta:resourcekey="lblCardDetailsResource">
                  
                        <asp:Label ID="lblRequestReference" runat="server" Text="Request Reference" CssClass="label leftclr"  meta:resourcekey="lblProductNameResource" />
                    <asp:TextBox ID="tbRequestReference" runat="server" Enabled="False" CssClass="input rightclr"  />

                      

                    <asp:Label ID="lblProductName" runat="server" Text="Card Product" CssClass="label leftclr" meta:resourcekey="lblProductNameResource" />
                    <asp:TextBox ID="tbProductName" runat="server" Enabled="False" CssClass="input rightclr" />

                    <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr" />
                    <asp:TextBox ID="tbIssuer" runat="server" Enabled="False" CssClass="input rightclr" />

                    <asp:Label ID="lblIssuingBranch" runat="server" Text="Issuing Branch" CssClass="label leftclr" meta:resourcekey="lblIssuingBranchResource" />
                    <asp:TextBox ID="tbIssuingBranch" runat="server" Enabled="False" CssClass="input rightclr" />

                    <asp:Label ID="lblCardPan" runat="server" Text="Card Pan" CssClass="label leftclr" />
                    <asp:TextBox ID="tbCardPan" runat="server" Enabled="False" CssClass="input rightclr" />

                    <asp:Label ID="lblExpiryYear" runat="server" Text="Card Exiry Year" CssClass="label leftclr" />
                    <asp:TextBox ID="tbExpiryYear" runat="server" Enabled="False" CssClass="input rightclr" />

                    <asp:Label ID="lblExpiryMonth" runat="server" Text="Card Expiry Month" CssClass="label leftclr" meta:resourcekey="lblBrachCardStatusResource" />
                    <asp:TextBox ID="tbExpiryMonth" runat="server" Enabled="False" CssClass="input rightclr" />

                    <asp:Label ID="lblPinDistMethod" runat="server" Text="Pin Distribution Method" CssClass="label leftclr" />
                    <asp:TextBox ID="tbPinDistMethod" runat="server" Enabled="False" CssClass="input rightclr" />

                         <asp:Label ID="lblAccountNumber" runat="server" Text="Customer Account Number" CssClass="label leftclr" />
                    <asp:TextBox ID="tbAccountNumber" runat="server" Enabled="False" CssClass="input rightclr" />

                        <asp:Label ID="lblContactNumber" runat="server" Text="Customer Contact Number" CssClass="label leftclr" />
                    <asp:TextBox ID="tbContactNumber" runat="server" Enabled="False" CssClass="input rightclr" />

                        <asp:Label ID="lblEmail" runat="server" Text="Customer Email" CssClass="label leftclr" />
                    <asp:TextBox ID="tbEmail" runat="server" Enabled="False" CssClass="input rightclr" />

                     <asp:Label ID="lblNumberOfTimesSent" runat="server" Text="Number of Times Pin was Sent" CssClass="label leftclr" />
                    <asp:TextBox ID="tbNumberOfTimesSent" runat="server" Enabled="False" CssClass="input rightclr" />

                     <asp:Label ID="lblLastSentDate" runat="server" Text="Last Pin Send Date" CssClass="label leftclr" />
                    <asp:TextBox ID="tbLastSentDate" runat="server" Enabled="False" CssClass="input rightclr" />

                        <asp:Label ID="lblReadOnlyRejectComment" runat="server" Text="Reject Comment" CssClass="label leftclr" />
                    <asp:TextBox ID="tbReadOnlyRejectComment" runat="server" Enabled="False" CssClass="input rightclr" />

                     <asp:Label ID="lblReadOnlyRejectDate" runat="server" Text="Reject Date" CssClass="label leftclr" />
                    <asp:TextBox ID="tbReadOnlyRejectDate" runat="server" Enabled="False" CssClass="input rightclr" />

                         <asp:Label ID="lblProductId" runat="server" Text="Product ID" Visible="false" CssClass="label leftclr" />
                    <asp:TextBox ID="tbProductId" runat="server" Enabled="False" Visible="false" CssClass="input rightclr" />

                         <asp:Label ID="lblMonthNumber" runat="server" Text="Month Number" Visible="false" CssClass="label leftclr" />
                    <asp:TextBox ID="tbMonthNumber" runat="server" Enabled="False" Visible="false" CssClass="input rightclr" />

                        <asp:TextBox ID="tbIssuerId" runat="server" Enabled="False" Visible="false" CssClass="input rightclr" />

                        <asp:TextBox ID="tbRequestStatus" runat="server" Enabled="False" Visible="false" CssClass="input rightclr" />

                    
                </asp:Panel>
            <br />
            <div style="width: 95%; float: left;">
             
                <asp:Label ID="lblRejectComments" runat="server" Text="Pin Reject Comment" CssClass="label leftclr" meta:resourcekey="lblSpoilCommentsResource" />
                <asp:TextBox ID="tbRejectComments" runat="server" CssClass="input rightclr" Style="width: 70%" />
            </div>


        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
            <asp:RegularExpressionValidator ID="revcontactnumber" runat="server"
                ErrorMessage="Contact number should valid in the form eg: (+233223344556) "
                ControlToValidate="tbContactnumber"
                ValidationExpression="^\+(?:[0-9]?){6,13}[0-9]$"
                ForeColor="Red">
            </asp:RegularExpressionValidator>
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnDecryptPin" runat="server" Text="Send Pin" OnClick="btnDecryptPin_Click" CssClass="button" />
            <asp:Button ID="btnResendPin" runat="server" Text="Resend Pin" OnClick="btnResendPin_Click" CssClass="button" />
            <asp:Button ID="btnRejectPin" runat="server" Text="Reject Request" OnClick="btnRejectPin_Click" CssClass="button" />
            <asp:Button ID="btnReissueApprove" runat="server" Text="Approve Re-issue" OnClick="btnApproveReissue_Click" CssClass="button" />
            <asp:Button ID="btnReissueReject" runat="server" Text="Reject Re-issue" OnClick="btnRejectReissue_Click" CssClass="button" />
             <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" OnClick="btnConfirm_Click" CssClass="button" />
             <asp:Button ID="btnConfirmReissue" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" OnClick="btnConfirmReissue_Click" CssClass="button" />
         
            <asp:Button ID="btnCancel" runat="server" Text="<%$ Resources: CommonLabels, Cancel %>" OnClick="btnCancel_Click" CssClass="button" CausesValidation="false" />
        </div>
    </div>

    
</asp:Content>

