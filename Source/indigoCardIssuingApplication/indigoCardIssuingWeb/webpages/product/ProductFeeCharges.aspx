<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="ProductFeeCharges.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.product.ProductFeeCharges" %>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblProductscreenheading" runat="server" Text="Manage Fee Charges" meta:resourcekey="lblProductscreenheadingResource" />
            </div>

            <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlIssuer" runat="server" CssClass="input rightclr" OnSelectedIndexChanged="ddlIssuer_OnSelectedIndexChanged" AutoPostBack="true" />

            <asp:Label ID="lblScheme" runat="server" Text="Fee Scheme" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlProductScheme" runat="server" CssClass="input rightclr"
                OnSelectedIndexChanged="ddlProductScheme_OnSelectedIndexChanged" AutoPostBack="true" />

            <asp:Label ID="lblFeeType" runat="server" Text="Fee band" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlFeeType" runat="server" OnSelectedIndexChanged="ddlFeeType_OnSelectedIndexChanged"
                AutoPostBack="true" CssClass="input righclr" />


            <div style="clear: both; width: 99%; height: 200px; overflow: Auto">
                <asp:Panel ID="pnlPrintingFields" runat="server" CssClass="bothclr" GroupingText="Printing field settings">
                            <asp:GridView ID="grdCharges" runat="server" AllowPaging="false" DataKeyNames="Id"
                                AutoGenerateColumns="False" OnRowDeleting="DeleteRow" OnRowDataBound="grdCharges_RowDataBound" OnRowUpdating="grdCharges_RowUpdating"
                                OnRowCancelingEdit="grdCharges_RowCancelingEdit" OnRowEditing="grdCharges_RowEditing" OnRowCommand="grdCharges_RowCommand"
                                 ShowFooter="true" EmptyDataText="No Rows Returned">
                                <AlternatingRowStyle backcolor="White" forecolor="#284775" />
                                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="Black" />
                                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                          <asp:Label ID="lblCurrency" runat="server" Text="Currency" />
                                        </HeaderTemplate>
                                        <EditItemTemplate>
                                             <asp:DropDownList ID="ddlCurrencycode" runat="server"  Width="150">
                                            </asp:DropDownList>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                          <asp:DropDownList ID="ddlCurrencycode" runat="server"  Width="150">
                                            </asp:DropDownList>
                                            
                                        </ItemTemplate>
                                        <FooterTemplate>
                                             <asp:DropDownList ID="ddlCurrencycode" runat="server"  Width="150">
                                            </asp:DropDownList>
                                        </FooterTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                           <asp:Label ID="lblIssuingScenario" runat="server" Text="Issuing Scenario"  />
                                        </HeaderTemplate>
                                        <EditItemTemplate>
                                             <asp:DropDownList ID="ddlIssuingScenario" runat="server"  Width="150">
                                            </asp:DropDownList>
                                        </EditItemTemplate>
                                        <ItemTemplate>
 <asp:DropDownList ID="ddlIssuingScenario" runat="server"  Width="150">
                                            </asp:DropDownList>
                                            
                                        </ItemTemplate>
                                        <FooterTemplate>
                                             <asp:DropDownList ID="ddlIssuingScenario" runat="server"  Width="150">
                                            </asp:DropDownList>
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                         <asp:Label ID="lblFeeCharge" runat="server" Text="Charge" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblFeeCharge" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "fee_charge") %>' />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                              <asp:TextBox ID="tbFeecharge" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "fee_charge") %>' OnKeyPress="return isNumericValue(event,this,6,4)" />
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                             <asp:TextBox ID="tbFeecharge" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "fee_charge") %>' OnKeyPress="return isNumericValue(event,this,6,4)" />
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                   <asp:TemplateField>
                                        <HeaderTemplate>
                                         <asp:Label ID="lblCBSAccountType" runat="server" Text="CBS Account Type" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblCBSAccountType" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "cbs_account_type") %>' />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                              <asp:TextBox ID="tbAccountType" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "cbs_account_type") %>'  />
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                             <asp:TextBox ID="tbAccountType" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "cbs_account_type") %>'  />
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Field Name" ItemStyle-Width="150">
                                        <HeaderTemplate>
                                            <asp:Label ID="lblVatRate" runat="server" Text="VAT Rate" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblVatRate" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "vat") %>' />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                           
                                             <asp:TextBox ID="tbVatRate" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "vat") %>' OnKeyPress="return isNumericValue(event,this,6,4)" />
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                           
                                             <asp:TextBox ID="tbVatRate" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "vat") %>' OnKeyPress="return isNumericValue(event,this,6,4)" />
                                        </FooterTemplate>

                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Edit" ShowHeader="False">

                                <EditItemTemplate>

                                    <asp:LinkButton ID="lnkupdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update"></asp:LinkButton>

                                    <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>

                                </EditItemTemplate>

                                <FooterTemplate>

                                    <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="False" CommandName="Create" Text="Create"></asp:LinkButton>

                                </FooterTemplate>

                                <ItemTemplate>

                                    <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit"></asp:LinkButton>

                                </ItemTemplate>

                            </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </asp:Panel>

                
            </div>

        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnEdit" runat="server" Text="<%$ Resources: CommonLabels, Edit %>"
                CssClass="button" OnClick="btnEdit_Click" />

            <asp:Button ID="btnUpdate" runat="server" Text="Update Product" CssClass="button"
                meta:resourcekey="btnUpdateResource" OnClick="btnUpdate_Click" />

            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>"
                CssClass="button" OnClick="btnConfirm_Click" />

            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources: CommonLabels, Back %>"
                CssClass="button" OnClick="btnBack_OnClick" Visible="False" CausesValidation="False" />
        </div>
    </div>
</asp:Content>
