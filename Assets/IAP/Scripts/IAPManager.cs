using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using IAPProducts;
using Newtonsoft.Json;

public class IAPManager : MonoBehaviour, IDetailedStoreListener
{
    public static IAPManager Instance;
    public static Action<PurchaseResult> OnPurchaseCompleted;
    public static Action OnFinishedInit;

    [SerializeField] private bool showLogs;
    
    private Action onInitialized;
    private IStoreController storeController;
    private IExtensionProvider extensions;
    private Products products;

    private string buyingProductId;
    
    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init(Action _callBack=null)
    {
        if (IsInit())
        {
            _callBack?.Invoke();
            return;
        }
        
        Products _products = new Products();

        foreach (var _productDisplay in FindObjectsOfType<ProductDisplayBase>())
        {
            switch (_productDisplay)
            {
                case ConsumableDisplay _consumableDisplay:
                    _products.Consumables.Add(_consumableDisplay.Product);
                    break;
                case NonConsumableDisplay _nonConsumable:
                    _products.NonConsumables.Add(_nonConsumable.Product);
                    break;
                case SubscriptionDisplay _subscription:
                    _products.Subscriptions.Add(_subscription.Product);
                    break;
            }
        }

        if (_products.Count==0)
        {
            ShowLog("There are no valid products detected, please add products on the scene or invoke with json data");
            return;
        }

        Init(JsonConvert.SerializeObject(_products), _callBack);
    }
    
    public void Init(string _productsData, Action _callBack=null)
    {
        if (IsInit())
        {
            _callBack?.Invoke();
            return;
        }
        
        InitializeGamingServices.Init(this,(_result, _message) =>
        {
            if (!_result)
            {
                ShowLog("Failed to initialize gaming services: "+_message);
                return;
            }
            
            onInitialized = _callBack;
        
            if (string.IsNullOrEmpty(_productsData))
            {
                ShowLog("Cant initialize with empty product data");
                onInitialized?.Invoke();
                return;
            }
        
            ConfigurationBuilder _builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            products = JsonConvert.DeserializeObject<Products>(_productsData);
        
            if (products==null)
            {
                ShowLog("Failed to convert productsData to Products");
                onInitialized?.Invoke();
                return;
            }

            AddProducts(products.Consumables.Cast<ProductBase>().ToList(), ProductType.Consumable);
            AddProducts(products.NonConsumables.Cast<ProductBase>().ToList(), ProductType.NonConsumable);
            AddProducts(products.Subscriptions.Cast<ProductBase>().ToList(), ProductType.Subscription);

            UnityPurchasing.Initialize(this, _builder);

            void AddProducts(List<ProductBase> _createdProducts, ProductType _type)
            {
                foreach (var _createdProduct in _createdProducts)
                {
                    _builder.AddProduct(_createdProduct.Id, _type);
                }
            }
        });
    }

    private bool IsInit()
    {
        return storeController != null;
    }
    
    #region Initialization handlers

    public void OnInitializeFailed(InitializationFailureReason _error)
    {
        ShowLog("Failed to initialize UnityPurchasing");
        ShowLog(JsonConvert.SerializeObject(_error));
    }

    public void OnInitializeFailed(InitializationFailureReason _error, string _message)
    {
        ShowLog("Failed to initialize UnityPurchasing: " + _message);
        ShowLog(JsonConvert.SerializeObject(_error));
    }
    
    public void OnInitialized(IStoreController _controller, IExtensionProvider _extensions)
    {
        ShowLog("Successfully initialized");
        storeController = _controller;
        extensions = _extensions;
        OnFinishedInit?.Invoke();
        onInitialized?.Invoke();
    }

    #endregion

    #region PurchaseHandlers

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs _purchaseEvent)
    {
        FinishPurchase(true,string.Empty);
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product _product, PurchaseFailureReason _failureReason)
    {
        string _reason = _failureReason.ToString();
        ShowLog("Purchase failed: "+_reason);
        FinishPurchase(false,_reason);
    }

    public void OnPurchaseFailed(Product _product, PurchaseFailureDescription _failureDescription)
    {
        string _reason = _failureDescription.message;
        ShowLog("Purchase failed: "+_reason);
        FinishPurchase(false,_reason);
    }

    private void FinishPurchase(bool _successful, string _message)
    {
        OnPurchaseCompleted?.Invoke(new PurchaseResult()
        {
            Successful = _successful,
            Message = _message,
            Product = GetProductBase(buyingProductId)
        });
    }

    private ProductBase GetProductBase(string _productId)
    {
        ProductBase _productBase = null;
        
        _productBase = CheckForProduct(products.Consumables.Cast<ProductBase>().ToList());
        _productBase = CheckForProduct(products.NonConsumables.Cast<ProductBase>().ToList());
        _productBase = CheckForProduct(products.Subscriptions.Cast<ProductBase>().ToList());

        ProductBase CheckForProduct(List<ProductBase> _products)
        {
            if (_productBase!=null)
            {
                return _productBase;
            }

            foreach (var _product in _products)
            {
                if (_product.Id==_productId)
                {
                    return _product;
                }
            }

            return null;
        }

        if (_productBase==null)
        {
            ShowLog("Failed to find product!");
            throw new Exception("Failed to find product! Product id: " + _productId);
        }

        return _productBase;
    }
    
    #endregion

    public void InitializePurchase(string _productId)
    {
        Product _product = storeController.products.WithID(_productId);

        if (_product is { availableToPurchase: true })
        {
            ShowLog("Initializing purchase: "+_productId);
            buyingProductId = _productId;
            storeController.InitiatePurchase(_product);
        }
        else
        {
            ShowLog("InitializePurchase: FAIL. Not purchasing product, either is not found or is not available for purchase. Product id: "+_productId);
        }
    }

    public bool CheckNonConsumable(string _id)
    {
        if (storeController == null)
        {
            ShowLog("Store is not initialized");
            return false;
        }
        
        Product _product = storeController.products.WithID(_id);
        if (_product == null)
        {
            return false;
        }
        
        if (!_product.hasReceipt)
        {
            return false;
        }
        
        return true;
    }

    public SubscriptionInfo CheckSubscription(string _id)
    {
        if (storeController == null)
        {
            ShowLog("Store is not initialized");
            return null;
        }
        
        Product _subProduct = storeController.products.WithID(_id);
        if (_subProduct == null)
        {
            ShowLog("Product not found");
            return null;
        }
       
        if (!_subProduct.hasReceipt)
        {
            ShowLog("Receipt not found");
            return null;
        }
        
        SubscriptionManager _subManager = new SubscriptionManager(_subProduct, null);
        SubscriptionInfo _info = _subManager.getSubscriptionInfo();

        return _info.isSubscribed() != Result.True ? null : _info;
    }

    public void RestorePurchases()
    {
        if (Application.platform is RuntimePlatform.IPhonePlayer or RuntimePlatform.OSXPlayer or RuntimePlatform.tvOS)
        {
            ShowLog("RestorePurchases started ...");
            var _apple = extensions.GetExtension<IAppleExtensions>();
            _apple.RestoreTransactions(OnTransactionsRestored);
        }
        else
        {
            ShowLog("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    private void OnTransactionsRestored(bool _success, string _message)
    {
        ShowLog($"Restore result: {_success}: {_message}");
    }

    private void ShowLog(string _message)
    {
        if (!showLogs)
        {
            return;
        }
        
        Debug.Log(_message);
    }
}
