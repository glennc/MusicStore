namespace MusicStore
{
    public class AppSettings
    {
        public string SiteTitle { get; set; }

        public bool CacheDbResults { get; set; } = true;

        public string AlbumsUrl {get; set;}

        public string CartUrl {get;set;}
    }
}