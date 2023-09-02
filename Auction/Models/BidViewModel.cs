using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Auction.Models
{
    public class BidViewModel
    {
        public int Id {get; set;}
        [Column(TypeName = "decimal(18,2)")]
        public decimal Bid { get; set; }

        public string UserId { get; set; }

        public int AuctionViewModelId { get; set; }
    }
}
