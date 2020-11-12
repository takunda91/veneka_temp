<%@ Page  Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/IndigoCardIssuance.Master" CodeBehind="UserPinAuthorisationMaintainance.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.account.UserPinAuthorisationMaintainance" culture="auto:en-US" meta:resourcekey="PageResource1" uiculture="auto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel" id="pnlPin">

        <div class="ContentHeader">
            <asp:Label ID="lblChangePin" runat="server" Text="Change Authorisation Pin" meta:resourcekey="lblChangePinResource1" />
        </div>

        <asp:Label ID="lblChangeAdminPin" runat="server" Text="Pin:" CssClass="label leftclr" meta:resourcekey="lblChangeAdminPinResource1" />
        <asp:TextBox ID="tbNewPin" runat="server" TextMode="Password" CssClass="input" MaxLength="15" meta:resourcekey="tbNewPinResource1" />
        <asp:RegularExpressionValidator ID="revPin" CssClass="validation rightclr" ForeColor="Red"
            runat="server" ControlToValidate="tbNewPin" ErrorMessage="Pin not strong enough"
            ValidationExpression="(?=^.{8,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$" meta:resourcekey="revPinResource1" />

        <asp:Label ID="lblConfirmAdminPin" runat="server" Text="Confirm Pin:" CssClass="label leftclr" meta:resourcekey="lblConfirmAdminPinResource1"/>
        <asp:TextBox ID="tbConfirmPin" runat="server" TextMode="Password" CssClass="input" MaxLength="15" meta:resourcekey="tbConfirmPinResource1" />
        <asp:RequiredFieldValidator ID="rfvnewPin" runat="server" ControlToValidate="tbNewPin"
            ErrorMessage="Please enter a Pin" CssClass="validation rightclr" meta:resourcekey="rfvnewPinResource1" />
        <asp:CompareValidator ID="cvPin" runat="server" ControlToCompare="tbNewPin" CssClass="validation rightclr"
            ControlToValidate="tbConfirmPin" ErrorMessage="Pins do not match." meta:resourcekey="cvPinResource1" />

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource1" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource1" />
        </div>

        <div class="ButtonPanel">
            <asp:Button ID="btnChangeAdminPin" runat="server" CssClass="button"
                OnClick="btnChangeAdminPin_Click" Text="Submit" meta:resourcekey="btnChangeAdminPinResource1"/>
        </div>
    </div>
</asp:Content>
