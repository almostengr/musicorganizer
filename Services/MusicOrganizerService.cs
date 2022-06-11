using System.Diagnostics;
using Almostengr.MusicOrganizer.DataTransferObjects;
using Almostengr.MusicOrganizer.Extensions;
using Newtonsoft.Json;

namespace Almostengr.MusicOrganizer.Services
{
    public class MusicOrganizerService : IMusicOrganizerService
    {
        private readonly string _oldMusicDirectory;
        private readonly string _newMusicDirectory;

        public MusicOrganizerService(string oldMusicDirectory, string newMusicDirectory)
        {
            _oldMusicDirectory = oldMusicDirectory;
            _newMusicDirectory = newMusicDirectory;
        }

        public void CleanMusicCollection()
        {

            if (Directory.Exists(_newMusicDirectory) == false)
            {
                Directory.CreateDirectory(_newMusicDirectory);
            }

            ProcessMusicFiles(_oldMusicDirectory);
        }

        private void ProcessMusicFiles(string directoryName)
        {
            Console.WriteLine($"Checking contents of {directoryName}");

            if (Directory.Exists(directoryName) == false)
            {
                return;
            }

            foreach (var directory in Directory.GetDirectories(directoryName))
            {
                ProcessMusicFiles(directory);
            }

            UpdateAndMoveMusicFiles(directoryName);
        }

        private void UpdateAndMoveMusicFiles(string directory)
        {
            foreach (string musicFile in Directory.GetFiles(directory, "*mp3"))
            {
                Console.WriteLine($"Processing {musicFile}");

                // ffprobe -v quiet -print_format json -show_format -show_streams "music file.mp3"
                Process ffprobe = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \"/usr/bin/ffprobe -v quiet -print_format json -show_format \\\"{musicFile}\\\"\"",
                        WorkingDirectory = _oldMusicDirectory,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                ffprobe.Start();
                ffprobe.WaitForExit();

                string output = ffprobe.StandardOutput.ReadToEnd();

                ffprobe.Close();
                ffprobe.Dispose();

                FfprobeMetaDTO metaData = JsonConvert.DeserializeObject<FfprobeMetaDTO>(output);

                string artistDirectory = Path.Combine(_newMusicDirectory, metaData.ToArtistOrAlbumArtistOrDefault());

                if (Directory.Exists(artistDirectory) == false)
                {
                    Directory.CreateDirectory(artistDirectory);
                }

                string albumDirectory =
                    Path.Combine(artistDirectory, metaData.Format.Tags.Album.ToAlbumNameOrDefault());

                if (Directory.Exists(albumDirectory) == false)
                {
                    Directory.CreateDirectory(albumDirectory);
                }

                string newMusicFileName = Path.Combine(
                    albumDirectory,
                    $"{metaData.Format.Tags.Track.ToTrackNumberOrDefault()}_{metaData.Format.Tags.Title.ToTrackTitleOrDefault()}_{metaData.ToArtistOrAlbumArtistOrDefault()}.mp3");

                if (File.Exists(newMusicFileName))
                {
                    Console.WriteLine($"{newMusicFileName} already exists. {musicFile} was not moved and considered a duplicate.");
                    continue;
                }

                try
                {
                    File.Move(musicFile, newMusicFileName);
                    Console.WriteLine($"Moved {musicFile} to {newMusicFileName})");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to move file. {ex.Message}");
                }
            }
        } // end method

    }
}