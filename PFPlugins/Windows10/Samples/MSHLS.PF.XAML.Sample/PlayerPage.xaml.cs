﻿/*********************************************************************************************************************
Microsft HLS SDK for Windows

Copyright (c) Microsoft Corporation

All rights reserved.

MIT License

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation
files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy,
modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software
is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

***********************************************************************************************************************/
using Microsoft.HLSClient;
using Microsoft.PlayerFramework.Adaptive.HLS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MSHLS.PF.XAML.Sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlayerPage : Page
    {
        private HLSPlugin _HLSPlugin;
        private IHLSController _HLSController;
        private IHLSPlaylist _HLSPlaylist;
        private DispatcherTimer _StatsTimer;

        public PlayerPage()
        {
            this.InitializeComponent();
            this._StatsTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(5) };
            this._StatsTimer.Tick += StatsTimer_Tick;

            var plugin = this.Player.Plugins.FirstOrDefault(p => typeof(HLSPlugin) == p.GetType());
            if (null != plugin)
            {
                this._HLSPlugin = plugin as HLSPlugin;
                this.WireHLSPluginHandlers();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null && e.Parameter is Uri)
                this.Player.Source = e.Parameter as Uri;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this._StatsTimer.Stop();
            this._StatsTimer.Tick -= StatsTimer_Tick;
            this.UnwireHLSPluginHandlers();
            this.Player.Dispose();
        }

        private void WireHLSPluginHandlers()
        {
            this._HLSPlugin.HLSControllerReady += HLSPlugin_HLSControllerReady;

            var plugin = Player.Plugins.FirstOrDefault(p => p.GetType() == typeof(Microsoft.PlayerFramework.CC608.CC608Plugin));
            if (plugin != null)
                (plugin as Microsoft.PlayerFramework.CC608.CC608Plugin).OnCaptionAdded += MainPage_OnCaptionAdded;
        }

        private void MainPage_OnCaptionAdded(object sender, Microsoft.PlayerFramework.CC608.UIElementEventArgs e)
        {
            var x = e.UIElement;
            var y = x;
        }

        private void UnwireHLSPluginHandlers()
        {
            this._HLSPlugin.HLSControllerReady -= HLSPlugin_HLSControllerReady;
            this.UnwireHLSPlaylistHandlers();
        }

        private void WireHLSPlaylistHandlers()
        {
            if (null != this._HLSPlaylist)
            {
                this._HLSPlaylist.BitrateSwitchSuggested += HLSPlaylist_BitrateSwitchSuggested;
                this._HLSPlaylist.BitrateSwitchCompleted += HLSPlaylist_BitrateSwitchCompleted;
            }
        }

        private void UnwireHLSPlaylistHandlers()
        {
            if (null != this._HLSPlaylist)
            {
                this._HLSPlaylist.BitrateSwitchSuggested -= HLSPlaylist_BitrateSwitchSuggested;
                this._HLSPlaylist.BitrateSwitchCompleted -= HLSPlaylist_BitrateSwitchCompleted;
            }
        }

        private void HLSPlugin_HLSControllerReady(object sender, IHLSController e)
        {
            if (null != e)
            {
                this._HLSController = e;

                if (null != this._HLSPlaylist)
                    this.UnwireHLSPlaylistHandlers();

                if (null != e.Playlist)
                {
                    this._HLSPlaylist = e.Playlist;
                    this.WireHLSPlaylistHandlers();
                }
            }
        }

        private void HLSPlaylist_BitrateSwitchSuggested(IHLSPlaylist sender, IHLSBitrateSwitchEventArgs args)
        {
            this.WriteStatsMsgAsync(
              "HLSPlaylist_BitrateSwitchSuggested",
              "from bitrate: {0}, to bitrate: {1}",
              args.FromBitrate, args.ToBitrate);
        }

        private void HLSPlaylist_BitrateSwitchCompleted(IHLSPlaylist sender, IHLSBitrateSwitchEventArgs args)
        {
            this.WriteStatsMsgAsync(
              "HLSPlaylist_BitrateSwitchCompleted",
              "from bitrate: {0}, to bitrate: {1}",
              args.FromBitrate, args.ToBitrate);
        }

        private async void StatsTimer_Tick(object sender, object e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.StatsPopup.IsOpen = false;
            });
        }

        private void StatsPopup_Opened(object sender, object e)
        {
            this._StatsTimer.Stop();
            this._StatsTimer.Start();
        }

        private void StatsPopup_Closed(object sender, object e)
        {
            this._StatsTimer.Stop();
        }

        private async Task WriteStatsMsgAsync(string title, string format, params object[] args)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (!string.IsNullOrWhiteSpace(format))
                {
                    this.StatsTitle.Text = title;
                    this.StatsMsg.Text = string.Format(format, args);
                    this.StatsPopup.IsOpen = true;
                }
                else
                {
                    this.StatsPopup.IsOpen = false;
                }
            });
        }
    }
}
