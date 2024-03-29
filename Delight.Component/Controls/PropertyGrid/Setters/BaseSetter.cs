﻿using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using Delight.Component.Extensions;
using Delight.Core.Extensions;

namespace Delight.Component.Controls
{
    public class BaseSetter : Control, ISetter
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyHelper.Register();

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public object[] Targets { get; private set; }

        public PropertyInfo[] TargetProperties { get; private set; }

        public Type PropertyType { get; private set; }

        public bool IsStable => MultiConverter.IsStable;

        public MultiPropertyConverter MultiConverter { get; private set; }

        ListViewItem listViewItem;
        bool isDisposed = false;

        public BaseSetter(object[] target, PropertyInfo[] pi)
        {
            this.Focusable = false;

            this.Targets = target;
            this.TargetProperties = pi;

            this.PropertyType = this.TargetProperties[0].PropertyType;

            InitializeMultiBinding();

            this.Loaded += BaseSetter_Loaded;
            this.Unloaded += BaseSetter_Unloaded;
        }

        private void InitializeMultiBinding()
        {
            MultiConverter = new MultiPropertyConverter(this.TargetProperties[0].PropertyType, this.Targets);

            OnCreateMultiConverter();

            var multiBinding = new MultiBinding()
            {
                Converter = MultiConverter,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.Default
            };
            
            for (int i = 0; i < this.Targets.Length; i++)
            {
                multiBinding.Bindings.Add(
                    new Binding(this.TargetProperties[i].Name)
                    {
                        Source = this.Targets[i],
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.Default
                    });
            }

            this.SetBinding(ValueProperty, multiBinding);
        }

        protected virtual void OnCreateMultiConverter()
        {
        }

        private void BaseSetter_Unloaded(object sender, RoutedEventArgs e)
        {
            if (listViewItem != null)
            {
                listViewItem.PreviewMouseLeftButtonDown -= ListViewItem_MouseLeftButtonDown;
                listViewItem = null;
            }
        }

        private void BaseSetter_Loaded(object sender, RoutedEventArgs e)
        {
            listViewItem = this.FindVisualParents<ListViewItem>().FirstOrDefault();

            if (listViewItem != null)
                listViewItem.PreviewMouseLeftButtonDown += ListViewItem_MouseLeftButtonDown;
        }

        private void ListViewItem_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OnSelected();
        }

        protected virtual void OnSelected()
        {
        }

        protected T GetTemplateChild<T>(string name)
            where T : DependencyObject
        {
            return (T)GetTemplateChild(name);
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;

                OnDispose();

                BindingOperations.ClearBinding(this, ValueProperty);

                Targets = null;
                TargetProperties = null;
            }
        }

        protected virtual void OnDispose()
        {
        }
    }
}