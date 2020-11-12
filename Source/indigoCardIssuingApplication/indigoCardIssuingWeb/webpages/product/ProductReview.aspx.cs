using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using System.Threading;
using System.Globalization;
using indigoCardIssuingWeb.utility;

namespace indigoCardIssuingWeb.webpages.product
{
    public partial class ProductReview : BasePage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                ProductResult productresult = (ProductResult)SessionWrapper.ProductlistResult;
                string buildstyle = "position: absolute;font-family:" + productresult.Product.font_name + ";left:" + productresult.Product.name_on_card_left.Value.ToString() + "px; top:" + productresult.Product.name_on_card_top.Value.ToString() + "px; font-size:" + productresult.Product.Name_on_card_font_size.Value.ToString() + "px;";
                lblnameoncard.Attributes.Add("style", buildstyle);
            }
        }     

        #region Page Events
        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            Server.Transfer("~\\webpages\\product\\ProductAdminScreen.aspx");
        }
        #endregion
    }
}