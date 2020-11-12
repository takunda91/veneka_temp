<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" 
    CodeBehind="ProductReview.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.product.ProductReview"   meta:resourcekey="PageResource" UICulture="auto" Culture="auto:en-US"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<style type="text/css">
    .carddiv
    {
        width: 400px;
        height: 220px;
        display: block;
    }
    .rectangle-box
    {
        border: 1px solid black;
        margin: 10px;
        width: 324px; 
        position: relative;
        height:204px;
    }
    .cardheader
    {
        width: 324px;
        background-color: #0082B6;
    }
    .cardheading
    {
        color: White;
        vertical-align: top;
        font-size: 20px; 
        font-weight: bold
    }
    .nameoncardlbl
    {
        display: block;
    }
</style>
    <div class="ContentPanel">
        <div class="ContentHeader">
            <asp:Label ID="lblProductReview" runat="server" Text="Card Preview" meta:resourcekey="lblProductReviewResource" />
        </div>
          <div class='carddiv' id="cardpopup">
            <div style="text-align: center">
              </div>
            <div class='rectangle-box' >
                <div class='rectangle-content'>
                    <div class='cardheader'>
                       <asp:Label ID="lblheading" runat="server" meta:resourcekey="lblheadingResource"   CssClass='cardheading' Text="Example Card" />
                    </div>
                     <asp:Label ID="lblnameoncard" runat="server" meta:resourcekey="lblnameoncardResource"   Text="EXAMPLE NAME ON CARD"  />
 
                </div>
            </div>
        </div>

         <asp:Button ID="btnBack" runat="server" Text="<%$ Resources: CommonLabels, Back %>" CssClass="button" OnClick="btnBack_OnClick"
                      />
 
    </div>
   
</asp:Content>
