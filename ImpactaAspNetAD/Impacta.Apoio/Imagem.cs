﻿using System;
using System.IO;
using System.Drawing;

namespace Impacta.Apoio
{
    public static class Imagem
    {
        public static byte[] ObterMiniatura(byte[] imagem, int largura, int altura)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (Image miniatura = Image.FromStream(new MemoryStream(imagem)).GetThumbnailImage(largura, altura, null, new IntPtr()))
                {
                    miniatura.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    return stream.ToArray();
                }
            }
        }
    }
}