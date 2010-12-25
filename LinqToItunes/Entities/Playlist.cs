using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace schaermu.utils.linq.itunes
{
    public class Playlist
    {
        public long PlaylistID { get; set; }
        public string UniqueID { get; set; }
        public string Name { get; set; }
        IList<Track> Tracks { get; set; }
    }
}
