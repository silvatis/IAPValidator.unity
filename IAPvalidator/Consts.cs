
namespace MHCSDK
{
    internal class Consts
    {
        
        // url для запросов (BASE64)
        internal const string   URL_VEREFY          = "Y2hrLmhlcm9jcmFmdC5jb20vc3RvcmVfdmVyaWZ5";                           //"chk.herocraft.com/store_verify"
        internal const string   URL_PROMO           = "YWR2Lmhlcm9jcmFmdC5jb20vZXh0ZXJuYWwvYXBpL2dldF9sYWJlbF9zYWxlcy8=";   //"adv.herocraft.com/external/api/get_label_sales/"
        internal const string   URL_PROPERTIES      = "bS5oZXJvY3JhZnQuY29tL2V4dGVybmFsL2FwaS9nYW1lL2dldF9jb25zdGFudHMv";   //"m.herocraft.com/external/api/game/get_constants/"
        internal const string   URL_ANALYTICS       = "Z2FtZXN0YXRzLmhlcm9jcmFmdC5jb20vZ2FtZV9ldmVudC8=";                   //"gamestats.herocraft.com/game_event/"

        //обфусцированные константы для лога (BASE64)
        internal const string   LOG_URL             = "TUhDU0RLOiB1cmwgPSA=";                                   //"MHCSDK: url = "
        internal const string   LOG_DATA            = "TUhDU0RLOiBkYXRhID0g";                                   //"MHCSDK: data = "
        internal const string   LOG_SIGN            = "TUhDU0RLOiBzaWduID0g";                                   //"MHCSDK: sign = "
        internal const string   LOG_SIGNRAW         = "TUhDU0RLOiBzaWduUkFXID0g";                               //"MHCSDK: signRAW = "
        internal const string   LOG_TIME            = "TUhDU0RLOiB0aW1lID0g";                                   //"MHCSDK: time = "
        internal const string   LOG_GAMEID          = "TUhDU0RLOiBnYW1lX2lkID0g";                               //"MHCSDK: game_id = "
        internal const string   LOG_STOREID         = "TUhDU0RLOiBzdG9yZV9pZCA9IA==";                           //"MHCSDK: store_id = "
        internal const string   LOG_PRODUCTID       = "TUhDU0RLOiBwcm9kdWN0SUQgPSA=";                           //"MHCSDK: productID = "
        internal const string   LOG_ERROR           = "TUhDU0RLOiBFcnJvciA9IA===";                              //"MHCSDK: Error = "
        internal const string   LOG_RESPONSE        = "TUhDU0RLOiBzZXJ2ZXIgcmVzcG9uc2UgPSA=";                   //"MHCSDK: server response = "
        internal const string   LOG_RESPONSE_CODE   = "TUhDU0RLOiBzZXJ2ZXIgcmVzcG9uc2UgY29kZSA9IA==";           //"MHCSDK: server response code = "
        internal const string   LOG_VALIDATESTR     = "TUhDU0RLOiB2YWxpZGF0ZSBzdHJpbmcgPSA=";                   //"MHCSDK: validate string = "
        internal const string   LOG_SANDBOX         = "TUhDU0RLOiBzYW5kYm94ID0gMQ==";                           //"MHCSDK: sandbox = 1"
        internal const string   LOG_CBIS_NULL       = "TUhDU0RLOiBjYiBpcyBudWxsIQ==";                           //"MHCSDK: cb is null!"
        internal const string   LOG_XCEPTION        = "TUhDU0RLOiBleGNlcHRpb24gPSA=";                           //"MHCSDK: exception = "
        internal const string   LOG_QUERY           = "TUhDU0RLOiBxdWVyeSA9IA==";                               //"MHCSDK: query = "
        internal const string   LOG_TRACK           = "SENTREs6IFRyYWNrID0g";                                   //"HCSDK: Track = "
        internal const string   LOG_TRACK_INITERROR = "SENTREs6IEFuYWxpdHljcyBpcyBub3QgaW5pdGVkLg==";           //"HCSDK: Analitycs is not inited."
        internal const string   LOG_TRACK_NOTHING   = "SENTREs6IEFuYWx5dHljcyBoYXMgbm90aGluZyB0byBzZW5kLg==";   //"HCSDK: Analytycs has nothing to send."
        internal const string   LOG_ANAL_CONTENT    = "TUhDU0RLOiBBbmFseXR5Y3MgY29udGVudCA9IA==";               //"MHCSDK: Analytycs content = "

        //обфусцированные константы для формирования GET запроса (BASE64)
        internal const string   FORM_KEY_V          = "dg==";                           //"v"
        internal const string   FORM_KEY_ACTION     = "YWN0aW9u";                       //"action"
        internal const string   FORM_KEY_DATA       = "ZGF0YQ==";                       //"data"
        internal const string   FORM_KEY_SIGN       = "c2lnbg==";                       //"sign"
        internal const string   FORM_KEY_TIME       = "dGltZQ==";                       //"time"
        internal const string   FORM_KEY_GAMEID     = "Z2FtZV9pZA==";                   //"game_id"
        internal const string   FORM_KEY_PROVID     = "cHJvdl9pZA==";                   //"prov_id"
        internal const string   FORM_KEY_STOREID    = "c3RvcmVfaWQ=";                   //"store_id"
        internal const string   FORM_KEY_PRODUCTID  = "cHJvZHVjdF9pZA==";               //"product_id"
        internal const string   FORM_KEY_SANDBOX    = "c2FuZGJveA==";                   //"sandbox"
        internal const string   FORM_VALUE_ACTION   = "c3RvcmVfdmVyaWZ5";               //"store_verify"
        internal const string   FORM_KEY_PORT       = "cG9ydA==";                       //"port"
        internal const string   FORM_KEY_GAMEV      = "Z2FtZV92ZXJzaW9u";               //"game_version"
        internal const string   FORM_KEY_PROD       = "cHJvZA==";                       //"prod"
        internal const string   FORM_KEY_PROV       = "cHJvdg==";                       //"prov"
        internal const string   FORM_KEY_LANG       = "bGFuZw==";                       //"lang"
        internal const string   FORM_KEY_APIV       = "YXBpX3ZlcnNpb24=";               //"api_version"
        internal const string   FORM_KEY_DEVICEID   = "ZGV2aWNlX2lk";                   //"device_id"
        internal const string   FORM_KEY_USERID     = "dXNlcl9pZA==";                   //"user_id"
        internal const string   FORM_KEY_GAME_TYPE  = "Z2FtZV90eXBl";                   //"game_type"
        internal const string   FORM_KEY_PROD_ID    = "cG9ydF9pZA==";                   //"port_id"
        internal const string   FORM_KEY_CHANNEL    = "Y2hhbm5lbA==";                   //"channel"
        internal const string   FORM_KEY_PHONESYSID = "cGhvbmVfc3lzX2lk";               //"phone_sys_id"
        internal const string   FORM_KEY_VERSION    = "dmVyc2lvbg==";                   //"version"
        internal const string   FORM_KEY_HCSDKVER   = "aGNzZGtfdmVyc2lvbg==";           //"hcsdk_version"
        internal const string   FORM_KEY_REFERRER   = "cmVmZXJyZXI=";                   //"referrer"
        internal const string   FORM_KEY_TYPE       = "dHlwZQ==";                       //"type"
        internal const string   FORM_KEY_SLOT       = "c2xvdA==";                       //"slot"

        // имена PlayerPrefs
        internal const string   MHCSDK_PREFS_COUNT  = "TUhDU0RLX1BSRUZTX0NPVU5U";       //MHCSDK_PREFS_COUNT
        internal const string   MHCSDK_PREFS_ANAL   = "TUhDU0RLX1BSRUZTX0FOQUw=";       //MHCSDK_PREFS_ANAL

        //прочие константы (BASE64)
        internal const string   BRACE_ON            = "Ww==";                           //"["
        internal const string   BRACE_OFF           = "XQ==";                           //"]"
        internal const string   C_BRACE_ON          = "ew==";                           //"{"
        internal const string   C_BRACE_OFF         = "fQ==";                           //"}"
        internal const string   SEPARATOR           = "LA==";                           //","
        internal const string   WS_SEPARATOR        = "LCA=";                           //", "
        internal const string   MD5_ENCODE_FORMAT   = "eDI=";                           //"x2"
        internal const string   GAMEOBJECT_NAME     = "TUhDU0RL";                       //"MHCSDK"
        internal const string   ANAL_TIME_FORMAT    = "eXl5eS1NTS1kZCBISDptbTpzcw==";   //"yyyy-MM-dd HH:mm:ss"
        internal const string   RESPONSE_URL_OK     = "MjAw";                           //"200"
        internal const string   Q_MARK              = "Pw==";                           //"?"
        internal const string   D_0                 = "MA==";                           //"0"
        internal const string   D_1                 = "MQ==";                           //"1"
        internal const string   D_2                 = "Mg==";                           //"2"

        //чтение KDSCH
        internal const string   KDSCH_FILENAME      = "kdsch";                          // "kdsch" - имя kdsch файла по умолчанию
        internal const string   KDSCH_SEPARATOR     = "Og==";                           // ":" - сепаратор для парсинга kdsch     

        //Версия СДК
        internal const string    VERSION            = "2.0.2";                          // версия библиотеки (при необходимости отладки)   
    }
}
