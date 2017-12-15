using System;
using System.Net;
using System.Collections.Generic;

using Newtonsoft.Json;
namespace Week9PrismExampleApp.Models
{
    public class YoutubeResource
    {

            public ResourceType type = ResourceType.VIDEO;

        public string DefaultThumbnailURL { get; set; }

            //Video Info
            public string VideoTitle { get; set; }
            public string VideoId { get; set; }
            public string VideoDescription { get; set; }
            public string VideoChannel { get; set; }
            public string VideoThumbnail { get; set; }

            public int VideoViews { get; set; }
            public int VideoLikes { get; set; }

            //Channel Info
            public string ChannelTitle { get; set; }
            public string ChannelId { get; set; }
            public string ChannelDescription { get; set; }
            public string ChannelURL { get; set; }
            public List<ThumbnailMetaData> ChannelThumbnails = new List<ThumbnailMetaData>(); 
            public ChannelStats stats;
            public List<ChannelItem> ChannelItems { get; set; }

            //Playlist Info
            public string PlaylistTitle { get; set; }
            public string PlaylistId { get; set; }
            public string PlaylistDescription { get; set; }
            public string PlaylistChannelId { get; set; }
            public List<ThumbnailMetaData> PlaylistThumbnails = new List<ThumbnailMetaData>();


    }

    public class ThumbnailMetaData
    {
        public string url;
        public int width;
        public int height;
        public string key;

        public ThumbnailMetaData()
        {
            
        }

        public ThumbnailMetaData(string url, int width, int height, string key)
        {
            this.url = url;
            this.width = width;
            this.height = height;
            this.key = key;
        }
    }

    public class ChannelStats
    {
        public long viewCount;
        public long subscriberCount;
        public long videoCount;
    }

    public class ChannelItem
    {
        public string type;
        public string videoId;
        public string websiteUrl;
    }

    public enum ResourceType
    {
        VIDEO, CHANNEL, PLAYLIST
    }
}
