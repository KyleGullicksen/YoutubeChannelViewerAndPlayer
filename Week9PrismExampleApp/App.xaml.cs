using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Practices.Unity;
using Prism.Unity;
using Week9PrismExampleApp.Views;
using Xamarin.Forms.Xaml;

namespace Week9PrismExampleApp
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer) { }

        protected override void OnInitialized()
        {
            InitializeComponent();

            //AppCenter.Start(ApiKeys.AnalyticsKeyiOS + ApiKeys.AnalyticsKeyAndroid,
                   //typeof(Analytics), typeof(Crashes));

            NavigationService.NavigateAsync($"MainPage");
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<MainPage>();
            Container.RegisterTypeForNavigation<SamplePageForNavigation>();
            Container.RegisterTypeForNavigation<ChannelPage>();
            Container.RegisterTypeForNavigation<PlaylistPage>();
		}

        public App()
        {
            
        }
    }
}

