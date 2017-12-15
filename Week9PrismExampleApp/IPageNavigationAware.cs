using System;
using Week9PrismExampleApp.Models;

namespace Week9PrismExampleApp
{
    public interface IPageNavigationAware
    {
		void OnAppearing();
		void OnDisappearing();
    }
}
