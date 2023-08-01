// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("yrZctwKtvD3HqhFS+p3i9jiRcfeG+/uobkP1jEv+1Zvtey7dwYUcxUvIxsn5S8jDy0vIyMl7loGLiSZao/GzYZsvXohv0t48w79UdKXkvYJ9z4qOxBw+CFUwjUXVCqA5iiwhT/G7PqOSZTIXZV9YkzfKMqxbkh++0kwsEgsksq4fD21lWcMdWE54GMq1NhpDSX6LUiOXarCZDSBBCmD0YHI3Z9ATlUymplfw34e5KEfNc14BOB09PQIdEBEtKvDkR9AxLtbAl+8M/6KRoEUZQWkq1QiGsDdNPOCDkfSrjZlqQbJhx57ulMLjv4oyjMwv+UvI6/nEz8DjT4FPPsTIyMjMycrSGDXDppZwLraUGLD9xRulOMo10lMoCQmVWHc0osvKyMnI");
        private static int[] order = new int[] { 8,3,3,4,10,9,9,7,13,11,11,11,13,13,14 };
        private static int key = 201;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
