<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="TerminalManager.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.system.TerminalManager" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblTerminalDetails" runat="server" Text="Terminal Details" meta:resourcekey="lblTerminalDetailsResource" />
            </div>
            <div style="overflow: auto">
                <asp:Label runat="server" ID="lblIssuer" Text="Issuer" meta:resourcekey="lblIssuerResource" CssClass="label leftclr"></asp:Label>
                <asp:DropDownList runat="server" ID="ddlIssuer" OnSelectedIndexChanged="ddlIssuer_SelectedIndexChanged" CssClass="input rightclr" meta:resourcekey="ddlIssuerResource1" AutoPostBack="true"></asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlIssuer" ID="rfvIssuer" ErrorMessage="Issuer Required"
                    meta:resourcekey="rfvIssuerResource" ForeColor="Red" CssClass="validation rightclr"></asp:RequiredFieldValidator>

                <asp:Label runat="server" ID="lblBranch" Text="Branch" meta:resourcekey="lblBranchResource" CssClass="label leftclr"></asp:Label>
                <asp:DropDownList runat="server" ID="ddlBranch" CssClass="input rightclr" meta:resourcekey="ddlBranchResource1"></asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlBranch" ID="rfvBranch" ErrorMessage="Branch Required"
                    meta:resourcekey="rfvBranchResource" ForeColor="Red" CssClass="validation rightclr"></asp:RequiredFieldValidator>

                <asp:Label runat="server" ID="lblMasterKey" Text="Master Key" meta:resourcekey="lblTerminalMasterKeyResource" CssClass="label leftclr"></asp:Label>
                <asp:DropDownList runat="server" ID="ddlTerminalMasterKey" CssClass="input rightclr" meta:resourcekey="ddlTerminalMasterKeyResource1"></asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlTerminalMasterKey" ID="rfvTerminalMasterKey" ErrorMessage="Master Key Required"
                    meta:resourcekey="rfvTerminalMasterKey" ForeColor="Red" CssClass="validation rightclr"></asp:RequiredFieldValidator>

                <asp:Label runat="server" ID="lblTerminalName" Text="Terminal Name" CssClass="label leftclr"
                    meta:resourcekey="lblTerminalNameResource"></asp:Label>
                <asp:TextBox runat="server" ID="txtTerminalName" CssClass="input" meta:resourcekey="txtTerminalNameResource1"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTerminalName" ID="rfvTerminalName"
                    meta:resourcekey="rfvTerminalNameResource" ErrorMessage="Terminal Name Required" ForeColor="Red" CssClass="validation rightclr"></asp:RequiredFieldValidator>

                <asp:Label runat="server" ID="lblTerminalModel" Text="Terminal Model" CssClass="label leftclr" meta:resourcekey="lblTerminalModelResource"></asp:Label>
                <asp:TextBox runat="server" ID="txtTerminalModel" CssClass="input" meta:resourcekey="txtTerminalModelResource1"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTerminalModel" ID="rfvTerminalModel" ErrorMessage="Terminal Model Required"
                    meta:resourcekey="rfvTerminalModelResource" ForeColor="Red" CssClass="validation rightclr"></asp:RequiredFieldValidator>

                <asp:Label runat="server" ID="lblDeviceId" Text="Device Number" CssClass="label leftclr" meta:resourcekey="lblDeviceIdResource"></asp:Label>
                <asp:TextBox runat="server" ID="txtDeviceId" CssClass="input" meta:resourcekey="txtDeviceIdResource1"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDeviceId" ID="rfvDeviceId" ErrorMessage="Device Number Required"
                    meta:resourcekey="rfvDeviceIdResource" ForeColor="Red" CssClass="validation rightclr"></asp:RequiredFieldValidator>

                <asp:Label runat="server" ID="lblPassword" Text="Password" CssClass="label leftclr" ></asp:Label>
                <asp:TextBox ID="tbPassword" runat="server" CssClass="input" MaxLength="100" TextMode="Password" />
               <%-- <asp:RequiredFieldValidator runat="server" ControlToValidate="tbPassword" ID="RequiredFieldValidator1" ErrorMessage="Password Required"
                    ForeColor="Red" CssClass="validation rightclr"></asp:RequiredFieldValidator>--%>

                <asp:CheckBox ID="chkMac" Text="IsMAC Used" runat="server"
                    CssClass="input leftclr" Checked="true" Style="padding-left:200px;"
                     />
            </div>
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>
        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnCreate" runat="server" Text="<%$ Resources:CommonLabels, Create %>" CssClass="button" meta:resourcekey="btnCreateResource"
                OnClick="btnCreate_Click" />
            <asp:Button ID="btnEdit" runat="server" Text="<%$ Resources:CommonLabels, Edit %>" CssClass="button" OnClick="btnEdit_Click" meta:resourcekey="btnEditResource1" />
            <asp:Button ID="btnUpdate" runat="server" Text="<%$ Resources:CommonLabels, Update %>" CssClass="button" meta:resourcekey="btnUpdateResource"
                OnClick="btnUpdate_Click" />
            <asp:Button ID="btnDelete" runat="server" Text="<%$ Resources: CommonLabels, Delete %>"
                CssClass="button" OnClick="btnDelete_Click" />
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources:CommonLabels, Confirm %>"
                CssClass="button" OnClick="btnConfirm_Click" meta:resourcekey="btnConfirmResource1" />
            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources:CommonLabels, Back %>"
                CssClass="button" OnClick="btnBack_Click" Visible="False" CausesValidation="False" meta:resourcekey="btnBackResource1" />
        </div>
    </div>
</asp:Content>
