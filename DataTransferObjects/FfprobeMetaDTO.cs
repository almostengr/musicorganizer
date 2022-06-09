namespace Almostengr.MusicOrganizer.DataTransferObjects
{
    public record FfprobeMetaDTO
    {
        public Format Format { get; set; }
    }

    public record Format
    {
        // public string Filename { get; set; }
        public Tags Tags { get; set; }
    }

    public record Tags
    {
        public string Album { get; set; }
        public string Album_Artist { get; set; }
        // public string Artist { get; set; }
        // public string Artists { get; set; }
        // public string Composer { get; set; }
        // public string Date { get; set; }
        // public string Disc { get; set; }
        // public string Genre { get; set; }
        // public string Lyrics_XXX { get; set; }
        // public string Publisher { get; set; }
        // public string OriginalYear { get; set; }
        public string Title { get; set; }
        public string Track { get; set; }
        public string Track_Int
        {
            get
            {
                var split = Track.Split("/");
                return string.IsNullOrEmpty(split[0]) ? 0.ToString() : split[0];
            }
        }
    }
}