using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Carbon.Components;

public class Vault
{
	private const int DEFAULT_KEY_BIT_SIZE = 256;
	private const int DEFAULT_MAC_BIT_SIZE = 128;
	private const int DEFAULT_NONCE_BIT_SIZE = 128;
	private static readonly SecureRandom RANDOM = new();
	private static readonly Dictionary<uint, Item> ITEMS = new();
	private static string CARBON_ID_CACHE;
	private static string CARBON_ID => CARBON_ID_CACHE ??= JObject.Parse(OsEx.File.ReadText(Path.Combine(Defines.GetRustIdentityFolder(), "carbon.id")))["UID"].ToObject<string>();
	private static readonly byte[] SALT = [0x26, 0xdc, 0xff, 0xfe, 0x00, 0xad, 0x7a, 0xee, 0x3c, 0xaf, 0xc5, 0x07, 0xed, 0x4d, 0x08, 0x22];

	private static byte[] DecryptData(byte[] buffer, string password)
	{
		using Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, SALT);
		return DecryptImpl(buffer, pdb.GetBytes(32));
	}
	private static byte[] EncryptData(byte[] buffer, string password)
	{
		using Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, SALT);
		return EncryptImpl(buffer, pdb.GetBytes(32));
	}

    private static byte[] DecryptImpl(byte[] encryptedMessage, byte[] key, int nonSecretPayloadLength = 0)
    {
	    ValidateKeyImpl(key);

	    if (encryptedMessage == null || encryptedMessage.Length == 0)
		    throw new ArgumentException("Encrypted Message Required!", nameof(encryptedMessage));

	    using var cipherStream = new MemoryStream(encryptedMessage);
	    using var cipherReader = new BinaryReader(cipherStream);

	    var nonSecretPayload = cipherReader.ReadBytes(nonSecretPayloadLength);
	    var nonce = cipherReader.ReadBytes(DEFAULT_NONCE_BIT_SIZE / 8);
	    var cipher = new GcmBlockCipher(new AesEngine());
	    var parameters = new AeadParameters(new KeyParameter(key), DEFAULT_MAC_BIT_SIZE, nonce, nonSecretPayload);
	    cipher.Init(false, parameters);

	    var cipherText = cipherReader.ReadBytes(encryptedMessage.Length - nonSecretPayloadLength - nonce.Length);
	    var plainText = new byte[cipher.GetOutputSize(cipherText.Length)];
	    var len = cipher.ProcessBytes(cipherText, 0, cipherText.Length, plainText, 0);
	    cipher.DoFinal(plainText, len);
	    return plainText;
    }
    private static byte[] EncryptImpl(byte[] messageToEncrypt, byte[] key, byte[] nonSecretPayload = null)
    {
	    ValidateKeyImpl(key);

	    nonSecretPayload ??= [];

	    var nonce = new byte[DEFAULT_NONCE_BIT_SIZE / 8];
	    RANDOM.NextBytes(nonce, 0, nonce.Length);

	    var cipher = new GcmBlockCipher(new AesEngine());
	    var parameters = new AeadParameters(new KeyParameter(key), DEFAULT_MAC_BIT_SIZE, nonce, nonSecretPayload);
	    cipher.Init(true, parameters);

	    var cipherText = new byte[cipher.GetOutputSize(messageToEncrypt.Length)];
	    var len = cipher.ProcessBytes(messageToEncrypt, 0, messageToEncrypt.Length, cipherText, 0);
	    cipher.DoFinal(cipherText, len);

	    using var combinedStream = new MemoryStream();
	    using var binaryWriter = new BinaryWriter(combinedStream);

	    binaryWriter.Write(nonSecretPayload);
	    binaryWriter.Write(nonce);
	    binaryWriter.Write(cipherText);
	    return combinedStream.ToArray();
    }
    private static void ValidateKeyImpl(byte[] key)
    {
        if (key is not { Length: DEFAULT_KEY_BIT_SIZE / 8 })
        {
            throw new ArgumentException($"Key needs to be {DEFAULT_KEY_BIT_SIZE} bit! actual:{key?.Length * 8}", nameof(key));
        }
    }
    private static void ClearToPool()
    {
	    foreach (var item in ITEMS)
	    {
		    var element = item.Value;
		    Facepunch. Pool.Free(ref element);
	    }
	    ITEMS.Clear();
    }

    public static void Add(string factory, string name, string value, bool encrypted = true)
    {
	    if (string.IsNullOrEmpty(factory) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))
	    {
		    Logger.Warn("Attempted to add a null factory, name or value into the Carbon.Vault");
		    return;
	    }

	    var identifier = Pool.GetOrAdd(string.Concat(factory, name));
	    if (!ITEMS.TryGetValue(identifier, out var item))
	    {
		    var buffer = Encoding.UTF8.GetBytes(value);
		    ITEMS[identifier] = item = Facepunch.Pool.Get<Item>();
		    item.identifier = identifier;
		    item.encrypted = encrypted;
		    item.hash = encrypted ? EncryptData(buffer, CARBON_ID) : buffer;
		    item.cache = value;
	    }
    }
    public static string Get(string factory, string name)
    {
	    if (string.IsNullOrEmpty(factory) || string.IsNullOrEmpty(name))
	    {
		    Logger.Warn($"Provided a null factory or name for retrieving a value from the Carbon.Vault");
		    return null;
	    }

	    var identifier = Pool.GetOrAdd(string.Concat(factory, name));
	    if (!ITEMS.TryGetValue(identifier, out var item))
	    {
		    Logger.Warn($"Identifier with '{name}' for factory '{factory}' does not exist in the Carbon.Vault");
		    return null;
	    }
	    if (string.IsNullOrEmpty(item.cache))
	    {
		    item.cache = Encoding.UTF8.GetString(item.encrypted ? DecryptData(item.hash, CARBON_ID) : item.hash);
	    }
	    return item.cache;
    }

    public static void Save()
    {
	    using var memoryStream = new MemoryStream();
	    using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
	    {
		    using var writer = new BinaryWriter(gzipStream);

		    writer.Write(ITEMS.Count);
		    foreach (var item in ITEMS)
		    {
			    writer.Write(item.Value.identifier);
			    writer.Write(item.Value.hash.Length);
			    writer.Write(item.Value.hash);
			    writer.Write(item.Value.encrypted);
		    }
	    }
	    OsEx.File.Create(Defines.GetVaultFile(), memoryStream.ToArray());
    }
    public static void Load()
    {
	    if (!OsEx.File.Exists(Defines.GetVaultFile()))
	    {
		    Logger.Warn($"Carbon.Vault does not exist yet");
		    return;
	    }

	    ClearToPool();
	    using var memoryStream = new MemoryStream(OsEx.File.ReadBytes(Defines.GetVaultFile()));
	    using var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
	    using var reader = new BinaryReader(gzipStream);

	    try
	    {
		    var count = reader.ReadInt32();
		    for (int i = 0; i < count; i++)
		    {
			    var item = Facepunch.Pool.Get<Item>();
			    item.identifier = reader.ReadUInt32();
			    item.hash = reader.ReadBytes(reader.ReadInt32());
			    item.encrypted = reader.ReadBoolean();
			    ITEMS.Add(item.identifier, item);
		    }
	    }
	    catch (Exception ex)
	    {
		    Logger.Error($"Failed loading Carbon.Vault", ex);
	    }
    }

    private class Item : Facepunch.Pool.IPooled
    {
	    public uint identifier;
	    public byte[] hash;
	    public bool encrypted;
	    public string cache;

	    public void EnterPool()
	    {
		    identifier = 0;
		    hash = null;
		    encrypted = false;
		    cache = null;
	    }
	    public void LeavePool()
	    {
	    }
    }

    private static class Pool
    {
	    private static Dictionary<string, uint> NamePoolString = new();
	    private static Dictionary<uint, string> NamePoolInt = new();

	    public static uint GetOrAdd(string name)
	    {
		    if (NamePoolString.TryGetValue(name, out var hash))
		    {
			    return hash;
		    }

		    hash = ManifestHash(name);
		    NamePoolString[name] = hash;
		    NamePoolInt[hash] = name;
		    return hash;
	    }
	    private static uint ManifestHash(string str)
	    {
		    return string.IsNullOrEmpty(str) ? 0 : BitConverter.ToUInt32(new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(str)), 0);
	    }
    }
}
