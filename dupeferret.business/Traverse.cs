using System;
using System.Collections.Generic;

namespace dupeferret.business
{
    public class Traverse
    {
        public string BaseDirectory { get; set; }

        public Traverse(string baseDir)
        {
            BaseDirectory = baseDir;
        }
    }
}
