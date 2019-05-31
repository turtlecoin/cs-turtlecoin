```
The wind howled by me
As I sat and watched the world;
All but powerless
```

## CantiLib

[![Build Status](https://travis-ci.org/turtlecoin/cs-turtlecoin.svg?branch=master)](https://travis-ci.org/turtlecoin/cs-turtlecoin)
[![Build status](https://ci.appveyor.com/api/projects/status/nxfp3iplp4fclfla?svg=true)](https://ci.appveyor.com/project/RocksteadyTC/cs-turtlecoin)

### Prerequisites

You need the `dotnet` command line tools.

Additionally, if the crypto portion of CantiLib is needed (as is in the case of CS-TurtleCoin), then you will also need to compile the turtlecoin-crypto-shared library from the link below and place the resulting file in the "Include" folder before compiling the solution.

https://github.com/turtlecoin/turtlecoin-crypto/tree/development

#### Windows

Install the dotnet sdk from here - https://www.microsoft.com/net/download

Once this is installed, you should have access to the `dotnet` executable from a standard cmd.

Alternatively you can attempt installing Visual Studio to install dotnet, however I found that it installed the wrong version for me.

Once you have the correct dotnet sdk installed (Verify by running `dotnet build` or another of the below commands via CLI) you can interact with the project via Visual Studio, if you like.

To do so, open up the CantiLib.sln file in Visual Studio. To run the daemon, just hit the green arrow labeled Daemon. To run the tests, choose the Test menu, then Run, then All Tests.

#### Ubuntu, Debian, Fedora, CentOS, RHEL, openSUSE

https://www.microsoft.com/net/download/linux-package-manager/ubuntu18-04/sdk-current

(There is a drop down box to choose the distro you are using)

#### Arch Linux

Yep, it's a special snowflake edition

* Install `dotnet-sdk` from the repos:

`pacman -S dotnet-sdk`

### Compiling

* Run `dotnet build` from this base directory.

### Running

* Enter the `CS-TurtleCoin` directory

* Run `dotnet run`

* This will launch the daemon.

### Exploring

The main code base is available in the `CantiLib` folder.

## License

```
Copyright (c) 2018-2019 The TurtleCoin Developers

Please see the included LICENSE file for more information.
```

## Thanks

Thank you to all the awesome developers who have made their software open source!

* The CryptoNote Developers, The ByteCoin Developers, The Monero Developers
* Andrey N.Sabelnikov (For his Epee serialization which was ported)
* The NewtonSoft Developers (For their excellent JSon library)
* The Microsoft Developers (For the Microsoft.Data.Sqlite.Core package)
* Eric Sink (For his bundle_green SQLite PCL package)

(Will add more as added)

If we have used your software and incorrectly attributed you, or not attributed you, please let us know!
