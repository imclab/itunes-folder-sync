using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ITunesLibrarySynchronizer.lib.ItunesEngine
{
    class LibraryLocator
    {
        private const string ITUNES_LIBRARY_FILENAME = "ITunes Music Library.xml";
        public string Locate()
        {
            // try to find itunes folder at default location
            return Locate(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic, Environment.SpecialFolderOption.Create));
        }
        public string Locate(string rootPath)
        {
            // search itunes library at given location
            try
            {
                foreach (string dir in Directory.GetDirectories(rootPath))
                {
                    string checkPath = dir + @"\" + ITUNES_LIBRARY_FILENAME;
                    if (File.Exists(checkPath))
                        return checkPath;
                }
                throw new ApplicationException("Could not locate Itunes library file at " + rootPath + "!");
            }
            catch (UnauthorizedAccessException UAEx)
            {
                throw new ApplicationException(UAEx.Message);
            }
            catch (PathTooLongException PathEx)
            {
                throw new ApplicationException(PathEx.Message);
            }
        }
    }
}
