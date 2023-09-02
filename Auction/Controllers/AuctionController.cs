using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Auction.Data;
using Auction.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Auction.Repositories;

namespace Auction.Controllers
{
    [Authorize]
    public class AuctionController : Controller
    {
        private readonly AuctionDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWalletRepository _walletRepo;

        public AuctionController(AuctionDbContext context, UserManager<ApplicationUser> userManager, IWalletRepository walletRepo
            )
        {
            _context = context;
            _userManager = userManager;
            _walletRepo = walletRepo;
        }

        // GET: Auction
        public async Task<IActionResult> Index()
        {
            List<AuctionProductModel> auctionProductlist = new List<AuctionProductModel>();
            var auction = await _context.Auctions.ToListAsync();
            foreach (var a in auction)
            {
                var userRecord = new ApplicationUser();
                if (a.ApplicationUserId != null)
                {
                    userRecord = await _userManager.FindByIdAsync(a.ApplicationUserId);
                }
                if (a.Deleted == false)
                {
                    if (a.EndDate < DateTime.Today)
                    {
                        //shton cmimin e bit nga wallet i userit qe ka krijuar auction update wallet
                        await _walletRepo.UpdateWallet(a.ApplicationUserId, a.Bid, "Add");
                        //gjej userin qe ka vendopsur bid me te larte per tja hequr shumen nga wallet
                        var userid = await _walletRepo.UserBid(a.Id);
                        await _walletRepo.UpdateWallet(userid, a.Bid, "Remove");

                        a.Deleted = true;
                        _context.Update(a);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        var productname = await _context.Products.Where(p => p.Id == a.ProductViewModelId).Select(p => p.ProductName).FirstOrDefaultAsync();

                        AuctionProductModel auctionProductModel = new AuctionProductModel();

                        auctionProductModel.Id = a.Id;
                        auctionProductModel.Bid = a.Bid;
                        auctionProductModel.EndDate = a.EndDate;
                        if (userRecord.FirstName != null)
                        {
                            auctionProductModel.ApplicantName = userRecord.FirstName;
                        }

                        auctionProductModel.ApplicantiD = a.ApplicationUserId;
                        auctionProductModel.ProductId = a.ProductViewModelId;
                        auctionProductModel.ProductName = productname;

                        auctionProductlist.Add(auctionProductModel);
                    }
                }

            }
            return View(auctionProductlist);
        }

        // GET: Auction/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auctionViewModel =  _context.Auctions
              .FirstOrDefault(m => m.Id == id);

            var userRecord = new ApplicationUser();
            if (auctionViewModel.ApplicationUserId != null)
            {
                userRecord = await _userManager.FindByIdAsync(auctionViewModel.ApplicationUserId);

            }

            var productname =  _context.Products.Where(p => p.Id == auctionViewModel.ProductViewModelId).FirstOrDefault();

            AuctionProductModel auctionProductModel = new AuctionProductModel();

            auctionProductModel.Id = auctionViewModel.Id;
            auctionProductModel.Bid = auctionViewModel.Bid;
            auctionProductModel.EndDate = auctionViewModel.EndDate;
            if (userRecord.FirstName != null)
            {
                auctionProductModel.ApplicantName = userRecord.FirstName;
            }

            auctionProductModel.ApplicantiD = auctionViewModel.ApplicationUserId;
            auctionProductModel.ProductId = auctionViewModel.ProductViewModelId;
            auctionProductModel.ProductName = productname.ProductName;
            auctionProductModel.Description = productname.ProductDescription;




            if (auctionProductModel == null)
            {
                return NotFound();
            }

            return View(auctionProductModel);
        }

        // GET: Auction/Create
        public IActionResult Create()
        {
            var productsList = (from product in _context.Products
                                select new SelectListItem()
                                {
                                    Text = product.ProductName,
                                    Value = product.Id.ToString(),
                                }).ToList();
            //E kam bere ne kete menyre thjeshte per te mbushur tabel produkt me nje vlere. Pasi tek pikat e detyres
            // nuk specifikohet produkti kur shtohet thuhej vetem qe ekziston. 
           
            if (productsList.Count == 0)
            {
                var produkt = new ProductViewModel();
                produkt.ProductName = "Produkt1";
                produkt.ProductDescription = "Pershkrimi i produktit1";
                _context.Add(produkt);

                 _context.SaveChanges();

                productsList = (from product in _context.Products
                                select new SelectListItem()
                                {
                                    Text = product.ProductName,
                                    Value = product.Id.ToString(),
                                }).ToList();

            }

            productsList.Insert(0, new SelectListItem()
            {
                Text = "----Select----",
                Value = string.Empty
            });

            ProductViewModel productViewModel = new ProductViewModel();
            ViewBag.List = productsList;
            return View();
        }

        // POST: Auction/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Bid,EndDate,ApplicationUserId,ProductViewModelId")] AuctionViewModel auctionViewModel)
        {
            if (ModelState.IsValid)
            {
                var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                auctionViewModel.ApplicationUserId = userid;
                var bid = new BidViewModel();
                bid.Bid = auctionViewModel.Bid;
                bid.UserId = userid;
                auctionViewModel.Bids = new List<BidViewModel>();
                auctionViewModel.Bids.Add(bid);
                _context.Add(auctionViewModel);

                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }
            return View(auctionViewModel);
        }



        // POST: Auction/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int id, decimal Bid, [Bind("Id,Bid")] AuctionViewModel auctionViewModel)
        {
            if (id != auctionViewModel.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var updatebid = _context.Auctions.Where(a => a.Id == id).FirstOrDefault();
                    updatebid.Bid = Bid;
                    _context.Update(updatebid);
                    //heq cmimin e bit nga wallet i userit te loguar. update wallet
                    // await _walletRepo.UpdateWallet(userid, auctionViewModel.Bid, "Remove");
                    //shtohet tek tabela Bid per te rujtuar userin qe beri veprimin
                    await _walletRepo.AddBid(userid, id, Bid);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuctionViewModelExists(auctionViewModel.Id))
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
            return View(auctionViewModel);
        }


        // POST: Auction/Delete/5

        public async Task<IActionResult> Delete(int id)
        {
            var auctionViewModel = await _context.Auctions.FindAsync(id);


            //shton cmimin e bit nga wallet i userit qe ka krijuar auction update wallet
            await _walletRepo.UpdateWallet(auctionViewModel.ApplicationUserId, auctionViewModel.Bid, "Add");
            //gjej userin qe ka vendopsur bid me te larte per tja hequr shumen nga wallet
            var userid = await _walletRepo.UserBid(auctionViewModel.Id);
            await _walletRepo.UpdateWallet(userid, auctionViewModel.Bid, "Remove");

            auctionViewModel.Deleted = true;
            _context.Update(auctionViewModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuctionViewModelExists(int id)
        {
            return _context.Auctions.Any(e => e.Id == id);
        }

        [HttpGet]
        public JsonResult GetProduct(int productId)
        {
            //  int orderID = _context.Products.Where(a => a.Id == orderId).Select().FirstOrDefault();
            var proddesc = (from od in _context.Products
                            where od.Id == productId
                            select new
                            {
                                od.ProductDescription
                            }).FirstOrDefault();


            return Json(proddesc);
        }

        [HttpGet]
        public JsonResult GetInfoWallet()
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var walletinfo = _walletRepo.InfoWallet(userid);

            return Json(walletinfo);
        }
    }


}
