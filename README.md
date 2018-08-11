```
The wind howled by me
As I sat and watched the world;
All but powerless
```

## CantiLib

### Prerequisites

You need the `dotnet` command line tools.

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

* Enter the `Daemon` directory

* Run `dotnet run`

* This will launch the daemon.

### Testing

* Enter the `Tests` directory

* Run `dotnet test`

* This will run the test suite, and report any failures.

### Benchmarking

We want our benchmark to be running at max speed, so we have a bit more of
an involved setup here.

* Enter the `Benchmark` directory

* Run `dotnet run --configuration Release`

You'll get an output something like this:

```
  CantiLib -> /home/zach/Code/turtlecoin/cs-turtlecoin/CantiLib/bin/Release/netstandard2.0/CantiLib.dll
  Benchmark -> /home/zach/Code/turtlecoin/cs-turtlecoin/Benchmark/bin/Release/netcoreapp2.1/Benchmark.dll
  Benchmark -> /home/zach/Code/turtlecoin/cs-turtlecoin/Benchmark/bin/Release/netcoreapp2.1/publish/
```

The second line gives us the location of the dll to run. Since we're already in the `Benchmark` directory, we need to run:

* `dotnet bin/Release/netcoreapp2.1/Benchmark.dll`

* This will launch the benchmark program.

Fun fact - this .dll is cross platform. You can copy it to another operating system and run it with dotnet in the same way.

### Experimenting

If you want to test out a few functions without having to set up a whole new
project, we have provided the `TestZone` project for you to do this in.

* Enter the `TestZone` directory

* Run `dotnet run`

* Change some code you want to experiment with

* Run `dotnet run` again.

* Happy Hacking!

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
* Adrian Herridge (For his swift version of AES file encryption)
* CodIsAFish (For his C# modification of Adrian Herridge's AES file encryption, which we modified)

If we have used your software and incorrectly attributed you, or not attributed you, please let us know!
