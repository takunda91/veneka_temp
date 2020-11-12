<%@ Page Title="External System Details" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="ExternalSystemsViewForm.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.system.ExternalSystemsViewForm" UICulture="auto" Culture="auto:en-US" meta:resourcekey="PageResource" %>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblExternalSystem" runat="server" Text="External System Details" meta:resourcekey="lblExternalSystemResource" />
            </div>
            <div>
                <asp:Label runat="server" ID="lblExternalSystemTypes" Text="External System Type" meta:resourcekey="lblExternalSystemTypesResource" CssClass="label leftclr"></asp:Label>
                <asp:DropDownList runat="server" ID="ddlexternalsytemtype" CssClass="input rightclr"></asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlexternalsytemtype" ErrorMessage="External System Type Required"
                    meta:resourcekey="rfvExternalSystemtypeResource" CssClass="validation rightclr" ForeColor="Red"></asp:RequiredFieldValidator>

                <asp:Label runat="server" ID="lblExternalSystemName" Text="External System Name" CssClass="label leftclr"
                    meta:resourcekey="lblExternalSystemNameResource"></asp:Label>
                <asp:TextBox runat="server" ID="txtExternalSystemName" CssClass="input" meta:resourcekey="txtExternalSystemNameResource"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtExternalSystemName" ID="rfvExternalSystemName"
                    meta:resourcekey="rfvExternalSystemNameResource" ErrorMessage="External System Name Required." ForeColor="Red" CssClass="validation rightclr"></asp:RequiredFieldValidator>



            </div>
            <div>
                <asp:GridView ID="GrdExternalSystemFields" runat="server" CellPadding="1" CssClass="bothclr" GroupingText="External System Fields" EmptyDataText="No Rows Returned"  GridLines="None"
                    AutoGenerateColumns="false" Width="80%" OnRowEditing="EditRow" OnRowDataBound="GrdExternalSystemFields_RowDataBound" OnRowCancelingEdit="CancelEditRow" OnRowCommand="GrdExternalSystemFields_RowCommand" ShowFooter="true"
                    OnRowUpdating="UpdateRow" DataKeyNames="external_system_field_id" OnRowDeleting="DeleteRow" AllowPaging="true"
                    OnPageIndexChanging="ChangePage">

                    <Columns>
                        <asp:TemplateField HeaderText="Edit">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" Text="Edit" CommandName="Edit" />
                                <asp:LinkButton ID="lnkDelete" runat="server" Text="Delete" CommandName="Delete" />

                            </ItemTemplate>

                            <EditItemTemplate>
                                <asp:LinkButton ID="lnkUpdate" runat="server" Text="Update" CommandName="Update" />
                                <asp:LinkButton ID="lnkCancel" runat="server" Text="Cancel" CommandName="Cancel" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkAdd" runat="server" Text="Create" CommandName="Create" />
                            </FooterTemplate>
                        </asp:TemplateField>

                        <asp:BoundField HeaderText="external_system_field_id" DataField="external_system_field_id" Visible="false" ReadOnly="true" />

                        <asp:TemplateField HeaderText="Field Name">
                            <ItemTemplate>
                                <asp:Label ID="lblfieldname" runat="server" Text='<%# Eval("field_name") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="etfieldname" runat="server" Text='<%# Bind("field_name") %>' />

                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="ftfieldname" runat="server" Text='<%# Eval("field_name") %>' />

                            </FooterTemplate>
                        </asp:TemplateField>

                    </Columns>
                    <AlternatingRowStyle backcolor="White" forecolor="#284775" />
                    <FooterStyle BackColor="White" Font-Bold="True" ForeColor="Black" />
                    <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="Black" />
                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />

                </asp:GridView>

            </div>
        </div>
        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" />
        </div>
        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnCreate" runat="server" CssClass="button" meta:resourcekey="btnCreateResource"
                OnClick="btnCreate_Click" />
            <asp:Button ID="btnEdit" runat="server" Text="<%$ Resources:CommonLabels, Edit %>"
                CssClass="button" OnClick="btnEdit_Click" meta:resourcekey="btnEditResource1" />
            <asp:Button ID="btnUpdate" runat="server" CssClass="button" meta:resourcekey="btnUpdateResource"
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
