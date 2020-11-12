<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="InterfaceList.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.system.InterfaceList" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div class="ContentHeader">
            <asp:Label ID="lblHeader" runat="server" Text="Interface List" meta:resourcekey="lblHeaderResource" />
        </div>
        <asp:DataList ID="dlInterfaceList" runat="server" DataKeyField="connection_parameter_id"
            OnItemCommand="dlInterfaceList_ItemCommand" CellPadding="0"
            ForeColor="#333333" Width="400px" CaptionAlign="Left"
            Style="text-align: left" meta:resourcekey="dlInterfaceListResource">
            <AlternatingItemStyle ForeColor="#284775" />
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <HeaderStyle Font-Bold="True" ForeColor="White" HorizontalAlign="Left" />
            <HeaderTemplate>
                <tr style="width: 800px; font-weight: bold;">
                    <td style="padding-right: 2px;">
                        <asp:Literal ID="lblInterfaceSettingName" Text="Interface Name" runat="server" meta:resourcekey="lblInterfaceSettingNameResource" />
                    </td>
                    <td>
                        <asp:Label ID="lblInterfaceAddress" Text="Interface Address" runat="server" meta:resourcekey="lblInterfaceAddressResource1" />
                    </td>
                    <td></td>
                </tr>
            </HeaderTemplate>
            <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <ItemTemplate>
                <tr class="ItemSelect">
                    <td>
                        <asp:LinkButton ID="lbtnInterfaceSettingName" CommandName="select" Text='<%# DataBinder.Eval(Container.DataItem, "connection_name") %>' runat="server" meta:resourcekey="lbtnInterfaceSettingNameResource" />
                    </td>
                    <td>
                        <asp:Label ID="lblAddress" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "address") %>' meta:resourcekey="lblAddressResource1" />
                    </td>
                    <td>
                        <asp:Label ID="lblConnectionParameterId" runat="server" Visible="False" Text='<%# DataBinder.Eval(Container.DataItem, "connection_parameter_id") %>' meta:resourcekey="lblConnectionParameterIdResource1" />
                    </td>
                </tr>
            </ItemTemplate>
            <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <SelectedItemTemplate>
            </SelectedItemTemplate>
        </asp:DataList>
        <div class="PaginationPanel">
            <asp:LinkButton ID="lnkFirst" runat="server" Text="<%$ Resources:CommonLabels, First %>" OnClick="lnkFirst_Click" meta:resourcekey="lnkFirstResource"></asp:LinkButton>
            <asp:LinkButton ID="lnkPrev" runat="server" Text="<%$ Resources:CommonLabels, Prev %>" OnClick="lnkPrev_Click" meta:resourcekey="lnkPrevResource"></asp:LinkButton>
            <asp:Label ID="lblPageIndex" runat="server" meta:resourcekey="lblPageIndexResource" />
            <asp:LinkButton ID="lnkNext" runat="server" Text="<%$ Resources:CommonLabels, Next %>" OnClick="lnkNext_Click" meta:resourcekey="lnkNextResource"></asp:LinkButton>
            <asp:LinkButton ID="lnkLast" runat="server" Text="<%$ Resources:CommonLabels, Last %>" OnClick="lnkLast_Click" meta:resourcekey="lnkLastResource"></asp:LinkButton>
        </div>
        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>
    </div>
</asp:Content>
