using System;

namespace Demo;

public class Invoice
{
    public decimal Subtotal { get; set; }

    public decimal Total()
    {
        return Subtotal;
    }
}
