<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="ManageAuthConfiguration.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.system.ManageAuthConfiguration" %>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblAuthConfigurationheading" runat="server" Text="Manage Authentication Configurations" />
            </div>
            <div>
                <asp:Label runat="server" ID="lblAuthconfiguration1" Text="Authentication Configuration Name"  CssClass="label leftclr"></asp:Label>
                
                <asp:TextBox runat="server" ID="tbAuthConfigurationName" CssClass="input" meta:resourcekey="tbAuthConfigurationNameResource"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="tbAuthConfigurationName" ID="rfvtbAuthConfigurationName"
                    meta:resourcekey="rfvAuthConfigurationNameResource" ErrorMessage="Authentication Configuration Name Required." ForeColor="Red" CssClass="validation rightclr"></asp:RequiredFieldValidator>



            </div>
            <div>
                <table style="width: 100%;">
                <tr>
                    <td style="width: 15%;"></td>
                    <td style="width: 25%">
                        <asp:Label ID="lblHeadProductionInterfaces" runat="server" Text="Authentication Interface" Font-Bold="true" />

                    </td>
                    <td style="width: 25%;">
                        <asp:Label ID="lblHeadIssuingInterfaces" runat="server" Text="External Interface" Font-Bold="true" />

                    </td>
                    <td style="width: 25%;">
                    </td>
                </tr>
                <tr>
                    <td style="width: 15%;">
                        <asp:Label ID="lblHSM" runat="server" Text="Auth Type" />
                    </td>
                    <td style="width: 25%;">
                        <asp:DropDownList ID="ddlInterface" runat="server" CssClass="input" Style="width: 80% !important" />
                    </td>
                    <td style="width: 25%;">
                        <asp:DropDownList ID="ddlexternalInterface" runat="server" CssClass="input" Style="width: 80% !important" />
                    </td>
                    <td style="width: 25%;">
                    </td>
                </tr>
              <tr>
                    <td style="width: 15%;"></td>
                    <td style="width: 25%">
                        <asp:Label ID="Label1" runat="server" Text="MultiFactor Interface" Font-Bold="true" />

                    </td>
                    <td style="width: 25%;">
                        <asp:Label ID="Label2" runat="server" Text="Interface Config" Font-Bold="true" />

                    </td>
                    <td style="width: 25%;">
                    </td>
                </tr>
                <tr>
                    <td style="width: 15%;">
                        <asp:Label ID="Label3" runat="server" Text="MultiFactor" />
                    </td>
                    <td style="width: 25%;">
                        <asp:DropDownList ID="ddlMultiFactorInterface" runat="server" CssClass="input" Style="width: 80% !important" />
                    </td>
                    <td style="width: 25%;">
                        <asp:DropDownList ID="ddlInterfaceConfig" runat="server" CssClass="input" Style="width: 80% !important" />
                    </td>
                    <td style="width: 25%;">
                    </td>
                </tr>
             </table>

            </div>
        </div>
        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" />
        </div>
        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnCreate" runat="server" CssClass="button" Text="Create" meta:resourcekey="btnCreateResource"
                OnClick="btnCreate_Click" />
            <asp:Button ID="btnEdit" runat="server" Text="<%$ Resources:CommonLabels, Edit %>"
                CssClass="button" OnClick="btnEdit_Click" meta:resourcekey="btnEditResource1" />
            <asp:Button ID="btnUpdate" runat="server" CssClass="button" Text="Update" meta:resourcekey="btnUpdateResource"
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