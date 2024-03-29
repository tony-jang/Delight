﻿using Delight.Core.Sources;
using Delight.Core.Template;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace Delight.ViewModel
{
    public class ExternalSourceViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ExternalSourceViewModel()
        {
            //OpenFileDialog ofd = new OpenFileDialog();
            //if (ofd.ShowDialog().Value)
            //{
            //    LoadTemplates(ofd.FileName);
            //}

            Sources = new ObservableCollection<BaseSource>();

            Sources.Add(new UnitySource("대나무 숲", @"C:\Users\uutak\Downloads\대나무 숲.png"));
            //Sources.Add(new UnitySource("테스트", @"C:\Users\uutak\바탕 화면\SoMa\초반 인트로.jpg"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=zOcrB6ia_DA&ab_channel=DocOptic"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=4V1QTOqaELo&ab_channel=DANKO"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=n6ikm5qfx0g&ab_channel=AlexMercer"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=pBuZEGYXA6E&ab_channel=ibighit"));

            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=0pXYp72dwl0&list=PLMTfiLScYpP4a8MSHoyMcRmNdSwV7ghK9&ab_channel=MatthiasM.de"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=a0yaOFwKKJ8&list=PLMTfiLScYpP4a8MSHoyMcRmNdSwV7ghK9&index=4&ab_channel=floopyFX"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=Yq_fVXatChk&index=20&list=PLMTfiLScYpP4a8MSHoyMcRmNdSwV7ghK9&ab_channel=AlexRideout"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=PopWkg8-hsI&list=PLMTfiLScYpP4a8MSHoyMcRmNdSwV7ghK9&index=65&ab_channel=LuenWarneke"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=RzE62hW-w0s&index=4&list=PL11S5Ezd1D92ZLY5i1m4ly8OkAKYNJh6O&ab_channel=footageisland"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=ZFd7jVc7x2g&list=PLj6XzcqwRpN4CVMKhTUQi1jh36H3uSMnM&ab_channel=AAVFX"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=JMzfgPHLTxY&list=PLj6XzcqwRpN4CVMKhTUQi1jh36H3uSMnM&index=2&ab_channel=AAVFX"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=arwXMBEiyyk&list=PLj6XzcqwRpN4CVMKhTUQi1jh36H3uSMnM&index=6"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=2qWtx0RmEyA&index=17&list=PLj6XzcqwRpN4CVMKhTUQi1jh36H3uSMnM&ab_channel=AAVFX"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=brPVoQlFyx4&index=24&list=PLj6XzcqwRpN4CVMKhTUQi1jh36H3uSMnM&ab_channel=AAVFX"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=I6c097lMEiI&index=21&list=PLj6XzcqwRpN4CVMKhTUQi1jh36H3uSMnM&ab_channel=AAVFX"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=sdv-Uy4otZw&list=PLj6XzcqwRpN4CVMKhTUQi1jh36H3uSMnM&index=25&ab_channel=AAVFX"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=VgWfNG7E6dU&list=PLj6XzcqwRpN4CVMKhTUQi1jh36H3uSMnM&index=28&ab_channel=AAVFX"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=gWdv2t_AC-k&list=PLj6XzcqwRpN4CVMKhTUQi1jh36H3uSMnM&index=34&ab_channel=AAVFX"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=xvM86rb-Q-8&list=PLj6XzcqwRpN4CVMKhTUQi1jh36H3uSMnM&index=37&ab_channel=AAVFX"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=id8fQzm38mE&list=PLj6XzcqwRpN4CVMKhTUQi1jh36H3uSMnM&index=40"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=THSy-UHrUq4&list=PLj6XzcqwRpN4CVMKhTUQi1jh36H3uSMnM&index=45&ab_channel=AAVFX"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=Eco_Jh0mN0M&list=PLj6XzcqwRpN4CVMKhTUQi1jh36H3uSMnM&index=46&ab_channel=AAVFX"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=EjyzNY5qD3s&list=PLeRGwsngMKZzz1RhCT3HqA4HWjI8WvvWw&ab_channel=bestofgreenscreen"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=UyLKvhM8HlE&index=2&list=PLeRGwsngMKZzz1RhCT3HqA4HWjI8WvvWw&ab_channel=BrunoVitaPintucci"));

            //AddFromYoutubeSource(ys);
        }

        public void LoadTemplates(string filePath)
        {
            JsonSerializer serializer = new JsonSerializer();
            
            var reader = new StreamReader(filePath);
            var sources = serializer.Deserialize(reader, typeof(ExternalSources)) as ExternalSources;

            sources.Sources.ForEach(i => Sources.Add(i));
        }

        public ObservableCollection<BaseSource> Sources { get; private set; }

    }
}
