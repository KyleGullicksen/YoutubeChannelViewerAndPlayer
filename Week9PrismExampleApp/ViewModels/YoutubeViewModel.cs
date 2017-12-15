using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using Week9PrismExampleApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Week9PrismExampleApp.Views;
using static Week9PrismExampleApp.Models.YoutubeResource;
using Microsoft.AppCenter.Analytics;

//Create an Observable Collection to hold all of the search results
//Search for Channel data
//Search for Playlist data
//search for Video data
//append all data into the Observable Collection
//click on Channel -> pass channel ID
//click on Playlist -> pass playlist id
//click on video -> open video in browser

namespace XamarinForms.ViewModels
{
    public class YoutubePageViewModel : BindableBase
    {
        INavigationService _navigationService;

        //public DelegateCommand GetYoutubeInfoCommand { get; set; }
        public DelegateCommand<Xamarin.Forms.Entry> GetYoutubeInfoCommand{get; set;}

        public void YoutubeViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            //GetYoutubeInfoCommand = new DelegateCommand(GetYoutubeInfo);
            GetYoutubeInfoCommand = new DelegateCommand<Xamarin.Forms.Entry>(HandleYoutubeInfoCommand);

        }

        private string _userSearchTopic;
        public string userSearchTopic
        {
            get { return _userSearchTopic; }
            set { SetProperty(ref _userSearchTopic, value); }
        }

        // use this link to get an api_key : https://console.developers.google.com/apis/api/youtube/
        private const string ApiKey = "AIzaSyDJtnAbi6KrX0Mw9yA6_HDJbbkskEP1JHY";

        private ObservableCollection<YoutubeResource> _searchResults = new ObservableCollection<YoutubeResource>();
        public ObservableCollection<YoutubeResource> SearchResults
        {
            get { return _searchResults; }
            set { SetProperty(ref _searchResults, value); }
        }

        private void HandleYoutubeInfoCommand(Xamarin.Forms.Entry entry)
        {
            _userSearchTopic = entry.Text;
            GetYoutubeInfo(_userSearchTopic);
        }

        internal async void GetYoutubeInfo(string _userSearchTopic)
        {
            var httpClientYoutube = new HttpClient();

            string searchChannelRequest = "https://www.googleapis.com/youtube/v3/search?part=snippet,statistics&maxResults=5&q="
                + _userSearchTopic
                + "&type=channel"
                + "&key="
                + ApiKey;

            string searchPlaylistRequest = "https://www.googleapis.com/youtube/v3/search?part=snippet&maxResults=5&q="
                + _userSearchTopic
                + "&type=playlist"
                + "&key="
                + ApiKey;

            string searchVideoRequest = "https://www.googleapis.com/youtube/v3/search?part=snippet,statistics&maxResults=5&q="
                + _userSearchTopic
                + "&type=video"
                + "&key="
                + ApiKey;

            var jsonChannel = await httpClientYoutube.GetStringAsync(searchChannelRequest);
            var jsonPlaylist = await httpClientYoutube.GetStringAsync(searchPlaylistRequest);
            var jsonVideo = await httpClientYoutube.GetStringAsync(searchVideoRequest);

            JObject channelResults = JsonConvert.DeserializeObject<dynamic>(jsonChannel);
            JObject playlistResults = JsonConvert.DeserializeObject<dynamic>(jsonPlaylist);
            JObject videoResults = JsonConvert.DeserializeObject<dynamic>(jsonVideo);

            var channelItems = channelResults.Value<JArray>("channelResults");
            var playlistItems = playlistResults.Value<JArray>("playlistResults");
            var videoItems = videoResults.Value<JArray>("videoResults");

            foreach (var item in channelItems)
            {
                YoutubeResource resource = new YoutubeResource();

                resource.type = ResourceType.CHANNEL;

                var snippet = item.Value<JObject>("snippet");
                var statistics = item.Value<JObject>("statistics");

                resource = ResourceAddThumbnailsChannel(resource, snippet, statistics);

                resource.ChannelURL = snippet.Value<string>("customUrl");
                resource.ChannelTitle = snippet.Value<string>("title");

                resource.ChannelId = item.Value<string>("id");

                resource.stats.subscriberCount = statistics.Value<long>("subscriberCount");
                resource.stats.viewCount = statistics.Value<long>("viewCount");
                resource.stats.videoCount = statistics.Value<long>("videoCount");

                //add resoruce to observable collection
                SearchResults.Add(resource);
            }
            foreach (var item in playlistItems)
            {
                YoutubeResource resource = new YoutubeResource();

                resource.type = ResourceType.PLAYLIST;

                var snippet = item.Value<JObject>("snippet");
                var statistics = item.Value<JObject>("statistics");

                resource = ResourceAddThumbnailsPlaylist(resource, snippet, statistics);

                resource.PlaylistTitle = snippet.Value<string>("title");
                resource.PlaylistId = item.Value<string>("id");
                resource.PlaylistDescription = snippet.Value<string>("description");
                resource.PlaylistChannelId = snippet.Value<string>("channelId");

                //add resource to observable collection
                SearchResults.Add(resource);
            }

            foreach (var item in videoItems)
            {
                YoutubeResource resource = new YoutubeResource();

                resource.type = ResourceType.VIDEO;

                var snippet = item.Value<JObject>("snippet");
                var statistics = item.Value<JObject>("statistics");

                resource.VideoId = item?.Value<string>("id");

                resource.VideoTitle = snippet.Value<string>("title");
                resource.VideoDescription = snippet.Value<string>("description");
                resource.VideoChannel = snippet.Value<string>("channelTitle");
                resource.VideoThumbnail = snippet?.Value<JObject>("thumbnails")?.Value<JObject>("default")?.Value<string>("url");

                resource.VideoLikes = statistics.Value<int>("likeCount");
                resource.VideoViews = statistics.Value<int>("viewCount");

                //add resource to observable collection
                SearchResults.Add(resource);
            }
        }

        public YoutubeResource ResourceAddThumbnailsChannel(YoutubeResource yR, JObject snippet, JObject statistics)
        {
            yR.ChannelThumbnails.Add(new ThumbnailMetaData
            (
                snippet?.Value<JObject>("thumbnails")?.Value<JObject>("default")?.Value<string>("url"),
                snippet.Value<JObject>("thumbnails").Value<JObject>("default").Value<int>("width"),
                snippet.Value<JObject>("thumbnails").Value<JObject>("default").Value<int>("height"),
                snippet?.Value<JObject>("thumbnails")?.Value<string>("default")
            ));

            yR.ChannelThumbnails.Add(new ThumbnailMetaData
            (
                snippet?.Value<JObject>("thumbnails")?.Value<JObject>("medium")?.Value<string>("url"),
                snippet.Value<JObject>("thumbnails").Value<JObject>("medium").Value<int>("width"),
                snippet.Value<JObject>("thumbnails").Value<JObject>("medium").Value<int>("height"),
                snippet?.Value<JObject>("thumbnails")?.Value<string>("medium")
            ));

            yR.ChannelThumbnails.Add(new ThumbnailMetaData
            (
                snippet?.Value<JObject>("thumbnails")?.Value<JObject>("high")?.Value<string>("url"),
                snippet.Value<JObject>("thumbnails").Value<JObject>("high").Value<int>("width"),
                snippet.Value<JObject>("thumbnails").Value<JObject>("high").Value<int>("height"),
                snippet?.Value<JObject>("thumbnails")?.Value<string>("high")
             ));

            var thumbnails = snippet.Value<JObject>("thumbnails");
            IDictionary<string, JToken> dictionary = thumbnails;

            //If the JsonObject has a standard field, extract it
            if (dictionary.ContainsKey("standard"))
            {
                yR.ChannelThumbnails.Add(new ThumbnailMetaData
                (
                    snippet?.Value<JObject>("thumbnails")?.Value<JObject>("standard")?.Value<string>("url"),
                    snippet.Value<JObject>("thumbnails").Value<JObject>("standard").Value<int>("width"),
                    snippet.Value<JObject>("thumbnails").Value<JObject>("standard").Value<int>("height"),
                    snippet?.Value<JObject>("thumbnails")?.Value<string>("standard")
                ));
            }

            //If the JsonObject has a maxres field, extract it
            if (dictionary.ContainsKey("maxres"))
            {
                yR.ChannelThumbnails.Add(new ThumbnailMetaData
                (snippet?.Value<JObject>("thumbnails")?.Value<JObject>("maxres")?.Value<string>("url"),
                        snippet.Value<JObject>("thumbnails").Value<JObject>("maxres").Value<int>("width"),
                        snippet.Value<JObject>("thumbnails").Value<JObject>("maxres").Value<int>("height"),
                        snippet?.Value<JObject>("thumbnails")?.Value<string>("maxres")
                ));
            }

            return yR;
        }

        public YoutubeResource ResourceAddThumbnailsPlaylist(YoutubeResource yR, JObject snippet, JObject statistics)
        {
            yR.PlaylistThumbnails.Add(new ThumbnailMetaData
            (
                snippet?.Value<JObject>("thumbnails")?.Value<JObject>("default")?.Value<string>("url"),
                snippet.Value<JObject>("thumbnails").Value<JObject>("default").Value<int>("width"),
                snippet.Value<JObject>("thumbnails").Value<JObject>("default").Value<int>("height"),
                snippet?.Value<JObject>("thumbnails")?.Value<string>("default")
            ));

            yR.PlaylistThumbnails.Add(new ThumbnailMetaData
            (
                snippet?.Value<JObject>("thumbnails")?.Value<JObject>("medium")?.Value<string>("url"),
                snippet.Value<JObject>("thumbnails").Value<JObject>("medium").Value<int>("width"),
                snippet.Value<JObject>("thumbnails").Value<JObject>("medium").Value<int>("height"),
                snippet?.Value<JObject>("thumbnails")?.Value<string>("medium")
            ));

            yR.PlaylistThumbnails.Add(new ThumbnailMetaData
            (
                snippet?.Value<JObject>("thumbnails")?.Value<JObject>("high")?.Value<string>("url"),
                snippet.Value<JObject>("thumbnails").Value<JObject>("high").Value<int>("width"),
                snippet.Value<JObject>("thumbnails").Value<JObject>("high").Value<int>("height"),
                snippet?.Value<JObject>("thumbnails")?.Value<string>("high")
             ));

            var thumbnails = snippet.Value<JObject>("thumbnails");
            IDictionary<string, JToken> dictionary = thumbnails;

            //If the JsonObject has a standard field, extract it
            if (dictionary.ContainsKey("standard"))
            {
                yR.PlaylistThumbnails.Add(new ThumbnailMetaData
                (
                    snippet?.Value<JObject>("thumbnails")?.Value<JObject>("standard")?.Value<string>("url"),
                    snippet.Value<JObject>("thumbnails").Value<JObject>("standard").Value<int>("width"),
                    snippet.Value<JObject>("thumbnails").Value<JObject>("standard").Value<int>("height"),
                    snippet?.Value<JObject>("thumbnails")?.Value<string>("standard")
                ));
            }

            //If the JsonObject has a maxres field, extract it
            if (dictionary.ContainsKey("maxres"))
            {
                yR.PlaylistThumbnails.Add(new ThumbnailMetaData
                (snippet?.Value<JObject>("thumbnails")?.Value<JObject>("maxres")?.Value<string>("url"),
                        snippet.Value<JObject>("thumbnails").Value<JObject>("maxres").Value<int>("width"),
                        snippet.Value<JObject>("thumbnails").Value<JObject>("maxres").Value<int>("height"),
                        snippet?.Value<JObject>("thumbnails")?.Value<string>("maxres")
                ));
            }

            return yR;
        }
    }
}
