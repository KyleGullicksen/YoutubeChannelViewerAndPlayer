using NUnit.Framework;
using System;
using Week9PrismExampleApp.ViewModels;
using Prism.Navigation;
using Moq;
using static Week9PrismExampleApp.Models.WeatherItemModel;

namespace Week9PrismExampleUnitTests
{
    [TestFixture]
    public class MainPageViewModelTests
    {
        MainPageViewModel mainPageViewModel;

        Mock<INavigationService> navigationServiceMock;

        [SetUp]
        public void Init()
        {
            navigationServiceMock = new Mock<INavigationService>();
            mainPageViewModel = new MainPageViewModel(navigationServiceMock.Object);
        }

        [Test]
        public void TestNavToNewPageCommandNavigateAsyncWithCorrectParameters()
        {
            // Arrange: create expected NavParameters, do Mock setup for navigation
            var expectedNavParams = new NavigationParameters();
            expectedNavParams.Add("NavFromPage", "MainPageViewModel");
            navigationServiceMock.Setup(
                ns => ns.NavigateAsync(It.IsAny<string>(),
                                     It.IsAny<NavigationParameters>(),
                                     It.IsAny<bool?>(),
                                       It.IsAny<bool>()));

            // Act: Call the method/command under test
            mainPageViewModel.NavToNewPageCommand.Execute();

            // Assert: Verify that proper navigate async call was made once
            navigationServiceMock.Verify(
                ns => ns.NavigateAsync("SamplePageForNavigation",
                                     expectedNavParams,
                                    false,
                                     true), Times.Once());
        }

		[Test]
		public void TestNavToMoreInfoPageCommandNavigateAsyncWithCorrectParameters()
		{
            WeatherItem weatherItemToPass = new WeatherItem();
			var expectedNavParams = new NavigationParameters();
            expectedNavParams.Add("WeatherItemInfo", weatherItemToPass);
			navigationServiceMock.Setup(
				ns => ns.NavigateAsync(It.IsAny<string>(),
									 It.IsAny<NavigationParameters>(),
									 It.IsAny<bool?>(),
									   It.IsAny<bool>()));

            mainPageViewModel.NavToMoreInfoPageCommand.Execute(weatherItemToPass);

			navigationServiceMock.Verify(
				ns => ns.NavigateAsync("MoreInfoPage",
									 expectedNavParams,
									false,
									 true), Times.Once());
		}
    }
}
