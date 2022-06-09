# Music Organizer

Years ago, my music library got reorganized in a way that it should not have. There was an option in the music
player that I was using at the time that allowed it to make copies of the music when the meta tags were updated 
instead of moving the original file to the new location based on the meta tags.

Thus I ended up with several versions of the same song. In some cases, up to 9 versions of the same song. Geesh. 
I attempted to use MusicBrainz to remove the duplicates and organize the library, but it did not do a great
job of this.

That's where this application comes in. It looks at the meta tag data for each of the files and organizes
them based on that meta data. When duplicate files are found, they are placed in a duplicates directory. 
Each file move is logged in the console for further analysis.
