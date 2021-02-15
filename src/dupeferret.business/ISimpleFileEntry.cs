using System;

namespace dupeferret.business
{
    public interface ISimpleFileEntry 
    {
        string FQFN { get; }
        long Length { get; }
        string Name { get; }
        DateTime CreationTime { get; }
        DateTime LastWriteTime { get; }
    }
}