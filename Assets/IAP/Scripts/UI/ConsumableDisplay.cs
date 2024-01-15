using IAPProducts;
using UnityEngine;

public class ConsumableDisplay : ProductDisplayBase
{
    [field: SerializeField] public Consumable Product { get; private set; }

    private void Awake()
    {
        Setup(Product);
    }
}
