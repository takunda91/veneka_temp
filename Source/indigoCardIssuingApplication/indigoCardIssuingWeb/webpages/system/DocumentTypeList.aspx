<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="DocumentTypeList.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.system.DocumentTypeList" %>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">

    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblCardRenewalList" runat="server" Text="Card Renewal Batches" meta:resourcekey="lblDistributionBatchListResource" />
            </div>

            <asp:DataList ID="dlDocumentTypeList" runat="server"
                OnItemCommand="dlDocumentTypeList_ItemCommand" OnItemDataBound="dlDocumentTypeList_ItemDataBound"
                Width="100%"
                HeaderStyle-Font-Bold="true"
                HeaderStyle-ForeColor="Azure"
                OnSelectedIndexChanged="dlDocumentTypeList_SelectedIndexChanged"
                CellPadding="0"
                ForeColor="#333333">


                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle Font-Bold="True" ForeColor="White" BackColor="White"></HeaderStyle>
                <HeaderTemplate>
                    <tr style="font-weight: bold;">
                        <td>
                            <asp:Label ID="lblName" runat="server" Text="Name" meta:resourcekey="lblName" />
                        </td>
                        <td>
                            <asp:Label ID="lblDescription" runat="server" Text="Description" meta:resourcekey="lblDescription" />
                        </td>
                        <td>
                            <asp:Label ID="lblActive" runat="server" Text="Active" meta:resourcekey="lblActive" />
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <ItemTemplate>
                    <tr class="ItemSelect">
                        <td>
                            <asp:Label ID="lblDocumentTypeId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Id") %>' Visible="false" />
                            <asp:LinkButton ID="lblName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>' CssClass="ItemSelect" />
                        </td>
                        <td>
                            <asp:LinkButton ID="lblDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Description") %>' CssClass="ItemSelect" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chksel" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "IsActive") %>' />
                        </td>
                    </tr>
                </ItemTemplate>

                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <SelectedItemTemplate>
                </SelectedItemTemplate>
            </asp:DataList>

            <div class="PaginationPanel">
                <asp:LinkButton ID="lnkFirst" runat="server" Text="<%$ Resources: CommonLabels, First %>" OnClick="lnkFirst_Click"> </asp:LinkButton>
                <asp:LinkButton ID="lnkPrev" runat="server" Text="<%$ Resources: CommonLabels, Prev %>" OnClick="lnkPrev_Click"> </asp:LinkButton>
                <asp:Label ID="lblPageIndex" runat="server" />
                <asp:LinkButton ID="lnkNext" runat="server" Text="<%$ Resources: CommonLabels, Next %>" OnClick="lnkNext_Click"> </asp:LinkButton>
                <asp:LinkButton ID="lnkLast" runat="server" Text="<%$ Resources: CommonLabels, Last %>" OnClick="lnkLast_Click"> </asp:LinkButton>
            </div>
        </div>
        <div id="pnlremarks" runat="server" visible="true">
            <asp:Label ID="lblStatusNote" runat="server" Text="Notes" CssClass="label leftclr" meta:resourcekey="lblStatusNoteResource" />
            <asp:TextBox ID="tbStatusNote" runat="server" MaxLength="150" CssClass="input rightclr" />
        </div>
        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>
    </div>

</asp:Content>
