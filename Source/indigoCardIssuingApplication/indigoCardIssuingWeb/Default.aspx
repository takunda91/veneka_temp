
<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" meta:resourcekey="PageResource" EnableViewState="true" CodeBehind="Default.aspx.cs" Inherits="indigoCardIssuingWeb.Default" UICulture="auto:en-US" Culture="auto" ValidateRequest="false" %>


<asp:Content ID="contLogin" ContentPlaceHolderID="cphLogin" runat="server">

    <script type="text/javascript">
        
            function Resend() {
                var authId = document.getElementById('<%= lblauthId.ClientID %>');
                var name = document.getElementById('<%= lblname.ClientID %>');
                $.ajax({
                    type: "POST",
                    url: "Default.aspx/Resend",
                    data: '{authId: "' + authId.innerText + '" , name: "' + name.innerText + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    //success: OnSuccess,
                    //failure: function (response) {
                    //    alert(response.d);
                    //}
                }).done(function (data) {
                    var json_response = data.d;
                    document.getElementById('ctl00_lgLogInView_cphLogin_lblInfoverificationmessage').innerText = json_response;
                    return false;

                }).fail(function (err) {

                    var json_response = err.responseJSON.Message;
                    document.getElementById('<%= lblInfoverificationmessage.ClientID %>').innerText = json_response;
                    return false;
                });
                ShowVerificationPanel();
            };

            function Verify() {
                var code = document.getElementById('<%= tbCode.ClientID %>');
                var authId = document.getElementById('<%= lblauthId.ClientID %>');
                var name = document.getElementById('<%= lblname.ClientID %>');
                var sessionKey = document.getElementById('<%= lblsessionkey.ClientID %>');

                alert("Code " + code.value + ", auth id " + authId.innerText + ", name " + name.innerText + ", session key " + sessionKey.innerText);

                jQuery.ajax({
                    type: "POST",
                    url: "Default.aspx/Verify",
                    data: '{Code: "' + code.value + '", authId: "' + authId.innerText + '" , name: "' + name.innerText + '" , sessionKey: "' + sessionKey.innerText + '" }',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: false,
                    success: function (Result) {
                        console.log("success noted");
                        console.log(Result);
                        if (Result.d == "SUCCESS") {

                            alert("Action Was Successful.");
                            window.location.assign("HomePage.aspx");
                        }
                        else {

                            var json_response = Result.d;

                            alert(json_response);
                            document.getElementById('<%= lblInfoverificationmessage.ClientID %>').innerText = json_response;


                        }
                    },
                    failure: function (response) {

                        var json_response = response.d;
                        alert(json_response);
                        document.getElementById('<%= lblInfoverificationmessage.ClientID %>').innerText = json_response;

                    }
                });
                alert("AFTER AJAX CALL");
                return false;
            };

            function ShowVerificationPanel() {

                document.getElementById('<%=pnlVerification.ClientID%>').style.display = "block";
                document.getElementById('<%=Login1.ClientID%>').style.display = "none";

            };

            function HideVerificationPanel() {

                document.getElementById('<%=pnlVerification.ClientID%>').style.display = "none";
                document.getElementById('<%=Login1.ClientID%>').style.display = "block";
            };
     
    </script>

    <asp:Login ID="Login1" runat="server" OnAuthenticate="Login1_Authenticate" BorderColor="#0082B6" CssClass="LoginTable" ClientIDMode="Static">
        <LayoutTemplate>
            <div class="ContentPanel">
                <div class="ContentHeader HeaderFill">
                    <asp:Label ID="Label1" Text="Log In" runat="server" meta:resourcekey="lblLoginResource" />
                </div>

                <asp:Label ID="Label2" runat="server" Text="Username" CssClass="label leftclr" Width="30%" meta:resourcekey="lblUsernameResource" />
                <asp:TextBox ID="UserName" AutoCompleteType="Disabled" runat="server" CausesValidation="True" CssClass="input rightclr" Width="50%" />

                <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" Text="Password" CssClass="label leftclr" Width="30%" meta:resourcekey="lblPasswordResource" />
                <asp:TextBox ID="Password" runat="server" autocomplete="off" AutoCompleteType="Disabled" TextMode="Password" CausesValidation="True" CssClass="input rightclr" Width="50%" />

                <div class="ErrorPanel">
                    <asp:Label ID="FailureText" runat="server" EnableViewState="False" Style="max-width: 300px" />
                </div>

                <div class="ButtonPanel">
                    <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Log In" CssClass="button"
                        ValidationGroup="Login1" Width="100px" meta:resourcekey="lblLoginResource" />
                </div>
            </div>
        </LayoutTemplate>
    </asp:Login>
    <asp:Panel ID="ChangePasswordPanel" runat="Server" Visible="False">
        <div class="ContentPanel">
            <div class="ContentHeader">
                <asp:Label ID="lblChangePassword" runat="server" Text="Change Password" meta:resourcekey="lblChangePasswordResource" />
            </div>
            <div class="InfoPanel">
                <asp:Label ID="lblMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
            </div>

            <asp:Label ID="Label2" runat="server" Text="Username :" CssClass="label leftclr" meta:resourcekey="lblUsernameResource" />
            <asp:Label ID="lblUserName" AutoCompleteType="Disabled" runat="server" CausesValidation="True" CssClass="input rightclr" Width="50%" />

            <asp:Label ID="lblOldPassword" runat="server" Text="Current password:" CssClass="label leftclr" meta:resourcekey="lblOldPasswordResource"></asp:Label>
            <asp:TextBox ID="tbOldPassword" runat="server" CssClass="input" MaxLength="15" AutoCompleteType="Disabled" TextMode="Password" />
            <asp:RequiredFieldValidator ID="rfvoldpassword" runat="server" ControlToValidate="tbOldPassword"
                ErrorMessage="Please enter your current password" CssClass="validation rightclr"
                meta:resourcekey="rfvoldpasswordResource" />

            <asp:Label ID="lblChangeAdminPass" runat="server" Text="New password:" CssClass="label leftclr" meta:resourcekey="lblChangeAdminPassResource" />
            <asp:TextBox ID="tbNewPassword" runat="server" TextMode="Password" CssClass="input" MaxLength="15" />
            <%--        <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="tbNewPassword" CssClass="validation rightclr"
                                    ErrorMessage="Please enter a password" meta:resourcekey="rfvPasswordResource" />--%>
            <asp:RegularExpressionValidator ID="revPassword" runat="server" CssClass="validation rightclr" ForeColor="Red"
                runat="server" ControlToValidate="tbNewPassword" ErrorMessage="Password not strong enough"
                ValidationExpression="(?=^.{8,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$"
                meta:resourcekey="revPasswordResource" />

            <asp:Label ID="lblConfirmAdminPass" runat="server" Text="Confirm new password:" CssClass="label leftclr" meta:resourcekey="lblConfirmAdminPassResource" />
            <asp:TextBox ID="tbConfirmPassword" runat="server" TextMode="Password" CssClass="input" MaxLength="15" />
            <asp:RequiredFieldValidator ID="rfvnewpassword" runat="server" ControlToValidate="tbNewPassword"
                ErrorMessage="Please enter a password" CssClass="validation rightclr"
                meta:resourcekey="rfvnewpasswordResource" />
            <asp:CompareValidator ID="cvpassword" runat="server" ControlToCompare="tbNewPassword" CssClass="validation rightclr"
                ControlToValidate="tbConfirmPassword" ErrorMessage="Passwords do not match."
                meta:resourcekey="cvpasswordResource" />
            <div class="ButtonPanel">
                <asp:Button ID="ChangePasswordButton" Text="Change Password" CssClass="button"
                    OnClick="ChangePassword_OnClick" runat="server" />
            </div>
        </div>

    </asp:Panel>
    <asp:Panel ID="pnlVerification" runat="Server" Visible="False">

        <div class="ContentPanel">
            <div class="ContentHeader">
                <asp:Label ID="lblVerifyHeadng" runat="server" Text="Verification Process" />
            </div>

            <div class="InfoPanel">

                <asp:Label ID="lblInfoverificationmessage" runat="server" />
            </div>
            <asp:Label ID="lblname" runat="server" Style="display: none" />
            <asp:Label ID="lblsessionkey" runat="server" Style="display: none" />
            <asp:Label ID="lblauthId" runat="server" Style="display: none" />
            <div style="padding-left:100px;">
                <asp:Label ID="lblVerificationCode" runat="server" Text="Authentication Code" CssClass="label leftclr" />
                <asp:TextBox ID="tbCode" runat="server" TextMode="Password" MaxLength="100" CssClass="input rightclr" />

            </div>
            <div id="pnlresendbutton" class="ButtonPanel" runat="server">
                <asp:Button ID="btnResend" runat="server" Text="Re-Send" Style="display: none" 
                    CssClass="button" OnClientClick="javascript:return Resend();" />
            </div>
            <div class="ButtonPanel">

                <asp:Button ID="btnVerify" runat="server" Text="Verify"
                    CssClass="button" OnClick="VerifyAuthCode"  />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel"
                    CssClass="button" OnClick="btnCancel_Click" />
            </div>
        </div>
    </asp:Panel>

</asp:Content>
