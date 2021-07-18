using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ComeMyFishMarket.Data;
using ComeMyFishMarket.Models;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.WindowsAzure.Storage;

namespace ComeMyFishMarket.Controllers
{
    public class MarketOrdersController : Controller
    {
        private readonly ComeMyFishMarketClassContext _context;

        public MarketOrdersController(ComeMyFishMarketClassContext context)
        {
            _context = context;
        }

        // GET: MarketOrders
        public async Task<IActionResult> Index()
        {
            return View(await _context.MarketOrder.ToListAsync());
        }

        // GET: MarketOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marketOrder = await _context.MarketOrder
                .FirstOrDefaultAsync(m => m.MarketOrderID == id);
            if (marketOrder == null)
            {
                return NotFound();
            }

            return View(marketOrder);
        }

        // GET: MarketOrders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MarketOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MarketOrderID,OrderDescription,TotalAmount,OrderStatus,OrderDate,UserID,HandledBy")] MarketOrder marketOrder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(marketOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(marketOrder);
        }

        // GET: MarketOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marketOrder = await _context.MarketOrder.FindAsync(id);
            if (marketOrder == null)
            {
                return NotFound();
            }
            return View(marketOrder);
        }

        // POST: MarketOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MarketOrderID,OrderDescription,TotalAmount,OrderStatus,OrderDate,UserID,HandledBy")] MarketOrder marketOrder)
        {
            if (id != marketOrder.MarketOrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(marketOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarketOrderExists(marketOrder.MarketOrderID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(marketOrder);
        }

        // GET: MarketOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marketOrder = await _context.MarketOrder
                .FirstOrDefaultAsync(m => m.MarketOrderID == id);
            if (marketOrder == null)
            {
                return NotFound();
            }

            return View(marketOrder);
        }

        // POST: MarketOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var marketOrder = await _context.MarketOrder.FindAsync(id);
            _context.MarketOrder.Remove(marketOrder);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MarketOrderExists(int id)
        {
            return _context.MarketOrder.Any(e => e.MarketOrderID == id);
        }


        // table storage - Feedback -------------------------------- start -------------------------------
        private CloudTable getTableContainerInformation()
        {
            //read appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            IConfigurationRoot configure = builder.Build();

            //get access key from appsettings.json
            CloudStorageAccount accountdetails = CloudStorageAccount
                .Parse(configure["ConnectionStrings:blobstorageconnection"]);

            //how to refer to an existing / new table in blob storage account
            CloudTableClient clientagent = accountdetails.CreateCloudTableClient();
            CloudTable table = clientagent.GetTableReference("Feedback");

            return table;
        }

        //insert data into the table storage
        public ActionResult AddFeedbackResult(string PartitionKey, string RowKey, string Feedback_Content, string Feedback_Reaction, string SellerID)
        {
            CloudTable table = getTableContainerInformation();
            table.CreateIfNotExistsAsync();

            //partition key = UserID (customer), row key = MarketOrderID

            DateTime today = DateTime.Now;

            FeedbackEntity feedback = new FeedbackEntity(PartitionKey, RowKey);
            feedback.Feedback_Content = Feedback_Content;
            feedback.Feedback_Date = today;
            feedback.Feedback_Reaction = Feedback_Reaction;
            feedback.SellerID = SellerID;


            try
            {
                // Loads the current context and find the entity by the id
                var marketOrder = _context.MarketOrder.Find(Convert.ToInt32(RowKey));

                // Perform the changes you want..
                marketOrder.GetFeedback = Feedback_Reaction;

                // Then call save normally.
                _context.Update(marketOrder);
                _context.SaveChangesAsync();


                TableOperation insertOperation = TableOperation.Insert(feedback);
                TableResult insertResult = table.ExecuteAsync(insertOperation).Result;
                ViewBag.result = insertResult.HttpStatusCode;
                ViewBag.TableName = table.Name;

            }
            catch (Exception ex)
            {
                ViewBag.result = 100;
                ViewBag.Message = "Error: " + ex.ToString();
            }

            return View();
        }

        public ActionResult AddFeedback(string id, string sellerid)
        {

            ViewData["ID"] = id;
            ViewData["SellerID"] = sellerid;

            return View();
        }

        public ActionResult ViewSingleFeedback(string pkey, string rkey)
        {
            CloudTable table = getTableContainerInformation();
            string message = null;

            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<FeedbackEntity>(pkey, rkey);
                TableResult result = table.ExecuteAsync(retrieveOperation).Result;

                if (result.Etag != null)
                {
                    //convert the Feedback information from table result to become FeedbackEntity type
                    var feedback = result.Result as FeedbackEntity;
                    return View(feedback);
                }

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }


            return RedirectToAction("Index", "MarketOrders", new { Message = message });
        }
        // table storage - Feedback -------------------------------- end ---------------------------------
    }
}
