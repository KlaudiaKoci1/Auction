using Auction.Data;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Auction.Models
{
    public class WalletViewModel
    {
        public int Id { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Wallet { get; set; }
        public string ApplicationUserId { get; set; }


    }
}
