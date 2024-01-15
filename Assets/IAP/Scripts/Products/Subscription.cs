using System;
using UnityEngine;

namespace IAPProducts
{
    [Serializable]
    public class Subscription : ProductBase
    {
        [Tooltip("In days")] public float TimeDuration;
    }
}