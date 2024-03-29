﻿using Delight.Component.ItemProperties;
using Delight.Core.Stage;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Delight.Component.Controls
{
    /// <summary>
    /// <see cref="TimeLine"/> 트랙에서 움직이는 컨트롤을 나타냅니다.
    /// </summary>
    /// <remarks>Represents a control that move in <see cref="TimeLine"/> Tracks.</remarks>
    [TemplatePart(Name = "dragLeft", Type = typeof(Rectangle))]
    [TemplatePart(Name = "dragMove", Type = typeof(Rectangle))]
    [TemplatePart(Name = "dragRight", Type = typeof(Rectangle))]
    public class TrackItem : Control, INotifyPropertyChanged
    {
        public TrackItem()
        {
            this.Style = FindResource("TrackItemStyle") as Style;

            KeyEvents = new ObservableCollection<Tuple<TimeSpan, object>>();
        }

        public TrackItem(SourceType sourceType) : this()
        {
            SourceType = sourceType;
        }

        public event MouseButtonEventHandler LeftSide_MouseLeftButtonDown;
        public event MouseButtonEventHandler RightSide_MouseLeftButtonDown;
        public event MouseButtonEventHandler MovingSide_MouseLeftButtonDown;

        public event MouseButtonEventHandler MouseRightButtonClick;
        public event MouseButtonEventHandler MouseLeftButtonClick;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        Rectangle leftSide, movingSide, rightSide;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            leftSide = GetTemplateChild("dragLeft") as Rectangle;
            movingSide = GetTemplateChild("dragMove") as Rectangle;
            rightSide = GetTemplateChild("dragRight") as Rectangle;

            leftSide.MouseLeftButtonDown += (s, e) => LeftSide_MouseLeftButtonDown?.Invoke(s, e);
            rightSide.MouseLeftButtonDown += (s, e) => RightSide_MouseLeftButtonDown?.Invoke(s, e);
            movingSide.MouseLeftButtonDown += (s, e) => MovingSide_MouseLeftButtonDown?.Invoke(s, e);

            this.MouseRightButtonDown += TrackItem_MouseRightButtonDown;
            this.MouseRightButtonUp += TrackItem_MouseRightButtonUp;

            this.MouseLeftButtonDown += TrackItem_MouseLeftButtonDown;
            this.MouseLeftButtonUp += TrackItem_MouseLeftButtonUp;
        }

        private void TrackItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isLeftDown)
            {
                MouseLeftButtonClick?.Invoke(sender, e);
            }

            isLeftDown = false;
        }

        private void TrackItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isLeftDown = true;
        }

        bool isLeftDown = false;
        bool isRightDown = false;

        private void TrackItem_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isRightDown)
            {
                MouseRightButtonClick?.Invoke(sender, e);
            }

            isRightDown = false;
        }

        private void TrackItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            isRightDown = true;
        }

        public SourceType SourceType { get; set; } = default(SourceType);

        //public bool IsVisibleLayer
        //{
        //    get => (bool)GetValue(IsVisibleLayerProperty);
        //}

        //public static readonly DependencyPropertyKey IsVisibleLayerKey = 
        //    DependencyProperty.RegisterReadOnly(nameof(IsVisibleLayer), 
        //        typeof(bool), 
        //        typeof(TrackType), 
        //        new PropertyMetadata(false)
        //        );

        //public static readonly DependencyProperty IsVisibleLayerProperty = IsVisibleLayerKey.DependencyProperty;

        public int Offset { get; set; }

        /// <summary>
        /// 앞쪽에서 시작할 오프셋을 가져오거나 설정합니다.
        /// </summary>
        public int ForwardOffset { get; set; }

        /// <summary>
        /// 뒤쪽에서 끝낼 오프셋을 가져오거나 설정합니다.
        /// </summary>
        public int BackwardOffset { get; set; }

        /// <summary>
        /// TrackItem에서 할당 가능한 최대 프레임을 가져오거나 설정합니다.
        /// </summary>
        public int MaxFrame { get; set; }

        /// <summary>
        /// 프레임 단위에서의 길이를 나타냅니다.
        /// </summary>
        public int FrameWidth { get; set; }

        public bool MaxSizeFixed { get; set; } = true;

        public string OriginalPath { get; set; }

        public static DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(TrackItem));

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        private object _dynamicProperty;

        public object DynamicProperty
        {
            get => _dynamicProperty;
            set
            {
                _dynamicProperty = value;
            }
        }


        public ObservableCollection<Tuple<TimeSpan, object>> KeyEvents { get; set; }

        private object _property;
        public object Property
        {
            get => _property;
            set
            {
                if (_property != null)
                    ((INotifyPropertyChanged)_property).PropertyChanged -= PropertyChanged;

                if (value is INotifyPropertyChanged changedValue)
                    changedValue.PropertyChanged += (s,e)=>
                    {
                        PropertyChanged?.Invoke(s, e);
                    };

                if (_property == null)
                {
                    _property = value;
                }
                else //if (_property is BaseLightSetterProperty baseSetterProperty)
                {
                    Type t = _property.GetType();

                    JObject obj = value as JObject;

                    foreach (JProperty jProp in obj.Properties())
                    {
                        PropertyInfo pi = t.GetProperty(jProp.Name);

                        if (pi.PropertyType == typeof(Brush))
                        {
                            pi.SetValue(_property, (new BrushConverter()).ConvertFromString(jProp.Value.ToString()));
                        }
                        else
                        {
                            var jValue = Convert.ChangeType(jProp.Value.ToObject<object>(), pi.PropertyType);
                            pi.SetValue(_property, jValue);
                        }
                    }
                }
            }
        }

        //public static DependencyProperty ColorThemeProperty = DependencyProperty.Register(nameof(ColorTheme), typeof(ColorTheme), typeof(TrackItem));

        //public ColorTheme ColorTheme
        //{
        //    get => (ColorTheme)GetValue(ColorThemeProperty);
        //    set => SetValue(ColorThemeProperty, value);
        //}

        public static DependencyProperty ThumbnailProperty = DependencyProperty.Register(nameof(Thumbnail), typeof(ImageSource), typeof(TrackItem));

        public ImageSource Thumbnail
        {
            get => GetValue(ThumbnailProperty) as ImageSource;
            set => SetValue(ThumbnailProperty, value);
        }

        public static DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(TrackItem));

        public string Text
        {
            get => GetValue(TextProperty) as string;
            set => SetValue(TextProperty, value);
        }
    }
}
