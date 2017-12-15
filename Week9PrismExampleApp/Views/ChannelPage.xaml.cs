using Xamarin.Forms;

namespace Week9PrismExampleApp.Views
{
    public partial class ChannelPage : ContentPage
    {
        public ChannelPage()
        {
            InitializeComponent();
        }

		protected override void OnAppearing()
		{
			(BindingContext as IPageNavigationAware)?.OnAppearing();
		}

		protected override void OnDisappearing()
		{
			(BindingContext as IPageNavigationAware)?.OnDisappearing();
		}
    }
}

