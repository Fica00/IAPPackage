using System;
using UnityEngine;

namespace IAPProducts
{
    [Serializable]
    public class ProductBase
    {
        public string Id;
        public string Name;
        public string Description;
        public int Amount;
        [Tooltip("In USD")]public float Price;
    }
}
