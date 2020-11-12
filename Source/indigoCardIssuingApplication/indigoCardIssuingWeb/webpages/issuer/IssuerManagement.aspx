<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="IssuerManagement.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.issuer.IssuerManagement" UICulture="auto" Culture="auto:en-US" meta:resourcekey="PageResource" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $(function () {
                $("#<%= tbexpirydate.ClientID %>").datepicker({
                    dateFormat: 'dd/mm/yy',
                    changeMonth: true,
                    changeYear: true,
                    onClose: function (selectedDate) {
                        $("#<%= tbexpirydate.ClientID %>").datepicker("option", "minDate", selectedDate);
                    }
                });
            });
        });
    </script>
     <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblHeader" runat="server" meta:resourcekey="lblIssuerheadingNameResource" />
            </div>

            <asp:Label ID="lblIssuerName" runat="server" Text="Issuer Name" CssClass="label leftclr" meta:resourcekey="lblIssuerNameResource" />
            <asp:TextBox ID="tbIssuerName" runat="server" CssClass="input" MaxLength="50" />
            <asp:RequiredFieldValidator ID="reqIssuerName" runat="server" ControlToValidate="tbIssuerName" ErrorMessage="Issuer name is required" meta:resourcekey="reqIssuerNameResource"
                CssClass="validation rightclr" ForeColor="red" />

            <asp:Label ID="lblIssuerCode" runat="server" CssClass="label leftclr" meta:resourcekey="lblIssuerCodeResource" />
            <asp:TextBox ID="tbIssuerCode" runat="server" CssClass="input" MaxLength="10" />
            <asp:RequiredFieldValidator ID="reqIssuerCode" runat="server" ControlToValidate="tbIssuerCode" ErrorMessage="Issuer code is required"
                CssClass="validation rightclr" ForeColor="red" meta:resourcekey="reqIssuerCodeResource" />

            <asp:Label ID="lblIssuerStatus" runat="server" CssClass="label leftclr" meta:resourcekey="lblIssuerStatusResource" />
            <asp:DropDownList ID="ddlIssuerStatus" runat="server" CssClass="input rightclr" />

            <asp:Label ID="lblIssuerCountry" runat="server" Text="Issuer Country" CssClass="label leftclr" meta:resourcekey="lblIssuerCountryResource" />
            <asp:DropDownList ID="ddlIssuerCountry" runat="server" CssClass="input rightclr" />

            <asp:Label ID="lblIssuerLanguage" runat="server" Text="Issuer Language" CssClass="label leftclr" meta:resourcekey="lblIssuerLanguageResource" />
            <asp:DropDownList ID="ddlIssuerLanguage" runat="server" CssClass="input rightclr" />

            <table style="width: 100%">
                <tr>
                    <td style="width: 33%">
                        <asp:CheckBox ID="chkEnableInstant" Text="Instant Card Issuing" runat="server" Style="width: 60% !important" />
                    </td>
                    <td style="width: 33%">
                        <asp:CheckBox ID="chkEnableCentralised" Text="Centralised Issuing" runat="server" Style="width: 60% !important" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 33%">
                        <asp:CheckBox ID="chkShowsCard" Text="Display PAN" runat="server" Style="width: 60% !important"
                            title="Display PAN instead of reference number for check-in/out and issuing screens." />
                    </td>
                    <td style="width: 33%">
                        <asp:CheckBox ID="chkMakerChecker" Text="Maker / Checker" runat="server" meta:resourcekey="chkMakerCheckerResource" Style="width: 60% !important" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 33%">
                        <asp:CheckBox ID="chkEnableInstantPin" Text="Enable Instant Pin" runat="server" meta:resourcekey="chkEnableInstantPin" Style="width: 60% !important" />
                    </td>
                    <td style="width: 33%">
                        <asp:CheckBox ID="chkBackOfficeAuth" Text="Back Office Authorise" runat="server" Style="width: 60% !important" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 33%">
                        <asp:CheckBox ID="chkAuthorisePinIssue" Text="Authorise Pin Issue" runat="server" meta:resourcekey="chkAuthorisePinIssue" Style="width: 60% !important" />
                    </td>
                    <td style="width: 33%">
                        <asp:CheckBox ID="chkAuthorisePinReissue" Text="Authorise Pin Reissue" runat="server" meta:resourcekey="chkAuthorisePinReissue" Style="width: 60% !important" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 33%">
                        <asp:CheckBox ID="chkAllowBranches" Text="Allow Branches to Search Cards" runat="server" meta:resourcekey="chkAllowBranches" Style="width: 60% !important" />
                    </td>
                    <td  style="width: 33%"></td>
                 </tr>
            </table>

            <asp:Panel ID="pnlPinCard" runat="server" CssClass="bothclr" GroupingText="PIN Printing" Visible="false" meta:resourcekey="pnlPinCardResource">
                <asp:CheckBox ID="chkDeletePinFile" Text="Delete PIN File" runat="server" meta:resourcekey="chkDeletePinFileResource" />
            </asp:Panel>

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
                        <asp:Label ID="lblHSM" runat="server" Text="HSM" />
                    </td>
                    <td style="width: 25%;">
                        <asp:DropDownList ID="ddlInterfaceHSM" runat="server" CssClass="input" Style="width: 80% !important" />
                    </td>
                    <td style="width: 25%;">
                        <asp:DropDownList ID="ddlProdHSM" runat="server" CssClass="input" Style="width: 80% !important" />
                    </td>
                    <td style="width: 25%;">
                        <asp:DropDownList ID="ddlIssueHSM" runat="server" CssClass="input" Style="width: 80% !important" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 15%;"></td>
                    <td style="width: 25%">
                        <%--<asp:Label ID="Label1" runat="server" Text="Interface" Font-Bold="true" />--%>
                    </td>
                    <td style="width: 25%;">
                        <asp:Label ID="Label2" runat="server" Text="Email" Font-Bold="true" />
                    </td>
                    <td style="width: 25%;">
                        <asp:Label ID="Label3" runat="server" Text="SMS" Font-Bold="true" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 15%;">
                        <asp:Label ID="lblNotification" runat="server" Text="Notification" />
                    </td>
                    <td style="width: 25%;">
                        <asp:DropDownList ID="ddlInterfaceNotification" runat="server" CssClass="input" Style="width: 80% !important" />
                    </td>
                    <td style="width: 25%;">
                        <asp:DropDownList ID="ddlprodNotification" runat="server" CssClass="input" Style="width: 80% !important" />
                    </td>
                    <td style="width: 25%;">
                        <asp:DropDownList ID="ddlIssuerNotification" runat="server" CssClass="input" Style="width: 80% !important" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="bothclr">
            <asp:UpdatePanel ID="updatepanel" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <fieldset>
                <legend>Remote Componet Details</legend>
                <asp:CheckBox ID="chkEnableToken" runat="server" Text="Enable Remote Token"  CssClass="label" OnCheckedChanged="chkEnableToken_CheckedChanged" AutoPostBack="true"/>


                <asp:Label ID="lblRemotetoken" runat="server" Text="Remote Token" CssClass="label leftclr" />


                <asp:TextBox ID="tbremotetoken" runat="server" CssClass="input rightclr" MaxLength="10" ReadOnly="true" />


                <asp:Button ID="btngenerateremotetoken" runat="server" Text="Generate" OnClick="btngenerateremotetoken_Click" CausesValidation="false" CssClass="button" />


                <asp:Label ID="lblexpirtydate" runat="server" CssClass="label leftclr" Text="Expiry Date" />


                <asp:TextBox ID="tbexpirydate" runat="server" CssClass="input rightclr" MaxLength="10" />
            </fieldset>
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btngenerateremotetoken" />
                    <asp:PostBackTrigger ControlID="chkEnableToken" />
                </Triggers>
            </asp:UpdatePanel>
            


        </div>

        <div class="bothclr">
            <asp:UpdatePanel ID="updatepanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <fieldset>
                <legend>Default Messaging </legend>
               
                   <asp:Label ID="Label1" runat="server" Text="Key Word : Customer Name" CssClass="label leftclr" />

                     <asp:TextBox ID="TextBox1" runat="server" Enabled="false" ReadOnly="true" CssClass="input rightclr" Text="@customername" />

                 <asp:Label ID="Label4" runat="server" Text="Key Word : Customer Account" CssClass="label leftclr" />

                         <asp:TextBox ID="TextBox2" runat="server"  Enabled="false" ReadOnly="true" CssClass="input rightclr" Text="@account" />

                  <asp:Label ID="Label5" runat="server" Text="Key Word : Decrypted Pin" CssClass="label leftclr" />
                  <asp:TextBox ID="TextBox3" runat="server"  Enabled="false" ReadOnly="true" CssClass="input rightclr" Text="@pin" />

                <asp:Label ID="lblPinText" runat="server" Text="Pin Message Body" CssClass="label leftclr" />
                <asp:TextBox ID="tbMessageBody"  runat="server"  Height="60px" TextMode="MultiLine" MaxLength="1000" CssClass="input rightclr" />

                 <asp:Label ID="lblMaxNumOfPinResend" runat="server" Text="Max Number of Pin Resend " CssClass="label leftclr" />
                  <asp:TextBox ID="tbMaxNumOfPinSend" runat="server"   CssClass="input rightclr" />

                 <asp:Label ID="lblPinBlockDelete" runat="server" Text=" Pin Block Delete Days" CssClass="label leftclr" />
                  <asp:TextBox ID="tbDeletePinBlockAfter" runat="server"  CssClass="input rightclr" />


            </fieldset>
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btngenerateremotetoken" />
                    <asp:PostBackTrigger ControlID="chkEnableToken" />
                </Triggers>
            </asp:UpdatePanel>
            


        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnCreate" runat="server" Text="<%$ Resources: CommonLabels, Create %>" OnClick="btnCreate_Click" CssClass="button" />
            <asp:Button ID="btnEdit" runat="server" Text="<%$ Resources: CommonLabels, Edit %>" OnClick="btnEdit_Click" CssClass="button" />
            <asp:Button ID="btnUpdateIssuer" runat="server" Text="<%$ Resources: btnUpdateIssuer.Text %>" OnClick="btnUpdateIssuer_Click" CssClass="button" />
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" OnClick="btnConfirm_Click" CssClass="button" />
            <asp:Button ID="btnBack" runat="server" OnClick="btnBack_Click" Text="<%$ Resources: btnBack.Text %>" CssClass="button" />
            <asp:Button ID="btnCancel" runat="server" Text="<%$ Resources: CommonLabels, Cancel %>" OnClick="btnCancel_Click" Visible="False" CssClass="button" />
        </div>
    </div>
</asp:Content>
