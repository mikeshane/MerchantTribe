using MerchantTribe.Commerce;
using MerchantTribe.Commerce.Catalog;
using MerchantTribe.Commerce.Content;
using MerchantTribe.Commerce.Orders;

namespace MerchantTribeStore
{

    partial class product_validate : BaseStoreJsonPage
    {

        private int qty = 1;

        public class ProductValidateResponse
        {
            public string Message = string.Empty;
            public string ImageUrl = string.Empty;
            public string Price = string.Empty;
            public string Sku = string.Empty;
            public string StockMessage = string.Empty;
            public bool IsValid = false;
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ProductValidateResponse result = new ProductValidateResponse();

            string bvin = Request.Form["productbvin"];
            Product p = MTApp.CatalogServices.Products.Find(bvin);
            if ((p != null))
            {

                OptionSelectionList selections = ParseSelections(p);

               

                UserSpecificPrice price = MTApp.PriceProduct(p, MTApp.CurrentCustomer, selections);

                result.Price = MerchantTribe.Commerce.Utilities.HtmlRendering.UserSpecificPriceForDisplay(price);
                result.Sku = price.Sku;
                result.ImageUrl = MerchantTribe.Commerce.Storage.DiskStorage.ProductImageUrlMedium(MTApp.CurrentStore.Id, p.Bvin, p.ImageFileSmall, false);
                result.IsValid = price.IsValid;
                if (result.IsValid)
                {
                    result.Message = string.Empty;
                }
                else
                {
                    result.Message = "<div class=\"flash-message-warning\">The combination of options you've selected isn't available at the moment. Please select different options.</div>";
                }
                if (price.VariantId.Length > 0)
                {
                    result.ImageUrl = MerchantTribe.Commerce.Storage.DiskStorage.ProductVariantImageUrlMedium(MTApp.CurrentStore.Id, p.Bvin, p.ImageFileSmall, price.VariantId, false);
                }

                // Make sure we have stock on the product or variant
                InventoryCheckData data = MTApp.CatalogServices.InventoryCheck(p, price.VariantId);
                result.StockMessage = data.InventoryMessage;
                if (result.IsValid)
                {
                    if (!data.IsAvailableForSale)
                    {
                        result.IsValid = false;
                        // Should this be here?
                        //result.Message = "Out of stock?";
                    }
                }

                // Make sure no "labels" are selected
                if (selections.HasLabelsSelected())
                {
                    result.IsValid = false;
                    result.Message = "<div class=\"flash-message-warning\">Please make all selections before adding to cart.</div>";
                }
            }

            this.litOutput.Text = MerchantTribe.Web.Json.ObjectToJson(result);
        }

        private OptionSelectionList ParseSelections(Product p)
        {
            OptionSelectionList result = new OptionSelectionList();

            foreach (Option opt in p.Options)
            {

                // No need to parse HTML or Input Fields as they do not affect the ajax
                // updates on products
                if (opt.OptionType == OptionTypes.Html ||
                    opt.OptionType == OptionTypes.TextInput ||
                    opt.OptionType == OptionTypes.FileUpload)
                {
                    continue;
                }

                string strippeddata = Request.Form["opt" + opt.Bvin.Replace("-", "")];
                // stuff value into a guid to restore dashes which are invalid for
                // asp.net control ids
                if (strippeddata == "" || strippeddata == "undefined" || strippeddata == null)
                {
                    continue;
                }
                else
                {
                    System.Guid g = System.Guid.NewGuid();

                    if (System.Guid.TryParse(strippeddata, out g))
                    {
                        result.Add(new OptionSelection(opt.Bvin, g.ToString()));
                    }
                    else
                    {
                        result.Add(new OptionSelection(opt.Bvin, strippeddata));
                    }                    
                }
            }

            return result;
        }

        private bool SetQuantity(int qty, Product p, LineItem li, string outputMessage)
        {
            bool result = true;

            if (p.MinimumQty > 0)
            {
                if (qty < p.MinimumQty)
                {
                    outputMessage = SiteTerms.ReplaceTermVariable(SiteTerms.GetTerm(SiteTermIds.ProductPageMinimumQuantityError), "quantity", p.MinimumQty.ToString());
                    result = false;
                }
            }

            // last minute safety check
            if (qty < 1) qty = 1;

            li.Quantity = qty;

            return result;
        }

    }
}