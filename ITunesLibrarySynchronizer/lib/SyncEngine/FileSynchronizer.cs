using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ITunesLibrarySynchronizer.lib.SyncEngine
{
    public class FileSynchronizer
    {
        public bool Synchronize(string source, string target, bool useHashCheck)
        {
            // check if both locations exist
            if (!File.Exists(source)) return false;

            // get target path
            var targetPath = Path.GetDirectoryName(target);
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            if (!File.Exists(target))
            {
                // copy file right away withouth checking hash
                File.Copy(source,target);
            }
            else
            {
                var fileIsEqual = false;
                // copy file using hashcode checking
                if (useHashCheck)
                {
                    using (var sSource = new FileStream(source, FileMode.Open))
                        using (var sTarget = new FileStream(target, FileMode.Open))
                            fileIsEqual = CompareStreams(sSource, sTarget);
                }

                if (!fileIsEqual)
                    File.Copy(source, target, !useHashCheck);
            }
            return true;
        }

        private bool CompareStreams(Stream s1, Stream s2)
        {
            const int bufferSize = 2048 * 2;

            var b1 = new byte[bufferSize];
            var b2 = new byte[bufferSize];

            while (true)
            {
                var c1 = s1.Read(b1, 0, bufferSize);
                var c2 = s2.Read(b2, 0, bufferSize);

                if (c1 != c2)
                    return false;

                if (c1 == 0)
                    return true;

                var iterations = (int)Math.Ceiling((double)c1 / sizeof(Int64));

                for (int i = 0; i < iterations; i++)
                    if (BitConverter.ToInt64(b1, i * sizeof(Int64)) != BitConverter.ToInt64(b2, i * sizeof(Int64)))
                        return false;
            }
            return true;
        }
    }
}
