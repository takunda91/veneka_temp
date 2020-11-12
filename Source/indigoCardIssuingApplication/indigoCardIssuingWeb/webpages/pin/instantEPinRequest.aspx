<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/IndigoCardIssuance.Master" CodeBehind="instantEPinRequest.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.pin.instantEPinRequest" Culture="auto:en-US" meta:resourcekey="PageResource" UICulture="auto" %>

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
                <asp:Label ID="lblCardAndCustomerDetails" runat="server" Text="Pin, Card And Customer Details" meta:resourcekey="lblCardAndCustomerDetailsResource" />
            </div>

            <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlIssuer" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlIssuer_OnSelectedIndexChanged" CssClass="input rightclr" />

            <asp:Label ID="lblBranch" runat="server" Text="<%$ Resources: CommonLabels, Branch %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlBranch" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlBranch_OnSelectedIndexChanged" CssClass="input rightclr" />

            <asp:Label ID="lblCardProduct" runat="server" Text="<%$ Resources: CommonLabels, Product %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlProduct" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlProduct_OnSelectedIndexChanged" CssClass="input rightclr" />

                     
            <asp:Label ID="lblDomBranch" runat="server" Text="Domicile Branch" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlDomBranch" runat="server" CssClass="input rightclr puidropdown" />

            <%-- Last 4 digit of the pan --%>

              <div id="pnlPan" runat="server" class="bothclr"  style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="lblPan" runat="server" Text="PAN (Last Four Digits)" CssClass="label leftclr" meta:resourcekey="lblAccountNumberResource" />
                <asp:TextBox ID="tbPan" runat="server" CssClass="input" MaxLength="4" />

                <%--<asp:Button ID="btnValidatePan" runat="server" Text="Validate" meta:resourcekey="btnValidatePanResource"
                    CssClass="button" Style="margin: 0px 0px 0px 5px !important; float: left;"
                    OnClick="btnValidatePan_Click" />--%>


                <asp:RequiredFieldValidator runat="server" ID="reqPan" ControlToValidate="tbPan" meta:resourcekey="reqAccNoResource"
                    ErrorMessage="Please enter last four digit of the PAN." CssClass="validation rightclr" />

<%--                <div runat="server" id="Div3" visible="false">
                    <asp:Label ID="Label9" runat="server" Text="Account Pin" CssClass="label leftclr" meta:resourcekey="lblAccountNumberPinResource" />
                    <asp:TextBox ID="TextBox2" runat="server" TextMode="Password" CssClass="input-pin" MaxLength="4" OnKeyPress="return isNumberKey(event)" />
                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" ControlToValidate="tbAccPin" meta:resourcekey="reqAccPanResource"
                        ErrorMessage="Please enter account pan." CssClass="validation rightclr" />
                </div>--%>

            </div>

               <%-- Expiry date Month and Year --%>

              <div id="pnlExpiryYear" runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="lblExpiry" runat="server" Text="Enter Expiry Year [YYYY]" CssClass="label leftclr" meta:resourcekey="lblExpiryResource" />
                  <asp:TextBox ID="tbExpiry" runat="server" CssClass="input" MaxLength="4" />
                
                <asp:RequiredFieldValidator runat="server" ID="reqExpiry" ControlToValidate="tbExpiry" meta:resourcekey="reqExpDateResource"
                    ErrorMessage="Please enter Correct Expiry Date." CssClass="validation rightclr" />

                <%--<div runat="server" id="Div5" visible="false">
                    <asp:Label ID="Label10" runat="server" Text="Account Pin" CssClass="label leftclr" meta:resourcekey="lblAccountNumberPinResource" />
                    <asp:TextBox ID="TextBox3" runat="server" TextMode="Password" CssClass="input-pin" MaxLength="4" OnKeyPress="return isNumberKey(event)" />
                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="tbAccPin" meta:resourcekey="reqAccExpiryResource"
                        ErrorMessage="Please enter account pan." CssClass="validation rightclr" />
                </div>--%>

            </div>

               <div id="pnlExpiryMonth" runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="lblExpMonth" runat="server" Text="Select Expiry Month" CssClass="label leftclr" meta:resourcekey="lblExpiryResource" />
                 <asp:DropDownList ID="ddlExpMonth" AutoPostBack="True"  runat="server" CssClass="input rightclr puidropdown" > 
                               <asp:ListItem Selected="True" Value="01"> Jan </asp:ListItem>
                              <asp:ListItem Value="02"> February </asp:ListItem>
                              <asp:ListItem Value="03"> March </asp:ListItem>
                              <asp:ListItem Value="05"> April </asp:ListItem>
                              <asp:ListItem Value="05"> May </asp:ListItem>
                              <asp:ListItem Value="06"> June </asp:ListItem>
                              <asp:ListItem Value="07"> July </asp:ListItem>
                              <asp:ListItem Value="08"> August </asp:ListItem>
                              <asp:ListItem Value="09"> September </asp:ListItem>
                              <asp:ListItem Value="10"> October </asp:ListItem>
                               <asp:ListItem Value="11"> November </asp:ListItem>
                              <asp:ListItem Value="12"> December </asp:ListItem>
                 </asp:DropDownList>

                <asp:RequiredFieldValidator runat="server" ID="reqExpMonth" ControlToValidate="tbExpiry" meta:resourcekey="reqExpDateResource"
                    ErrorMessage="Please enter Correct Expiry Date." CssClass="validation rightclr" />

                <%--<div runat="server" id="Div5" visible="false">
                    <asp:Label ID="Label10" runat="server" Text="Account Pin" CssClass="label leftclr" meta:resourcekey="lblAccountNumberPinResource" />
                    <asp:TextBox ID="TextBox3" runat="server" TextMode="Password" CssClass="input-pin" MaxLength="4" OnKeyPress="return isNumberKey(event)" />
                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="tbAccPin" meta:resourcekey="reqAccExpiryResource"
                        ErrorMessage="Please enter account pan." CssClass="validation rightclr" />
                </div>--%>

            </div>

            <%-- Pin Distribution Method --%>
             <div id="pnlDistMethod" runat="server" class="bothclr"  style="margin-top: 10px; margin-bottom: 10px;">
            <asp:Label ID="lblPinDistMethod" runat="server" Text="Pin Distribution Method" CssClass="label leftclr" />
            <asp:RadioButton id="rbSMS" Text="SMS"  Checked="True" GroupName="RadioGroup1" runat="server" CssClass="" Style="margin: 0px 0px 0px 5px !important; float: left;"/>
            <asp:RadioButton id="rbEMAIL" Text="EMAIL"   GroupName="RadioGroup1" runat="server" CssClass="" Style="margin: 0px 0px 0px 5px !important; float: left;"/>
             <asp:RadioButton id="rbUSSD" Text="USSD"   GroupName="RadioGroup1" runat="server" CssClass="" Style="margin: 0px 0px 0px 5px !important; float: left;"/>
             </div>

            <%-- Account number look up --%>

           <div id="pnlAccountNumber" runat="server" class="bothclr" style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="lblAccountNumber" runat="server" Text="Account number" CssClass="label leftclr" meta:resourcekey="lblAccountNumberResource" />
                <asp:TextBox ID="tbAccountNumber" runat="server" CssClass="input" MaxLength="27" />

                <asp:Button ID="btnValidateAccount" runat="server" Text="Validate" meta:resourcekey="btnValidateAccountResource"
                    CssClass="button" Style="margin: 0px 0px 0px 5px !important; float: left;"
                    OnClick="btnValidateAccount_Click" />

                <asp:RequiredFieldValidator runat="server" ID="reqAccNo" ControlToValidate="tbAccountNumber" meta:resourcekey="reqAccNoResource"
                    ErrorMessage="Please enter account number." CssClass="validation rightclr" />

                <div runat="server" id="showPinValidationCapture" visible="false">
                    <asp:Label ID="lblAccPin" runat="server" Text="Account Pin" CssClass="label leftclr" meta:resourcekey="lblAccountNumberPinResource" />
                    <asp:TextBox ID="tbAccPin" runat="server" TextMode="Password" CssClass="input-pin" MaxLength="4" OnKeyPress="return isNumberKey(event)" />
                    <asp:RequiredFieldValidator runat="server" ID="reqAccPin" ControlToValidate="tbAccPin" meta:resourcekey="reqAccPinResource"
                        ErrorMessage="Please enter account pin." CssClass="validation rightclr" />
                </div>

            </div>

            <asp:Panel ID="pnlCustomerDetail" runat="server" Visible="false">
                               
                <div id="divPrintFields" runat="server">
                </div>
               <%-- <div id="divuserTitle" runat="server">
                    <asp:Label ID="userTitle" runat="server" Text="Title" CssClass="label leftclr" meta:resourcekey="userTitleResource" />
                   <asp:TextBox ID="tbTitle" runat="server" CssClass="input" MaxLength="50" />
                    <asp:RequiredFieldValidator runat="server" ID="requserTitle" ControlToValidate="ddlTitle" InitialValue="-99"
                        ErrorMessage="Please enter user title." CssClass="validation rightclr" meta:resourcekey="requserTitleResource" />
                </div>--%>
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
                    <asp:Label ID="lblcontactnumber" runat="server" Text="Contact number" CssClass="label leftclr" meta:resourcekey="lblcontactnumberResource" />
                    <asp:TextBox ID="tbContactnumber" runat="server" CssClass="input rightclr" MaxLength="50" OnKeyPress="return isNumberKeyWithPlus(event)" />
                    <asp:RequiredFieldValidator runat="server" ID="reqcontactnumber" ControlToValidate="tbcontactnumber"
                        ErrorMessage="Please update contact number in CMS" CssClass="validation rightclr" meta:resourcekey="reqcontactnumberResource"></asp:RequiredFieldValidator>

                </div>

                <div id="divEmailAddress" runat="server">
                    <asp:Label ID="Label2" runat="server" Text="Email address" CssClass="label leftclr" meta:resourcekey="lblEmailResource" />
                    <asp:TextBox ID="tbEmailAddress" runat="server" CssClass="input" MaxLength="250" />
                    <asp:RequiredFieldValidator runat="server" ID="reqEmailAddress" ControlToValidate="tbEmailAddress"
                        ErrorMessage="Please update email address in CMS." CssClass="validation rightclr" meta:resourcekey="reqEmailAddressResource"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator runat="server" ID="regExpressionEmail" ControlToValidate="tbEmailAddress"
                        ErrorMessage="Invalid email address." CssClass="validation rightclr" meta:resourcekey="reqEmailAddressResource" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>

                </div>

               <%-- <div id="divResident" runat="server">
                    <asp:Label ID="lblResident" runat="server" Text="Resident" CssClass="label leftclr" meta:resourcekey="lblResidentResource" />
                    <asp:DropDownList ID="ddlResident" runat="server" CssClass="puidropdown" />
                    <asp:RequiredFieldValidator runat="server" ID="reqResident" ControlToValidate="ddlResident" InitialValue="-99"
                        ErrorMessage="Please select resident." CssClass="validation rightclr" meta:resourcekey="reqResidentResource"></asp:RequiredFieldValidator>
                </div>--%>
               
              <%--  <div id="divContractNumber" runat="server">
                    <asp:Label ID="lblContractNumber" runat="server" Text="CMS Agreement Number" CssClass="label leftclr" meta:resourcekey="lblContractNumberResource" />
                    <asp:TextBox ID="tbContractNumber" runat="server" Enabled="False" MaxLength="15" CssClass="input" OnKeyPress="return isNumberKey(event)" />
                    <asp:RequiredFieldValidator runat="server" ID="reqContractNumber" ControlToValidate="tbContractNumber" Enabled="false" ForeColor="Red"
                        ErrorMessage="Please enter Agreement number." CssClass="validation rightclr" meta:resourcekey="rfvContractNumberResource" />
                </div>--%>
               
              <%--  <div id="divPassporttype" runat="server">
                    <asp:Label ID="lblPassporttype" runat="server" Text="Passport Type" Visible="false" CssClass="label leftclr" />
                    <asp:DropDownList ID="ddlPassporttype" runat="server" Visible="false" CssClass="input rightclr puidropdown" />
                    <asp:RequiredFieldValidator runat="server" ID="reqPassporttype" ControlToValidate="ddlPassporttype"
                        ErrorMessage="Please enter passport type." CssClass="validation rightclr" />
                </div>--%>

            </asp:Panel>      


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
            <asp:Button ID="btnSave" runat="server" Text="<%$ Resources: CommonLabels, Save %>" OnClick="btnSave_Click" CssClass="button" Visible="false" />
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" OnClick="btnConfirm_Click" CssClass="button" Visible="false" />
            <asp:Button ID="btnCancel" runat="server" Text="<%$ Resources: CommonLabels, Cancel %>" OnClick="btnCancel_Click" CssClass="button" CausesValidation="false" Visible="false" />
        </div>
    </div>

    
</asp:Content>
