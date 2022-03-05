#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("QRjzaEDF0fscyLgaUeWO4beMDedHejOSvWv3ZrdKLhHlMBChs49/J5MQHhEhkxAbE5MQEBGw8nOMfygcaJ5A4vAqXeHD+aZ1mPeRFFTiXPvjmQR9V02CUgT6W3XXim+HZoUP3xDS3uiGpy2b5WUxsgVNrr/DtCvsx6j85DEnaLJ6/hmr2CeuLDzeWgchkxAzIRwXGDuXWZfmHBAQEBQREiylA72+Sfuw1nCYEVZx1BCRCeeMcFW8Vqd9JDCHxeUHJcdlijdVjAk64QD6q4nN/DEYdo682OpphxiB/K+0zkXPKP35a5/d4kkow4T9Xclr8kGAg98jJX1YZsFGaMJVicsM1GNaZa+1dfxjC0/0lc05+O91PGHcbY9QZijCTTiYgBMSEBEQ");
        private static int[] order = new int[] { 11,9,9,6,12,5,6,11,12,11,10,12,12,13,14 };
        private static int key = 17;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
