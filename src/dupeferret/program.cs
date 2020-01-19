using System;
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
                _traverser.GetAllFiles();
                _traverser.CleanSingles();
                foreach(var sameLengthSet in _traverser.FilesByLength.Values)
                {
                    var dupeSets = _traverser.FindPossibleDupes(sameLengthSet);

                    foreach(var key in dupeSets.Keys)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("-----------------------------------");
                        dupeSets[key].ForEach(entry => { Console.WriteLine($"{entry.Info.Length}: {entry.FQFN}"); });
                        Console.WriteLine("");
                    }
                }

                // foreach(var length in _traverser.FilesByLength.Keys)
                // {
                //     Console.WriteLine("FileLength: {0} -- {1}", length);
                //     Console.WriteLine("---------------------------------'");
                
                //     foreach(var fileEntry in _traverser.FilesByLength[length])
                //     {
                //         Console.WriteLine(fileEntry.FQFN);
                //     }
                // }
            }
        }
    }
}
