namespace Almostengr.MusicOrganizer.DataTransferObjects
{
    public record FfprobeMetaDTO
    {
        public Format Format { get; set; }
    }

    public record Format
    {
        public Tags Tags { get; set; }
    }

    public record Tags
    {
        public string Album { get; set; }
        public string Album_Artist { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Track { get; set; }
    }
}