<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="PinResetView.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.pin.PinResetView" %>
<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblPinReissue" Text="PIN Reissue" runat="server" />
            </div>            

            <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr" />
            <asp:TextBox ID="tbIssuer" runat="server" Enabled="false"  CssClass="input rightclr" />

            <asp:Label ID="lblBranch" runat="server" Text="<%$ Resources: CommonLabels, Branch %>" CssClass="label leftclr" />
            <asp:TextBox ID="tbBranch" runat="server" Enabled="false" CssClass="input rightclr" />

            <asp:Label ID="lblProduct" runat="server" Text="<%$ Resources: CommonLabels, Product %>" CssClass="label leftclr" />
            <asp:TextBox ID="tbProduct" runat="server" Enabled="false" CssClass="input rightclr" />

            <asp:Label ID="lblRequestDate" runat="server" Text="Requested" CssClass="label leftclr" />
            <asp:TextBox ID="tbRequestDate" runat="server" Enabled="false" CssClass="input rightclr" />

            <asp:Label ID="lblCardNumber" runat="server" Text="<%$ Resources: CommonLabels, CardNumber %>" CssClass="label leftclr" />
            <asp:TextBox ID="tbCardNumber" runat="server" Enabled="false" CssClass="input rightclr" />

            <asp:Label ID="lbloperator" runat="server" Text="Requested By" CssClass="label leftclr" />
            <asp:TextBox ID="tbOperator" runat="server" Enabled="false" CssClass="input rightclr" />

            <asp:Label ID="lblAuthUser" runat="server" Text="Authorised By" CssClass="label leftclr" />
            <asp:TextBox ID="tbAuthUser" runat="server" Enabled="false" CssClass="input rightclr" />

            <asp:Label ID="lblStatusDate" runat="server" Text="Status Date" CssClass="label leftclr" />
            <asp:TextBox ID="tbStatusDate" runat="server" Enabled="false" CssClass="input rightclr" />

            <asp:Label ID="lblStatus" runat="server" Text="Status" CssClass="label leftclr" />
            <asp:TextBox ID="tbStatus" runat="server" Enabled="false" CssClass="input rightclr" />

            <asp:Label ID="lblExpiry" runat="server" Text="Request Expiry" CssClass="label leftclr" />
            <asp:TextBox ID="tbExpiry" runat="server" Enabled="false" CssClass="input rightclr" />

            <asp:Label ID="lblComments" runat="server" Text="Comments" CssClass="label leftclr" />
            <asp:TextBox ID="tbComments" runat="server" Enabled="false" CssClass="input rightclr" />
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnApprove" runat="server" Text="Approve" CssClass="button" OnClick="btnApprove_OnClick" Visible="false" />
            <asp:Button ID="btnReject" runat="server" Text="Reject" CssClass="button" OnClick="btnReject_OnClick" Visible="false" />
            <asp:Button ID="btnUpload" runat="server" Text="Upload PIN" CssClass="button" OnClick="btnUpload_OnClick" Visible="false" />
          <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="button" OnClick="btnCancel_Click" Visible="false" />
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" CssClass="button" OnClick="btnConfirm_OnClick" Visible="false" />
            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources: CommonLabels, Back %>" CssClass="button" OnClick="btnBack_OnClick" Visible="false" />
        </div>
    </div>
</asp:Content>
