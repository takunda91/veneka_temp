<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="FileLoadList.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.load.FileLoadList" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            $("#<%= tbDateFrom.ClientID %>").datepicker({
                changeMonth: true,
                changeYear: true,
                maxDate: 1,
                dateFormat: '<%=DATEPICKER_FORMAT%>',
                onClose: function (selectedDate) {
                    $("#<%= tbDateTo.ClientID %>").datepicker("option", "minDate", selectedDate);
                }
            });
            $("#<%= tbDateTo.ClientID %>").datepicker({
                changeMonth: true,
                changeYear: true,
                maxDate: 1,
                dateFormat: '<%=DATEPICKER_FORMAT%>',
                onClose: function (selectedDate) {
                    $("#<%= tbDateFrom.ClientID %>").datepicker("option", "maxDate", selectedDate);
                }
            });
        });
    </script>

    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Literal ID="lblFileLoadResults" runat="server" meta:resourcekey="lblFileLoadResultsResource1" />
            </div>
            <div>
                <asp:Label ID="lblDateFrom" runat="server" Text="Date From" meta:resourcekey="lblDateFromResource" CssClass="label leftclr" />
                <asp:TextBox ID="tbDateFrom" runat="server" CssClass="input rightclr" />

                <asp:Label ID="lblDateTo" runat="server" Text="Date To" meta:resourcekey="lblDateToResource" CssClass="label leftclr" />
                <asp:TextBox ID="tbDateTo" runat="server" CssClass="input rightclr" />

                <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_OnClick" Text="Find" CssClass="button bothclr" meta:resourcekey="btnSearchResource" />

                <asp:DataList ID="dlFileloadlist" runat="server" HeaderStyle-Font-Bold="true" Width="100%"
                    CellPadding="0" ForeColor="#333333" OnItemCommand="dlFileloadlist_ItemCommand"
                    meta:resourcekey="dlFileloadlistResource1">
                    <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="White" />
                    <HeaderTemplate>
                        <tr style="font-weight: bold;">
                            <td colspan="0">
                                <asp:Label ID="lblhStart" runat="server" Text="Start" meta:resourcekey="lblDateFromResource" />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblhEnd" runat="server" Text="End" meta:resourcekey="lblDateToResource" />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblhNoOfFiles" runat="server" Text="No. of files" meta:resourcekey="lblhNumberOfFiles" />
                            </td>
                        </tr>
                    </HeaderTemplate>
                    <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                    <ItemTemplate>
                        <tr class="ItemSelect">
                            <td colspan="0">
                                <asp:Label ID="lblFileLoadId" runat="server"
                                    Text='<%# DataBinder.Eval(Container.DataItem, "file_load_id") %>'
                                    Visible="False" meta:resourcekey="lblFileLoadIdResource2" />
                                <asp:LinkButton ID="lnkStart" CommandName="Select" runat="server"
                                    Text='<%# DataBinder.Eval(Container.DataItem, "file_load_start", DATETIME_ASPX_FORMAT) %>'
                                    CssClass="ItemSelect" /></td>
                            <td colspan="0">
                                <asp:Label ID="lblEnd" runat="server"
                                    Text='<%# DataBinder.Eval(Container.DataItem, "file_load_end", DATETIME_ASPX_FORMAT) %>' />
                            </td>
                            <td colspan="0">
                                <asp:Label ID="lblNoOfFiles" runat="server"
                                    Text='<%# DataBinder.Eval(Container.DataItem, "files_to_process") %>' />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                </asp:DataList>
                <div class="PaginationPanel" id="pnlpage" runat="server">
                    <asp:LinkButton ID="lnkFirst" runat="server"
                        Text="<%$ Resources:CommonLabels, First %>" OnClick="lnkFirst_Click"
                        meta:resourcekey="lnkFirstResource2"></asp:LinkButton>
                    <asp:LinkButton ID="lnkPrev" runat="server"
                        Text="<%$ Resources:CommonLabels, Prev %>" OnClick="lnkPrev_Click"
                        meta:resourcekey="lnkPrevResource2"></asp:LinkButton>
                    <asp:Label ID="lblPageIndex" runat="server"
                        meta:resourcekey="lblPageIndexResource2" />
                    <asp:LinkButton ID="lnkNext" runat="server"
                        Text="<%$ Resources:CommonLabels, Next %>" OnClick="lnkNext_Click"
                        meta:resourcekey="lnkNextResource2"></asp:LinkButton>
                    <asp:LinkButton ID="lnkLast" runat="server"
                        Text="<%$ Resources:CommonLabels, Last %>" OnClick="lnkLast_Click"
                        meta:resourcekey="lnkLastResource2"></asp:LinkButton>
                </div>
            </div>
        </div>
        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" />
        </div>
    </div>
</asp:Content>
