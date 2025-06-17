using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using BarcodeLib;

namespace Domain.Entities
{
    public static class BarcodeHelper
    {
        public static byte[] GenerateBarcodeBase64(string barcodeText)
        {
            var barcode = new BarcodeLib.Barcode
            {
                IncludeLabel = true,
                Alignment = AlignmentPositions.CENTER,
                LabelPosition = LabelPositions.BOTTOMCENTER
            };


            int minWidth = 300;
            int width = Math.Max(minWidth, barcodeText.Length * 25); 
            int height = 120;

            if (width < 300)
                width = 300;

            using var image = barcode.Encode(TYPE.CODE128, barcodeText, Color.Black, Color.White, width, height);

            using var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }
    }
        public static class ImageHelper
    {
        public static byte[] ConvertToBase64(IFormFile file)
        {
            if (file == null) return null;
            using var ms = new MemoryStream();
            file.CopyTo(ms);
            return ms.ToArray();
        }
    }
  

}
