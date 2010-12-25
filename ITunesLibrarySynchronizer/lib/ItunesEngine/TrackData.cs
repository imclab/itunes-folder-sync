using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Collections;

namespace ITunesLibrarySynchronizer.lib.ItunesEngine
{    
    public class TrackData
    {
        public long TrackId { get; set; }
        public string Name { get; set; }
        public string Artist { get; set; }
        public string Composer { get; set; }
        public string Album { get; set; }
        public string Genre { get; set; }
        public string Kind { get; set; }
        public long Size { get; set; }
        public long TotalTime { get; set; }
        public int Year { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        public int BitRate { get; set; }
        public int SampleRate { get; set; }
        public string Comments { get; set; }
        public int PlayCount { get; set; }
        public long PlayDate { get; set; }
        public DateTime PlayDateUtc { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int ArtworkCount { get; set; }
        public string PersistendId { get; set; }
        public bool IsExplicit { get; set; }
        public string TrackType { get; set; }
        public bool IsPodcast { get; set; }
        public string Location { get; set; }
        public int FileFolderCount { get; set; }
        public int LibraryFolderCount { get; set; }
    }
    
    public class TrackDataCreator
    {
        private static Hashtable _propertyCache;

        public static TrackData CreateTrackData(XElement data)
        {
            var instance = (TrackData)Activator.CreateInstance(typeof(TrackData));
            if (_propertyCache == null) _propertyCache = new Hashtable();

            foreach (var key in data.Elements("key"))
            {
                var valueData = key.NextNode as XElement;
                // find class member for node
                var keyName = key.Value.Replace(" ", String.Empty);
                
                // save property to Hashtable cache if not yet added
                if (!_propertyCache.ContainsKey(keyName))
                {
                    var member = instance.GetType().GetProperty(keyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                    if (member == null) continue;
                    _propertyCache.Add(keyName, member);
                }

                var pi = _propertyCache[keyName] as PropertyInfo;
                // assign converted value
                pi.SetValue(instance, Convert.ChangeType(valueData.Value, pi.PropertyType), null);
            }
            return instance;
        }

        private static bool FindMemberFilterDelegate(MemberInfo mi, object objToSearch)
        {
            if (mi.Name.ToString().ToLower() == objToSearch.ToString().ToLower())
                return true;
            else
                return false;

        }
    }
}
