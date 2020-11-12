﻿<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="AuthConfigurationList.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.system.AuthConfigurationList"  UICulture="auto" Culture="auto:en-US" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblAuthConfiguration" runat="server" Text="Authentication Configuration List" meta:resourcekey="lblnotificationbatchHeadingResource" />
            </div>

           
            <asp:DataList ID="dlauthconfiguration" runat="server"  DataKeyField="authentication_configuration_id"
                HeaderStyle-Font-Bold="true"
                Width="100%"
                OnItemCommand="dlauthconfiguration_ItemCommand"
                CellPadding="0" ForeColor="#333333"
             >

                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="White" />

                <HeaderTemplate>
                    <tr style="font-weight: bold;">
                        <td colspan="0">
                            <asp:Label ID="lblAuthConfigName" runat="server" Text="Authentication Configuration" meta:resourcekey="lblResourcePathResource" /></td>
                       
                        
                       
                    </tr>
                </HeaderTemplate>

                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />

                <ItemTemplate>
                    <tr class="ItemSelect">
                        <td colspan="0">
                            <asp:Label ID="lblauthConfigId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "authentication_configuration_id") %>' style="display:none" />

                            <asp:LinkButton ID="lnkAuthConfigName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "authentication_configuration") %>' CssClass="ItemSelect" CommandName="Select" /></td>
                     
                      
                     

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