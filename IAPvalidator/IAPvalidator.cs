using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace MHCSDK
{
    public class IAPvalidator
    {
        public enum Result
        {
            OK = 0,
            ERROR_REQUEST = 1,
            ERROR_VALIDATE = 2
        }

        public enum StoreID
        {
            GooglePlay = 2,
            Amazon = 6,
            AppleAppStore = 10
        }

        public delegate void ValidatorCB(Result res, string productID); // делегат колбека валидации покупок

        #region public class Data - класс с данными для валидации покупки
        /// <summary>
        /// storeID - тип стора.
        /// timeUTC - время покупки(Unix time). Можно использовать конвертор: IAPvalidator.ToUnixTime(DataTime);
        /// productID - (необязательный для  iOS 6 и Amazon) ID продукта(типа 'com.herocraft.game.carrot')
        /// packageName - Название пакета(типа com.herocraft.game.free.gibbets)
        /// appID - ID игры
        /// transactionReceipt - SKPaymentTransaction.transactionReceipt' из StoreKit. только iOS, для остальных платформ - оставить пустым.
        /// token - Токен покупки.Только Android, для остальных платформ - оставить пустым.
        /// userID = ID пользователя.Только Amazon, для остальных платформ - оставить пустым.
        /// cb - Колбек, в который приходит результат проверки.
        /// </summary>
        public class Data
        {
            public Data()
            {
                this.storeID = 0;
                this.timeUTC = "";
                this.productID = "";
                this.packageName = "";
                this.appID = "";
                this.transactionReceipt = "";
                this.token = "";
                this.userID = "";
                this.cb = null;
                this.sandbox = false;
            }

            public Data(
                IAPvalidator.StoreID storeID,
                string timeUTC,
                string productID,
                string packageName,
                string appID,
                string transactionReceipt,
                string token,
                string userID,
                ValidatorCB userCB,
                bool sandbox
                )
            {
                this.storeID = storeID;
                this.timeUTC = timeUTC;
                this.productID = productID;
                this.packageName = packageName;
                this.appID = appID;
                this.transactionReceipt = transactionReceipt;
                this.token = token;
                this.userID = userID;
                this.cb = userCB;
                this.sandbox = sandbox;
            }

            public IAPvalidator.StoreID storeID;
            public string timeUTC;
            public string productID;
            public string packageName;
            public string appID;
            public string transactionReceipt;
            public string token;
            public string userID;
            public ValidatorCB cb;
            public bool sandbox;
        }
        #endregion

        #region public void Validate(Data data) - Проверка покупки
        /// <summary>
        /// Проверка покупки
        /// </summary>
        /// <param name="data">Входные данные</param>
        public static void Validate(Data data)
        {
            //зачем проверять, если результат не узнать
            if (data == null || data.cb == null)
            {
                Debug.LogWarning(Tools.Base64Decode(Consts.LOG_CBIS_NULL));
                return;
            }

            //Поправим формат данных, что бы не возникало ошибок
            if (data.timeUTC == null)               data.timeUTC            = string.Empty;
            if (data.productID == null)             data.productID          = string.Empty;
            if (data.packageName == null)           data.packageName        = string.Empty;
            if (data.appID == null)                 data.appID              = string.Empty;
            if (data.transactionReceipt == null)    data.transactionReceipt = string.Empty;
            if (data.token == null)                 data.token              = string.Empty;
            if (data.userID == null)                data.userID             = string.Empty;

            string dataSTR;
            string store_id;
            try
            {
                dataSTR = MakeData(data);
                store_id = ((int)data.storeID).ToString();

                //sign=MD5(MD5(data) + time + game_id + store_id)
                string signRAW = Tools.MD5Encode(dataSTR) + data.timeUTC + data.appID + store_id;

                if (Core.log_issue)
                {
                    Debug.Log(Tools.Base64Decode(Consts.LOG_URL) + Tools.Base64Decode(Consts.URL_VEREFY));
                    Debug.Log(Tools.Base64Decode(Consts.LOG_DATA) + dataSTR);
                    Debug.Log(Tools.Base64Decode(Consts.LOG_SIGN) + Tools.MD5Encode(signRAW));
                    Debug.Log(Tools.Base64Decode(Consts.LOG_SIGNRAW) + signRAW);
                    Debug.Log(Tools.Base64Decode(Consts.LOG_TIME) + data.timeUTC);
                    Debug.Log(Tools.Base64Decode(Consts.LOG_GAMEID) + data.appID);
                    Debug.Log(Tools.Base64Decode(Consts.LOG_STOREID) + store_id);
                    Debug.Log(Tools.Base64Decode(Consts.LOG_PRODUCTID) + data.productID);
                    if (data.sandbox) Debug.Log(Consts.LOG_SANDBOX);
                }

                // Сделаем строчку с запросом                
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add(Tools.Base64Decode(Consts.FORM_KEY_V), Tools.Base64Decode(Consts.D_2));
                args.Add(Tools.Base64Decode(Consts.FORM_KEY_ACTION), Tools.Base64Decode(Consts.FORM_VALUE_ACTION));
                args.Add(Tools.Base64Decode(Consts.FORM_KEY_DATA), dataSTR);
                args.Add(Tools.Base64Decode(Consts.FORM_KEY_SIGN), Tools.MD5Encode(signRAW));
                args.Add(Tools.Base64Decode(Consts.FORM_KEY_TIME), data.timeUTC);
                args.Add(Tools.Base64Decode(Consts.FORM_KEY_GAMEID), data.appID);
                args.Add(Tools.Base64Decode(Consts.FORM_KEY_STOREID), store_id);
                args.Add(Tools.Base64Decode(Consts.FORM_KEY_PRODUCTID), data.productID);
                //args.Add("mode", "Silvatis");
                if (data.storeID == StoreID.AppleAppStore) args.Add(Tools.Base64Decode(Consts.FORM_KEY_SANDBOX), data.sandbox ? "1" : "0");
                string postData = Network.BuildPostBody(args);

                Network.Post(Tools.Base64Decode(Consts.URL_VEREFY), postData, PostResponse, data);
            }
            catch (Exception e)
            {
                if (Core.log_issue) Debug.Log(Tools.Base64Decode(Consts.LOG_XCEPTION) + e.Message);
                data.cb(Result.ERROR_REQUEST, data.productID);
                return;
            }

        }
        #endregion

        #region private static void PostResponse(string responseCode, byte[] response, object tag) - Обработка ответа от сервера
        /// <summary>
        /// Обработка ответа от сервера
        /// </summary>
        /// <param name="responseCode"></param>
        /// <param name="response"></param>
        /// <param name="tag"></param>
        private static void PostResponse(string responseCode, byte[] responseBytes, object tag)
        {
            Data dataStruct = (Data) tag;
            string response = System.Text.Encoding.UTF8.GetString(responseBytes);

            try
            {
                if (responseCode != "200")
                {
                    Debug.Log(Tools.Base64Decode(Consts.LOG_ERROR) + " PostResponse " + responseCode);
                    dataStruct.cb(Result.ERROR_REQUEST, dataStruct.productID);
                    return;
                }

                // MD5(data + time + game_id + store_id + "true")
                string validateSTR = Tools.MD5Encode(MakeData(dataStruct) + dataStruct.timeUTC + dataStruct.appID + ((int)dataStruct.storeID).ToString() + "true");

                if (Core.log_issue)
                {
                    Debug.Log(Tools.Base64Decode(Consts.LOG_RESPONSE) + response);
                    Debug.Log(Tools.Base64Decode(Consts.LOG_VALIDATESTR) + validateSTR);
                }

                if (validateSTR == response) dataStruct.cb(Result.OK, dataStruct.productID);

                else dataStruct.cb(Result.ERROR_VALIDATE, dataStruct.productID);
                return;
            }
            catch (Exception e)
            {
                if (Core.log_issue) Debug.Log(Tools.Base64Decode(Consts.LOG_XCEPTION) + e.Message);
                dataStruct.cb(Result.ERROR_REQUEST, dataStruct.productID);
                return;
            }
        }
        #endregion

        #region private static string MakeData(Data data) - формирует данные для запроса на сервер
        /// <summary>
        /// формирует данные для запроса на сервер
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string MakeData(Data data)
        {
            switch (data.storeID)
            {
                case StoreID.GooglePlay:
                {
                    //В этом случае смысл параметра 'data' - это зашифрованная(BASE64) строка вида "[packageName],[productId],[token]".Пример: data = BASE64("com.herocraft.game.zombiederby,coins1,abcdefghijklmnopqrstuvwxyz") = "Y29tLmhlcm9jcmFmdC5nYW1lLnpvbWJpZWRlcmJ5LGNvaW5zMSxhYmNkZWZnaGlqa2xtbm9wcXJzdHV2d3h5eg=="
                    string rawSTR =
                        data.packageName + Tools.Base64Decode(Consts.SEPARATOR) +
                        data.productID + Tools.Base64Decode(Consts.SEPARATOR) +
                        data.token;
                    return Tools.Base64Encode(rawSTR);
                }
                case StoreID.AppleAppStore:
                {
                    return data.transactionReceipt;
                }
                case StoreID.Amazon:
                {
                    return Tools.Base64Encode(data.userID + Tools.Base64Decode(Consts.SEPARATOR) + data.transactionReceipt);
                }
                default:
                {
                    return "";
                }
            }
        }
        #endregion
    }
}