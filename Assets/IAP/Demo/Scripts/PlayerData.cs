using System;

public static class PlayerData
{
    public static Action UpdatedDiamonds;
    public static Action UpdatedCoins;
    public static Action UpdatedAdsStatus;
    
    private static int diamonds;
    private static int coins;
    private static bool showAdsStatus = true;
    
    public static bool HasExperienceBoost;
    public static int Diamonds
    {
        get => diamonds;
        set
        {
            diamonds = value;
            UpdatedDiamonds?.Invoke();
        }
    }

    public static int Coins
    {
        get => coins;
        set
        {
            coins = value;
            UpdatedCoins?.Invoke();
        }
    }

    public static bool ShowAdsStatus
    {
        get => showAdsStatus;
        set
        {
            showAdsStatus = value;
            UpdatedAdsStatus?.Invoke();
        }
    }
}
