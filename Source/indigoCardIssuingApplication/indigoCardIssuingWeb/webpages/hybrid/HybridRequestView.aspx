<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="HybridRequestView.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.hybrid.HybridRequestView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
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
            $('#dlgRequestStatus').puidialog({
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
                        $('#dlgRequestStatus').puidialog('hide');
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
                $('#dlgRequestStatus').puidialog('show');
            });
        }

        function hideStatus() {
            $(document).ready(function () {
                $('#dlgRequestStatus').puidialog('hide');
            });
        }
    </script>
    
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

     <div id="dlgRequestStatus" title="Request Status History">
                <div>
                    <asp:DataList ID="dlRequestStatus" runat="server"
                        HeaderStyle-Font-Bold="true"
                        Width="100%"
                        OnItemCommand="dlRequestStatus_ItemCommand"
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
                                    <asp:Label ID="lblStatusNameField" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "hybrid_request_statuses") %>' meta:resourcekey="lblStatusNameResource" />
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
                                    <asp:Label ID="lblDateCreated" runat="server" Text="Date Created" meta:resourcekey="lblDateCreatedResource" /></td>
                            </tr>
                        </HeaderTemplate>
                        <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                        <ItemTemplate>
                            <tr>
                                <td colspan="0">
                                    <asp:Label ID="lblPrintBatchId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "print_batch_id") %>' Visible="false" meta:resourcekey="lblDistBatchIdFieldResource" />
                                    <asp:Label ID="lblBatchRefeferenceField" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "print_batch_reference") %>' meta:resourcekey="lnkBatchRefeferenceField" /></td>
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
                <asp:Label ID="lblCardDetails" Text="Request Details" runat="server" meta:resourcekey="lblCardDetailsResource" />
            </div>
        
            <div style="width: 50%; float: left;">
                <asp:Panel ID="pnlRequestDetails" runat="server" GroupingText="Request Refernce Details" CssClass="bothclr" meta:resourcekey="lblrequestDetailsResource">
                    <asp:Label ID="lblRequestReference" runat="server" Text="Request Reference No" CssClass="label2col leftclr" meta:resourcekey="lblrequestRefNoResource" />
                    <asp:TextBox ID="tbRequestReference" runat="server" Enabled="False" CssClass="input2col rightclr" />

                    <asp:Label ID="lblCardNumber" runat="server" Text="Card Number" CssClass="label2col leftclr" meta:resourcekey="lblCardNumberResource" />
                    <asp:TextBox ID="tbCardNumber" runat="server" Enabled="False" CssClass="input2col rightclr" />

                    <asp:Label ID="lblProductName" runat="server" Text="Request Product" CssClass="label2col leftclr" meta:resourcekey="lblProductNameResource" />
                    <asp:TextBox ID="tbProductName" runat="server" Enabled="False" CssClass="input2col rightclr" />

                    <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label2col leftclr" />
                    <asp:TextBox ID="tbIssuer" runat="server" Enabled="False" CssClass="input2col rightclr" />

                    <asp:Label ID="lblRequestedBranch" runat="server" Text="Issuing Branch" CssClass="label2col leftclr" meta:resourcekey="lblIssuingBranchResource" />
                    <asp:TextBox ID="tbRequestedBranch" runat="server" Enabled="False" CssClass="input2col rightclr" />

                    <asp:Label ID="lblRequestPriority" runat="server" Text="Request Priority" CssClass="label2col leftclr" />
                    <asp:TextBox ID="tbRequestPriority" runat="server" Enabled="False" CssClass="input2col rightclr" />

                    <asp:Label ID="lblCardIssueMethod" runat="server" Text="Issue Method" CssClass="label2col leftclr" />
                    <asp:TextBox ID="tbCardIssueMethod" runat="server" Enabled="False" CssClass="input2col rightclr" />

                    <asp:Label ID="lblHybridCardStatus" runat="server" Text="Hybrid Card Status" CssClass="label2col leftclr" meta:resourcekey="lblhybridCardStatusResource" />
                    <asp:TextBox ID="tbHybridCardStatus" runat="server" Enabled="False" CssClass="input2col rightclr" />

                    <asp:Label ID="lblStatusDate" runat="server" Text="Last Updated" CssClass="label2col leftclr" />
                    <asp:TextBox ID="tbStatusDate" runat="server" Enabled="False" CssClass="input2col rightclr" />
                    
                    <div id="divBatchReference" runat="server">
                        <asp:Label ID="lblPrintBatchReference" runat="server" Text="Batch Reference" CssClass="label2col leftclr" />
                        <asp:Button ID="btnPrintBatchBatchReference" runat="server" Text="View Print Batch Reference" OnClick="btnBatchReference_Click" CssClass="input2col rightclr" />
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
            <asp:Panel ID="pnlIssuingDetails" runat="server" GroupingText="Request Details" CssClass="bothclr" meta:resourcekey="lblIssuingDetailsResource1">
                    <%--<asp:Label ID="lblIssuedDate" runat="server" Text="Date Issued" CssClass="label2col leftclr" meta:resourcekey="lblDateIssuedResource" />
                    <asp:TextBox ID="tbIssuedDate" runat="server" Enabled="False" CssClass="input2col rightclr" />

                    <asp:Label ID="lblIssuedBy" runat="server" Text="Issued By" CssClass="label2col leftclr" meta:resourcekey="lblIssuedByResource" />
                    <asp:TextBox ID="tbIssuedBy" runat="server" Enabled="False" CssClass="input2col rightclr" />--%>

                    <asp:Label ID="lblAccountNumber" runat="server" Text="Account Number" CssClass="label2col leftclr" meta:resourcekey="lblAccountNumberResource" />
                    <asp:TextBox ID="tbAccountNumber" runat="server" Enabled="False" CssClass="input2col rightclr" />

                    <asp:Label ID="lblDomicileBranch" runat="server" Text="Domicile Branch" CssClass="label2col leftclr" />
                    <asp:TextBox ID="tbDomicileBranch" runat="server" Enabled="False" CssClass="input2col rightclr" />

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
                    <div id="divrequestHistory" runat="server">
                        <asp:Label ID="lblCardHistory" runat="server" Text="Hybrid Request History" CssClass="label2col leftclr"></asp:Label>
                        <asp:Button ID="btnCardStatus" runat="server" Text="View Request History" OnClick="btnCardStatus_Click"  CssClass="input2col rightclr"/>
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
        <div id="pnlButtons" class="ButtonPanel" runat="server">
           
            <asp:Button ID="btnApprove" runat="server" Text="<%$ Resources: CommonLabels, Approve %>" CssClass="button" OnClick="btnApprove_Click" Visible="false" />
            <asp:Button ID="btnReject" runat="server" Text="<%$ Resources: CommonLabels, Reject %>" CssClass="button" OnClick="btnReject_Click" Visible="false" />
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" CssClass="button" OnClick="btnConfirm_OnClick" Visible="false" />

            <asp:Button ID="btnRefresh" runat="server" Text="<%$ Resources: CommonLabels, Refresh %>" CssClass="button" OnClick="btnRefresh_OnClick" />
            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources: CommonLabels, Back %>" CssClass="button" OnClick="btnBack_OnClick" />
        </div>
    
        </div>
</asp:Content>

