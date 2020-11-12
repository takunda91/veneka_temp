<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="UseradminSettingsViewForm.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.users.UseradminSettingsViewForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblHeader" runat="server" Text="User Admin Settings" meta:resourcekey="lblCreateUseradminResource" />
            </div>


            <asp:Label ID="lblpasswordsvalidation" runat="server" Text="Password Validation Period(in days)" Width="250px" CssClass="label leftclr" meta:resourcekey="lblpasswordsvalidationResource" />
            <asp:TextBox ID="tbpasswordvalidation" runat="server" CssClass="input" Width="150" OnKeyPress="return isNumberKey(event)" />
            <asp:RequiredFieldValidator ID="rfvUserGruopName" runat="server" ControlToValidate="tbpasswordvalidation" CssClass="validation rightclr"
                ErrorMessage="Password Validation Period may not be empty." meta:resourcekey="rfvpasswordvalidationResource" />

            <asp:Label ID="lblPreviousPasswordsCount" runat="server" Text="Max length of Previous Passwords" Width="250px" CssClass="label leftclr" meta:resourcekey="lblPreviousPasswordsCountResource" />
            <asp:TextBox ID="tbPreviousPasswordsCount" runat="server" CssClass="input" Width="150" OnKeyPress="return isNumberKey(event)" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="tbPreviousPasswordsCount" CssClass="validation rightclr"
                ErrorMessage="max length of Previous Passwords may not be empty." meta:resourcekey="rfvPreviousPasswordsCountResource" />

            <asp:Label ID="lblPasswordminlength" runat="server" Text="Password Min length" Width="250px" CssClass="label leftclr" meta:resourcekey="lblPasswordminlengthResource" />
            <asp:TextBox ID="tbPasswordminlength" runat="server" CssClass="input" Width="150" OnKeyPress="return isNumberKey(event)" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tbPasswordminlength" CssClass="validation rightclr"
                ErrorMessage="Password Min length may not be empty." meta:resourcekey="rfvPasswordminlengthResource" />

         

            <asp:Label ID="lblPasswordMaxlength" runat="server" Text="Password Max length" Width="250px" CssClass="label leftclr" meta:resourcekey="lblPasswordMaxlengthResource" />
            <asp:TextBox ID="tbPasswordMaxlength" runat="server" CssClass="input" Width="150" OnKeyPress="return isNumberKey(event)" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="tbPasswordMaxlength" CssClass="validation rightclr"
                ErrorMessage="Password Max length may not be empty." meta:resourcekey="rfvPasswordMaxlengthResource" />



            <asp:Label ID="lblmaxnofpasswordattempts" runat="server" Text="Max Invalid Password Attempts" Width="250px" CssClass="label leftclr" meta:resourcekey="lblmaxnofpasswordattemptsResource" />
            <asp:TextBox ID="tbmaxnofpasswordattempts" runat="server" CssClass="input" Width="150" OnKeyPress="return isNumberKey(event)" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="tbmaxnofpasswordattempts" CssClass="validation rightclr"
                ErrorMessage="Max Invalid Password Attempts may not be empty." meta:resourcekey="rfvmaxnofpasswordattemptsResource" />

            <asp:Label ID="lblPasswordAttemptLockoutDuration" runat="server" Text="Password Attempt Lockout Duration(in hr)" Width="250px" CssClass="label leftclr" meta:resourcekey="lblmaxnofpasswordattemptsResource" />
            <asp:TextBox ID="tbPasswordAttemptLockoutDuration" runat="server" CssClass="input" Width="150" OnKeyPress="return isNumberKey(event)" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="tbPasswordAttemptLockoutDuration" CssClass="validation rightclr"
                ErrorMessage="Password Attempt Lockout Duration may not be empty." meta:resourcekey="rfvPasswordAttemptLockoutDurationResource" />
           <asp:CompareValidator
                ID="rvcpasswordminlengh" CssClass="validation leftclr"
                ControlToValidate="tbPasswordminlength" meta:resourcekey="rvcpasswordminlengthResource"
                ValueToCompare="5"
                Type="Integer"
                Operator="GreaterThanEqual"
                ErrorMessage="Password Min length should not greater than 5"
                Text="Password Min length should not greater than 5"
                runat="server"> 
            </asp:CompareValidator>
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnCreate" runat="server" Text="Create" CssClass="button" OnClick="btnCreate_Click" meta:resourcekey="btnCreateUserGroupResource" />
            <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="button" OnClick="btnEdit_Click" meta:resourcekey="btnEditUserGroupResource" />
            <asp:Button ID="btnUpdate" runat="server" Text="Update" CssClass="button" OnClick="btnUpdate_Click" meta:resourcekey="btnUpdateGroupResource" />

            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" CssClass="button" OnClick="btnConfirm_Click" />



            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources: CommonLabels, Back %>" CssClass="button" CausesValidation="false" OnClick="btnBack_OnClick" />
        </div>
    </div>
</asp:Content>
