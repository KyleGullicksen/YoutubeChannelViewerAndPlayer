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
    public class ChannelViewModel : BindableBase
    {
        private string channelURL = "";
        private YoutubeResource youtubeResource;

        private DelegateCommand<YoutubeResource> _videoTappedCommand;
        public DelegateCommand<YoutubeResource> VideoTappedCommand
        {
            get { return _videoTappedCommand; }
            set { SetProperty(ref _videoTappedCommand, value); }
        }

        private ObservableCollection<YoutubeResource> _channelVideos;
        public ObservableCollection<YoutubeResource> ChannelVideos
        {
            get { return _channelVideos; }
            set { SetProperty(ref _channelVideos, value); }
        }

        INavigationService _navigationService;
        public ChannelViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            VideoTappedCommand = new DelegateCommand<YoutubeResource>(HandleVideoTappedCommand);
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
                channelURL = makeURL(youtubeResource.PlaylistId);

                //Request the videos for this channel
                await GetVideoIdsFromChannelAsync();
            }
            else //Run our testing code
            {
                YoutubeResource testResource = new YoutubeResource();
                testResource.ChannelId = "UC_x5XG1OV2P6uZZ5FSM9Ttw";
                channelURL = makeURL(youtubeResource.PlaylistId);

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


        protected string makeURL(string channelID)
        {
            return
                "https://www.googleapis.com/youtube/v3/search?part=snippet&q="
                + channelID
                + "&type=channel"
                + "&key="
                + Constants.API.KEY;
        }


        //Request the resources in this channel from the youtube data api
        private async Task<ObservableCollection<YoutubeResource>> GetVideoIdsFromChannelAsync()
        {
            var httpClient = new HttpClient();
            var jsonResponse = await httpClient.GetStringAsync(channelURL);

            ObservableCollection<YoutubeResource> videoResources = new ObservableCollection<YoutubeResource>();
            YoutubeResource currentResource;
            string currentVideoId;

            try
            {
                JObject response = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                var items = response.Value<JArray>("items");

                foreach (var item in items)
                {
                    var id = item.Value<JObject>("id");
                    currentVideoId = id.Value<string>("videoId");

                    currentResource = new YoutubeResource
                    {
                        VideoId = currentVideoId
                    };
                    videoResources.Add(currentResource);
                }

                ChannelVideos = videoResources;
            }
            catch (Exception e)
            {
                //Log the issue, but just return an empty list to the user
                System.Diagnostics.Debug.WriteLine(e);
            }

            return videoResources; //or whatever we've got
        }



    }
}
