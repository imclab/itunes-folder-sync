using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace schaermu.utils.linq.itunes
{
    public class Track
    {
        public long TrackID { get; set; }
        public string UniqueID { get; set; }
        public string Name { get; set; }
        public Artist Artist { get; set; }
        public Genre Genre { get; set; }
        public string Type { get; set; }
        public long FileSizeBytes { get; set; }
        public TimeSpan PlayTime { get; set; }
        public int Year { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        public int Bitrate { get; set; }
        public int SampleRate { get; set; }
        public long PlayCount { get; set; }
        public DateTime DatePlayed { get; set; }
        public DateTime DateReleased { get; set; }
        public bool IsPodcast { get; set; }
    }
}
