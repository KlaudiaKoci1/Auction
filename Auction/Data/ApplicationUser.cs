using Auction.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auction.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual ICollection<AuctionViewModel> AuctionViewModel { get; set; }
        public virtual ICollection<WalletViewModel> WalletViewModel { get; set; }
    }
}
