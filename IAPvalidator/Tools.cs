using System;
using System.Collections.Generic;
using System.Security.Cryptography;

using UnityEngine;

namespace MHCSDK
{
    /// <summary>
    /// Набор различных вспомогательных методов
    /// </summary>
    public class Tools
    {
        #region public static long DateTimeToUnix(DateTime date) - Конвертировать DateTime в UnixTime
        /// <summary>
        /// Конвертировать DateTime в UnixTime
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static long DateTimeToUnix(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }
        #endregion

        #region public static string MD5Encode(string text) - хешировать MD5
        /// <summary>
        /// хешировать MD5
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string MD5Encode(string text)
        {
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(text);
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(inputBytes);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < hash.Length; i++) sb.Append(hash[i].ToString(Base64Decode(Consts.MD5_ENCODE_FORMAT)));
            return sb.ToString();
        }
        #endregion

        #region public static string Base64Encode(string text) - шифрование Base64Encode
        /// <summary>
        /// шифрование Base64Encode
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Base64Encode(string text)
        {
            byte[] plainTextBytes = System.Text.Encoding.Default.GetBytes(text);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        #endregion

        #region public static string Base64Decode(string base64text) - расшифровка Base64
        /// <summary>
        /// расшифровка Base64
        /// </summary>
        /// <param name="base64text"></param>
        /// <returns></returns>
        public static string Base64Decode(string base64text)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64text);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        #endregion

        #region public static int GetSDKLevel() - возвращает версию API андроид
        /// <summary>
        /// возвращает версию API андроид
        /// </summary>
        /// <returns></returns>
        public static int GetSDKLevel()
        {
            
            //android.os.Build$VERSION нельзя, так как люди в смысле особые могут запихнуть вызов валидатора туда, куда его совать не стоит 
            string versionInfo = "API-15";

            string[] os = SystemInfo.operatingSystem.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string api in os)
            {
                if (api.Contains("API-"))
                {
                    versionInfo = api;
                    break;
                }
            }

            string[] versionInfoSplit = versionInfo.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            int version = System.Convert.ToInt32(versionInfoSplit[versionInfoSplit.Length - 1]);

            return version;
        }
        #endregion

        public static void WebRequest(Dictionary<string,string> postParams, string url)
        {
            //строка для пост запроса
            string requestURL = "";

            //на "младших" версиях дроида хттпс не работает
            if ((Application.platform == RuntimePlatform.Android) && (Tools.GetSDKLevel() <= 20)) requestURL = "http://";
            else requestURL = "https://";

            //допишем адрес и параметры, удалив лишнюю & в конце:
            requestURL = requestURL + url;
            foreach (KeyValuePair<string, string> postParam in postParams) requestURL = requestURL + postParam.Key + "=" + postParam.Value + "&";
            requestURL = requestURL.Remove(requestURL.Length - 1);
        }
    }
}
