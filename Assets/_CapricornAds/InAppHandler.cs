using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Purchasing;

#region InAppStructure
public enum InAppItemName
{
    Coins,
    Stars,
    RemoveAds,
    UnlockAllVehicles,
    UnlockEveryThing,
    SubscriptionWeekly,
    SubscriptionMonthly,
    SubscriptionQuaterly,
}
[System.Serializable]
public class ConsumableInApps
{
    public string inAppName, inAppId;
    public int quantityToGive;
    //public int priceForInApp;
    public InAppItemName inAppType = InAppItemName.Coins;
}

[System.Serializable]
public class NonConsumeableInApps
{
    public string inAppName;
    public string inAppId;
    //public int priceForInApp;
    public InAppItemName inAppType = InAppItemName.RemoveAds;
}
[System.Serializable]
public class SubscriptionInApps
{
    public string inAppName;
    public string inAppId;
    //public int priceForInApp;
    public InAppItemName inAppType = InAppItemName.SubscriptionWeekly;
}
#endregion
public class InAppHandler : MonoBehaviour, IStoreListener
{

    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.


    public List<ConsumableInApps> consumeableInApps;
    public List<NonConsumeableInApps> nonConsumeableInApps;
    public List<SubscriptionInApps> subscriptionInApps;
    public static InAppHandler Instance;


    //    #region NonConsumableInAppsKeys
    //#if UNITY_ANDROID

    //    public string REMOVE_ADS = "removeads";
    //    public string UNLOCK_ALL = "unlockall";

    //#endif

    //#if UNITY_IOS
    //        public string REMOVE_ADS = "removeads";
    //        public string UNLOCK_ALL = "unlockall";
    //#endif
    //    #endregion

    //    #region SubscriptionInAppsKeys
#if UNITY_ANDROID

    //    public string WEEKLY_SUBSCRIPTION = "weeklysubscription";
    //    public string MONTHLY_SUBSCRIPTION = "monthlysubscription";
    //    public string QUATERLY_SUBSCRIPTION = "quaterlysubscription";

    //    // Google Play Store-specific product identifier subscription product.
    private static string kProductNameGooglePlaySubscription = "com.unity3d.subscription.original";
#endif

#if UNITY_IOS

    //public string WEEKLY_SUBSCRIPTION = "weeklysubscription";
    //public string MONTHLY_SUBSCRIPTION = "monthlysubscription";
    //public string QUATERLY_SUBSCRIPTION = "quaterlysubscription";
        // Apple App Store-specific product identifier for the subscription product.
    private static string kProductNameAppleSubscription = "com.unity3d.subscription.new";
#endif
    //#endregion
    public void OnPurchaseComplete(string id)
    {
        Debug.LogError("Coins have been added successfully");
    }
    public void OnPurchaseFailed()
    {
        Debug.LogError("Purchase Failed");
    }
    public void RemoveAdsPurchased()
    {
        Debug.LogError("Remove Ads purchased!");
    }
    void Start()
    {
        Instance = this;
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {

            return;
        }
        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        foreach (var consumable in consumeableInApps)
        {
            builder.AddProduct(consumable.inAppId, ProductType.Consumable);
        }

        foreach (var nonConsumable in nonConsumeableInApps)
        {

            builder.AddProduct(nonConsumable.inAppId, ProductType.NonConsumable);
            //Toast.current.Show("Marked consume");

        }

        foreach (var subscription in subscriptionInApps)
        {
            builder.AddProduct(subscription.inAppId, ProductType.Subscription);
        }

        UnityPurchasing.Initialize(this, builder);
    }

    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);

    }

    public void Buy_RemoveAds()
    {
        BuyProductID("removeads");
    }

    public void Buy_UnlockAll_Vehicles()
    {
        BuyProductID("unlockallvehicles");
    }

    public void Buy_Remove_EveryThing()
    {
        BuyProductID("unlockall");
    }

    public void Buy_Coins1()
    {
        BuyProductID("morecoinone");
    }

    public void BuyProductID(string productId)
    {

        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        ConsumableInApps consumableSucceededInApp = consumeableInApps.Find(x => x.inAppId == args.purchasedProduct.definition.id);

        // A consumable product has been purchased by this user.
        if (consumableSucceededInApp != null)
        {
            if (consumableSucceededInApp.inAppType == InAppItemName.Coins)
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + consumableSucceededInApp.quantityToGive);
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);//reload Scene
            }
            else if (consumableSucceededInApp.inAppType == InAppItemName.Stars)
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            }

        }

        // Or ... a non-consumable product has been purchased by this user.
        else
        {
            NonConsumeableInApps nonConsumableSucceededInapp = nonConsumeableInApps.Find(x => x.inAppId == args.purchasedProduct.definition.id);
            if (nonConsumableSucceededInapp != null)
            {
                if (nonConsumableSucceededInapp.inAppType == InAppItemName.RemoveAds)
                {
                    Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                //    PreferenceManager.SetAdsStatus(1);
                    SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);//reload Scene
                }
                else if (nonConsumableSucceededInapp.inAppType == InAppItemName.UnlockAllVehicles)
                {
                    Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                    PlayerPrefs.SetInt("cars", 10);
                    SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);//reload Scene
                }
                else if (nonConsumableSucceededInapp.inAppType == InAppItemName.UnlockEveryThing)
                {
                    Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                //    PreferenceManager.SetAdsStatus(1);
                    PlayerPrefs.SetInt("cars", 10);
                    SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);//reload Scene
                }
            }

            // Or ... a subscription product has been purchased by this user.
            else
            {
                SubscriptionInApps subscriptionSucceededInApp = subscriptionInApps.Find(x => x.inAppId == args.purchasedProduct.definition.id);
                if (subscriptionSucceededInApp != null)
                {
                    if (subscriptionSucceededInApp.inAppType == InAppItemName.SubscriptionWeekly)
                    {
                        Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                    }
                    // Or ... an unknown product has been purchased by this user. Fill in additional products here....
                    else if (subscriptionSucceededInApp.inAppType == InAppItemName.SubscriptionMonthly)
                    {
                        Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                    }
                    else if (subscriptionSucceededInApp.inAppType == InAppItemName.SubscriptionQuaterly)
                    {
                        Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                    }
                    else
                    {
                        Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
                    }
                }
            }

        }
        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }

    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }
}
