# Invoice Capture SDK for .NET

[![Build status](https://ci.appveyor.com/api/projects/status/bvvchuog648l3dlo?svg=true)](https://ci.appveyor.com/project/jmadureira/invoicecapturelib)
[![NuGet](https://img.shields.io/nuget/v/InvoiceCaptureLib.svg?label=NuGet&style=flat-square)](https://www.nuget.org/packages/InvoiceCaptureLib/)
[![Issues](https://img.shields.io/github/issues/invisiblecloud/InvoiceCaptureLib.svg?style=flat-square)](https://github.com/invisiblecloud/InvoiceCaptureLib/issues)

#### Download

Download the [latest release] or via [NuGet].


## Docs Build Instructions

TO build the docs you need to have mono installed

### For Linux (bash)

```bash
cd docfx_project # from project root
./docs.sh install # downloads docfx binary
./docs.sh build # builds the statis api docs pages, needs mono, this will build into /docs
```

### For Windows (cmd)

[docfx](https://dotnet.github.io/docfx/) should be installed before running this script:

```cmd
cd docfx_project
docfx docfx.json
```

#### License

[MIT]

[latest release]: https://github.com/invisiblecloud/InvoiceCaptureLib/releases
[NuGet]: https://www.nuget.org/packages/InvoiceCaptureLib/
[MIT]: https://github.com/invisiblecloud/InvoiceCaptureLib/blob/master/LICENSE
