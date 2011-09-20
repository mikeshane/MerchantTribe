using System.Web.UI;
using MerchantTribe.Commerce;

namespace MerchantTribeStore
{

    partial class BVModules_Controls_EmailThisPage : MerchantTribe.Commerce.Content.BVUserControl
    {

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                this.imgEmail.ImageUrl = MyPage.MTApp.ThemeManager().ButtonUrl("emailthispage", Request.IsSecureConnection);
            }

            int w = 400;
            int h = 200;

            string id = Request.QueryString["ProductID"];
            this.EmailLink.Style.Add("CURSOR", "pointer");
            this.EmailLink.Attributes.Add("onclick", "JavaScript:window.open('" + Page.ResolveUrl("~/EmailFriend.aspx") + "?productID=" + id + "','Images','width=" + w + ", height=" + h + ", menubar=no, scrollbars=yes, resizable=yes, status=no, toolbar=no')");

        }
    }
}