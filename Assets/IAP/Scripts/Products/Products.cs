using System;
using System.Collections.Generic;

namespace IAPProducts
{
    [Serializable]
    public class Products
    {
        public int Count => Consumables.Count + NonConsumables.Count + Subscriptions.Count;
        public List<Consumable> Consumables = new();
        public List<NonConsumable> NonConsumables = new();
        public List<Subscription> Subscriptions = new();
    }
}