﻿using Delight.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Delight.Controls
{
    [TemplatePart(Name = "mediaElement", Type = typeof(MediaElement))]
    public partial class MediaElementPro : Control, INotifyPropertyChanged
    {
        #region Properties
        #region MediaElement
        /// <summary>
        /// Play next item if current item fails.
        /// </summary>
        public bool PlayNextIfFails { get; set; } = false;
        /// <summary>
        /// Shuffle <see cref="MediaItems">MediaItems</see> playlist.
        /// </summary>
        public bool IsShuffle { get; set; } = false;

        bool _isPlaying = false;

        /// <summary>
        /// <see cref="MediaElementPro"/> is playing?!
        /// </summary>
        public bool IsPlaying
        {
            get => _isPlaying;
            private set
            {
                _isPlaying = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsPlaying"));
            }
        }

        /// <summary>
        /// Is next item available for playback.
        /// </summary>
        public bool IsNextItemAvailable
        {
            get
            {
                if (MediaItems == null || MediaItems != null && !MediaItems.Any())
                    return false;
                var nextSelection = (SelectedIndex + 1) % MediaItems.Count;
                if (nextSelection != 0)
                    return true;
                else return false;
            }
        }
        /// <summary>
        /// Is previouse item available for playback.
        /// </summary>
        public bool IsPreviousItemAvailable
        {
            get
            {
                if (MediaItems == null || MediaItems != null && !MediaItems.Any())
                    return false;
                var prevSelection = (SelectedIndex - 1) % MediaItems.Count;
                if (prevSelection >= 0)
                    return true;
                else return false;
            }
        }
        /// <summary>
        /// Media items. DON'T USE THIS TO ADD ITEM DIRECTLY!!!! default value is null.
        /// </summary>
        public List<Uri> MediaItems { get; private set; }
        /// <summary>
        /// Gets natural duration of the media.
        /// </summary>
        public Duration NaturalDuration => mediaElement.NaturalDuration;
        /// <summary>
        /// Gets or sets the current position of progress through the media's playback time.
        /// </summary>
        public TimeSpan Position
        {
            get => mediaElement.Position;
            set => mediaElement.Position = value;
        }
        public int NaturalVideoHeight => mediaElement.NaturalVideoHeight;
        public int NaturalVideoWidth => mediaElement.NaturalVideoWidth;
        public double BufferingProgress => mediaElement.BufferingProgress;
        public bool CanPause => mediaElement.CanPause;
        public bool HasAudio => mediaElement.HasAudio;
        public bool HasVideo => mediaElement.HasVideo;
        public bool IsBuffering => mediaElement.IsBuffering;
        public double DownloadProgress => mediaElement.DownloadProgress;
        /// <summary>
        /// Selected index of <see cref="MediaItems"/>, if it wasn't null!
        /// </summary>
        public int SelectedIndex { get; private set; } = -1;
        /// <summary>
        /// Selected item of <see cref="MediaItems"/>, if it wasn't null!
        /// </summary>
        public Uri SelectedItem { get; private set; }
        public MediaClock Clock
        {
            get => mediaElement.Clock;
            set => mediaElement.Clock = value;
        }



        public readonly static DependencyProperty SourceProperty = DependencyProperty.Register(
"Source", typeof(Uri), typeof(MediaElementPro),
new PropertyMetadata(null, SourcePropertyChanged));
        /// <summary>
        /// Video source uri.
        /// </summary>
        public Uri Source
        {
            get
            {
                return (Uri)GetValue(SourceProperty);
            }
            set
            {
                SetValue(SourceProperty, value);
            }
        }
        private static void SourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;
            var uri = (Uri)e.NewValue;
            try
            {
                Current.SetState(PlayerState.Opening);
                Current.mediaElement.Source = uri;
                //Current.SetState(PlayerState.Playing);
                //Current.Timer.Start();
            }
            catch { }
        }


        public readonly static DependencyProperty CurrentStateProperty = DependencyProperty.Register(
 "CurrentState", typeof(PlayerState), typeof(MediaElementPro),
 new PropertyMetadata(PlayerState.None, CurrentStatePropertyChanged));
        /// <summary>
        /// Current state of playback.
        /// </summary>
        public PlayerState CurrentState
        {
            get
            {
                return (PlayerState)GetValue(CurrentStateProperty);
            }
            private set
            {
                SetValue(CurrentStateProperty, value);
            }
        }
        private static void CurrentStatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            //if (e.NewValue == null)
            //    return;
            //var url = (PlayerState)e.NewValue;
        }


        public readonly static DependencyProperty IsLoopProperty = DependencyProperty.Register(
 "IsLoop", typeof(bool), typeof(MediaElementPro),
 new PropertyMetadata(false, IsLoopPropertyChanged));
        /// <summary>
        /// Loop the <see cref="MediaItems"/>, if it wasn't null!
        /// </summary>
        public bool IsLoop
        {
            get
            {
                return (bool)GetValue(IsLoopProperty);
            }
            set
            {
                SetValue(IsLoopProperty, value);
            }
        }
        private static void IsLoopPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            //if (e.NewValue == null)
            //    return;
            //var url = (bool)e.NewValue;
        }


        //       public readonly static DependencyProperty AutoPlayProperty = DependencyProperty.Register(
        //"AutoPlay", typeof(bool), typeof(MediaElementPro),
        //new PropertyMetadata(false, AutoPlayPropertyChanged));
        //       /// <summary>
        //       /// NOT COMPLETE
        //       /// </summary>
        //       public bool AutoPlay
        //       {
        //           get
        //           {
        //               return (bool)GetValue(AutoPlayProperty);
        //           }
        //           set
        //           {
        //               SetValue(AutoPlayProperty, value);
        //           }
        //       }
        //       private static void AutoPlayPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        //       {
        //           if (e.NewValue == null)
        //               return;
        //           var autoPlay = (bool)e.NewValue;
        //           //try
        //           //{
        //           //    Current.MediaElement.AutoPlay = autoPlay;
        //           //}
        //           //catch { }
        //       }


        public readonly static DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(MediaElementPro), new PropertyMetadata(default(Stretch), StretchPropertyChanged));
        public Stretch Stretch
        {
            get
            {
                return mediaElement.Stretch;
                //return (Stretch)GetValue(StretchProperty);
            }
            set
            {
                SetValue(StretchProperty, value);
            }
        }
        private static void StretchPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;
            var stretch = (Stretch)e.NewValue;
            try
            {
                Current.mediaElement.Stretch = stretch;
            }
            catch { }
        }



        public readonly static DependencyProperty StretchDirectionProperty = DependencyProperty.Register(
"StretchDirection", typeof(StretchDirection), typeof(MediaElementPro),
new PropertyMetadata(default(StretchDirection), StretchDirectionPropertyChanged));
        public StretchDirection StretchDirection
        {
            get
            {
                return mediaElement.StretchDirection;
                //return (StretchDirection)GetValue(StretchDirectionProperty);
            }
            set
            {
                SetValue(StretchDirectionProperty, value);
            }
        }
        private static void StretchDirectionPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;
            var stretchDir = (StretchDirection)e.NewValue;

            try
            {
                Current.mediaElement.StretchDirection = stretchDir;
            }
            catch { }
        }


        public readonly static DependencyProperty IsMutedProperty = DependencyProperty.Register(
"IsMuted", typeof(bool), typeof(MediaElementPro),
new PropertyMetadata(default(bool), IsMutedPropertyChanged));
        public bool IsMuted
        {
            get
            {
                return mediaElement.IsMuted;
                //return (bool)GetValue(IsMutedProperty);
            }
            set
            {
                SetValue(IsMutedProperty, value);
            }
        }
        private static void IsMutedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;
            var isMute = (bool)e.NewValue;

            try
            {
                Current.mediaElement.IsMuted = isMute;
            }
            catch { }
        }



        public readonly static DependencyProperty ScrubbingEnabledProperty = DependencyProperty.Register(
"ScrubbingEnabled", typeof(bool), typeof(MediaElementPro),
new PropertyMetadata(default(bool), ScrubbingEnabledPropertyChanged));
        public bool ScrubbingEnabled
        {
            get
            {
                return mediaElement.ScrubbingEnabled;
                //return (bool)GetValue(ScrubbingEnabledProperty);
            }
            set
            {
                SetValue(ScrubbingEnabledProperty, value);
            }
        }
        private static void ScrubbingEnabledPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;
            var scrubbing = (bool)e.NewValue;

            try
            {
                Current.mediaElement.ScrubbingEnabled = scrubbing;
            }
            catch { }
        }



        public readonly static DependencyProperty VolumeProperty = DependencyProperty.Register(
"Volume", typeof(double), typeof(MediaElementPro),
new PropertyMetadata(default(double), VolumePropertyChanged));
        public double VolumeEnabled
        {
            get
            {
                return mediaElement.Volume;
                //return (double)GetValue(VolumeProperty);
            }
            set
            {
                SetValue(VolumeProperty, value);
            }
        }
        private static void VolumePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;
            var volume = (double)e.NewValue;
            try
            {
                Current.mediaElement.Volume = volume;
            }
            catch { }
        }



        public readonly static DependencyProperty SpeedRatioProperty = DependencyProperty.Register(
"SpeedRatio", typeof(double), typeof(MediaElementPro),
new PropertyMetadata(default(double), SpeedRatioPropertyChanged));
        public double SpeedRatio
        {
            get
            {
                return mediaElement.SpeedRatio;
                //return (double)GetValue(SpeedRatioProperty);
            }
            set
            {
                SetValue(SpeedRatioProperty, value);
            }
        }
        private static void SpeedRatioPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;
            var speedRatio = (double)e.NewValue;
            try
            {
                Current.mediaElement.SpeedRatio = speedRatio;
            }
            catch { }
        }



        public readonly static DependencyProperty BalanceProperty = DependencyProperty.Register(
"Balance", typeof(double), typeof(MediaElementPro),
new PropertyMetadata(default(double), BalancePropertyChanged));
        public double Balance
        {
            get
            {
                return mediaElement.Balance;
                //return (double)GetValue(BalanceProperty);
            }
            set
            {
                SetValue(BalanceProperty, value);
            }
        }
        private static void BalancePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;
            var balance = (double)e.NewValue;
            try
            {
                Current.mediaElement.Balance = balance;
            }
            catch { }
        }



        #endregion


        #region Poster Image Properties
        //       public readonly static DependencyProperty PosterSourceProperty = DependencyProperty.Register(
        //"PosterSource", typeof(Uri), typeof(MediaElementPro),
        //new PropertyMetadata(null, SourcePropertyChanged));
        //       public Uri PosterSource
        //       {
        //           get
        //           {
        //               return (Uri)GetValue(PosterSourceProperty);
        //           }
        //           set
        //           {
        //               SetValue(PosterSourceProperty, value);
        //           }
        //       }
        //       private static void PosterSourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        //       {
        //           if (e.NewValue == null)
        //               return;
        //           var url = (Uri)e.NewValue;
        //           try
        //           {
        //               //SetSource(url);
        //           }
        //           catch { }
        //       }

        #endregion

        #endregion


        #region Events
        public event RoutedEventHandler MediaEnded;
        public event RoutedEventHandler MediaOpened;
        public event RoutedEventHandler MediaFailed;
        public event RoutedEventHandler BufferingStarted;
        public event RoutedEventHandler BufferingEnded;
        public event CurrentStateChangedHandler CurrentStateChanged;
        public event PositionChangedHandler PositionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        DispatcherTimer Timer = new DispatcherTimer();
        public static MediaElementPro Current;
        public MediaElementPro()
        {
            Current = this;
            
            Timer.Interval = TimeSpan.FromSeconds(.5);
            Timer.Tick += OnTimerTick;
        }

        MediaElement mediaElement;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mediaElement = GetTemplateChild("mediaElement") as MediaElement;

            mediaElement.LoadedBehavior = MediaState.Manual;
            mediaElement.UnloadedBehavior = MediaState.Manual;
            mediaElement.MediaEnded += OnMediaEnded;
            mediaElement.MediaFailed += OnMediaFailed;
            mediaElement.MediaOpened += OnMediaOpened;
            mediaElement.BufferingStarted += OnBufferingStarted;
            mediaElement.BufferingEnded += OnBufferingEnded;
        }


        private void OnBufferingStarted(object sender, RoutedEventArgs e)
        {
            BufferingStarted?.Invoke(this, e);
            SetState(PlayerState.Buffering);
        }

        private void OnBufferingEnded(object sender, RoutedEventArgs e)
        {
            BufferingEnded?.Invoke(this, e);
            //SetState(PlayerState.Playing);
        }

        private void OnMediaOpened(object sender, RoutedEventArgs e)
        {
            MediaOpened?.Invoke(this, e);
            SetState(PlayerState.Opened);
        }

        private void OnMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MediaFailed?.Invoke(this, e);
            SetState(PlayerState.Failed);
            if (PlayNextIfFails && MediaItems != null && MediaItems.Any())
            {
                Position = TimeSpan.FromSeconds(0);
                PlayNext();
                return;
            }
        }

        private void OnMediaEnded(object sender, RoutedEventArgs e)
        {
            MediaEnded?.Invoke(this, e);
            SetState(PlayerState.Ended);
            if (IsLoop)
            {
                Position = TimeSpan.FromSeconds(0);
                Play();
                return;
            }
            if (IsShuffle)
            {
                PlayShuffle();
                return;
            }
            if (MediaItems != null && MediaItems.Any())
            {
                PlayNext();
            }
        }

        #region Functions
        public void Play()
        {
            mediaElement.Play();
            Position = TimeSpan.FromMilliseconds(0);
            IsPlaying = true;
            Timer.Start();
        }
        public void Pause()
        {
            mediaElement.Pause();
            SetState(PlayerState.Paused);
            IsPlaying = false;
            Timer.Stop();
        }
        public void Stop()
        {
            mediaElement.Stop();
            SetState(PlayerState.Stoped);
            IsPlaying = false;
            Timer.Stop();
        }
        public void SetSource(Uri uri)
        {
            Source = uri;
        }
        public void SetMediaItems(List<Uri> mediaList)
        {
            MediaItems = mediaList;
            if (mediaList != null && mediaList.Any())
            {
                SelectedIndex = 0;
                Source = mediaList.FirstOrDefault();
            }

        }

        public void PlayNext()
        {
            lastPosition = TimeSpan.FromSeconds(0);
            if (MediaItems == null || MediaItems != null && !MediaItems.Any())
            {
                Play();
                return;
            }
            var nextSelection = (SelectedIndex + 1) % MediaItems.Count;
            SelectedIndex = nextSelection;
            SelectedItem = MediaItems[nextSelection];
            Source = SelectedItem;
            Position = TimeSpan.FromSeconds(0);
            Play();
            //SetState(PlayerState.Playing);
            //IsPlaying = true;
        }
        public void PlayPrevious()
        {
            lastPosition = TimeSpan.FromSeconds(0);
            if (MediaItems == null || MediaItems != null && !MediaItems.Any())
            {
                Play();
                return;
            }
            var prevSelection = (SelectedIndex - 1) % MediaItems.Count;
            if (prevSelection < 0)
                prevSelection = MediaItems.Count - 1;
            SelectedIndex = prevSelection;
            SelectedItem = MediaItems[prevSelection];
            Source = SelectedItem;
            Position = TimeSpan.FromSeconds(0);
            Play();
            //SetState(PlayerState.Playing);
            //IsPlaying = true;
        }
        public void PlayItem(int index)
        {
            lastPosition = TimeSpan.FromSeconds(0);
            if (index < 0 || MediaItems == null || MediaItems != null && !MediaItems.Any() ||
                MediaItems != null && MediaItems.Any() && index > MediaItems.Count)
            {
                return;
            }
            SelectedIndex = index;
            SelectedItem = MediaItems[index];
            Source = SelectedItem;
            Position = TimeSpan.FromSeconds(0);
            Play();
            //SetState(PlayerState.Playing);
            //IsPlaying = true;
        }
        public void PlayShuffle()
        {
            lastPosition = TimeSpan.FromSeconds(0);
            if (MediaItems == null || MediaItems != null && !MediaItems.Any())
            {
                Play();
                return;
            }
            var shuffle = new Random().Next(MediaItems.Count);
            SelectedIndex = shuffle;
            SelectedItem = MediaItems[shuffle];
            Source = SelectedItem;
            Position = TimeSpan.FromSeconds(0);
            Play();
            //SetState(PlayerState.Playing);
            //IsPlaying = true;
        }
        #endregion

        TimeSpan lastPosition = TimeSpan.FromSeconds(0);
        private void OnTimerTick(object sender, EventArgs e)
        {
            if (lastPosition != Position)
            {
                SetPosition(Position);
                SetState(PlayerState.Playing);
            }

            lastPosition = Position;
        }
        PlayerState lastState = default(PlayerState);
        //// private functions
        private void SetState(PlayerState state)
        {
            CurrentState = state;
            if (lastState != state)
                CurrentStateChanged?.Invoke(this, state);
            if (state == PlayerState.Playing)
                IsPlaying = true;
            else IsPlaying = false;
            //if (state == PlayerState.Buffering || state == PlayerState.Playing)
            //    Timer.Start();
            //else
            //    Timer.Stop();
            lastState = state;
        }
        private void SetPosition(TimeSpan time)
        {
            PositionChanged?.Invoke(this, time);
        }
    }
}
