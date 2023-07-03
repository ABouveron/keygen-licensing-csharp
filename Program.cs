using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Linq;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using NSec.Cryptography;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Windows.System.Profile.SystemManufacturers;

public class Program
{
  private const string licenseFile = "-----BEGIN MACHINE FILE-----\neyJlbmMiOiJ0S3JYUmw4NCtDSTJ6WDVpN1NSUWJqdkt1d1BpRWFhZDdudlF2\nbWNVVnd2L3B2ajEvcDdLb0NzWVVvbWJzT0tQdkwyS0xUcTBhNUtKeGpLRmNP\nRG1oc3JUR2NSNUF4bktoSVRJWTA0THFJelJzNW9tZzRpbWRDSVpZZWdzbzdK\nRk9ISDVZaFBnaEFYK1FkK0U4aDUzZWlkNXM4KzVXL0xwN1h2ZUJVamhHeEFZ\nSjhTb3VwOFZpOXQwTmlDSHlaL1hIMDRNM0YwSm83MmI0b1VXSWlQT3NNcm9n\nVVhudllTckk1TU9abWNhZWlIN1NtNTlDWWozeUVhOE1BcW0xOTdCWktEU0k3\ncDdUV2l2VW5FeHprRUhFN2xaUHVleDdGbCtHaURQRHhGbjlkamRRQ3hyamp2\nOWRVUG0rdXkzS0Rzb0RwRjZWMWp4OU8wVWNpNFdZZzlUaWNoVm5KK0Yxd3g4\naFJmQ25lU3QvSmo3SHVXSDZ4MC9DS0ZHczFZU0NicUU5VmMxaHFDWTdYNCsw\nKzdYQmNhZWpRSituUU5TemVkdXNya0JTa0NEcm1PUHpzUW9JNzJFUWUxS20x\ndkY0MlRxbHBXUzB3aFVFSml2a1VlcmZqVFQ4aGpPZTlJZEFzTEphQytqS3F3\nS2hmQ0QxVERqZVQ4QnMwT0JLOXpZV0JUbmlDY2Q1bUlUSFVySWZDanhmOGNh\nMDdmaW1TdnV0UTdCN3FNNlMvWEJ6VWhqNXAvNlJ2OU9TQS9pcDFkT0RuWHhm\nK2FOempTM1planB1Q2M3UGkzM0ltejZabCtZUWl0cklnMUpaaXhmZ0l0aGpr\nVS9BakVqaW1PekdxcS9NTjlSdjIwTUk3RWRrWE40Z3VhNGFjMGJNQzNQQnhs\na3BZK0hMQW1HSWlxckJZSVAzVDRMNzBFMk5JRkl5bldtKzk2aHdKazBVR1RQ\ncUlVdCt1QmIzdWRueWhuS3h6bnR5eXR1eTBuNzVOaVpndlFONE14MUc5dWlF\na2xIOWVQN2Jjbk5PWGEvRW9Mdm9NKy9NZDNZc01KdlkzZ09Sd2w0R0FFZ1Bv\nZ3VMZTc4L3R3T1dsU3NSS2ZrWlM1QW1GdWd2cU13dmdwOVhzT3NwOFRPa1F0\nMEtCdHVZejFFSjh5TmthSzBReXMwMUNjWDUzelJHZDJYc0dKMEo0TTBnR2FX\nQVBJZndVTm5iSThnRTV6eEZ0T2lGdXZXMTRML1JHRXdJZUxVaXhhbkVpa28z\nTjMrMHdhdGhjTzlVcGJreXR1VlpFOGprR0t3OHRZdGdyaFRDalc0bllEQW5O\nb2Z2Tm9hUkt2ZHp2RzEvODg0SDZuVWhYbEw5ZVBvaE1pZXJVOE9iYWhIQTR4\nT0dnQjhGQXp6QVhndGtaK3ZFRUpBOUl5Y2x5QWdzWjR3elVHMEpZcDF6bEhL\nRmdjMjA4eldxeUcxdW9VR3czTEtMZjRjZklNRlBkS2ozOWpCNms1TVFSOXc4\nUnBmSkJvU2pnemt3eS9GSnhVcnk0Ni9Pdlk4VDNSMGFxb0VOZHVrRHVvOUF2\na1N3VmY2WkRadjR4Qk1FVmFJUFRxbUxIc1pyZmkwcTdRa3RjdklzNHpmSEZK\nSW15MnZHQndMb0ZzOGhteXNOOVQzbnB2MzFiM0ZaVmQ3VUR4SmxTR21qa281\nenNaNEVFeDFEUjhHRzMzYjg5Q1Yxc09GUytBWWxXem9iM0sza3NnREt5NzhY\nQk9ucEttY3ZKZnNvWm83ZSt2dTZtOHAzQzVzak53UVJycUJJb1Z1K0czbWIv\nU2dvTHRkWExZcGlEaE5pTzVFS3hDVSsvUCtNcEFPbERRU211cy85N0VadlRP\nR05RVDZGWkVjWEpocUl0Zm9DemFYZGQxenJYamhXbGpCNng3Q0F0Y3Fudkl5\nMlZ0UHplSlB6Z2hFSzZlZ1VyL2MybDBHU3ZVNjNmTnFxUzZoQzJpZUZaWTg0\nT2ptZ1hKb1BZU2YxODAwTlRlMUkyRzdrZi9LaTh0S2FqMVVONndDQ1Y0RDk0\nd3B1WElJdVZiaXl0U1ArYktudGtiQ1NUZ1ZpZEwyZHdZTnE0Y2o1bHdTM0xn\nR1BpVnpQZDNPMWs3V2tlemlpRVJ0K0xKNTFNbHZIVS9kVkI2RTZIREY2dFVt\nUHpHaDhaSTFUTGhOUU1GV3pSMWJUaFhzdFRuZ2FrR3ZKc0I4WFd1Y1R5RGg5\ncXdnYlhWYytqNmFQZ0FBb3M0enh3VVowdTJTV2lGWHpVTWxlUUtqWTM4VGk4\ndFp1ZTlac1ZBUEJVUFo2TTVSV0dEQVZ1OGttVno3VjFVTkhEQS9OTTA5dGZS\naFVkUjVJNlhlRXZ0ZWJFdmM1YktNNnFXOTBId3V0QWx2b0dXcmFpak9qbGti\nVmZZV2d3RlloYmg0eEdJL2NoYkR0OWtBcVFIMDhPUCsyZVhTbmxZWU1GSVdI\nQk8rK1g5WGd0RktmK2dJb3N5K2hKZjVIekhZL3hGWHFiVzJzWnE4U0lZaUhW\nRTVWMXA1cGVKbUNHaVZnTEV0YUZSU0o1bWxWeUxWdldLSmdIRG9SWUlPaFR4\nWFUvVnkxM3hXVmF3c0dTa2s0NDU5bHZoVEt1bUhIK3o4aTkyOG9EY2dsWnk0\nbENtbTFGTUhEQ3dHMUkzUjJNbzR3ZkQ1MXhVNHVCTW1iR0dBckowSlBJZHNV\nZUZUeEFpS2lLV25KRzBrbGF5cWNUR0huTGR0QUY0TWJ0R0N5YytsMldzZ0xh\ncFZlOE9ablJ1MWZUY1M3TGE0RjZudUdrMVpxeVhJeEhlU3RVUlFWOVZ0ZFI3\nZGwzcHFsaXh1eDZHTTRBNVhMbVp1NG9DeWtqU1c5SGp2MFhQdjFwdnFsemtq\nb2xoTjF6RzlHbmJVQ2c2cExmMGJ4VFcwdlJSV0kwWU9NZm1kNkZMeURRZ1ZE\nYkY3L2UwVS9jRXllRHZTWkM0KzZCcTFjdFhTZlZ4NDFqaHBMV3E2eG9WT3hl\nNHg3UXZreEEzM3JkdFVYeWNEa0RneFNDQ1lSZzFxckFWOUx5SjdRMlIwOVZs\ncnd5YitMWm1RQkVRMGRxdlVmNVB0UU1LQVRnWXlvclo2cEFTWmI4QUdMeis3\nQzNXTjJHbnNoR1dwZUlCV2RzSGJFTXZnR0ZTUGt1NlpCaDR3MVlTRm44NUwr\na3FGNnJTTUVscVRtdjcyVVJRdTRyNE85L0FSa3VOMHFYRmZNMldVNlpTUTkz\nTzR4STdpbW92SWMyWktaUHFKOG5nRUR4UEIyMFhpaUFFSmM2Y0tCUUNmYTA1\nSnlRYWt2Z0ZnQ0gvTDl3YkQxcUJMVDRRSTJ3d0M0TnlYcHlHNm9mMWpaVElN\nSFprVVBhMDlwYVNnVXZvVDRTaUtYZXBObG41dzdIWHdVY1JMQjBRUzVvUHZL\nOXBoYW0zcTBMbHlFMHVkUFhINEkvdEFWZThIcDJwQWRiUkl5c0E0dHJ1bUFv\nVS9EN3E5czZVc0JiMW02azBwQ0w1Z3ZLV3Z2M096YnBGOXRNbEFhYW12ZnFG\na1dIanNQK2pwZnVoeGhsL3llTEl3ZU5lZjJmTmxIOVNtWjExbzVsL3lpVTJ1\nQWVrd0dabGVQUmx2QzgrS3ovd3o4ejgxUllLVnVWSGNvcVc2dkY3OEJadkFa\nVDgvbVZyQzR5TElpcXp3NkhvYmpQWnhsNW5rRXhVSHdLalRsRXdPWExWNFJu\ndlY1ZitjZlk3Qnh1RGhCdlpGU2dEb0NiUmpOK2JIOTRxM3A3SDQzUlV4SG9N\nazY4M1pOMmZCM09nNW1CSUZxOFA0SU94c0dSWGltcDZvR1FzQTcxMGdxV1Z1\nYWdVbWJ5YWhjWWRsQW1majVlWC8zV2RCN3VOdm5idDViZmlTL05GS21LTWxy\nT2NoSUNLUU56bU1IZ2xPZW95SzA3ZlAwUVZLc2hsZEJhZzNicXF0MSsxU3hi\nakFwVTN3UHNjakVuSVBhUjBUemhrcFVuMm5rR25iWDhxSVVOcURsMlkyc3RI\nS3V5VmEybUpZaUVLVm1sdE9GakYyVm92ZFZPY3lqT3lTOWFlUTduY0tlZTdp\nc1E1RC9NdURlK091aWhLY3RzLzRobnRCMHV3a0x5SVZKdE5xSHgwQzlxVE9D\nNEtNbjdlcGlDT0ltdjBHRVdaQkZyS2kvOXJEbE1FOXBnWTZPTU5ySXZhK1Br\nUWVRQllmRXo1bnAzTzJNZm5zS1B2dHVjamF2OWg5TlQyUjkwcVBDSXkzcjl5\nYUFZV1hTR1cyTlZtNFdXcENnM05jdjFDTDhZTEMwR1I5OFI3LzgwdUxEQ1A0\na04wRG1PLzFZN2wxb2VXVlVVY0ZvTnFBSmo2NCtGS1BsL21CUDNYY0FDMWhE\ncmlENTlDYVFNWVhkOGdXVkJiRzVYeEpIdU5mNmdvZjRidEhFVUM3UHdaUjVp\ncDROai9OdUh4Vk4rVVhselVFUmh1UTR2a2daWkFpVE9IbE5Pc3ZlLzJmNTlt\nVkhMSnpDRTF3Wlc1c09TZlVuN2Zwb2hzUUMrRVROdHhRemdURkU3N1NFUDVi\ndFdSZytFWDNmU1Fibjh6LzBiektUYTc3VFpmdzYrL2g3SXN6UmNCTDVZOFN5\nUzZnbGxjbmtoVTVLZjVwc0dJc0JXaklDMjhwQnhMMkhqaWF4ZlowVkpKTVNF\nMWkzSFVQU29RbmdUaFdxckQvd3dzN1Fjb280VVdrbWF4VlF5Z3k4MWRaZlhr\namdMcmxvQVhodSthb0lxa0I2ZkxYbTUxT2JUK3dEVHQ3SHpwVkRTb0VDamEy\nVUR3MDRPNXNOcitDR3p6aGc0LzdYZTQvZWhQbW85MmhyeVFPRUFaK3pveVRp\nTGx1bWhtbE1zMUhTeHRxSGw2TU93VnZNV3lEVW9BWXNCc3MzTWpGazk4b0hM\nRTFIajVCMUNxYTBKbk9kQXF2UE0wOUZ0UjZxdlVRSFVLbE9mRnhuY2xlSlpm\nWnJreVBlV0tCeld6RHhYL1hGR1BpT3JldUVLeE9hZWRyaUQ5STNHalg4clpk\nY08xUUVmWlFCN1NQdkc2Q25XZUVzWDFWV2RNclVTQlJLaldUQkVlRlVjdmdH\nQzhRamVFS1RlcXpuTFpkZFh5MmdpVnl5ejdMSW1kOWVNUkJpOHNmbzZYSTgr\nTWsyOXJUZksvNGZJd0tub0ZtNVdCS1hSWENvalJMVTV1MGw1MUwyVmtkZHNR\nVUlzZjc1UlFIZkxKUVZEODY3elJJM2JaZTJOUkRMOEtQQU95bUVMd3hKMmRq\nOURJa1N2WXJSR3kxOGE4VXdzRVNrS1RtZFRyYlhRQ0ZCYUMxeXlhTng1dUJo\ndU85eTJSMlEzRWE0NGJlQmFXaHRZVm9uQ0ZQM01WSTQ5bXI2MzNyM3F3QlAz\nRW1yMHBrck9aczA0NUNDa3diTktLdmlYREtBbm1FcUduRmxBMmNtQnl2TURP\nNktadEpyR2wvUWFqZ05wck1PQnNycE5wWFZjdVNhRVIrT0JFQXR1WnNXYUZT\nT1RUaFFVVWRBbFN3NWN4N3BnL2xTRlV5aWdudFBBWVVjVllzM3N1NEZpU0Zl\nL0UwT3Qzek5CL2wzUHIyb1g1bHFYNG9mVEZEZ2NySm5FVDBmU1ZhaW5zR3Zy\nTXNtTXV5UHhMcjJiSTJWWEIxMXNWNDhKQmR5aXhDNGxWSTZEUCtWNWs2MW01\nQzd4bEFPYUFtSXVLWHBkdjROV3dkRXdrZmk2d3ZnSjluMGh4K09ORk1GT20y\nS0ZoR1NFeEpxQ1JNOXREa255elNtZWN4YnRqRllkdGp1bm4vL2kvTFNZdVhh\nbzFWQUFXcUVPNEloVXcwQUxJM0Z2ZUt3TDhUdWxIL05OWEpkVXJUOGh3Rllu\nak1RMC9yYWhMTmpqNkEzeVB5YU1aWnFpYjJLbVlQcnRBOWxSc1pDN1h6SEJx\ncVJrTUMxVjlFRnJEcjBsYjNaeUNWUzlVYzNWZmUvWlhwSjlmZXBhQUlpb096\nOEt3N1JaUlZERVNmS3VFN2dYY2RsYkF6V2x2UjhWR3k5OWswMUo1Vms0Q2wy\nNS9mMmR1UERHUmgvMzR2V2hNbGhpc3A3bjE2VjM0a3NrcFgra2JkTjVwcC94\nUGc1M2VWcHVIVWNheHlhQ3pLWHdTZUlFY2xQQTM2SFJ5SGwrU2o5aUEvS0Fn\nNWJ2enVIQU9RVm1nNkVxVml3aE1Ub1pJRUlaZE85K1pGZ2pnd0U0LzVlOTMv\nbXN6RC8vazdSU2RkdkZKaTJTdjMvV2xwMHhkTUFIbkQwM0dWNTdNaFEzelRw\nV1ZnRGlscmo0RzA0R2dCVjBGUkd6dUxqMDNQNy9iZk84Z08zOUFScEh1Tlo1\ncjJmT2dVcTllOWZYTGNGS0tDdFltbk5iLy85NmljQzJ5QUhqUGR6NHlac1gz\nT085TEFYek5wOVJWK3RrU0p0S1hZRjh6Wk1rWTg5WmZEbEh5QkFleDByVFlZ\nZlVVUS84OXFCR1Uwd2g3WVhGdWtPMUVqMkhCN0ZIb1lxSlNyMG5SeVFLMlJw\nSDBTeEhHR1NwK2lnaWpMbHp5VkJTVFZwL2EvdUpuZHRvOHhhOGJXZ3BiTkhn\nZE9XaHNWMDRLQ0hFNWlXTS9BQ1RwVW9EOVU5QzdPZzRBPS5sTWhWTEpOV1VF\nS3lHcGt4LmMyUmhDbEJZWGY0SVRmcEV0WHRUdmc9PSIsInNpZyI6ImlHMGh2\nY1VaeGUwU3BDZWlNWi95aldFcEFnV2t6Ly9vV2FNRmFkUjRmWGd1UkozTGZO\nK3FIOCt4eFhhRmRoSS8rbVFvdTdnZzRSRzdxSVUwdjRoUERRPT0iLCJhbGci\nOiJhZXMtMjU2LWdjbStlZDI1NTE5In0=\n-----END MACHINE FILE-----\n";
  private const string licenseKey = "B10760-1B177D-656D1F-C03298-9AF89E-V3";
  private const string publicKey = "e8601e48b69383ba520245fd07971e983d06d22c4257cfd82304601479cee788";
  private const string fingerprint = "198e9fe586114844f6a4eaca5069b41a7ed43fb5a2df84892b69826d64573e39";

  String Get_Serial_Number() 
  {
    try
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
        return Windows.System.Profile.SystemManufacturers
      }
      elif system == 'Linux':
        with open('/sys/class/dmi/id/product_serial') as f:
          return f.read().strip()
      elif system == 'Darwin':
        return platform.system_profiler().get('Hardware').get('Serial Number')
      else:
        return None
    }
    catch(Exception e) {
      Console.WriteLine($"Impossible de récupérer le numéro de série : {e.Message}");
      Environment.Exit(1);
    }
  }
  
serial_number = get_serial_number()
if (serial_number == ""):
  Console.WriteLine($"Impossible de récupérer le numéro de série : {e.Message}");
  sys.exit(1)
else:
  print("Serial number : ", serial_number)

  public class LicenseFile
  {
    public string enc { get; set; }
    public string sig { get; set; }
    public string alg { get; set; }
  }

  public static void Main()
  {
    // Parse signed license file (removing cert header, newlines and footer)
    var encodedPayload = Regex.Replace(licenseFile, "(^-----BEGIN MACHINE FILE-----\\n|\\n|-----END MACHINE FILE-----\\n$)", "");
    var payloadBytes = Convert.FromBase64String(encodedPayload);
    var payload = Encoding.UTF8.GetString(payloadBytes);
    var encryptedData = "";
    var encodedSignature = "";
    var algorithm = "";

    // Deserialize license file certificate
    try
    {
      var lic = JsonSerializer.Deserialize<LicenseFile>(payload);

      encryptedData = lic.enc;
      encodedSignature = lic.sig;
      algorithm = lic.alg;
    }
    catch (JsonException e)
    {
      Console.WriteLine($"Failed to parse machine file: {e.Message}");

      return;
    }

    // Verify license file algorithm
    if (algorithm != "aes-256-gcm+ed25519")
    {
      Console.WriteLine("Unsupported algorithm!");

      return;
    }

    // Verify signature
    var ed25519 = SignatureAlgorithm.Ed25519;
    var signatureBytes = Convert.FromBase64String(encodedSignature);
    var signingDataBytes = Encoding.UTF8.GetBytes($"machine/{encryptedData}");
    var publicKeyBytes = Convert.FromHexString(publicKey);
    var key = PublicKey.Import(ed25519, publicKeyBytes, KeyBlobFormat.RawPublicKey);

    if (ed25519.Verify(key, signingDataBytes, signatureBytes))
    {
      Console.WriteLine("Machine file is valid! Decrypting...");

      // Decrypt license file dataset
      var plaintext = "";
      try
      {
        var encodedCipherText = encryptedData.Split(".", 3)[0];
        var encodedIv = encryptedData.Split(".", 3)[1];
        var encodedTag = encryptedData.Split(".", 3)[2];
        var cipherText = Convert.FromBase64String(encodedCipherText);
        var iv = Convert.FromBase64String(encodedIv);
        var tag = Convert.FromBase64String(encodedTag);
        var secret = new byte[32];

        // Hash license key to get decryption secret 
        try
        {
          var licenseKeyBytes = Encoding.UTF8.GetBytes(licenseKey);
          var fingerprintBytes = Encoding.UTF8.GetBytes(fingerprint);
          var sha256 = new Sha256();

          secret = sha256.Hash(licenseKeyBytes.Concat(fingerprintBytes).ToArray());
        }
        catch (Exception e)
        {
          Console.WriteLine($"Failed to hash license key: {e.Message}");

          return;
        }

        // Init AES-GCM
        var cipherParams = new AeadParameters(new KeyParameter(secret), 128, iv);
        var aesEngine = new AesEngine();
        var cipher = new GcmBlockCipher(aesEngine);

        cipher.Init(false, cipherParams);

        // Concat auth tag to ciphertext
        var input = cipherText.Concat(tag).ToArray();
        var output = new byte[cipher.GetOutputSize(input.Length)];

        // Decrypt
        var len = cipher.ProcessBytes(input, 0, input.Length, output, 0);
        cipher.DoFinal(output, len);

        // Convert decrypted bytes to string
        plaintext = Encoding.UTF8.GetString(output);
      }
      catch (Exception e)
      {
        Console.WriteLine($"Failed to decrypt machine file: {e.Message}");

        return;
      }

      Console.WriteLine("Machine file was successfully decrypted!");
      //Console.WriteLine($"Decrypted: {plaintext}");
    }
    else
    {
      Console.WriteLine("Invalid machine file!");
    }
  }
}
