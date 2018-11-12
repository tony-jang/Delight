﻿using Delight.Component.Common;
using Delight.Component.Controls;
using Delight.Component.PropertyEditor;
using System;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Delight.Component.ItemProperties
{
    public class VideoItemProperty : BaseTrackItemProperty
    {

        double _left = 0;
        [DesignElement(Category = "공통", DisplayName = "왼쪽 위치", Key = "Percentage")]
        public double Left
        {
            get => _left;
            set { _left = value; PropChanged("Left"); }
        }

        double _top = 0;

        [DesignElement(Category = "공통", DisplayName = "위쪽 위치", Key = "Percentage")]

        public double Top
        {
            get => _top;
            set { _top = value; PropChanged("Top"); }
        }

        double _opacity = 1;

        [DesignElement(Category = "공통", DisplayName = "투명도", Key = "Percentage")]
        public double Opacity
        {
            get => _opacity;
            set { _opacity = value; PropChanged("Opacity"); }
        }

        bool _chromaKeyUse = false;

        [DesignElement(Category = "크로마키", DisplayName = "사용")]
        public bool ChromaKeyUse
        {
            get => _chromaKeyUse;
            set { _chromaKeyUse = value; PropChanged("ChromaKeyUse"); }
        }

        Brush _chromaKeyColor = Brushes.Green;

        [DesignElement(Category ="크로마키", DisplayName ="색상")]
        public Brush ChromaKeyColor
        {
            get => _chromaKeyColor;
            set { _chromaKeyColor = value; PropChanged("ChromaKeyColor"); }
        }
        
        Percentage _chromaKeyUsage = new Percentage(0.4, 0, 1);

        [DesignElement(Category = "크로마키", DisplayName = "사용도", Key = "Percentage")]
        public double ChromaKeyUsage
        {
            get => _chromaKeyUsage;
            set { _chromaKeyUsage = value; PropChanged("ChromaKeyUsage"); }
        }



        private void Test()
        {
        }
    }
}
