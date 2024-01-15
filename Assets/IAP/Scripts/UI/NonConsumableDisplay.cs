using IAPProducts;
using UnityEngine;

public class NonConsumableDisplay : ProductDisplayBase
{
    [field: SerializeField] public NonConsumable Product { get; private set; }
    [SerializeField] private bool oneTimePurchase;

    protected override void OnEnable()
    {
        base.OnEnable();
        IAPManager.OnPurchaseCompleted += CheckPurchasedProduct;
        IAPManager.OnFinishedInit += ManageOneTimePurchase;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        IAPManager.OnPurchaseCompleted -= CheckPurchasedProduct;
        IAPManager.OnFinishedInit -= ManageOneTimePurchase;
    }

    private void CheckPurchasedProduct(PurchaseResult _purchasedProduct)
    {
        if (!_purchasedProduct.Successful)
        {
            return;
        }

        if (_purchasedProduct.Product.Id!=Product.Id)
        {
            return;
        }

        ManageOneTimePurchase();
    }
    
    private void ManageOneTimePurchase()
    {
        if (!oneTimePurchase)
        {
            return;
        }
        
        bool _owned = IAPManager.Instance.CheckNonConsumable(Product.Id);
        if (_owned)
        {
            ManagePurchaseButton(false);
        }
    }

    private void Awake()
    {
        Setup(Product);
    }

    protected override bool CanPurchase()
    {
        if (!oneTimePurchase)
        {
            return true;
        }
        
        bool _owned = IAPManager.Instance.CheckNonConsumable(Product.Id);
        if (_owned)
        {
            ManagePurchaseButton(false);
            return false;
        }
    
        return true;
    }
}
