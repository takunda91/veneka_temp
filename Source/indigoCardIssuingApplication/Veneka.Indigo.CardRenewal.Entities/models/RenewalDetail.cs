﻿using System;

namespace Veneka.Indigo.Renewal.Entities
{
    public class RenewalDetail
    {
        public long RenewalDetailId { get; set; }
        public long RenewalId { get; set; }
        public string CardNumber { get; set; }
        public string PSAction { get; set; }
        public string Blocked { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? RenewalDate { get; set; }
        public string Status { get; set; }
        public string ContractNumber { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? ODAmount { get; set; }
        public decimal? OLAmount { get; set; }
        public decimal? LimitBalance { get; set; }
        public string EmbossingName { get; set; }
        public string ClientId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string InternalAccountNumber { get; set; }
        public string ExternalAccountNumber { get; set; }
        public string PassportIDNumber { get; set; }
        public string ContractStatus { get; set; }
        public string EmailAddress { get; set; }
        public string CustomerName { get; set; }
        public DateTime? CreationDate { get; set; }
        public string OnlineStatus { get; set; }
        public string ContactPhone { get; set; }
        public string MobilePhone { get; set; }
        public int ProductId { get; set; }
        public int BranchId { get; set; }
        public int DeliveryBranchId { get; set; }
        public string MBR { get; set; }
        public RenewalDetailStatusType RenewalStatus { get; set; }
    }
}
