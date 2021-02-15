using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using dupeferret.business;
using System.Text.Json;

namespace dupeferret
{
    class program
    {
        static Traverser _traverser = new dupeferret.business.Traverser();

        static void Main(string[] args)
        {
            _traverser.RaiseFoundDirectoryEvent += ShowEvent;
            _traverser.RaiseDupeFoundEvent += ShowEvent;
            foreach(var arg in args)
            {
                _traverser.AddBaseDirectory(arg);
            }
            var dupeSets = _traverser.GetJsonFriendlyDupeSets();

            // foreach(var dupeSet in dupeSets)
            // {
            //     dupeSet.Sort(e => )
            // }

            Console.WriteLine(JsonSerializer.Serialize(dupeSets.Dupes));

            long dupesFound = 0;
            foreach(var dupeSet in dupeSets.Dupes)
            {
                dupesFound += (dupeSet.Count - 1);
            }
            Console.Error.WriteLine("{0} files checked. {1} dupes found", _traverser.UniqueFiles.Count, dupesFound);
        }

        private static void ShowEvent(object sender, EventMessageArgs e)
        {
            // Console.Error.WriteLine(e.Message);
        }
    }
}
