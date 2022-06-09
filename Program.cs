// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Almostengr.MusicOrganizer.DataTransferObjects;
using Newtonsoft.Json;

int fileCounter = 0;
const string OLD_MUSIC_DIRECTORY = "/home/almostengineer/Downloads";
const string DUPLICATES_DIRECTORY = $"{OLD_MUSIC_DIRECTORY}/duplicates";
const string NEW_MUSIC_DIRECTORY = "/home/almostengineer/Downloads/newMusic";

const string FFPROBE_BINARY = "/usr/bin/ffprobe";

if (Directory.Exists(DUPLICATES_DIRECTORY) == false)
{
    Directory.CreateDirectory(DUPLICATES_DIRECTORY);
}

if (Directory.Exists(NEW_MUSIC_DIRECTORY) == false)
{
    Directory.CreateDirectory(NEW_MUSIC_DIRECTORY);
}

if (Directory.Exists(OLD_MUSIC_DIRECTORY) == false)
{
    Console.WriteLine("Old music directory does not exist");
    return;
}

string[] directories = Directory.GetDirectories(OLD_MUSIC_DIRECTORY);

foreach (string directory in directories)
{
    // string[] musicFiles = Directory.GetFiles(directory);
    string[] musicFiles = Directory.GetFiles(directory, "*mp3");

    foreach (string musicFile in musicFiles)
    {
        Console.WriteLine($"Processing {musicFile}");
        fileCounter++;

        // ffprobe -v quiet -print_format json -show_format -show_streams "music file.mp3"
        Process ffprobe = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = FFPROBE_BINARY,
                Arguments = $"-v quiet -print_format json -show_format \"{musicFile}\"",
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = false,
                UseShellExecute = true,
            }
        };

        ffprobe.Start();
        ffprobe.WaitForExit();

        FfprobeMetaDTO metaData = JsonConvert.DeserializeObject<FfprobeMetaDTO>(ffprobe.StandardError.ReadToEnd().ToString());

        string artistDirectory = Path.Combine(NEW_MUSIC_DIRECTORY, metaData.Format.Tags.Album_Artist);

        if (Directory.Exists(artistDirectory) == false)
        {
            Directory.CreateDirectory(artistDirectory);
        }

        string albumDirectory = Path.Combine(artistDirectory, metaData.Format.Tags.Album);

        if (Directory.Exists(albumDirectory) == false)
        {
            Directory.CreateDirectory(albumDirectory);
        }

        string newMusicFileName = Path.Combine(albumDirectory, $"{metaData.Format.Tags.Track_Int}_{metaData.Format.Tags.Title}_{metaData.Format.Tags.Album_Artist}.mp3");
        string destination = newMusicFileName;

        if (File.Exists(newMusicFileName) == false)
        {
            // destination = Path.Combine(DUPLICATES_DIRECTORY, musicFile);

            File.Move(musicFile, destination);
            Console.WriteLine($"Moved {musicFile} to {destination})");
        }
    }
}

Console.WriteLine($"Processed {fileCounter} mp3 files");
