﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Delight
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko-KR");

            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;

            Unosquare.FFME.MediaElement.FFmpegDirectory = @"c:\ffmpeg";
        }
       
    }
}
