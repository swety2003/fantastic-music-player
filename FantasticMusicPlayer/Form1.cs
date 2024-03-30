using FantasticMusicPlayer.dbo.Model;
using FantasticMusicPlayer.lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FantasticMusicPlayer.lib.GdiSystem;

namespace FantasticMusicPlayer
{
    public partial class Form1 : Form
    {
        BassPlayerImpl player = new BassPlayerImpl();
        PlayerController controller;

        

        public Form1()
        {
            InitializeComponent();
            player.init();
            Disposed += Form1_Disposed;
            player.Stopped += Player_Stopped;
            player.CoverAvailable += Player_CoverAvailable;
            player.SongInfoAvailable += Player_SongInfoAvailable;
            controller = new PlayerController(new CurrentDirectorySongProvider());
            controller.SongChanged += Controller_SongChanged;
        }

        private void Invoke(Action p)
        {
            Invoke((Delegate)p);
        }

        private void Controller_SongChanged(object sender, SongSwitchedEventArgs e)
        {
            player.Looping = controller.LoopMode == 1;
            player.Load(e.CurrentSong.Path);
            player.Play();
        }

        private void Player_SongInfoAvailable(object sender, SongInfoEventArgs e)
        {
            runOnUiThread(() =>
            {
                lblTitle.Text = e.title;
                lblArtsit.Text = (e.artist + "    " + e.album).Trim();
                Text = e.title;
                imgHiResAudio.Enabled = e.HiResAudio;
                this.lyricManager = e.LyricManager;
            });
            checkFavStatus();
            updateTopControl();
           

        }

        void runOnUiThread(Action a)
        {
            Invoke(a);
        }

        GaussianBlur blurer = new GaussianBlur(48);

        public Bitmap copyForGuassianBlur(Image img) {
            Bitmap bmp = new Bitmap(100, 100);
            using (Graphics g = Graphics.FromImage(bmp)) {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.DrawImage(img, 0, 0, 100, 100);
            }
            return bmp;
        }

        private void Player_CoverAvailable(object sender, AlbumEventArgs e)
        {
            Bitmap bmp = new Bitmap(locMask.Width, locMask.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                g.DrawImage(e.cover, 0, 0, 228, 228);
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

                float r = bmp.Width * (float)((Math.Sqrt(2) - 1)) / 1.4142136f;

                float r2 = 34.5f;



                Pen p = new Pen(Brushes.Transparent, r);
                g.DrawEllipse(p, 0 - r2, 0 - r2, bmp.Width + r2 + r2 - 1, bmp.Height + r2 + r2 - 1);
            }

            if (!e.Fallback)
            {
                currentBackground = new Bitmap(blurer.ProcessImage(copyForGuassianBlur(e.cover)), this.Width + 20, this.Height + 20);

            }
            else
            {
                currentBackground = null;
            }
            gdi.Graphics.Clear(Color.FromArgb(1, 0, 0, 0));
            gdi.Graphics.DrawImage(e.cover, (768 - 440) / 2, 0, 440, 440);
            runOnUiThread(() =>
            {
                gdi.UpdateWindow();
            });
            crossfadeCountdown = 1;
            e.cover.Dispose();
            currentCover?.Dispose();
            currentCover = bmp;
            runOnUiThread(() =>
            {
                this.Icon = IconMaker.toIcon(currentCover);
            });
            discangel = 0;
        }

        private void Player_Stopped(object sender, EventArgs e)
        {
            runOnUiThread(() => controller.onSongStop());

        }

        Bitmap currentCover = cropToCircle(new Bitmap(Properties.Resources.default_cover, new Size(228, 228)));


        static Bitmap cropToCircle(Bitmap bmp)
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                float r = bmp.Width * (float)((Math.Sqrt(2) - 1)) / 1.4142136f;

                float r2 = 34.5f;

                Pen p = new Pen(Brushes.Transparent, r);
                g.DrawEllipse(p, 0 - r2, 0 - r2, bmp.Width + r2 + r2 - 1, bmp.Height + r2 + r2 - 1);
            }
            return bmp;
        }

        private void Form1_Disposed(object sender, EventArgs e)
        {
            player.Dispose();
        }

        GdiSystem gdi;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.icon;
            gdi = new GdiSystem(this);
            resizeBitmap();
            initLayers();
            layersInited();
            gdi.Graphics.Clear(Color.FromArgb(1, 0, 0, 0));
            gdi.Graphics.DrawImage(Properties.Resources.default_cover, (768 - 440) / 2, 0, 440, 440);
            gdi.UpdateWindow();
            renderTimer.Start();
            infoTimer.Start();
            if (controller.CurrentPlaying != null)
            {
                controller.ImReady();
                player.Pause();
                controller.LoopMode = Properties.Settings.Default.playmode;
                controller.Shuffe = Properties.Settings.Default.shuffemode;
                player.BassBoost = Properties.Settings.Default.bassboost;
                int dspType = Properties.Settings.Default.dsptype;
                this.dsptype = dspType;
                if(dspType == 1)
                {
                    ((DspSwitcher)player.SurroundSound).WrappedDSP = new SurroundDSP();
                }
                if (dspType == 2)
                {
                    ((DspSwitcher)player.SurroundSound).WrappedDSP = new SpeakerInRoomDSP();
                }
                SpectrumMode = Properties.Settings.Default.spectrummode;
                try
                {
                    player.LoadFx(Properties.Settings.Default.fxfile);
                }
                catch (Exception)
                {
                }
            }
            RegisterHotKey(this.Handle, 2, 0, Keys.MediaPreviousTrack);
            RegisterHotKey(this.Handle, 3, 0, Keys.MediaPlayPause);
            RegisterHotKey(this.Handle, 4, 0, Keys.MediaNextTrack);
            initLyrics();
        }

        void resizeBitmap()
        {
            frame = new Bitmap(frame, this.Size);
            defaultBackground = new Bitmap(defaultBackground, this.Size);
            glowBall = new Bitmap(glowBall, locButtonBlur.Size);
            progressThumb = new Bitmap(progressThumb, locButtonBlur.Size);
            ic_btnPause = new Bitmap(ic_btnPause, btnPlay.Size);
            ic_btnPlay = new Bitmap(ic_btnPlay, btnPlay.Size);
            coverGlowing = new Bitmap(coverGlowing, locGlowing.Size);
        }

        Bitmap frame = new Bitmap(Properties.Resources.window_frame);
        Bitmap defaultBackground = new Bitmap(Properties.Resources.default_background);
        Bitmap glowBall = new Bitmap(Properties.Resources.glow_ball);
        Bitmap currentBackground = null;
        Bitmap coverGlowing = new Bitmap(Properties.Resources.bg_glowing);

        


        GraphicsLayer shadowLayer, backgroundLayer, gfxLayer, frameLayer, bottomControlLayer, topTextLayer, spectrumLayer, discDisplayBackLayer, discDisplayLayer;
        GraphicsLayer siderBackgroundLayer, siderForegroundLayer;
        GraphicsLayer listLayer;
        GraphicsLayer lyricLayer;

        internal List<GraphicsLayer> layers = new List<GraphicsLayer>();

        public void initLayers()
        {
            shadowLayer = new GraphicsLayer(this, new Control() { Location = new Point(-16, -8), Size = new Size(800, 470) }) { Text = "窗口阴影层" };
            backgroundLayer = new GraphicsLayer(this, new Control() { Location = new Point(0, 0), Size = this.Size }) { Text = "背景层" };
            spectrumLayer = new GraphicsLayer(this, locSpectrumArea) { Text = "频谱显示层" };
            siderBackgroundLayer = new GraphicsLayer(this, tblVolumn) { Text = "右控制器背景" };
            gfxLayer = new GraphicsLayer(this, new Control() { Location = new Point(0, 0), Size = this.Size }) { Text = "控件特效层" };
            frameLayer = new GraphicsLayer(this, new Control() { Location = new Point(0, 0), Size = this.Size }) { Text = "框架层" };
            siderForegroundLayer = new GraphicsLayer(this, tblVolumn) { Text = "右控制器前景" };

            discDisplayBackLayer = new GraphicsLayer(this, locGlowing) { Text = "唱片发光特效层" };
            discDisplayLayer = new GraphicsLayer(this, locMask) { Text = "唱片层" };
            lyricLayer = new GraphicsLayer(this, locLyric) { Text = "歌词层" };
            listLayer = new GraphicsLayer(this, tblList) { Text = "列表层" };

            bottomControlLayer = new GraphicsLayer(this, tblBottomControl) { Text = "底部控制器" };
            topTextLayer = new GraphicsLayer(this, tblTopInfo) { Text = "SpyXX好玩吗" };
            for (int i = 0; i < layers.Count; i++)
            {
                Form parent = i == 0 ? (Form)this : layers[i - 1];

                //layers[i].ShowInTaskbar = true;
                layers[i].Show(parent);
            }
        }

        public void layersInited()
        {
            shadowLayer.g.DrawImage(Properties.Resources.bg_layer, 0, 0, shadowLayer.Width, shadowLayer.Height);
            backgroundLayer.g.DrawImage(defaultBackground, 0, 0, Width + 20, Height + 20);
            frameLayer.g.DrawImage(frame, 0, 0, backgroundLayer.Width + 1, backgroundLayer.Height);
            bottomControlLayer.g.Clear(Color.Transparent);
            spectrumLayer.g.Clear(Color.Transparent);
            topTextLayer.g.Clear(Color.Transparent);
            discDisplayLayer.g.Clear(Color.Transparent);
            discDisplayLayer.g.Clear(Color.Transparent);
            listLayer.g.Clear(Color.Transparent);
            lyricLayer.g.Clear(Color.Transparent);
            updateAll();
            updateAll2();
        }

        void updateBackground()
        {
            crossfadeCountdown -= 0.01f;
            if (crossfadeCountdown > 0.8)
            {
                DrawUtils.drawAlphaImage(backgroundLayer.g, currentBackground == null ? defaultBackground : currentBackground, 0, 0, this.Width + 20, this.Height + 20, 1 - crossfadeCountdown);
                backgroundLayer.UpdateWindow();
            }
            else
            {
                backgroundLayer.g.Clear(Color.Transparent);
                backgroundLayer.g.DrawImage(currentBackground == null ? defaultBackground : currentBackground, 0, 0, this.Width + 20, this.Height + 20);
                backgroundLayer.UpdateWindow();
                crossfadeCountdown = 0;
            }
        }

        public void updateAll() => layers.ForEach(l => l.UpdateWindow());
        public void updateAll2() => layers.ForEach(l => l.updatePosition());


        bool dragging = false;
        int dragX, dragY;
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragging = true;
                dragX = e.X; dragY = e.Y;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                this.Left += e.X - dragX;
                this.Top += e.Y - dragY;
                updateAll2();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragging = false;
            }
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            layers.ForEach(l => l.Visible = this.WindowState != FormWindowState.Minimized);
            renderTimer.Enabled = this.WindowState != FormWindowState.Minimized;
        }



        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void sfxAttaker(object sender, EventArgs e)
        {
            Control ctl = (Control)sender;
            Point loc = PointToClient(ctl.PointToScreen(Point.Empty));
            lightTargetX = loc.X + ctl.Width / 2;
            lightTargetY = loc.Y + ctl.Height / 2;
            if (alpha == 0)
            {
                lightMovementX = lightTargetX;
                lightMovementY = lightTargetY;
            }
            mouseInAnyControl = true;
        }


        private void sfxReleaser(object sender, EventArgs e)
        {
            mouseInAnyControl = false;
        }

        bool mouseInAnyControl = false;
        float lightLumianceOverlay = 0.0f;

        bool emphasis = false;

        private void sfxEmphasis(object sender, MouseEventArgs e)
        {
            emphasis = true;
        }

        private void sfxDeemphasis(object sender, MouseEventArgs e)
        {
            emphasis = false;
        }

        float emphasisStrength = 0f;

        float alpha = 0;
        float lightMovementX = 0;

        StringFormat alignLeft = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Near, FormatFlags = StringFormatFlags.NoWrap };
        StringFormat alignRight = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Far, FormatFlags = StringFormatFlags.NoWrap };

        private void infoTimer_Tick(object sender, EventArgs e)
        {
            updateBottomControl();
        }

        String timeToString(long ms)
        {
            long minutes = ms / 60000;
            long second = (ms % 60000) / 1000;
            return String.Format("{0:D2}:{1:D2}", minutes, second);
        }

        float lightMovementY = 0;
        float lightTargetX = 0;
        float lightTargetY = 0;

        private void btnPlay_Click(object sender, EventArgs e)
        {
            player.PlayOrPause();
        }

        float movementSmoothFactor = 0.17f;



        bool progressChanging = false;

        float crossfadeCountdown = 0;

        private void lblProgressManager_MouseDown(object sender, MouseEventArgs e)
        {
            progressChanging = true;
            infoTimer.Stop();
        }

        private void lblProgressManager_MouseMove(object sender, MouseEventArgs e)
        {
            if (progressChanging)
            {
                int targetvalue = (int)((float)numProgress.Maximum * e.X / lblProgressManager.Width);
                if (targetvalue > numProgress.Maximum) { targetvalue = numProgress.Value; }
                if (targetvalue < 0) { targetvalue = 0; }
                player.CurrentPosition = targetvalue;
                numProgress.Value = targetvalue;
                updateBottomControl();
            }
        }


        private void lblProgressManager_MouseUp(object sender, MouseEventArgs e)
        {
            if (progressChanging)
            {
                int targetvalue = (int)((float)numProgress.Maximum * e.X / lblProgressManager.Width);
                if (targetvalue > numProgress.Maximum) { targetvalue = numProgress.Value; }
                if (targetvalue < 0) { targetvalue = 0; }
                player.CurrentPosition = targetvalue;
                numProgress.Value = targetvalue;
            }
            progressChanging = false;
            infoTimer.Start();
        }

        public void computeSfx()
        {
            if (mouseInAnyControl)
            {
                alpha = Math.Min(3, alpha + 0.05f);
            }
            else
            {
                alpha = Math.Max(0, alpha - 0.009f);
            }
            if (emphasis)
            {
                emphasisStrength = Math.Min(0.3f, emphasisStrength + 0.05f);
            }
            else
            {
                emphasisStrength = Math.Max(0, emphasisStrength - 0.05f);
            }
            lightLumianceOverlay += 0.006f;

            float finalAlpha = Math.Min(1, alpha);
            if (lightLumianceOverlay < 0.5) { finalAlpha -= lightLumianceOverlay; }
            if (lightLumianceOverlay >= 0.5) { finalAlpha -= (1 - lightLumianceOverlay); }

            finalAlpha = finalAlpha * 0.7f + emphasisStrength;

            float dx = (lightTargetX - lightMovementX) * movementSmoothFactor;
            float dy = (lightTargetY - lightMovementY) * movementSmoothFactor;
            lightMovementX += dx;
            lightMovementY += dy;

            float movementMultiplier = 1f / (1f + (float)Math.Sqrt(dx * dx + dy * dy));

            while (lightLumianceOverlay > 1) { lightLumianceOverlay -= 1; }
            Graphics g = gfxLayer.g;
            g.Clear(Color.Transparent);
            DrawUtils.drawAlphaImage(g, glowBall, lightMovementX - locButtonBlur.Width / 2, lightMovementY - locButtonBlur.Height / 2, locButtonBlur.Width, locButtonBlur.Height, finalAlpha * movementMultiplier);

            gfxLayer.UpdateWindow();

        }

        Brush white = Brushes.White;

        Bitmap progressThumb = new Bitmap(Properties.Resources.glowing_drag_thumb);
        Bitmap ic_btnPlay = new Bitmap(Properties.Resources.ic_play);



        Bitmap ic_btnPause = new Bitmap(Properties.Resources.ic_pause);
        void updateBottomControl()
        {
            numProgress.Maximum = (int)player.TotalPosition;
            
            numProgress.Value = (int)(player.CurrentPosition >= numProgress.Maximum ? numProgress.Maximum : player.CurrentPosition);


            lock (white)
            {
                Graphics g = bottomControlLayer.g;
                g.Clear(Color.Transparent);
                g.DrawImage(player.IsPlaying ? ic_btnPause : ic_btnPlay, btnPlay.Location);
                g.DrawString(timeToString(player.CurrentPosition), lblCurrentTime.Font, white, new Rectangle(lblCurrentTime.Location, lblCurrentTime.Size), alignRight);
                g.DrawString(timeToString(player.TotalPosition), lblTotalTime.Font, white, new Rectangle(lblTotalTime.Location, lblTotalTime.Size), alignLeft);

                g.DrawImage(progressThumb, (float)this.Width * numProgress.Value / numProgress.Maximum - locButtonBlur.Width / 2, locProgress.Top - locButtonBlur.Height / 2);

                float finalAlpha = 1;
                if (lightLumianceOverlay < 0.5) { finalAlpha -= lightLumianceOverlay; }
                if (lightLumianceOverlay >= 0.5) { finalAlpha -= (1 - lightLumianceOverlay); }

                DrawUtils.drawAlphaImage(g, progressThumb, (float)this.Width * numProgress.Value / numProgress.Maximum - locButtonBlur.Width / 2, locProgress.Top - locButtonBlur.Height / 2, locButtonBlur.Width, locButtonBlur.Height, finalAlpha);

                bottomControlLayer.UpdateWindow();
            }
        }

        private void btnVolume_Click(object sender, EventArgs e)
        {
            if (!tblVolumn.Visible)
            {
                tblVolumn.Visible = true;
                tblUtils.Visible = false;
                sliderAnimationFadein = true;
                sliderAnimationCountdown = 1;
            }
            else
            {
                sliderAnimationFadein = false;
                sliderAnimationCountdown = 1;
            }
        }

        private void btnMore_Click(object sender, EventArgs e)
        {
            if (!tblUtils.Visible)
            {
                tblUtils.Visible = true;
                tblVolumn.Visible = false;
                sliderAnimationFadein = true;
                sliderAnimationCountdown = 1;
            }
            else
            {
                sliderAnimationFadein = false;
                sliderAnimationCountdown = 1;
            }
        }


        private int _spectrumMode = 1;
        int SpectrumMode
        {
            get
            {
                return _spectrumMode;
            }
            set
            {
                _spectrumMode = value;
                if (value == 1 || value == 3)
                {
                    spectrumLayer.changePosition(locSpectrumArea.Location);
                }
                if (value == 2 || value==4)
                {
                    spectrumLayer.changePosition(new Point(0, locSpectrum.Top - 64));
                }
            }
        }
        float[] processedFft = new float[200];

        float[] targetFft = new float[200];

        float fftSmoothFactor = 0.4f;

        Pen fftline = createFFTLines1();
        Pen fftline4 = createFFTLines2();
        Pen fftline2 = new Pen(new SolidBrush(Color.FromArgb(192, 255, 255, 255)), 2f) { StartCap = System.Drawing.Drawing2D.LineCap.Round, EndCap = System.Drawing.Drawing2D.LineCap.Round };
        Pen fftline3 = new Pen(new SolidBrush(Color.FromArgb(96, 255, 255, 255)), 1f) { StartCap = System.Drawing.Drawing2D.LineCap.Round, EndCap = System.Drawing.Drawing2D.LineCap.Round };

        private static Pen createFFTLines1() {
            ColorBlend cb = new ColorBlend(3);
            cb.Colors[0] = Color.Transparent;
            cb.Colors[1] = Color.White;
            cb.Colors[2] = Color.White;
            cb.Positions[0] = 0;
            cb.Positions[1] = 0.8f;
            cb.Positions[2] = 1;
            LinearGradientBrush brush = new LinearGradientBrush(new Point(0, -1), new Point(0, 128), Color.Transparent, Color.White);
            brush.InterpolationColors = cb;
            return new Pen(brush,2f) { StartCap = LineCap.Round };
        }
        private static Pen createFFTLines2()
        {
            ColorBlend cb = new ColorBlend(3);
            cb.Colors[0] = Color.Transparent;
            cb.Colors[1] = Color.White;
            cb.Colors[2] = Color.Transparent;
            cb.Positions[0] = 0;
            cb.Positions[1] = 0.5f;
            cb.Positions[2] = 1;
            LinearGradientBrush brush = new LinearGradientBrush(new Point(0, -1), new Point(0, 128), Color.Transparent, Color.White);
            brush.InterpolationColors = cb;
            return new Pen(brush, 2f) { StartCap = LineCap.Round,EndCap = LineCap.Round };
        }

        void updateSpectrum()
        {
            Graphics g = spectrumLayer.g;
            g.Clear(Color.Transparent);


            float[] data = player.Spectrum;
            int len = processedFft.Length;
            if (SpectrumMode != 0)
            {
                for (int i = 0; i < len; i++)
                {
                    float receivedValue = 0;

                    int span = i / 50;

                    int beginindex;
                    switch (span) {
                        case 0:
                            receivedValue = data[i];
                            break;
                        case 1:
                            beginindex = 2 * i - 50;//(i - 25) * 2 + 25;
                            receivedValue = (data[beginindex] + data[beginindex + 1]) / 2f;
                            break;
                        case 2:
                            beginindex = 3 * i - 150;//(i - 50) * 3 + 75;
                            receivedValue = (data[beginindex] + data[beginindex + 1] + data[beginindex + 2]) / 3;
                            break;
                        case 3:
                            beginindex = 4 * i - 300;//(i - 75) * 4 + 150;
                            receivedValue = (data[beginindex] + data[beginindex + 1] + data[beginindex + 2]+(data[beginindex + 2])) / 4;
                            break;
                    }


                    processedFft[i] = Math.Max(0f, Math.Max(processedFft[i] - 0.05f, dec2log(Math.Abs(receivedValue))));
                    if (!player.IsPlaying)
                    {
                        processedFft[i] = 0;
                    }
                    targetFft[i] += (processedFft[i] - targetFft[i]) * fftSmoothFactor;

                }
            }

            if (SpectrumMode == 1 || SpectrumMode == 3)
            {
                for (int i = 0; i < len; i++)
                {
                    float y = 2f + (locSpectrumArea.Width - 4f) / (len - 1) * i;
                    float x1 = locSpectrumArea.Height;
                    float x2 = x1 - targetFft[i] * locSpectrumArea.Height;
                    //fftline.TranslateTransform(x2, x2);
                    g.TranslateTransform(0, x2);
                    g.DrawLine(SpectrumMode==1 ? fftline2 : fftline, y, 0, y, x1);
                    g.TranslateTransform(0, -x2);
                    //fftline.TranslateTransform(-x2, -x2);
                }
            }
            if (SpectrumMode == 2 || SpectrumMode == 4)
            {
                for (int i = 0; i < len; i ++)
                {
                    float multiplier = SpectrumMode ==2 ? (1f - (i / 200f) * 0.94f) : (len - i < 64 ? (len - i) / 64f : 1);

                    float x = 0;
                    if (i % 2 == 0) {
                        x =  (locMask.Right - 32) + (Width - locMask.Right + 32) * (i / 2) / 100f;
                    } else
                    {
                        x = locMask.Left + 32 - (locMask.Left + 32) * ((i - 1) / 2) / 100f;
                    }
                    float y1 = 64 - targetFft[i] * multiplier * locSpectrumArea.Height / 2;
                    float y2 = 64 + targetFft[i] * multiplier * locSpectrumArea.Height / 2;
                    if (SpectrumMode == 2)
                    {
                        g.DrawLine(fftline2, x, y2, x, y1);
                    }
                    else
                    {
                        g.TranslateTransform(0, y1);
                        float scale = 1;
                        if(y2 - y1 < 0.5)
                        {
                            scale =1f / 1024f;
                        }
                        else
                        {
                            scale = (y2 - y1) /128f ;
                        }
                        g.ScaleTransform(1, scale);
                        g.DrawLine(fftline4, x, 0, x, 128);
                        g.ResetTransform();
                    }
                }
                //for (int i = 1; i < len; i += 2)
                //{

                //    float multiplier = 1f - ((i - 1) / 200f) * 0.94f;
                //    float x = locMask.Left + 32 - (locMask.Left + 32) * ((i - 1) / 2) / 100f;
                //    float y1 = 64 - targetFft[i] * multiplier * locSpectrumArea.Height / 2;
                //    float y2 = 64 + targetFft[i] * multiplier * locSpectrumArea.Height / 2;
                //    g.DrawLine(fftline2, x, y2, x, y1);
                //}
                g.DrawLine(fftline3, 0, 64, 768, 64);
            }
            spectrumLayer.UpdateWindow();
        }

        float dec2log(float x) => x == 0 ? 0 : Math.Max(0, (float)(Math.Log10(x * 10000) / 4));



        private Bitmap imgFavOn = new Bitmap(Properties.Resources.heart_fill,28,28);
        private Bitmap imgFavOff = new Bitmap(Properties.Resources.heart,28,28);

        void updateTopControl()
        {
            lock (white)
            {
                Graphics g = topTextLayer.g;
                g.Clear(Color.Transparent);
                g.DrawString(lblTitle.Text, lblTitle.Font, white, new Rectangle(lblTitle.Location, lblTitle.Size), alignLeft);
                g.DrawString(lblArtsit.Text, lblArtsit.Font, white, new Rectangle(lblArtsit.Location, lblArtsit.Size), alignLeft);
                if (imgHiResAudio.Enabled)
                {
                    g.DrawImage(imgHiResAudio.Image, new Rectangle(imgHiResAudio.Location, imgHiResAudio.Size));
                }
                
                g.DrawImage(isFavChecked ? imgFavOn : imgFavOff, new Rectangle(imgBass.Location, imgBass.Size));
                
                topTextLayer.UpdateWindow();
                
            }
        }

        bool adjustVolume = false;
        private void volumnDown(object sender, MouseEventArgs e)
        {
            adjustVolume = true;
            if (adjustVolume)
            {
                float targetVolume = 1f - (e.Y - locVolumnMaxPoint.Top) / (float)(locVolumeMinPoint.Top - locVolumnMaxPoint.Top);
                if (targetVolume > 1) { targetVolume = 1; }
                if (targetVolume < 0) { targetVolume = 0; }
                player.Volume = targetVolume * targetVolume;
            }
        }

        private void volumeMove(object sender, MouseEventArgs e)
        {
            if (adjustVolume)
            {
                float targetVolume = 1f - (e.Y - locVolumnMaxPoint.Top) / (float)(locVolumeMinPoint.Top - locVolumnMaxPoint.Top);
                if (targetVolume > 1) { targetVolume = 1; }
                if (targetVolume < 0) { targetVolume = 0; }
                player.Volume = targetVolume * targetVolume;
            }
        }

        private void volumeUp(object sender, MouseEventArgs e)
        {
            adjustVolume = false;
            if (adjustVolume)
            {
                float targetVolume = 1f - (e.Y - locVolumnMaxPoint.Top) / (float)(locVolumeMinPoint.Top - locVolumnMaxPoint.Top);
                if (targetVolume > 1) { targetVolume = 1; }
                if (targetVolume < 0) { targetVolume = 0; }
                player.Volume = targetVolume * targetVolume;
            }
        }


        float sliderAnimationCountdown = 0;
        bool sliderAnimationFadein = false;

        Bitmap sliderVolumnBg = new Bitmap(Properties.Resources.bg_volumeSlider, 60, 201);
        Bitmap sliderUtilBg = new Bitmap(Properties.Resources.bg_extendControl, 60, 201);
        Bitmap spectrumBottom = new Bitmap(Properties.Resources.ic_spectrum_bottom, 32, 32);
        Bitmap spectrumDisable = new Bitmap(Properties.Resources.ic_spectrum_disable, 32, 32);
        Bitmap spectrumCenter = new Bitmap(Properties.Resources.ic_spectrum_center, 32, 32);

        Bitmap srsOn = new Bitmap(Properties.Resources.sr_on, 32, 32);
        Bitmap rsrOn = new Bitmap(Properties.Resources.rs_on, 32, 32);
        Bitmap srsOff = new Bitmap(Properties.Resources.sr_off, 32, 32);

        int dsptype = 0;

        void updateSliderControl()
        {
            Graphics bg = siderBackgroundLayer.g;
            Graphics fg = siderForegroundLayer.g;
            if (tblVolumn.Visible)
            {
                bg.Clear(Color.Transparent);
                fg.Clear(Color.Transparent);
                if (sliderAnimationCountdown > 0)
                {
                    sliderAnimationCountdown -= 0.072f;
                    if (sliderAnimationCountdown < 0)
                    {
                        sliderAnimationCountdown = 0;
                        if (!sliderAnimationFadein)
                        {
                            tblVolumn.Visible = false;
                        }
                    }

                }
                float x = locVolumnMaxPoint.Left;
                float y = locVolumeMinPoint.Top - (locVolumeMinPoint.Top - locVolumnMaxPoint.Top) * (float)Math.Sqrt(player.Volume);
                if (sliderAnimationFadein)
                {
                    DrawUtils.drawAlphaImage(bg, sliderVolumnBg, 60 - deceleration(1 - sliderAnimationCountdown) * 60, 0, 60, 201, 1 - sliderAnimationCountdown);
                    DrawUtils.drawAlphaImage(fg, progressThumb, x - 20 + (60 - deceleration(1 - sliderAnimationCountdown) * 60), y - 20, 43, 43, 1 - sliderAnimationCountdown);
                }
                else
                {
                    DrawUtils.drawAlphaImage(bg, sliderVolumnBg, acceleration(1 - sliderAnimationCountdown) * 60, 0, 60, 201, sliderAnimationCountdown);
                    DrawUtils.drawAlphaImage(fg, progressThumb, x - 20 + (acceleration(1 - sliderAnimationCountdown) * 60), y - 20, 43, 43, sliderAnimationCountdown);
                }


                siderBackgroundLayer.UpdateWindow();
                siderForegroundLayer.UpdateWindow();
            }
            else if (tblUtils.Visible)
            {

                bg.Clear(Color.Transparent);
                fg.Clear(Color.Transparent);
                if (sliderAnimationCountdown > 0)
                {
                    sliderAnimationCountdown -= 0.072f;
                    if (sliderAnimationCountdown < 0)
                    {
                        sliderAnimationCountdown = 0;
                        if (!sliderAnimationFadein)
                        {
                            tblUtils.Visible = false;
                        }
                    }

                }

                float offsetx = 0;
                float alpha = 1;

                if (sliderAnimationFadein)
                {
                    offsetx = 60 - deceleration(1 - sliderAnimationCountdown) * 60;
                    alpha = 1 - sliderAnimationCountdown;
                }
                else
                {
                    offsetx = acceleration(1 - sliderAnimationCountdown) * 60;
                    alpha = sliderAnimationCountdown;
                }

                DrawUtils.drawAlphaImage(bg, sliderUtilBg, offsetx, 0, 60, 201, alpha);
                if (loopMode == 0) { DrawUtils.drawAlphaImage(fg, ic_loop_all, btnLoopMode.Left + offsetx, btnLoopMode.Top, btnLoopMode.Width, btnLoopMode.Height, alpha); }
                if (loopMode == 1) { DrawUtils.drawAlphaImage(fg, ic_loop_one, btnLoopMode.Left + offsetx, btnLoopMode.Top, btnLoopMode.Width, btnLoopMode.Height, alpha); }
                if (loopMode == 2) { DrawUtils.drawAlphaImage(fg, ic_loop_list, btnLoopMode.Left + offsetx, btnLoopMode.Top, btnLoopMode.Width, btnLoopMode.Height, alpha); }
                DrawUtils.drawAlphaImage(fg, shuffeMode ? ic_shuffe_on : ic_shuffe_off, btnShuffe.Left + offsetx, btnShuffe.Top, btnLoopMode.Width, btnLoopMode.Height, alpha);

                Bitmap spectrumMode = SpectrumMode == 0 ? spectrumDisable : (SpectrumMode == 1 ? spectrumBottom : spectrumCenter);

                DrawUtils.drawAlphaImage(fg, spectrumMode, btnShuffe.Left + offsetx, btnSpectrumMode.Top, btnLoopMode.Width, btnLoopMode.Height, alpha);

                if (dsptype == 0) { DrawUtils.drawAlphaImage(fg, srsOff , btnShuffe.Left + offsetx, btnSrs.Top, btnLoopMode.Width, btnLoopMode.Height, alpha); }
                if (dsptype == 1) { DrawUtils.drawAlphaImage(fg, srsOn, btnShuffe.Left + offsetx, btnSrs.Top, btnLoopMode.Width, btnLoopMode.Height, alpha); }
                if (dsptype == 2) { DrawUtils.drawAlphaImage(fg, rsrOn, btnShuffe.Left + offsetx, btnSrs.Top, btnLoopMode.Width, btnLoopMode.Height, alpha); }
                
                



                siderBackgroundLayer.UpdateWindow();
                siderForegroundLayer.UpdateWindow();
            }


        }

        public float deceleration(float input) => (float)(1.0f - (1.0f - input) * (1.0f - input));

        public float acceleration(float input) => input * input;

        public float easeOut(float input) {

            return (float)(1 - Math.Pow(1 - input, 5) * (2 - (1 - input)));

        }

        private void renderTimer_Tick(object sender, EventArgs e)
        {
            computeSfx();
            updateSpectrum();
            updateLyric();
            if (crossfadeCountdown > 0)
            {
                updateBackground();
            }


            updateSpinningDisc();
            updateSliderControl();
            if (tblList.Visible)
            {
                updateListLayer();
            }
        }

        Bitmap ic_loop_one = new Bitmap(Properties.Resources.ic_loop_one);//0

        private void btnLoopMode_Click(object sender, EventArgs e)
        {
            loopMode++;
            if (loopMode > 2) { loopMode = 0; }
            player.Looping = loopMode == 1;
        }

        private void btnShuffe_Click(object sender, EventArgs e)
        {
            shuffeMode = !shuffeMode;
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            controller.onPrevButtonClick();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            controller.onNextButtonClick();
        }

        private void btnPreserved1_Click(object sender, EventArgs e)
        {
            SpectrumMode++;
            if (SpectrumMode > 4) { SpectrumMode = 0; }
            //player.SkipFFT = SpectrumMode == 0;
        }

        Bitmap ic_loop_all = new Bitmap(Properties.Resources.ic_loop_all);//1
        Bitmap ic_loop_list = new Bitmap(Properties.Resources.ic_loop_list);//2

        int loopMode { get => controller.LoopMode; set => controller.LoopMode = value; }

        Bitmap ic_shuffe_on = new Bitmap(Properties.Resources.ic_shuffe);
        Bitmap ic_shuffe_off = new Bitmap(Properties.Resources.ic_shuffe_off);

        bool shuffeMode { get => controller.Shuffe; set => controller.Shuffe = value; }
        public bool Bassboosted { get => player.BassBoost; set { 
                player.BassBoost=value;
            }
        }



        Bitmap ic_commingsoon1 = new Bitmap(Properties.Resources.ic_commingsoon_1);
        Bitmap ic_commingsoon2 = new Bitmap(Properties.Resources.ic_commingsoon_2);

        float discangel = 0.0f;

        void updateSpinningDisc()
        {
            float finalAlpha = 1;
            if (lightLumianceOverlay < 0.5) { finalAlpha -= lightLumianceOverlay; }
            if (lightLumianceOverlay >= 0.5) { finalAlpha -= (1 - lightLumianceOverlay); }
            discDisplayBackLayer.g.Clear(Color.Transparent);
            DrawUtils.drawAlphaImage(discDisplayBackLayer.g, coverGlowing, 0, 0, locGlowing.Width, locGlowing.Height, finalAlpha);
            discDisplayBackLayer.UpdateWindow();

            if (crossfadeCountdown > 0)
            {
                DrawUtils.drawAlphaImage(discDisplayLayer.g, currentCover, -1f, -1f, discDisplayLayer.Width + 1, discDisplayLayer.Height + 1, 1 - crossfadeCountdown);
                discDisplayLayer.UpdateWindow();
                return;
            }

            if (player.IsPlaying) { discangel += 0.2f; }

            discDisplayLayer.g.Clear(Color.Transparent);
            DrawUtils.drawRotateImg(discDisplayLayer.g, currentCover, discangel, discDisplayLayer.Width / 2, discDisplayLayer.Height / 2);
            discDisplayLayer.UpdateWindow();
        }


        Bitmap listSurface = null;



        Graphics listDc = null;
        float listAnimationCountdown = 0;
        bool isExpanding = false;
        void updateListLayer()
        {
            if (null == listSurface)
            {
                listSurface = new Bitmap(tblList.Width, tblList.Height);
                listDc = Graphics.FromImage(listSurface);
                listDc.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                listDc.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                tblList.MouseWheel += TblList_MouseWheel;
            }
            Graphics g = listLayer.g;
            g.Clear(Color.Transparent);
            drawList();

            float offsetx = 0;
            if (listAnimationCountdown > 0) {

                if (isExpanding)
                {
                    offsetx = -tblList.Width * (1f-easeOut(1-listAnimationCountdown));
                    listAnimationCountdown -= 0.035f;
                }
                else {
                    offsetx = -tblList.Width * acceleration(1-listAnimationCountdown);
                    listAnimationCountdown -= 0.07f;
                }

                g.DrawImage(listSurface, 0 + offsetx, 0f);
            }
            else {
                offsetx = 0;
                if (!isExpanding)
                {
                    tblList.Visible = false;
                    listHidedAction?.Invoke();
                }
                else {

                    g.DrawImage(listSurface, 0 , 0f);
                }
            }

            listLayer.UpdateWindow();
        }

        private void TblList_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
            {
                velotry = 0.2f;
            }
            else {
                velotry = -0.2f;
            }
        }

        bool isScrollBar = false;
        bool hasClickEvent = false;
        float beginClickX, beginClickY;

        bool clicked = false;

        private void listMouseDown(object sender, MouseEventArgs e)
        {
            clicked = true;
            isScrollBar = e.X > tblList.Width - scrollbarSize;
            hasClickEvent = false;
            beginClickX = e.X;
            beginClickY = e.Y;
            nowX = beginClickX;
            nowY = beginClickY;
            velotry = 0;
        }

        float nowX, nowY;
        float dragspeed = 0;
        float requestDraggingDistance = 0;
        float requestDraggingVelotry = 0;
        float requestScrollbar = 0;
        private void listMouseMove(object sender, MouseEventArgs e)
        {
            if (clicked)
            {
                float x = e.X;
                float y = e.Y;

                dragspeed = y - nowY;
                if (!isScrollBar)
                {
                    requestDraggingDistance -= dragspeed;
                }
                else
                {
                    requestScrollbar += dragspeed;
                }
                nowY = y; nowX = x;
            }
        }

        

        float lastClickX, lastClickY;
        private void listMouseUp(object sender, MouseEventArgs e)
        {
           
            float x = e.X;
            float y = e.Y;
            if (Math.Sqrt(Math.Pow(x - beginClickX, 2) + Math.Pow(y - beginClickY, 2)) > 5)
            {
                if (!isScrollBar)
                {
                    requestDraggingVelotry = -dragspeed;
                }
            }
            else {
                lastClickX = x;lastClickY = y;
                hasClickEvent = true;
            }
            clicked = false;
        }

        float velotry = 0f;
        float position = 0f;

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            controller.PlayListProvider.saveProgress();
            Properties.Settings.Default.playmode = controller.LoopMode;
            Properties.Settings.Default.shuffemode = controller.Shuffe;
            Properties.Settings.Default.bassboost = player.BassBoost;
            Properties.Settings.Default.spectrummode = SpectrumMode;
            Properties.Settings.Default.dsptype = this.dsptype;
            Properties.Settings.Default.Save();
            UnregisterHotKey(this.Handle, 2);
            UnregisterHotKey(this.Handle, 3);
            UnregisterHotKey(this.Handle, 4);
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            if (tblVolumn.Visible) { btnVolume_Click(sender, e); }
            if (tblUtils.Visible) { btnMore_Click(sender, e); }
            if (tblList.Visible) { hideList(null); }
        }

        private void btnPreserved2_Click(object sender, EventArgs e)
        {
            if (tblList.Visible)
            {
                if (currentlist is FxAdapter)
                {
                    hideList(null);
                }
                else
                {
                    hideList(() =>
                    {
                        showList(new FxAdapter(this));
                        position = controller.AllPlayList.IndexOf(controller.CurrentList) - 5;
                        velotry = 0.01f;
                    });
                }
            }
            else
            {
                showList(new FxAdapter(this));
                position = controller.AllPlayList.IndexOf(controller.CurrentList) - 5;
                velotry = 0.01f;
            }
        }

        float scrollbarSize = 16;

        Pen listDividerLine = new Pen(new SolidBrush(Color.FromArgb(128, 255, 255, 255)), 1);
        Brush scrollbar = new SolidBrush(Color.FromArgb(128, 255, 255, 255));

        void drawList()
        {
            Graphics g = listDc;
            g.Clear(Color.FromArgb(192, 0, 0, 0));
            if (currentlist != null) {
                IListAdapter list = currentlist;
                float itemheight = list.getItemHeight();
                float totalItems = list.getCount();
                velotry *= 0.97f;
                float maxPosition = totalItems - tblList.Height / itemheight;
                
                position += velotry;
                if (requestDraggingDistance != 0)
                {
                    position += requestDraggingDistance / itemheight;
                    requestDraggingDistance = 0;
                }
                if (requestDraggingVelotry != 0) {
                    velotry = requestDraggingVelotry / itemheight;
                    requestDraggingVelotry = 0;
                }
                if (requestScrollbar != 0 && maxPosition>0) {

                    float scrollbarLength = tblList.Height * ((tblList.Height / itemheight) / totalItems);

                    position += maxPosition / (tblList.Height - scrollbarLength) * requestScrollbar;
                    requestScrollbar = 0;
                }


                if (position < 0) { position = 0; if (velotry < 0) { velotry = 0; }}
                if (position >Math.Max(0,maxPosition)) { position = Math.Max(0, maxPosition); velotry = 0; }

                int beginItem = (int)position / 1;
                int endItem =  Math.Min(list.getCount() - 1, beginItem + (int)(tblList.Height / itemheight + 2));

                float beginoffset = -(1- position + beginItem);
                for (int i = beginItem; i <= endItem; i++)
                {
                    float bx = 0, by = ((i-beginItem-1) - beginoffset)* itemheight + beginoffset, bw = tblList.Width - scrollbarSize, bh = itemheight;
                    bool hasClick = false;
                    if (hasClickEvent && new RectangleF(bx, by, bw, bh).Contains(lastClickX, lastClickY)) {
                        hasClick = true;
                        hasClickEvent = false;
                    }
                    list.drawItem(g, i, bx, by, bw, bh, hasClick);
                    g.DrawLine(listDividerLine, 0, by + bh, tblList.Width, by + bh);
                }
                if (maxPosition > 0)
                {
                    float scrollbarLength = tblList.Height * ((tblList.Height / itemheight) / totalItems);
                    float scrollbarPosition = (tblList.Height - scrollbarLength) * (position / maxPosition);
                    g.FillRectangle(scrollbar,tblList.Width - scrollbarSize, 0, scrollbarSize, tblList.Height);
                    g.FillRectangle(scrollbar,tblList.Width - scrollbarSize, scrollbarPosition, scrollbarSize, scrollbarLength);
                }
            }
        }


        private void btnSrs_Click(object sender, EventArgs e)
        {
            dsptype++;
            if (dsptype > 2) { dsptype = 0; }
            if (dsptype == 0)
            {
                ((DspSwitcher)player.SurroundSound).WrappedDSP = null;
            }
            if (dsptype == 1)
            {
                ((DspSwitcher)player.SurroundSound).WrappedDSP = new SurroundDSP();
            }
            if (dsptype == 2)
            {
                ((DspSwitcher)player.SurroundSound).WrappedDSP = new SpeakerInRoomDSP();
                if (Properties.Settings.Default.fxfile.ToLower().EndsWith(".wav"))
                {
                    Properties.Settings.Default.fxfile = null;
                    Properties.Settings.Default.Save();
                }
            }

            
        }


        public bool isFavChecked = false;
        void checkFavStatus() {
            isFavChecked = favourite.Songs.Contains(controller.CurrentPlaying);
        }

        private VirtualDir favourite = new VirtualDir("收藏",Path.GetFullPath("收藏.pl"),Path.GetFullPath("."));

        

        private void btnFav_Click(object sender, EventArgs e)
        {
            if (!isFavChecked)
            {
                List<SongEntry> refSongs = favourite.Songs;
                refSongs.Add(controller.CurrentPlaying);
                favourite.save();

                if (controller.PlayListProvider is CurrentDirectorySongProvider) {
                    ((CurrentDirectorySongProvider)controller.PlayListProvider).UpdatePlaylist();
                }
            }
            else {
                List<SongEntry> refSongs = favourite.Songs;
                refSongs.Remove(controller.CurrentPlaying);
                favourite.save();

                if (controller.PlayListProvider is CurrentDirectorySongProvider)
                {
                    ((CurrentDirectorySongProvider)controller.PlayListProvider).UpdatePlaylist();
                }
            }
            checkFavStatus();
            updateTopControl();
        }

        IListAdapter currentlist = null;

        interface IListAdapter {
            float getItemHeight();
            int getCount();
            object getItem(int position);
            void drawItem(Graphics g,int position, float bx, float by, float w, float h,bool clicked);
        }

        class FolderAdapter : IListAdapter
        {
            Form1 _this;

            public FolderAdapter(Form1 @this)
            {
                _this = @this;
            }

            Brush white = Brushes.White;
            Brush yellow = Brushes.Yellow;

            Bitmap playlistImg = new Bitmap(Properties.Resources.img_folder,26,26);
            Bitmap playlistImg2 = new Bitmap(Properties.Resources.ic_playlist,26,26);

            public void drawItem(Graphics g, int position, float bx, float by, float w, float h, bool clicked)
            {
                RectangleF textRect = new RectangleF(bx + 36, by, w - 36, h);
                PlayList pl = (PlayList)getItem(position);
                g.DrawString(pl.Name, _this.lblArtsit.Font, pl==_this.controller.CurrentList ? yellow : white, textRect, _this.alignLeft);
                g.DrawImage(pl is VirtualDir ? playlistImg2 : playlistImg, 3, 3+by);
                if (clicked) {
                    _this.hideList(() => _this.showList(new SongAdapter(_this, pl)));
                }
            }

            public int getCount()
            {
                return _this.controller.AllPlayList.Count;
            }

            public object getItem(int position)
            {
                return _this.controller.AllPlayList[position];
            }

            public float getItemHeight()
            {
                return 32;
            }
        }

        class FxAdapter : IListAdapter
        {
            Form1 _this;
            private List<string> fxfiles = new List<string>();
            public FxAdapter(Form1 @this)
            {
                _this = @this;
                fxfiles.Add("");
                if (Directory.Exists(".musicfx"))
                {
                    Directory.EnumerateFiles(".musicfx","*.wav").ToList().ForEach(f=>fxfiles.Add(Path.GetFileName(f)));
                    Directory.EnumerateFiles(".musicfx","*.eq").ToList().ForEach(f=>fxfiles.Add(Path.GetFileName(f)));
                }
            }

            Brush white = Brushes.White;
            Brush yellow = Brushes.Yellow;

            Bitmap playlistImg = new Bitmap(Properties.Resources.ic_spectrum_bottom,26,26);

            public void drawItem(Graphics g, int position, float bx, float by, float w, float h, bool clicked)
            {
                RectangleF textRect = new RectangleF(bx + 36, by, w - 36, h);
                string item = fxfiles[position];
                string displayName = String.IsNullOrEmpty(item) ? "<无音效>" : item;
                g.DrawString(displayName, _this.lblArtsit.Font,white, textRect, _this.alignLeft);
                g.DrawImage(playlistImg, 3, 3+by);
                if (clicked) {
                    _this.hideList(() =>
                    {
                        string fxpath = fxfiles[position];
                        if (fxpath != null && fxpath!="")
                        {
                            fxpath = Path.Combine(".musicfx", fxpath);
                            
                        }
                        try
                        {
                            _this.player.LoadFx(fxpath);
                            Properties.Settings.Default.fxfile = fxpath;
                            if (fxpath.ToLower().EndsWith(".wav"))
                            {
                                Properties.Settings.Default.dsptype = 0;
                                _this.dsptype = 0;
                            }
                            Properties.Settings.Default.Save();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message,"加载FX失败");
                        }
                    });
                }
            }

            public int getCount()
            {
                return fxfiles.Count;
            }

            public object getItem(int position)
            {
                return fxfiles[position];
            }

            public float getItemHeight()
            {
                return 32;
            }
        }
        
        class SongAdapter : IListAdapter
        {
            Form1 _this;
            PlayList usingList;
            private bool showshuffed = false;
            public SongAdapter(Form1 @this,PlayList pl)
            {
                _this = @this;
                usingList = pl;
            }

            Brush white = Brushes.White;
            Brush yellow = Brushes.Yellow;

            Bitmap playlistImg = new Bitmap(Properties.Resources.ic_song, 30, 30);

            public void drawItem(Graphics g, int position, float bx, float by, float w, float h, bool clicked)
            {
                RectangleF textRect = new RectangleF(bx + 36, by, w - 36, h);
                SongEntry pl = (SongEntry)getItem(position);
                g.DrawString(pl.Name, _this.lblArtsit.Font, pl == _this.controller.CurrentPlaying ? yellow : white, textRect, _this.alignLeft);
                g.DrawImage(playlistImg, 1, 1 + by);
                if (clicked)
                {
                    _this.controller.CurrentList = usingList;
                    _this.controller.CurrentPlaying = pl;
                    _this.controller.ImReady();
                }
            }

            public int getCount()
            {
                return usingList.Songs.Count;
            }

            public object getItem(int position)
            {
                
                return usingList.Songs[position];
            }

            public float getItemHeight()
            {
                return 32;
            }
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            if (tblList.Visible)
            {
                if (currentlist is FolderAdapter)
                {
                    hideList(null);
                }
                else
                {
                    hideList(() =>
                    {
                        showList(new FolderAdapter(this));
                        position = controller.AllPlayList.IndexOf(controller.CurrentList) - 5;
                        velotry = 0.01f;
                    });
                }
            }
            else
            {
                showList(new FolderAdapter(this));
                position = controller.AllPlayList.IndexOf(controller.CurrentList) - 5;
                velotry = 0.01f;
            }
        }

        void showList(IListAdapter list) {
            if (!this.tblList.Visible)
            {
                tblList.Visible = true;
                isExpanding = true;
                listAnimationCountdown = 1;
            }
            currentlist = list;
            position = 0;
        }

        Action listHidedAction = null;
        void hideList(Action hidedAction) {
            isExpanding = false;
            listAnimationCountdown = 1;
            listHidedAction = hidedAction;
        }

        private void btnPlayList_Click(object sender, EventArgs e)
        {
            if (tblList.Visible)
            {
                if (currentlist is SongAdapter)
                {
                    hideList(null);
                }
                else {
                    hideList(() =>
                    {
                        actualShowSongList();

                    });
                }
            }
            else
            {
                actualShowSongList();
            }
        }

        private void actualShowSongList() {
            showList(new SongAdapter(this, controller.CurrentList));
            position = controller.CurrentList.Songs.IndexOf(controller.CurrentPlaying) - 5;
            velotry = 0.01f;
        }

        private void locSpectrumArea_Click(object sender, EventArgs e)
        {

        }

        private void tblBottomControl_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblTitle_Click(object sender, EventArgs e)
        {

        }

        private void locMask_Click(object sender, EventArgs e)
        {

        }

        private void tblUtils_Paint(object sender, PaintEventArgs e)
        {

        }


        private void initLyrics()
        {
            lyricBackgroundBitmap = new Bitmap(Properties.Resources.bg_lyric,locLyric.Width,locLyric.Height);
            lyricComposedBitmap = new Bitmap(locLyric.Width,locLyric.Height);
            lyricComposeShadowBitmap = new Bitmap(locLyric.Width / 2,locLyric.Height / 2);
            lyricComposeTargetBitmap = new Bitmap(locLyric.Width, locLyric.Height);
            lyricCenter = new StringFormat() { Alignment = StringAlignment.Center,LineAlignment = StringAlignment.Center };
            lyricRect = new RectangleF(-locLyric.Width, 0, locLyric.Width * 3, locLyric.Height);
            lyricShadowBlurer = new TextShadow();
            lyricShadowBlurer.Radius = 3;
            lyricShadowBlurer.Distance = 0;
            if (File.Exists("lrc.ttf"))
            {
                try
                {
                    lyricPrivateFont = new PrivateFontCollection();
                    lyricPrivateFont.AddFontFile(Path.GetFullPath("lrc.ttf"));
                    Font f = new Font(lyricPrivateFont.Families[0], locLyric.Font.Size, FontStyle.Bold);
                    locLyric.Font = f;
                }catch (Exception) {
                }  
            }
        }

        private LyricManager lyricManager = null;
        private PrivateFontCollection lyricPrivateFont;
        private Bitmap lyricBackgroundBitmap;
        private Bitmap lyricComposedBitmap;
        private Bitmap lyricComposeShadowBitmap;
        private Bitmap lyricComposeTargetBitmap;
        private TextShadow lyricShadowBlurer;
        Brush lyricFontColorShadow = Brushes.Black;
        Brush lyricFontColor = Brushes.White;

        private StringFormat lyricCenter;

        private void btnToggleCompressor_Click(object sender, EventArgs e)
        {
            player.DynamicRangeCompressed = ! player.DynamicRangeCompressed;
        }

        private RectangleF lyricRect;


        private LyricManager.LyricEntry currentLyricEntry = null;

        void updateLyric()
        {
            LyricManager.LyricEntry newLyric = null;
            if (lyricManager != null)
            {
                newLyric = lyricManager.GetLyric(TimeSpan.FromMilliseconds(player.CurrentPosition));
            }
            if(newLyric != currentLyricEntry)
            {
                currentLyricEntry = newLyric;

                updateLyricCd = 32;
                inUpdateLyric = true;
            }

            lyricLayer.g.Clear(Color.Transparent);
            if (inUpdateLyric)
            {
                animatedUpdateLyric();
            }
            else
            {
                lyricAlpha -= 1f / 128f;
                if(lyricAlpha < 0f) { lyricAlpha = 0f; }
                if(lyricAlpha > 1f)
                {
                    if (currentShowingLyricBackground)
                    {
                        lyricLayer.g.DrawImage(lyricBackgroundBitmap, 0, 0);
                    }
                    lyricLayer.g.DrawImage(lyricComposeTargetBitmap, 0, 0);
                }
                else
                {
                    if (currentShowingLyricBackground)
                    {
                        DrawUtils.drawAlphaImage(lyricLayer.g, lyricBackgroundBitmap, 0, 0, locLyric.Width, locLyric.Height, lyricAlpha);
                    }
                    
                    DrawUtils.drawAlphaImage(lyricLayer.g, lyricComposeTargetBitmap, 0, 0, locLyric.Width, locLyric.Height, lyricAlpha);
                }
            }
            lyricLayer.UpdateWindow();
        }

        private float lyricAlpha = 10;

        private int updateLyricCd = 0;
        private bool inUpdateLyric = false;

        private bool currentShowingLyricBackground = false;

        void animatedUpdateLyric()
        {
            if(updateLyricCd == 32)
            {
                preRenderLyric();
            }
            if (updateLyricCd == 16)
            {
                postRenderLyric();
            }
            float alpha = 0.0f;

            float bgAlpha = currentShowingLyricBackground ? 1.0f : 0.0f;

            if(currentShowingLyricBackground && (currentLyricEntry != null && !currentLyricEntry.isEmpty)) {
                bgAlpha = 1.0f;
            }
            else if((!currentShowingLyricBackground) && (currentLyricEntry == null || currentLyricEntry.isEmpty))
            {
                bgAlpha = 0.0f;
            }
            else
            {
                if (currentShowingLyricBackground)
                {
                    if(updateLyricCd >= 16)
                    {
                        bgAlpha = 1.0f;
                    }
                    if(updateLyricCd < 16)
                    {
                        bgAlpha = (updateLyricCd * 1.0f) / 16f;
                    }
                }
                else
                {
                    if (updateLyricCd >= 16)
                    {
                        bgAlpha = 0.0f;
                    }
                    if (updateLyricCd < 16)
                    {
                        bgAlpha = 1f - (updateLyricCd * 1.0f) / 16f;
                    }
                }
            }
            float postAlpha = lyricAlpha > 1.0f ? 1.0f : lyricAlpha;
            if(updateLyricCd > 16)
            {
                alpha = (float)(updateLyricCd - 16f) / 16f * postAlpha;
                if(postAlpha < 1.0f)
                {
                    bgAlpha *= postAlpha + (1f - postAlpha) * (1f - (updateLyricCd - 16f) / 16f);
                }
            }
            if(updateLyricCd < 16 && (currentLyricEntry != null && !currentLyricEntry.isEmpty))
            {
                alpha = (float)(16f - updateLyricCd) / 16f;
                
            }
            if(bgAlpha < alpha) { bgAlpha = alpha; }
            
            DrawUtils.drawAlphaImage(lyricLayer.g, lyricBackgroundBitmap, 0, 0, locLyric.Width, locLyric.Height, bgAlpha);
            DrawUtils.drawAlphaImage(lyricLayer.g, lyricComposeTargetBitmap, 0, 0, locLyric.Width, locLyric.Height, alpha);

            updateLyricCd--;
            if(updateLyricCd < 0)
            {
                lyricAlpha = currentLyricEntry == null ? 3f : currentLyricEntry.text.Length * 0.7f;
                currentShowingLyricBackground = currentLyricEntry != null && !currentLyricEntry.isEmpty;
                inUpdateLyric = false;
            }
        }

        void preRenderLyric()
        {
            using(Graphics bg = Graphics.FromImage(lyricComposeShadowBitmap))
            {
                bg.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                bg.CompositingQuality= CompositingQuality.HighQuality;

                bg.ScaleTransform(0.5f, 0.5f);
                bg.Clear(Color.Transparent);
                using(Graphics tg = Graphics.FromImage(lyricComposedBitmap))
                {
                    tg.Clear(Color.Transparent);
                    tg.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                    var lyric = this.currentLyricEntry;
                    if(lyric == null || lyric.isEmpty) { return; }

                    if (lyric.hasTranslation)
                    {
                        bg.TranslateTransform(0, -14);
                        tg.TranslateTransform(0, -14);
                        bg.DrawString(lyric.translatedText, locLyric.Font, lyricFontColorShadow, lyricRect, lyricCenter);
                        tg.DrawString(lyric.translatedText, locLyric.Font, lyricFontColor, lyricRect, lyricCenter);
                        bg.TranslateTransform(0, 28);
                        tg.TranslateTransform(0, 28);
                        bg.DrawString(lyric.text, fntSub.Font, lyricFontColorShadow, lyricRect, lyricCenter);
                        tg.DrawString(lyric.text, fntSub.Font, lyricFontColor, lyricRect, lyricCenter);
                    }
                    else
                    {
                        bg.DrawString(lyric.text, locLyric.Font, lyricFontColorShadow, lyricRect, lyricCenter);
                        tg.DrawString(lyric.text, locLyric.Font, lyricFontColor, lyricRect, lyricCenter);
                    }



                }
            }

            lyricShadowBlurer.MaskShadow(lyricComposeShadowBitmap);
        }

        void postRenderLyric()
        {
            using(Graphics g = Graphics.FromImage(lyricComposeTargetBitmap))
            {
                g.Clear(Color.Transparent);
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                if (currentLyricEntry == null || currentLyricEntry.isEmpty)
                {
                    return;
                }
                //g.DrawImage(lyricBackgroundBitmap, 0, 0, locLyric.Width, locLyric.Height);
                g.DrawImage(lyricComposeShadowBitmap, 0, 0, locLyric.Width, locLyric.Height);
                g.DrawImage(lyricComposedBitmap, 0, 0);
            }

        }







        [DllImport("user32.dll")]
        public static extern UInt32 RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, Keys vk);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            if (m.Msg == WM_HOTKEY)
            {
                if (m.WParam.ToInt32() == 2)
                {
                    btnPrev_Click(null, null);
                }
                if (m.WParam.ToInt32() == 3)
                {
                    btnPlay_Click(null, null);
                }
                if (m.WParam.ToInt32() == 4)
                {
                    btnNext_Click(null, null);
                }
            }

            base.WndProc(ref m);
        }


    }

    class GraphicsLayer : Form{
        Form1 parent;
        Rectangle relativeRect;

        public bool debug = false;

        public GraphicsLayer(Form1 form,Control sizeRef) {
            AutoScaleMode = AutoScaleMode.None;
            parent = form;
            relativeRect = new Rectangle(sizeRef.Location, sizeRef.Size);
            parent.layers.Add(this);
            this.Size = relativeRect.Size;
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            updatePosition();
        }

        public void updatePosition() {
            if (debug) {
                Top = 0;
                Left = 0;
                return;
            }
            this.Top = parent.Top + relativeRect.Y;
            this.Left = parent.Left + relativeRect.X;

        }

        public GdiSystem gdi;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            gdi = new GdiSystem(this);
            Win32.SetWindowLong(Handle, Win32.GWL_EXSTYLE, Win32.GetWindowLong(Handle, Win32.GWL_EXSTYLE) | Win32.WS_EX_TRANSPARENT);
            gdi.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            gdi.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
        }

        internal Graphics g => gdi.Graphics;

        public void changePosition(Point location) {
            this.relativeRect.X = location.X;
            this.relativeRect.Y = location.Y;
            this.updatePosition();
        }

        internal void UpdateWindow()
        {
            if (InvokeRequired) {
                runOnUiThread(() => gdi.UpdateWindow());
                return;
            }
            gdi.UpdateWindow();
        }
        void runOnUiThread(Action a)
        {
            Invoke(a);
        }

       

        
    }
}
