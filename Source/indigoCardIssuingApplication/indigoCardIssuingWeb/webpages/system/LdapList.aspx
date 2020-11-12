<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" 
    CodeBehind="LdapList.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.system.LdapList" culture="auto"
     meta:resourcekey="PageResource1" uiculture="auto" %>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">

        <div class="ContentHeader">
            <asp:Label ID="lblHeader" runat="server" Text="LDAP List" meta:resourcekey="lblHeaderResource" />
        </div>

        <%--<div id="divInputs" runat="server">
            <asp:Label ID="lblLdapList" runat="server" Text="LDAP Settings" CssClass="label leftclr" meta:resourcekey="lblLdapListResource" />
            <asp:DropDownList ID="ddlLdapSettings" runat="server" CssClass="input rightclr" AutoPostBack="true"
                OnSelectedIndexChanged="ddlLdapSettings_SelectedIndexChanged" CausesValidation="false" />
        </div>--%>

        <asp:DataList ID="dlLDAPList" runat="server" DataKeyField="ldap_setting_id"
            OnItemCommand="dlLDAPList_ItemCommand" CellPadding="0"
            ForeColor="#333333" Width="400px" CaptionAlign="Left"
            Style="text-align: left" meta:resourcekey="dlLDAPListResource1">
            <AlternatingItemStyle ForeColor="#284775" />
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <HeaderStyle Font-Bold="True" ForeColor="White" HorizontalAlign="Left" />
            <HeaderTemplate>
                <tr style="width: 800px; font-weight: bold;">
                    <td style="padding-right: 2px;">
                        <asp:Literal ID="lblLdapSettingName" Text="Setting Name" runat="server" meta:resourcekey="lblLdapSettingNameResource" />
                    </td>
                    <td style="padding-right: 2px;">
                        <asp:Literal ID="lblHostName" Text="Host Name" runat="server" meta:resourcekey="lblLdapHostNameResource" />
                    </td>
                    <td style="padding-right: 2px;">
                        <asp:Literal ID="lblPath" Text="Path" runat="server" meta:resourcekey="lblLdapPathResource" />
                    </td>
                    <td></td>
                </tr>
            </HeaderTemplate>
            <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <ItemTemplate>
                <tr class="ItemSelect">
                    <td>
                        <asp:LinkButton ID="lbtnLdapSettingName" CommandName="select" Text='<%# DataBinder.Eval(Container.DataItem, "ldap_setting_name") %>' runat="server" meta:resourcekey="lbtnLdapSettingNameResource1" />
                    </td>
                    <td>
                        <asp:Label ID="lbtnHostName" Text='<%# DataBinder.Eval(Container.DataItem, "hostname_or_ip") %>' runat="server" meta:resourcekey="lbtnHostNameResource1" />
                    </td>
                    <td>
                        <asp:Label ID="lbtnPath" Text='<%# DataBinder.Eval(Container.DataItem, "path") %>' runat="server" meta:resourcekey="lbtnPathResource1" />
                    </td>
                    <td>
                        <asp:Label ID="lblLdapID" runat="server" Visible="False" Text='<%# DataBinder.Eval(Container.DataItem, "ldap_setting_id") %>' meta:resourcekey="lblLdapIDResource1"  />
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
        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>
        <div class="ButtonPanel">
            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources:CommonLabels, Back %>" CssClass="button" OnClick="btnBack_Click" Visible="False" meta:resourcekey="btnBackResource1" />
        </div>
    </div>
</asp:Content>
