<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="ManageBranchNotification.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.system.ManageBranchNotification" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">


    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblManageBranchNotification" runat="server" Text="Manage Branch Notification" meta:resourcekey="lblManageBranchNotificationResource" />
            </div>

            <asp:Label ID="lblIssuer" Text="<%$ Resources: CommonLabels, Issuer %>" runat="server" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlIssuer" runat="server" CssClass="input rightclr" AutoPostBack="true" OnSelectedIndexChanged="ddlIssuer_OnSelectedIndexChanged" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlIssuer" ID="rfvIssuer" ErrorMessage="Issuer Required"
                meta:resourcekey="rfvIssuerResource" ForeColor="Red" CssClass="validation rightclr"></asp:RequiredFieldValidator>
            <asp:Label ID="lblCardIssueMethod" runat="server" Text="Card Issue Method" CssClass="label leftclr" meta:resourcekey="lblCardIssueMethodResource" />
            <asp:DropDownList ID="ddlCardIssueMethod" runat="server" CssClass="input rightclr" />
           <%-- <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlCardIssueMethod" ID="rfvcardissuemethodid" ErrorMessage="BatchStatus Required"
                meta:resourcekey="rfvcardissuemethodidResource" ForeColor="Red" CssClass="validation rightclr"></asp:RequiredFieldValidator>--%>
            <asp:Label ID="lblBatchStatus" runat="server" Text="Batch Status" CssClass="label leftclr" meta:resourcekey="lblBatchStatusResource" />
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="input rightclr" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlStatus" ID="rfvStatus" ErrorMessage="BatchStatus Required"
                meta:resourcekey="rfvStatusResource" ForeColor="Red" CssClass="validation rightclr"></asp:RequiredFieldValidator>

            <asp:Label ID="lblChannel" Text="Channel" runat="server" CssClass="label leftclr" meta:resourcekey="lblChannelStatusResource" />
            <asp:DropDownList ID="ddlchannel" runat="server" CssClass="input rightclr">
            </asp:DropDownList>
        </div>
        <asp:GridView ID="GrdNotificationFields" runat="server" CellPadding="1" CssClass="bothclr" GroupingText="External System Fields" EmptyDataText="No Rows Returned" GridLines="None"
            AutoGenerateColumns="false" Width="100%" OnRowDataBound="GrdNotificationFields_RowDataBound">

            <Columns>

                <asp:TemplateField HeaderText="Language">
                    <ItemTemplate>
                        <asp:Label ID="lbllanguage" Width="100px" runat="server" Text='<%#Bind("language_name") %>' />
                        <asp:Label ID="lbllanguageid" Width="100px" Visible="false" runat="server" Text='<%#Bind("language_id") %>' />
                    </ItemTemplate>

                </asp:TemplateField>


                <asp:TemplateField HeaderText="Notification Text">
                    <ItemTemplate>
                        <asp:TextBox ID="txtnotificationtext" runat="server" Width="250" Height="30px" TextMode="MultiLine" MaxLength="1000" Text='<%#Bind("notification_text") %>' />
                    </ItemTemplate>

                </asp:TemplateField>

                <asp:TemplateField HeaderText="Subject Text">
                    <ItemTemplate>
                        <asp:TextBox ID="txtSubjectText" runat="server" Width="250" Height="30px" TextMode="MultiLine" MaxLength="1000" Text='<%#Bind("subject_text") %>' />
                    </ItemTemplate>

                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lblfromaddress" Width="100px" runat="server" Text="From Address" meta:resourcekey="lblfromaddressResource" />

                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:TextBox ID="txtfromaddress" runat="server" Width="250" Height="30px" TextMode="MultiLine" MaxLength="1000" Text='<%#Bind("from_address") %>' />
                    </ItemTemplate>

                </asp:TemplateField>
            </Columns>
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />

        </asp:GridView>
        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnCreate" runat="server" Text="<%$ Resources:CommonLabels, Create %>" CssClass="button"
                OnClick="btnCreate_Click" />
            <asp:Button ID="btnEdit" runat="server" Text="<%$ Resources:CommonLabels, Edit %>" CssClass="button" OnClick="btnEdit_Click" />
            <asp:Button ID="btnUpdate" runat="server" Text="<%$ Resources:CommonLabels, Update %>" CssClass="button"
                OnClick="btnUpdate_Click" />
            <asp:Button ID="btnDelete" runat="server" Text="<%$ Resources: CommonLabels, Delete %>"
                CssClass="button" OnClick="btnDelete_Click" />
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources:CommonLabels, Confirm %>"
                CssClass="button" OnClick="btnConfirm_Click" />
            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources:CommonLabels, Back %>"
                CssClass="button" OnClick="btnBack_Click" Visible="False" CausesValidation="False" />
        </div>
    </div>
</asp:Content>
