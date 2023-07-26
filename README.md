# Sources

Original repo : https://github.com/ABouveron/example-csharp-cryptographic-machine-files

# Official API interaction + machine file fallback if offline
*The commands listed often need to have root access on Linux, so execute them with `sudo`.*  

The following tutorial helps you to activate a license, verify it and check periodically that it is still valid. The licensing system works online and offline if you setup both parts.
## Initial activation:
* Fill the `.env` file with your credentials. You will find almost everything in the [settings](https://app.keygen.sh/settings) of your Keygen account. The public key is the `Ed25519 128-bit Verify Key`. If you are not trying to do special API requests through the program, you don't need an admin token.
* Go to [keygen.sh](https://keygen.sh) and create a new license (you will probably need to create a product and a policy before that). Copy the license key (not the ID).
* Execute the following command to get your machine fingerprint:
```shell
dotnet run fingerprint
```
The fingerprint will be written on the console. Copy it.

* Online activation:
  * Execute the following command to activate the license (replace `licenseKey` and `fingerprint` with the values you copied):
  ```shell
  dotnet run license licenseKey fingerprint
  ```
* Offline activation:
  * Execute the following command to get the serial number of your machine:
  ```shell
  dotnet run serialnumber
  ```
  * Go on an online **SHA3_512** hash generator (for example [this one](https://emn178.github.io/online-tools/sha3_512.html)) and compute the hash of the serial number you just got. Copy the hash.
  * Go on [keygen.sh](https://keygen.sh) and create a machine for the license you are trying to activate with as fingerprint the hash you just copied. 
  * Checkout the machine to get a machine file `machine.lic` and put it in the same folder as `Program.cs`. 
  * Then copy the license key (not the ID) and put it in a new file named `license.lic` in the same folder as `Program.cs`.
  * Execute the following command to activate the license:
  ```shell
  dotnet run license
  ```
## Verification:

If you want to have a system able to verify the license validity whether or not the machine has an internet access, you need to do the following steps:
* If you have activated the system online, a machine entity has been created on [keygen.sh](https://keygen.sh). 
  * Checkout this machine to get a machine file `machine.lic`. 
  * Copy the license key (not the ID) and put it in a new file named `license.lic`. 
  * Put the two files in the same folder as `Program.cs`.
* If you have activated the system offline, you don't need to do anything more besides copying the license key and the fingerprint of your machine on [keygen.sh](https://keygen.sh).

Once you have done that, you can execute the following command to verify the license (replace `licenseKey` and `fingerprint` with the values you copied):
```shell
dotnet run license licenseKey fingerprint
```


## Periodic check:
In case you want to check periodically that the license is still valid, you can execute the following command (replace `licenseKey` and `fingerprint` with the values you copied, and `timespan` with the time between each check in seconds):
```shell
dotnet run timecheck timespan licenseKey fingerprint
```

# Run basic offline file verification

## Example config:

**Linux / Windows:**

```shell 
dotnet run examples/license.lic examples/machine.lic 198e9fe586114844f6a4eaca5069b41a7ed43fb5a2df84892b69826d64573e39
```

## Normal config:

* Your fingerprint should be the hash of the serial number of your machine (you can execute the program to see it)
  computed with **SHA3_512** ([Online Converter](https://emn178.github.io/online-tools/sha3_512.html)).
* Fill the `.env` file with your [keygen.sh](https://keygen.sh) public key.
* Get your machine file on [keygen.sh](https://keygen.sh) and put the raw license key in a new file named `license.lic`.
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