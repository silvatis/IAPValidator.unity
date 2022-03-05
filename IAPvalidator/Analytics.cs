using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



using UnityEngine;

namespace MHCSDK
{
    public class Analytics : MonoBehaviour
    {
        private static string   jsonHeader = null;
        private static int      dataCount = 0;

        public enum GameType
        {
            not_set,
            full,
            lite,
            free
        }

        public enum EventType
        {
            not_set = -1,
            custom = 0,
            login_new = 1,
            login = 2,
            login_failed = 3,
            more_games = 4,
            ext_net = 5,
            discussion = 6,
            banner_show = 7,
            banner_press = 8,
            news_show = 9,
            news_details = 10,
            news_jump = 11,
            send_scores = 12,
            send_progress = 13,
            send_ach = 14,
            iap = 15,
            iap_ok = 16,
            share = 17,
            email = 18,
            app_start = 19,
            app_stop = 20,
            rate_rate = 21,
            rate_later = 22,
            rate_never = 23,
            pcode_activate = 24,
            cache_dld_begin = 25,
            cache_dld_end = 26,
            cache_dld_error = 27,
            offerwall_get_offers = 28,
            offerwall_get_bonuses = 29,
            offerwall_show_ui = 30,
            offerwall_hide_ui = 31,
            offerwall_click_ui = 32,
            debug = 33
        }

        public class Settings
        {
            public string   gameID      = "";
            public GameType gameType    = GameType.not_set;
            public string   providerID  = "";
            public string   portID      = "";
            public string   deviceID    = "";
            public string   channel     = "";
            public string   phoneSysID  = "";
            public string   lang        = "";
            public string   version     = "";
            public string   referrer    = "";
        }

        public class Slot
        {
            public string name  = "";
            public string value = "";
        }

        public class Event
        {
            public EventType type           = EventType.not_set;
            public string customEventName   = "";

            public List<Slot> slots = new List<Slot>();
        }

        #region public static void Init(Settings settings) - Инициализация аналитики
        /// <summary>
        /// Инициализация аналитики
        /// </summary>
        /// <param name="settings"></param>
        public static void Init(Settings settings)
        {
            // подготовим заголовок
            jsonHeader = "";
            string gameType;
            if (settings.gameType == GameType.not_set) gameType = "";
            else gameType = settings.gameType.ToString();

            jsonHeader = jsonHeader + Tools.Base64Decode(Consts.C_BRACE_ON);
            jsonHeader = jsonHeader + MakeJsonPair(Tools.Base64Decode(Consts.FORM_KEY_GAMEID), settings.gameID) + Tools.Base64Decode(Consts.SEPARATOR);
            jsonHeader = jsonHeader + MakeJsonPair(Tools.Base64Decode(Consts.FORM_KEY_GAME_TYPE), gameType) + Tools.Base64Decode(Consts.SEPARATOR);
            jsonHeader = jsonHeader + MakeJsonPair(Tools.Base64Decode(Consts.FORM_KEY_PROVID), settings.providerID) + Tools.Base64Decode(Consts.SEPARATOR);
            jsonHeader = jsonHeader + MakeJsonPair(Tools.Base64Decode(Consts.FORM_KEY_PROD_ID), settings.portID) + Tools.Base64Decode(Consts.SEPARATOR);
            jsonHeader = jsonHeader + MakeJsonPair(Tools.Base64Decode(Consts.FORM_KEY_USERID), "") + Tools.Base64Decode(Consts.SEPARATOR);
            jsonHeader = jsonHeader + MakeJsonPair(Tools.Base64Decode(Consts.FORM_KEY_DEVICEID), settings.deviceID) + Tools.Base64Decode(Consts.SEPARATOR);
            jsonHeader = jsonHeader + MakeJsonPair(Tools.Base64Decode(Consts.FORM_KEY_CHANNEL), settings.channel) + Tools.Base64Decode(Consts.SEPARATOR);
            jsonHeader = jsonHeader + MakeJsonPair(Tools.Base64Decode(Consts.FORM_KEY_PHONESYSID), settings.phoneSysID) + Tools.Base64Decode(Consts.SEPARATOR);
            jsonHeader = jsonHeader + MakeJsonPair(Tools.Base64Decode(Consts.FORM_KEY_VERSION), settings.version) + Tools.Base64Decode(Consts.SEPARATOR);
            jsonHeader = jsonHeader + MakeJsonPair(Tools.Base64Decode(Consts.FORM_KEY_HCSDKVER), Consts.VERSION) + Tools.Base64Decode(Consts.SEPARATOR);
            jsonHeader = jsonHeader + MakeJsonPair(Tools.Base64Decode(Consts.FORM_KEY_LANG), settings.lang) + Tools.Base64Decode(Consts.SEPARATOR);
            jsonHeader = jsonHeader + MakeJsonPair(Tools.Base64Decode(Consts.FORM_KEY_REFERRER), settings.referrer);
            jsonHeader = jsonHeader + Tools.Base64Decode(Consts.C_BRACE_OFF);

            dataCount = PlayerPrefs.GetInt(Tools.Base64Decode(Consts.MHCSDK_PREFS_COUNT), 0);
        }
        #endregion

        #region public static void Track(Event eventData) - регестрация события аналитики
        /// <summary>
        /// регестрация события аналитики
        /// </summary>
        /// <param name="eventData"></param>
        public static void Track(Event eventData)
        {
            //если кастомный тип события - берем его название со строчки
            string type = "";
            if (eventData.type == EventType.custom) type = eventData.customEventName;
            else type = eventData.type.ToString();

            // сформируем часть запроса: время и тип события
            string timeFormat   = Tools.Base64Decode(Consts.ANAL_TIME_FORMAT);
            string trackData    = Tools.Base64Decode(Consts.C_BRACE_ON);

            trackData = trackData + MakeJsonPair(Tools.Base64Decode(Consts.FORM_KEY_TIME), System.DateTime.Now.ToString(timeFormat)) + Tools.Base64Decode(Consts.WS_SEPARATOR);
            trackData = trackData + MakeJsonPair(Tools.Base64Decode(Consts.FORM_KEY_TYPE), type) + Tools.Base64Decode(Consts.WS_SEPARATOR);

            //сформируем "слоты" события
            int slotCount = 1;
            foreach (Slot slot in eventData.slots)
            {
                string key = slot.name;
                if (String.IsNullOrEmpty(key))
                {
                    key = Tools.Base64Decode(Consts.FORM_KEY_SLOT) + slotCount;
                    slotCount++;
                }
                trackData = trackData + MakeJsonPair(key, slot.value) + Tools.Base64Decode(Consts.WS_SEPARATOR);
            }

            //удалим лишнюю запятую и завершим тег xmlки
            trackData = trackData.Remove(trackData.Length - 2);
            trackData = trackData + Tools.Base64Decode(Consts.C_BRACE_OFF);

            if (Core.log_issue) Debug.Log(Tools.Base64Decode(Consts.LOG_TRACK) + trackData);

            PlayerPrefs.SetString(Tools.Base64Decode(Consts.MHCSDK_PREFS_ANAL) + dataCount.ToString(), trackData);
            dataCount++;
            PlayerPrefs.SetInt(Tools.Base64Decode(Consts.MHCSDK_PREFS_COUNT), dataCount);
            PlayerPrefs.Save();

            // отправка статистики происходит при этих ивентах обязательно
            if (eventData.type == EventType.banner_press || eventData.type == EventType.iap_ok || eventData.type == EventType.app_start) Commit();
        }
        #endregion

        #region public static void Commit() - отправить накопленную аналитику в обход основной логики
        /// <summary>
        /// отправить накопленную аналитику в обход основной логики
        /// </summary>
        public static void Commit()
        {
            //аналитика не инициализирована
            if (string.IsNullOrEmpty(jsonHeader))
            {
                if (Core.log_issue) Debug.Log(Tools.Base64Decode(Consts.LOG_TRACK_INITERROR));
                return;
            }

            //нечего отправлять
            if (dataCount <= 0)
            {
                if (Core.log_issue) Debug.Log(Tools.Base64Decode(Consts.LOG_TRACK_NOTHING));
                return;
            }

            string xml = Tools.Base64Decode(Consts.BRACE_ON);
            xml = xml + jsonHeader;

            for (int i = 0; i < dataCount; i++)
            {
                string analEvent = PlayerPrefs.GetString(Tools.Base64Decode(Consts.MHCSDK_PREFS_ANAL) + i, "");
                if (string.IsNullOrEmpty(analEvent) == false) xml = xml + Tools.Base64Decode(Consts.WS_SEPARATOR) + analEvent;
            }

            xml = xml + Tools.Base64Decode(Consts.BRACE_OFF);

            if (Core.log_issue) Debug.Log(Tools.Base64Decode(Consts.LOG_ANAL_CONTENT) + xml);

            Network.Post(Tools.Base64Decode(Consts.URL_ANALYTICS), xml, PostResponse);
        }
        #endregion

        #region private static void PostResponse(string responseCode, byte[] response, object tag) - Обработка ответа от сервера
        /// <summary>
        /// Обработка ответа от сервера
        /// </summary>
        /// <param name="responseCode"></param>
        /// <param name="response"></param>
        /// <param name="tag"></param>
        private static void PostResponse(string responseCode, byte[] response, object tag)
        {
            //Если сервер ответил нечто отличное от 200, значит что то пошло не так, закругляемся.
            if (Core.log_issue) Debug.Log(Tools.Base64Decode(Consts.LOG_RESPONSE_CODE) + responseCode);
            if (responseCode != Tools.Base64Decode(Consts.RESPONSE_URL_OK)) return;

            string responseSTR = Encoding.UTF8.GetString(response);
            if (Core.log_issue) Debug.Log(Tools.Base64Decode(Consts.LOG_RESPONSE) + responseSTR);

            // если сервер не вернул 0 - храним аналитику дальше
            if (responseSTR != Tools.Base64Decode(Consts.D_0)) return;

            for (int i = 0; i< dataCount; i++) PlayerPrefs.DeleteKey(Tools.Base64Decode(Consts.MHCSDK_PREFS_ANAL) + i.ToString());
            PlayerPrefs.SetInt(Tools.Base64Decode(Consts.MHCSDK_PREFS_COUNT), 0);
            dataCount = 0;
        }
        #endregion

        #region private static string MakeJsonPair(string key, string value) - скрываю сотни стремных плюсков и символов в коде. формирует строку вида "key": "value"
        /// <summary>
        /// скрываю сотни стремных плюсков и символов в коде. формирует строку вида "key": "value"
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string MakeJsonPair(string key, string value)
        {
            return "\"" + key + "\"" + ": " + "\"" + value + "\"";
        }
        #endregion
    }
}
