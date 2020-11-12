<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="E-PINRequest.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.pin.E_PINRequest" %>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
       
             function ValidateMobNumber() {

            var rb = document.getElementById("<%=dlCardList.ClientID%>");
        var radio = rb.getElementsByTagName("input");
        var isChecked = false;
        for (var i = 0; i < radio.length; i++) {
            if (radio[i].checked) {
                isChecked = true;
                break;
            }
        }
        if (!isChecked) {
            alert("Please select an option!");
            return isChecked
        }
            var fld = document.getElementById("<%=tbContactnumber.ClientID%>");
            var result = confirm("are you sure the mobile number is correct?");
            if (result) {
                //Logic to delete the item
                return true;
            }
            var tbaccountnumber = document.getElementById("<%=tbAccountNumber.ClientID%>");
            var ddlIssuer = document.getElementById("<%=ddlIssuer.ClientID%>");

            var hdnContactnumber = document.getElementById('<%= tbContactnumber.ClientID %>');
            var Contactnumber = hdnContactnumber.value;
            $.ajax({
                type: "POST",
                url: "E-PINRequest.aspx/InsertAuditRequest",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{accountnumber: "' + tbaccountnumber.value + '",issuerId: "' + ddlIssuer.value + '",Contactnumber: "' + Contactnumber + '" }',
            }).done(function (data) {
                var btnPinRequest = document.getElementById("<%=btnPinRequest.ClientID%>");
                btnPinRequest.disabled = true;
                var lblInfoMessage = document.getElementById("<%=lblInfoMessage.ClientID%>");
                lblInfoMessage.innerText = "";
                var lblErrorMessage = document.getElementById("<%=lblErrorMessage.ClientID%>");
                lblErrorMessage.innerText = "e-pin cann't be requested if mobile number is worng";
            }).fail(function (data) {
                alert(data.d);
            });
            return false;
        }

</script>
      
 

    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <asp:HiddenField ID="hdnContactnumber" runat="server" />      
           
            <div class="ContentHeader">
                <asp:Label ID="lblPinReissue" Text="e-Pin Request" runat="server" />
            </div>            

            <asp:Label ID="lblIssuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlIssuer" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlIssuer_OnSelectedIndexChanged" CssClass="input rightclr" />

            <asp:Label ID="lblBranch" runat="server" Text="<%$ Resources: CommonLabels, Branch %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlBranch" runat="server" CssClass="input rightclr"  />
       
               <asp:Label ID="lblCardProduct" runat="server" Text="<%$ Resources: CommonLabels, Product %>" CssClass="label leftclr" />
            <asp:DropDownList ID="ddlProduct" runat="server" AutoPostBack="true"  CssClass="input rightclr" />

               <div id="pnlAccountNumber" runat="server" class="bothclr"  style="margin-top: 10px; margin-bottom: 10px;">
                <asp:Label ID="lblAccountNumber" runat="server" Text="Account number" CssClass="label leftclr" meta:resourcekey="lblAccountNumberResource" />
                <asp:TextBox ID="tbAccountNumber" runat="server" CssClass="input" MaxLength="27" OnKeyPress="return isNumberKey(event)" />
                <asp:Button ID="btnValidateAccount" runat="server" Text="Validate" meta:resourcekey="btnValidateAccountResource"
                    CssClass="button" Style="margin: 0px 0px 0px 5px !important; float: left;"
                    OnClick="btnValidateAccount_Click" />

                <asp:RequiredFieldValidator runat="server" ID="reqAccNo" ControlToValidate="tbAccountNumber" meta:resourcekey="reqAccNoResource"
                    ErrorMessage="Please enter account number." CssClass="validation rightclr" />

                
            </div> 

            <div id="divContactnumber" runat="server" visible="false">
                    <asp:Label ID="lblcontactnumber" runat="server" Text="ContactNumber"  CssClass="label leftclr" meta:resourcekey="lblcontactnumberResource" />
                    <asp:TextBox ID="tbContactnumber" runat="server" Enabled="false" CssClass="input rightclr"  MaxLength="50" OnKeyPress="return isNumberKey(event)" />
              <%--<asp:Button ID="btnDisplayCards" runat="server"  meta:resourcekey="btnDisplayCardsResource"  Text="Display Cards"
                    CssClass="button" Style="margin: 0px 0px 0px 5px !important; float: left;"
                    OnClick="btnDisplayCards_Click" />--%>
                     </div>

             </div>
         <asp:DataList ID="dlCardList" runat="server"
                Width="80%"
                HeaderStyle-Font-Bold="true" CssClass="leftclr"
                HeaderStyle-ForeColor="Azure" Style="margin-top: 0px" CellPadding="0"
                ForeColor="#333333" Font-Names="Verdana" Font-Size="Small" OnItemDataBound="dlCardList_ItemDataBound">

                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle Font-Bold="True" ForeColor="White"></HeaderStyle>

                <HeaderTemplate>
                    <tr style="font-weight: bold">
                        <td>
                            <asp:Label ID="lblSelect" runat="server" Text="Select" meta:resourcekey="lblCardNumberResource" />
                        </td>
                        <td>
                            <asp:Label ID="lblcardnumber" runat="server" Text="Card Number" meta:resourcekey="lblcardStatusResource" />
                        </td>
                       
                    </tr>

                </HeaderTemplate>
                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <ItemTemplate>

                    <tr class="ItemSelect">
                        <td>
                            <asp:Radiobutton ID="radCardNumber" runat="server" GroupName="cardnumber" CssClass="ItemSelect" OnCheckedChanged="radCardNumber_CheckedChanged" />
                        </td>
                        <td>
                             <asp:Label ID="lblcardnumber" runat="server"   meta:resourcekey="lblcardStatusResource" Text='<%# DataBinder.Eval(Container.DataItem, "PAN") %>'/>
                            <asp:Label ID="hdncardnumber" runat="server" Visible="false" meta:resourcekey="lblcardStatusResource" Text='<%# DataBinder.Eval(Container.DataItem, "PAN") %>'/>
                        </td>
                        
                    </tr>

                </ItemTemplate>

                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />

                <SelectedItemTemplate>
                </SelectedItemTemplate>
            </asp:DataList>
        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource" />
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource" />
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnPinRequest" runat="server" Text="e-PIN Request" CssClass="button" OnClick="btnPinRequest_OnClick" meta:resourcekey="btnPinRequestResource" Visible="false"  OnClientClick="return ValidateMobNumber();" />
           
            <asp:Button ID="btnConfirm" runat="server" Text="<%$ Resources: CommonLabels, Confirm %>" CssClass="button" OnClick="btnConfirm_OnClick" Visible="false" />
            <asp:Button ID="btnCancel" runat="server" Text="<%$ Resources: CommonLabels, Cancel %>" OnClick="btnCancel_Click" CssClass="button" CausesValidation="false" Visible="false" />
        
        </div>
    </div>
</asp:Content>
