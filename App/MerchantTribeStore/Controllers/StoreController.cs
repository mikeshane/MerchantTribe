﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MerchantTribeStore.Controllers
{
    public class StoreController : Shared.BaseStoreController
    {
        //
        // GET: /StoreNotFound
        public ActionResult NotFound()
        {
            return View();
        }

        //
        // GET: /StoreNotAvailable
        public ActionResult NotAvailable()
        {
            return View();
        }

        public ActionResult Closed()
        {
            string message = MTApp.CurrentRequestContext.CurrentStore.Settings.StoreClosedDescription;
            if (string.IsNullOrEmpty(message))
            {
                message = "Our store is currently closed while we perform updates. We appreciate your patience.";
            }

            ViewBag.ClosedMessage = message;
            ViewBag.Title = MTApp.CurrentStore.StoreName;

            return View();
        }
    }
}