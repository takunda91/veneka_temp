<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/IndigoCardIssuance.Master" CodeBehind="PinMailerView.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.pin.PinMailerView" Culture="auto:en-US" meta:resourcekey="PageResource" UICulture="auto" %>


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
                <asp:Label ID="lblPinMailerDetails" runat="server" Text="Pin Mailer Details" meta:resourcekey="lblPinMailerDetails" />
            </div>
                    <asp:Panel ID="pnlCardDetails" runat="server" GroupingText="Pin Mailer Details" CssClass="bothclr" meta:resourcekey="lblCardDetailsResource">
                  
                        <asp:Label ID="lblCardId" runat="server" Text="Batch Reference" CssClass="label leftclr"  meta:resourcekey="lblCardIdResource" />
                    <asp:TextBox ID="tbBatchReference" runat="server" Enabled="False" CssClass="input rightclr"  />

                      

                    <asp:Label ID="lblNumberOfCards" runat="server" Text="Number Of Cards On Request" CssClass="label leftclr" meta:resourcekey="lblPinBlockResource" />
                    <asp:TextBox ID="tbNumberOfCards" runat="server" Enabled="False" CssClass="input rightclr" />

                          
                   <asp:Label ID="lblUploadDate" runat="server" Text="Pin File Upload Date" CssClass="label leftclr" />
                    <asp:TextBox ID="tbUploadDate" runat="server" Enabled="False" CssClass="input rightclr" />

                        <asp:Label ID="lblApprovalStatus" runat="server" Text="Approval Status" CssClass="label leftclr" />
                    <asp:TextBox ID="tbApprovalStatus" runat="server" Enabled="False" CssClass="input rightclr" />

                        <asp:Label ID="lblApprovalDate" runat="server" Text="Approval Date" CssClass="label leftclr" />
                    <asp:TextBox ID="tbApprovalDate" runat="server" Enabled="False" CssClass="input rightclr" />


                        <asp:TextBox ID="tbIssuerId" runat="server" Enabled="False" Visible="false" CssClass="input rightclr" />
                         <asp:TextBox ID="tbheaderid" runat="server" Enabled="False" Visible="false" CssClass="input rightclr" />

                </asp:Panel>
            <br />
            <div style="width: 95%; float: left;">
             
                <asp:Label ID="lblApprovalComments" runat="server" Text="Pin Batch Approval Comment" CssClass="label leftclr" meta:resourcekey="lblSpoilCommentsResource" />
                <asp:TextBox ID="tbApprovalComments" runat="server" CssClass="input rightclr" Style="width: 70%" />
            </div>


        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
            
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnApprove" runat="server" Text="Approve Batch" OnClick="btnApprove_Click" CssClass="button" />
            <asp:Button ID="btnReject" runat="server" Text="Reject Batch" OnClick="btnReject_Click" CssClass="button" />
             <asp:Button ID="btnGenerateReport" runat="server" Text="Generate Report" Visible="true" OnClick="btnGenerateReport_Click" CssClass="button" />
           
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" OnClick="btnConfirm_Click" CssClass="button" />
            <asp:Button ID="btnCancel" runat="server" Text="<%$ Resources: CommonLabels, Cancel %>" OnClick="btnCancel_Click" CssClass="button" CausesValidation="false" />
        </div>
    </div>

    
</asp:Content>

