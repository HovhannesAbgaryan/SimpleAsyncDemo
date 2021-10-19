using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace WPFUserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Events

        #region Sync

        /// <summary>
        /// Execute sync
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Routed event args</param>
        private void executeSync_Click(object sender, RoutedEventArgs e)
        {
            //Start new stopwatch
            var watch = Stopwatch.StartNew();

            //Run download sync
            RunDownloadSync();

            //Stop watch
            watch.Stop();

            //Print total execution time for sync in results window
            resultsWindow.Text += $"Total execution time for sync: { watch.ElapsedMilliseconds }ms";
        }

        #endregion Sync

        #region Async

        /// <summary>
        /// Execute async
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Routed event args</param>
        private async void executeAsync_Click(object sender, RoutedEventArgs e)
        {
            //Start new stopwatch
            var watch = Stopwatch.StartNew();

            //Run download async
            await RunDownloadAsync();

            //Stop watch
            watch.Stop();

            //Print total execution time for async in results window
            resultsWindow.Text += $"Total execution time for async: { watch.ElapsedMilliseconds }ms";
        }

        #endregion Async

        #region Parallel async

        /// <summary>
        /// Execute parallel async
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Routed event args</param>
        private async void executeParallelAsync_Click(object sender, RoutedEventArgs e)
        {
            //Start new stopwatch
            var watch = Stopwatch.StartNew();

            //Run download parallel async
            await RunDownloadParallelAsync();

            //Stop watch
            watch.Stop();

            //Print total execution time for parallel async in results window
            resultsWindow.Text += $"Total execution time: { watch.ElapsedMilliseconds }ms";
        }

        #endregion Parallel async

        #endregion Events

        #region Functions

        #region Common

        /// <summary>
        /// Get some websites
        /// </summary>
        /// <returns>Websites</returns>
        private IEnumerable<string> GetWebsites()
        {
            //Clear results window
            resultsWindow.Text = "";

            //Create list of some websites
            var websites = new List<string>
            {
                //"https://www.yahoo.com",
                "https://www.google.com",
                "https://www.microsoft.com",
                "https://www.cnn.com",
                "https://www.codeproject.com",
                "https://www.stackoverflow.com"
            };

            return websites;
        }

        /// <summary>
        /// Report website info
        /// </summary>
        /// <param name="websiteDataModel">Website data model</param>
        private void ReportWebsiteInfo(WebsiteDataModel websiteDataModel)
        {
            //Print information about website data model in results window
            resultsWindow.Text += $"{ websiteDataModel.Url } downloaded: { websiteDataModel.Data.Length } characters long.{ Environment.NewLine }";
        }

        #endregion Common

        #region Sync

        /// <summary>
        /// Download website sync
        /// </summary>
        /// <param name="websiteURL">Website URL</param>
        /// <returns>Website data model</returns>
        private WebsiteDataModel DownloadWebsiteSync(string websiteURL)
        {
            //Create web client
            var webClient = new WebClient();

            //Create website data model
            var websiteDataModel = new WebsiteDataModel
            {
                Url = websiteURL,
                Data = webClient.DownloadString(websiteURL)
            };

            return websiteDataModel;
        }

        /// <summary>
        /// Run download sync
        /// </summary>
        private void RunDownloadSync()
        {
            //Get some websites
            var websites = GetWebsites();

            //For each website from the list of websites
            foreach (var website in websites)
            {
                //download website sync
                var data = DownloadWebsiteSync(website);

                //report website info
                ReportWebsiteInfo(data);
            }
        }

        #endregion Sync

        #region Async

        /// <summary>
        /// Download website async
        /// </summary>
        /// <param name="websiteURL">Website URL</param>
        /// <returns>Website data model</returns>
        private async Task<WebsiteDataModel> DownloadWebsiteAsync(string websiteURL)
        {
            //Create web client
            var webClient = new WebClient();

            //Create website data model
            var websiteDataModel = new WebsiteDataModel
            {
                Url = websiteURL,
                Data = await webClient.DownloadStringTaskAsync(websiteURL)
            };

            return websiteDataModel;
        }

        /// <summary>
        /// Run download async
        /// </summary>
        private async Task RunDownloadAsync()
        {
            //Get some websites
            var websites = GetWebsites();

            //For each website from the list of websites
            foreach (string website in websites)
            {
                //Download website async

                //Variant 1: if we have access to DownloadWebsiteSync function => we can make async version of that function and use it
                //var data = await DownloadWebsiteAsync(website);

                //Variant 2: if we don't have access to DownloadWebsiteSync function => we can use sync version of that function in async way
                var data = await Task.Run(() => DownloadWebsiteSync(website));

                //Report website info
                ReportWebsiteInfo(data);
            }
        }

        #endregion Async

        #region Parallel async

        /// <summary>
        /// Run download parallel async
        /// </summary>
        private async Task RunDownloadParallelAsync()
        {
            //Get some websites
            var websites = GetWebsites();

            //Create list of tasks of type WebsiteDataModel
            var tasks = new List<Task<WebsiteDataModel>>();

            //For each website from the list of websites
            foreach (var website in websites)
                //download website async and add downloaded website data model to list of tasks
                tasks.Add(DownloadWebsiteAsync(website));

            //Get the results when all tasks are done
            var results = await Task.WhenAll(tasks);

            //For each data in results
            foreach (var data in results)
                //report website info
                ReportWebsiteInfo(data);
        }

        #endregion Parallel async

        #endregion Functions
    }
}
