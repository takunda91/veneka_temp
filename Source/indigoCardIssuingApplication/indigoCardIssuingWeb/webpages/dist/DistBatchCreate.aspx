<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="DistBatchCreate.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.dist.DistributionBatchDetails" Culture="auto:en-US" meta:resourcekey="PageResource" UICulture="auto" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        var optSelected;
        function radioButtons(pobjButton) {
            if (pobjButton.id == optSelected) { return; }
            optSelected = pobjButton.id;
            //alert('You selected ' + pobjButton.value);
            // your code goes here
            $('#pnlOption1').prop('disabled', true);
            $('#pnlOption2').prop('disabled', true);
            //$('#pnlOption3').prop('disabled', true);

            $('#pnlOption1').children().prop('disabled', true);
            $('#pnlOption2').children().prop('disabled', true);
            //$('#pnlOption3').children().prop('disabled', true);

            if (pobjButton.value == 'rdbOption1') {
                $('#pnlOption1').prop('disabled', false);
                $('#pnlOption1').children().prop('disabled', false);
                $("#pnlOption2 :input").val('');
                //$("#pnlOption3 :input").val('');
            }
            else if (pobjButton.value == 'rdbOption2') {
                $('#pnlOption2').prop('disabled', false);
                $('#pnlOption2').children().prop('disabled', false);
                $("#pnlOption1 :input").val('');
                //$("#pnlOption3 :input").val('');
            }
            //else if (pobjButton.value == 'rdbOption3') {
            //    $('#pnlOption3').prop('disabled', false);
            //}
        }



        var optSelected1;
        function radioButtonsCardStock(pobjButton) {
            if (pobjButton.id == optSelected1) { return; }
            optSelected1 = pobjButton.id;
            //alert('You selected ' + pobjButton.value);
            // your code goes here
            $('#pnlOption1').prop('disabled', true);
            $('#pnlOption2').prop('disabled', true);
            $('#pnldistBatch').prop('disabled', true);

            $('#pnlOption1').children().prop('disabled', true);
            $('#pnlOption2').children().prop('disabled', true);
            $('#pnldistBatch').children().prop('disabled', true);

            //$('#pnlOption3').children().prop('disabled', true);

            if (pobjButton.value == 'rdbCardStock') {
                $('#pnlOption1').prop('disabled', false);
                $('#pnlOption1').children().prop('disabled', false);
                //$("#pnlOption2 :input").val('');
                $('#pnlOption2').prop('disabled', false);
                $('#pnlOption2').children().prop('disabled', false);
                //$("#pnlOption1 :input").val('');
            }
            else if (pobjButton.value == 'rdbdistBatch') {
                $('#pnldistBatch').prop('disabled', false);
                $('#pnldistBatch').children().prop('disabled', false);
                $("#pnldistBatch :input").val('');
                //$("#pnlOption3 :input").val('');
            }
           
        }
    </script>


    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblCreateDistributionBranch" runat="server" Text="Create Distribution Branch" meta:resourcekey="lblCreateDistributionBranchResource" />
            </div>

            <asp:Label ID="lblIssuer" runat="server" CssClass="label leftclr" Text="<%$ Resources: CommonLabels, Issuer %>" />
            <asp:DropDownList ID="ddlIssuer" runat="server" CssClass="input rightclr" OnSelectedIndexChanged="ddlIssuer_OnSelectedIndexChanged" />

            <asp:Label ID="lblFromBranch" runat="server" Text="<%$ Resources: CommonLabels, FromBranch %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlBranch" runat="server" CssClass="input rightclr" OnSelectedIndexChanged="ddlBranch_OnSelectedIndexChanged" AutoPostBack="true" />

            <asp:Label ID="lblProduct" runat="server" CssClass="label leftclr" Text="Product" meta:resourcekey="lblProductResource1" />
            <asp:DropDownList ID="ddlProduct" runat="server" CssClass="input rightclr" meta:resourcekey="ddlProductResource1" OnSelectedIndexChanged="ddlProduct_OnSelectedIndexChanged" AutoPostBack="true" />

            <asp:RadioButton ID="rdbCardStock" runat="server" Text="Card Stock" GroupName="StockInfo"  Checked="true" OnCheckedChanged="rdbCardStock_CheckedChanged" AutoPostBack="true"
                CssClass="label leftclr" />
            <asp:RadioButton ID="rdbdistBatch" runat="server" Text="Batch" GroupName="StockInfo" Checked="false" OnCheckedChanged="rdbdistBatch_CheckedChanged" AutoPostBack="true"
                CssClass="label rightclr" />
            <div id="pnldistBatch">
                <asp:Label ID="lbldistBatch" runat="server" CssClass="label leftclr" Text="BatchRef" meta:resourcekey="lblProductResource1" />
                <asp:DropDownList ID="ddldistBatch" runat="server" CssClass="input rightclr" meta:resourcekey="ddldistBatchResource" OnSelectedIndexChanged="ddldistBatch_SelectedIndexChanged" AutoPostBack="true" Enabled="false" />
            </div>

            <asp:Label ID="lblNumberOfCardsAvailable" runat="server" Text="Available cards" CssClass="label leftclr" meta:resourcekey="lblNumberOfCardsAvailableResource" />
            <asp:TextBox ID="tbNumberOfCardsAvailabe" runat="server" Enabled="False" CssClass="input rightclr" />

            <asp:Label ID="lblToBranch" runat="server" Text="<%$ Resources: CommonLabels, ToBranch %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlToBranch" runat="server" CssClass="input rightclr" />


            <asp:RadioButton ID="rdbOption1" runat="server" Text="Option 1" GroupName="batchInfo" Checked="true"
                CssClass="label bothclr" />

            <div id="pnlOption1">
                <asp:Label ID="lblNumberOfCardsForBatch" runat="server" Text="Number of cards" CssClass="label leftclr" meta:resourcekey="lblNumberOfCardsForBatchResource" />
                <asp:TextBox ID="tbNumberOfCards" runat="server" CssClass="input" MaxLength="5" OnKeyPress="return isNumberKey(event)" />
                <%--<asp:RequiredFieldValidator ID="rfvNumberOfCards" runat="server" ControlToValidate="tbNumberOfCards" Endabled="false"
                                            ErrorMessage="Please enter the number of cards required for this batch." 
                                            CssClass="validation rightclr" meta:resourcekey="rfvNumberOfCardsResource" />--%>
            </div>

            <asp:RadioButton ID="rdbOption2" runat="server" Text="Option 2" GroupName="batchInfo" CssClass="label bothclr" />
            <div id="pnlOption2">
                <asp:Label ID="lblRefFRom" runat="server" Text="Range From" CssClass="label leftclr" />
                <asp:TextBox ID="tbRefFrom" runat="server" CssClass="input" MaxLength="100" />
                <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tbRefFrom" Enabled="false"
                                            ErrorMessage="Please provide a value."  CssClass="validation rightclr" />--%>

                <asp:Label ID="lblRefTo" runat="server" Text="Range To" CssClass="label leftclr" />
                <asp:TextBox ID="tbRefTo" runat="server" CssClass="input" MaxLength="100" />
                <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="tbRefTo" Enabled="false"
                                            ErrorMessage="Please provide a value." CssClass="validation rightclr"  />--%>
            </div>

            <%--<asp:RadioButton ID="rdbOption3" runat="server" Text="Option 3" GroupName="batchInfo" CssClass="label bothclr" />

            <div id="pnlOption3">

            </div>--%>
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnCreateBatch" runat="server" Text="Create Batch" OnClick="btnCreateBatch_Click" CssClass="button" meta:resourcekey="btnCreateBatchResource" />

            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" OnClick="btnConfirm_OnClick"
                Enabled="false" Visible="false" CssClass="button" />

            <asp:Button ID="btnBack" runat="server" Text="<%$ Resources: CommonLabels, Back %>" OnClick="btnBack_OnClick"
                Enabled="false" Visible="false" CssClass="button" />
        </div>
    </div>
</asp:Content>
