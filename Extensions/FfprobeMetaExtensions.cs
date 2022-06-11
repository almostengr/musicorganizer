using System.Text;
using Almostengr.MusicOrganizer.DataTransferObjects;

namespace Almostengr.MusicOrganizer.Extensions
{
    public static class FfprobeMetaExtensions
    {
        private const string DEFAULT_TRACK_NUMBER = "00";

        public static string ToAlbumNameOrDefault(this string albumName)
        {
            if (string.IsNullOrEmpty(albumName))
            {
                return "Unknown Album";
            }

            return albumName;
        }

        public static string ToTrackNumberOrDefault(this string trackNumber)
        {
            if (string.IsNullOrEmpty(trackNumber))
            {
                return DEFAULT_TRACK_NUMBER;
            }

            var split = trackNumber.Split("/");
            return string.IsNullOrEmpty(split[0]) ? DEFAULT_TRACK_NUMBER : split[0];
        }

        public static string ToArtistOrAlbumArtistOrDefault(this FfprobeMetaDTO metaDto)
        {
            if (string.IsNullOrEmpty(metaDto.Format.Tags.Album_Artist) == false)
            {
                return metaDto.Format.Tags.Album_Artist;
            }

            if (string.IsNullOrEmpty(metaDto.Format.Tags.Artist) == false)
            {
                return metaDto.Format.Tags.Artist;
            }

            return "Unknown Artist";
        }

        public static string ToTrackTitleOrDefault(this string trackTitle)
        {
            if (string.IsNullOrEmpty(trackTitle))
            {
                string chars = "0123456789";
                StringBuilder output = new StringBuilder();
                Random random = new Random();

                output.Append("Track");

                for (int i = 0; i < 10; i++)
                {
                    output.Append(chars[random.Next(chars.Length)]);
                }

                return output.ToString();
            }

            return trackTitle.Replace("/", "-")
                .Replace("\\", "-");
        }

    }
}