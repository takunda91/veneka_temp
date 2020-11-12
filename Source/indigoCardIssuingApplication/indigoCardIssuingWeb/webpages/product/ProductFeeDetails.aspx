<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="ProductFeeDetails.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.product.ProductFeeDetails" %>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            $('.dlg').puidialog({
                minimizable: false,
                maximizable: false,
                modal: true,
                width: 450,
                height: 100,
                buttons: [{
                    text: 'Done',
                    icon: 'ui-icon-check',
                    click: function () {
                        __doPostBack('', 'btnAddDetailDone')
                    }
                }, {
                    text: 'Close',
                    icon: 'ui-icon-close',
                    click: function () {
                        $('.dlg').puidialog('hide');
                    }
                }
                ]
            });

            $('.btn-show-comments').puibutton({
                icon: 'ui-icon-plus',
                click: function () {
                    $(this).prev(".dlg").puidialog('show');
                }
            });
        });

    </script>

    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblProductscreenheading" runat="server" Text="Fee Scheme Admin" />
            </div>

            <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlIssuer" runat="server" CssClass="input rightclr" AutoPostBack="true" OnSelectedIndexChanged="ddlIssuer_SelectedIndexChanged" />

            <asp:Label ID="lblSchemeName" runat="server" Text="Fee Scheme" CssClass="label leftclr" meta:resourcekey="lblProductnameResource" />
            <asp:TextBox ID="tbSchemeName" runat="server" CssClass="input" MaxLength="100" />
            <asp:RequiredFieldValidator ID="rfvSchemeName" runat="server" ControlToValidate="tbSchemeName"
                ErrorMessage="Scheme Name Required." ForeColor="Red" CssClass="validation rightclr" />

            <asp:Label ID="lblFeeAccountingId" runat="server" Text="Accounting" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlFeeAccounting" runat="server" CssClass="input" />

            <asp:DataList ID="dlFeeDetails" runat="server" Width="100%"
                HeaderStyle-Font-Bold="true"
                HeaderStyle-ForeColor="Azure"
                CellPadding="0"
                ForeColor="#333333"
                OnItemCommand="dlFeeDetails_ItemCommand">

                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle Font-Bold="True" ForeColor="White" BackColor="White"></HeaderStyle>
                <HeaderTemplate>
                    <tr style="font-weight: bold;">
                        <td>
                            <asp:Label ID="lblFeeNameH" runat="server" Text="Fee Type" />
                        </td>
                        <td>
                            <asp:Label ID="lblWaiverH" runat="server" Text="Allow Waiver" />
                        </td>
                        <td>
                            <asp:Label ID="lblEditableH" runat="server" Text="Fee Editable" />
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <ItemTemplate>
                    <tr class="ItemSelect">
                        <td>
                            <asp:Label ID="lblDlFeeDetailsId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "fee_detail_id") %>' Visible="false" />
                            <asp:Label ID="lblDlFeeDetailsName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "fee_detail_name") %>' />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkbFeeWaiver" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "fee_waiver_YN") %>' Enabled="false" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkbFeeEditable" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "fee_editable_YN") %>' Enabled="false" />
                        </td>
                        <td>
                            <asp:Button ID="btnedit" runat="server" Text="<%$ Resources: CommonLabels, Edit %>" CssClass="button" CommandName="edit" />
                            <asp:Button ID="btnRemoveDetail" runat="server" Text="Remove" CssClass="button btnremove" CommandName="remove" />
                        </td>
                    </tr>
                </ItemTemplate>

                <EditItemTemplate>
                    <tr class="ItemSelect">
                        <td>
                            <asp:Label ID="lblFeeDetailsId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "fee_detail_id") %>' Visible="false" />
                            <asp:TextBox ID="tbFeeDetailsName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "fee_detail_name") %>' Enabled="true" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkFeeWaiver" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "fee_waiver_YN") %>' Enabled="true" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkFeeEditable" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "fee_editable_YN") %>' />
                        </td>
                        <td>
                            <asp:Button ID="btnupdate" runat="server" CommandName="update" CssClass="button" Text="<%$ Resources: CommonLabels, Update %>" />
                            <asp:Button ID="btncancel" runat="server" CommandName="cancel" CssClass="button" Text="<%$ Resources: CommonLabels, Cancel %>" />
                        </td>
                    </tr>
                </EditItemTemplate>
                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <SelectedItemTemplate>
                </SelectedItemTemplate>
            </asp:DataList>

            <div class="dlg" runat="server" title="Fee Band Details">
                <asp:Label ID="lblFeeDetailName" runat="server" Text="Fee band name" CssClass="label leftclr" />
                <asp:TextBox ID="tbFeeDetailName" runat="server" CssClass="input" />


                <asp:Label ID="lblFeeWaiver" runat="server" Text="Allow Waiver" CssClass="label leftclr" />
                <asp:CheckBox ID="chkbFeeWaiver" runat="server" CssClass="input rightclr" />

                <asp:Label ID="lblFeeEditable" runat="server" Text="Fee Editable" CssClass="label leftclr" />
                <asp:CheckBox ID="chkbFeeEditable" runat="server" CssClass="input rightclr" />
            </div>

            <button id="btnAddFeeDetail" runat="server" class="btn-show-comments bothclr button" style="float: left" type="button">add fee band</button>
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnCreate" runat="server" Text="Create Fee Scheme" CssClass="button"
                meta:resourcekey="btnCreateResource" OnClick="btnCreate_Click" />

            <asp:Button ID="btnEdit" runat="server" Text="<%$ Resources: CommonLabels, Edit %>"
                Visible="false" CssClass="button" OnClick="btnEdit_Click" />

            <asp:Button ID="btnUpdate" runat="server" Text="Update Fee Scheme" CssClass="button"
                Visible="false" meta:resourcekey="btnUpdateResource" OnClick="btnUpdate_Click" />

            <asp:Button ID="btnDelete" runat="server" Text="<%$ Resources: CommonLabels, Delete %>"
                Visible="false" CssClass="button" OnClick="btnDelete_Click" />

            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>"
                Visible="false" CssClass="button" OnClick="btnConfirm_Click" />

            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources: CommonLabels, Back %>"
                Visible="false" CssClass="button" OnClick="btnBack_OnClick" CausesValidation="False" />
        </div>
    </div>
</asp:Content>
