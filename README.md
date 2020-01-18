# DupeFerret
Find duplicate files

Consider what constitutes a dupe:
* size is primary. 
* md5 of first 512 bites, then whole file?
* name may play roled, but not required.

Key Contents:
* base 64 size?
* maybe size always part of key? If not same size, can't be dupe. hmm.

Things to do:
* Search directory.
* Build key on selected dupe entries
* Allow searching on only one type of file? JPG, etc.
* Allow saving dictionary?
* Build dictionary using keys, list<string> with full path.
* Each entry has paths of files with same sizes
* Ignore errors 
* Allow stopping at any time... get current results.
* When stopped or done, discard dupicates?
* Allow searching several places (including network drives)
* Copy one copy of duplicate file to a given directory, erase others. 
  * Give a final location
  * How to choose name of file? 
    * Maybe by type and/or modify date
    * Extract date from EXIF data or use file modified date?
* UI to show files (Icons?) 
  * What about other types, like text or excel/word/pdf?
