﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Delight.Common;
using Delight.Components.Common;
using Delight.Controls;
using Delight.Extensions;

namespace Delight.Timing
{
    public class TimingReader
    {
        public TimingReader(TimeLine timeLine)
        {
            TimeLine = timeLine;

            TimeLine.FrameChanged += TimeLine_FrameChanged;
            TimeLine.FrameMouseChanged += (s, e) => StopLoad();
            
            TimeLine.ItemAdded += TimeLine_ItemAdded;
        }

        public void SetPlayer(MediaElementPro player1, MediaElementPro player2)
        {
            if (player1 == player2)
                throw new Exception("player1와 player2는 같은 인스턴스 일 수 없습니다.");

            this.player1 = player1;
            this.player2 = player2;

            player1.VideoRenderer = WPFMediaKit.DirectShow.MediaPlayers.VideoRendererType.VideoMixingRenderer9;
            player2.VideoRenderer = WPFMediaKit.DirectShow.MediaPlayers.VideoRendererType.VideoMixingRenderer9;

            player1.CurrentStateChanged += Player_CurrentStateChanged;
            player2.CurrentStateChanged += Player_CurrentStateChanged;

            loader1 = new MediaElementLoader(player1);
            loader2 = new MediaElementLoader(player2);
            
            StopLoad();
        }

        private void Player_CurrentStateChanged(MediaElementPro sender, PlayerState state)
        {
            if (state == PlayerState.Ended)
            {
                sender.Close();
            }
        }

        Queue<TrackItem> _allVideos = new Queue<TrackItem>();
        Queue<TrackItem> _loadWaitVideos = new Queue<TrackItem>();
        MediaElementPro player1, player2;
        MediaElementLoader loader1, loader2;

        public bool IsPlaying => 
            (player1?.IsPlaying).GetValueOrDefault() || (player2?.IsPlaying).GetValueOrDefault();

        public bool PlayerAssigned => (player1 != null) && (player2 != null);

        bool loading = false;

        #region [  TimeLine Event  ]

        private void TimeLine_ItemAdded(object sender, ItemEventArgs e)
        {
            if (e.Item.TrackType == TrackType.Video)
            {
                _allVideos.Enqueue(e.Item);
            }
        }

        bool p1Playing = false;

        private void TimeLine_FrameChanged(object sender, EventArgs e)
        {
            if (!PlayerAssigned)
                throw new NullReferenceException("player1 또는 player2가 할당되지 않았습니다. SetPlayer 함수를 이용해서 할당해주세요.");

            if (!TimeLine.IsRunning)
                return;

            if (loading)
            {
                LoadCheck();
                Task.Run(() =>
                {
                    LoadWaitingVideos();
                    
                    player1.Dispatcher.Invoke(() =>
                    {
                        void localPositionChanged(MediaElementPro s, TimeSpan p)
                        {
                            s.PositionChanged -= localPositionChanged;

                            TrackItem itm = s.GetTag<TrackItem>();
                            s.Position = MediaTools.FrameToTimeSpan(itm.ForwardOffset + TimeLine.Position - itm.Offset, TimeLine.FrameRate);
                            s.Volume = 1;
                            s.Visibility = Visibility.Visible;
                            if (s == player1)
                                player2.Visibility = Visibility.Hidden;
                            else
                                player1.Visibility = Visibility.Hidden;
                        }   
                        MainWindow mw = Application.Current.MainWindow as MainWindow;

                        if (player1.Tag == null && player2.Tag == null)
                        {
                            return;
                        }

                        if (player1.IsPlaying && player1.Source != null)
                        {
                            var p1Tag = player1.GetTag<TrackItem>();
                            if (TimeLine.Position == p1Tag.Offset + p1Tag.FrameWidth)
                            {
                                DisablePlayer(player1);
                            }
                        }

                        if (player2.IsPlaying && player2.Source != null)
                        {
                            var p2Tag = player2.GetTag<TrackItem>();
                            if (TimeLine.Position == p2Tag.Offset + p2Tag.FrameWidth)
                            {
                                DisablePlayer(player2);
                            }
                        }

                        DebugHelper.WriteLine(player1.Position + " :: " + player2.Position);
                        if (!p1Playing)
                        {
                            if (!player1.IsPlaying && player1.Tag != null &&
                                TimeLine.Position - 1 > player1.GetTag<TrackItem>().Offset)
                            {
                                player1.Play();
                                player1.PositionChanged += localPositionChanged;
                            }
                            p1Playing = true;
                        }
                        else
                        {
                            if (!player2.IsPlaying && player2.Tag != null &&
                                TimeLine.Position - 1 > player2.GetTag<TrackItem>().Offset)
                            {
                                player2.Play();
                                player2.PositionChanged += localPositionChanged;
                            }

                            p1Playing = false;
                        }

                    });
                });
            }
        }

        #endregion

        TimeLine TimeLine { get; }

        private void LoadWaitingVideos()
        {
            if (!TimeLine.IsRunning && !TimeLine.IsReady)
                return;

            if (_loadWaitVideos.Count == 0)
                return;

            if (!loader1.IsReadyForPlay)
            {
                loader1.LoadVideo(_loadWaitVideos.Dequeue()).Wait();
            }
            else if (!loader2.IsReadyForPlay)
            {
                loader2.LoadVideo(_loadWaitVideos.Dequeue()).Wait();
            }
        }

        public void StartLoad()
        {
            loading = true;
            _allVideos.Clear();

            TimeLine.Dispatcher.Invoke(() =>
            {
                foreach (TrackItem item in TimeLine.GetItems(TrackType.Video, TimeLine.Position).OrderBy(i => i.Offset))
                {
                    _allVideos.Enqueue(item);
                }

                LoadCheck();
            });

            LoadWaitingVideos();
            DebugHelper.WriteLine("StartLoad Method End");
        }

        public void StopLoad()
        {
            loading = false;

            if (player1.CurrentState == PlayerState.Playing)
            {
                DisablePlayer(player1);
            }
            
            if (player2.CurrentState == PlayerState.Playing)
            {
                DisablePlayer(player2);
            }
        }

        public void DisablePlayer(MediaElementPro player)
        {
            player.Stop();
            player.Close();
            player.Source = null;
            player.Volume = 0;
            player.Play();
            player.Visibility = Visibility.Hidden;
        }

        public void SwitchPlayer(bool showPlayer1)
        {
            if (showPlayer1)
            {
                player1.Visibility = Visibility.Visible;
                player2.Visibility = Visibility.Hidden;
            }
            else
            {
                player2.Visibility = Visibility.Visible;
                player1.Visibility = Visibility.Hidden;
            }
        }

        public IEnumerable<TrackItem> GetTrackItem(int frame)
        {
            return TimeLine.Items.Where(i => i.Offset <= frame && i.Offset + i.FrameWidth > frame);
        }
        
        private void LoadCheck()
        {
            if (_allVideos.Count == 0)
                return;

            TrackItem item = _allVideos.Peek();
            if ((item.Offset - TimeLine.Position) < MediaTools.TimeSpanToFrame(TimeSpan.FromSeconds(10), TimeLine.FrameRate))
            {
                DebugHelper.WriteLine("Should be Load!" + item.OriginalPath);

                _loadWaitVideos.Enqueue(item);

                _allVideos.Dequeue();
            }
        }
    }
}