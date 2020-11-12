<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="ManageBranch.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.issuer.ManageBranch" UICulture="auto" Culture="auto:en-US" meta:resourcekey="PageResource" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblHeader" runat="server" Text="Branch Details" meta:resourcekey="lblBranchDetailsResource" />
            </div>
            <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlIssuer" runat="server" CssClass="input rightclr" />

            <asp:Label ID="lblBranchName" runat="server" Text="Branch name" CssClass="label leftclr" meta:resourcekey="lblBranchNameResource" />
            <asp:TextBox ID="tbBranchName" runat="server" CssClass="input" />
            <asp:RequiredFieldValidator ID="rfvBranchname" runat="server" ControlToValidate="tbBranchName" ForeColor="red"
                ErrorMessage="Branch Name Required" CssClass="validation rightclr"
                meta:resourcekey="rfvBranchnameResource" />

            <asp:Label ID="lblBranchCode" runat="server" Text="Branch code" CssClass="label leftclr" meta:resourcekey="lblBranchCodeResource" />
            <asp:TextBox ID="tbBranchCode" runat="server" CssClass="input" />
            <asp:RequiredFieldValidator ID="rfvBranchCode" runat="server" ControlToValidate="tbBranchCode" ForeColor="red"
                ErrorMessage="Branch Code Required" CssClass="validation rightclr"
                meta:resourcekey="rfvBranchCodeResource" />

             <asp:Label ID="lblEmpBranchCode" runat="server" Text="Emp Branch code" CssClass="label leftclr" meta:resourcekey="lblEmpBranchCodeResource" />
            <asp:TextBox ID="tbEmpBranchCode" runat="server" CssClass="input" />

            <asp:Label ID="lblBranchType" runat="server" Text="Branch Type" CssClass="label leftclr" meta:resourcekey="lblBranchTypeResource" />
            <asp:DropDownList ID="ddlBranchType" runat="server" CssClass="input rightclr"  OnSelectedIndexChanged="ddlBranchType_SelectedIndexChanged" AutoPostBack="true" />


            <div id="divbranchtype" style="display:none" runat="server">
                    <asp:Label ID="lblBranch" runat="server" Text="Branch" CssClass="label leftclr" meta:resourcekey="lblBranchResource" />
            <asp:DropDownList ID="ddlBranch" runat="server" CssClass="input rightclr"   />
            </div>
              <asp:Panel ID="pnlsatellitebranches" runat="server" CssClass="bothclr" Visible="false" GroupingText="satellite branches"  Width="80%">
                     <asp:GridView ID="grdsatellite" runat="server" AutoGenerateColumns="false" Width="100%" Style="padding-left:30%;overflow: auto" OnRowDeleting="grdsatellite_RowDeleting" ShowFooter="true" OnRowDataBound="grdsatellite_RowDataBound" DataKeyNames="branch_id" OnPageIndexChanging="grdsatellite_PageIndexChanging" PageSize="20"  OnRowCommand="grdsatellite_RowCommand" EmptyDataText="No Rows Returned">
                                <AlternatingRowStyle backcolor="White" forecolor="#284775" />
                                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="Black" />
                                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                         <Columns>
                             <asp:TemplateField ItemStyle-Width="150px" HeaderText="Branch Name">
                                 <ItemTemplate>
                                     <asp:Label ID="lblbranchname" runat="server"  Text='<%# Eval("branch_name")%>'></asp:Label>
                                 </ItemTemplate>
                                 <FooterTemplate>
                                    <asp:DropDownList ID="ddlBranch" runat="server" Width="150px" CssClass="input rightclr"   />
                                 </FooterTemplate>

                             </asp:TemplateField>
                             <asp:TemplateField HeaderText="Action">

                                 <ItemTemplate>

                                     <asp:LinkButton ID="lnkRemove" runat="server"
                                         OnClientClick="return confirm('Do you want to delete?')"
                                         Text="Delete" CommandName="Delete" CommandArgument='<%# Container.DataItemIndex %>' ></asp:LinkButton>

                                 </ItemTemplate>

                                 <FooterTemplate>

                                     <asp:Button ID="btnAdd" runat="server" Text="Add"
                                          CommandName="Add" />

                                 </FooterTemplate>

                             </asp:TemplateField>
                         </Columns>
                     </asp:GridView>

                </asp:Panel>
            <%--<asp:Label ID="lblCardCentreBranch" runat="server" Text="Card Centre Branch" CssClass="label leftclr" />
            <asp:CheckBox ID="chkCardCentreBranch" runat="server" CssClass="input rightclr" />--%>

            <asp:Label ID="lblStatus" runat="server" Text="Status" CssClass="label leftclr" meta:resourcekey="lblStatusResource" />
            <asp:DropDownList ID="ddlBranchStatus" runat="server" CssClass="input rightclr" />

            <asp:Label ID="lblBranchLocation" runat="server" Text="Location" CssClass="label leftclr" meta:resourcekey="lblBranchLocationResource" />
            <asp:TextBox ID="tbLocation" runat="server" CssClass="input rightclr" />

            <asp:Label ID="lblBranchContactPerson" runat="server" Text="Contact person" CssClass="label leftclr" meta:resourcekey="lblBranchContactPersonResource" />
            <asp:TextBox ID="tbContactPerson" runat="server" CssClass="input rightclr" />

            <asp:Label ID="lblContactEmail" runat="server" Text="Contact email" CssClass="label leftclr" meta:resourcekey="lblContactEmailResource" />
            <asp:TextBox ID="tbContactEmail" runat="server" CssClass="input rightclr" />
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnCreate" runat="server" Text="<%$ Resources: CommonLabels, Create %>" OnClick="btnCreate_OnClick" CssClass="button" />
            <asp:Button ID="btnEdit" runat="server" Text="<%$ Resources: CommonLabels, Edit %>" OnClick="btnEdit_OnClick" CssClass="button" />
            <asp:Button ID="btnUpdate" runat="server" Text="<%$ Resources: CommonLabels, Update %>" OnClick="btnUpdate_OnClick" CssClass="button" />
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" OnClick="btnConfirm_OnClick" CssClass="button" />

            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources: CommonLabels, Back %>" OnClick="btnBack_OnClick" CausesValidation="False" CssClass="button" />
        </div>
    </div>
</asp:Content>
