# Sources

Original repo : https://github.com/ABouveron/example-csharp-cryptographic-machine-files

# How to run
## Example config:

**Linux / Windows:**
```shell 
dotnet run examples/license.lic examples/machine.lic 198e9fe586114844f6a4eaca5069b41a7ed43fb5a2df84892b69826d64573e39
```

## Normal config:
* Your fingerprint should be the hash of the serial number of your machine (you can execute the program to see it) computed with **SHA3_512** ([Online Converter](https://emn178.github.io/online-tools/sha3_512.html)).
* Replace the public key from [keygen.sh](keygen.sh) line 94 of `Program.cs`.
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
docker build . -t license-example-csharp
docker run -it license-example-csharp
```