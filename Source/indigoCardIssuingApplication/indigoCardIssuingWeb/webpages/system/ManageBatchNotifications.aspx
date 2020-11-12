<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="ManageBatchNotifications.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.system.ManageBatchNotifications" ValidateRequest="false" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">


    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblmangebatchnotifications" runat="server" Text="Manage Batch Notifications"   meta:resourcekey="lblmangebatchnotificationsResource" />
            </div>

            <asp:Label ID="lblIssuer" Text="<%$ Resources: CommonLabels, Issuer %>" runat="server" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlIssuer" runat="server" CssClass="input rightclr" AutoPostBack="true" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlIssuer" ID="rfvIssuer" ErrorMessage="Issuer Required"
                meta:resourcekey="rfvIssuerResource" ForeColor="Red" CssClass="validation rightclr"></asp:RequiredFieldValidator>
            <asp:Label ID="lblBatchType" Text="Batch Type" runat="server" CssClass="label leftclr"  meta:resourcekey="lblBatchTypeResource"/>
            <asp:DropDownList ID="ddlBatchType" runat="server" CssClass="input rightclr">
                <asp:ListItem Text="-ALL-" Value="-99" Selected="True" />
                <asp:ListItem Text="Production Batch" Value="0" />
                <asp:ListItem Text="Distribution Batch" Value="1" />
            </asp:DropDownList>
             <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlBatchType" ID="rfvBatchType" ErrorMessage="Batch Type Required"
                meta:resourcekey="rfvBatchTypeResource" ForeColor="Red" InitialValue="-99" CssClass="validation rightclr"></asp:RequiredFieldValidator>

            <asp:Label ID="lblBatchStatus" runat="server" Text="Batch Status" CssClass="label leftclr" meta:resourcekey="lblBatchStatusResource" />
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="input rightclr" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlStatus" ID="rfvStatus" ErrorMessage="BatchStatus Required"
                meta:resourcekey="rfvStatusResource" ForeColor="Red" CssClass="validation rightclr"></asp:RequiredFieldValidator>

            <asp:Label ID="lblChannel" Text="Channel" runat="server" CssClass="label leftclr" meta:resourcekey="lblChannelResource"/>
            <asp:DropDownList ID="ddlchannel" runat="server" CssClass="input rightclr">
            </asp:DropDownList>

        </div>
        <asp:GridView ID="GrdNotificationFields" runat="server" ForeColor="#333333" CellPadding="1" CssClass="bothclr" GroupingText="External System Fields" EmptyDataText="No Rows Returned" GridLines="None"
            AutoGenerateColumns="false" Width="80%" OnRowDataBound="GrdNotificationFields_RowDataBound">

            <Columns>

                <asp:TemplateField >
                    <HeaderTemplate>
                        <asp:Label ID="lblheadingLanguage" Width="100px" runat="server" Text="Language" meta:resourcekey="lblheadingLanguageResource"/>

                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lbllanguage" Width="100px" runat="server" Text='<%#Bind("language_name") %>'/>
                        <asp:Label ID="lbllanguageid" Width="100px" Visible="false" runat="server" Text='<%#Bind("language_id") %>'/>

                    </ItemTemplate>

                </asp:TemplateField>


                <asp:TemplateField >
                     <HeaderTemplate>
                        <asp:Label ID="lblheadingnotificationtext" Width="100px" runat="server" Text="Notification Text" meta:resourcekey="lblheadingnotificationtextResource"/>

                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:TextBox ID="txtnotificationtext" runat="server" Width="250" Height="30px" TextMode="MultiLine" MaxLength="1000" Text='<%#Bind("notification_text") %>' />
                    </ItemTemplate>

                </asp:TemplateField>

                <asp:TemplateField >
                     <HeaderTemplate>
                        <asp:Label ID="lblheadingsubjecttext" Width="100px" runat="server" Text="Subject Text" meta:resourcekey="lblheadingsubjecttextResource"/>

                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:TextBox ID="txtSubjectText" runat="server" Width="250" Height="30px" TextMode="MultiLine" MaxLength="1000" Text='<%#Bind("subject_text") %>' />
                    </ItemTemplate>

                </asp:TemplateField>
                <asp:TemplateField >
                     <HeaderTemplate>
                        <asp:Label ID="lblfromaddress" Width="100px" runat="server" Text="From Address" meta:resourcekey="lblfromaddressResource"/>

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
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnCreate" runat="server" Text="<%$ Resources:CommonLabels, Create %>" CssClass="button" 
                OnClick="btnCreate_Click" />
            <asp:Button ID="btnEdit" runat="server" Text="<%$ Resources:CommonLabels, Edit %>" CssClass="button" OnClick="btnEdit_Click"/>
            <asp:Button ID="btnUpdate" runat="server" Text="<%$ Resources:CommonLabels, Update %>" CssClass="button" 
                OnClick="btnUpdate_Click" />
            <asp:Button ID="btnDelete" runat="server" Text="<%$ Resources: CommonLabels, Delete %>"
                CssClass="button" OnClick="btnDelete_Click" />
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources:CommonLabels, Confirm %>"
                CssClass="button" OnClick="btnConfirm_Click"/>
            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources:CommonLabels, Back %>"
                CssClass="button" OnClick="btnBack_Click" Visible="False" CausesValidation="False"  />
        </div>
    </div>
</asp:Content>
