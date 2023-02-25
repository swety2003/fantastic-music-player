using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasticMusicPlayer.lib
{
    [Serializable]
    public enum BlurType
    {
        Both,
        HorizontalOnly,
        VerticalOnly,
    }

    [Serializable]
    public class GaussianBlur
    {
        private int _radius = 1;
        private int[] _kernel;
        private int _kernelSum;
        private int[,] _multable;
        private BlurType _blurType;

        public GaussianBlur()
        {
            PreCalculateSomeStuff();
        }

        public GaussianBlur(int radius)
        {
            _radius = radius;
            PreCalculateSomeStuff();
        }

        private void PreCalculateSomeStuff()
        {
            int sz = _radius * 2 + 1;
            _kernel = new int[sz];
            _multable = new int[sz, 256];
            for (int i = 1; i <= _radius; i++)
            {
                int szi = _radius - i;
                int szj = _radius + i;
                _kernel[szj] = _kernel[szi] = (szi + 1) * (szi + 1);
                _kernelSum += (_kernel[szj] + _kernel[szi]);
                for (int j = 0; j < 256; j++)
                {
                    _multable[szj, j] = _multable[szi, j] = _kernel[szj] * j;
                }
            }
            _kernel[_radius] = (_radius + 1) * (_radius + 1);
            _kernelSum += _kernel[_radius];
            for (int j = 0; j < 256; j++)
            {
                _multable[_radius, j] = _kernel[_radius] * j;
            }
        }

        public long t1;
        public long t2;
        public long t3;
        public long t4;

        public Bitmap ProcessImage(Image inputImage)
        {
            Bitmap origin = new Bitmap(inputImage);
            Bitmap blurred = new Bitmap(inputImage.Width, inputImage.Height);

            using (RawBitmap src = new RawBitmap(origin))
            {
                using (RawBitmap dest = new RawBitmap(blurred))
                {
                    int pixelCount = src.Width * src.Height;
                    //Stopwatch sw = new Stopwatch();
                    //sw.Start();
                    int[] b = new int[pixelCount];
                    int[] g = new int[pixelCount];
                    int[] r = new int[pixelCount];

                    int[] b2 = new int[pixelCount];
                    int[] g2 = new int[pixelCount];
                    int[] r2 = new int[pixelCount];
                    //sw.Stop();
                    //t1 = sw.ElapsedMilliseconds;

                    int offset = src.GetOffset();
                    int index = 0;
                    unsafe
                    {
                        //sw.Reset();
                        //sw.Start();

                        byte* ptr = src.Begin;
                        for (int i = 0; i < src.Height; i++)
                        {
                            for (int j = 0; j < src.Width; j++)
                            {
                                b[index] = *ptr * 128 / 255;
                                ptr++;
                                g[index] = *ptr * 128 / 255;
                                ptr++;
                                r[index] = *ptr * 128 / 255;
                                ptr++;

                                ++index;
                            }
                            ptr += offset;
                        }

                        //sw.Stop();
                        //t2 = sw.ElapsedMilliseconds;

                        int bsum;
                        int gsum;
                        int rsum;
                        int read;
                        int start = 0;
                        index = 0;

                        //sw.Reset();
                        //sw.Start();

                        if (_blurType != BlurType.VerticalOnly)
                        {
                            for (int i = 0; i < src.Height; i++)
                            {
                                for (int j = 0; j < src.Width; j++)
                                {
                                    bsum = gsum = rsum = 0;
                                    read = index - _radius;

                                    for (int z = 0; z < _kernel.Length; z++)
                                    {
                                        //if (read >= start && read < start + src.Width)
                                        //{
                                        //    bsum += _multable[z, b[read]];
                                        //    gsum += _multable[z, g[read]];
                                        //    rsum += _multable[z, r[read]];
                                        //    sum += _kernel[z];
                                        //}

                                        if (read < start)
                                        {
                                            bsum += _multable[z, b[start]];
                                            gsum += _multable[z, g[start]];
                                            rsum += _multable[z, r[start]];
                                        }
                                        else if (read > start + src.Width - 1)
                                        {
                                            int idx = start + src.Width - 1;
                                            bsum += _multable[z, b[idx]];
                                            gsum += _multable[z, g[idx]];
                                            rsum += _multable[z, r[idx]];
                                        }
                                        else
                                        {
                                            bsum += _multable[z, b[read]];
                                            gsum += _multable[z, g[read]];
                                            rsum += _multable[z, r[read]];
                                        }
                                        ++read;
                                    }

                                    //b2[index] = (bsum / sum);
                                    //g2[index] = (gsum / sum);
                                    //r2[index] = (rsum / sum);

                                    b2[index] = (bsum / _kernelSum);
                                    g2[index] = (gsum / _kernelSum);
                                    r2[index] = (rsum / _kernelSum);

                                    if (_blurType == BlurType.HorizontalOnly)
                                    {
                                        //byte* pcell = dest[j, i];
                                        //*pcell = (byte)(bsum / sum);
                                        //pcell++;
                                        //*pcell = (byte)(gsum / sum);
                                        //pcell++;
                                        //*pcell = (byte)(rsum / sum);
                                        //pcell++;

                                        byte* pcell = dest[j, i];
                                        *pcell = (byte)(bsum / _kernelSum);
                                        pcell++;
                                        *pcell = (byte)(gsum / _kernelSum);
                                        pcell++;
                                        *pcell = (byte)(rsum / _kernelSum);
                                        pcell++;
                                    }

                                    ++index;
                                }
                                start += src.Width;
                            }
                        }
                        if (_blurType == BlurType.HorizontalOnly)
                        {
                            return blurred;
                        }

                        //sw.Stop();
                        //t3 = sw.ElapsedMilliseconds;

                        //sw.Reset();
                        //sw.Start();

                        int tempy;
                        for (int i = 0; i < src.Height; i++)
                        {
                            int y = i - _radius;
                            start = y * src.Width;
                            for (int j = 0; j < src.Width; j++)
                            {
                                bsum = gsum = rsum = 0;
                                read = start + j;
                                tempy = y;
                                for (int z = 0; z < _kernel.Length; z++)
                                {
                                    //if (tempy >= 0 && tempy < src.Height)
                                    //{
                                    //    if (_blurType == BlurType.VerticalOnly)
                                    //    {
                                    //        bsum += _multable[z, b[read]];
                                    //        gsum += _multable[z, g[read]];
                                    //        rsum += _multable[z, r[read]];
                                    //    }
                                    //    else
                                    //    {
                                    //        bsum += _multable[z, b2[read]];
                                    //        gsum += _multable[z, g2[read]];
                                    //        rsum += _multable[z, r2[read]];
                                    //    }
                                    //    sum += _kernel[z];
                                    //}

                                    if (_blurType == BlurType.VerticalOnly)
                                    {
                                        if (tempy < 0)
                                        {
                                            bsum += _multable[z, b[j]];
                                            gsum += _multable[z, g[j]];
                                            rsum += _multable[z, r[j]];
                                        }
                                        else if (tempy > src.Height - 1)
                                        {
                                            int idx = pixelCount - (src.Width - j);
                                            bsum += _multable[z, b[idx]];
                                            gsum += _multable[z, g[idx]];
                                            rsum += _multable[z, r[idx]];
                                        }
                                        else
                                        {
                                            bsum += _multable[z, b[read]];
                                            gsum += _multable[z, g[read]];
                                            rsum += _multable[z, r[read]];
                                        }
                                    }
                                    else
                                    {
                                        if (tempy < 0)
                                        {
                                            bsum += _multable[z, b2[j]];
                                            gsum += _multable[z, g2[j]];
                                            rsum += _multable[z, r2[j]];
                                        }
                                        else if (tempy > src.Height - 1)
                                        {
                                            int idx = pixelCount - (src.Width - j);
                                            bsum += _multable[z, b2[idx]];
                                            gsum += _multable[z, g2[idx]];
                                            rsum += _multable[z, r2[idx]];
                                        }
                                        else
                                        {
                                            bsum += _multable[z, b2[read]];
                                            gsum += _multable[z, g2[read]];
                                            rsum += _multable[z, r2[read]];
                                        }
                                    }


                                    read += src.Width;
                                    ++tempy;
                                }

                                byte* pcell = dest[j, i];

                                //pcell[0] = (byte)(bsum / sum);
                                //pcell[1] = (byte)(gsum / sum);
                                //pcell[2] = (byte)(rsum / sum);

                                pcell[0] = (byte)(bsum / _kernelSum);
                                pcell[1] = (byte)(gsum / _kernelSum);
                                pcell[2] = (byte)(rsum / _kernelSum);
                            }
                        }
                        //sw.Stop();
                        //t4 = sw.ElapsedMilliseconds;
                    }
                }
            }

            return blurred;
        }

        public int Radius
        {
            get { return _radius; }
            set
            {
                if (value < 1)
                {
                    throw new InvalidOperationException("Radius must be greater then 0");
                }
                _radius = value;

            }
        }

        public BlurType BlurType
        {
            get { return _blurType; }
            set
            {
                _blurType = value;
            }
        }
    }

    public unsafe class RawBitmap : IDisposable
    {
        private Bitmap _originBitmap;
        private BitmapData _bitmapData;
        private byte* _begin;

        public RawBitmap(Bitmap originBitmap)
        {
            _originBitmap = originBitmap;
            _bitmapData = _originBitmap.LockBits(new Rectangle(0, 0, _originBitmap.Width, _originBitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            _begin = (byte*)(void*)_bitmapData.Scan0;
        }

        #region IDisposable Members

        public void Dispose()
        {
            _originBitmap.UnlockBits(_bitmapData);
        }

        #endregion

        public unsafe byte* Begin
        {
            get { return _begin; }
        }

        public unsafe byte* this[int x, int y]
        {
            get
            {
                return _begin + y * (_bitmapData.Stride) + x * 3;
            }
        }

        public unsafe byte* this[int x, int y, int offset]
        {
            get
            {
                return _begin + y * (_bitmapData.Stride) + x * 3 + offset;
            }
        }

        //public unsafe void SetColor(int x, int y, int color)
        //{
        //    *(int*)(_begin + y * (_bitmapData.Stride) + x * 3) = color;
        //}

        public int Stride
        {
            get { return _bitmapData.Stride; }
        }

        public int Width
        {
            get { return _bitmapData.Width; }
        }

        public int Height
        {
            get { return _bitmapData.Height; }
        }

        public int GetOffset()
        {
            return _bitmapData.Stride - _bitmapData.Width * 3;
        }

        public Bitmap OriginBitmap
        {
            get { return _originBitmap; }
        }
    }



    /// <summary>
    /// Summary description for TextShadow
    /// </summary>
    public class TextShadow
    {
        private int radius = 5;
        private int distance = 10;
        private double angle = 60;
        private byte alpha = 255;

        /// <summary>
        /// 高斯卷积矩阵
        /// </summary>
        private int[] gaussMatrix;
        /// <summary>
        /// 卷积核
        /// </summary>
        private int nuclear = 0;

        /// <summary>
        /// 阴影半径
        /// </summary>
        public int Radius
        {
            get
            {
                return radius;
            }
            set
            {
                if (radius != value)
                {
                    radius = value;
                    MakeGaussMatrix();
                }
            }
        }

        /// <summary>
        ///  阴影距离
        /// </summary>
        public int Distance
        {
            get
            {
                return distance;
            }
            set
            {
                distance = value;
            }
        }

        /// <summary>
        ///  阴影输出角度(左边平行处为0度。顺时针方向)
        /// </summary>
        public double Angle
        {
            get
            {
                return angle;
            }
            set
            {
                angle = value;
            }
        }

        /// <summary>
        /// 阴影文字的不透明度
        /// </summary>
        public byte Alpha
        {
            get
            {
                return alpha;
            }
            set
            {
                alpha = value;
            }
        }

        /// <summary>
        /// 对文字阴影位图按阴影半径计算的高斯矩阵进行卷积模糊
        /// </summary>
        /// <param name="bmp">文字阴影位图</param>
        public unsafe void MaskShadow(Bitmap bmp)
        {
            if (nuclear == 0)
                MakeGaussMatrix();
            Rectangle r = new Rectangle(0, 0, bmp.Width, bmp.Height);
            // 克隆临时位图，作为卷积源
            Bitmap tmp = (Bitmap)bmp.Clone();
            BitmapData dest = bmp.LockBits(r, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            BitmapData source = tmp.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                // 源首地址(0, 0)的Alpha字节，也就是目标首像素的第一个卷积乘数的像素点
                byte* ps = (byte*)source.Scan0;
                ps += 3;
                // 目标地址为卷积半径点(radius, radius)的Alpha字节
                byte* pd = (byte*)dest.Scan0;
                pd += (radius * (dest.Stride + 4) + 3);
                // 位图实际卷积的部分
                int width = dest.Width - radius * 2;
                int height = dest.Height - radius * 2;
                int matrixSize = radius * 2 + 1;
                // 卷积矩阵字节偏移
                int mOffset = dest.Stride - matrixSize * 4;
                // 行尾卷积半径(radius)的偏移
                int rOffset = radius * 8;
                int count = matrixSize * matrixSize;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {

                        byte* s = ps - mOffset;
                        int v = 0;
                        for (int i = 0; i < count; i++, s += 4)
                        {
                            if ((i % matrixSize) == 0)
                                s += mOffset;           // 卷积矩阵的换行
                            v += gaussMatrix[i] * *s;   // 位图像素点Alpha的卷积值求和
                        }
                        // 目标位图被卷积像素点Alpha等于卷积和除以卷积核
                        *pd = (byte)(v / nuclear);
                        pd += 4;
                        ps += 4;
                    }
                    pd += rOffset;
                    ps += rOffset;
                }
            }
            finally
            {
                tmp.UnlockBits(source);
                bmp.UnlockBits(dest);
                tmp.Dispose();
            }
        }

        /// <summary>
        /// 按给定的阴影半径生成高斯卷积矩阵
        /// </summary>
        protected virtual void MakeGaussMatrix()
        {
            double Q = (double)radius / 2.0;
            if (Q == 0.0)
                Q = 0.1;
            int n = radius * 2 + 1;
            int index = 0;
            nuclear = 0;
            gaussMatrix = new int[n * n];

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    gaussMatrix[index] = (int)Math.Round(Math.Exp(-((double)x * x + y * y) / (2.0 * Q * Q)) /
                                                         (2.0 * Math.PI * Q * Q) * 1000.0);
                    nuclear += gaussMatrix[index];
                    index++;
                }
            }
        }

        public TextShadow()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }

}
