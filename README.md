# Sources

Original repo : https://github.com/ABouveron/example-csharp-cryptographic-machine-files

# Test official and localhost API

The goal of this project is to be able to contact the API while online & offline. The official API can be contacted
while online, and the localhost one will be used when you are offline.  
You can only access the dashboard when online.

*Not functional on Windows.*

**Requirements:**

- Docker
- scripts working on Ubuntu (other distributions not tested)
- filling `install.env` with the credentials you want to use in localhost mode
- filling `.env` *\*_OFFICIAL* and *\*_LOCALHOST* variables with the appropriate values

OFFICIAL values can be found on [keygen.sh](https://app.keygen.sh/settings).  
A token for the API will be generated on the first run. Execute the run.sh script two times to get both tokens: one
while having Internet access and one without.

**Linux:**

```shell
source install.sh
```

```shell
source run.sh
```

In another terminal:

```shell
sudo dotnet run api
```

(You need to run it as root because sending an async ping is limited to root. If not running as root, the official API
cannot be contacted.)

# Run basic offline file verification

## Example config:

**Linux / Windows:**

```shell 
dotnet run examples/license.lic examples/machine.lic 198e9fe586114844f6a4eaca5069b41a7ed43fb5a2df84892b69826d64573e39
```

## Normal config:

* Your fingerprint should be the hash of the serial number of your machine (you can execute the program to see it)
  computed with **SHA3_512** ([Online Converter](https://emn178.github.io/online-tools/sha3_512.html)).
* Replace the public key from [keygen.sh](keygen.sh) line 96 of `Program.cs`.
* Get your machine file on [keygen.sh](keygen.sh) and put the raw license key in a new file named `license.lic`.
* Put your `machine.lic` & `license.lic` in the same folder as `Program.cs` and run:

**Linux:**

```shell
sudo dotnet run
```

(You need to run it as root because it needs to access `/dev/sda` to get the serial number of your machine.)

**Windows:**

```shell
dotnet run
```

## Docker:

```shell
docker build . -t offlicensecsharp
docker run -it offlicensecsharp
```

## Docker Compose:

```shell
docker compose up
```

# Obfuscation

Just put `obfuscation` as first argument of the run command to obfuscate the code:

```shell
dotnet run obfuscation
```
It will generate an obfuscated DLL in your `bin/Debug/net7.0/example-csharp-licensing-Docker_Secure` folder.