<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true"
    CodeBehind="FileLoaderLogView.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.load.FileLoaderLogView"
    UICulture="auto" meta:resourcekey="PageResource" Culture="auto:en-US" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            $('.dlg').puidialog({
                minimizable: false,
                maximizable: false,
                modal: true,
                width: 750,
                height: 300,
                buttons: [{
                    text: 'Close',
                    icon: 'ui-icon-check',
                    click: function () {
                        $('.dlg').puidialog('hide');
                    }
                }
                ]
            });

            $('.btn-show-comments').puibutton({
                icon: 'ui-icon-arrow-4-diag',
                click: function () {
                    $(this).prev(".dlg").puidialog('show');
                }
            });
        });
    </script>

    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Literal ID="lblFileLoadResults" runat="server" meta:resourcekey="lblFileLoadResultsResource" />
            </div>
            <div>
                <asp:DataList ID="dlFileloaderloglist" runat="server" HeaderStyle-Font-Bold="true" Width="100%"
                    CellPadding="0" ForeColor="#333333">
                    <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="White" />
                    <HeaderTemplate>
                        <tr style="font-weight: bold;">
                            <td colspan="0">
                                <asp:Label ID="lblhDate" runat="server" Text="Date" meta:resourcekey="lblhDateResource" />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblhFileName" runat="server" Text="File Name" meta:resourcekey="lblhFileNameResource" />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblhStatus" runat="server" Text="Status" meta:resourcekey="lblhStatusResource" />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblhFileComments" runat="server" Text="File Comments" meta:resourcekey="lblhFileCommentsResource" />
                            </td>
                        </tr>
                    </HeaderTemplate>
                    <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                    <ItemTemplate>
                        <tr class="ItemSelect">
                            <td colspan="0">
                                <asp:Label ID="lblDate" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "load_date", DATETIME_ASPX_FORMAT) %>' />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblFileName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "name_of_file") %>' />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblStatus" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "file_status") %>' />
                            </td>
                            <td colspan="0">
                                <div class="dlg" runat="server" title="<%$ Resources: lblhFileCommentsResource.Text %>">
                                    <asp:Label ID="lblFileComments" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "file_load_comments") %>' />
                                </div>
                                <button class="btn-show-comments" type="button">Show</button>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                </asp:DataList>
            </div>

            <div class="PaginationPanel" id="pnlpage" runat="server">
                <asp:LinkButton ID="lnkFirst" runat="server" Text="<%$ Resources: CommonLabels, First %>" OnClick="lnkFirst_Click"> </asp:LinkButton>
                <asp:LinkButton ID="lnkPrev" runat="server" Text="<%$ Resources: CommonLabels, Prev %>" OnClick="lnkPrev_Click"> </asp:LinkButton>
                <asp:Label ID="lblPageIndex" runat="server" />
                <asp:LinkButton ID="lnkNext" runat="server" Text="<%$ Resources: CommonLabels, Next %>" OnClick="lnkNext_Click"> </asp:LinkButton>
                <asp:LinkButton ID="lnkLast" runat="server" Text="<%$ Resources: CommonLabels, Last %>" OnClick="lnkLast_Click"> </asp:LinkButton>
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
        <asp:Button ID="btnBack" runat="server" Text="<%$ Resources: CommonLabels, Back %>" CssClass="button" OnClick="btnBack_Click"
            CausesValidation="False" />
    </div>
</asp:Content>
