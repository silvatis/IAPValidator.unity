using UnityEngine;

namespace MHCSDK
{
    public class Core : MonoBehaviour
    {
        internal  static bool         log_issue         = false;    // Выводить сообщения в лог

        private static GameObject   instance    = null;     // синглтон

        public enum GameType
        {
            not_set,
            full,
            lite,
            free
        }

        public static void EnableDebug(bool logStatus)
        {
            log_issue = logStatus;
        }

        #region public static string Version() - Возвращает версию СДК
        /// <summary>
        /// Возвращает версию СДК
        /// </summary>
        /// <returns></returns>
        public static string Version()
        {
            return Consts.VERSION;
        }
        #endregion

        #region internal static GameObject Instance() - получить GameObject от СДК 
        internal static GameObject Instance()
        {
            if (instance != null) return instance;
            instance = new GameObject(Tools.Base64Decode(Consts.GAMEOBJECT_NAME));
            DontDestroyOnLoad(instance);
            return instance;
        }
        #endregion

    }
}
