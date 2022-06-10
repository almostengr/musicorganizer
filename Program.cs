// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Almostengr.MusicOrganizer.DataTransferObjects;
using Almostengr.MusicOrganizer.Extensions;
using Newtonsoft.Json;

const string OLD_MUSIC_DIRECTORY = "/home/almostengineer/Downloads/oldMusic";
const string NEW_MUSIC_DIRECTORY = "/home/almostengineer/Downloads/newMusic";

if (Directory.Exists(NEW_MUSIC_DIRECTORY) == false)
{
    Directory.CreateDirectory(NEW_MUSIC_DIRECTORY);
}

if (Directory.Exists(OLD_MUSIC_DIRECTORY) == false)
{
    Console.WriteLine("Old music directory does not exist");
    return;
}

ProcessMusicFiles(OLD_MUSIC_DIRECTORY);


static void ProcessMusicFiles(string directoryName)
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
} // end method

static void UpdateAndMoveMusicFiles(string directory)
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
                WorkingDirectory = OLD_MUSIC_DIRECTORY,
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

        string artistDirectory = Path.Combine(NEW_MUSIC_DIRECTORY, metaData.ToArtistOrAlbumArtist());

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
            $"{metaData.Format.Tags.Track.ToTrackNumberOrDefault()}_{metaData.Format.Tags.Title.ToTrackTitleOrDefault()}_{metaData.ToArtistOrAlbumArtist()}.mp3");

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