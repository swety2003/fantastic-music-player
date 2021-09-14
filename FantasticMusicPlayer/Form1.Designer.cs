namespace FantasticMusicPlayer
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.numProgress = new System.Windows.Forms.HScrollBar();
            this.locProgress = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblArtsit = new System.Windows.Forms.Label();
            this.locGlowing = new System.Windows.Forms.PictureBox();
            this.locMask = new System.Windows.Forms.PictureBox();
            this.btnMin = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.locSpectrum = new System.Windows.Forms.Label();
            this.btnVolume = new System.Windows.Forms.Button();
            this.btnMore = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.lblCurrentTime = new System.Windows.Forms.Label();
            this.btnPlayList = new System.Windows.Forms.Button();
            this.btnFolder = new System.Windows.Forms.Button();
            this.locButtonBlur = new System.Windows.Forms.Label();
            this.renderTimer = new System.Windows.Forms.Timer(this.components);
            this.tblBottomControl = new System.Windows.Forms.Panel();
            this.btnPreserved2 = new System.Windows.Forms.Button();
            this.lblProgressManager = new System.Windows.Forms.Panel();
            this.lblTotalTime = new System.Windows.Forms.Label();
            this.infoTimer = new System.Windows.Forms.Timer(this.components);
            this.locSpectrumArea = new System.Windows.Forms.Label();
            this.tblTopInfo = new System.Windows.Forms.Panel();
            this.imgBass = new System.Windows.Forms.PictureBox();
            this.imgHiResAudio = new System.Windows.Forms.PictureBox();
            this.tblUtils = new System.Windows.Forms.Panel();
            this.btnSrs = new System.Windows.Forms.Button();
            this.btnSpectrumMode = new System.Windows.Forms.Button();
            this.btnShuffe = new System.Windows.Forms.Button();
            this.btnLoopMode = new System.Windows.Forms.Button();
            this.tblVolumn = new System.Windows.Forms.Panel();
            this.locVolumeMinPoint = new System.Windows.Forms.Label();
            this.locVolumnMaxPoint = new System.Windows.Forms.Label();
            this.tblList = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.locGlowing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.locMask)).BeginInit();
            this.tblBottomControl.SuspendLayout();
            this.tblTopInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgBass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgHiResAudio)).BeginInit();
            this.tblUtils.SuspendLayout();
            this.tblVolumn.SuspendLayout();
            this.SuspendLayout();
            // 
            // numProgress
            // 
            this.numProgress.LargeChange = 1;
            this.numProgress.Location = new System.Drawing.Point(-16, 32);
            this.numProgress.Maximum = 0;
            this.numProgress.Name = "numProgress";
            this.numProgress.Size = new System.Drawing.Size(804, 19);
            this.numProgress.TabIndex = 1;
            // 
            // locProgress
            // 
            this.locProgress.AutoSize = true;
            this.locProgress.Location = new System.Drawing.Point(0, 43);
            this.locProgress.Name = "locProgress";
            this.locProgress.Size = new System.Drawing.Size(41, 12);
            this.locProgress.TabIndex = 2;
            this.locProgress.Text = "label1";
            this.locProgress.Visible = false;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(49, 2);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(618, 24);
            this.lblTitle.TabIndex = 3;
            this.lblTitle.Text = "label1";
            this.lblTitle.Visible = false;
            // 
            // lblArtsit
            // 
            this.lblArtsit.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblArtsit.Location = new System.Drawing.Point(51, 27);
            this.lblArtsit.Name = "lblArtsit";
            this.lblArtsit.Size = new System.Drawing.Size(618, 17);
            this.lblArtsit.TabIndex = 3;
            this.lblArtsit.Text = "label1";
            this.lblArtsit.Visible = false;
            // 
            // locGlowing
            // 
            this.locGlowing.Image = global::FantasticMusicPlayer.Properties.Resources.bg_glowing;
            this.locGlowing.Location = new System.Drawing.Point(256, 89);
            this.locGlowing.Name = "locGlowing";
            this.locGlowing.Size = new System.Drawing.Size(256, 256);
            this.locGlowing.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.locGlowing.TabIndex = 4;
            this.locGlowing.TabStop = false;
            this.locGlowing.Visible = false;
            // 
            // locMask
            // 
            this.locMask.Image = global::FantasticMusicPlayer.Properties.Resources.bg_glowing;
            this.locMask.Location = new System.Drawing.Point(271, 104);
            this.locMask.Name = "locMask";
            this.locMask.Size = new System.Drawing.Size(227, 227);
            this.locMask.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.locMask.TabIndex = 4;
            this.locMask.TabStop = false;
            this.locMask.Visible = false;
            // 
            // btnMin
            // 
            this.btnMin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMin.Location = new System.Drawing.Point(682, 7);
            this.btnMin.Name = "btnMin";
            this.btnMin.Size = new System.Drawing.Size(40, 40);
            this.btnMin.TabIndex = 5;
            this.btnMin.Text = "button1";
            this.btnMin.UseVisualStyleBackColor = true;
            this.btnMin.Click += new System.EventHandler(this.btnMin_Click);
            this.btnMin.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sfxEmphasis);
            this.btnMin.MouseEnter += new System.EventHandler(this.sfxAttaker);
            this.btnMin.MouseLeave += new System.EventHandler(this.sfxReleaser);
            this.btnMin.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sfxDeemphasis);
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Location = new System.Drawing.Point(720, 7);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(40, 40);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "button1";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            this.btnClose.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sfxEmphasis);
            this.btnClose.MouseEnter += new System.EventHandler(this.sfxAttaker);
            this.btnClose.MouseLeave += new System.EventHandler(this.sfxReleaser);
            this.btnClose.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sfxDeemphasis);
            // 
            // locSpectrum
            // 
            this.locSpectrum.AutoSize = true;
            this.locSpectrum.Location = new System.Drawing.Point(0, 219);
            this.locSpectrum.Name = "locSpectrum";
            this.locSpectrum.Size = new System.Drawing.Size(41, 12);
            this.locSpectrum.TabIndex = 6;
            this.locSpectrum.Text = "label3";
            this.locSpectrum.Visible = false;
            // 
            // btnVolume
            // 
            this.btnVolume.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolume.Location = new System.Drawing.Point(682, 54);
            this.btnVolume.Name = "btnVolume";
            this.btnVolume.Size = new System.Drawing.Size(35, 35);
            this.btnVolume.TabIndex = 5;
            this.btnVolume.Text = "button1";
            this.btnVolume.UseVisualStyleBackColor = true;
            this.btnVolume.Click += new System.EventHandler(this.btnVolume_Click);
            this.btnVolume.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sfxEmphasis);
            this.btnVolume.MouseEnter += new System.EventHandler(this.sfxAttaker);
            this.btnVolume.MouseLeave += new System.EventHandler(this.sfxReleaser);
            this.btnVolume.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sfxDeemphasis);
            // 
            // btnMore
            // 
            this.btnMore.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMore.Location = new System.Drawing.Point(723, 54);
            this.btnMore.Name = "btnMore";
            this.btnMore.Size = new System.Drawing.Size(35, 35);
            this.btnMore.TabIndex = 5;
            this.btnMore.Text = "button1";
            this.btnMore.UseVisualStyleBackColor = true;
            this.btnMore.Click += new System.EventHandler(this.btnMore_Click);
            this.btnMore.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sfxEmphasis);
            this.btnMore.MouseEnter += new System.EventHandler(this.sfxAttaker);
            this.btnMore.MouseLeave += new System.EventHandler(this.sfxReleaser);
            this.btnMore.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sfxDeemphasis);
            // 
            // btnPlay
            // 
            this.btnPlay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPlay.Location = new System.Drawing.Point(361, 49);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(42, 42);
            this.btnPlay.TabIndex = 5;
            this.btnPlay.Text = "button1";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            this.btnPlay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sfxEmphasis);
            this.btnPlay.MouseEnter += new System.EventHandler(this.sfxAttaker);
            this.btnPlay.MouseLeave += new System.EventHandler(this.sfxReleaser);
            this.btnPlay.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sfxDeemphasis);
            // 
            // btnNext
            // 
            this.btnNext.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNext.Location = new System.Drawing.Point(404, 52);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(35, 35);
            this.btnNext.TabIndex = 5;
            this.btnNext.Text = "button1";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            this.btnNext.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sfxEmphasis);
            this.btnNext.MouseEnter += new System.EventHandler(this.sfxAttaker);
            this.btnNext.MouseLeave += new System.EventHandler(this.sfxReleaser);
            this.btnNext.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sfxDeemphasis);
            // 
            // btnPrev
            // 
            this.btnPrev.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPrev.Location = new System.Drawing.Point(327, 52);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(35, 35);
            this.btnPrev.TabIndex = 5;
            this.btnPrev.Text = "button1";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            this.btnPrev.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sfxEmphasis);
            this.btnPrev.MouseEnter += new System.EventHandler(this.sfxAttaker);
            this.btnPrev.MouseLeave += new System.EventHandler(this.sfxReleaser);
            this.btnPrev.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sfxDeemphasis);
            // 
            // lblCurrentTime
            // 
            this.lblCurrentTime.BackColor = System.Drawing.Color.Transparent;
            this.lblCurrentTime.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentTime.ForeColor = System.Drawing.Color.White;
            this.lblCurrentTime.Location = new System.Drawing.Point(234, 55);
            this.lblCurrentTime.Name = "lblCurrentTime";
            this.lblCurrentTime.Size = new System.Drawing.Size(85, 29);
            this.lblCurrentTime.TabIndex = 3;
            this.lblCurrentTime.Text = "label1";
            this.lblCurrentTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblCurrentTime.Visible = false;
            // 
            // btnPlayList
            // 
            this.btnPlayList.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPlayList.Location = new System.Drawing.Point(51, 54);
            this.btnPlayList.Name = "btnPlayList";
            this.btnPlayList.Size = new System.Drawing.Size(35, 35);
            this.btnPlayList.TabIndex = 5;
            this.btnPlayList.Text = "button1";
            this.btnPlayList.UseVisualStyleBackColor = true;
            this.btnPlayList.Click += new System.EventHandler(this.btnPlayList_Click);
            this.btnPlayList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sfxEmphasis);
            this.btnPlayList.MouseEnter += new System.EventHandler(this.sfxAttaker);
            this.btnPlayList.MouseLeave += new System.EventHandler(this.sfxReleaser);
            this.btnPlayList.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sfxDeemphasis);
            // 
            // btnFolder
            // 
            this.btnFolder.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFolder.Location = new System.Drawing.Point(10, 54);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(35, 35);
            this.btnFolder.TabIndex = 5;
            this.btnFolder.Text = "button1";
            this.btnFolder.UseVisualStyleBackColor = true;
            this.btnFolder.Click += new System.EventHandler(this.btnFolder_Click);
            this.btnFolder.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sfxEmphasis);
            this.btnFolder.MouseEnter += new System.EventHandler(this.sfxAttaker);
            this.btnFolder.MouseLeave += new System.EventHandler(this.sfxReleaser);
            this.btnFolder.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sfxDeemphasis);
            // 
            // locButtonBlur
            // 
            this.locButtonBlur.Location = new System.Drawing.Point(60, 277);
            this.locButtonBlur.Name = "locButtonBlur";
            this.locButtonBlur.Size = new System.Drawing.Size(40, 40);
            this.locButtonBlur.TabIndex = 7;
            this.locButtonBlur.Text = "label3";
            this.locButtonBlur.Visible = false;
            // 
            // renderTimer
            // 
            this.renderTimer.Interval = 1;
            this.renderTimer.Tick += new System.EventHandler(this.renderTimer_Tick);
            // 
            // tblBottomControl
            // 
            this.tblBottomControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.tblBottomControl.Controls.Add(this.btnPreserved2);
            this.tblBottomControl.Controls.Add(this.lblProgressManager);
            this.tblBottomControl.Controls.Add(this.lblTotalTime);
            this.tblBottomControl.Controls.Add(this.locProgress);
            this.tblBottomControl.Controls.Add(this.btnPlay);
            this.tblBottomControl.Controls.Add(this.lblCurrentTime);
            this.tblBottomControl.Controls.Add(this.btnMore);
            this.tblBottomControl.Controls.Add(this.btnVolume);
            this.tblBottomControl.Controls.Add(this.btnPrev);
            this.tblBottomControl.Controls.Add(this.btnPlayList);
            this.tblBottomControl.Controls.Add(this.btnNext);
            this.tblBottomControl.Controls.Add(this.btnFolder);
            this.tblBottomControl.Controls.Add(this.numProgress);
            this.tblBottomControl.Location = new System.Drawing.Point(0, 345);
            this.tblBottomControl.Name = "tblBottomControl";
            this.tblBottomControl.Size = new System.Drawing.Size(768, 98);
            this.tblBottomControl.TabIndex = 8;
            this.tblBottomControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.tblBottomControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.tblBottomControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
            // 
            // btnPreserved2
            // 
            this.btnPreserved2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPreserved2.Location = new System.Drawing.Point(96, 54);
            this.btnPreserved2.Name = "btnPreserved2";
            this.btnPreserved2.Size = new System.Drawing.Size(35, 35);
            this.btnPreserved2.TabIndex = 0;
            this.btnPreserved2.Text = "button1";
            this.btnPreserved2.UseVisualStyleBackColor = true;
            this.btnPreserved2.Click += new System.EventHandler(this.btnPreserved2_Click);
            this.btnPreserved2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sfxEmphasis);
            this.btnPreserved2.MouseEnter += new System.EventHandler(this.sfxAttaker);
            this.btnPreserved2.MouseLeave += new System.EventHandler(this.sfxReleaser);
            this.btnPreserved2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sfxDeemphasis);
            // 
            // lblProgressManager
            // 
            this.lblProgressManager.Cursor = System.Windows.Forms.Cursors.VSplit;
            this.lblProgressManager.Location = new System.Drawing.Point(0, 32);
            this.lblProgressManager.Name = "lblProgressManager";
            this.lblProgressManager.Size = new System.Drawing.Size(768, 19);
            this.lblProgressManager.TabIndex = 6;
            this.lblProgressManager.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblProgressManager_MouseDown);
            this.lblProgressManager.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblProgressManager_MouseMove);
            this.lblProgressManager.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblProgressManager_MouseUp);
            // 
            // lblTotalTime
            // 
            this.lblTotalTime.BackColor = System.Drawing.Color.Transparent;
            this.lblTotalTime.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalTime.ForeColor = System.Drawing.Color.White;
            this.lblTotalTime.Location = new System.Drawing.Point(448, 55);
            this.lblTotalTime.Name = "lblTotalTime";
            this.lblTotalTime.Size = new System.Drawing.Size(85, 29);
            this.lblTotalTime.TabIndex = 3;
            this.lblTotalTime.Text = "label1";
            this.lblTotalTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTotalTime.Visible = false;
            // 
            // infoTimer
            // 
            this.infoTimer.Interval = 20;
            this.infoTimer.Tick += new System.EventHandler(this.infoTimer_Tick);
            // 
            // locSpectrumArea
            // 
            this.locSpectrumArea.Location = new System.Drawing.Point(0, 259);
            this.locSpectrumArea.Name = "locSpectrumArea";
            this.locSpectrumArea.Size = new System.Drawing.Size(768, 128);
            this.locSpectrumArea.TabIndex = 9;
            this.locSpectrumArea.Text = "label3";
            this.locSpectrumArea.Visible = false;
            // 
            // tblTopInfo
            // 
            this.tblTopInfo.BackColor = System.Drawing.Color.Transparent;
            this.tblTopInfo.Controls.Add(this.imgBass);
            this.tblTopInfo.Controls.Add(this.imgHiResAudio);
            this.tblTopInfo.Controls.Add(this.lblTitle);
            this.tblTopInfo.Controls.Add(this.lblArtsit);
            this.tblTopInfo.Location = new System.Drawing.Point(10, 3);
            this.tblTopInfo.Name = "tblTopInfo";
            this.tblTopInfo.Size = new System.Drawing.Size(671, 52);
            this.tblTopInfo.TabIndex = 10;
            this.tblTopInfo.Visible = false;
            // 
            // imgBass
            // 
            this.imgBass.Enabled = false;
            this.imgBass.Image = global::FantasticMusicPlayer.Properties.Resources.bassboost;
            this.imgBass.Location = new System.Drawing.Point(625, 2);
            this.imgBass.Name = "imgBass";
            this.imgBass.Size = new System.Drawing.Size(42, 42);
            this.imgBass.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imgBass.TabIndex = 11;
            this.imgBass.TabStop = false;
            this.imgBass.Visible = false;
            // 
            // imgHiResAudio
            // 
            this.imgHiResAudio.Enabled = false;
            this.imgHiResAudio.Image = global::FantasticMusicPlayer.Properties.Resources.hiresaudio;
            this.imgHiResAudio.Location = new System.Drawing.Point(2, 3);
            this.imgHiResAudio.Name = "imgHiResAudio";
            this.imgHiResAudio.Size = new System.Drawing.Size(43, 43);
            this.imgHiResAudio.TabIndex = 11;
            this.imgHiResAudio.TabStop = false;
            this.imgHiResAudio.Visible = false;
            // 
            // tblUtils
            // 
            this.tblUtils.Controls.Add(this.btnSrs);
            this.tblUtils.Controls.Add(this.btnSpectrumMode);
            this.tblUtils.Controls.Add(this.btnShuffe);
            this.tblUtils.Controls.Add(this.btnLoopMode);
            this.tblUtils.Location = new System.Drawing.Point(700, 119);
            this.tblUtils.Name = "tblUtils";
            this.tblUtils.Size = new System.Drawing.Size(60, 201);
            this.tblUtils.TabIndex = 11;
            this.tblUtils.Visible = false;
            // 
            // btnSrs
            // 
            this.btnSrs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSrs.Location = new System.Drawing.Point(13, 157);
            this.btnSrs.Name = "btnSrs";
            this.btnSrs.Size = new System.Drawing.Size(32, 32);
            this.btnSrs.TabIndex = 0;
            this.btnSrs.Text = "button1";
            this.btnSrs.UseVisualStyleBackColor = true;
            this.btnSrs.Click += new System.EventHandler(this.btnSrs_Click);
            this.btnSrs.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sfxEmphasis);
            this.btnSrs.MouseEnter += new System.EventHandler(this.sfxAttaker);
            this.btnSrs.MouseLeave += new System.EventHandler(this.sfxReleaser);
            this.btnSrs.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sfxDeemphasis);
            // 
            // btnSpectrumMode
            // 
            this.btnSpectrumMode.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSpectrumMode.Location = new System.Drawing.Point(14, 108);
            this.btnSpectrumMode.Name = "btnSpectrumMode";
            this.btnSpectrumMode.Size = new System.Drawing.Size(32, 32);
            this.btnSpectrumMode.TabIndex = 0;
            this.btnSpectrumMode.Text = "button1";
            this.btnSpectrumMode.UseVisualStyleBackColor = true;
            this.btnSpectrumMode.Click += new System.EventHandler(this.btnPreserved1_Click);
            this.btnSpectrumMode.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sfxEmphasis);
            this.btnSpectrumMode.MouseEnter += new System.EventHandler(this.sfxAttaker);
            this.btnSpectrumMode.MouseLeave += new System.EventHandler(this.sfxReleaser);
            this.btnSpectrumMode.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sfxDeemphasis);
            // 
            // btnShuffe
            // 
            this.btnShuffe.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnShuffe.Location = new System.Drawing.Point(14, 60);
            this.btnShuffe.Name = "btnShuffe";
            this.btnShuffe.Size = new System.Drawing.Size(32, 32);
            this.btnShuffe.TabIndex = 0;
            this.btnShuffe.Text = "button1";
            this.btnShuffe.UseVisualStyleBackColor = true;
            this.btnShuffe.Click += new System.EventHandler(this.btnShuffe_Click);
            this.btnShuffe.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sfxEmphasis);
            this.btnShuffe.MouseEnter += new System.EventHandler(this.sfxAttaker);
            this.btnShuffe.MouseLeave += new System.EventHandler(this.sfxReleaser);
            this.btnShuffe.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sfxDeemphasis);
            // 
            // btnLoopMode
            // 
            this.btnLoopMode.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoopMode.Location = new System.Drawing.Point(14, 12);
            this.btnLoopMode.Name = "btnLoopMode";
            this.btnLoopMode.Size = new System.Drawing.Size(32, 32);
            this.btnLoopMode.TabIndex = 0;
            this.btnLoopMode.Text = "button1";
            this.btnLoopMode.UseVisualStyleBackColor = true;
            this.btnLoopMode.Click += new System.EventHandler(this.btnLoopMode_Click);
            this.btnLoopMode.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sfxEmphasis);
            this.btnLoopMode.MouseEnter += new System.EventHandler(this.sfxAttaker);
            this.btnLoopMode.MouseLeave += new System.EventHandler(this.sfxReleaser);
            this.btnLoopMode.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sfxDeemphasis);
            // 
            // tblVolumn
            // 
            this.tblVolumn.Controls.Add(this.locVolumeMinPoint);
            this.tblVolumn.Controls.Add(this.locVolumnMaxPoint);
            this.tblVolumn.Location = new System.Drawing.Point(700, 119);
            this.tblVolumn.Name = "tblVolumn";
            this.tblVolumn.Size = new System.Drawing.Size(60, 201);
            this.tblVolumn.TabIndex = 11;
            this.tblVolumn.Visible = false;
            this.tblVolumn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.volumnDown);
            this.tblVolumn.MouseMove += new System.Windows.Forms.MouseEventHandler(this.volumeMove);
            this.tblVolumn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.volumeUp);
            // 
            // locVolumeMinPoint
            // 
            this.locVolumeMinPoint.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.locVolumeMinPoint.Location = new System.Drawing.Point(28, 178);
            this.locVolumeMinPoint.Name = "locVolumeMinPoint";
            this.locVolumeMinPoint.Size = new System.Drawing.Size(22, 22);
            this.locVolumeMinPoint.TabIndex = 0;
            this.locVolumeMinPoint.Text = "label1";
            this.locVolumeMinPoint.Visible = false;
            // 
            // locVolumnMaxPoint
            // 
            this.locVolumnMaxPoint.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.locVolumnMaxPoint.Location = new System.Drawing.Point(28, 24);
            this.locVolumnMaxPoint.Name = "locVolumnMaxPoint";
            this.locVolumnMaxPoint.Size = new System.Drawing.Size(22, 22);
            this.locVolumnMaxPoint.TabIndex = 0;
            this.locVolumnMaxPoint.Text = "label1";
            this.locVolumnMaxPoint.Visible = false;
            // 
            // tblList
            // 
            this.tblList.Location = new System.Drawing.Point(0, 54);
            this.tblList.Name = "tblList";
            this.tblList.Size = new System.Drawing.Size(248, 333);
            this.tblList.TabIndex = 12;
            this.tblList.Visible = false;
            this.tblList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listMouseDown);
            this.tblList.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listMouseMove);
            this.tblList.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listMouseUp);
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackgroundImage = global::FantasticMusicPlayer.Properties.Resources.control_blueprint;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(768, 440);
            this.Controls.Add(this.tblList);
            this.Controls.Add(this.tblUtils);
            this.Controls.Add(this.tblVolumn);
            this.Controls.Add(this.tblTopInfo);
            this.Controls.Add(this.locSpectrumArea);
            this.Controls.Add(this.locButtonBlur);
            this.Controls.Add(this.locSpectrum);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnMin);
            this.Controls.Add(this.locMask);
            this.Controls.Add(this.locGlowing);
            this.Controls.Add(this.tblBottomControl);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Music Player";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.Click += new System.EventHandler(this.Form1_Click);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.locGlowing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.locMask)).EndInit();
            this.tblBottomControl.ResumeLayout(false);
            this.tblBottomControl.PerformLayout();
            this.tblTopInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imgBass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgHiResAudio)).EndInit();
            this.tblUtils.ResumeLayout(false);
            this.tblVolumn.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.HScrollBar numProgress;
        private System.Windows.Forms.Label locProgress;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblArtsit;
        private System.Windows.Forms.PictureBox locGlowing;
        private System.Windows.Forms.PictureBox locMask;
        private System.Windows.Forms.Button btnMin;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label locSpectrum;
        private System.Windows.Forms.Button btnVolume;
        private System.Windows.Forms.Button btnMore;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Label lblCurrentTime;
        private System.Windows.Forms.Button btnPlayList;
        private System.Windows.Forms.Button btnFolder;
        private System.Windows.Forms.Label locButtonBlur;
        private System.Windows.Forms.Timer renderTimer;
        private System.Windows.Forms.Panel tblBottomControl;
        private System.Windows.Forms.Label lblTotalTime;
        private System.Windows.Forms.Timer infoTimer;
        private System.Windows.Forms.Panel lblProgressManager;
        private System.Windows.Forms.Label locSpectrumArea;
        private System.Windows.Forms.Panel tblTopInfo;
        private System.Windows.Forms.PictureBox imgHiResAudio;
        private System.Windows.Forms.Panel tblUtils;
        private System.Windows.Forms.Button btnPreserved2;
        private System.Windows.Forms.Button btnSpectrumMode;
        private System.Windows.Forms.Button btnShuffe;
        private System.Windows.Forms.Button btnLoopMode;
        private System.Windows.Forms.Panel tblVolumn;
        private System.Windows.Forms.Label locVolumeMinPoint;
        private System.Windows.Forms.Label locVolumnMaxPoint;
        private System.Windows.Forms.Panel tblList;
        private System.Windows.Forms.PictureBox imgBass;
        private System.Windows.Forms.Button btnSrs;
    }
}

