﻿using System.ComponentModel.DataAnnotations;

namespace RxBlockChain.Core.DTO
{
    public class TransactionDTO
    {
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string FromAddress { get; set; } = string.Empty;

        [Required]
        public string ToAddress { get; set; } = string.Empty;

        public string Mnemonic { get; set; } = string.Empty;
    }
}
