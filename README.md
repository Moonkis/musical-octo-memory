# musical-octo-memory (octo)
A webpage mirrorer for offline browsing.

The components are intentially kept small in size (and scope). Some classes can't be mocked easily (HttpClient, HtmlWeb) and relies on dependency injections using wrappers on the classes that would normally make it hard to write tests.

Abstractions are used when they give larger benefits but the hierarchy of abstractions is kept relativly flat overall. I tried to not overcomplicate the solution so that refactoring further is kept open and lightweight if needs/requirements changes.

# Getting Started

## Requirements
* .NET version 7.0 or greater.

## Run
1. Clone the repository.
2. Build the release configuration:

```
dotnet build --configuration <Release|Debug>
```

3. Configure the `octo.dll.config` located in the release folder `root > octo > bin > Release > net7.0`. Specifically the `OutputDirectory` and `Url` settings.
4. Run the project:
```
dotnet run --configuration <Release|Debug>
```

# Known Issues
* Css files are currently not parsed so fonts included there will not be downloaded. Mainly this affects the star icons of the rating system.
* External library links (JQuery for example) references are downloaded but links in the DOM needs to be updated to use the offline version of them for a full offline experience.