using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ComeMyFishMarket.Data;
using ComeMyFishMarket.Models;
using ComeMyFishMarket.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using System.IO;

namespace ComeMyFishMarket.Controllers
{
    public class MarketOrdersController : Controller
    {
        private readonly ComeMyFishMarketClassContext _context;

        private readonly UserManager<ComeMyFishMarketUser> _userManager;

        private readonly ComeMyFishMarketContext _context1;
        public MarketOrdersController(ComeMyFishMarketClassContext context, ComeMyFishMarketContext context1, UserManager<ComeMyFishMarketUser> userManager)
        {
            _context = context;
            _context1 = context1;
            _userManager = userManager;
        }

        // GET: MarketOrders
        public async Task<IActionResult> Index()
        {
            return View(await _context.MarketOrder.ToListAsync());
        }

        public async Task<IActionResult> CustomerOrder()
        {
            return View(await _context.MarketOrder.ToListAsync());
        }
        public async Task<IActionResult> OrderList()
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

        public async Task<IActionResult> OrderPayment(int? id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderPayment(int id, string desc, double totalamount, string userid, string receivedby)
        {
            var order = await _context.MarketOrder.FindAsync(id);
            var user = _context1.Users.FirstOrDefault(x => x.Id.Equals(userid));
            var receiver = _context1.Users.FirstOrDefault(x => x.Id.Equals(receivedby));
            var admin = _context1.Users.FirstOrDefault(x => x.Role.Equals("Admin"));
            if (user.UserWallet < totalamount)
            {
                ViewBag.Message = "Current Wallet Amount: RM" + user.UserWallet.ToString() + "! Please Top Up To Continue The Payment Process!!";
                return View("OrderPayment");
            }
            else
            {
                if(receiver.Role.Equals("Admin"))
                {
                    user.UserWallet -= totalamount;
                    receiver.UserWallet += totalamount;
                    _context1.SaveChanges();
                    Payment odpay = new Payment
                    {
                        PaymentDescription = desc,
                        PaymentDate = DateTime.Now,
                        CustomerID = userid,
                        ReceivedBy = receivedby,
                        TotalAmount = totalamount,
                        MarketOrderID = order.MarketOrderID
                    };
                    WalletHistory userwal = new WalletHistory
                    {
                        HistoryDesc = "Payment For OrderID:" + order.MarketOrderID.ToString(),
                        HistoryAmount = "-" + totalamount.ToString(),
                        HistoryDate = DateTime.Now,
                        UserID = user.Id
                    };
                    WalletHistory recwal = new WalletHistory
                    {
                        HistoryDesc = "Payment Receive From OrderID:" + order.MarketOrderID.ToString(),
                        HistoryAmount = "+" + totalamount.ToString(),
                        HistoryDate = DateTime.Now,
                        UserID = receiver.Id
                    };
                    _context.Payment.Add(odpay);
                    _context.WalletHistory.Add(userwal);
                    _context.WalletHistory.Add(recwal);
                    
                    order.OrderStatus = "Paid";
                    _context.SaveChanges();
                    TempData["CompleteMsg"] = "Payment Has Been Proceed!!";
                    return RedirectToAction("CustomerOrder", "MarketOrders");
                }
                else if(receiver.Role.Equals("Seller"))
                {
                    double commission = totalamount * 0.05;
                    double recamount = totalamount - commission;
                    user.UserWallet -= totalamount;
                    receiver.UserWallet += recamount;
                    admin.UserWallet += commission;
                    _context1.SaveChanges();
                    Payment odpay = new Payment
                    {
                        PaymentDescription = desc,
                        PaymentDate = DateTime.Now,
                        CustomerID = userid,
                        ReceivedBy = receivedby,
                        TotalAmount = totalamount,
                        MarketOrderID = order.MarketOrderID
                    };
                    WalletHistory userwal = new WalletHistory
                    {
                        HistoryDesc = "Payment For OrderID:" + order.MarketOrderID.ToString(),
                        HistoryAmount = "-" + totalamount.ToString(),
                        HistoryDate = DateTime.Now,
                        UserID = user.Id
                    };
                    WalletHistory recwal = new WalletHistory
                    {
                        HistoryDesc = "Payment Receive From OrderID:" + order.MarketOrderID.ToString(),
                        HistoryAmount = "+" + recamount.ToString(),
                        HistoryDate = DateTime.Now,
                        UserID = receiver.Id
                    };
                    WalletHistory adwal = new WalletHistory
                    {
                        HistoryDesc = "Commission Received From Seller:" + receiver.UserName + " On OrderID:" + order.MarketOrderID.ToString(),
                        HistoryAmount = "+" + commission.ToString(),
                        HistoryDate = DateTime.Now,
                        UserID = admin.Id
                    };
                    _context.Payment.Add(odpay);
                    _context.WalletHistory.Add(userwal);
                    _context.WalletHistory.Add(recwal);
                    _context.WalletHistory.Add(adwal);
                    order.OrderStatus = "Paid";
                    _context.SaveChanges();
                    TempData["CompleteMsg"] = "Payment Has Been Proceed!!";
                    return RedirectToAction("CustomerOrder","MarketOrders");
                }
                return View(order);
            }
        }

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


            return RedirectToAction("CustomerOrder", "MarketOrders", new { Message = message });
        }

        public ActionResult ViewReport(int Year, int Month)
        {
            //select all from MarketOrder
            var marketorder = from mo in _context.MarketOrder
                              select mo;

            //select UserName(Seller) from AspNetUser
            IQueryable<string> sql_seller = from mo in _context1.Users
                                       where mo.Role != "Customer"
                                       select mo.UserName;

            ViewBag.Seller = sql_seller.Distinct().ToList();

            //select Year(OrderDate) from MarketOrder OrderBy Year(OrderDate)
            IQueryable<int> sql_year = from mo in _context.MarketOrder
                                          orderby mo.OrderDate.Year descending
                                          select mo.OrderDate.Year;


            IEnumerable<SelectListItem> exist_year =
                new SelectList( sql_year.Distinct().ToList() );

            ViewBag.Year = exist_year;
            ViewData["SelectedYear"] = "";
            ViewData["SelectedMonth"] = "";

            if (Year != 0)
            {
                marketorder = marketorder.Where(mo => mo.OrderDate.Year.Equals(Year));
                ViewData["SelectedYear"] = Year;
            }

            if (Month != 0)
            {
                marketorder = marketorder.Where(mo => mo.OrderDate.Month.Equals(Month));
                ViewData["SelectedMonth"] = Month;
            }

            return View( marketorder.ToList() );
        }


    }
}
