// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("m+D3+xc8EWV3n8VCpjeCOq16ka2loKN0aeHq1/VY+PBRCJDyBiuU6w4CnbHJGpv0Sa6U8CluOyI3HtSh9K0I6OBxP2hZ/qTbDEqdUYQ7Tqvla0HfhaQu+rkmq3Za6FplXN0D5hF0k+LcHjLi9NIeRVuwhul6LESLINA0un3kcCI97QXIRTsjjAgjuVZdJcm/lTVKj7wbGFEDcbw6ANfr+C242eol9jol2ofOOk3EAlkFEwThnshfBtZH8JYfVM8s5fiG/erZYU5wB3jIYaV1nM1tMnwS8d0EbCNjQl/c0t3tX9zX31/c3N1e2AQgKVk7gsLOlprdSYYu9SxnRpIQ/oV5RmztX9z/7dDb1PdblVsq0Nzc3Njd3i0RgUasCfWOyt/e3N3c");
        private static int[] order = new int[] { 11,9,4,13,12,7,12,8,8,12,13,12,13,13,14 };
        private static int key = 221;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
