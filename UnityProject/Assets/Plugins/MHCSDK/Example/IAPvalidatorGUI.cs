using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

using UnityEngine.Networking;

public class IAPvalidatorGUI : MonoBehaviour
{
    public UnityEngine.UI.Text          statusText;
    public UnityEngine.UI.Button        initButton;
    public UnityEngine.UI.Button        purchaseButton;
    public UnityEngine.UI.Button        propertiesButton;
    public UnityEngine.UI.Toggle        consumableToggle;
    public UnityEngine.UI.Toggle        debugToggle;
    public UnityEngine.UI.InputField    gameIDinput;
    public UnityEngine.UI.InputField    productIDinput;
    public UnityEngine.UI.InputField    storeIDinput;

    const string DEF_GAME_ID    = "420";
    const string DEF_STORE_ID   = "629";
    const string DEF_PRODUCT_ID = "com.herocraft.game.free.gibbets3.stars2";

	// Use this for initialization
	void Start ()
    {
        statusText.text     = "Awaiting initialize";
        gameIDinput.text    = DEF_GAME_ID;
        productIDinput.text = DEF_PRODUCT_ID;

        purchaseButton.interactable = false;
    }

    public void InitIAP()
    {
        //отключить доступность элементов
        gameIDinput.interactable        = false;
        productIDinput.interactable     = false;
        consumableToggle.interactable   = false;
        debugToggle.interactable        = false;
        initButton.interactable         = false;

        statusText.text = "Initialize Unity IAP";

        //инициировать UnityIAP
        ProductType type = ProductType.Subscription;
        if (consumableToggle.isOn ) type = ProductType.Consumable;

        MHCSDK.Core.EnableDebug(debugToggle.isOn);      

        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(productIDinput.text, type);
        UnityIAPmanager.Instance().Init(builder, gameIDinput.text, InitIAPcb);
    }

    public void InitIAPcb(bool result)
    {
        if (result == false) statusText.text = "Initialize Unity IAP error";
       
        purchaseButton.interactable = true;
        statusText.text = "Awaiting purchase";
    }

    public void Purchase()
    {
        purchaseButton.interactable = false;

        statusText.text = "Purchasing...";
        UnityIAPmanager.Instance().BuyItem(productIDinput.text, PurchaseCB);
    }

    public void GetServerProperties()
    {
        propertiesButton.interactable = false;

        MHCSDK.Core.EnableDebug(debugToggle.isOn);
        MHCSDK.ServerProperties.Get(gameIDinput.text, storeIDinput.text, ServerPropertiesCB);
    }

    void PurchaseCB(bool purchaseResult)
    {
        if (purchaseResult) statusText.text = "Validate Result ok";
        else statusText.text = "Validate failed";

        purchaseButton.interactable = true;
    }

    /// <summary>
    /// Callback, вызываемый после запроса свойств с сервера
    /// </summary>
    /// <param name="properties"> свойства, null при наличии проблем запроса с сервера</param>
    /// <param name="hasError">наличие ошибки при запросе</param>
    public void ServerPropertiesCB(Dictionary<string, string> properties, bool hasError)
    {
        propertiesButton.interactable = true;

        if (hasError == true)
        {
            statusText.text = "Get Properties failed";
            return;
        }
        statusText.text = "Get Properties ok";

        foreach (KeyValuePair<string,string> property in properties) Debug.Log("key = <" + property.Key + "> value = <" + property.Value + ">");
    }

    public void GetProperties()
    {
        MHCSDK.Properties.Init(245);
        Debug.Log(MHCSDK.Properties.Get("PNAME"));
    }

    public void GetPromo()
    {
        MHCSDK.Promo.Data data = new MHCSDK.Promo.Data();

        data.port = "46922";
        data.gameVer = "3.3.2";
        data.provider = "629";
        data.userCB = PromoCB;

        MHCSDK.Promo.Get(data);
    }

    public void PromoCB(List<MHCSDK.Promo.Item> result)
    {
        foreach (MHCSDK.Promo.Item item in result)
        {
            Debug.Log("id = " + item.id + " text = " + item.text);

            foreach (MHCSDK.Promo.Resource res in item.resources)
            {
                Debug.Log("resid = " + res.resID + " url = " + res.url + " cache = " + res.cache);
            }
        }
    }

    /// <summary>
    /// ПРимер отправки аналитики
    /// </summary>
    public void Analytics()
    {
        MHCSDK.Analytics.Settings settings = new MHCSDK.Analytics.Settings();

        settings.gameID = "325";
        settings.gameType = MHCSDK.Analytics.GameType.free;
        settings.providerID = "629";
        settings.portID = "46922";
        settings.deviceID = "";
        settings.channel = "";
        settings.phoneSysID = SystemInfo.deviceModel;
        settings.version = Application.identifier;
        settings.lang = "ru";
        settings.referrer = "";

        MHCSDK.Analytics.Init(settings);

        MHCSDK.Analytics.Event eventHC = new MHCSDK.Analytics.Event();
        eventHC.type = MHCSDK.Analytics.EventType.app_start;
        eventHC.customEventName = "";

        MHCSDK.Analytics.Track(eventHC);
    }

    /// <summary>
    /// Пример чтения KDSCH
    /// </summary>
    public void ReadKDSCH()
    {
        MHCSDK.Properties.Init(138);
        string gameID = MHCSDK.Properties.Get("GAME_ID");
    }

}
