namespace dupeferret.business
{
    public class ErrorMessages
    {
        public static Formattable InvalidDirectory  = new Formattable("Invalid directory: {0}");
        public static Formattable DuplicateBaseDirectory = new Formattable("Duplicate base directory: {0}");
        public static Formattable DirectoryNotInFQFN = new Formattable("Directory {0} not in FQFN: ");
    }
}
