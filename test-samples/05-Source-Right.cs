using System;

namespace Demo;

public class Invoice
{
    public decimal Subtotal { get; set; }
    public decimal TaxRate { get; set; } = 0.05m;

    public decimal Total()
    {
        return Subtotal + (Subtotal * TaxRate);
    }
}
