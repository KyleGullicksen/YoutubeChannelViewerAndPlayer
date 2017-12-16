using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Week9PrismExampleApp.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Plugin.Share;
using Week9PrismExampleApp.Constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Week9PrismExampleApp.ViewModels
{
    public class PlaylistPageViewModel : BindableBase, INavigationAware
    {
        private string playlistURL = "";
        private YoutubeResource youtubeResource;

        public DelegateCommand GoBackCommand { get; set; }

        private DelegateCommand<YoutubeResource> _videoTappedCommand;
        public DelegateCommand<YoutubeResource> VideoTappedCommand
        {
            get { return _videoTappedCommand; }
            set { SetProperty(ref _videoTappedCommand, value); }
        }

        private ObservableCollection<YoutubeResource> _playlistVideos = new ObservableCollection<YoutubeResource>();
        public ObservableCollection<YoutubeResource> PlaylistVideos
        {
            get { return _playlistVideos; }
            set { SetProperty(ref _playlistVideos, value); }
        }

        //SelectedItem
        private YoutubeResource _selectedItem;
        public YoutubeResource SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;

                if (_selectedItem == null)
                    return;

                HandleVideoTappedCommand(value);
            }
        }

        INavigationService _navigationService;
        public PlaylistPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            VideoTappedCommand = new DelegateCommand<YoutubeResource>(HandleVideoTappedCommand);
            GoBackCommand = new DelegateCommand(GoBack);
        }

        public async void GoBack()
        {
            _navigationService.GoBackAsync();
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        //SearchResults -> here 
        //Expected parameters:
        //Youtube Resource
        async public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey(Constants.ParameterKeys.YoutubeResource))
            {
                youtubeResource = parameters[Constants.ParameterKeys.YoutubeResource] as YoutubeResource;
                playlistURL = makeURL(youtubeResource.PlaylistId);

                //Request the videos for this channel
                await GetVideoIdsFromChannelAsync();
            }
            else //Run our testing code
            {
                YoutubeResource testResource = new YoutubeResource();
                testResource.PlaylistId = "UC_x5XG1OV2P6uZZ5FSM9Ttw";
                playlistURL = makeURL(youtubeResource.PlaylistId);

                //Request the videos for this channel
                await GetVideoIdsFromChannelAsync();

            }
        }

        //Goin nowhere right now
        public void OnNavigatingTo(NavigationParameters parameters)
        {

        }

        protected void HandleVideoTappedCommand(YoutubeResource item)
        {
            if (item.type != ResourceType.VIDEO)
                return;

            CrossShare.Current.OpenBrowser("https://www.youtube.com/watch?v=" + item.VideoId);
        }

        protected string makeURL(string playlistID)
        {
            return
                "https://www.googleapis.com/youtube/v3/search?part=snippet&q="
                + playlistID
                + "&type=video"
                + "&key="
                + Constants.API.KEY;
        }


        //Request the resources in this channel from the youtube data api
        private async Task GetVideoIdsFromChannelAsync()
        {
            var httpClient = new HttpClient();
            var jsonResponse = await httpClient.GetStringAsync(playlistURL);

            try
            {
                JObject response = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                var items = response.Value<JArray>("items");
                YoutubeResource resource;

                foreach (JObject item in items)
                {
                    resource = new YoutubeResource();
                    processVideoItem(item, resource);
                    //PlaylistVideos.Add(resource);
                }
            }
            catch (Exception e)
            {
                //Log the issue, but just return an empty list to the user
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        protected void processVideoItem(JObject item, YoutubeResource resource)
        {
            JObject id = item.Value<JObject>("id");
            resource.VideoId = id.Value<string>("videoId");

            JObject snippet = item.Value<JObject>("snippet");
            JObject thumbnails = snippet.Value<JObject>("thumbnails");
            JObject defaultThumbnails = thumbnails.Value<JObject>("default");

            resource.VideoTitle = snippet.Value<string>("title");
            resource.VideoDescription = snippet.Value<string>("description");

            resource.VideoThumbnail = defaultThumbnails.Value<string>("url");
            resource.DefaultThumbnailURL = resource.VideoThumbnail;

            PlaylistVideos.Add(resource);
        }
    }
}
