<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="UserPasswordMaintainance.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.account.UserPasswordMaintainance" culture="auto:en-US" meta:resourcekey="PageResource" uiculture="auto" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="ContentPanel">

        <div class="ContentHeader">
            <asp:Label ID="lblChangePassword" runat="server" Text="Change Password"  meta:resourcekey="lblChangePasswordResource" />
        </div>
    
        <asp:Label ID="lblOldPassword" runat="server" Text="Current password:" CssClass="label leftclr" meta:resourcekey="lblOldPasswordResource"></asp:Label>
        <asp:TextBox ID="tbOldPassword" runat="server" CssClass="input" MaxLength="15" autocomplete="off" AutoCompleteType="Disabled" TextMode="Password"  />
        <asp:RequiredFieldValidator ID="rfvoldpassword" runat="server" ControlToValidate="tbOldPassword"
                                            ErrorMessage="Please enter your current password" CssClass="validation rightclr"
                                            meta:resourcekey="rfvoldpasswordResource" />

        <asp:Label ID="lblChangeAdminPass" runat="server" Text="New password:" CssClass="label leftclr" meta:resourcekey="lblChangeAdminPassResource" />
        <asp:TextBox ID="tbNewPassword" runat="server" TextMode="Password" autocomplete="off" CssClass="input" MaxLength="15"  />
<%--        <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="tbNewPassword" CssClass="validation rightclr"
                                    ErrorMessage="Please enter a password" meta:resourcekey="rfvPasswordResource" />--%>
        <asp:RegularExpressionValidator ID="revPassword" CssClass="validation rightclr" ForeColor="Red"
                    runat="server" ControlToValidate="tbNewPassword" ErrorMessage="Password not strong enough"
                    ValidationExpression="(?=^.{8,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$"
                    meta:resourcekey="revPasswordResource" />

        <asp:Label ID="lblConfirmAdminPass" runat="server" Text="Confirm new password:" CssClass="label leftclr" meta:resourcekey="lblConfirmAdminPassResource" />
        <asp:TextBox ID="tbConfirmPassword" runat="server" TextMode="Password" autocomplete="off" CssClass="input" MaxLength="15"  />
        <asp:RequiredFieldValidator ID="rfvnewpassword" runat="server" ControlToValidate="tbNewPassword"
                                    ErrorMessage="Please enter a password" CssClass="validation rightclr"
                                    meta:resourcekey="rfvnewpasswordResource" />
        <asp:CompareValidator ID="cvpassword" runat="server" ControlToCompare="tbNewPassword" CssClass="validation rightclr"
                              ControlToValidate="tbConfirmPassword" ErrorMessage="Passwords do not match." 
                              meta:resourcekey="cvpasswordResource" />

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>
        
        <div class="ButtonPanel">
            <asp:Button ID="btnChangeAdminPass" runat="server" CssClass="button"
                        OnClick="btnChangeAdminPass_Click" 
                        meta:resourcekey="btnChangeAdminPassResource" />
        </div>
    </div>
</asp:Content>