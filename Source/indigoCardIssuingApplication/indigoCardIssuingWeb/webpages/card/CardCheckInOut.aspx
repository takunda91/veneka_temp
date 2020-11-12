<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="CardCheckInOut.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.card.CardCheckInOut" Culture="auto" meta:resourcekey="PageResource" UICulture="auto" ClientIDMode="Static" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">        
    <asp:HiddenField ID="sourceSel" runat="server" />
    <asp:HiddenField ID="targetSel" runat="server" />
    <asp:HiddenField ID="hdnConfirmMsg" runat="server" />
    <asp:HiddenField ID="hdnsourcecaption" runat="server" />
    <asp:HiddenField ID="hdntargetcaption" runat="server" />
    <asp:HiddenField ID="hdnHasChanges" runat="server" />
    <asp:HiddenField ID="hdnlog" runat="server" />

    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblCheckInOutHeader" runat="server" Text="Card Check In/Out"
                    meta:resourcekey="lblCheckInOutHeaderResource" />
            </div>

            <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlIssuer" runat="server" AutoPostBack="True"
                OnSelectedIndexChanged="ddlIssuer_OnSelectedIndexChanged"
                CssClass="input rightclr" />

            <asp:Label ID="lblBranch" runat="server" Text="Branch" CssClass="label leftclr"
                meta:resourcekey="lblBranchResource" />
            <asp:DropDownList ID="ddlBranch" runat="server" AutoPostBack="True"
                OnSelectedIndexChanged="ddlBranch_OnSelectedIndexChanged"
                CssClass="input rightclr" />

            <asp:Label ID="lblCardProduct" runat="server" Text="<%$ Resources: CommonLabels, Product %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlProduct" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlProduct_OnSelectedIndexChanged" CssClass="input rightclr" />

            <asp:Label ID="lblOperator" runat="server" Text="Operator" CssClass="label leftclr"
                meta:resourcekey="lblOperatorResource" />
            <asp:DropDownList ID="ddlOperator" runat="server" AutoPostBack="True"
                OnSelectedIndexChanged="ddlOperator_OnSelectedIndexChanged"
                CssClass="input rightclr" />

            <asp:Label ID="lblCardFilter" runat="server" Text="Card" CssClass="label leftclr"
                meta:resourcekey="lblCardFilterResource" Visible="false" />
            <asp:TextBox ID="txbCardFilter" runat="server" CssClass="input" MaxLength="19" Visible="false" />
            <asp:Button ID="btnLoadCards" name="btnLoadCards" runat="server" Text="Load" ClientIDMode="Static"
                OnClick="btnLoadCards_OnClick" CssClass="button rightclr" Style="margin: 0px 0px 0px 5px !important; float: left;"
                meta:resourcekey="btnLoadCardsResource" />

            <div id="cardCheckInOut" class="Panel bothclr" style="padding: 10px">
                <select id="source" name="source">
                </select>
                <select id="target" name="target">
                </select>
            </div>

            <asp:Panel ID="pnlPage" runat="server" ClientIDMode="Static" style="max-width: 400px">           
            
                <asp:DropDownList CssClass="input  leftclr" ID="ddlPerPage"  AutoPostBack="True" runat="server" Width="50" style="margin-left:35%"
                                     onChange="saveCheckedOutCards('ddlPerPage')" OnSelectedIndexChanged="ddlPerPage_SelectedIndexChanged">
                    <asp:ListItem Selected="True" Text="100" Value="100"></asp:ListItem>
                    <asp:ListItem Text="200" Value="200"></asp:ListItem>
                    <asp:ListItem Text="300" Value="300"></asp:ListItem>
                    <asp:ListItem Text="400" Value="400"></asp:ListItem>
                    <asp:ListItem Text="500" Value="500"></asp:ListItem>
                </asp:DropDownList>
                
                <asp:Label ID="Label1" runat="server" CssClass="label rightclr" style="text-align:left" Text="Per Page" Width="100" />
                
                <div class="bothclr" style="padding-left:115px" >
                    <asp:LinkButton ID="lnkFirst" runat="server" Text="<%$ Resources: CommonLabels, First %>" OnClick="lnkFirst_Click" OnClientClick="return saveCheckedOutCards('First');"> </asp:LinkButton>
                    <asp:LinkButton ID="lnkPrev" runat="server" Text="<%$ Resources: CommonLabels, Prev %>" OnClick="lnkPrev_Click" OnClientClick="return saveCheckedOutCards('Prev');"></asp:LinkButton>
                    <asp:Label ID="lblPageIndex" Text="0/0" runat="server" />
                    <asp:LinkButton ID="lnkNext" runat="server" Text="<%$ Resources: CommonLabels, Next %>" OnClick="lnkNext_Click" OnClientClick="return saveCheckedOutCards('Next');"></asp:LinkButton>
                    <asp:LinkButton ID="lnkLast" runat="server" Text="<%$ Resources: CommonLabels, Last %>" OnClick="lnkLast_Click" OnClientClick="return saveCheckedOutCards('Last');"></asp:LinkButton>
                </div>

            </asp:Panel>
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server"
                meta:resourcekey="lblInfoMessageResource" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server"
                meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="dlgCheckOutStatus" title="Summary" runat="server" visible="false">
            <asp:Label ID="lblCheckOutSummary" runat="server"
                meta:resourcekey="lblCheckOutSummaryResource" />
            <div id="tblSummary" runat="server"></div>
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnUpdate" name="btnUpdate" ClientIDMode="Static" Text="<%$ Resources: CommonLabels, Update %>" runat="server"
                CssClass="button" Enabled="False" OnClientClick="return ConfirmScreen();"  UseSubmitBehavior="false"/>
            <asp:Button ID="btnConfirm" name="btnConfirm" ClientIDMode="Static" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" OnClientClick="return saveCheckedOutCards(this);" OnClick="btnConfirm_Click" CssClass="button" />
            <asp:Button ID="btnCancel" name="btnCancel" ClientIDMode="Static" runat="server" Text="<%$ Resources: CommonLabels, Cancel %>" OnClientClick="return EditScreen();" CssClass="button" UseSubmitBehavior="false" />
        </div>

        <div id="pnlReportButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnCheckedInReport" Text="Check-In Report" runat="server"
                CssClass="button" OnClick="btnCheckedInReport_OnClick" Visible="False"
                OnClientClick="timedRefresh(2000)" ClientIDMode="Static" 
                meta:resourcekey="btnCheckedInReportResource" />
            <asp:Button ID="btnCheckedOutReport" Text="Check-Out Report" runat="server"
                CssClass="button" OnClick="btnCheckedOutReport_OnClick" Visible="False"
                OnClientClick="timedRefresh(2000)" ClientIDMode="Static" 
                meta:resourcekey="btnCheckedOutReportResource" />
        </div>
    </div>

    <script type="text/javascript" src="<%=Page.ResolveClientUrl("~/Scripts/check-in-out.js")%>"></script>
</asp:Content>
