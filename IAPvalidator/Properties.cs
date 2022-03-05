using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace MHCSDK
{
    public class Properties
    {
        //Поля ----------------------------------------------------------------------------------------------------------------------------------------------------------
        private static bool inited = false;

        // словарь со значениями из файла kdsch, когда недоступен натив
        private static Dictionary<string, string> UserProperties = new Dictionary<string, string>();

        //Методы --------------------------------------------------------------------------------------------------------------------------------------------------------
        //некоторая их часть бралась из нативного кода копипастом, что говорит о необходимости рефакторинга.
        //реквестирую 1024 часа в сутках

        #region public static void Init(int xorKey, string kdschFileName = Consts.KDSCH_FILENAME) - Считывает kdsch файл
        /// <summary>
        /// Считывает kdsch файл
        /// </summary>
        /// <param name="xorKey"></param>
        /// <param name="kdschFileName"></param>
        public static void Init(int xorKey, string kdschFileName = Consts.KDSCH_FILENAME)
        {
            if (inited)
            {
                return;
            }

            //счетчик ошибок.
            int errors = 0;

            //Инициализируем словарь с результатом, собираем путь к файлу
            UserProperties = new System.Collections.Generic.Dictionary<string, string>();
            string kdschPath = System.IO.Path.Combine(Application.streamingAssetsPath, kdschFileName);

            byte[] bkdsch;
            if (Application.platform == RuntimePlatform.Android)
            {
                WWW reader = new WWW(kdschPath);
                while (!reader.isDone){}
                bkdsch = reader.bytes;
            }
            else bkdsch = System.IO.File.ReadAllBytes(kdschPath); 

            //считываем файл конфига в массив байтов, создаем удобный считыватель 
            MemoryStream streamKDSCH = new MemoryStream(bkdsch);
            BinaryReader kdschReader = new BinaryReader(streamKDSCH);

            //построчно читаем kdsch файл
            while (kdschReader.BaseStream.Position < kdschReader.BaseStream.Length)
            {
                byte[] bstring = ReadString(kdschReader);
                bstring = Unxor(bstring, xorKey, false);

                string rawValue = System.Text.Encoding.UTF8.GetString(bstring);

                //каждая строка имеет вид <ключ:значение> ,причем, в значении могут быть двоеточия. забиваем словарь со значениями.
                int pos = rawValue.IndexOf(Tools.Base64Decode(Consts.KDSCH_SEPARATOR));

                //в любой непонятной ситуации с сепаратором,
                //считаем ошибки, и берем следующее значение.
                if (pos < 0)
                {
                    Debug.LogWarning("kdsch splittedResult has problem with value = " + rawValue);
                    errors++;
                    continue;
                }

                //все прошло нормально, добавим результат в словарик.
                UserProperties.Add(rawValue.Substring(0, pos), rawValue.Substring(pos + 1));
            }

            //сообщим об ошибке, если такое были
            if (errors != 0)
            {
                Debug.LogError("Number of errors, while reading kdsch: " + errors);
            }

            inited = true;
        }
        #endregion

        #region public static string Get(string key) - Получить проперти из файла
        /// <summary>
        /// Получить проперти из файла
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Get(string key)
        {
            string value = "";
            if (UserProperties.TryGetValue(key, out value)) value = value.Trim();
            return value;
        }
        #endregion

        #region private static byte[] Unxor(byte[] strHex, int xorParam, bool withLength) - Разксорирование строчки из конфига
        /// <summary>
        /// Разксорирование строчки из конфига
        /// </summary>
        /// <param name="strHex">входной массив байтов</param>
        /// <param name="xorParam">ключ</param>
        /// <param name="withLength">являются ли первые два байта длинной строки</param>
        /// <returns></returns>
        private static byte[] Unxor(byte[] strHex, int xorParam, bool withLength)
        {
            byte xor = System.Convert.ToByte(xorParam);

            int delta = 0;
            int strLen = strHex.Length >> 1;

            if (withLength) delta = 2;

            byte[] bRet = new byte[strLen + delta];

            if (withLength)
            {
                bRet[0] = (byte)((strLen & 0xFF00) >> 8);
                bRet[1] = (byte)(strLen & 0x00FF);
            }

            for (int i = 0, ib = delta; i < strHex.Length; i += 2, ib++)
            {
                int ah = GetHexValue(strHex[i]) << 4;
                int al = GetHexValue(strHex[i + 1]);
                bRet[ib] = (byte)(((ah & 0xF0) | (al & 0x0F)) ^ xor);
            }

            return bRet;
        }
        #endregion

        #region private static int getHexValue(int code) - private static int getHexValue(int code)
        /// <summary>
        /// Конвертация кода символа
        /// </summary>
        /// <param name="code">код</param>
        /// <returns></returns>
        private static int GetHexValue(int code)
        {
            //мне не нравиться этот говнокод, нужно немножко рефакторинга!
            if ((code >= 48) && (code <= 57))
                return (code - 48); // коды цыфирок 0..9
            if ((code >= 65) && (code <= 90))
                return (code - 65 + 10); // коды A..Z
            if ((code >= 97) && (code <= 122))
                return (code - 97 + 10); // коды a..z
            return 0;
        }
        #endregion

        #region private static byte[] ReadString(BinaryReader dis) - Выбирает строчку из массива байтов
        /// <summary>
        /// Выбирает строчку из массива байтов
        /// </summary>
        /// <param name="dis">поток</param>
        /// <returns></returns>
        private static byte[] ReadString(BinaryReader dis)
        {
            byte ch1 = dis.ReadByte();
            byte ch2 = dis.ReadByte();
            int count = (ch1 << 8) | ch2;

            byte[] result = new byte[count];
            for (int i = 0; (i < count) && (i < dis.BaseStream.Length); i++)
            {
                result[i] = dis.ReadByte();
            }
            return result;
        }
        #endregion

    }
}
