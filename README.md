```
The wind howled by me
As I sat and watched the world;
All but powerless
```

## CantiLib

### Prerequisites

You need the `dotnet` command line tools.

#### Windows

??? someone fill this in

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

* Enter the `Daemon` directory

* Run `dotnet run`

* This will launch the daemon.

### Testing

* Enter the `Tests` directory

* Run `dotnet test`

### Experimenting

If you want to test out a few functions without having to set up a whole new
project, we have provided the `TestZone` project for you to do this in.

* Enter the `TestZone` directory

* Run `dotnet run`

* Change some code you want to experiment with

* Run `dotnet run` again.

* Hopefully it compiled!

### Exploring

The main code base is available in the `CantiLib` folder.

## License

```
Copyright (c) 2018, The TurtleCoin Developers

Please see the included LICENSE file for more information.
```

## Thanks

Thank you to all the awesome developers who have made their software open source!

* The CryptoNote Developers, The ByteCoin Developers, The Monero Developers
* Diego Alejandro Gómez (For his C# version of Groestl)
* Nabil S. Al Ramli (For his C version of OpenAES which we ported)
* Dominik Reichl (For his C# version of Blake256)
* Daniel J. Bernstein / djb (For his C version of ED25519, Ref10, which we ported)
* Markku-Juhani O. Saarinen (For his C version of Keccak, which we ported)
* Alberto Fajardo (For his C# version of Skein)
* Pavel Kovalenko (For his tweaks to the C# version of Skein)
* Hongjun Wu (For his C version of JH, which we ported)

If we have used your software and incorrectly attributed you, or not attributed you, please let us know!
