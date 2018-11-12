﻿using Delight.Component.Converters;
using Delight.Component.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Delight.Component.Controls
{
    [TemplatePart(Name = "PART_slider", Type = typeof(Slider))]
    [TemplatePart(Name = "PART_valueBox", Type = typeof(TextBox))]
    [Setter(Key = "BytePercentage", Type = typeof(double))]
    public class BytePercentageSetter : BaseSetter
    {
        public BytePercentageSetter(object[] target, PropertyInfo[] pi) : base(target, pi)
        {
        }

        
        Slider slider;
        TextBox valueBox;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            slider = GetTemplateChild<Slider>("PART_slider");
            valueBox = GetTemplateChild<TextBox>("PART_valueBox");

            BindingHelper.SetBinding(
                this, ValueProperty,
                slider, Slider.ValueProperty,
                converter: new ByteToPercentageConverter());

            BindingHelper.SetBinding(
                this, ValueProperty,
                valueBox, TextBox.TextProperty,
                converter: new ByteToPercentageStringConverter());
        }

        protected override void OnDispose()
        {
            if (slider == null || valueBox == null)
                return;

            BindingOperations.ClearAllBindings(slider);
            BindingOperations.ClearAllBindings(valueBox);

            valueBox = null;
            slider = null;
        }
        
    }
}
