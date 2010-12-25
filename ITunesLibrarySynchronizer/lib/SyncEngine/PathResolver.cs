using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITunesLibrarySynchronizer.lib.ItunesEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;

namespace ITunesLibrarySynchronizer.lib.SyncEngine
{
    public class PathResolver : IDisposable
    {
        private string _currentPattern
        {
            get
            {
                return Properties.Settings.Default.StructurePattern;
            }
        }
        private string _synchronizationRoot
        {
            get
            {
                return Properties.Settings.Default.SynchRootFolder;
            }
        }

        private TrackData _currentTrack;
        private Hashtable _mappings = new Hashtable();

        public PathResolver(TrackData trackData)
        {
            // fill mappings
            // TODO: think of some way to create those mappings automatically
            _mappings.Add("%artist%", "Artist");
            _mappings.Add("%genre%", "Genre");
            _mappings.Add("%album%", "Album");
            _mappings.Add("%year%", "Year");
            _mappings.Add("%dateadded", "DateAdded");

            // save current track
            _currentTrack = trackData;
        }

        private bool ValidateFolderPattern()
        {
            // make sure there are only allowed tags and chars in use
            var regex = new Regex("%[a-zA-Z0-9]+%");
            var matches = regex.Matches(_synchronizationRoot);

            foreach (Match match in matches)
                if (!_mappings.Contains(match.Value))
                    return false;

            return true;
        }

        public string ResolveSourcePath()
        {
            if (!Uri.IsWellFormedUriString(_currentTrack.Location, UriKind.Absolute))
                return String.Empty;

            var uriPath = new Uri(_currentTrack.Location);
            return SanitizeFilePath(uriPath.LocalPath.Replace(@"\\localhost\", String.Empty)); // fucking itunes adds host information to path...
        }

        public string BuildTargetPath()
        {
            if (!ValidateFolderPattern()) throw new ApplicationException("Path pattern contains illegal placeholders!");
            var uriPath = new Uri(_currentTrack.Location);
            // get filename from data object
            var filename = Path.GetFileName(uriPath.LocalPath);

            var targetPathPart = _currentPattern;

            foreach (var mapKey in _mappings.Keys)
            {
                // get membername of mapping
                var memberName = (string)_mappings[mapKey];

                // resolve member on trackdata object
                var propInfo = _currentTrack.GetType().GetProperty(memberName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                if (propInfo == null) continue;

                // get value of member and replace it on the target path
                var rawVal = propInfo.GetValue(_currentTrack, null);
                if (rawVal == null)
                    targetPathPart = targetPathPart.Replace((string)mapKey, "Unknown " + propInfo.Name);                
                else
                {
                    string value = rawVal.ToString();
                    if (propInfo.PropertyType == typeof(DateTime))
                        // apply special formatting for datetime types
                        value = ((DateTime)Convert.ChangeType(rawVal, typeof(DateTime))).ToString("YYYY-mm-dd");

                    targetPathPart = targetPathPart.Replace((string)mapKey, value);
                }
            }

            // build remaining parts of path
            var fullPath = new Uri(_synchronizationRoot + Path.DirectorySeparatorChar +
                targetPathPart.Replace(@"/",@"\") + Path.DirectorySeparatorChar + filename);
            return SanitizeFilePath(fullPath.LocalPath);
        }
        
        private string SanitizeFilePath(string input)
        {
            var filterArr = new char[] { '?','|','>','<','"' };

            foreach (var c in filterArr)
                input = input.Replace(c, '_');

            return input;
        }

        #region IDisposable Members

        public void Dispose()
        {
            _currentTrack = null;
        }

        #endregion
    }
}
