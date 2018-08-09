﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using Delight.Core.Common;
using Delight.Core.Extension;

namespace Delight.TimeLineComponents
{
    public class TimeLineTimer
    {
        public delegate void EmptyDelegate();
        public event EmptyDelegate Tick;

        Stopwatch sw;
        public FrameRate FrameRate { get; set; }

        int FrameRateInt => (int)(FrameRate.GetEnumAttribute<DefaultValueAttribute>().Value);

        public TimeLineTimer(FrameRate frameRate)
        {
            sw = new Stopwatch();
            FrameRate = frameRate;
        }

        Thread thr;

        public bool IsRunning => sw.IsRunning;

        public void Start()
        {
            sw.Restart();

            thr = new Thread(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                int lastValue = -1;
                while (true)
                {
                    long elapsed = sw.ElapsedMilliseconds % 1000;
                    int value = (int)(elapsed / (1000.0 / FrameRateInt));
                    //Console.WriteLine(value);
                    if (lastValue != value && value != FrameRateInt)
                    {
                        Tick?.Invoke();
                    }
                    lastValue = value;
                    Thread.Sleep(1);
                }
            });

            thr.Start();

        }

        public void Stop()
        {
            sw.Stop();
            thr?.Abort();
        }
    }
}