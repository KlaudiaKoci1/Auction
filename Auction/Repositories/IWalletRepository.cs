using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auction.Repositories
{
   public interface IWalletRepository
    {
         Task<bool> AddWallet(string applicantId);
         Task<bool> UpdateWallet(string applicantId , decimal bit, string removeoreadd);

         decimal InfoWallet(string applicantId);
        Task<bool> AddBid(string applicantId, int AuctionId, decimal bid);
        Task<string> UserBid(int AuctionId);
    }
}
