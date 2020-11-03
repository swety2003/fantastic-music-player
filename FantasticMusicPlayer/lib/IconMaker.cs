using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasticMusicPlayer.lib
{
    public static class IconMaker
    {
        public static Icon toIcon(Image img)
        {
            MemoryStream ms = new MemoryStream();
            using (Bitmap bmp = new Bitmap(img, 48, 48)) bmp.Save(ms, ImageFormat.Png);
            MemoryStream icoms = new MemoryStream();
            BinaryWriter bin = new BinaryWriter(icoms);

            bin.Write((short)0);           //0-1保留
            bin.Write((short)1);           //2-3文件类型。1=图标, 2=光标
            bin.Write((short)1);           //4-5图像数量（图标可以包含多个图像）

            bin.Write((byte)48);  //6图标宽度
            bin.Write((byte)48); //7图标高度
            bin.Write((byte)0);            //8颜色数（若像素位深>=8，填0。这是显然的，达到8bpp的颜色数最少是256，byte不够表示）
            bin.Write((byte)0);            //9保留。必须为0
            bin.Write((short)0);           //10-11调色板
            bin.Write((short)32);          //12-13位深
            bin.Write((int)ms.Length);  //14-17位图数据大小
            bin.Write(22);                 //18-21位图数据起始字节

            //写图像数据
            bin.Write(ms.ToArray());

            bin.Flush();
            bin.Seek(0, SeekOrigin.Begin);
            Icon ico = new Icon(icoms);
            ms.Dispose();
            bin.Dispose();
            icoms.Dispose();
            return ico;
        }

    }
}
