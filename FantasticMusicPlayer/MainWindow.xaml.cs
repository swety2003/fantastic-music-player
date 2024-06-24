using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FantasticMusicPlayer.Contracts;
using FantasticMusicPlayer.Extensions;
using FantasticMusicPlayer.Models;
using FantasticMusicPlayer.Utils;
using Microsoft.Extensions.Configuration;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FantasticMusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ObservableObject]
    public partial class MainWindow : Window, IShellWindow
    {

        IBassPlayer player;
        PlayerController controller;
        DispatcherTimer ui_timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
        public MainWindow(IConfiguration configuration, IBassPlayer player
            , PlayerController controller)
        {
            InitializeComponent();
            DataContext = this;

            this.player = player;
            this.Configuration = configuration;
            this.controller = controller;


            player.init();
            player.Stopped += Player_Stopped;
            player.CoverAvailable += Player_CoverAvailable;
            player.SongInfoAvailable += Player_SongInfoAvailable;

            controller.SongChanged += Controller_SongChanged;


            var bmp = Bitmap.FromFile(@"D:\Repo\FantasticMusicPlayer\FantasticMusicPlayer\Assets\sample_cover.jpg");
            BackgroundBrush = new GaussianBlur(48).ProcessImage(GaussianBlur.copyForGuassianBlur(bmp)).ToBitmapSource();
            CoverSource = new Bitmap(bmp).ToBitmapSource();

            Loaded += MainWindow_Loaded;
            ui_timer.Tick += Ui_timer_Tick;
            ui_timer.Start();
        }

        private void Ui_timer_Tick(object? sender, EventArgs e)
        {
            TotalPosition = player.TotalPosition;

            CurrentPosition = (player.CurrentPosition >= TotalPosition ? TotalPosition : player.CurrentPosition);
            
            IsPlaying = player.IsPlaying;

            if (player is BassPlayerImpl bass_player)
            {
                PeakDB = bass_player.peakDB;
            }
        
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (controller.CurrentPlaying != null)
            {
                controller.ImReady();
                player.Pause();
                //controller.LoopMode = Properties.Settings.Default.playmode;
                //controller.Shuffe = Properties.Settings.Default.shuffemode;
                //player.BassBoost = Properties.Settings.Default.bassboost;
                //int dspType = Properties.Settings.Default.dsptype;

                //this.dsptype = dspType % 10;
                //player.DynamicRangeCompressed = dspType / 10 % 10 > 0;
                //if (dspType == 1)
                //{
                //    ((DspSwitcher)player.SurroundSound).WrappedDSP = new SurroundDSP();
                //}
                //if (dspType == 2)
                //{
                //    ((DspSwitcher)player.SurroundSound).WrappedDSP = new SpeakerInRoomDSP();
                //}

                //int spectrumMode = Properties.Settings.Default.spectrummode;

                //SpectrumMode = spectrumMode % 100;
                //_showDesktopLyrics = ((spectrumMode / 100) % 10) > 0;
                //try
                //{
                //    player.LoadFx(Properties.Settings.Default.fxfile);
                //}
                //catch (Exception)
                //{
                //}
            }
        }



        #region Player
        private void Controller_SongChanged(object sender, SongSwitchedEventArgs e)
        {
            player.Looping = controller.LoopMode == 1;
            player.Load(e.CurrentSong.Path);
            player.Play();

            CurrentPlayingSong = e.CurrentSong;
        }

        private void Player_SongInfoAvailable(object sender, SongInfoEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                SongName = e.title;
                SongArtist = e.artist;
                SongAlbum = e.album;
                //Text = e.title;
                HiResAudio = e.HiResAudio;
                
                Title =$"{e.title}-{e.artist}";
                //this.lyricManager = e.LyricManager;
            });

            //checkFavStatus();
            //updateTopControl();


        }

        private void Player_Stopped(object sender, EventArgs e)
        {
            App.Current.Dispatcher.Invoke(() => controller.onSongStop());

        }

        private void Player_CoverAvailable(object sender, AlbumEventArgs e)
        {
            if (e.cover!=null)
            {

                BackgroundBrush = new GaussianBlur(48)
                    .ProcessImage(GaussianBlur.copyForGuassianBlur(e.cover)).ToBitmapSource();
                CoverSource = new Bitmap(e.cover).ToBitmapSource();
            }
        }
        #endregion

        [ObservableProperty]
        bool isPlaying;
        
        [ObservableProperty]
        float peakDB;

        [ObservableProperty]
        SongEntry currentPlayingSong;

        [ObservableProperty]
        long totalPosition;
        [ObservableProperty]
        long currentPosition;

        [ObservableProperty]
        bool hiResAudio;

        [ObservableProperty]
        ImageSource backgroundBrush;
        [ObservableProperty]
        string songName = "HiResAudio";
        [ObservableProperty]
        string songAlbum = "APP_LOCATION";
        [ObservableProperty]
        string songArtist;

        [ObservableProperty]
        ImageSource coverSource;

        [ObservableProperty]
        List<PlayList> playLists;

        [ObservableProperty]
        PlayList activePlayList;

        [ObservableProperty]
        IEnumerable<SongEntry> songEntries;

        [ObservableProperty]
        bool showPlayList;
        [ObservableProperty]
        int loopMode=0;
        [ObservableProperty]
        bool shuffleMode = false;

        [RelayCommand]
        void Play()
        {
            player.PlayOrPause();
        }
        [RelayCommand]
        void Previous()
        {
            controller.onPrevButtonClick();
        }
        [RelayCommand]
        void Next()
        {
            controller.onNextButtonClick();
        }
        [RelayCommand]
        void Playlist()
        {
            //ShowPlayList = !ShowPlayList;
            PlayLists =controller.AllPlayList;
            ActivePlayList = PlayLists.FirstOrDefault();
            if (ActivePlayList!=null)
            {
                SongEntries = ActivePlayList.Songs;
            }


        }
        [RelayCommand]
        void ChangeSong()
        {
            if (CurrentPlayingSong!=controller.CurrentPlaying)
            {
                controller.CurrentPlaying = CurrentPlayingSong;

                controller.ImReady();
            }
        }
        [RelayCommand]
        void ChangeLoopMode()
        {

            /// 0 - 全部播放 1 - 单曲循环 2 - 列表循环
            //controller.LoopMode
            if (LoopMode<2)
            {
                LoopMode++;
            }
            else
            {
                LoopMode = 0;
            }

            controller.LoopMode = LoopMode;
        }
        partial void OnShuffleModeChanged(bool value)
        {
            controller.Shuffe = value;
        }
        //[RelayCommand]
        //void ChangeShuffleMode()
        //{
        //    //controller.Shuffe = !controller.Shuffe;
        //}
        public IConfiguration Configuration { get; }

        public void CloseWindow()
        {
            this.Close();
        }

        public void ShowWindow()
        {
            Show();
        }


    }

}