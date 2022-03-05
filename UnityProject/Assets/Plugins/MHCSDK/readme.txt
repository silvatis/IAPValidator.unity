IAPValidator
------------

IAPValidator позволяет валидировать через сервер HeroCraft покупки, совершенные в Google Play и AppStore.

IAPValidator не умеет совершать покупки в указанных магазинах приложений, сами покупки должны быть реализованы
либо стандартными средствами Unity, либо какой-то сторонней библиотекой. Единственным условием является возможность
получить от библиотеки квитанцию о покупке.

Пример кода для проверки покупки, совершенной в Google Play
-----------------------------------------------------------

Пример основан на использовании стандартной платежной библиотеки Unity

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        Debug.Log("ProcessPurchase recept = " + e.purchasedProduct.receipt);      
        GooglePurchaseData gData = new GooglePurchaseData(e.purchasedProduct.receipt);

        IAPvalidator.Data data = new IAPvalidator.Data();

        // данные о покупке
        data.timeUTC        = gData.json.purchaseTime;
        data.productID      = gData.json.productId;
        data.token          = gData.json.purchaseToken;

        // магазин приложений
        data.storeID        = IAPvalidator.StoreID.GooglePlay;

        // имя пакета
        data.packageName    = Application.identifier;
		
        // внутренний ID игры в базе проектов HeroCraft
        data.appID          = appID;
		
        // callback, который будет вызван по завершении прверки
        data.cb             = ValidatorCB;

        // вызываем проверку
        IAPvalidator.Instance().Validate(data);

        return PurchaseProcessingResult.Complete;
    }

Пример кода для проверки покупки, совершенной в App Store
-----------------------------------------------------------

Пример основан на использовании стандартной платежной библиотеки Unity

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        Debug.Log("ProcessPurchase recept = " + e.purchasedProduct.receipt);      
        ApplePurchaseData aData = new ApplePurchaseData(e.purchasedProduct.receipt);

		// App Store возвращает все покупки, из них надо взять самый свежий
		CrossPlatformValidator validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
        IPurchaseReceipt[] receptArr = validator.Validate(e.purchasedProduct.receipt);
        IPurchaseReceipt currentRecept = receptArr[0];
        foreach (IPurchaseReceipt receptINarr in receptArr) if (receptINarr.purchaseDate > currentRecept.purchaseDate) currentRecept = receptINarr;
		
        IAPvalidator.Data data = new IAPvalidator.Data();

		// для тестирования платежей выставить true (будет использован сервер валидации sandbox.itunes.apple.com). Для релизной сборки обязательно должно быть false (значение по умолчанию)
		data.sandbox            = true; 
		
        // данные о покупке
        data.timeUTC            = IAPvalidator.ToUnixTime(currentRecept.purchaseDate).ToString();
        data.productID          = currentRecept.productID;
        data.transactionReceipt = aData.Payload;

        // магазин приложений
        data.storeID        	= IAPvalidator.StoreID.AppleAppStore;

        // имя пакета
        data.packageName    	= Application.identifier;
		
        // внутренний ID игры в базе проектов HeroCraft
        data.appID          	= appID;
				
        // callback, который будет вызван по завершении прверки
        data.cb             	= ValidatorCB;

        // вызываем проверку
        IAPvalidator.Instance().Validate(data);

        return PurchaseProcessingResult.Complete;
    }
	

Пример кода Callback для получения результата проверки.
-----------------------------------------------------------
	
    /// <summary>
    /// Callback, вызываемый после проверки покупки
    /// </summary>
    /// <param name="res">результат проверки</param>
    /// <param name="productID">id продукта, для которого проводилась проверка</param>
    void ValidatorCB(IAPvalidator.Result res, string productID)
    {
        switch (res)
        {
            case IAPvalidator.Result.OK: // проверка прошла успешно
                break;

            case IAPvalidator.Result.ERROR_REQUEST: // проверить не удалось
                break;

            case IAPvalidator.Result.ERROR_VALIDATE: // проверка не прошла
                break;
        }
    }


Пример кода для получения свойств с сервера
-----------------------------------------------------------
    public void GetServerProperties()
    {
        propertiesButton.interactable = false;

        IAPvalidator.log = debugToggle.isOn;
        IAPvalidator.Instance().GetServerProperties("420", "629", ServerPropertiesCB);
    }
	
Пример кода Callback для получения результата проверки.
-----------------------------------------------------------	
    /// <summary>
    /// Callback, вызываемый после запроса свойств с сервера
    /// </summary>
    /// <param name="properties"> свойства, null при наличии проблем запроса с сервера</param>
    /// <param name="hasError">наличие ошибки при запросе</param>
	    public void ServerPropertiesCB(Dictionary<string, string> properties, bool hasError)
    {
        if (hasError == true)
        {
			Debug.Log("error!");
            return;
        }

        foreach (KeyValuePair<string,string> property in properties) Debug.Log("key = <" + property.Key + "> value = <" + property.Value + ">");
    }