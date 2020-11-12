<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="ExternalSystemFieldsViewForm.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.system.ExternalSystemFieldsViewForm"   UICulture="auto" Culture="auto:en-US" meta:resourcekey="PageResource" %>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblExternalSystemField" runat="server" Text="External System Field Details" meta:resourcekey="lblExternalSystemFieldResource" />
            </div>
            <div>
                <asp:Label runat="server" ID="lblExternalSystem" Text="External System" meta:resourcekey="lblExternalSystem" CssClass="label leftclr"></asp:Label>
                <asp:DropDownList runat="server" ID="ddlexternalsytem" CssClass="input rightclr" meta:resourcekey="ddlexternalsytemResource"></asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlexternalsytem" ErrorMessage="External System Required"
                    meta:resourcekey="rfvExternalSystemResource" CssClass="validation rightclr" ForeColor="Red"></asp:RequiredFieldValidator>

                <asp:Label runat="server" ID="lblExternalSystemfieldName" Text="Field Name" CssClass="label leftclr"
                    meta:resourcekey="lblExternalSystemNameResource"></asp:Label>
                <asp:TextBox runat="server" ID="txtExternalSystemfieldName" CssClass="input" meta:resourcekey="txtExternalSystemNameResource"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtExternalSystemfieldName" ID="rfvExternalSystemName"
                    meta:resourcekey="rfvExternalSystemNameResource" ErrorMessage="External System Name Required." ForeColor="Red" CssClass="validation rightclr"></asp:RequiredFieldValidator>

              

            </div>
        </div>
        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server"  />
        </div>
        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnCreate" runat="server" CssClass="button" meta:resourcekey="btnCreateResource"
                OnClick="btnCreate_Click" />
            <asp:Button ID="btnEdit" runat="server" Text="<%$ Resources:CommonLabels, Edit %>"
                CssClass="button" OnClick="btnEdit_Click" meta:resourcekey="btnEditResource1" />
            <asp:Button ID="btnUpdate" runat="server"  CssClass="button" meta:resourcekey="btnUpdateResource"
                OnClick="btnUpdate_Click" />
            <asp:Button ID="btnDelete" runat="server" Text="<%$ Resources: CommonLabels, Delete %>"
                CssClass="button" OnClick="btnDelete_Click" />
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources:CommonLabels, Confirm %>"
                CssClass="button" OnClick="btnConfirm_Click" />
            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources:CommonLabels, Back %>"
                CssClass="button" OnClick="btnBack_Click" Visible="False" CausesValidation="False" />
        </div>
    </div>
</asp:Content>