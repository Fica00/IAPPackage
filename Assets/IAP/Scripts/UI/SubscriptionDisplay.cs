using IAPProducts;
using UnityEngine;

public class SubscriptionDisplay : ProductDisplayBase
{
    [field: SerializeField] public Subscription Product { get; private set; }

    private void Awake()
    {
        Setup(Product);
    }

    protected override bool CanPurchase()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor ||
            Application.platform == RuntimePlatform.LinuxEditor)

        {
            Debug.Log("Subscription purchases are not allowed inside Unity editor");
            return false;
        }

        return true;
    }
}
