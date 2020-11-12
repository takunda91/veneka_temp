<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true"
    meta:resourcekey="PageResource" UICulture="auto" Culture="auto:en-US" CodeBehind="ProductAdminScreen.aspx.cs"
    Inherits="indigoCardIssuingWeb.webpages.product.ProductAdminScreen" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        $(function () {
            $('#default').puitabview({
                activeIndex: document.getElementById('<%= hdnActiveTab.ClientID %>').value,
                change: function (event, ui) {
                    var hiddenSource = document.getElementById('<%= hdnActiveTab.ClientID %>');
                    hiddenSource.value = $('#default').puitabview('getActiveIndex');
                }
            });
        });
        window.onload = function () {


            var ddlpincal = document.getElementById("<%= ddlPinCalcMethod.ClientID%>");
            PinCalucationOnChange(ddlpincal);
        }


        function PinCalucationOnChange(obj) {
            if (obj != null) {
                var Decimasationvalidator = document.getElementById("<%= rfvDecimasation.ClientID%>");
                var Pinvalidationvalidator = document.getElementById("<%= rfvPinvalidation.ClientID%>");
                if (obj.options[obj.selectedIndex].value == '1')// if it is "IBM Method"
                {
                    ValidatorEnable(Decimasationvalidator, true);
                    ValidatorEnable(Pinvalidationvalidator, true);
                    document.getElementById('<%=divpincal.ClientID%>').style.display = 'block';
                    document.getElementById('<%=divIBMPinoffset.ClientID%>').style.display = 'block';
                }
                else {
                    ValidatorEnable(Decimasationvalidator, false);
                    ValidatorEnable(Pinvalidationvalidator, false);
                    document.getElementById("<%= tbDecimalisation.ClientID%>").value = '';
                    document.getElementById("<%= tbPinvalidation.ClientID%>").value = '';

                    document.getElementById('<%=divIBMPinoffset.ClientID%>').style.display = 'none';

                }
            }
        }
    </script>

    <asp:HiddenField ID="hdnActiveTab" runat="server" />

    <div id="content" class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblProductscreenheading" runat="server" Text="Product Admin" meta:resourcekey="lblProductscreenheadingResource" />
            </div>
            <div id="default" class="indigo-tab">
                <ul>
                    <li><a href="#detailsTab">Details</a></li>
                    <li><a href="#issuingTab">Issuing</a></li>
                    <li><a href="#pinTab">PIN</a></li>
                    <li><a href="#tab2">File Processing</a></li>
                    <li><a href="#cmsTab">CMS</a></li>
                    <li><a href="#cbsTab">CBS</a></li>
                    <li><a href="#cpsTab">CPS</a></li>
                    <li><a href="#tab4">Currency</a></li>
                    <li><a href="#tab5">Encryption</a></li>
                    <li><a href="#tab6">Printing</a></li>
                    <li><a href="#tab7">Account Type Mapping</a></li>
                    <li><a href="#fundsLoadTab">Funds Load</a></li>
                    <li><a href="#documentsTab">Documents</a></li>
                </ul>
                <div>
                    <div id="detailsTab" style="overflow: auto">
                        <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr" />
                        <asp:DropDownList ID="ddlIssuer" runat="server" CssClass="input rightclr"
                            OnSelectedIndexChanged="ddlIssuer_SelectedIndexChanged" AutoPostBack="true" />

                        <asp:Label ID="lblProductname" runat="server" Text="Product Name" CssClass="label leftclr"
                            meta:resourcekey="lblProductnameResource" />
                        <asp:TextBox ID="tbProductname" runat="server" CssClass="input" MaxLength="100" />
                        <asp:RequiredFieldValidator ID="rfvProductname" runat="server" ControlToValidate="tbProductname"
                            ErrorMessage="Product Name Required." ForeColor="Red" CssClass="validation rightclr"
                            meta:resourcekey="rfvProductnameResource" />

                        <asp:Label ID="lblProductCode" runat="server" Text="Product Code" CssClass="label leftclr"
                            meta:resourcekey="lblProductCodeResource" />
                        <asp:TextBox ID="tbProductCode" runat="server" CssClass="input" MaxLength="50" />
                        <asp:RequiredFieldValidator ID="rfvProductcode" runat="server" ControlToValidate="tbProductCode"
                            ErrorMessage="Product Code Required." ForeColor="Red" CssClass="validation rightclr"
                            meta:resourcekey="rfvProductcodeResource" />

                        <asp:Label ID="lblproductbin" runat="server" Text="Product BIN" CssClass="label leftclr"
                            meta:resourcekey="lblproductbinResource" />
                        <asp:TextBox ID="tbproductbin" runat="server" CssClass="input" MaxLength="6" OnKeyPress="return isNumberKey(event)" />
                        <asp:RequiredFieldValidator ID="rfvproductbin" runat="server" ControlToValidate="tbproductbin"
                            ErrorMessage="Product BIN Required." ForeColor="Red" meta:resourcekey="rfvproductbinResource"
                            CssClass="validation" />

                        <asp:Label ID="lblSubProductCode" runat="server" Text="SubProduct Code" CssClass="label leftclr" />
                        <asp:TextBox ID="tbSubProductCode" runat="server" CssClass="input rightclr" MaxLength="3" OnKeyPress="return isNumberKey(event)"
                            Title="Allows for grouping of cards into a sub-product by using a unique sequence of digits that succeeding the BIN." />

                        <asp:Label ID="lblPanLength" runat="server" Text="PAN Length" CssClass="label leftclr" />
                        <asp:DropDownList ID="ddlPanLength" runat="server" CssClass="input rightclr">
                            <asp:ListItem Text="12" Value="12" />
                            <asp:ListItem Text="13" Value="13" />
                            <asp:ListItem Text="14" Value="14" />
                            <asp:ListItem Text="15" Value="15" />
                            <asp:ListItem Text="16" Value="16" Selected="True" />
                            <asp:ListItem Text="17" Value="17" />
                            <asp:ListItem Text="18" Value="18" />
                            <asp:ListItem Text="19" Value="19" />
                        </asp:DropDownList>

                        <asp:Label ID="lblExpiryMonth" runat="server" Text="Expiry Months" CssClass="label leftclr" />
                        <asp:TextBox ID="tbExpiryMonth" runat="server" CssClass="input" OnKeyPress="return isNumberKey(event)"
                            Title="Indicates in months how long the card is valid for and used to determine the expiry date of a card." />

                        <asp:Label ID="lblCardIssueMethod" runat="server" Text="Card Issue Method" CssClass="label leftclr" />
                        <asp:DropDownList ID="ddlCardIssueMethod" runat="server" CssClass="input rightclr" OnSelectedIndexChanged="ddlCardIssueMethod_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Text="Centralised" Value="0" />
                            <asp:ListItem Text="Instant" Value="1" Selected="True" />
                        </asp:DropDownList>

                        <asp:Label ID="lblProdFlow" runat="server" Text="Production Flow" CssClass="label leftclr" />
                        <asp:DropDownList ID="ddlProdFlow" runat="server" CssClass="input rightclr" />

                        <asp:Label ID="lblDistFlow" runat="server" Text="Distribution Flow" CssClass="label leftclr" />
                        <asp:DropDownList ID="ddlDistFlow" runat="server" CssClass="input rightclr" />

                        <asp:Label ID="lblScheme" runat="server" Text="Fee Scheme" CssClass="label leftclr" />
                        <asp:DropDownList ID="ddlProductScheme" runat="server" CssClass="input rightclr" />

                        <asp:Label ID="lblChargeToIssueBranch" runat="server" Text="Fee to Issuing Branch" CssClass="label leftclr" />
                        <asp:CheckBox ID="chkbChargeToIssueBranch" runat="server" CssClass="input rightclr" />

                        <asp:Label ID="lblChargeFeeAtCardRequest" runat="server" Text="Charge Fee At Card Request" CssClass="label leftclr" />
                        <asp:CheckBox ID="chkChargeFeeAtCardRequest" runat="server" CssClass="input rightclr" />
                        
                        <asp:Label ID="lblAllowM20Print" runat="server" Text="Allow M20 Print" CssClass="label leftclr" />
                        <asp:CheckBox ID="allowM20Print" runat="server" CssClass="input rightclr" />

                        <asp:Label ID="lblPrintIssueCard" runat="server" Text="Print Card On Issue" CssClass="label leftclr" />
                        <asp:CheckBox ID="chkPrintIssueCard" runat="server" Checked="true" CssClass="input rightclr" />

                        <asp:Label ID="lblAllowManualUpload" runat="server" Text="Allow Manual Upload" CssClass="label leftclr" />
                        <asp:CheckBox ID="chkbAllowManualUpload" runat="server" CssClass="input rightclr" />

                        <asp:Label ID="lblAllowReupload" runat="server" Text="Allow Re-Upload" CssClass="label leftclr" />
                        <asp:CheckBox ID="chkbAllowReupload" runat="server" CssClass="input rightclr" />

                        <asp:Label ID="Label23" runat="server" Text="Renewal Production Flow" CssClass="label leftclr" />
                        <asp:DropDownList ID="ddlProdFlowRenewal" runat="server" CssClass="input rightclr" />

                        <asp:Label ID="Label24" runat="server" Text="Renewal Distribution Flow" CssClass="label leftclr" />
                        <asp:DropDownList ID="ddlDistFlowRenewal" runat="server" CssClass="input rightclr" />

                    </div>
                    <div id="issuingTab" style="overflow: auto">


                        <asp:Panel ID="pnlIssuingScenarios" runat="server" GroupingText="Supported Issuing" CssClass="bothclr">
                            <asp:CheckBoxList ID="chklIssuingScenarios"
                                CellPadding="5"
                                CellSpacing="5"
                                RepeatColumns="2"
                                RepeatDirection="Horizontal"
                                RepeatLayout="Table"
                                TextAlign="Right"
                                runat="server"
                                CssClass="chkboxlist">
                            </asp:CheckBoxList>
                        </asp:Panel>
                        <asp:Panel ID="pnlSupportedAccounts" runat="server" GroupingText="Supported Accounts" CssClass="bothclr">
                            <asp:CheckBoxList ID="chklAccounts"
                                CellPadding="5"
                                CellSpacing="5"
                                RepeatColumns="3"
                                RepeatDirection="Horizontal"
                                RepeatLayout="Table"
                                TextAlign="Right"
                                runat="server"
                                CssClass="chkboxlist">
                            </asp:CheckBoxList>
                        </asp:Panel>
                    </div>
                    <div id="pinTab" style="overflow: auto">
                        <asp:Panel ID="pnlInstantPinSettings" runat="server" GroupingText="Instant PIN Settings" CssClass="bothclr">
                            <asp:Label ID="lblInstantPin" runat="server" Text="Enable Instant Pin" CssClass="label leftclr" />
                            <asp:CheckBox ID="chkInstantPin" runat="server" CssClass="input rightclr" />

                            <asp:Label ID="lblInstantPinReissue" runat="server" Text="Instant Pin Reissue" CssClass="label leftclr" />
                            <asp:CheckBox ID="chkInstantPinResissue" runat="server" CssClass="input rightclr" />


                            <asp:Label ID="lblePinRequest" runat="server" Text="e-Pin Request" CssClass="label leftclr" />
                            <asp:CheckBox ID="chkePinRequest" runat="server" CssClass="input rightclr" />
                        </asp:Panel>

                        <asp:Panel ID="pnlPINMailerSettings" runat="server" GroupingText="PIN Mailer Settings" CssClass="bothclr">
                            <asp:Label ID="lblPinMailer" runat="server" Text="Enable PIN Mailer" CssClass="label leftclr" />
                            <asp:CheckBox ID="chkPrintPIN" runat="server" CssClass="input rightclr" />

                            <asp:Label ID="lblPinMailerReprint" runat="server" Text="PIN Mailer Reprinting" CssClass="label leftclr" />
                            <asp:CheckBox ID="chkRePrintPin" runat="server" CssClass="input rightclr" />
                        </asp:Panel>

                        <asp:Panel ID="pnlPinSettings" runat="server" GroupingText="PIN Settings" CssClass="bothclr">



                            <asp:Label ID="lblMinPinLength" runat="server" Text="Min Pin Length" CssClass="label leftclr" />
                            <asp:TextBox ID="tbMinPinLength" runat="server" CssClass="input" OnKeyPress="return isNumberKey(event)" />
                            <asp:RequiredFieldValidator ID="rfvMinPinLength" runat="server" ControlToValidate="tbMinPinLength"
                                ErrorMessage="Minimum PIN length required." ForeColor="Red"
                                CssClass="validation" />

                            <asp:Label ID="lblMaxPinLength" runat="server" Text="Max Pin Length" CssClass="label leftclr" />
                            <asp:TextBox ID="tbMaxPinLength" runat="server" CssClass="input" OnKeyPress="return isNumberKey(event)" />
                            <asp:RequiredFieldValidator ID="rfvMaxPinLength" runat="server" ControlToValidate="tbMaxPinLength"
                                ErrorMessage="Maximum PIN length required." ForeColor="Red"
                                CssClass="validation" />

                            <asp:Label ID="lblpinblock" runat="server" Text="PIN Block Format" CssClass="label leftclr" />
                            <asp:DropDownList ID="ddlPinblcok" runat="server" CssClass="input rightclr">
                            </asp:DropDownList>
                            <div id="divpincal" runat="server" class="bothclr">
                                <asp:Label ID="lblPinCalc" runat="server" Text="PIN Calculation Method" CssClass="label leftclr" />
                                <asp:DropDownList ID="ddlPinCalcMethod" runat="server" CssClass="input rightclr" onchange="PinCalucationOnChange(this)">
                                    <asp:ListItem Text="VISA Method" Value="0" Selected="True" />
                                    <asp:ListItem Text="IBM Method" Value="1" />
                                    <asp:ListItem Text="No Calculation" Value="2" />
                                    <asp:ListItem Text="Custom Method" Value="3" />
                                </asp:DropDownList>
                            </div>
                            <div id="divIBMPinoffset" runat="server" class="bothclr" style="display: none">
                                <asp:Label ID="lblDecimalisation" runat="server" Text="Decimalisation table" CssClass="label leftclr" />
                                <asp:TextBox ID="tbDecimalisation" runat="server" CssClass="input" MaxLength="16" />
                                <asp:RequiredFieldValidator ID="rfvDecimasation" runat="server" ControlToValidate="tbDecimalisation" Enabled="false"
                                    ErrorMessage="Decimalisation table required." ForeColor="Red"
                                    CssClass="validation" />

                                <asp:Label ID="lblPinvalidation" runat="server" Text="PIN validation data" CssClass="label leftclr" />
                                <asp:TextBox ID="tbPinvalidation" runat="server" CssClass="input" MaxLength="12" />
                                <asp:RequiredFieldValidator ID="rfvPinvalidation" runat="server" ControlToValidate="tbPinvalidation" Enabled="false"
                                    ErrorMessage="PIN validation data required." ForeColor="Red"
                                    CssClass="validation" />
                            </div>
                        </asp:Panel>

                        <asp:Panel ID="pnlExternalSystemsHSM" runat="server" GroupingText="External Systems" CssClass="bothclr">
                            <asp:Label runat="server" ID="lblExternalSystemHSM" Text="External System" meta:resourcekey="lblExternalSystemsHSMResource" CssClass="label leftclr"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlexternalsytemsHSM" CssClass="input rightclr" OnSelectedIndexChanged="ddlexternalsytemsHSM_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>

                            <asp:DataList ID="dlExternalfieldsHSM" runat="server" Width="50%" Visible="false">
                                <HeaderTemplate>
                                    <table border="0" width="100%">
                                        <tr>
                                            <th>Field Name</th>
                                            <th>Field Value</th>
                                        </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>

                                        <td>
                                            <asp:Label ID="lblexternalsystemfieldid" runat="server" Visible="false" Text='<%# Eval("external_system_field_id").ToString() %>' />

                                            <asp:Label ID="tbFieldName" runat="server" Text='<%#  Eval("field_name") ?? "" %>' /></td>
                                        <td>
                                            <asp:TextBox ID="tbFieldValue" runat="server" Text='<%#  Eval("field_value") ?? "" %>' /></td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </table>
                                </FooterTemplate>
                            </asp:DataList>

                        </asp:Panel>
                    </div>
                    <div id="tab2" style="overflow: auto">
                        <asp:Panel ID="pnlFileSettings" runat="server" GroupingText="Settings" CssClass="bothclr">
                            <asp:Label ID="lblLoadBatchType" runat="server" Text="File Load Type" CssClass="label leftclr" />
                            <asp:DropDownList ID="ddlLoadBatchType" runat="server" CssClass="input rightclr" />

                            <asp:Label ID="lblAutoApproveBatch" runat="server" Text="Auto Approve Batch" CssClass="label leftclr" />
                            <asp:CheckBox ID="chkAutoApproveBatch" runat="server" CssClass="input rightclr"
                                Title="If loading to production or distribution batches this option will approve those batches when load batch is approved." />
                        </asp:Panel>

                        <asp:Panel ID="pnlFileIntegration" runat="server" GroupingText="Integration" CssClass="bothclr">
                            <table style="width: 100%;">
                                <tr>
                                    <td style="width: 15%;"></td>
                                    <td style="width: 25%">
                                        <asp:Label ID="lblFileProccessorH" runat="server" Text="Interface" Font-Bold="true" />
                                    </td>
                                    <td style="width: 25%;">
                                        <asp:Label ID="lblFileLoadingHeading" runat="server" Text="Settings" Font-Bold="true" />
                                    </td>
                                    <%--<td style="width: 25%;">
                                        <asp:Label ID="lblFileOutputH" runat="server" Text="Export" Font-Bold="true" />
                                    </td>--%>
                                </tr>
                                <tr>
                                    <td style="width: 15%;">
                                        <asp:Label ID="lblFileProcessing" runat="server" Text="File Loading" meta:resourcekey="lblFileProcessing" />
                                    </td>
                                    <td style="width: 25%;">
                                        <asp:DropDownList ID="ddlInterfaceFileLoader" runat="server" CssClass="input" Style="width: 80% !important" />
                                    </td>
                                    <td style="width: 25%;">
                                        <asp:DropDownList ID="ddlFileLoader" runat="server" CssClass="input" Style="width: 80% !important"
                                            Title="Specifies the configuration to use when loading files." />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 15%;">
                                        <asp:Label ID="Label7" runat="server" Text="File Export" />
                                    </td>
                                    <td style="width: 25%;">
                                        <asp:DropDownList ID="ddlFileExport" runat="server" CssClass="input" Style="width: 80% !important" />
                                    </td>
                                    <td style="width: 25%;">
                                        <asp:DropDownList ID="ddlFileExportSettings" runat="server" CssClass="input" Style="width: 80% !important"
                                            Title="Specifies the configuartion to use when exporting file." />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </div>
                    <div id="cmsTab" style="overflow: auto">
                        <asp:Panel ID="pnlCmsSettings" runat="server" GroupingText="Settings" CssClass="bothclr">
                            <asp:Label ID="lblCMSExportable" runat="server" Text="CMS File Export" CssClass="label leftclr" />
                            <asp:CheckBox ID="chkCMSExportable" runat="server" CssClass="input rightclr" />
                            <asp:Label ID="lblremoteCMSEnable" runat="server" Text="Remote CMS Update" CssClass="label leftclr" />
                            <asp:CheckBox ID="chkremoteCMSEnable" runat="server" CssClass="input rightclr" />
                        </asp:Panel>
                        <asp:Panel ID="pnlCmsIntegrarion" runat="server" GroupingText="Integration" CssClass="bothclr">
                            <table style="width: 100%;">
                                <tr>
                                    <td style="width: 15%;"></td>
                                    <td style="width: 25%">
                                        <asp:Label ID="lblHeadInterface" runat="server" Text="Interface" Font-Bold="true" />
                                    </td>
                                    <td style="width: 25%;">
                                        <asp:Label ID="lblHeadProductionInterfaces" runat="server" Text="Production" Font-Bold="true" />
                                    </td>
                                    <td style="width: 25%;">
                                        <asp:Label ID="lblHeadIssuingInterfaces" runat="server" Text="Issuing" Font-Bold="true" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 15%;">
                                        <asp:Label ID="lblCMS" runat="server" Text="CMS" meta:resourcekey="lblCMSResource" />
                                    </td>
                                    <td style="width: 25%;">
                                        <asp:DropDownList ID="ddlInterfaceCMS" runat="server" CssClass="input" Style="width: 80% !important" />
                                    </td>
                                    <td style="width: 25%;">
                                        <asp:DropDownList ID="ddlProdCMS" runat="server" CssClass="input" Style="width: 80% !important" />
                                    </td>
                                    <td style="width: 25%;">
                                        <asp:DropDownList ID="ddlIssueCMS" runat="server" CssClass="input" Style="width: 80% !important" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 15%;">
                                        <asp:Label ID="lblRemoteCMS" runat="server" Text="CMS" meta:resourcekey="lblCMSResource" />
                                    </td>
                                    <td style="width: 25%;">
                                        <asp:DropDownList ID="ddlInterfaceRemoteCMS" runat="server" CssClass="input" Style="width: 80% !important" />
                                    </td>
                                    <td style="width: 25%;"></td>
                                    <td style="width: 25%;">
                                        <asp:DropDownList ID="ddlIssuerRemoteCMS" runat="server" CssClass="input" Style="width: 80% !important" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 15%;">
                                        <asp:Label ID="lblThreedSecure" runat="server" Text="3D-Secure" meta:resourcekey="lblThreedResource" />
                                    </td>
                                    <td style="width: 25%;">
                                        <asp:DropDownList ID="ddlInterfaceThreed" runat="server" CssClass="input" Style="width: 80% !important" />
                                    </td>
                                    <td style="width: 25%;">
                                        <asp:DropDownList ID="ddlProductThreedSecure" runat="server" CssClass="input" Style="width: 80% !important" />
                                    </td>
                                    <td style="width: 25%;">
                                        <asp:DropDownList ID="ddlIssureThreedSecure" runat="server" CssClass="input" Style="width: 80% !important" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <asp:Panel ID="pnlExternalSystemCMS" runat="server" GroupingText="External Systems" CssClass="bothclr">
                            <asp:Label runat="server" ID="lblExternalSystemsCMS" Text="External System" meta:resourcekey="lblExternalSystemsCMSResource" CssClass="label leftclr"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlexternalsytemsCMS" CssClass="input rightclr" OnSelectedIndexChanged="ddlexternalsytemsCMS_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>

                            <asp:DataList ID="dlExternalfieldsCMS" runat="server" Width="50%" Visible="false">
                                <HeaderTemplate>
                                    <table border="0" width="100%">
                                        <tr>
                                            <th>Field Name</th>
                                            <th>Field Value</th>
                                        </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>

                                        <td>
                                            <asp:Label ID="lblexternalsystemfieldid" runat="server" Visible="false" Text='<%# Eval("external_system_field_id").ToString() %>' />

                                            <asp:Label ID="tbFieldName" runat="server" Text='<%#  Eval("field_name") ?? "" %>' /></td>
                                        <td>
                                            <asp:TextBox ID="tbFieldValue" runat="server" Text='<%#  Eval("field_value") ?? "" %>' /></td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </table>
                                </FooterTemplate>
                            </asp:DataList>

                        </asp:Panel>

                        <asp:Panel ID="Panel2" runat="server" GroupingText="Renewal Settings" CssClass="bothclr">
                            <asp:Label ID="Label6" runat="server" Text="Activate Card" CssClass="label leftclr" />
                            <asp:CheckBox ID="chkRenewalActivate" runat="server" CssClass="input rightclr" />

                            <asp:Label ID="Label8" runat="server" Text="Charge Fee" CssClass="label leftclr" />
                            <asp:CheckBox ID="chkRenewalCharge" runat="server" CssClass="input rightclr" />
                        </asp:Panel>

                        <asp:Panel ID="Panel3" runat="server" GroupingText="Credit Card Settings" CssClass="bothclr">
                            <asp:Label ID="Label13" runat="server" Text="Capture Limit" CssClass="label leftclr" />
                            <asp:CheckBox ID="chkCreditLimitCapture" runat="server" CssClass="input rightclr" />

                            <asp:Label ID="Label14" runat="server" Text="Update Limit" CssClass="label leftclr" />
                            <asp:CheckBox ID="chkCreditLimitUpdate" runat="server" CssClass="input rightclr" />


                            <asp:Label ID="Label12" runat="server" Text="Approve Limit" CssClass="label leftclr" />
                            <asp:CheckBox ID="chkCreditLimitApprove" runat="server" CssClass="input rightclr" />

                            <asp:Label ID="Label15" runat="server" Text="Email Required" CssClass="label leftclr" />
                            <asp:CheckBox ID="chkEmailRequired" runat="server" CssClass="input rightclr" />

                            <asp:Label ID="Label16" runat="server" Text="Generate Reference Number" CssClass="label leftclr" />
                            <asp:CheckBox ID="chkGenerateReferenceNumber" runat="server" CssClass="input rightclr" />

                            <asp:Label ID="Label17" runat="server" Text="Manual Reference Number" CssClass="label leftclr" />
                            <asp:CheckBox ID="chkManualReferenceNumber" runat="server" CssClass="input rightclr" />

                            <asp:Label ID="Label18" runat="server" Text="Parallel Approval" CssClass="label leftclr" />
                            <asp:CheckBox ID="chkParallelApproval" runat="server" CssClass="input rightclr" />

                            <asp:Label ID="Label19" runat="server" Text="Active at Center Operations" CssClass="label leftclr" />
                            <asp:CheckBox ID="chkCenterOpsActivation" runat="server" CssClass="input rightclr" />

                            <asp:Label ID="Label20" runat="server" Text="Credit Contract # Prefix" CssClass="label leftclr" />
                            <asp:TextBox ID="tbCreditContractPrefix" runat="server" CssClass="input" MaxLength="20" />
                            
                            <asp:Label ID="Label21" runat="server" Text="Credit Contract # Format" CssClass="label leftclr" />
                            <asp:TextBox ID="tbCreditContractSuffixFormat" runat="server" CssClass="input" MaxLength="20" />
                            
                            <asp:Label ID="Label22" runat="server" Text="Last Sequence Number" CssClass="label leftclr" />
                            <asp:TextBox ID="tbCreditContractLastSequence" runat="server" CssClass="input" MaxLength="12"  OnKeyPress="return isNumberKey(event)" />

                        </asp:Panel>

                    </div>
                    <div id="cbsTab" style="overflow: auto">
                        <asp:Panel ID="pnlCbsSettings" runat="server" GroupingText="Settings" CssClass="bothclr">
                            <asp:Label ID="lblAccountValidation" runat="server" Text="Enable Account Validation" CssClass="label leftclr" />
                            <asp:CheckBox ID="chkAccountValidation" runat="server" Style="width: 60% !important" CssClass="input rightclr" />

                            <asp:Label ID="lblAccPinVal" runat="server" Text="Enable Pin Account Validation" CssClass="label leftclr" />
                            <asp:CheckBox ID="chkPinAccountValidation" runat="server" Style="width: 40% !important" CssClass="input rightclr" />

                        </asp:Panel>

                        <asp:Panel ID="pnlCbsIntegration" runat="server" GroupingText="Integration" CssClass="bothclr">
                            <table style="width: 100%;">
                                <tr>
                                    <td style="width: 15%;"></td>
                                    <td style="width: 25%">
                                        <asp:Label ID="Label3" runat="server" Text="Interface" Font-Bold="true" />
                                    </td>
                                    <td style="width: 25%;">
                                        <asp:Label ID="Label4" runat="server" Text="Production" Font-Bold="true" />
                                    </td>
                                    <td style="width: 25%;">
                                        <asp:Label ID="Label5" runat="server" Text="Issuing" Font-Bold="true" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 15%;">
                                        <asp:Label ID="lblCoreBanking" runat="server" Text="Core Banking" meta:resourcekey="lblCoreBankingResource" />
                                    </td>
                                    <td style="width: 25%;">
                                        <asp:DropDownList ID="ddlInterfaceCBS" runat="server" CssClass="input" Style="width: 80% !important" />
                                    </td>
                                    <td style="width: 25%;"></td>
                                    <td style="width: 25%;">
                                        <asp:DropDownList ID="ddlIssueCoreBanking" runat="server" CssClass="input" Style="width: 80% !important" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblFeeScheme" runat="server" Text="Fee Scheme:" meta:resourcekey="lblFeeSchemeResource" />
                                    </td>
                                    <td style="width: 25%;">
                                        <asp:DropDownList ID="ddlInterfaceFeeScheme" runat="server" CssClass="input" Style="width: 80% !important" />
                                    </td>
                                    <td style="width: 25%;"></td>
                                    <td style="width: 25%;">
                                        <asp:DropDownList ID="ddlIssueFeeScheme" runat="server" CssClass="input" Style="width: 80% !important" />
                                    </td>
                                </tr>
                            </table>


                        </asp:Panel>

                        <asp:Panel ID="pnlCBSExternalSystems" runat="server" GroupingText="External Systems" CssClass="bothclr">
                            <asp:Label runat="server" ID="lblExternalSystemsCBS" Text="External System" meta:resourcekey="lblExternalSystemsCBSResource" CssClass="label leftclr"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlexternalsytemsCBS" CssClass="input rightclr" OnSelectedIndexChanged="ddlexternalsytemsCBS_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>

                            <asp:DataList ID="dlExternalfieldsCBS" runat="server" class="bothclr" Width="50%" Visible="false">
                                <HeaderTemplate>
                                    <table border="0" width="100%">
                                        <tr>
                                            <th>Field Name</th>
                                            <th>Field Value</th>
                                        </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>

                                        <td>
                                            <asp:Label ID="lblexternalsystemfieldid" runat="server" Visible="false" Text='<%# Eval("external_system_field_id").ToString() %>' />

                                            <asp:Label ID="tbFieldName" runat="server" Text='<%#  Eval("field_name") ?? "" %>' /></td>
                                        <td>
                                            <asp:TextBox ID="tbFieldValue" runat="server" Text='<%#  Eval("field_value") ?? "" %>' /></td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </table>
                                </FooterTemplate>
                            </asp:DataList>

                        </asp:Panel>
                    </div>
                    <div id="cpsTab" style="overflow: auto">
                        <asp:Panel ID="Panel1" runat="server" GroupingText="Integration" CssClass="bothclr">
                            <table style="width: 100%;">
                                <tr>
                                    <td style="width: 15%;"></td>
                                    <td style="width: 25%">
                                        <asp:Label ID="Label9" runat="server" Text="Interface" Font-Bold="true" />
                                    </td>
                                    <td style="width: 25%;">
                                        <asp:Label ID="Label10" runat="server" Text="Production" Font-Bold="true" />
                                    </td>
                                    <td style="width: 25%;">
                                        <asp:Label ID="Label11" runat="server" Text="Issuing" Font-Bold="true" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 15%;">
                                        <asp:Label ID="lblCPS" runat="server" Text="CPS" />
                                    </td>
                                    <td style="width: 25%;">
                                        <asp:DropDownList ID="ddlInterfaceCPS" runat="server" CssClass="input" Style="width: 80% !important" />
                                    </td>
                                    <td style="width: 25%;">
                                        <asp:DropDownList ID="ddlProdCPS" runat="server" CssClass="input" Style="width: 80% !important" />
                                    </td>
                                    <td style="width: 25%;"></td>
                                </tr>
                            </table>

                        </asp:Panel>
                        <asp:Panel ID="PnlExternalSystems" runat="server" GroupingText="External Systems" CssClass="bothclr">

                            <asp:Label runat="server" ID="lblExternalSystemsCPS" Text="External System" meta:resourcekey="lblExternalSystemsCPSResource" CssClass="label leftclr"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlexternalsytemsCPS" CssClass="input rightclr" AutoPostBack="true" OnSelectedIndexChanged="ddlexternalsytemsCPS_SelectedIndexChanged"></asp:DropDownList>

                            <asp:DataList ID="dlExternalfieldsCPS" runat="server" Width="50%" Visible="false">
                                <HeaderTemplate>
                                    <table border="0" width="100%">
                                        <tr>
                                            <th>Field Name</th>
                                            <th>Field Value</th>
                                        </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>

                                        <td>
                                            <asp:Label ID="lblexternalsystemfieldid" runat="server" Visible="false" Text='<%# Eval("external_system_field_id").ToString() %>' />

                                            <asp:Label ID="tbFieldName" runat="server" Text='<%#  Eval("field_name") ?? "" %>' /></td>
                                        <td>
                                            <asp:TextBox ID="tbFieldValue" runat="server" Text='<%#  Eval("field_value") ?? "" %>' /></td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </table>
                                </FooterTemplate>
                            </asp:DataList>

                        </asp:Panel>
                    </div>
                    <div id="tab4" style="overflow: auto">

                        <asp:DataList ID="dlCurrency" runat="server" Width="100%">
                            <HeaderTemplate>
                                <table border="0" width="100%">
                                    <tr>
                                        <th>Currency</th>
                                        <th>Allow</th>
                                        <th>Base</th>
                                        <th>Field Name</th>
                                        <th>Field Value</th>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <asp:Label ID="chkCurrId" runat="server" Visible="false" Text='<%# Eval("currency_id").ToString() %>' />
                                        <asp:Label ID="chkCode" runat="server" Text='<%# String.Format("{0}/{1} - {2}", Eval("currency_code").ToString(), Eval("iso_4217_numeric_code").ToString(), Eval("currency_desc").ToString()) %>' /></td>
                                    <td>
                                        <asp:CheckBox ID="chkAllow" runat="server" Checked='<%# ((int?)Eval("product_id")).HasValue %>' /></td>
                                    <td>
                                        <asp:CheckBox ID="chkBase" runat="server" Checked='<%#  (bool?)Eval("is_base") ?? false %>' /></td>
                                    <td>
                                        <asp:TextBox ID="tbFieldName1" runat="server" Text='<%#  Eval("usr_field_name_1") ?? "" %>' /></td>
                                    <td>
                                        <asp:TextBox ID="tbFieldVal1" runat="server" Text='<%#  Eval("usr_field_val_1") ?? "" %>' /></td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:DataList>

                        <%--<asp:Table id="tblCurrency" 
                                    GridLines="Both" 
                                    HorizontalAlign="Center" 
                                    Font-Names="Verdana" 
                                    Font-Size="8pt" 
                                    CellPadding="5" 
                                    CellSpacing="0" 
                                    Runat="server" Enabled="false" EnableViewState="true"/>--%>
                        <%--                            <asp:TableHeaderRow runat="server" Font-Bold="true">
                                <asp:TableHeaderCell>Currency</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Allow</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Base</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Field Name</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Field Value</asp:TableHeaderCell>
                            </asp:TableHeaderRow>
                         </asp:Table>--%>


                        <%--<asp:CheckBoxList ID="chkCurrency" runat="server"
                            CellPadding="5"
                            CellSpacing="5"
                            RepeatColumns="2"
                            RepeatDirection="Horizontal"
                            RepeatLayout="Table" />--%>
                    </div>
                    <div id="tab5" style="overflow: auto">
                        <asp:Label ID="lblSRC1" runat="server" Text="SRC 1" CssClass="label leftclr" />
                        <asp:DropDownList ID="ddlSRC1" runat="server" CssClass="input rightclr" />

                        <asp:Label ID="lblSRC2" runat="server" Text="SRC 2" CssClass="label leftclr" />
                        <asp:DropDownList ID="ddlSRC2" runat="server" CssClass="input rightclr" />

                        <asp:Label ID="lblSRC3" runat="server" Text="SRC 3" CssClass="label leftclr" />
                        <asp:DropDownList ID="ddlSRC3" runat="server" CssClass="input rightclr" />

                        <asp:Label ID="lblPVK1" runat="server" Text="PVK Index" CssClass="label leftclr" />
                        <asp:DropDownList ID="ddlPVK1" runat="server" CssClass="input rightclr">
                            <asp:ListItem Text="0" Value="0" Selected="True" />
                            <asp:ListItem Text="1" Value="1" />
                            <asp:ListItem Text="2" Value="2" />
                            <asp:ListItem Text="3" Value="3" />
                            <asp:ListItem Text="4" Value="4" />
                            <asp:ListItem Text="5" Value="5" />
                            <asp:ListItem Text="6" Value="6" />
                        </asp:DropDownList>

                        <asp:Label ID="lblPVK" runat="server" Text="Pin Verification Key" CssClass="label leftclr" />
                        <asp:TextBox ID="tbPVK" runat="server" CssClass="input" />

                        <asp:Label ID="lblCVKA" runat="server" Text="Card Verification Key A" CssClass="label leftclr" />
                        <asp:TextBox ID="tbCVKA" runat="server" CssClass="input" />

                        <asp:Label ID="lblCVKB" runat="server" Text="Card Verification Key B" CssClass="label leftclr" />
                        <asp:TextBox ID="tbCVKB" runat="server" CssClass="input" />
                    </div>

                    <div id="tab6" style="overflow: auto">
                        <%--<asp:Panel ID="pnlView" runat="server" CssClass="bothclr" GroupingText="Name printing co-ordinates"
                            meta:resourcekey="pnlViewResource">
                            <asp:Label ID="lblnameoncardtop" runat="server" Text="Top" CssClass="label leftclr"
                                meta:resourcekey="lblnameoncardtopResource" />
                            <asp:TextBox ID="tbnameoncardtop" runat="server" CssClass="input" MaxLength="5" Text="175.0" />
                            <asp:RequiredFieldValidator ID="rfvnameontop" runat="server" ControlToValidate="tbnameoncardtop"
                                ErrorMessage="Name on card Top Required" ForeColor="Red" meta:resourcekey="rfvnameontopResource"
                                CssClass="validation" />
                            <asp:Label ID="lblnameoncardleft" runat="server" Text="Left" CssClass="label leftclr"
                                meta:resourcekey="lblnameoncardleftResource" />
                            <asp:TextBox ID="tbnameoncardleft" runat="server" CssClass="input" MaxLength="5" Text="10.0" />
                            <asp:RequiredFieldValidator ID="rfvnameoncardleft" runat="server" ControlToValidate="tbnameoncardleft"
                                meta:resourcekey="rfvnameoncardleftResource" ErrorMessage="Name on card Left Required"
                                ForeColor="Red" CssClass="validation" />
                        </asp:Panel>--%>
                        <%--<asp:Panel ID="pnlFonts" runat="server" CssClass="bothclr" GroupingText="Name printing settings">
                            <asp:Label ID="lblfontdropdown" runat="server" Text="Font Drop down" CssClass="label leftclr"
                                meta:resourcekey="lblfontdropdownResource" />
                            <asp:DropDownList ID="ddlfontdropdown" runat="server" CssClass="input rightclr" />
                            <asp:RequiredFieldValidator ID="rfvfondropdown" runat="server" ControlToValidate="ddlfontdropdown"
                                meta:resourcekey="rfvfondropdownResource" ErrorMessage="Font Family Required."
                                InitialValue="-99" ForeColor="Red" CssClass="validation" />

                            <asp:Label ID="lblfontsize" runat="server" Text="Font Size" CssClass="label leftclr"
                                meta:resourcekey="lblfontsizeResource" />
                            <asp:DropDownList ID="ddlFontSize" runat="server" CssClass="input rightclr">
                                <asp:ListItem Text="10" Value="10" Selected="True" />
                                <asp:ListItem Text="11" Value="11" />
                                <asp:ListItem Text="12" Value="12" />
                                <asp:ListItem Text="13" Value="13" />
                                <asp:ListItem Text="14" Value="14" />
                                <asp:ListItem Text="15" Value="15" />
                                <asp:ListItem Text="16" Value="16" />
                            </asp:DropDownList>

                            <asp:Button ID="btnPreview" runat="server" Text="Preview Card" CssClass="button" CausesValidation="false"
                                OnClick="btnPreview_Click" meta:resourcekey="btnPreviewResource" />

                        </asp:Panel>--%>
                        <asp:Panel ID="pnlPrintingFields" runat="server" CssClass="bothclr" GroupingText="Printing field settings">
                            <asp:GridView ID="grdPrintingFields" runat="server" AllowPaging="false"
                                AutoGenerateColumns="False" OnRowDataBound="grdPrintingFields_RowDataBound" OnRowUpdating="grdPrintingFields_RowUpdating"
                                OnRowCancelingEdit="grdPrintingFields_RowCancelingEdit" OnRowEditing="grdPrintingFields_RowEditing"
                                DataKeyNames="product_field_id,product_id" ShowFooter="false" EmptyDataText="No Rows Returned">
                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="Black" />
                                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkAll" runat="server" AutoPostBack="true" OnCheckedChanged="Unnamed_CheckedChanged" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkfield" runat="server" AutoPostBack="true" OnCheckedChanged="Unnamed_CheckedChanged" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="product_field_id"
                                        SortExpression="product_field_id" ReadOnly="True" Visible="false" />
                                    <asp:TemplateField HeaderText="Field Name" ItemStyle-Width="150">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Text='<%# Eval("field_name") %>'></asp:Label>
                                            <asp:TextBox ID="txt_field_name" runat="server" Text='<%# Eval("field_name") %>' Visible="false"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Field Type" ItemStyle-Width="50">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Text='<%# Eval("print_field_name") %>'></asp:Label>
                                            <%--<asp:TextBox ID="txt_print_field_type_id" runat="server" Text='<%# Eval("print_field_type_id") %>' Visible="false" Width="50"></asp:TextBox>--%>

                                            <asp:DropDownList ID="ddl_fieldtype" runat="server" Visible="false" Width="150">
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Left" ItemStyle-Width="50">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Text='<%# Eval("X") %>'></asp:Label>
                                            <asp:TextBox ID="txt_X" runat="server" Text='<%# Eval("X") %>' Visible="false" Width="50"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Top" ItemStyle-Width="50">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Text='<%# Eval("Y") %>'></asp:Label>
                                            <asp:TextBox ID="txt_Y" runat="server" Text='<%# Eval("Y") %>' Visible="false" Width="50"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Width" ItemStyle-Width="50">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Text='<%# Eval("width") %>'></asp:Label>
                                            <asp:TextBox ID="txt_width" runat="server" Text='<%# Eval("width") %>' Visible="false" Width="50"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Height" ItemStyle-Width="50">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Text='<%# Eval("height") %>'></asp:Label>
                                            <asp:TextBox ID="txt_height" runat="server" Text='<%# Eval("height") %>' Visible="false" Width="50"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Font" ItemStyle-Width="150">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Text='<%# Eval("font") %>'></asp:Label>
                                            <asp:DropDownList ID="ddl_font" runat="server" Visible="false" Width="150">
                                            </asp:DropDownList>
                                            <%--<asp:TextBox ID="txt_font" runat="server" Text='<%# Eval("font") %>' Visible="false"></asp:TextBox>--%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Font Size" ItemStyle-Width="50">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Text='<%# Eval("font_size") %>'></asp:Label>
                                            <asp:TextBox ID="txt_font_size" runat="server" Text='<%# Eval("font_size") %>' Visible="false" Width="50" OnKeyPress="return isNumberKey(event)"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Mapped Name" ItemStyle-Width="150">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Text='<%# Eval("mapped_name") %>'></asp:Label>
                                            <asp:TextBox ID="txt_mapped_name" runat="server" Text='<%# Eval("mapped_name") %>' Visible="false"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Editable" ItemStyle-Width="50">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chk_editable" runat="server" Checked='<%# Convert.ToBoolean(Eval("editable")) %>' Enabled="false" Width="50"></asp:CheckBox>

                                            <asp:CheckBox ID="txt_editable" runat="server" Checked='<%# Convert.ToBoolean(Eval("editable"))   %>' Visible="false" Width="50"></asp:CheckBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Deleted" ItemStyle-Width="50">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chk_deleted" runat="server" Checked='<%# Convert.ToBoolean(Eval("deleted")) %>' Enabled="false" Width="50"></asp:CheckBox>

                                            <asp:CheckBox ID="txt_deleted" runat="server" Checked='<%# Convert.ToBoolean(Eval("deleted")) %>' Visible="false" Width="50"></asp:CheckBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Printable" ItemStyle-Width="50">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chk_printable" runat="server" Checked='<%# Convert.ToBoolean(Eval("printable")) %>' Enabled="false" Width="50"></asp:CheckBox>

                                            <asp:CheckBox ID="txt_printable" runat="server" Checked='<%# Convert.ToBoolean(Eval("printable")) %>' Visible="false" Width="50"></asp:CheckBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="PrintSide" ItemStyle-Width="150">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_printside" runat="server" Text='<%# Eval("printside") %>'></asp:Label>
                                            <asp:DropDownList ID="ddl_printside" runat="server" Visible="false" Width="150">
                                            </asp:DropDownList>
                                            <%--<asp:TextBox ID="txt_font" runat="server" Text='<%# Eval("font") %>' Visible="false"></asp:TextBox>--%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Label" ItemStyle-Width="100">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Text='<%# Eval("label") %>'></asp:Label>
                                            <asp:TextBox ID="txt_label" runat="server" Text='<%# Eval("label") %>' Visible="false"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Max Length" ItemStyle-Width="50">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Text='<%# Eval("max_length") %>'></asp:Label>
                                            <asp:TextBox ID="txt_max_length" runat="server" Text='<%# Eval("max_length") %>' Visible="false" Width="50"></asp:TextBox>
                                        </ItemTemplate>
                                        <%--<FooterStyle HorizontalAlign="Right" />
                                        <FooterTemplate>
                                            <asp:Button ID="btnAddRow" runat="server" OnClick="btnAddRow_Click" Text="Add" CausesValidation="false" />
                                        </FooterTemplate>--%>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </asp:Panel>

                    </div>

                    <div id="tab7" style="overflow: auto">
                        <asp:GridView ID="Grdaccounttype" runat="server" CellPadding="1" CssClass="bothclr" GroupingText="External System Fields" EmptyDataText="No Rows Returned" GridLines="None" PageSize="20"
                            AutoGenerateColumns="false" Width="80%" OnRowEditing="EditRow" OnRowDataBound="Grdaccounttype_RowDataBound" OnRowCancelingEdit="CancelEditRow" OnRowCommand="Grdaccounttype_RowCommand" ShowFooter="true"
                            OnRowUpdating="UpdateRow" DataKeyNames="Id" OnRowDeleting="DeleteRow" AllowPaging="true"
                            OnPageIndexChanging="ChangePage">

                            <Columns>



                                <asp:TemplateField HeaderText="CBS Account Type" SortExpression="CbsAccountType">

                                    <EditItemTemplate>

                                        <asp:TextBox ID="tbCBSAccountType" Width="100px" runat="server" Text='<%# Bind("CbsAccountType") %>'></asp:TextBox>

                                    </EditItemTemplate>

                                    <FooterTemplate>

                                        <asp:TextBox ID="tbCBSAccountType" runat="server" Width="100px"></asp:TextBox>

                                    </FooterTemplate>

                                    <ItemTemplate>

                                        <asp:Label ID="lblCBSAccountType" runat="server" Text='<%# Bind("CbsAccountType") %>'></asp:Label>

                                    </ItemTemplate>

                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Indigo Account Type" SortExpression="IndigoAccountTypeId">

                                    <EditItemTemplate>

                                        <asp:TextBox ID="tbIndigoAccountType" Width="100px" runat="server" Text='<%# Bind("IndigoAccountTypeId") %>'></asp:TextBox>

                                    </EditItemTemplate>

                                    <FooterTemplate>

                                        <asp:TextBox ID="tbIndigoAccountType" Width="100px" runat="server"></asp:TextBox>

                                    </FooterTemplate>

                                    <ItemTemplate>

                                        <asp:Label ID="lblIndigoAccountType" runat="server" Text='<%# Bind("IndigoAccountTypeId") %>'></asp:Label>

                                    </ItemTemplate>

                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="CMS Account Type" SortExpression="CmsAccountType">

                                    <EditItemTemplate>

                                        <asp:TextBox ID="tbCMSAccountType" Width="100px" runat="server" Text='<%# Bind("CmsAccountType") %>'></asp:TextBox>

                                    </EditItemTemplate>

                                    <FooterTemplate>

                                        <asp:TextBox ID="tbCMSAccountType" Width="100px" runat="server"></asp:TextBox>

                                    </FooterTemplate>

                                    <ItemTemplate>

                                        <asp:Label ID="lblCMSAccountType" runat="server" Text='<%# Bind("CmsAccountType") %>'></asp:Label>

                                    </ItemTemplate>

                                </asp:TemplateField>


                                <asp:TemplateField HeaderText="Edit" ShowHeader="False">

                                    <EditItemTemplate>

                                        <asp:LinkButton ID="lnkupdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update"></asp:LinkButton>

                                        <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>

                                    </EditItemTemplate>

                                    <FooterTemplate>

                                        <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="False" CommandName="Create" Text="Create"></asp:LinkButton>

                                    </FooterTemplate>

                                    <ItemTemplate>

                                        <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit"></asp:LinkButton>

                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:CommandField HeaderText="Delete" ShowDeleteButton="True" ShowHeader="True" />


                            </Columns>

                            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                            <FooterStyle BackColor="White" Font-Bold="True" ForeColor="Black" />
                            <HeaderStyle BackColor="#0082B6" Font-Bold="True" ForeColor="Black" />
                            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />

                        </asp:GridView>

                    </div>
                    <div id="fundsLoadTab" style="overflow: auto">
                        <table style="width: 100%;">
                            <tr>
                                <td style="width: 15%;">
                                    <asp:Label ID="lblFundsCBS" runat="server" Text="Core Banking" meta:resourcekey="lblCoreBankingResource" />
                                </td>
                                <td style="width: 25%;">
                                    <asp:DropDownList ID="ddlFundsCoreBanking" runat="server" CssClass="input" Style="width: 80% !important" />
                                </td>
                                <td style="width: 15%;">
                                    <asp:Label ID="Label1" runat="server" Text="Connection" />
                                </td>
                                <td style="width: 25%;">
                                    <asp:DropDownList ID="ddlFundsConnectionCBS" runat="server" CssClass="input" Style="width: 80% !important" />
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 15%;">
                                    <asp:Label ID="lblFundsPrepaid" runat="server" Text="Prepaid Interface" meta:resourcekey="lblPrepaidResource" />
                                </td>
                                <td style="width: 25%;">
                                    <asp:DropDownList ID="ddlFundsPrepaid" runat="server" CssClass="input" Style="width: 80% !important" />
                                </td>
                                <td style="width: 15%;">
                                    <asp:Label ID="Label2" runat="server" Text="Connection" />
                                </td>
                                <td style="width: 25%;">
                                    <asp:DropDownList ID="ddlFundsConnectionPrepaid" runat="server" CssClass="input" Style="width: 80% !important" />
                                </td>
                            </tr>
                        </table>
                    </div>

                    <div id="documentsTab" style="overflow: auto">
                        <asp:Panel ID="Panel4" runat="server" CssClass="bothclr" GroupingText="Document List">
                            <asp:GridView ID="grdDocuments" runat="server" AllowPaging="false" Width="100%"
                                AutoGenerateColumns="False" OnRowDataBound="grdDocuments_RowDataBound"
                                OnRowCancelingEdit="grdDocuments_RowCancelingEdit" OnRowEditing="grdDocuments_RowEditing"
                                DataKeyNames="Id,ProductId" ShowFooter="false" EmptyDataText="No Rows Returned">
                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="Black" />
                                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                <Columns>

                                    <asp:BoundField DataField="Id" ReadOnly="True" Visible="false" />
                                    <asp:TemplateField HeaderText="Document" ItemStyle-Width="250">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Text='<%# Eval("DocumentTypeName") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Description">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Text='<%# Eval("DocumentTypeDescription") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    
                                    <asp:TemplateField HeaderText="Include" ItemStyle-Width="50">
                                        <ItemTemplate>
                                            <asp:HiddenField ID="hidDocumentId" runat="server" Value='<%# Eval("Id") %>' />
                                            <asp:HiddenField ID="hidDocumentTypeId" runat="server" Value='<%# Eval("DocumentTypeId") %>' />
                                            <asp:CheckBox ID="chkInclude" runat="server" Checked='<%# Convert.ToBoolean(Eval("Included")) %>' Enabled="true" Width="50"></asp:CheckBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                   
                                    <asp:TemplateField HeaderText="Required" ItemStyle-Width="50">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkRequired" runat="server" Checked='<%# Convert.ToBoolean(Eval("IsRequired")) %>' Enabled="true" Width="50"></asp:CheckBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </asp:Panel>
                    </div>
                </div>

                <div>
                    <asp:ValidationSummary ID="vsProductValidation" runat="server" DisplayMode="BulletList" />
                </div>
            </div>

            <div class="InfoPanel">
                <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
            </div>
            <div class="ErrorPanel">
                <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
            </div>
            <div id="pnlButtons" class="ButtonPanel" runat="server">
                <asp:Button ID="btnCreate" runat="server" Text="Create Product" CssClass="button"
                    meta:resourcekey="btnCreateResource" OnClick="btnCreate_Click" />
                <asp:Button ID="btnEdit" runat="server" Text="<%$ Resources: CommonLabels, Edit %>"
                    CssClass="button" OnClick="btnEdit_Click" />
                <asp:Button ID="btnUpdate" runat="server" Text="Update Product" CssClass="button"
                    meta:resourcekey="btnUpdateResource" OnClick="btnUpdate_Click" />
                <asp:Button ID="btnDelete" runat="server" Text="<%$ Resources: CommonLabels, Delete %>"
                    CssClass="button" OnClick="btnDelete_Click" />
                <asp:Button ID="btnActivate" runat="server" Text="Activate"
                    CssClass="button" OnClick="btnActivate_Click" />
                <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>"
                    CssClass="button" OnClick="btnConfirm_Click" />
                <asp:Button ID="btnBack" runat="server" Text="<%$ Resources: CommonLabels, Back %>"
                    CssClass="button" OnClick="btnBack_OnClick" CausesValidation="False" />
            </div>
        </div>

    </div>
</asp:Content>
