using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows;

namespace FantasticMusicPlayer.Extensions
{
    internal static class BitmapExtension
    {
        public static ImageSource ToBitmapSource(this Bitmap bitmap)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            //var bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //BitmapSource bitmapSource = BitmapSource.Create(bitmap.Width, bitmap.Height, 96, 96, PixelFormats.Bgr24, myPalette, bmpData.Scan0, bitmap.Width * bitmap.Height * 3, bitmap.Width * 3);
            //bitmap.UnlockBits(bmpData);
            //return bitmapSource;
        }

        //public static ImageSource ToBitmapSource(this Image img)
        //{
        //    return ToBitmapSource(null);
        //}
    }
}
