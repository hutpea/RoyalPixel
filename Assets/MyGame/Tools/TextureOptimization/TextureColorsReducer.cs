using UnityEngine;

namespace Tools.TextureOptimization
{
    public class TextureColorsReducer
    {
        private static int s_width;

        private static int s_height;

        private static int s_colorsCount;

        public static Texture2D Process(Texture2D sourceTex, int colorsCount)
        {
            s_colorsCount = colorsCount;
            s_width = sourceTex.width;
            s_height = sourceTex.height;
            Color32[] pixels = ImageQuantizer.QuantizeImage(sourceTex.GetPixels32(), 64);
            //byte[] bytes = ConvertColorsToBytes(sourceTex.GetPixels32());
            //Color32[] pixels = AnalyzeAndChangePixels(bytes);
            Texture2D texture2D = new Texture2D(s_width, s_height, sourceTex.format, mipChain: false);
            texture2D.filterMode = sourceTex.filterMode;
            Texture2D texture2D2 = texture2D;
            texture2D2.SetPixels32(pixels);
            texture2D2.Apply();
            return texture2D2;
        }

        protected static byte[] ConvertColorsToBytes(Color32[] data)
        {
            byte[] array = new byte[3 * s_width * s_height];
            int num = 0;
            for (int num2 = s_height - 1; num2 >= 0; num2--)
            {
                for (int i = 0; i < s_width; i++)
                {
                    Color32 color = data[num2 * s_width + i];
                    array[num] = color.r;
                    num++;
                    array[num] = color.g;
                    num++;
                    array[num] = color.b;
                    num++;
                }
            }
            return array;
        }

        protected static Color32[] AnalyzeAndChangePixels(byte[] bytes)
        {
            int num = bytes.Length;
            int num2 = num / 3;
            Color32[] array = new Color32[num2];
            NeuQuant neuQuant = new NeuQuant(bytes, num, 2, s_colorsCount);
            byte[] array2 = neuQuant.Process();
            int num3 = 0;
            for (int num4 = s_height - 1; num4 >= 0; num4--)
            {
                for (int i = 0; i < s_width; i++)
                {
                    int num8 = neuQuant.Map(bytes[num3++] & 0xFF, bytes[num3++] & 0xFF, bytes[num3++] & 0xFF);
                    array[num4 * s_width + i] = new Color32(array2[num8 * 3], array2[num8 * 3 + 1], array2[num8 * 3 + 2], byte.MaxValue);
                }
            }
            return array;
        }
    }
}
