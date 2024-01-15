using System;
using IAPProducts;

[Serializable]
public class PurchaseResult
{
    public bool Successful;
    public string Message;
    public ProductBase Product;
}
