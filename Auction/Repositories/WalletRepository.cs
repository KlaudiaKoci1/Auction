using Auction.Data;
using Auction.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auction.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly AuctionDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WalletRepository(AuctionDbContext db, IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> AddWallet(string applicantId)
        {
            try
            {
                if (string.IsNullOrEmpty(applicantId))
                    throw new Exception("user is not logged-in");
                var wallet = new WalletViewModel();
                wallet.ApplicationUserId = applicantId;
                wallet.Wallet = (decimal)1000.00;
                _db.Add(wallet);
                await _db.SaveChangesAsync();
                return true;
                
            }
            catch (Exception)
            {
                return false;
            }
        }



        public async Task<bool> UpdateWallet(string applicantId, decimal bit, string removeoreadd)
        {
            var auctionViewModel = _db.Wallets
               .Where(m => m.ApplicationUserId == applicantId).FirstOrDefault();
            if (auctionViewModel != null)
            {
                if (removeoreadd.Contains("Remove"))
                {
                    auctionViewModel.Wallet = auctionViewModel.Wallet - bit;
                }
                else if (removeoreadd.Contains("Add"))
                {
                    auctionViewModel.Wallet = auctionViewModel.Wallet + bit;
                }


                _db.Update(auctionViewModel);
                await _db.SaveChangesAsync();

            }

         

            return true;
        }

        public decimal InfoWallet(string applicantId)
        {
            var auctionViewModel =  _db.Wallets
              .Where(m => m.ApplicationUserId == applicantId).FirstOrDefault();

            return auctionViewModel.Wallet;
           
        }

        public async Task<bool> AddBid(string applicantId, int AuctionId, decimal bid)
        {
            try
            {
                if (string.IsNullOrEmpty(applicantId))
                    throw new Exception("user is not logged-in");
                var bids = new BidViewModel();
                bids.AuctionViewModelId = AuctionId;
                bids.Bid = bid;
                bids.UserId = applicantId;
                _db.Add(bids);
                await _db.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<string> UserBid(int AuctionId)
        {
            var auctionViewModel = await _db.Bids
            .Where(m => m.AuctionViewModelId == AuctionId).OrderByDescending(i => i.Bid).FirstAsync();


            return (auctionViewModel.UserId);

        }

    }

}
