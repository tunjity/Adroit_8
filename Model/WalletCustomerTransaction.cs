using System;
using System.Collections.Generic;

namespace Adroit_v8.Model;

public partial class WalletCustomerTransaction
{
    public long Id { get; set; }

    public decimal TransAmount { get; set; }

    public DateOnly TransDate { get; set; }

    public string? TransDescription { get; set; }

    public char IsReversed { get; set; }

    public DateOnly? DateReversed { get; set; }

    public string? ReasonForReversal { get; set; }

    public string? PaystackPaymentReference { get; set; }

    public decimal? ServiceCharge { get; set; }

    public long? PayerWalletId { get; set; }

    public long? ReceiverWalletId { get; set; }

    public string? TransLocation { get; set; }

    public string? PhoneId { get; set; }

    public decimal? PaystackCharge { get; set; }

    public int? PaymentTypeId { get; set; }

    public char? DrCr { get; set; }

    public decimal? PayerBalanceBeforeDebit { get; set; }

    public decimal? PayerBalanceAfterDebit { get; set; }

    public decimal? ReceiverBalanceBeforeCredit { get; set; }

    public decimal? ReceiverBalanceAfterCredit { get; set; }

    public string? TransactionReference { get; set; }

    public virtual PaymentType? PaymentType { get; set; }
}
