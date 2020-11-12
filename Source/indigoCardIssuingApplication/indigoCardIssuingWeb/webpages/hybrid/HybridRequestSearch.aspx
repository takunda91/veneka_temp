<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true" CodeBehind="HybridRequestSearch.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.hybrid.HybridRequestSearch" %>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
  

    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblSearchHybridRequest" runat="server" Text="Hybrid Request Search" />
            </div>  
        
            <asp:Label ID="lblIssuer" Text="<%$ Resources: CommonLabels, Issuer %>"  runat="server" CssClass="label leftclr"  />
            <asp:DropDownList ID="ddlIssuer" runat="server" CssClass="input rightclr"   AutoPostBack="true"/>
           
            <asp:Label ID="lblBranchAllocated" Text="Branch Allocated" runat="server" CssClass="label leftclr" meta:resourcekey="lblBranchAllocatedResource"  Visible="false" />
            <asp:DropDownList ID="ddlBranch" runat="server" CssClass="input rightclr"  Visible="false"/> 

            <asp:Label ID="lblCardProduct" runat="server" Text="<%$ Resources: CommonLabels, Product %>" CssClass="label leftclr" />
             <asp:DropDownList ID="ddlProduct" runat="server" AutoPostBack="true" CssClass="input rightclr" />
         
             <asp:Label ID="lblIssueMethod" Text="Issue Method" runat="server" CssClass="label leftclr"  />
            <asp:DropDownList ID="ddlIssueMethod" runat="server" CssClass="input rightclr"  /> 

             <asp:Label ID="lblBatchStatus" runat="server" Text="Batch Status" CssClass="label leftclr" meta:resourcekey="lblbranchstatusResource" />
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="input rightclr"  />

            <asp:Label ID="lblrequestrefno" runat="server" Text="Request Ref No" CssClass="label leftclr"  />
            <asp:TextBox ID="tbrequestrefno" runat="server" CssClass="input" MaxLength="25" />
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server" meta:resourcekey="lblInfoMessageResource"/>
        </div>

        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server" meta:resourcekey="lblErrorMessageResource"/>
        </div>

        <div class="ButtonPanel" id="pnlButtons" runat="server">
            <asp:Button ID="btnSearch" runat="server" Text="<%$ Resources: CommonLabels, Search %>"  OnClick="btnSearch_Click"
                        CssClass="button" />

            <asp:Button ID="btnCancel" runat="server" Text="<%$ Resources: CommonLabels, Clear %>"  CssClass="button"  
                        OnClick="btnCancel_Click" />
        </div>
    </div>
</asp:Content>

