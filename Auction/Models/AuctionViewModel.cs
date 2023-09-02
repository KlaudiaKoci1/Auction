using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Auction.Models
{
    public class AuctionViewModel
    {
        public int Id { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal Bid { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        public bool Deleted { get; set; }

        public string ApplicationUserId {get; set;}

       public int ProductViewModelId { get; set; }

        public ICollection<BidViewModel> Bids { get; set; } = null!;
      
    }
}
