using System;
using System.Collections.Generic;

namespace Adroit_v8.Model;

public partial class PaymentType
{
    public int Id { get; set; }

    public string? PaymentTypeName { get; set; }

    public virtual ICollection<WalletCustomerTransaction> WalletCustomerTransactions { get; set; } = new List<WalletCustomerTransaction>();
}
