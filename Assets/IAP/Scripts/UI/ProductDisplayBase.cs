using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IAPProducts
{
    public class ProductDisplayBase : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameDisplay;
        [SerializeField] private TextMeshProUGUI descriptionDisplay;
        [SerializeField] private Button purchaseButton;
        [SerializeField] private TextMeshProUGUI priceDisplay;

        private ProductBase product;

        protected virtual void OnEnable()
        {
            purchaseButton.onClick.AddListener(Purchase);
        }

        protected virtual void OnDisable()
        {
            purchaseButton.onClick.RemoveListener(Purchase);
        }

        private void Purchase()
        {
            if (!CanPurchase())
            {
                return;
            }
            
            IAPManager.Instance.InitializePurchase(product.Id);
        }

        protected virtual bool CanPurchase()
        {
            return true;
        }

        protected virtual void Setup(ProductBase _product)
        {
            product = _product;

            if (nameDisplay != default)
            {
                nameDisplay.text = GetName(_product);
            }

            if (descriptionDisplay != default)
            {
                descriptionDisplay.text = GetDescription(_product);
            }

            if (priceDisplay != default)
            {
                priceDisplay.text = GetPrice(_product);
            }
        }

        protected virtual string GetName(ProductBase _product)
        {
            return _product.Name;
        }

        protected virtual string GetDescription(ProductBase _product)
        {
            return _product.Description;
        }

        protected virtual string GetPrice(ProductBase _product)
        {
            return _product.Price.ToString(CultureInfo.InvariantCulture);
        }

        protected void ManagePurchaseButton(bool _interactable)
        {
            purchaseButton.interactable = _interactable;
        }
    }
}