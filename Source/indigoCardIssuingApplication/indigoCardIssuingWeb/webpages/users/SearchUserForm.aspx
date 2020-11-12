<%@ Page Title="" Language="C#" MasterPageFile="~/IndigoCardIssuance.Master" AutoEventWireup="true"
    CodeBehind="SearchUserForm.aspx.cs" Inherits="indigoCardIssuingWeb.webpages.users.SearchUserForm"
    Culture="auto:en-US" meta:resourcekey="PageResource" UICulture="auto" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ContentPanel">
        <div id="pnlDisable" runat="server">
            <div class="ContentHeader">
                <asp:Label ID="lblUserSearchheading" runat="server" meta:resourcekey="lblUserSearchResource" />
            </div> 
                      
            <asp:Label ID="lblUsername" runat="server" Text="User Name" CssClass="label leftclr"
                meta:resourcekey="lblUsernameResource"></asp:Label>
            <asp:TextBox ID="tbUsername" runat="server"  MaxLength="50"
                CssClass="input rightclr"></asp:TextBox>
            
            <asp:Label ID="lblFirstName" runat="server" Text="First Name" CssClass="label leftclr"
                meta:resourcekey="lblFirstNameResource"></asp:Label>
            <asp:TextBox ID="tbFirstName" runat="server"  MaxLength="50"
                CssClass="input rightclr"></asp:TextBox>
          
            <asp:Label ID="lblLastName" runat="server" Text="Last Name" CssClass="label leftclr"
                meta:resourcekey="lblLastNameResource"></asp:Label>
            <asp:TextBox ID="tbLastName" runat="server"   MaxLength="50"
                CssClass="input rightclr"></asp:TextBox>
           
            <asp:Label ID="lblUserRole" runat="server" Text="User Group" CssClass="label leftclr"
                    meta:resourcekey="lblUserRoleResource"></asp:Label>
            <asp:DropDownList ID="ddluserrole" runat="server" 
                   CssClass="input rightclr">
            </asp:DropDownList>

            <asp:Label ID="lblissuer" runat="server" Text="<%$ Resources: CommonLabels, Issuer %>"   CssClass="label leftclr"
                    meta:resourcekey="lblissuerResource"></asp:Label>
            <asp:DropDownList ID="ddlissuerlist" runat="server" 
               AutoPostBack="true"
                CssClass="input rightclr" OnSelectedIndexChanged="ddlissuerlist_SelectedIndexChanged">
            </asp:DropDownList>

            <asp:Label ID="lblBranch" runat="server" Text="Branch"  CssClass="label leftclr"
                    meta:resourcekey="lblBranchResource"></asp:Label>
            <asp:DropDownList ID="ddlBranches" runat="server" CssClass="input rightclr"
               >
            </asp:DropDownList>
        </div>

        <div class="InfoPanel">
            <asp:Label ID="lblInfoMessage" runat="server"  meta:resourcekey="lblInfoMessageResource"/>
        </div>
        <div class="ErrorPanel">
            <asp:Label ID="lblErrorMessage" runat="server"  meta:resourcekey="lblErrorMessageResource"/>
        </div>

        <div id="pnlButtons" class="ButtonPanel" runat="server">
            <asp:Button ID="btnSearch" runat="server"  OnClick="btnSearch_Click" 
                CssClass="button"  Text="<%$ Resources: CommonLabels, Search %>"  />
            <asp:Button ID="btnClear" runat="server" CssClass="button" OnClick="btnClear_OnClick"  meta:resourcekey="btnClear" />
        </div>
    </div>
</asp:Content>
