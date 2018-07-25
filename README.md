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

* Install `libcurl-openssl-1.0` from the AUR - https://aur.archlinux.org/packages/libcurl-openssl-1.0/
* Download the pkgbuild from https://aur.archlinux.org/packages/dotnet-cli-git/
* Change the line 
```
"${pkgname}-${pkgver}.tar.gz::https://dotnetcli.blob.core.windows.net/dotnet/Sdk/master/dotnet-dev-linux-x64.latest.tar.gz"
```

to

```
"${pkgname}-${pkgver}.tar.gz::https://dotnetcli.blob.core.windows.net/dotnet/Sdk/2.1.301/dotnet-sdk-2.1.301-linux-x64.tar.gz"
```

And the line

```
'90bc8e1cc9c89fc94ec6a0264200e297e00371136f3b574eccfc077d40d3746d'
```

to

```
'SKIP'
```

(Hopefully this will be fixed later - the latest dotnet sdk is newer than the latest corefx, so you will see a result like `Failed to initialize CoreCLR, HRESULT: 0x80131523` when attempting to do anything without this fix.

* Proceed with installing the package (`makepkg -si`)

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

### License

The code base is licensed under the GNU GPL V3. See the LICENSE file for more
information.
