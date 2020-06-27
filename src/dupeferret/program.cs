﻿using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using dupeferret.business;

namespace dupeferret
{
    class program
    {
        static Traverser _traverser = new dupeferret.business.Traverser();

        static void Main(string[] args)
        {
            foreach(var arg in args)
            {
                _traverser.AddBaseDirectory(arg);
            }
            var dupeSets = _traverser.GetDupeSets();

            // foreach(var dupeSet in dupeSets)
            // {
            //     dupeSet.Sort(e => )
            // }

            long dupesFound = 0;
            foreach(var dupeSet in dupeSets)
            {
                dupesFound += (dupeSet.Count - 1);
            }
            Console.WriteLine("{0} files checked. {1} dupes found", _traverser.UniqueFiles.Count, dupesFound);
        }
    }
}
