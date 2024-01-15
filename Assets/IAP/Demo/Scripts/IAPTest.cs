using System;
using UnityEngine;

namespace IAPTest
{
    public class IAPTest : MonoBehaviour
    {
        [SerializeField] private GameObject shopHolder;
        
        private void OnEnable()
        {
            IAPManager.OnPurchaseCompleted += RewardPlayer;
        }

        private void OnDisable()
        {
            IAPManager.OnPurchaseCompleted -= RewardPlayer;
        }

        private void RewardPlayer(PurchaseResult _result)
        {
            if (!_result.Successful)
            {
                Debug.Log(_result.Message);
                return;
            }

            switch (_result.Product.Id)
            {
                case "diamonds":
                    PlayerData.Diamonds += _result.Product.Amount;
                    break;
                case "coins":
                    PlayerData.Coins += _result.Product.Amount;
                    break;
                case "remove_ads":
                    PlayerData.ShowAdsStatus = false;
                    break;
                case "boost_experience":
                    PlayerData.HasExperienceBoost = true;
                    break;
                default:
                    throw new Exception("Failed to reward player for purchase of product with id: " + _result.Product.Id);
            }
        }

        private void Start()
        {
            IAPManager.Instance.Init(EnableShop);
        }

        private void EnableShop()
        {
            shopHolder.SetActive(true);
        }
    }
}

