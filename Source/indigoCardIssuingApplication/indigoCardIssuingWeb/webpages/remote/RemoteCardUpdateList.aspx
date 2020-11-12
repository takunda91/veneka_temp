<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="RemoteCardUpdateList.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.remote.RemoteCardUpdateList" Culture="auto:en-US" meta:resourcekey="PageResource" UICulture="auto" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblremoteCardUpdateList" Text="Remote Card Update List" runat="server" meta:resourcekey="lblremoteCardUpdateListResource" />
            </div>

            <asp:DataList ID="dlCardUpdateList" runat="server"
                OnItemCommand="dlCardUpdateList_ItemCommand"
                Width="100%"
                HeaderStyle-Font-Bold="true"
                HeaderStyle-ForeColor="Azure" Style="margin-top: 0px" CellPadding="0"
                ForeColor="#333333" Font-Names="Verdana" Font-Size="Small" meta:resourcekey="dlCardUpdateListResource1">

                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle Font-Bold="True" ForeColor="White"></HeaderStyle>

                <HeaderTemplate>
                    <tr style="font-weight: bold">
                        <td id="tdSelectH" runat="server">
                            <asp:Label ID="lblSelect" runat="server" Text=""  />
                        </td>
                        <td>
                            <asp:Label ID="lblCardNumber" runat="server" Text="Card Number" meta:resourcekey="lblCardNumberResource" />
                        </td>
                        <td>
                            <asp:Label ID="lblCardReferenceNumber" runat="server" Text="Card Reference Number" meta:resourcekey="lblCardReferenceNumberResource" />
                        </td>
                        <td>
                            <asp:Label ID="lblRemoteStatus" runat="server" Text="Remote Status" meta:resourcekey="lblRemoteStatusResource" />
                        </td>
                        <td>
                            <asp:Label ID="lblRemoteAddress" runat="server" Text="Remote Address" meta:resourcekey="lblRemoteAddressResource" />
                        </td>
                        <td>
                            <asp:Label ID="lbllastupdated" runat="server" Text="Last Updated" meta:resourcekey="lbllastupdatedResource" />
                        </td>
                        <td>
                            <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources:CommonLabels, Issuer %>" meta:resourcekey="lblIssuerResource1" />
                        </td>
                        <td>
                            <asp:Label ID="lblBranch" runat="server" Text="<%$ Resources:CommonLabels, Branch %>" meta:resourcekey="lblBranchResource1" />
                        </td>
                    </tr>

                </HeaderTemplate>
                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <ItemTemplate>

                    <tr class="ItemSelect">
                        <td id="tdSelect" runat="server">
                            <asp:CheckBox ID="chksel" runat="server" />
                        </td>
                        <td>                            
                            <asp:Label ID="lblCardId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "card_id") %>' Visible="False" meta:resourcekey="lblCardIdResource1" />
                            <asp:LinkButton ID="lbtnCardNumber" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "card_number") %>' CssClass="ItemSelect" meta:resourcekey="lbtnCardNumberResource1" />
                        </td>
                        <td>
                            <asp:Label ID="lblCardReferenceNumber" runat="server" CommandName="select" Text='<%# DataBinder.Eval(Container.DataItem, "card_request_reference") %>' meta:resourcekey="lblCardReferenceNumberResource1" />
                        </td>
                        <td>
                            <asp:Label ID="lblCardStatus" runat="server" CommandName="select" Text='<%# DataBinder.Eval(Container.DataItem, "remote_update_statuses_name") %>' meta:resourcekey="lblCardStatusResource1" />
                        </td>
                        <td>
                            <asp:Label ID="lbldRemoteAddress" runat="server" CommandName="select" Text='<%# DataBinder.Eval(Container.DataItem, "remote_component") %>' meta:resourcekey="lbldRemoteAddressResource1" />
                        </td>
                        <td>
                            <asp:Label ID="lblLastUpdated" runat="server" CommandName="select" Text='<%# DataBinder.Eval(Container.DataItem, "status_date", DATETIME_ASPX_FORMAT) %>' meta:resourcekey="lblLastUpdatedResource1" />
                        </td>
                        <td>
                            <asp:Label ID="lbldlIssuerName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "issuer_name") %>' meta:resourcekey="lbldlIssuerNameResource1" />
                        </td>
                        <td>
                            <asp:Label ID="lbldlBranchName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "branch_name") %>' meta:resourcekey="lbldlBranchNameResource1" />
                        </td>
                    </tr>

                </ItemTemplate>

                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />

                <SelectedItemTemplate>
                </SelectedItemTemplate>
            </asp:DataList>

            <div class="PaginationPanel">
                <asp:LinkButton ID="lnkFirst" runat="server" Text="<%$ Resources:CommonLabels, First %>" OnClick="lnkFirst_Click" meta:resourcekey="lnkFirstResource1"></asp:LinkButton>
                <asp:LinkButton ID="lnkPrev" runat="server" Text="<%$ Resources:CommonLabels, Prev %>" OnClick="lnkPrev_Click" meta:resourcekey="lnkPrevResource1"></asp:LinkButton>
                <asp:Label ID="lblPageIndex" runat="server" meta:resourcekey="lblPageIndexResource1" />
                <asp:LinkButton ID="lnkNext" runat="server" Text="<%$ Resources:CommonLabels, Next %>" OnClick="lnkNext_Click" meta:resourcekey="lnkNextResource1"></asp:LinkButton>
                <asp:LinkButton ID="lnkLast" runat="server" Text="<%$ Resources:CommonLabels, Last %>" OnClick="lnkLast_Click" meta:resourcekey="lnkLastResource1"></asp:LinkButton>
            </div>

            <div class="PaginationPanel">
                <asp:Label runat="server" Text="Total Records: " meta:resourcekey="lblTotalRecordsResource" ID="lblTotalRecords" />
                <asp:Label ID="lblTotalRecords2" runat="server" Text="0" meta:resourcekey="lblTotalRecords2Resource1" />
            </div>
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnResend" runat="server" Text="Resend" CssClass="button" OnClick="btnResend_OnClick" Visible="False" />
            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources: CommonLabels, Back %>" CssClass="button" OnClick="btnBack_OnClick" Visible="False" />
        </div>
    </div>
</asp:Content>
