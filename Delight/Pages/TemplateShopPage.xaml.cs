﻿using Delight.Component.Common;
using Delight.Core.Stage.Components.Media;
using Delight.Core.Template;
using Delight.Core.Template.Items;
using Delight.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;

namespace Delight.Pages
{
    /// <summary>
    /// TemplateShopPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TemplateShopPage : UserControl
    {
        public TemplateShopPage()
        {
            InitializeComponent();
            InitializeViewModel();
            btnDownload.Click += BtnDownload_Click;
            cb.SelectionChanged += Cb_SelectionChanged;
        }

        private void Cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb.Items.Count == 0)
                return;

            if (cb.SelectedIndex != 0)
                cb.SelectedIndex = 0;
        }

        public void InitializeViewModel()
        {
            this.DataContext = GlobalViewModel.ExternalSourceViewModel;
            templates.ItemsSource = GlobalViewModel.ExternalSourceViewModel.Sources;
        }

        private void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (templates.SelectedItem is YoutubeSource source)
            {
                source.Download(cb.SelectedIndex);

                var item = FileCacheDictionary.GetPathFromId(source.Id);

                string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Delight", item.Value.Item1);

                GlobalViewModel.MainWindowViewModel.MediaItems.Add(new VideoMedia()
                {
                    Identifier = item.Key,
                    Time = MediaTools.GetMediaDuration(path),
                    Path = path,
                    Thumbnail = new Uri(source.ThumbnailUri),
                });
            }
        }

    }
}
