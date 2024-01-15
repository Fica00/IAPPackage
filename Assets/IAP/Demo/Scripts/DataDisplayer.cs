using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DataDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI diamondsDisplay;
    [SerializeField] private TextMeshProUGUI coinsDisplay;
    [SerializeField] private TextMeshProUGUI adsDisplay;
    [SerializeField] private TextMeshProUGUI experienceBoostExpirationDisplay;

    private void OnEnable()
    {
        PlayerData.UpdatedDiamonds += ShowDiamonds;
        PlayerData.UpdatedCoins += ShowCoins;
        PlayerData.UpdatedAdsStatus += ShowAdsStatus;
        
        ShowCoins();
        ShowDiamonds();
        ShowAdsStatus();
    }

    private void OnDisable()
    {
        PlayerData.UpdatedDiamonds -= ShowDiamonds;
        PlayerData.UpdatedCoins -= ShowCoins;
        PlayerData.UpdatedAdsStatus -= ShowAdsStatus;
    }

    private void ShowDiamonds()
    {
        diamondsDisplay.text = $"Diamonds: {PlayerData.Diamonds}";
    }
    
    private void ShowCoins()
    {
        coinsDisplay.text = $"Coins: {PlayerData.Coins}";
    }

    private void ShowAdsStatus()
    {
        adsDisplay.text = $"Should show ads: {PlayerData.ShowAdsStatus}";
    }

    private void Start()
    {
        StartCoroutine(ShowExperienceBoostExpiration());
    }

    private IEnumerator ShowExperienceBoostExpiration()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            var _info = IAPManager.Instance.CheckSubscription("boost_experience");
            if (_info != null)
            {
                experienceBoostExpirationDisplay.text = $"Experience boost expiring in:\n{(DateTime.Now - _info.getExpireDate()).TotalMinutes}min";
            }
            else
            {
                experienceBoostExpirationDisplay.text = "Experience boost not available";
                if (PlayerData.HasExperienceBoost)
                {
                    PlayerData.HasExperienceBoost = false;
                }
            }
        }
    }
}
