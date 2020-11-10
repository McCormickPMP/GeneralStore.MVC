using GeneralStore.MVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace GeneralStore.MVC.Controllers
{
    public class TransactionController : Controller
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Transaction
        public ActionResult Index()
        {
            var transactions = _db.Transactions.Include(t => t.Customer).Include(t => t.Product).ToList();
            return View(transactions);
        }

        // GET: Transaction/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(_db.Customers, "CustomerID", "FullName");
            ViewBag.ProductID = new SelectList(_db.Products, "ProductID", "Name");
            return View();
        }

        // POST: Transaction/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Transactions transaction)
        {
            if (ModelState.IsValid)
            {
                var product = _db.Products.Find(transaction.ProductID);
                if (product.InventoryCount < 1)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                product.InventoryCount--;

                _db.Transactions.Add(transaction);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(_db.Customers, "CustomerID", "FullName", transaction.CustomerID);
            ViewBag.ProductID = new SelectList(_db.Products, "ProductID", "Name", transaction.ProductID);

            return View(transaction);
        }
    }
}