using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections;
using System.Xml.Linq;

namespace ITunesLibrarySynchronizer.lib.ItunesEngine
{
    struct LibraryEntry
    {
        public long TrackId { get; set; }
        public TrackData Data { get; set; }
    }

    class LibraryParser
    {
        private List<LibraryEntry> _library;
        private Hashtable _rawData;
        private string _pathToLibrary;
        
        public int NumberOfEntries
        {
            get
            {
                return _rawData.Count;
            }
        }
        public string ItunesLibraryLocation
        {
            get
            {
                return _pathToLibrary;
            }
        }
        public List<LibraryEntry> Library
        {
            get
            {
                return _library;
            }
        }

        public event EventHandler OnEntryComplete;
        public event EventHandler OnReadComplete;
        public event EventHandler OnLoadComplete;

        public LibraryParser()
        {
            // initialize locator
            var loc = new LibraryLocator();

            try
            {
                if (!String.IsNullOrEmpty(Properties.Settings.Default.ITunesFolder))
                    _pathToLibrary = loc.Locate(Properties.Settings.Default.ITunesFolder);
                else
                    _pathToLibrary = loc.Locate();
            }
            catch (ApplicationException aEx)
            {
                // display error, itunes library could not be located
                throw new ApplicationException("ITunes library could not be located!");
            }

            try
            {
                // read data to hashtable
                LoadLibrary();
            }
            catch (ApplicationException aEx)
            {
                // display error, itunes library could not be loaded
                throw new ApplicationException("ITunes library could not be loaded!");
            }
        }

        public void ParseLibrary()
        {
            if (_library == null) _library = new List<LibraryEntry>();
            // fine-parse data from hashtable
            foreach (long key in _rawData.Keys)
            {
                // get data
                var xmlData = _rawData[key] as XElement;
                if (xmlData == null) continue;

                // parse xml element to structure
                _library.Add(new LibraryEntry { TrackId = key, Data = TrackDataCreator.CreateTrackData(xmlData) });
            }
        }

        private void LoadLibrary()
        {
            if (_rawData == null) _rawData = new Hashtable();

            // parse all entries
            var doc = XDocument.Load(_pathToLibrary);
            // get header information of library

            var entryRoot = doc.Root.Elements("dict").Elements("dict");

            // get entries
            long trackId = 0;
            XElement data = default(XElement);

            foreach (var dictEntry in entryRoot.Elements())
            {
                // check for key
                if (dictEntry.Name == "key")
                    // save track id
                    trackId = long.Parse(dictEntry.Value);

                if (dictEntry.Name == "dict")
                    // save sub entry as data
                    data = new XElement(dictEntry);

                // check if both values are filled in
                if (trackId > 0 && data != default(XElement))
                {
                    // save data to hashtable and reset
                    _rawData.Add(trackId, data);
                    trackId = 0;
                    data = default(XElement);
                }
            }

            if (OnReadComplete != null)
                OnReadComplete(this, new EventArgs());
        }
        
    }
}
