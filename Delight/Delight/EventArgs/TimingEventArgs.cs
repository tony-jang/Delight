﻿using Delight.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight
{
    public class TimingEventArgs
    {
        public TimingEventArgs(TimeLine timeLine, int frame)
        {
            TimeLine = timeLine;
            Frame = frame;
        }

        public int Frame { get; set; }

        public TimeLine TimeLine { get; set; }
    }

    public class TimingReadyEventArgs
    {
        public TimingReadyEventArgs(TimeLine timeLine, int frame, int remainFrame)
        {
            TimeLine = timeLine;
            Frame = frame;
            RemainFrame = remainFrame;
        }

        public int Frame { get; set; }

        public TimeLine TimeLine { get; set; }

        public int RemainFrame { get; set; }
    }

    public delegate void TimingDelegate(TrackItem sender, TimingEventArgs e);

    public delegate void TimingReadyDelegate(TrackItem sender, TimingReadyEventArgs e);


}
