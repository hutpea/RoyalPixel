using System.Collections;
using System.Collections.Generic;
using JeremyAnsel.ColorQuant;
using UnityEngine;

public class ImageQuantizer 
{

    public static Color32[] QuantizeImage(Color32[] colors, int colorCount) {
        byte[] pixels = new byte[colors.Length * 4];
        for (int i = 0; i < colors.Length; i++) {
            pixels[i * 4] = colors[i].r;
            pixels[i * 4 + 1] = colors[i].g;
            pixels[i * 4 + 2] = colors[i].b;
            pixels[i * 4 + 3] = colors[i].a;
        }
        WuAlphaColorQuantizer quantizer = new WuAlphaColorQuantizer();
        ColorQuantizerResult result = quantizer.Quantize(pixels, colorCount);
        byte[] outputPixels = result.Bytes;
        byte[] pallete = result.Palette;
        Color32[] outputColors = new Color32[colors.Length];
        for (int i = 0; i < outputPixels.Length; i++) {
            byte pal = outputPixels[i];
            Color32 color = new Color32(pallete[pal * 4], pallete[pal * 4 + 1], pallete[pal * 4 + 2], pallete[pal * 4 + 3]);
            outputColors[i] = color;
        }
        return outputColors;
    }
}
