# DupeFerret
Find duplicate files

Consider what constitutes a dupe:
* size is primary. (Done)
* hash of first 512 bites (Done)
* hash of complete file. 

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

# Project Title

DupeFerret - Find duplicate files across a set of directore

## Getting Started

TBA later

### Prerequisites

    * dotnet 3.1 or later with SDK. See https://dotnet.microsoft.com/download

### Installing

A step by step series of examples that tell you how to get a development env running

Say what the step will be

```
Give the example
```

And repeat

```
until finished
```

End with an example of getting some data out of the system or using it for a little demo

## Running the tests

Explain how to run the automated tests for this system

### Break down into end to end tests

Explain what these tests test and why

```
Give an example
```

### And coding style tests

Explain what these tests test and why

```
Give an example
```

## Deployment

Add additional notes about how to deploy this on a live system

## Built With

* [Dropwizard](http://www.dropwizard.io/1.0.2/docs/) - The web framework used
* [Maven](https://maven.apache.org/) - Dependency Management
* [ROME](https://rometools.github.io/rome/) - Used to generate RSS Feeds

## Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/PurpleBooth/b24679402957c63ec426) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/your/project/tags). 

## Authors

* **Billie Thompson** - *Initial work* - [PurpleBooth](https://github.com/PurpleBooth)

See also the list of [contributors](https://github.com/your/project/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Hat tip to anyone whose code was used
* Inspiration
* etc
