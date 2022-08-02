# Invisible Collector SDK for .NET

[![Build status](https://ci.appveyor.com/api/projects/status/bvvchuog648l3dlo?svg=true)](https://ci.appveyor.com/project/jmadureira/invisiblecollectorlib)
[![NuGet](https://img.shields.io/nuget/v/InvisibleCollectorLib.svg?label=NuGet&style=flat-square)](https://www.nuget.org/packages/InvisibleCollectorLib/)
[![Issues](https://img.shields.io/github/issues/invisiblecloud/InvisibleCollectorLib.svg?style=flat-square)](https://github.com/invisiblecloud/InvisibleCollectorLib/issues)

Check the [API documentation](https://invisiblecloud.github.io/InvisibleCollectorLib/)

#### Download

Download the [latest release] or via [NuGet].

## Docs Build Instructions

TO build the docs you need to have mono installed

### For Linux and Mac (bash)

First make sure you have a clean repository (run `git clean -dfx`)

```bash
cd docfx # from project root
./docs.sh install # downloads docfx binary
./docs.sh build # builds the static api docs pages, needs mono, this will build into /docs

# OR
./docs.sh serve # servers the pages with the correct sidebar, for testing
```

### For Windows (cmd)

[docfx](https://dotnet.github.io/docfx/) should be installed before running this script:

```cmd
cd docfx
docfx docfx.jsonmono docfx/docfx.exe docfx.json
```

## Build Instructions

Node: on windows replace `-` with `/` on flags

To build from command line run:
```bash
$ msbuild 
```

or to build for release:
```bash
$ msbuild -p:Configuration=Release
```

To create a nuget package run:
```bash
$ msbuild -t:pack -p:Configuration=Release
```

or with a specific version:

```bash
$ msbuild -t:pack -p:Configuration=Release -p:Version=1.2.3
```

The generated `.nukpg` will be `/InvoiceCaptureLib/bin/Release/InvoiceCaptureLib<version>.nupkg`

## License

[MIT]

[latest release]: https://github.com/invisiblecloud/InvisibleCollectorLib/releases
[NuGet]: https://www.nuget.org/packages/InvoiceCaptureLib/
[MIT]: https://github.com/invisiblecloud/InvisibleCollectorLib/blob/master/LICENSE
