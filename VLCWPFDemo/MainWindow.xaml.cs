using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Xml.Linq;
using Vlc.DotNet.Wpf;

namespace VLCWPFDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VlcVideoSourceProvider sourceProvider;
        private VlcVideoSourceProvider sourceProvider2;
        string currentDirectory;

        private DispatcherTimer _videoDurationTimer = null;

        private static readonly ILog log = LogManager.GetLogger((typeof(MainWindow).Name));

        public MainWindow()
        {
            try
            {
                log.Info("MainWindow - ctor ");

                InitializeComponent();
                var currentAssembly = Assembly.GetEntryAssembly();
                currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
                // Default installation path of VideoLAN.LibVLC.Windows
                var libDirectory = new DirectoryInfo(Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));

                this.sourceProvider = new VlcVideoSourceProvider(this.Dispatcher);
                this.sourceProvider2 = new VlcVideoSourceProvider(this.Dispatcher);

                this.sourceProvider.CreatePlayer(libDirectory/* pass your player parameters here */);
                this.sourceProvider2.CreatePlayer(libDirectory/* pass your player parameters here */);

                this.Video.SetBinding(System.Windows.Controls.Image.SourceProperty,
                    new Binding(nameof(VlcVideoSourceProvider.VideoSource)) { Source = sourceProvider }); 

                this.Video2.SetBinding(System.Windows.Controls.Image.SourceProperty,
                     new Binding(nameof(VlcVideoSourceProvider.VideoSource)) { Source = sourceProvider });
                Start();
            }
            catch (Exception ex)
            {
                log.Error("MainWindow - ctor -", ex);
            }
        }
        public void Start()
        {
            string FilePath;
            double dur = 30;
            try
            {
                log.Info("MainWindow - Start ");

                FilePath = Path.Combine(currentDirectory, "1.mp4");
                this.sourceProvider.MediaPlayer.Play(new Uri(FilePath));
                this.sourceProvider2.MediaPlayer.Play(new Uri(FilePath));
            }
            catch (Exception ex)
            {
                log.Error("MainWindow - Start - ", ex);

            }
            CreateVideoDurationTimer(dur);

        }
        private void CreateVideoDurationTimer(double duration = 30)
        {
            try
            {
                log.Info("MainWindow - CreateVideoDurationTimer ");
                log.Info("MainWindow - CreateVideoDurationTimer - interval - " + duration);
                if (_videoDurationTimer == null)
                {
                    _videoDurationTimer = new DispatcherTimer();
                    _videoDurationTimer.Tick -= _videoDurationTimer_Tick;
                    _videoDurationTimer.Tick += _videoDurationTimer_Tick;
                }
                _videoDurationTimer.Stop();
                _videoDurationTimer.Interval = TimeSpan.FromSeconds(duration);
                _videoDurationTimer.Start();
            }
            catch (Exception ex)
            {
                log.Error("MainWindow - CreateVideoDurationTimer - ", ex);
            }
        }
        private void _videoDurationTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                log.Info("MainWindow - _videoDurationTimer_Tick ");
                this.sourceProvider.MediaPlayer.Pause();
                this.sourceProvider2.MediaPlayer.Pause();
                this.Start();
            }
            catch (Exception ex)
            {
                log.Error("MainWindow - _videoDurationTimer_Tick - ", ex);

            }

        }

    }
   
}
