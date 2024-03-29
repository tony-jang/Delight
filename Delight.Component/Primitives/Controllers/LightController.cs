﻿using Delight.Component.Controls;
using Delight.Component.Extensions;
using Delight.Component.MovingLight.Effects;
using Delight.Component.MovingLight.Effects.Setters;
using Delight.Component.MovingLight.Effects.Values;
using Delight.Component.MovingLight.Effects.Values.Base;
using Delight.Component.Primitives.TimingReaders;
using Delight.Core.Extensions;
using Delight.Core.MovingLight;
using Delight.Core.Stage.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Delight.Component.Primitives.Controllers
{
    public class LightController : BaseController
    {
        public LightController(Track track, TimingReader reader) : base(track, reader, false)
        {
            _startPort = ((Track.TrackNumber - 1) * 16) + 1;
        }

        public override void ItemStarted(TrackItem sender, TimingEventArgs e)
        {
            _setterBoard = sender.GetTag<LightComponent>().SetterBoard;
            BeginAction(sender.Property as BaseLightSetterProperty);
        }

        public override void ItemEnded(TrackItem sender, TimingEventArgs e)
        {
            Stop();
        }

        public override void ItemPlaying(TrackItem sender, TimingEventArgs e)
        {
        }

        public override void ItemReady(TrackItem sender, TimingReadyEventArgs e)
        {
        }

        public override void TimeLineStopped()
        {
        }

        int _startPort;
        Stack<Thread> threads = new Stack<Thread>();

        SetterBoard _setterBoard;

        public void BeginAction(BaseLightSetterProperty property)
        {
            byte GetValue(BaseValue value)
            {
                if (value is StaticValue sv)
                {
                    return sv.Value;
                }
                else if (value is PropertyValue pv)
                {
                    byte b = (byte)property.GetType().GetRuntimeProperty(pv.PropertyName).GetValue(property);

                    return b;
                }
                else if (value is RelativeValue rv)
                {
                    byte b = (byte)property.GetType().GetRuntimeProperty(rv.PropertyName).GetValue(property);

                    if (rv.Sign == RelativeValue.RelativeSign.Minus)
                    {
                        int calculatedValue = b - rv.Value;

                        if (calculatedValue >= 0)
                            return (byte)calculatedValue;
                        else
                            return 0;
                    }
                    else
                    {
                        int calculatedValue = b + rv.Value;

                        if (calculatedValue <= 255)
                            return (byte)calculatedValue;
                        else
                            return 0;
                    }
                }

                throw new Exception("StaticValue 또는 PropertyValue가 아닙니다.");
            }

            DMXController lightController = new DMXController(_startPort);

            byte[] firstValue = new byte[16];

            if (_setterBoard.InitalizeValue != null)
            {
                foreach (ValueSetter inVSetter in _setterBoard.InitalizeValue)
                {
                    byte value = GetValue(inVSetter.Value);

                    lightController.SetValue(inVSetter.Port, value);
                    firstValue[(int)inVSetter.Port - 1] = value;
                    lightController.Send();
                }
            }

            Thread propThread = new Thread(() =>
            {
                IEnumerable<SetterProperty> staticProperties = _setterBoard.SetterProperties.Where(k => k.IsStatic);
                while (true)
                {
                    foreach (SetterProperty sp in staticProperties)
                    {
                        lightController.SetValue(sp.PortNumber, (byte)property.GetType().GetRuntimeProperty(sp.PropertyName).GetValue(property));
                    }

                    lightController.Send();
                    Thread.Sleep(100);
                }
            });

            threads.Push(propThread);

            foreach (SetterGroup setterGroup in _setterBoard.SetterGroups)
            {
                Thread thr = new Thread(() =>
                {
                    byte[] lastValue = new byte[16];
                    firstValue.CopyTo(lastValue, 0);

                    while (true)
                    {
                        for (int i = 0; i < setterGroup.Setters.Count; i++)
                        {
                            
                            MovingLight.Effects.Setters.BaseSetter setter = setterGroup.Setters[i];
                            if (setter is ValueSetter valueSetter)
                            {
                                byte value = GetValue(valueSetter.Value);
                                lightController.SetValue(valueSetter.Port, value);
                                lastValue[(int)valueSetter.Port - 1] = value;
                                lightController.Send();
                            }
                            else if (setter is ValuesSetter valuesSetter)
                            {
                                foreach (ValueSetter vSetter in valuesSetter.ValueSetters)
                                {
                                    byte value = GetValue(vSetter.Value);
                                    lightController.SetValue(vSetter.Port, value);
                                    lastValue[(int)vSetter.Port - 1] = value;
                                }

                                lightController.Send();
                            }
                            else if (setter is ContinueSetter continueSetter)
                            {
                                var max = (continueSetter.ContinueMilliseconds / property.Speed) / 16;

                                MovingLight.Effects.Setters.BaseSetter nextSetter = setterGroup.Setters[i + 1];
                                i++;

                                var copiedValue = new List<byte>(lastValue);

                                for (int j = 1; j <= max; j++)
                                {

                                    if (nextSetter is ValueSetter nVSetter)
                                    {
                                        int lastState = lastValue[(int)nVSetter.Port - 1];
                                        int k = GetValue(nVSetter.Value) - lastState;

                                        int finalValue = (int)((k / (double)max) * j);

                                        lightController.SetValue(nVSetter.Port, (byte)(lastState + finalValue));
                                        copiedValue[(int)nVSetter.Port - 1] = (byte)(lastState + finalValue);
                                        lightController.Send();
                                    }
                                    else if (nextSetter is ValuesSetter nVsSetter)
                                    {
                                        foreach (ValueSetter vSetter in nVsSetter.ValueSetters)
                                        {
                                            int lastState = lastValue[(int)vSetter.Port - 1];
                                            int k = GetValue(vSetter.Value) - lastState;

                                            int finalValue = (int)((k / (double)max) * j);

                                            lightController.SetValue(vSetter.Port, (byte)(lastState + finalValue));

                                            //Console.WriteLine((byte)(lastState + finalValue));

                                            copiedValue[(int)vSetter.Port - 1] = (byte)(lastState + finalValue);
                                        }
                                        lightController.Send();
                                    }
                                }

                                lastValue = copiedValue.ToArray();
                            }
                            else if (setter is WaitSetter waitSetter)
                            {
                                Thread.Sleep((int)(waitSetter.WaitMilliseconds / property.Speed));
                            }
                        }
                    }
                });
                threads.Push(thr);
            }

            threads.ForEach(i => i.Start());
        }

        public void Stop()
        {
            threads.ForEach(i => i.Abort());
            threads.Clear();
        }
    }

}
