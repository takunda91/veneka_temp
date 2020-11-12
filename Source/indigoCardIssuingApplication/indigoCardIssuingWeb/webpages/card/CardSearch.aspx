<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="CardSearch.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.card.CardSearchForm" Culture="auto:en-US" meta:resourcekey="PageResource" UICulture="auto" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            $("#<%= tbDateFrom.ClientID %>").datepicker({
                changeMonth: true,
                changeYear: true,
                maxDate: 1,
                dateFormat: '<%=DATEPICKER_FORMAT%>',
                onClose: function (selectedDate) {
                    $("#<%= tbDateTo.ClientID %>").datepicker("option", "minDate", selectedDate);
                }
            });
            $("#<%= tbDateTo.ClientID %>").datepicker({
                changeMonth: true,
                changeYear: true,
                maxDate: 1,
                dateFormat: '<%=DATEPICKER_FORMAT%>',
                onClose: function (selectedDate) {
                    $("#<%= tbDateFrom.ClientID %>").datepicker("option", "maxDate", selectedDate);
                }
            });
        });
    </script>

    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblCardSearch" runat="server" Text="Card Search" meta:resourcekey="lblCardSearchResource" />
            </div>


            <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlIssuer" runat="server" CssClass="input rightclr" AutoPostBack="true" OnSelectedIndexChanged="ddlIssuer_OnSelectedIndexChanged" />

            <asp:Label ID="lblBranch" runat="server" Text="<%$ Resources: CommonLabels, Branch %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlBranch" runat="server" CssClass="input rightclr" />

            <asp:Label ID="lblCardIssueMethod" runat="server" Text="Card Issue Method" CssClass="label leftclr" meta:resourcekey="lblCardIssueMethodResource"  />
            <asp:DropDownList ID="ddlCardIssueMethod" runat="server" CssClass="input rightclr"  OnSelectedIndexChanged="ddlCardIssueMethod_SelectedIndexChanged" AutoPostBack="true"/>

            <asp:Label ID="lblCardProduct" runat="server" Text="<%$ Resources: CommonLabels, Product %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlProduct" runat="server" AutoPostBack="true" CssClass="input rightclr" OnSelectedIndexChanged="ddlProduct_SelectedIndexChanged" />

            <asp:Label ID="lblCardNumber" runat="server" Text="Card Number" CssClass="label leftclr" meta:resourcekey="lblCardNumberResource" />
            <asp:TextBox ID="tbCardNumber" runat="server" MaxLength="19" CssClass="input rightclr" />

            <asp:Label ID="lblcardrefno" runat="server" Text="Card Reference No" CssClass="label leftclr" meta:resourcekey="lblcardrefnoResource" />
            <asp:TextBox ID="tbcardrefno" runat="server" CssClass="input" MaxLength="25" />    

            <asp:Label ID="lblBatchReference" Text="Batch Reference" CssClass="label leftclr" runat="server" meta:resourcekey="lblbatchReferenceResource" />
            <asp:TextBox ID="tbBatchReference" runat="server" MaxLength="100" CssClass="input rightclr"
                ToolTip="Will search on load batch reference and distribution batch reference" meta:resourcekey="tbBatchReference" />

            <asp:Label ID="LabelCardStatus" runat="server" Text="Status" CssClass="label leftclr" meta:resourcekey="LabelCardStatusResource" />
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="input rightclr" />                  

            <asp:Label ID="lblDateFrom" runat="server" Text="Created between" CssClass="label leftclr" meta:resourcekey="lblDateFromResource" />
            <asp:TextBox ID="tbDateFrom" runat="server" CssClass="input rightclr" MaxLength="10" />

            <asp:Label ID="lblDateTo" runat="server" Text="and" CssClass="label leftclr" meta:resourcekey="lblDateToResource" />
            <asp:TextBox ID="tbDateTo" runat="server" CssClass="input rightclr" MaxLength="10" />
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnSearch" runat="server" Text="<%$ Resources: CommonLabels, Search %>" CssClass="button"
                OnClick="btnSearch_Click" />

            <asp:Button ID="btnCancel" runat="server" Text="<%$ Resources: CommonLabels, Cancel %>" CssClass="button" OnClick="btnCancel_OnClick" />
        </div>
    </div>
</asp:Content>
