using System;

namespace dupeferret.business
{
    public class SimpleFileEntry : IComparable {
        public SimpleFileEntry(FileEntry fe)
        {
            this.FQFN = fe.FQFN;
            this.Name = fe.Info.Name;
            this.Length = fe.Info.Length;
            this.CreationTime = fe.Info.CreationTime;
            this.LastWriteTime = fe.Info.LastWriteTime;
        }

        public virtual string FQFN { get; private set; }
        public virtual long Length { get; private set; }
        public virtual string Name { get; private set; }
        public virtual DateTime CreationTime { get; private set; }
        public virtual DateTime LastWriteTime { get; private set; }

        public int CompareTo(object other)
        {
            if (other == null)                                  // q. other null?
                return 1;                                       // a. yes ... this comes afer null.

            if (this == other)                                  // q. comparing to self?
                return 0;                                       // a. yes .. they are the same.

            SimpleFileEntry otherEntry = other as SimpleFileEntry;                  

            if (otherEntry == null)                                                 // q. other a filentry?
                throw new ArgumentException("Object is not a SimpleFileEntry");     // a. no .. can't compare it.

            int rc = this.CreationTime.CompareTo(otherEntry.CreationTime);
            
            if (rc != 0)
                return rc;
            
            rc = this.LastWriteTime.CompareTo(otherEntry.LastWriteTime);

            if (rc != 0)
                return rc;
            
            return this.FQFN.CompareTo(otherEntry.FQFN);
        }
    }
}