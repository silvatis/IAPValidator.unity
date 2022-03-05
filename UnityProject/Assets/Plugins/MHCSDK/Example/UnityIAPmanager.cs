using UnityEngine;
using UnityEngine.Purchasing;

using MHCSDK;

public class UnityIAPmanager : IStoreListener
{
    private static UnityIAPmanager  instance;

    private IStoreController        storeController;
    private IExtensionProvider      storeExtensionProvider;

    public delegate void IAPcb(bool result);    // делегат колбека покупки
    public delegate void InitCB(bool result);   // делегат колбека инициализации

    private InitCB  initCB; // внешний колбек инициализации
    private IAPcb   cb;     // внешний колбек покупки
    private string  appID;  // ID приложения ХК (нужен только для проверки покупок)

    /// <summary>
    /// Инициализация UNITY IAP
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="appID"></param>
    /// <param name="userInitCB"></param>
    public void Init(ConfigurationBuilder builder, string appID, InitCB userInitCB)
    {
        Debug.Log("UnityIAPmanager Init");
        initCB = userInitCB;
        this.appID = appID;
        UnityPurchasing.Initialize(this, builder);
    }

    void successCallback(bool res)
    {
        Debug.Log("RestoreTransactions CB return " + res);
    }

    /// <summary>
    /// Колбек инициализации
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="extensions"></param>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("UnityIAPmanager OnInitialized");
        this.storeController        = controller;
        this.storeExtensionProvider = extensions;

        //при необходимости, можно насильно восстановить покупки
        //this.storeExtensionProvider.GetExtension<IGooglePlayStoreExtensions>().RestoreTransactions(successCallback);

        if (initCB != null) initCB(true);
    }

    /// <summary>
    /// Выполнить покупку. Результат придет в колбек
    /// </summary>
    /// <param name="product_id"></param>
    /// <param name="userCB"></param>
    public void BuyItem(string product_id, IAPcb userCB)
    {
        Debug.Log("BuyItem");

        if (IsInitialized() == false)
        {
            Debug.Log("IStoreListener is not inited");
            return;
        }

        if (userCB == null)
        {
            Debug.Log("IStoreListener callback is null");
            return;
        }

        Product product = storeController.products.WithID(product_id);
        if (product == null) return;
        
        if (product.availableToPurchase)
        {
            cb = userCB;
            storeController.InitiatePurchase(product);
        }        
    }

    /// <summary>
    /// собирает из receipt данные и запускает валидатор
    /// </summary>
    /// <param name="receipt"></param>
    private void PrepareAndValidateData(string receipt)
    {
        IAPvalidator.Data data = new IAPvalidator.Data();

        #if UNITY_ANDROID
        GooglePurchaseData gData = new GooglePurchaseData(receipt);

        //произошла ошибка в распознании ответа
        if (gData.parseResult == false)
        {
            cb(false);
            return;
        }

        data.storeID = IAPvalidator.StoreID.GooglePlay;
        data.timeUTC = gData.json.purchaseTime;
        data.productID = gData.json.productId;
        data.packageName = Application.identifier;
        data.appID = appID;
        data.token = gData.json.purchaseToken;
        data.cb = ValidatorCB;
        //data.userID  for amazon only
        //data.transactionReceipt for iOS only
        //data.sandbox for iOS only
        #endif

        #if UNITY_IOS
        ApplePurchaseData aData = new ApplePurchaseData(receipt);

        CrossPlatformValidator validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
        IPurchaseReceipt[] receptArr = validator.Validate(receipt);

        //в ответе нет продуктов
        if (receptArr.Length < 1)
        {
            cb(false);
            return;
        }

        // Эпл возвращает все покупки. Пройдемся и найдем самый свежий по дате продукт.
        IPurchaseReceipt currentRecept = receptArr[0];
        foreach (IPurchaseReceipt receptINarr in receptArr) if (receptINarr.purchaseDate > currentRecept.purchaseDate) currentRecept = receptINarr;

        data.storeID            = IAPvalidator.StoreID.AppleAppStore;
        data.timeUTC            = IAPvalidator.ToUnixTime(currentRecept.purchaseDate).ToString();
        data.productID          = currentRecept.productID;
        data.packageName        = Application.identifier;
        data.appID              = appID;
        data.cb                 = ValidatorCB;
        data.transactionReceipt = aData.Payload;
        data.sandbox            = true; // Для тестирования платежей выставить true (будет использован сервер валидации sandbox.itunes.apple.com), но для релизной сборки обязательно должно быть false!
        //data.token for Android only
        //data.userID  for amazon only
        #endif

        IAPvalidator.Validate(data);
    }

    /// <summary>
    /// колбек покупки
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        Debug.Log("ProcessPurchase recept = " + e.purchasedProduct.receipt);
        PrepareAndValidateData(e.purchasedProduct.receipt);
        return PurchaseProcessingResult.Pending;
    }

    /// <summary>
    /// колбек валидатора
    /// </summary>
    /// <param name="res"></param>
    /// <param name="productID"></param>
    void ValidatorCB(IAPvalidator.Result res, string productID)
    {
        if (cb == null) return;
        if (productID == null) productID = "";

        Debug.Log("Result = " + res + " Product = " + productID);

        //отобразим в гуи результат
        if (res == IAPvalidator.Result.OK)
        {

            Product product = storeController.products.WithID(productID);
            this.storeController.ConfirmPendingPurchase(product);
            cb(true);
        }
        else cb(false);
    }

    /// <summary>
    /// проверка на инициализацию UNITY IAP
    /// </summary>
    /// <returns></returns>
    private bool IsInitialized()
    {
        return storeController != null && storeExtensionProvider != null;
    }

    /// <summary>
    /// колбек неудачной инициализации UNITY IAP
    /// </summary>
    /// <param name="error"></param>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogWarning("Init UnityIAPmanager error " + error.ToString());
        if (initCB != null) initCB(false);
    }

    /// <summary>
    /// Колбек безуспешной покупки
    /// </summary>
    /// <param name="i"></param>
    /// <param name="p"></param>
    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        Debug.LogWarning("Purchase UnityIAPmanager error. Product = " + i.definition.id + ", error - " + p.ToString());

        //для "зависших" покупок, что куплены но не потрачены.
        if (p == PurchaseFailureReason.DuplicateTransaction)
        {
            Debug.Log("Purchase UnityIAPmanager FinishAdditionalTransaction id = " + i.definition.id + " transactionID = " + i.transactionID);

            Debug.Log("Purchase UnityIAPmanager FinishAdditionalTransaction i.receipt = " + i.receipt);

            IAPvalidator.Data data = new IAPvalidator.Data();

            #if UNITY_ANDROID
            PrepareAndValidateData(i.receipt);
            #endif
        }
        else if (cb != null) cb(false);
    }

    public static UnityIAPmanager Instance()
    {
        if (instance == null) instance = new UnityIAPmanager();
        return instance;
    }
}

#region class GooglePurchaseData - Класс для распознания ответа от стора google
/// <summary>
/// Класс для распознания ответа от стора google
/// </summary>
class GooglePurchaseData
{
    public GoogleJson   json;
    public bool         parseResult;

    [System.Serializable]
    public struct GoogleJson
    {
        public string orderId;
        public string packageName;
        public string productId;
        public string purchaseTime;
        public string purchaseState;
        public string developerPayload;
        public string purchaseToken;
    }

    [System.Serializable]
    private struct GooglePurchaseReceipt
    {
        public string Payload;
    }

    [System.Serializable]
    private struct GooglePurchasePayload
    {
        public string json;
        public string signature;
    }

    public GooglePurchaseData(string receipt)
    {
        try
        {
            Debug.Log("purchaseReceipt = " + receipt);

            GooglePurchaseReceipt purchaseReceipt = JsonUtility.FromJson<GooglePurchaseReceipt>(receipt);
            GooglePurchasePayload purchasePayload = JsonUtility.FromJson<GooglePurchasePayload>(purchaseReceipt.Payload);

            Debug.Log("Payload = " + purchaseReceipt.Payload);
            Debug.Log("Payload = " + purchasePayload.json);

            string payload      = purchaseReceipt.Payload;
            string signature    = purchasePayload.signature;
            json                = JsonUtility.FromJson<GoogleJson>(purchasePayload.json);
            parseResult = true;


        }
        catch
        {
            Debug.Log("Could not parse receipt: " + receipt);
            parseResult = false;
        }
    }
}
#endregion

#region class ApplePurchaseData - Класс для распознания ответа от стора Apple
/// <summary>
/// Класс для распознания ответа от стора Apple
/// </summary>
[System.Serializable]
class ApplePurchaseData
{
    public string Store;
    public string AppleAppStore;
    public string TransactionID;
    public string Payload;

    public ApplePurchaseData(string receipt)
    {
        Debug.Log("purchaseReceipt = " + receipt);

        ApplePurchaseData purchaseReceipt = JsonUtility.FromJson<ApplePurchaseData>(receipt);

        this.Store          = purchaseReceipt.Store;
        this.AppleAppStore  = purchaseReceipt.AppleAppStore;
        this.TransactionID  = purchaseReceipt.TransactionID;
        this.Payload        = purchaseReceipt.Payload;
    }
}
#endregion