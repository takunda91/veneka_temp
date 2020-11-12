<%@ Page Title="Manage Font" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master"
    AutoEventWireup="true" CodeBehind="ManageFont.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.system.ManageFont"
    meta:resourcekey="PageResource" UICulture="auto" Culture="auto:en-US" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblFontHeading" runat="server" Text="Font Screen" meta:resourcekey="lblFontHeadingResource" />
            </div>
            <div>
                <asp:Label ID="lblFontname" runat="server" Text="Font Name" CssClass="label leftclr"
                    meta:resourcekey="lblFontnameResource" />
                <asp:TextBox ID="tbFontname" runat="server" CssClass="input" MaxLength="100" />
                <asp:RequiredFieldValidator ID="rfvfontname" runat="server" ControlToValidate="tbFontname"
                    ErrorMessage="Font Name Required." ForeColor="Red" CssClass="validation rightclr"
                    meta:resourcekey="rfvfontnameResource" />

                <div id="divfileupload" runat="server" visible="false">
                    <asp:Label ID="lblresourcepath" runat="server" Text="Resource Path" CssClass="label leftclr"
                        meta:resourcekey="lblresourcepathResource" />
                    <asp:FileUpload ID="furesourcepath" runat="server" CssClass="input rightclr" />
                </div>
                <div id="divFileuploadresult" runat="server" visible="false">
                    <asp:Label ID="lblresourcepath1" runat="server" Text="Resource Path" CssClass="label leftclr"
                        meta:resourcekey="lblresourcepathResource" />
                    <asp:Label ID="lblfilename" Text="" runat="server" CssClass="label rightclr"></asp:Label>
                </div>
            </div>
        </div>
        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>
        <div id="pnlButtons" class="ButtonPanel" runat="server">

            <asp:Button ID="btnCreate" runat="server" Text="Create Font" CssClass="button" meta:resourcekey="btnCreateResource"
                OnClick="btnCreate_Click" />
            <asp:Button ID="btnDelete" runat="server" Text="<%$ Resources: CommonLabels, Delete %>" CssClass="button"
                OnClick="btnDelete_Click" />
            <asp:Button ID="btnEdit" runat="server" Text="<%$ Resources: CommonLabels, Edit %>" CssClass="button"
                OnClick="btnEdit_Click" />

            <asp:Button ID="btnUpdate" runat="server" Text="Update Font" CssClass="button" meta:resourcekey="btnUpdateResource"
                OnClick="btnUpdate_Click" />

            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" CssClass="button"
                OnClick="btnConfirm_Click" />

            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources: CommonLabels, Back %>" CssClass="button" OnClick="btnBack_OnClick"
                Visible="False" CausesValidation="False" />
        </div>
    </div>
</asp:Content>
