using System;
using System.Collections.Generic;
using System.Linq;
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
using ITunesLibrarySynchronizer.lib.ItunesEngine;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
using ITunesLibrarySynchronizer.lib.Utils;
using System.Windows.Media.Animation;
using ITunesLibrarySynchronizer.lib.SyncEngine;
using System.Windows.Threading;

namespace ITunesLibrarySynchronizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker _bgWrk;
        private LibraryParser _itunesLibrary;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _bgWrk = new BackgroundWorker();
            _bgWrk.DoWork += new DoWorkEventHandler(LoadItunesLibrary);
            _bgWrk.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadItunesLibraryComplete);
            _bgWrk.RunWorkerAsync();

            // load settings
            tbSynchFolder.Text = Properties.Settings.Default.SynchRootFolder;
            tbStructurePattern.Text = Properties.Settings.Default.StructurePattern;

            Properties.Settings.Default.SettingsSaving += new System.Configuration.SettingsSavingEventHandler(SavingSettings);

            btnStopSynch.IsEnabled = false;
            btnStartSynch.IsEnabled = false;
        }

        void SavingSettings(object sender, CancelEventArgs e)
        {
            var dA = new DoubleAnimationUsingKeyFrames();

            dA.KeyFrames.Add(new LinearDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)), Value = 0.0 });
            dA.KeyFrames.Add(new LinearDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5)), Value = 1.0 });
            dA.KeyFrames.Add(new LinearDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(3.5)), Value = 1.0 });
            dA.KeyFrames.Add(new LinearDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(4)), Value = 0.0 });

            imgSaveIndicator.BeginAnimation(Image.OpacityProperty, dA);
        }

        void LoadItunesLibraryComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            var msgObj = e.Result as BackgroundWorkerMessage;
            if (msgObj.WorkerException != null)
                sbBottomStatusMessage.Text = "Error: Failed parsing ITunes Library, check configuration!";
            else
                sbBottomStatusMessage.Text = "Status: " + msgObj.WorkResult + " records loaded in " + msgObj.MilisecondsElapsed + "ms!";

            tbITunesLibraryLocation.Text = _itunesLibrary.ItunesLibraryLocation;
            btnStartSynch.IsEnabled = true;
        }

        void LoadItunesLibrary(object sender, DoWorkEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Exception workerException = default(ApplicationException);
            _itunesLibrary = default(LibraryParser);
            try
            {
                _itunesLibrary = new LibraryParser();
                _itunesLibrary.ParseLibrary();
            }
            catch (ApplicationException parseException)
            {
                workerException = parseException;
            }
            sw.Stop();

            e.Result = new BackgroundWorkerMessage
                           {
                               MilisecondsElapsed = sw.ElapsedMilliseconds,
                               WorkResult = (_itunesLibrary == default(LibraryParser) ? 0 : _itunesLibrary.NumberOfEntries),
                               WorkerException = workerException
                           };
        }

        private void OpenFolderBrowser(object sender, RoutedEventArgs e)
        {
            // open folder browser
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            if (!String.IsNullOrEmpty(tbSynchFolder.Text))
                dlg.SelectedPath = tbSynchFolder.Text;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                tbSynchFolder.Text = dlg.SelectedPath;
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SynchRootFolder = tbSynchFolder.Text;
            Properties.Settings.Default.StructurePattern = tbStructurePattern.Text;
            Properties.Settings.Default.Save();
        }

        private void StartLibrarySynchronization(object sender, RoutedEventArgs e)
        {
            _bgWrk = null;
            prgbSynchronization.SetValue(ProgressBar.ValueProperty, 0.0);

            _bgWrk = new BackgroundWorker();
            _bgWrk.WorkerSupportsCancellation = true;
            _bgWrk.DoWork += new DoWorkEventHandler(DoSynchronizationWork);
            _bgWrk.RunWorkerCompleted += new RunWorkerCompletedEventHandler(SynchronizationComplete);
            _bgWrk.RunWorkerAsync(_itunesLibrary.Library);

            btnStartSynch.IsEnabled = false;
            btnStopSynch.IsEnabled = true;
        }

        void SynchronizationComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            var msgObj = e.Result as BackgroundWorkerMessage;
            if (msgObj.WorkerException != null)
                sbBottomStatusMessage.Text = "Error: Failed synchronizing ITunes Library, check configuration!";
            else
                sbBottomStatusMessage.Text = "Status: " + msgObj.WorkResult + " records synchronized in " + msgObj.MilisecondsElapsed + "ms!";

            Dispatcher.BeginInvoke(DispatcherPriority.Background,
                (SendOrPostCallback)delegate(object cancelFlag)
                {
                    btnStartSynch.IsEnabled = true;
                    btnStopSynch.IsEnabled = false;
                    tbProgressText.Text = ((bool)cancelFlag ? "Synchronization was cancelled!" : "Synchronization complete!");
                }, msgObj.Cancelled);
        }

        internal class SynchronizationStateObject
        {
            public int NumberOfFilesProcessed { get; set; }
            public double PercentDone { get; set; }
        }

        private void DoSynchronizationWork(object sender, DoWorkEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Exception workerException = default(ApplicationException);

            int processCount = 0;
            try
            {
                double progress = 0;
                var library = e.Argument as List<LibraryEntry>;
                var synchronizer = new FileSynchronizer();

                foreach (var track in library)
                {
                    using (var resolver = new PathResolver(track.Data))
                    {
                        var sourcePath = resolver.ResolveSourcePath();
                        var targetPath = resolver.BuildTargetPath();

                        synchronizer.Synchronize(sourcePath, targetPath, false);

                        processCount++;
                    }

                    var currBgWrk = sender as BackgroundWorker;

                    if (currBgWrk.CancellationPending)
                    {
                        sw.Stop();
                        e.Result = new BackgroundWorkerMessage
                        {
                            MilisecondsElapsed = sw.ElapsedMilliseconds,
                            WorkResult = processCount,
                            Cancelled = true
                        };
                        break;
                    }

                    // set progress
                    progress += (100 / (double)library.Count);
                    Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        (SendOrPostCallback)delegate
                        {
                            prgbSynchronization.SetValue(ProgressBar.ValueProperty, progress);
                            prgbProcessAmount.SetValue(TextBlock.TextProperty, string.Format("{0}% done!", Math.Round(progress, 2)));
                            tbProgressText.SetValue(TextBlock.TextProperty, "Processed " + processCount + " of " + library.Count + " tracks!");
                        }, null);
                }
            }
            catch (ApplicationException synchException)
            {
                workerException = synchException;
            }
            sw.Stop();

            var check = e.Result as BackgroundWorkerMessage;
            if (check == null) // check if result message has already been sent
            {
                e.Result = new BackgroundWorkerMessage
                {
                    MilisecondsElapsed = sw.ElapsedMilliseconds,
                    WorkerException = workerException,
                    WorkResult = processCount
                };
            }
        }

        private void StopLibrarySynchronization(object sender, RoutedEventArgs e)
        {
            if (_bgWrk != null)
                if (_bgWrk.IsBusy)
                    _bgWrk.CancelAsync();
        }
    }
}
