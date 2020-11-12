<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="ExternalSystemFieldsManitanance.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.system.ExternalSystemFieldsManitanance"   UICulture="auto" Culture="auto:en-US" meta:resourcekey="PageResource" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblexternalsystem" runat="server" Text="External Systems" meta:resourcekey="lblexternalsystemHeadingResource" />
            </div>

           

            <asp:DataList ID="dlExternalSystemsfieldsList" runat="server"
                HeaderStyle-Font-Bold="true"
                Width="100%"
                OnItemCommand="dlExternalSystemsfieldsList_ItemCommand"
                CellPadding="0" ForeColor="#333333" meta:resourcekey="dlExternalSystemsfieldsListResource">

                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="White" />

                <HeaderTemplate>
                    <tr style="font-weight: bold;">
                        <td colspan="0">
                            <asp:Label ID="lblExternalSystemName" runat="server" Text="External System" meta:resourcekey="lblExternalSystemNameResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblExternalSystemfieldname" runat="server" Text="External System Field Name" meta:resourcekey="lblExternalSystemTypeResource" /></td>
                    </tr>
                </HeaderTemplate>

                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />

                <ItemTemplate>
                    <tr class="ItemSelect">
                        <td colspan="0">
                            <asp:Label ID="lblExternalSystemId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "external_system_field_id") %>' Visible="False" />
                            <asp:LinkButton ID="lblExternalSystemNameValue" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "system_name") %>' CssClass="ItemSelect" CommandName="Select" /></td>
                        <td colspan="0">
                            <asp:Label ID="lblExternalSystemTypevalue" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "field_name") %>' CommandName="Select" />
                        </td>


                    </tr>
                </ItemTemplate>
                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            </asp:DataList>

            <div class="PaginationPanel">
                <asp:LinkButton ID="lnkFirst" runat="server" Text="<%$ Resources:CommonLabels, First %>" OnClick="lnkFirst_Click" meta:resourcekey="lnkFirstResource1"></asp:LinkButton>
                <asp:LinkButton ID="lnkPrev" runat="server" Text="<%$ Resources:CommonLabels, Prev %>" OnClick="lnkPrev_Click" meta:resourcekey="lnkPrevResource1"></asp:LinkButton>
                <asp:Label ID="lblPageIndex" runat="server" meta:resourcekey="lblPageIndexResource1" />
                <asp:LinkButton ID="lnkNext" runat="server" Text="<%$ Resources:CommonLabels, Next %>" OnClick="lnkNext_Click" meta:resourcekey="lnkNextResource1"></asp:LinkButton>
                <asp:LinkButton ID="lnkLast" runat="server" Text="<%$ Resources:CommonLabels, Last %>" OnClick="lnkLast_Click" meta:resourcekey="lnkLastResource1"></asp:LinkButton>
            </div>
        </div>
        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>
    </div>
</asp:Content>

