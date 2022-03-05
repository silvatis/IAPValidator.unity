using System;
using System.Collections.Generic;

using UnityEngine;

namespace MHCSDK
{
    /// <summary>
    /// работа с серверными свойствами
    /// </summary>
    public class ServerProperties
    {
        /// <summary>
        /// делегат колбека получения серверных данных
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="hasError"></param>
        public delegate void ServerPropertiesCB(Dictionary<string, string> properties, bool hasError);

        #region public static void Get(string gameID, string provID, ServerPropertiesCB cb) - получить серверные свойства
        /// <summary>
        /// получить серверные свойства
        /// </summary>
        /// <param name="gameID"></param>
        /// <param name="provID"></param>
        /// <param name="cb"></param>
        public static void Get(string gameID, string provID, ServerPropertiesCB cb)
        {
            try
            {
                Dictionary<string, string> paramtrs = new Dictionary<string, string>();
                paramtrs.Add(Tools.Base64Decode(Consts.FORM_KEY_GAMEID), gameID);
                paramtrs.Add(Tools.Base64Decode(Consts.FORM_KEY_PROVID), provID);
                string postData = Network.BuildPostBody(paramtrs);

                string url = Tools.Base64Decode(Consts.URL_PROPERTIES) + Tools.Base64Decode(Consts.Q_MARK) + postData;

                Network.Post(url, "", PostResponse, cb);
            }
            catch (Exception e)
            {
                if (Core.log_issue) Debug.Log(Tools.Base64Decode(Consts.LOG_XCEPTION) + e.Message);
            }

        }
        #endregion

        #region private static void PostResponse(string responseCode, byte[] ravResult, object tag) - обработчик ответа от сервера
        /// <summary>
        /// обработчик ответа от сервера
        /// </summary>
        /// <param name="responseCode"></param>
        /// <param name="ravResult"></param>
        /// <param name="tag"></param>
        private static void PostResponse(string responseCode, byte[] ravResult, object tag)
        {
            ServerPropertiesCB cb = (ServerPropertiesCB)tag;

            try
            {
                if (responseCode != "200")
                {
                    if (Core.log_issue) Debug.Log(Tools.Base64Decode(Consts.LOG_ERROR) + responseCode);
                    cb(null, true);
                    return;
                }

                Dictionary<string, string> result = new Dictionary<string, string>();

                int i = 0;
                while (i < ravResult.Length)
                {
                    //2 байта - размер строки
                    short length = (short)((ravResult[i] << 8) | ravResult[i + 1]);
                    i += 2;

                    // вычленим строку
                    byte[] data = new byte[length];
                    System.Array.Copy(ravResult, i, data, 0, length);

                    //вытащим строку длинной length
                    string[] resArr = System.Text.Encoding.UTF8.GetString(data).Trim().Split(new string[] { ": " }, System.StringSplitOptions.RemoveEmptyEntries);
                    if (resArr.Length == 2)
                    {
                        if (Core.log_issue) Debug.Log("key = <" + resArr[0] + "> value = <" + resArr[1] + ">");
                        result.Add(resArr[0], resArr[1]);
                    }
                    else Debug.LogWarning("Failed to parse string:" + System.Text.Encoding.UTF8.GetString(data));

                    i += length;
                }

                cb(result, false);
            }
            catch (Exception e)
            {
                if (Core.log_issue) Debug.Log(Tools.Base64Decode(Consts.LOG_XCEPTION) + e.Message);
                cb(null, true);
            }
        }
        #endregion
    }
}
