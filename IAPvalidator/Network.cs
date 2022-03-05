using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace MHCSDK
{
    class Network : MonoBehaviour
    {
        /// <summary>
        /// делегат колбека обработки POST запросов
        /// </summary>
        /// <param name="responseCode"></param>
        /// <param name="response"></param>
        /// <param name="tag"></param>
        public delegate void PostResponse(string responseCode, byte[] response, object tag); // делегат колбека валидации покупок

        #region private static Network Instance() - получение синглтона
        /// <summary>
        /// получение синглтона
        /// </summary>
        /// <returns></returns>
        private static Network Instance()
        {
            Network netw = Core.Instance().GetComponent<Network>();
            if (netw == null) netw = Core.Instance().AddComponent<Network>();

            return netw;
        }
        #endregion

        #region public static void Post(string url, string body, PostResponse cb, object tag = null) - Выполняет POST запрос
        /// <summary>
        /// Выполняет POST запрос
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="cb"></param>
        /// <param name="tag"></param>
        public static void Post(string url, string body, PostResponse cb, object tag = null)
        {
            byte[] bodyRaw = new byte[0];
            if (string.IsNullOrEmpty(body) == false) bodyRaw = Encoding.UTF8.GetBytes(body);
            Post(url, bodyRaw, cb, tag);
        }
        #endregion

        #region public static void Post(string url, byte[] body, PostResponse cb, object tag = null) - Выполняет POST запрос
        /// <summary>
        /// Выполняет POST запрос
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="cb"></param>
        /// <param name="tag"></param>
        public static void Post(string url, byte[] body, PostResponse cb, object tag = null)
        {
            Instance().StartCoroutine(Instance().PostWorker(url, body, cb, tag));
        }
        #endregion

        #region public static string BuildPostBody(Dictionary<string,string> parametrs) - Строит строку запроса из набора параметров
        /// <summary>
        /// Строит строку запроса из набора параметров
        /// </summary>
        /// <param name="parametrs"></param>
        /// <returns></returns>
        public static string BuildPostBody(Dictionary<string,string> parametrs)
        {
            string result = "";
            foreach (KeyValuePair<string, string> param in parametrs)
            {
                if (String.IsNullOrEmpty(param.Key) == false && String.IsNullOrEmpty(param.Value) == false)
                result = result + param.Key + "=" + Uri.EscapeDataString(param.Value) + "&";
            }
            result = result.Remove(result.Length - 1);

            if (Core.log_issue) Debug.Log(Tools.Base64Decode(Consts.LOG_QUERY) + result);

            return result;
        }
        #endregion

        #region private IEnumerator PostWorker(string url, byte[] body, PostResponse cb, object tag) - Асинхронный обработчик post запросов
        /// <summary>
        /// Асинхронный обработчик post запросов
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="cb"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        private IEnumerator PostWorker(string url, byte[] body, PostResponse cb, object tag)
        {
            UnityWebRequest www;

            try
            {
                //строка для пост запроса
                string requestURL = "";

                //на "младших" версиях дроида хттпс не работает
                if ((Application.platform == RuntimePlatform.Android) && (Tools.GetSDKLevel() <= 20)) requestURL = "http://";
                else requestURL = "https://";

                //допишем адрес
                requestURL = requestURL + url;

                if (Core.log_issue)
                {
                    Debug.Log(Tools.Base64Decode(Consts.LOG_URL) + requestURL);
                    Debug.Log(Tools.Base64Decode(Consts.LOG_DATA) + System.Text.Encoding.UTF8.GetString(body));
                }

                www = new UnityWebRequest(url, "POST");
                if (body.Length > 0) www.uploadHandler = new UploadHandlerRaw(body);
                www.downloadHandler = new DownloadHandlerBuffer();
            }
            catch (Exception e)
            {
                if (Core.log_issue) Debug.Log(Tools.Base64Decode(Consts.LOG_XCEPTION) + e.Message);

                cb(e.Message,null, tag);
                yield break;
            }

            #pragma warning disable CS0618  //устаревший метод используется осознанно для совместимости со старыми версиями UNITY
            yield return www.Send();
            #pragma warning restore CS0618

            if (Core.log_issue)
            {
                Debug.Log(Tools.Base64Decode(Consts.LOG_RESPONSE) + www.downloadHandler.text);
                Debug.Log(Tools.Base64Decode(Consts.LOG_RESPONSE_CODE) + www.responseCode);
            }

            cb(www.responseCode.ToString(), www.downloadHandler.data, tag);
        }
        #endregion

    }
}
