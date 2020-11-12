<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="BatchNotificationList.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.system.BatchNotificationList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblnotificationbatch" runat="server" Text="Notification Batch List" meta:resourcekey="lblnotificationbatchHeadingResource" />
            </div>

            <asp:Label runat="server" ID="lblIssuer" Text="Issuer" meta:resourcekey="lblIssuerResource" CssClass="label leftclr"></asp:Label>
            <asp:DropDownList runat="server" ID="ddlIssuer" CssClass="input rightclr" AutoPostBack="true" OnSelectedIndexChanged="ddlIssuer_SelectedIndexChanged"></asp:DropDownList>

           
            <asp:DataList ID="dlnotificationlist" runat="server"
                HeaderStyle-Font-Bold="true"
                Width="100%"
                OnItemCommand="dlnotificationlist_ItemCommand"
                CellPadding="0" ForeColor="#333333"
             >

                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="White" />

                <HeaderTemplate>
                    <tr style="font-weight: bold;">
                       

                        <td colspan="0">
                            <asp:Label ID="lblissuer" runat="server" Text="Issuer" meta:resourcekey="lblResourcePathResource" /></td>
                        <td colspan="0">
                            <asp:Label ID="lbldistbatchtypename" runat="server" Text="batchtype" meta:resourcekey="lbldistbatchtypenameResource" /></td>
                         <td colspan="0">
                            <asp:Label ID="lbldistbatchstatus" runat="server" Text="batch Status" meta:resourcekey="lbldistbatchstatusResource" /></td>
                       
                    </tr>
                </HeaderTemplate>

                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />

                <ItemTemplate>
                    <tr class="ItemSelect">
                        <td colspan="0">
                            <asp:Label ID="lblissuerid" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "issuer_id") %>' Visible="False" />
                            <asp:Label ID="lbldistbatchtypeid" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "dist_batch_type_id") %>' Visible="False" />
                            <asp:Label ID="lbldistbatchstatusid" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "dist_batch_statuses_id") %>' Visible="False" />
                            
                            <asp:Label ID="lblchannelid" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "channel_id") %>' Visible="False" />

                            <asp:LinkButton ID="lnkissuername" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "issuer_name") %>' CssClass="ItemSelect" CommandName="Select" /></td>
                        <td colspan="0">
                            <asp:Label ID="lbldistbatchtype" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "dist_batch_type_name") %>' CommandName="Select"  />
                        </td>
                        <td colspan="0">
                            <asp:Label ID="lbldistbatchstatus" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "dist_batch_status_name") %>' CommandName="Select"  />
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
