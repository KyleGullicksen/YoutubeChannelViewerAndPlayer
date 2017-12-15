using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using Week9PrismExampleApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Microsoft.AppCenter.Analytics;
using Week9PrismExampleApp.Constants;
using Xamarin.Forms.Xaml;
using Plugin.Share;

namespace Week9PrismExampleApp.ViewModels {

    public class MainPageViewModel: BindableBase,
    INavigationAware {
        private INavigationService _navigationService;

        //public DelegateCommand GetYoutubeInfoCommand { get; set; }
        public DelegateCommand < Xamarin.Forms.Entry > GetYoutubeInfoCommand {
            get;
            set;
        }

        public DelegateCommand<YoutubeResource> TappedCommand { get; set; }


        public MainPageViewModel(INavigationService navigationService) {
            _navigationService = navigationService;
            SelectedItem = new YoutubeResource();

            //GetYoutubeInfoCommand = new DelegateCommand(GetYoutubeInfo);
            GetYoutubeInfoCommand = new DelegateCommand < Xamarin.Forms.Entry > (HandleYoutubeInfoCommand);
            TappedCommand = new DelegateCommand<YoutubeResource>(HandleTappedCommand);
        }

        private string _userSearchTopic;
        public string userSearchTopic {
            get {
                return _userSearchTopic;
            }
            set {
                SetProperty(ref _userSearchTopic, value);
            }
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

                HandleTappedCommand(value);
            }
        }


        private ObservableCollection<YoutubeResource> _channelSearchResults = new ObservableCollection < YoutubeResource > ();
        public ObservableCollection < YoutubeResource > ChannelSearchResults {
            get {
                return _channelSearchResults;
            }
            set {
                SetProperty(ref _channelSearchResults, value);
            }
        }

        private ObservableCollection<YoutubeResource> _playlistSearchResults = new ObservableCollection<YoutubeResource>();
        public ObservableCollection<YoutubeResource> PlaylistSearchResults
        {
            get
            {
                return _playlistSearchResults;
            }
            set
            {
                SetProperty(ref _playlistSearchResults, value);
            }
        }


        private ObservableCollection<YoutubeResource> _videoSearchResults = new ObservableCollection<YoutubeResource>();
        public ObservableCollection<YoutubeResource> VideoSearchResults
        {
            get
            {
                return _videoSearchResults;
            }
            set
            {
                SetProperty(ref _videoSearchResults, value);
            }
        }

        private async void HandleTappedCommand(YoutubeResource resource)
        {
            switch(resource.type)
            {
                case ResourceType.CHANNEL:
                    var navParams = new NavigationParameters();
                    navParams.Add(Constants.ParameterKeys.YoutubeResource, resource);
                    await _navigationService.NavigateAsync("ChannelPage", navParams);
                    break;
                case ResourceType.VIDEO:
                    await CrossShare.Current.OpenBrowser("https://www.youtube.com/watch?v=" + resource.VideoId);
                    break;
                case ResourceType.PLAYLIST:
                    //var navParams = new NavigationParameters();
                    //navParams.Add(Constants.ParameterKeys.YoutubeResource, resource);
                    //await _navigationService.NavigateAsync("ChannelPage", navParams);
                    break;
                default:
                    return;
            }
        }

        private void HandleYoutubeInfoCommand(Xamarin.Forms.Entry entry) {

            //_userSearchTopic = entry.Text;
            //_userSearchTopic = _userSearchTopic.ToLower();
            //entry.Text = "https://www.googleapis.com/youtube/v3/search?part=snippet,contentDetails&maxResults=5&q=" + _userSearchTopic + "&type=video" + "&key=" + ApiKey;
            ////GetYoutubeInfo(_userSearchTopic);

            string query = entry.Text.ToLower();
            userSearchTopic = query;

            VideoSearchResults.Clear();
            ChannelSearchResults.Clear();
            PlaylistSearchResults.Clear();

            GetSearchResultsFromYoutube(query, "channel");
            GetSearchResultsFromYoutube(query, "video");
            GetSearchResultsFromYoutube(query, "playlist");
        }


        protected string MakeURL(string query, string type)
        {
            //https://www.googleapis.com/youtube/v3/search?part=snippet&q=UC_x5XG1OV2P6uZZ5FSM9Ttw&type=channel&key=AIzaSyDJtnAbi6KrX0Mw9yA6_HDJbbkskEP1JHY
            string address = 
                "https://www.googleapis.com/youtube/v3/search?part=snippet&maxResults=25&q="
                + query
                + "&type="
                + type
                + "&key="
                + Constants.API.KEY;

            return address;
        }

        internal async void GetSearchResultsFromYoutube(string query, string type)
        {
            string address = MakeURL(query, type);

            //Open a GET conenction
            HttpClient httpClient = new HttpClient();
            string responseJson = await httpClient.GetStringAsync(address);

            //Deserialize the json payload
            JObject results = JsonConvert.DeserializeObject<dynamic>(responseJson);

            //Convert the results into our internal representation
            //We want to process the items from the results
            JArray items = results.Value<JArray>("items");
            YoutubeResource currentYoutubeResource;

            JObject id;

            foreach(JObject item in items)
            {
                currentYoutubeResource = new YoutubeResource();

                //extract the kind code
                id = item.Value<JObject>("id");
                currentYoutubeResource.type = GetType(id.Value<string>("kind"));

                //process this item based on its type (Also adds the final object to the appropiate collection)
                processItem(item, currentYoutubeResource.type, currentYoutubeResource);
            }
        }

        protected ResourceType GetType(string youtubeTypeCode)
        {
            if (youtubeTypeCode.EndsWith("channel"))
                return ResourceType.CHANNEL;
            else if (youtubeTypeCode.EndsWith("video"))
                return ResourceType.VIDEO;
            else if (youtubeTypeCode.EndsWith("playlist"))
                return ResourceType.PLAYLIST;
            else
                return ResourceType.VIDEO;
        }

        protected void processItem(JObject item, ResourceType type, YoutubeResource resource)
        {
            switch (type)
            {
                case ResourceType.CHANNEL:
                    processChannelItem(item, resource);
                    break;
                case ResourceType.VIDEO:
                    processVideoItem(item, resource);
                    break;
                case ResourceType.PLAYLIST:
                    processPlaylistItem(item, resource);
                    break;
                default:
                    return;
            }
        }

        protected void processVideoItem(JObject item, YoutubeResource resource)
        {
            JObject id = item.Value<JObject>("id");
            resource.VideoId = id.Value<string>("videoId");

            JObject snippet = item.Value<JObject>("snippet");
            JObject thumbnails = snippet.Value<JObject>("thumbnails");
            JObject defaultThumbnails = thumbnails.Value<JObject>("default");

            //TOM ADDED THIS - PLEASE DELETE IF NO WORK
            resource.VideoTitle = snippet.Value<string>("title");
            resource.VideoDescription = snippet.Value<string>("description");

            resource.VideoThumbnail = defaultThumbnails.Value<string>("url");
            resource.DefaultThumbnailURL = resource.VideoThumbnail;

            VideoSearchResults.Add(resource);
        }

        protected void processChannelItem(JObject item, YoutubeResource resource)
        {
            JObject snippet = item.Value<JObject>("snippet");

            resource.ChannelId = snippet.Value<string>("channelId");
            resource.ChannelTitle = snippet.Value<string>("title");
            resource.ChannelDescription = snippet.Value<string>("description");

            //Get the default thumbnail...
            JObject thumbnails = snippet.Value<JObject>("thumbnails");
            JObject defaultThumbnails = thumbnails.Value<JObject>("default");
            ThumbnailMetaData defaultThumbnailMeta = new ThumbnailMetaData();
            defaultThumbnailMeta.url = defaultThumbnails.Value<string>("url");  //...and thats all we really care about

            //And add her on
            resource.ChannelThumbnails.Add(defaultThumbnailMeta);
            resource.DefaultThumbnailURL = defaultThumbnailMeta.url;

            ChannelSearchResults.Add(resource);
        }

        protected void processPlaylistItem(JObject item, YoutubeResource resource)
        {
            JObject id = item.Value<JObject>("id");
            resource.PlaylistId = id.Value<string>("playlistId");

            JObject snippet = item.Value<JObject>("snippet");

            resource.PlaylistTitle = snippet.Value<string>("title");
            resource.PlaylistDescription = snippet.Value<string>("description");

            //Get the default thumbnail...
            JObject thumbnails = snippet.Value<JObject>("thumbnails");
            JObject defaultThumbnails = thumbnails.Value<JObject>("default");
            ThumbnailMetaData defaultThumbnailMeta = new ThumbnailMetaData();
            defaultThumbnailMeta.url = defaultThumbnails.Value<string>("url");  //...and thats all we really care about

            //And add her on
            resource.PlaylistThumbnails.Add(defaultThumbnailMeta);
            resource.DefaultThumbnailURL = defaultThumbnailMeta.url;

            PlaylistSearchResults.Add(resource);
        }

        public void OnNavigatedFrom(NavigationParameters parameters) {
            //throw new NotImplementedException();
        }

        public void OnNavigatedTo(NavigationParameters parameters) {
            //throw new NotImplementedException();
        }

        public void OnNavigatingTo(NavigationParameters parameters) {
            //throw new NotImplementedException();
        }
    }

}