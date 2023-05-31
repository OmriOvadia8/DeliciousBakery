using System;
using System.Collections.Generic;
using UnityEngine.Purchasing;

namespace DB_Core
{
    public class DBInAppPurchase : IStoreListener
    {
        private IStoreController storeController;
        private IExtensionProvider extensionProvider;
        private bool isInitialized;
        private Action<bool> purchaseCompleteAction;

        public DBInAppPurchase()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            builder.AddProduct("com.omri.deliciousbakery.stars.big", ProductType.Consumable);
            builder.AddProduct("com.omri.deliciousbakery.stars.small", ProductType.Consumable);
            builder.AddProduct("com.omri.deliciousbakery.stars.medium", ProductType.Consumable);

            UnityPurchasing.Initialize(this, builder);

            DBDebug.Log("DBInAppPurchase initialized.");
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            storeController = controller;
            extensionProvider = extensions;
            isInitialized = true;
            DBDebug.Log("OnInitialized called in DBInAppPurchase.");
        }

        public void Purchase(string productID, Action<bool> onPurchaseComplete)
        {
            if (!isInitialized)
            {
                DBDebug.Log("Attempted to purchase before initialization completed.");
                return;
            }
            purchaseCompleteAction = onPurchaseComplete;
            storeController.InitiatePurchase(productID);
        }

        public void OnInitializeFailed(InitializationFailureReason error) =>
            DBManager.Instance.CrashManager.LogExceptionHandling(error.ToString());

        public void OnInitializeFailed(InitializationFailureReason error, string message) =>
            DBManager.Instance.CrashManager.LogExceptionHandling(error.ToString() + "   " + message);

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            var keys = new Dictionary<DBDataKeys, object>
            {
                {DBDataKeys.product_id, purchaseEvent.purchasedProduct.definition.id},
                {DBDataKeys.product_receipt, purchaseEvent.purchasedProduct.receipt}
            };

            DBManager.Instance.AnalyticsManager.ReportEvent(DBEventType.purchase_complete, keys);

            purchaseCompleteAction?.Invoke(true);
            purchaseCompleteAction = null;

            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            if (!string.IsNullOrEmpty(product.receipt))
            {
                DBManager.Instance.CrashManager.LogBreadcrumb(product.receipt);
            }

            string failureMessage = failureReason.ToString();
            if (!string.IsNullOrEmpty(failureMessage))
            {
                DBManager.Instance.CrashManager.LogExceptionHandling(failureMessage);
            }

            purchaseCompleteAction?.Invoke(false);
            purchaseCompleteAction = null;
        }
    }
}