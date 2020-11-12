<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="CentralCard.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.card.CentralCard" Culture="auto:en-US" meta:resourcekey="PageResource" UICulture="auto" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {

            $('#<%= ddlIssuer.ClientID %>').puidropdown({
                effectSpeed: 1,
                change: function () {
                    __doPostBack('<%= ddlIssuer.ClientID %>', '')
                }
            });

            $('#<%= ddlBranch.ClientID %>').puidropdown({
                effectSpeed: 1,
                change: function () {
                    __doPostBack('<%= ddlBranch.ClientID %>', '')
                }
            });

            $('#<%= ddlProduct.ClientID %>').puidropdown({
                effectSpeed: 1,
                change: function () {
                    __doPostBack('<%= ddlProduct.ClientID %>', '')
                }
            });

            $('#<%= ddlPriority.ClientID %>').puidropdown({
                effectSpeed: 1
            });

            $('#<%= ddlReasonForIssue.ClientID %>').puidropdown({
                effectSpeed: 1,
                change: function () {
                    __doPostBack('<%= ddlReasonForIssue.ClientID %>', '')
                }
            });

            $('#<%= ddlFeeType.ClientID %>').puidropdown({
                effectSpeed: 1,
                change: function () {
                    __doPostBack('<%= ddlFeeType.ClientID %>', '')
                }
            });

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

    </script>

    <asp:HiddenField ID="hdnCardIssueMethod" runat="server" />
    <asp:HiddenField ID="hdnCMSAccounType" runat="server" />
    <asp:HiddenField ID="hdnCBSAccounType" runat="server" />
    <asp:HiddenField ID="hdnGuid" ClientIDMode="Static" runat="server" />

    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblCardAndCustomerDetails" runat="server" Text="Card And Customer Details" meta:resourcekey="lblCardAndCustomerDetailsResource" />
            </div>

            <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlIssuer" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlIssuer_OnSelectedIndexChanged" CssClass="input rightclr" />

            <asp:Label ID="lblBranch" runat="server" Text="<%$ Resources: CommonLabels, Branch %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlBranch" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlBranch_OnSelectedIndexChanged" CssClass="input rightclr" />

            <asp:Label ID="lblCardProduct" runat="server" Text="<%$ Resources: CommonLabels, Product %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlProduct" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlProduct_OnSelectedIndexChanged" CssClass="input rightclr" />

            <asp:Label ID="lblPriority" runat="server" Text="Priority" CssClass="label leftclr" Visible="false" meta:resourcekey="lblPriorityResource" />
            <asp:DropDownList ID="ddlPriority" runat="server" CssClass="input rightclr" Visible="false" />

            <%--            <asp:Label ID="lblCardNumber" runat="server" Text="Card number" CssClass="label leftclr" Visible="false" meta:resourcekey="lblCardNumberResource" />
            <asp:DropDownList ID="ddlCardNumber" runat="server" Visible="false" Style="margin-bottom: 10px !important;" />
            <asp:RequiredFieldValidator runat="server" ID="rvfCardNumber" ControlToValidate="ddlCardNumber" InitialValue="-99" 
                    ErrorMessage="Please select card." CssClass="validation rightclr"  /> --%>

            <div id="divReason" runat="server" class="bothclr">
                <asp:Label ID="lblReason" runat="server" Text="Reason for issue" CssClass="label leftclr" Visible="false" meta:resourcekey="lblReasonResource" />
                <asp:DropDownList ID="ddlReasonForIssue" runat="server" Visible="false" CssClass="puidropdown" AutoPostBack="true" OnSelectedIndexChanged="ddlReasonForIssue_SelectedIndexChanged" />
                <asp:RequiredFieldValidator runat="server" ID="reqReason" ControlToValidate="ddlReasonForIssue" Visible="false" InitialValue="-99"
                    ErrorMessage="Please select reason." CssClass="validation rightclr" meta:resourcekey="reqReasonResource" />
            </div>

            <asp:Label ID="lblDomBranch" runat="server" Text="Domicile Branch" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlDomBranch" runat="server" CssClass="input rightclr puidropdown" />

            <asp:Label ID="lblDelBranch" runat="server" Text="Delivery Branch" Visible="false" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlDelBranch" runat="server" Visible="false" CssClass="input rightclr puidropdown" />

            <div id="pnlAccountNumber" runat="server" class="bothclr" visible="false" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="lblAccountNumber" runat="server" Text="Account number" CssClass="label leftclr" meta:resourcekey="lblAccountNumberResource" />
                <asp:TextBox ID="tbAccountNumber" runat="server" CssClass="input" MaxLength="27" />
                <asp:Button ID="btnValidateAccount" runat="server" Text="Validate" meta:resourcekey="btnValidateAccountResource"
                    CssClass="button" Style="margin: 0px 0px 0px 5px !important; float: left;"
                    OnClick="btnValidateAccount_Click" />

                <asp:RequiredFieldValidator runat="server" ID="reqAccNo" ControlToValidate="tbAccountNumber" meta:resourcekey="reqAccNoResource"
                    ErrorMessage="Please enter account number." CssClass="validation rightclr" />
            </div>

            <asp:Panel ID="pnlCustomerDetail" runat="server" Visible="false">

                <div id="divNameOnCard" runat="server">
                    <asp:Label ID="lblNameOnCard" runat="server" Text="Name on Card" CssClass="label leftclr" meta:resourcekey="lblNameOnCardResource" />
                    <asp:TextBox ID="tbNameOnCard" runat="server" CssClass="input" MaxLength="25" Style="text-transform: uppercase;" />
                    <asp:RequiredFieldValidator runat="server" ID="reqNameOnCard" ControlToValidate="tbNameOnCard"
                        ErrorMessage="Please enter name on card." CssClass="validation rightclr" meta:resourcekey="reqNameOnCardResource" />
                </div>
                <div id="divPrintFields" runat="server">
                </div>
                <div id="divuserTitle" runat="server">
                    <asp:Label ID="userTitle" runat="server" Text="Title" CssClass="label leftclr" meta:resourcekey="userTitleResource" />
                    <asp:DropDownList ID="ddlTitle" runat="server" CssClass="puidropdown" />
                    <asp:RequiredFieldValidator runat="server" ID="requserTitle" ControlToValidate="ddlTitle" InitialValue="-99"
                        ErrorMessage="Please enter user title." CssClass="validation rightclr" meta:resourcekey="requserTitleResource" />
                </div>
                <div id="divFirstName" runat="server">
                    <asp:Label ID="lblFirstName" runat="server" Text="First name" CssClass="label leftclr" meta:resourcekey="lblFirstNameResource" />
                    <asp:TextBox ID="tbFirstName" runat="server" CssClass="input" MaxLength="50" />
                    <asp:RequiredFieldValidator runat="server" ID="reqFirstName" ControlToValidate="tbFirstName"
                        ErrorMessage="Please enter customer first name." CssClass="validation rightclr" meta:resourcekey="reqFirstNameResource" />
                </div>
                <div id="divMiddleName" runat="server">
                    <asp:Label ID="lblMiddleName" runat="server" Text="Middle name" CssClass="label leftclr" meta:resourcekey="lblMiddleNameResource" />
                    <asp:TextBox ID="tbMiddleName" runat="server" CssClass="input rightclr" MaxLength="50" />
                    <asp:RequiredFieldValidator runat="server" ID="reqMiddleName" ControlToValidate="tbMiddleName"
                        ErrorMessage="Please enter customer middel name." CssClass="validation rightclr" meta:resourcekey="reqMiddleNameCardResource" />
                </div>
                <div id="divLastName" runat="server">
                    <asp:Label ID="lblLastName" runat="server" Text="Last name" CssClass="label leftclr" meta:resourcekey="lblLastNameResource" />
                    <asp:TextBox ID="tbLastName" runat="server" CssClass="input" MaxLength="50" />
                    <asp:RequiredFieldValidator runat="server" ID="reqLastName" ControlToValidate="tbLastName"
                        ErrorMessage="Please enter customer last name." CssClass="validation rightclr" meta:resourcekey="reqLastNameResource" />
                </div>
                <div id="divContactnumber" runat="server">
                    <asp:Label ID="lblcontactnumber" runat="server" Text="ContactNumber" CssClass="label leftclr" meta:resourcekey="lblcontactnumberResource" />
                    <asp:TextBox ID="tbContactnumber" runat="server" CssClass="input rightclr" MaxLength="50" OnKeyPress="return isNumberKeyWithPlus(event)" />
                    <asp:RequiredFieldValidator runat="server" ID="reqcontactnumber" ControlToValidate="tbcontactnumber"
                        ErrorMessage="Please enter contact number." CssClass="validation rightclr" meta:resourcekey="reqcontactnumberResource"></asp:RequiredFieldValidator>

                </div>
                <div id="divIDNumber" runat="server">
                    <asp:Label ID="lblIDNumber" runat="server" Text="ID/Passport Number" CssClass="label leftclr" meta:resourcekey="lblIDNumberResource" />
                    <asp:TextBox ID="tbIDNumber" runat="server" CssClass="input rightclr" MaxLength="50" />
                    <asp:RequiredFieldValidator runat="server" ID="reqIDNumber" ControlToValidate="tbIdNumber"
                        ErrorMessage="Please enter ID Number." CssClass="validation rightclr" meta:resourcekey="reqIDNumberResource"></asp:RequiredFieldValidator>
                </div>
                <div id="divAccountType" runat="server">
                    <asp:Label ID="lblAccountType" runat="server" Text="Account type" CssClass="label leftclr" meta:resourcekey="lblAccountTypeResource" />
                    <asp:DropDownList ID="ddlAccountType" runat="server" CssClass="puidropdown" />
                    <asp:RequiredFieldValidator runat="server" ID="reqAccountType" ControlToValidate="ddlAccountType" InitialValue="-99"
                        ErrorMessage="Please select account type." CssClass="validation rightclr" meta:resourcekey="reqAccountTypeResource"></asp:RequiredFieldValidator>
                </div>
                <div id="divCurrency" runat="server">
                    <asp:Label ID="lblCurrency" runat="server" Text="Account Currency" CssClass="label leftclr" meta:resourcekey="lblCurrencyResource" />
                    <asp:DropDownList ID="ddlCurrency" runat="server" CssClass="puidropdown" OnSelectedIndexChanged="ddlCurrency_SelectedIndexChanged" AutoPostBack="true" />
                    <asp:RequiredFieldValidator runat="server" ID="reqCurrency" ControlToValidate="ddlCurrency" InitialValue="-99"
                        ErrorMessage="Please select currency." CssClass="validation rightclr" meta:resourcekey="reqCurrencyResource"></asp:RequiredFieldValidator>
                </div>
                <div id="divCustomerType" runat="server">
                    <asp:Label ID="lblCustomerType" runat="server" Text="Customer Type" CssClass="label leftclr" meta:resourcekey="lblCustomerTypeResource" />
                    <asp:DropDownList ID="ddlCustomerType" runat="server" CssClass="puidropdown" />
                    <asp:RequiredFieldValidator runat="server" ID="reqCustomerType" ControlToValidate="ddlCustomerType" InitialValue="-99"
                        ErrorMessage="Please select cusomter type." CssClass="validation rightclr" meta:resourcekey="reqCustomerTypeResource"></asp:RequiredFieldValidator>
                </div>
                <div id="divResident" runat="server">
                    <asp:Label ID="lblResident" runat="server" Text="Resident" CssClass="label leftclr" meta:resourcekey="lblResidentResource" />
                    <asp:DropDownList ID="ddlResident" runat="server" CssClass="puidropdown" />
                    <asp:RequiredFieldValidator runat="server" ID="reqResident" ControlToValidate="ddlResident" InitialValue="-99"
                        ErrorMessage="Please select resident." CssClass="validation rightclr" meta:resourcekey="reqResidentResource"></asp:RequiredFieldValidator>
                </div>
                <div id="divCustomerID" runat="server">
                    <asp:Label ID="lblCustomerID" runat="server" Text="CMS Client ID" CssClass="label leftclr" meta:resourcekey="LabelCustomerIDResource" />
                    <asp:TextBox ID="tbCustomerID" runat="server" MaxLength="8" CssClass="input" OnKeyPress="return isNumberKey(event)" />
                    <asp:RequiredFieldValidator runat="server" ID="reqCustomerID" ControlToValidate="tbCustomerID" ForeColor="Red"
                        ErrorMessage="Please enter client ID." CssClass="validation rightclr" meta:resourcekey="rfvCustomerIDResource" />
                </div>
                <div id="divContractNumber" runat="server">
                    <asp:Label ID="lblContractNumber" runat="server" Text="CMS Agreement Number" CssClass="label leftclr" meta:resourcekey="lblContractNumberResource" />
                    <asp:TextBox ID="tbContractNumber" runat="server" Enabled="False" MaxLength="15" CssClass="input" OnKeyPress="return isNumberKey(event)" />
                    <asp:RequiredFieldValidator runat="server" ID="reqContractNumber" ControlToValidate="tbContractNumber" Enabled="false" ForeColor="Red"
                        ErrorMessage="Please enter Agreement number." CssClass="validation rightclr" meta:resourcekey="rfvContractNumberResource" />
                </div>
                <div id="divInternalAccountNo" runat="server">
                    <asp:Label ID="lblInternalAccountNo" runat="server" Text="Internal Account No" CssClass="label leftclr" />
                    <asp:TextBox ID="tbInternalAccountNo" runat="server" Enabled="False" MaxLength="15" CssClass="input" OnKeyPress="return isNumberKey(event)" />
                    <asp:RequiredFieldValidator runat="server" ID="reqInternalAccountNo" ControlToValidate="tbInternalAccountNo" Enabled="false" ForeColor="Red"
                        ErrorMessage="Please enter internal account number." CssClass="validation rightclr" />
                </div>
                <div id="divPassporttype" runat="server">
                    <asp:Label ID="lblPassporttype" runat="server" Text="Passport Type" Visible="false" CssClass="label leftclr" />
                    <asp:DropDownList ID="ddlPassporttype" runat="server" Visible="false" CssClass="input rightclr puidropdown" />
                    <asp:RequiredFieldValidator runat="server" ID="reqPassporttype" ControlToValidate="ddlPassporttype"
                        ErrorMessage="Please enter passport type." CssClass="validation rightclr" />
                </div>

            </asp:Panel>

            <asp:Panel ID="pnlCardFees" runat="server">
                <asp:Label ID="lblFeeType" runat="server" Text="Fee Type" CssClass="label leftclr" />
                <asp:DropDownList ID="ddlFeeType" runat="server" OnSelectedIndexChanged="ddlFeeType_OnSelectedIndexChanged"
                    AutoPostBack="true" CssClass="input righclr" />

                <asp:Label ID="lblOverrideFee" runat="server" Text="Override Fee" CssClass="label leftclr" />
                <asp:CheckBox ID="chkOverrideFee" runat="server" CssClass="input rightclr" OnCheckedChanged="chkOverrideFee_CheckedChanged" AutoPostBack="true" />

                <asp:Label ID="lblWaiveFee" runat="server" Text="Waive Fee" CssClass="label leftclr" />
                <asp:CheckBox ID="chkWaiveFee" runat="server" CssClass="input rightclr" OnCheckedChanged="chkWaiveFee_CheckedChanged" AutoPostBack="true" />

                <asp:Label ID="lblApplicableFee" runat="server" Text="Card Fee" CssClass="label leftclr" />
                <asp:TextBox ID="tbApplicableFee" runat="server" Enabled="false" CssClass="input rightclr" MaxLength="10" OnKeyPress="return isNumericValue(event,this,6,4)" />

                <asp:Label ID="lblVatRate" runat="server" Text="VAT" CssClass="label leftclr" />
                <asp:TextBox ID="tbVatRate" runat="server" Enabled="false" CssClass="input rightclr" MaxLength="10" />

                <asp:Label ID="lblTotalFee" runat="server" Text="Total Fee" CssClass="label leftclr" />
                <asp:TextBox ID="tbTotalFee" runat="server" Enabled="false" CssClass="input rightclr" MaxLength="10" />
            </asp:Panel>

            <asp:Panel ID="pnlOther" runat="server" Visible="false" CssClass="bothclr">
                <asp:Label ID="lblComments" runat="server" Text="Comments" CssClass="bothclr" />
                <asp:TextBox ID="tbComments" runat="server" CssClass="bothclr" Enabled="false" TextMode="MultiLine"
                    Style="overflow: hidden; width: 80%; height: 56px" />
            </asp:Panel>
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
            <asp:RegularExpressionValidator ID="revcontactnumber" runat="server"
                ErrorMessage="Please Enter Valid Number. eg: (+233223344556) "
                ControlToValidate="tbContactnumber"
                ValidationExpression="^\+(?:[0-9]?){6,13}[0-9]$"
                ForeColor="Red">
            </asp:RegularExpressionValidator>
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnSave" runat="server" Text="<%$ Resources: CommonLabels, Save %>" OnClick="btnSave_Click" CssClass="button" Visible="false" />
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" OnClick="btnConfirm_Click" CssClass="button" Visible="false" />
            <asp:Button ID="btnCancel" runat="server" Text="<%$ Resources: CommonLabels, Cancel %>" OnClick="btnCancel_Click" CssClass="button" CausesValidation="false" Visible="false" />
        </div>
    </div>

    <div id="dlgCardList" class="DialogCardList" title="Card Results Details">
        <asp:DataList ID="dlCardslist" runat="server"
            OnItemCommand="dlCardslist_ItemCommand"
            Width="100%"
            HeaderStyle-Font-Bold="true"
            HeaderStyle-ForeColor="Azure" Style="margin-top: 0px" CellPadding="0"
            ForeColor="#333333" Font-Names="Verdana" Font-Size="Small">

            <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <HeaderStyle Font-Bold="True" ForeColor="White"></HeaderStyle>

            <HeaderTemplate>
                <tr style="font-weight: bold;">
                    <td>
                        <asp:Label ID="btnCardNumber" runat="server" Text="CardNumber" meta:resourcekey="lblhCardNumberResource" />
                    </td>
                    <td>
                        <asp:Label ID="lblStatus" runat="server" Text="CardStatus" meta:resourcekey="lblhStatusResource" />
                    </td>
                    <td>
                        <asp:Label ID="lblExpDate" runat="server" Text="ExpDate" meta:resourcekey="lblhExpDateResource" />
                    </td>
                    <td>
                        <asp:Label ID="lblIsBaseCard" runat="server" Text="isBaseCard" meta:resourcekey="lblhIsBaseCardResource" />
                    </td>
                </tr>
            </HeaderTemplate>

            <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <ItemTemplate>
                <tr class="ItemSelect">
                    <td>
                        <asp:LinkButton ID="btnCardNumber" CommandName="select" CausesValidation="false" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PAN") %>' CssClass="ItemSelect" />
                    </td>
                    <td>
                        <asp:Label ID="lblStatus" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CardStatus") %>' />
                    </td>
                    <td>
                        <asp:Label ID="lblExpDate" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ExpiryDate", indigoCardIssuingWeb.BasePage.DATE_ASPX_FORMAT) %>' />
                    </td>
                    <td>
                        <asp:Label ID="lblIsBaseCard" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "IsBaseCard") %>' />
                    </td>
                </tr>
            </ItemTemplate>

            <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <SelectedItemTemplate>
            </SelectedItemTemplate>
        </asp:DataList>
    </div>
</asp:Content>
