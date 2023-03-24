using System;
using System.Collections.Generic;
using UnityEngine.Purchasing;

namespace Core
{
    public class HOGInAppPurchase : IStoreListener
    {
        private IStoreController storeController;
        private IExtensionProvider extensionProvider;

        private Action<bool> purchaseCompleteAction;

        public HOGInAppPurchase()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            UnityPurchasing.Initialize(this, builder);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            storeController = controller;
            extensionProvider = extensions;
        }

        public void Purchase(string productID, Action<bool> onPurchaseComplete)
        {
            purchaseCompleteAction = onPurchaseComplete;
            storeController.InitiatePurchase(productID);
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            HOGManager.Instance.CrashManager.LogExceptionHandling(error.ToString());
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            HOGManager.Instance.CrashManager.LogExceptionHandling(error.ToString() + "   " + message);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            var keys = new Dictionary<HOGDataKeys, object>
            {
                {HOGDataKeys.product_id, purchaseEvent.purchasedProduct.definition.id},
                {HOGDataKeys.product_receipt, purchaseEvent.purchasedProduct.receipt}
            };

            HOGManager.Instance.AnalyticsManager.ReportEvent(HOGEventType.purchase_complete, keys);

            purchaseCompleteAction?.Invoke(true);
            purchaseCompleteAction = null;

            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            HOGManager.Instance.CrashManager.LogBreadcrumb(product.receipt);
            HOGManager.Instance.CrashManager.LogExceptionHandling(failureReason.ToString());

            purchaseCompleteAction?.Invoke(false);
            purchaseCompleteAction = null;
        }
    }
}