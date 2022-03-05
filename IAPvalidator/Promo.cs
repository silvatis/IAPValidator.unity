using System;
using System.Xml;
using System.Collections.Generic;

using UnityEngine;

namespace MHCSDK
{
    public class Promo
    {
        /// <summary>
        /// Колбек для запроса
        /// </summary>
        /// <param name="result"></param>
        public delegate void PromoCB(List<Item> result);

        #region public class Data - клас с исходными данными для запроса
        /// <summary>
        /// клас с исходными данными для запроса
        /// </summary>
        public class Data
        {
            public string port = "";
            public string gameVer = "";
            public string gameID = "";
            public string product = "";
            public string provider = "";
            public string lang = "";
            public string deviceID = "";
            public string userID = "";
            public PromoCB userCB;
        }
        #endregion

        #region public class Item - класс с результатами запроса
        /// <summary>
        /// класс с результатами запроса
        /// </summary>
        public class Item
        {
            public string id = "";
            public string text = "";
            public List<Resource> resources = new List<Resource>();
        }
        #endregion

        #region public class Resource - класс с веб ресурсами для акций
        /// <summary>
        /// класс с веб ресурсами для акций
        /// </summary>
        public class Resource
        {
            public string url = "";
            public string resID = "";
            public string cache = "";
        }
        #endregion

        #region public static void Get(Data data) - получение данных о акциях
        /// <summary>
        /// получение данных о акциях
        /// </summary>
        /// <param name="data"></param>
        public static void Get(Data data)
        {
            if (data.userCB == null)
            {
                if (Core.log_issue) Debug.LogWarning(Tools.Base64Decode(Consts.LOG_CBIS_NULL));
                return;
            }

            try
            {
                Dictionary<string, string> paramsSet = new Dictionary<string, string>();
                paramsSet.Add(Tools.Base64Decode(Consts.FORM_KEY_PORT), data.port);
                paramsSet.Add(Tools.Base64Decode(Consts.FORM_KEY_GAMEV), data.gameVer);
                paramsSet.Add(Tools.Base64Decode(Consts.FORM_KEY_PROD), data.product);
                paramsSet.Add(Tools.Base64Decode(Consts.FORM_KEY_PROV), data.provider);
                paramsSet.Add(Tools.Base64Decode(Consts.FORM_KEY_GAMEID), data.gameID);
                paramsSet.Add(Tools.Base64Decode(Consts.FORM_KEY_LANG), data.lang);
                paramsSet.Add(Tools.Base64Decode(Consts.FORM_KEY_APIV), Tools.Base64Decode(Consts.D_1));
                paramsSet.Add(Tools.Base64Decode(Consts.FORM_KEY_DEVICEID), data.deviceID);
                paramsSet.Add(Tools.Base64Decode(Consts.FORM_KEY_USERID), data.userID);
                string request = Network.BuildPostBody(paramsSet);

                string url = Tools.Base64Decode(Consts.URL_PROMO) + Tools.Base64Decode(Consts.Q_MARK) + request;
                Network.Post(url, "", PostResponse, data.userCB);
            }
            catch (Exception e)
            {
                if (Core.log_issue) Debug.LogWarning(Tools.Base64Decode(Consts.LOG_XCEPTION) + e.Message);
            }
        }
        #endregion

        #region private static void PostResponse(string responseCode, byte[] response, object tag) - Колбек запроса на сервер
        /// <summary>
        /// Колбек запроса на сервер
        /// </summary>
        /// <param name="responseCode"></param>
        /// <param name="response"></param>
        /// <param name="tag"></param>
        private static void PostResponse(string responseCode, byte[] response, object tag)
        {
            PromoCB cb = (PromoCB)tag;
            List<Item> result = new List<Item>();
            try
            {
                string responseSTR = System.Text.Encoding.UTF8.GetString(response);
                result = Parse(responseSTR);
            }
            catch (Exception e)
            {
                if (Core.log_issue) Debug.LogWarning(Tools.Base64Decode(Consts.LOG_XCEPTION) + e.Message);
            }
            cb(result);
        }
        #endregion

        #region private static List<Item> Parse(string xml) - Парсер XML
        /// <summary>
        /// Парсер XML
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private static List<Item> Parse(string xml)
        {
            List<Item> result = new List<Item>();

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xml);

            XmlElement xRoot = xDoc.DocumentElement;

            foreach (XmlNode xnode in xRoot)
            {
                Item item = new Item();

                XmlAttribute attr = xnode.Attributes["id"];
                if (attr != null) item.id = attr.Value;

                item.text = xnode.InnerText;

                XmlNode res = xnode["resources"];

                foreach (XmlNode child in res.ChildNodes)
                {
                    Resource resor = new Resource();

                    XmlAttribute data = child.Attributes["url"];
                    if (data != null) resor.url = data.Value;

                    data = child.Attributes["res-id"];
                    if (data != null) resor.resID = data.Value;

                    data = child.Attributes["cache"];
                    if (data != null) resor.cache = data.Value;

                    item.resources.Add(resor);
                }

                result.Add(item);
            }

            return result;
        }
        #endregion
    }
}
