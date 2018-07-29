namespace dupeferret.business
{
    public class BaseDirectoryEntry
    {
        public int Number
        {
            get; private set;
        }

        public string Directory
        {
            get; private set;
        }
        public BaseDirectoryEntry(int number, string directory)
        {
            Number = number; 
            Directory = directory;
        }
    }
}
