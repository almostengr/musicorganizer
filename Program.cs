// See https://aka.ms/new-console-template for more information

using Almostengr.MusicOrganizer.Services;

if (args.Length != 4)
{
    Console.WriteLine("Not enough arguments passed");
    return;
}

string oldDirectory = string.Empty;
string newDirectory = string.Empty;

for (int i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "-old":
            oldDirectory = args[i + 1];
            break;

        case "-new":
            newDirectory = args[i + 1];
            break;

        default:
            break;
    }
}

if (string.IsNullOrEmpty(oldDirectory) || Directory.Exists(oldDirectory) == false)
{

    Console.WriteLine("Old music directory does not exist or is not valid.");
    return;
}

if (string.IsNullOrEmpty(newDirectory))
{
    Console.WriteLine("New music directory is not valid.");
    return;
}


IMusicOrganizerService musicOrganizerService = new MusicOrganizerService(oldDirectory, newDirectory);
musicOrganizerService.CleanMusicCollection();
