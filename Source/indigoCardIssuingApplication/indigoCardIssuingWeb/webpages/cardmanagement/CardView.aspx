<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="CardView.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.cardmanagement.CardView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function PrintPanel() {
            var hiddenSource = document.getElementById('<%= hdnNameOnCard.ClientID %>');
            var nameOnCard = hiddenSource.value;

            //var cardPrintStyle = document.getElementById('<%= hdnCardPrintStyle.ClientID %>');
            var cardPrintStyle = '';

            $.ajax({
                type: "POST",
                url: "CardView.aspx/GetPrintingHtml",
                data: '{token: "' + nameOnCard + '" }',
                contentType: "application/json; charset=utf-8",
                dataType: "json"
            }).done(function (data) {                
                cardPrintStyle = data;
                var printWindow = window.open('', '', 'height=800,width=1100');
                printWindow.document.write('<html><head><title>PRINT Card</title>');
                printWindow.document.write('</head><body style="height:204.02px; width:323.53px;">');
                printWindow.document.write(data.d);
                printWindow.document.write('</body></html>');
                printWindow.document.close();

                setTimeout(function () {
                    printWindow.print();
                }, 500);
            }).fail(function (data) {                
                alert(data.d);
            });

            
            return false;
        }

        $(document).ready(function () {
            $('#dlgBatchReference').puidialog({
                minimizable: false,
                maximizable: false,
                closable: false,
                closeOnEscape: false,
                width: 600,
                height: 250,
                minHeight: 600,
                minHeight: 250,
                modal: true,
                buttons: [{
                        text: 'Close',
                        click: function () {
                            $('#dlgBatchReference').puidialog('hide');
                        }
                    }
                ]
            });
        });

        function showReference() {
            $(document).ready(function () {
                $('#dlgBatchReference').puidialog('show');
            });
        }

        function hideReference() {
            $(document).ready(function () {
                $('#dlgBatchReference').puidialog('hide');
            });
        }

        $(document).ready(function () {
            $('#dlgCardStatus').puidialog({
                minimizable: false,
                maximizable: false,
                closable: false,
                closeOnEscape: false,
                width: 600,
                height: 250,
                minHeight: 600,
                minHeight: 250,
                modal: true,
                buttons: [{                    
                        text: 'Close',
                        click: function () {
                            $('#dlgCardStatus').puidialog('hide');
                        }
                    }
                ]
            });
        });

        function showAddFields() {
            $(document).ready(function () {
                $('#dlgAdditionalFields').puidialog('show');
            });
        }

        function hideAddFields() {
            $(document).ready(function () {
                $('#dlgAdditionalFields').puidialog('hide');
            });
        }

        $(document).ready(function () {
            $('#dlgAdditionalFields').puidialog({
                minimizable: false,
                maximizable: false,
                closable: false,
                closeOnEscape: false,
                width: 600,
                height: 250,
                minHeight: 600,
                minHeight: 250,
                modal: true,
                buttons: [{
                    text: 'Close',
                    click: function () {
                        $('#dlgAdditionalFields').puidialog('hide');
                    }
                }
                ]
            });
        });

        function showStatus() {
            $(document).ready(function () {
                $('#dlgCardStatus').puidialog('show');
            });
        }

        function hideStatus() {
            $(document).ready(function () {
                $('#dlgCardStatus').puidialog('hide');
            });
        }
    </script>

    <asp:HiddenField ID="hdnNameOnCard" runat="server" />
    <asp:HiddenField ID="hdnCardPrintStyle" runat="server" />

    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            
            <div id="dlgAdditionalFields" title="Additional Fields">
                <div>
                    <asp:DataList ID="dlFields" runat="server"
                        HeaderStyle-Font-Bold="true"
                        Width="100%"
                        CellPadding="0" ForeColor="#333333">
                        <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="White" />
                        <HeaderTemplate>
                            <tr style="font-weight: bold;">
                                <td colspan="0">
                                    <asp:Label ID="lblHFieldName" runat="server" Text="Field"/>
                                </td>
                                <td colspan="0">
                                    <asp:Label ID="lblHFieldLabel" runat="server" Text="Label" />
                                </td>
                                <td colspan="0">
                                    <asp:Label ID="lblHFieldValue" runat="server" Text="Value" />
                                </td>
                            </tr>
                        </HeaderTemplate>
                        <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                        <ItemTemplate>
                            <tr class="ItemSelect">
                                <td colspan="0">
                                    <asp:Label ID="lblFieldName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>'/>
                                </td>
                                <td colspan="0">
                                    <asp:Label ID="lblFieldLabel" runat="server" style="white-space: pre"
                                        Text='<%# DataBinder.Eval(Container.DataItem, "Label") %>' />
                                </td>
                                <td colspan="0">
                                    <asp:Label ID="lblFieldValue" runat="server" style="white-space: pre"
                                                Visible='<%#String.Format("{0}",DataBinder.Eval(Container.DataItem, "TextValue"))!="" %>' 
                                                Text='<%# DataBinder.Eval(Container.DataItem, "TextValue") %>' />
                                    <img src="<%# DataBinder.Eval(Container.DataItem, "ImageValue") %>" alt="" />                                    
                                </td>
                            </tr>
                        </ItemTemplate>
                        <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                    </asp:DataList>
                </div>
                <div class="ErrorPanel">
                    <asp:Label ID="Label1" runat="server" />
                </div>
                <div class="InfoPanel">
                    <asp:Label ID="Label2" runat="server" />
                </div>
            </div>

            <div id="dlgCardStatus" title="Card Status History">
                <div>
                    <asp:DataList ID="dlCardStatus" runat="server"
                        HeaderStyle-Font-Bold="true"
                        Width="100%"
                        OnItemCommand="dlCardStatus_ItemCommand"
                        CellPadding="0" ForeColor="#333333"
                        meta:resourcekey="dlCardStatusResource">
                        <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="White" />
                        <HeaderTemplate>
                            <tr style="font-weight: bold;">
                                <td colspan="0">
                                    <asp:Label ID="lblStatusName" runat="server" Text="Status" meta:resourcekey="lblStatusNameResource" /></td>
                                <td colspan="0">
                                    <asp:Label ID="lblStatusDate" runat="server" Text="Status Date" meta:resourcekey="lblStatusDateResource" /></td>
                                <td colspan="0">
                                    <asp:Label ID="lblStatusUser" runat="server" Text="User" meta:resourcekey="lblStatusUserResource" /></td>
                            </tr>
                        </HeaderTemplate>
                        <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                        <ItemTemplate>
                            <tr class="ItemSelect">
                                <td colspan="0">
                                    <asp:Label ID="lblStatusNameField" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "branch_card_statuses_name") %>' meta:resourcekey="lblStatusNameResource" />
                                </td>
                                <td colspan="0">
                                    <asp:Label ID="lblStatusDateField" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "status_date") %>' meta:resourcekey="lblStatusDateFieldResource" />
                                </td>
                                <td colspan="0">
                                    <asp:Label ID="lblStatusUserField" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "username") %>' meta:resourcekey="lblStatusUserResource" />
                                </td>
                            </tr>
                        </ItemTemplate>
                        <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                    </asp:DataList>
                </div>
                <div class="ErrorPanel">
                    <asp:Label ID="lblStatusError" runat="server" />
                </div>
                <div class="InfoPanel">
                    <asp:Label ID="lblStatusWarning" runat="server" />
                </div>
            </div>

            <div id="dlgBatchReference" title="Batch Reference Numbers">
                <div>
                    <asp:DataList ID="dlBatchReference" runat="server"
                        HeaderStyle-Font-Bold="true"
                        Width="100%" OnItemCommand="dlBatchReference_ItemCommand"
                        CellPadding="0" ForeColor="#333333"
                        meta:resourcekey="dlBatchReferenceResource">
                        <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="White" />
                        <HeaderTemplate>
                            <tr style="font-weight: bold;">
                                <td colspan="0">
                                    <asp:Label ID="lblBatchRefeference" runat="server" Text="Batch Reference" meta:resourcekey="lblBatchRefeference" /></td>
                                <td colspan="0">
                                    <asp:Label ID="lblBatchTypeName" runat="server" Text="Batch Type" meta:resourcekey="lblBatchTypeNameResource" /></td>
                                <td colspan="0">
                                    <asp:Label ID="lblDateCreated" runat="server" Text="Date Created" meta:resourcekey="lblDateCreatedResource" /></td>
                            </tr>
                        </HeaderTemplate>
                        <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                        <ItemTemplate>
                            <tr>
                                <td colspan="0">
                                    <asp:Label ID="lblDistBatchId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "dist_batch_id") %>' Visible="false" meta:resourcekey="lblDistBatchIdFieldResource" />
                                    <asp:Label ID="lblBatchRefeferenceField" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "dist_batch_reference") %>' meta:resourcekey="lnkBatchRefeferenceField" /></td>
                                </td>
                                <td colspan="0">
                                    <asp:Label ID="lblBatchTypeNameField" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "dist_batch_type_name") %>' meta:resourcekey="lblBatchTypeNameFieldResource" />
                                </td>
                                <td colspan="0">
                                    <asp:Label ID="lblDateCreatedField" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "date_created") %>' meta:resourcekey="lblDateCreatedFieldResource" />
                                </td>
                            </tr>
                        </ItemTemplate>
                        <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                    </asp:DataList>
                </div>
                <div class="ErrorPanel">
                    <asp:Label ID="lblReferenceError" runat="server" />
                </div>
                <div class="InfoPanel">
                    <asp:Label ID="lblReferenceInfo" runat="server" />
                </div>
            </div>

            <div class="ContentHeader">
                <asp:Label ID="lblCardDetails" Text="Card Details" runat="server" meta:resourcekey="lblCardDetailsResource" />
            </div>

            <div style="width: 50%; float: left;">
                <asp:Panel ID="pnlCardDetails" runat="server" GroupingText="Card Details" CssClass="bothclr" meta:resourcekey="lblCardDetailsResource">
                    <asp:Label ID="lblCardReference" runat="server" Text="Card Reference No" CssClass="label2col leftclr" meta:resourcekey="lblCardRefNoResource" />
                    <asp:TextBox ID="tbCardReference" runat="server" Enabled="False" CssClass="input2col rightclr" />

                    <asp:Label ID="lblCardNumber" runat="server" Text="Card Number" CssClass="label2col leftclr" meta:resourcekey="lblCardNumberResource" />
                    <asp:TextBox ID="tbCardNumber" runat="server" Enabled="False" CssClass="input2col rightclr" />

                    <asp:Label ID="lblProductName" runat="server" Text="Card Product" CssClass="label2col leftclr" meta:resourcekey="lblProductNameResource" />
                    <asp:TextBox ID="tbProductName" runat="server" Enabled="False" CssClass="input2col rightclr" />

                    <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label2col leftclr" />
                    <asp:TextBox ID="tbIssuer" runat="server" Enabled="False" CssClass="input2col rightclr" />

                    <asp:Label ID="lblIssuingBranch" runat="server" Text="Issuing Branch" CssClass="label2col leftclr" meta:resourcekey="lblIssuingBranchResource" />
                    <asp:TextBox ID="tbIssuingBranch" runat="server" Enabled="False" CssClass="input2col rightclr" />

                    <asp:Label ID="lblCardPriority" runat="server" Text="Card Priority" CssClass="label2col leftclr" />
                    <asp:TextBox ID="tbCardPriority" runat="server" Enabled="False" CssClass="input2col rightclr" />

                    <asp:Label ID="lblCardIssueMethod" runat="server" Text="Issue Method" CssClass="label2col leftclr" />
                    <asp:TextBox ID="tbCardIssueMethod" runat="server" Enabled="False" CssClass="input2col rightclr" />

                    <asp:Label ID="lblBranchCardStatus" runat="server" Text="Branch Card Status" CssClass="label2col leftclr" meta:resourcekey="lblBrachCardStatusResource" />
                    <asp:TextBox ID="tbBranchCardStatus" runat="server" Enabled="False" CssClass="input2col rightclr" />

                    <asp:Label ID="lblStatusDate" runat="server" Text="Last Updated" CssClass="label2col leftclr" />
                    <asp:TextBox ID="tbStatusDate" runat="server" Enabled="False" CssClass="input2col rightclr" />
                    
                    <div id="divBatchReference" runat="server">
                        <asp:Label ID="lblBatchReference" runat="server" Text="Batch Reference" CssClass="label2col leftclr" />
                        <asp:Button ID="btnBatchReference" runat="server" Text="View Batch Reference" OnClick="btnBatchReference_Click" CssClass="input2col rightclr" />
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlCustomerDetails" runat="server" GroupingText="Customer Details" CssClass="bothclr" meta:resourcekey="lblCustomerDetailsResource1">

                    <div id="divuserTitle" runat="server">
                        <asp:Label ID="userTitle" runat="server" Text="Title" CssClass="label2col leftclr" meta:resourcekey="userTitleResource" />
                        <asp:TextBox ID="tbTitle" runat="server" Enabled="False" CssClass="input2col rightclr" />
                    </div>
                    <div id="divFirstName" runat="server">
                        <asp:Label ID="lblFirstNames" runat="server" Text="First Names" CssClass="label2col leftclr" meta:resourcekey="lblFirstNamesResource" />
                        <asp:TextBox ID="tbFirstName" runat="server" Enabled="False" CssClass="input2col rightclr" />
                    </div>
                    <div id="divMiddleName" runat="server">
                        <asp:Label ID="lblMiddleName" runat="server" Text="Middle Name" CssClass="label2col leftclr" meta:resourcekey="lblMiddleNameResource" />
                        <asp:TextBox ID="tbMiddleName" runat="server" Enabled="False" CssClass="input2col rightclr" />
                    </div>
                    <div id="divLastName" runat="server">
                        <asp:Label ID="lblLastName" runat="server" Text="Last Name" CssClass="label2col leftclr" meta:resourcekey="lblLastNameResource" />
                        <asp:TextBox ID="tbLastName" runat="server" Enabled="False" CssClass="input2col rightclr" />
                    </div>
                    <div id="divContactnumber" runat="server">
                        <asp:Label ID="lblContactNumber" runat="server" Text="Contact Number" CssClass="label2col leftclr" meta:resourcekey="lblContactNumberResource" />
                        <asp:TextBox ID="tbContactNumber" runat="server" Enabled="False" CssClass="input2col rightclr" />
                    </div>
                    <div id="divIDNumber" runat="server">
                        <asp:Label ID="lblIDNumber" runat="server" Text="ID/Passport Number" CssClass="label2col leftclr" meta:resourcekey="lblIDNumberResource" />
                        <asp:TextBox ID="tbIdNumber" runat="server" CssClass="input2col rightclr" MaxLength="50" Enabled="False" />
                    </div>
                </asp:Panel>
            </div>

            <div style="width: 50%; float: right;">
                <asp:Panel ID="pnlIssuingDetails" runat="server" GroupingText="Issuing Details" CssClass="bothclr" meta:resourcekey="lblIssuingDetailsResource1">
                    <asp:Label ID="lblDateIssued" runat="server" Text="Date Issued" CssClass="label2col leftclr" meta:resourcekey="lblDateIssuedResource" />
                    <asp:TextBox ID="tbDateIssued" runat="server" Enabled="False" CssClass="input2col rightclr" />

                    <asp:Label ID="lblIssuedBy" runat="server" Text="Issued By" CssClass="label2col leftclr" meta:resourcekey="lblIssuedByResource" />
                    <asp:TextBox ID="tbIssuedBy" runat="server" Enabled="False" CssClass="input2col rightclr" />

                    <asp:Label ID="lblAccountNumber" runat="server" Text="Account Number" CssClass="label2col leftclr" meta:resourcekey="lblAccountNumberResource" />
                    <asp:TextBox ID="tbAccountNumber" runat="server" Enabled="False" CssClass="input2col rightclr" />

                    <asp:Label ID="lblDomicileBranch" runat="server" Text="Domicile Branch" CssClass="label2col leftclr" />
                    <asp:TextBox ID="tbDomicileBranch" runat="server" Enabled="False" CssClass="input2col rightclr" />

                    <div id="divReason" runat="server" class="bothclr">
                        <asp:Label ID="lblReasonForIssue" runat="server" Text="Reason For Issue" CssClass="label2col leftclr" meta:resourcekey="lblReasonForIssueResource" />
                        <asp:TextBox ID="tbReasonForIssue" runat="server" Enabled="False" CssClass="input2col rightclr" />
                    </div>

                    <div id="divAccountType" runat="server">
                        <asp:Label ID="lblAccountType" runat="server" Text="Account Type" CssClass="label2col leftclr" meta:resourcekey="lblAccountTypeResource" />
                        <asp:TextBox ID="tbAccountType" runat="server" Enabled="False" CssClass="input2col rightclr" />
                    </div>
                    <div id="divCustomerType" runat="server">
                        <asp:Label ID="lblCustomerType" runat="server" Text="Customer Type" CssClass="label2col leftclr" meta:resourcekey="lblCustomerTypeResource" />
                        <asp:TextBox ID="tbCustomerType" runat="server" Enabled="False" CssClass="input2col rightclr" />
                    </div>
                    <div id="divCurrency" runat="server">
                        <asp:Label ID="lblCurrency" runat="server" Text="Currency" CssClass="label2col leftclr" meta:resourcekey="lblCurrencyResource" />
                        <asp:TextBox ID="tbCurrency" runat="server" Enabled="False" CssClass="input2col rightclr" />
                    </div>
                    <div id="divResident" runat="server">
                        <asp:Label ID="lblResident" runat="server" Text="Resident" CssClass="label2col leftclr" meta:resourcekey="lblResidentResource" />
                        <asp:TextBox ID="tbResident" runat="server" Enabled="False" CssClass="input2col rightclr" />
                    </div>
                    <div id="divCustomerID" runat="server">
                        <asp:Label ID="lblContractId" runat="server" Text="CMS Client ID" CssClass="label2col leftclr" meta:resourcekey="lblContractIdResource" />
                        <asp:TextBox ID="tbContractId" runat="server" Enabled="False" CssClass="input2col rightclr" />
                    </div>
                    <div id="divContractNumber" runat="server">
                        <asp:Label ID="lblContractNumber" runat="server" Text="CMS Agreement #" CssClass="label2col leftclr" meta:resourcekey="lblCmsagreementResource" />
                        <asp:TextBox ID="tbContractNumber" runat="server" Enabled="False" CssClass="input2col rightclr" />
                    </div>
                    <div id="divProductFields" runat="server">
                        <asp:Label ID="lblAddFields" runat="server" Text="Additional Fields" CssClass="label2col leftclr"></asp:Label>
                        <asp:Button ID="btnAddFields" runat="server" Text="View" OnClick="btnAddFields_Click"  CssClass="input2col rightclr"/>
                    </div>
                    <div id="divCardHistory" runat="server">
                        <asp:Label ID="lblCardHistory" runat="server" Text="Card History" CssClass="label2col leftclr"></asp:Label>
                        <asp:Button ID="btnCardStatus" runat="server" Text="View Card History" OnClick="btnCardStatus_Click"  CssClass="input2col rightclr"/>
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnlFeeDetails" runat="server" GroupingText="Fee Details" CssClass="bothclr">
                    <asp:Label ID="lblFee" runat="server" Text="Fee" CssClass="label2col leftclr" />
                    <asp:TextBox ID="tbFee" runat="server" Enabled="False" CssClass="input2col rightclr" />
                    <asp:Label ID="lblFeeRef" runat="server" Text="Fee Reference No" CssClass="label2col leftclr" />
                    <asp:TextBox ID="tbFeeRefNo" runat="server" Enabled="False" CssClass="input2col rightclr" />
                    <asp:Label ID="lblFeeReversalRef" runat="server" Text="Reversal Ref No" CssClass="label2col leftclr" />
                    <asp:TextBox ID="tbFeeRevNo" runat="server" Enabled="False" CssClass="input2col rightclr" />
                    <asp:CheckBox ID="chkFeeWaived" runat="server" Enabled="false" Text="Waived" CssClass="label2col" />
                    <asp:CheckBox ID="chkFeeOverridden" runat="server" Enabled="false" Text="Overridden" CssClass="input2col" />
                </asp:Panel>
            </div>

            <div style="width: 95%; float: left;">
                <asp:Label ID="lblSpoilReason" runat="server" Text="Reason" CssClass="label leftclr" meta:resourcekey="lblSpoilReasonResource" />
                <asp:DropDownList ID="ddlSpoilReason" runat="server" CssClass="input rightclr" />

                <asp:Label ID="lblSpoilComments" runat="server" Text="Comments" CssClass="label leftclr" meta:resourcekey="lblSpoilCommentsResource" />
                <asp:TextBox ID="tbSpoilComments" runat="server" CssClass="input rightclr" Style="width: 70%" />
            </div>

        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlPrintButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnPrint" runat="server" Text="Print Card" CssClass="button" OnClick="btnPrint_OnClick" OnClientClick="return PrintPanel();" Visible="false" meta:resourcekey="btnPrintResource" />
            <asp:Button ID="btnPrintSuccess" runat="server" Text="Print Success" CssClass="button" OnClick="btnPrintSuccess_OnClick" Visible="false" meta:resourcekey="btnPrintSuccessResource" />
            <asp:Button ID="btnPrintFailed" runat="server" Text="Print Failed" CssClass="button" OnClick="btnPrintFailed_OnClick" Visible="false" meta:resourcekey="btnPrintFailedResource" />
        </div>

        <div id="pnlCmsButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnLinkCard" runat="server" Text="Upload Card Details" CssClass="button" OnClick="btnLinkCard_OnClick" Visible="false" meta:resourcekey="btnLinkCardResource" />
            <asp:Button ID="btnManualLink" runat="server" Text="Manual Card Link" CssClass="button" OnClick="btnManualLink_OnClick" Visible="false" meta:resourcekey="btnManualLinkResource" />
            <asp:Button ID="btnReuploadCard" runat="server" Text="Re-Upload Details" CssClass="button" OnClick="btnReuploadCard_OnClick" Visible="false" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnReprintPinMailer" runat="server" Text="PIN Reprint" CssClass="button" Visible="false" OnClick="btnReprintPinMailer_OnClick" />
            <asp:Button ID="btnReprintPinMailerApprove" runat="server" Text="Approve PIN Reprint" CssClass="button" Visible="false" OnClick="btnReprintPinMailerApprove_OnClick" />
            <asp:Button ID="btnReprintPinMailerReject" runat="server" Text="Reject PIN Reprint" CssClass="button" Visible="false" OnClick="btnReprintPinMailerReject_OnClick" />
            <asp:Button ID="btnApprove" runat="server" Text="<%$ Resources: CommonLabels, Activate %>" CssClass="button" OnClick="btnApprove_Click" Visible="false" />
            <asp:Button ID="btnReject" runat="server" Text="<%$ Resources: CommonLabels, Stop %>" CssClass="button" OnClick="btnReject_Click" Visible="false" />
            <asp:Button ID="btnSpoil" runat="server" Text="<%$ Resources: CommonLabels, Block %>" CssClass="button" OnClick="btnSpoil_OnClick" Visible="false" />
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" CssClass="button" OnClick="btnConfirm_OnClick" Visible="false" />

            <asp:Button ID="btnRefresh" runat="server" Text="<%$ Resources: CommonLabels, Refresh %>" CssClass="button" OnClick="btnRefresh_OnClick" />
            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources: CommonLabels, Back %>" CssClass="button" OnClick="btnBack_OnClick" />
        </div>
    </div>
</asp:Content>
