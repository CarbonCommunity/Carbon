using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Shims;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Carbon.Components;

public class Vault
{
	public static readonly string Global = "global";

	private const int DEFAULT_KEY_BIT_SIZE = 256;
	private const int DEFAULT_MAC_BIT_SIZE = 128;
	private const int DEFAULT_NONCE_BIT_SIZE = 128;
	private const int SALT_SIZE = 16;
	private static readonly SecureRandom RANDOM = new();
	private static readonly List<Factory> FACTORIES = new();
	private static string CARBON_ID_CACHE;
	private static string CARBON_ID => CARBON_ID_CACHE ??= JObject.Parse(OsEx.File.ReadText(Path.Combine(Defines.GetRustIdentityFolder(), "carbon.id")))["UID"].ToObject<string>();

	private static byte[] EncryptData(byte[] buffer, string password, out byte[] salt)
	{
		using Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt = RANDOM.GenerateSeed(SALT_SIZE));
		return EncryptImpl(buffer, pdb.GetBytes(32));
	}
	private static byte[] DecryptData(byte[] buffer, string password, byte[] salt)
	{
		using Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt);
		return DecryptImpl(buffer, pdb.GetBytes(32));
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
	    for (int i = 0; i < FACTORIES.Count; i++)
	    {
		    var item = FACTORIES[i];
		    Facepunch.Pool.Free(ref item);
	    }
	    FACTORIES.Clear();
    }

    private static Factory CreateFactory(uint id)
    {
	    if (id == 0)
	    {
		    return null;
	    }
	    var factory = GetFactory(id) ?? Facepunch.Pool.Get<Factory>();
	    factory.id = id;
	    if (!FACTORIES.Contains(factory))
	    {
		    FACTORIES.Add(factory);
	    }
	    return factory;
    }

    public static Factory GetFactory(uint id)
    {
	    for (int i = 0; i < FACTORIES.Count; i++)
	    {
		    var factory = FACTORIES[i];
		    if (factory.id.Equals(id))
		    {
			    return factory;
		    }
	    }
	    return null;
    }
    public static List<Factory> GetFactories() => FACTORIES;

    public static bool Add(string factory, string name, string value, bool encrypted = true)
    {
	    if (string.IsNullOrEmpty(factory) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))
	    {
		    Logger.Warn("Attempted to add a null factory, name or value into the Carbon.Vault");
		    return false;
	    }

	    var factoryInstance = CreateFactory(Pool.Get(factory));
	    var identifier = Pool.Get(name);

	    var salt = (byte[])default;
	    var buffer = Encoding.UTF8.GetBytes(value);
	    var hasItem = factoryInstance.HasItem(identifier, out var item);
	    item ??= Facepunch.Pool.Get<Item>();
	    item.id = identifier;
	    item.encrypted = encrypted;
	    item.hash = encrypted ? EncryptData(buffer, CARBON_ID, out salt) : buffer;
	    item.salt = salt;
	    item.cache = value;
	    item.runtimeId = "{" + factory + ":" + name + "}";
	    if (!hasItem)
	    {
		    factoryInstance.AddItem(item);
		    Save(true);
	    }
	    return true;
    }
    public static bool Remove(string factory, string name)
    {
	    if (string.IsNullOrEmpty(factory) || string.IsNullOrEmpty(name))
	    {
		    Logger.Warn("Attempted to remove a non-existent factory or factory item from the Carbon.Vault");
		    return false;
	    }

	    if (CreateFactory(Pool.Get(factory)).RemoveItem(Pool.Get(name)))
	    {
		    Save(true);
		    return true;
	    }

	    return false;
    }
    public static string Get(string factory, string name)
    {
	    if (string.IsNullOrEmpty(factory) || string.IsNullOrEmpty(name))
	    {
		    Logger.Warn($"Provided a null factory or name for retrieving a value from the Carbon.Vault");
		    return null;
	    }

	    var factoryInstance = CreateFactory(Pool.Get(factory));
	    var identifier = Pool.Get(name);
	    if (!factoryInstance.HasItem(identifier, out var item))
	    {
		    Logger.Warn($"Identifier with '{name}' for factory '{factory}' does not exist in the Carbon.Vault");
		    return null;
	    }
	    return item.GetCache();
    }

    public static void Save(bool silent = false)
    {
	    var factories = 0;
	    var items = 0;

	    using var memoryStream = new MemoryStream();
	    using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
	    {
		    using var writer = new BinaryWriter(gzipStream);

		    Pool.Save(writer);

		    writer.Write(FACTORIES.Count);
		    foreach (var factory in FACTORIES)
		    {
			    factories++;
			    writer.Write(factory.id);
			    writer.Write(factory.Count);
			    foreach (var item in factory)
			    {
				    items++;
				    writer.Write(item.id);
				    writer.Write(item.hash.Length);
				    writer.Write(item.hash);
				    writer.Write(item.salt != null);
				    if (item.salt != null)
				    {
					    writer.Write(item.salt.Length);
					    writer.Write(item.salt);
				    }
				    writer.Write(item.encrypted);
			    }
		    }
	    }
	    OsEx.File.Create(Defines.GetVaultFile(), memoryStream.ToArray());
	    if (!silent) Logger.Log($"Saved Carbon.Vault with {factories:n0} {factories.Plural("factory", "factories")} and {items:n0} {items.Plural("item", "items")}");
    }
    public static void Load(bool silent = false)
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
		    Pool.Load(reader);

		    var items = 0;
		    var factoryCount = reader.ReadInt32();
		    for (int f = 0; f < factoryCount; f++)
		    {
			    var factory = Facepunch.Pool.Get<Factory>();
			    factory.id = reader.ReadUInt32();
			    var itemCount = reader.ReadInt32();
			    for (int i = 0; i < itemCount; i++)
			    {
				    items++;
				    var item = Facepunch.Pool.Get<Item>();
				    item.id = reader.ReadUInt32();
				    item.hash = reader.ReadBytes(reader.ReadInt32());
				    if (reader.ReadBoolean())
				    {
					    item.salt = reader.ReadBytes(reader.ReadInt32());
				    }
				    item.encrypted = reader.ReadBoolean();
				    item.runtimeId = "{" + Pool.Get(factory.id) + ":" + Pool.Get(item.id) + "}";
				    factory.AddItem(item);
			    }
			    FACTORIES.Add(factory);
		    }

		    if (!silent) Logger.Log($"Loaded Carbon.Vault with {factoryCount:n0} {factoryCount.Plural("factory", "factories")} and {items:n0} {items.Plural("item", "items")}");
	    }
	    catch (Exception ex)
	    {
		    Logger.Error($"Failed loading Carbon.Vault", ex);
	    }
    }

    public static string ReverseReplacement(string source, bool encrypted = true)
    {
	    if (string.IsNullOrEmpty(source))
	    {
		    return null;
	    }
	    for (int f = 0; f < FACTORIES.Count; f++)
	    {
		    var factory = FACTORIES[f];
		    for (int i = 0; i < factory.Count; i++)
		    {
			    var item = factory[i];
			    if (!encrypted && item.encrypted)
			    {
				    continue;
			    }
			    if (item.GetCache().Equals(source))
			    {
				    return item.runtimeId;
			    }
		    }
	    }
	    return null;
    }
    public static string ApplyReplacement(string source, bool encrypted = true)
    {
	    if (string.IsNullOrEmpty(source))
	    {
		    return null;
	    }
	    for (int f = 0; f < FACTORIES.Count; f++)
	    {
		    var factory = FACTORIES[f];
		    for (int i = 0; i < factory.Count; i++)
		    {
			    var item = factory[i];
			    if (!encrypted && item.encrypted)
			    {
				    continue;
			    }
			    if (source.Equals(item.runtimeId))
			    {
				    return item.GetCache();
			    }
		    }
	    }
	    return null;
    }

    public class Factory : List<Item>, Facepunch.Pool.IPooled
    {
	    public uint id;
	    public void EnterPool()
	    {
		    id = 0;
		    foreach (var item in this)
		    {
			    var i = item;
			    Facepunch.Pool.Free(ref i);
		    }
		    Clear();
	    }
	    public void LeavePool()
	    {
	    }

	    public bool HasItem(uint item)
	    {
		    for (int i = 0; i < Count; i++)
		    {
			    var value = this[i];
			    if(value.id == item) return true;
		    }
		    return false;
	    }
	    public bool HasItem(uint item, out Item value)
	    {
		    for (int i = 0; i < Count; i++)
		    {
			    value = this[i];
			    if(value.id == item) return true;
		    }
		    value = null;
		    return false;
	    }
	    public Item GetItem(uint item)
	    {
		    for (int i = 0; i < Count; i++)
		    {
			    var value = this[i];
			    if(value.id == item) return value;
		    }
		    return null;
	    }
	    public void AddItem(Item item)
	    {
		    if (GetItem(item.id) != null) return;
		    Add(item);
	    }
	    public bool RemoveItem(uint item)
	    {
		    return HasItem(item, out var value) && Remove(value);
	    }
    }

    public class Item : Facepunch.Pool.IPooled
    {
	    public uint id;
	    public bool encrypted;
	    internal string runtimeId;
	    internal string cache;
	    internal byte[] salt;
	    internal byte[] hash;

	    public string GetCache()
	    {
		    if (string.IsNullOrEmpty(cache)) cache = Encoding.UTF8.GetString(encrypted ? DecryptData(hash, CARBON_ID, salt) : hash);
		    return cache;
	    }

	    public void EnterPool()
	    {
		    id = 0;
		    hash = null;
		    salt = null;
		    encrypted = false;
		    cache = null;
	    }
	    public void LeavePool()
	    {
	    }
    }

    public static class Pool
    {
	    private static Dictionary<string, uint> NamePoolString = new();
	    private static Dictionary<uint, string> NamePoolInt = new();

	    public static void Save(BinaryWriter writer)
	    {
		    writer.Write(NamePoolString.Count);
		    foreach (var value in NamePoolString)
		    {
			    writer.Write(value.Key);
			    writer.Write(value.Value);
		    }
	    }
	    public static void Load(BinaryReader reader)
	    {
		    var count = reader.ReadInt32();
		    for (int i = 0; i < count; i++)
		    {
			    var key = reader.ReadString();
			    var value = reader.ReadUInt32();
			    NamePoolString[key] = value;
			    NamePoolInt[value] = key;
		    }
	    }
	    public static uint Get(string name)
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

	    public static string Get(uint hash)
	    {
		    return NamePoolInt.TryGetValue(hash, out var name) ? name : null;
	    }
	    private static uint ManifestHash(string str)
	    {
		    return string.IsNullOrEmpty(str) ? 0 : BitConverter.ToUInt32(new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(str)), 0);
	    }
    }

    [Preserve]
    public class Protected : JsonConverter
    {
	    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	    {
		    if (value is not string strVal)
		    {
			    writer.WriteNull();
			    return;
		    }

		    writer.WriteValue(ReverseReplacement(strVal) ?? strVal);
	    }

	    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	    {
		    var value = reader.Value?.ToString();

		    if (string.IsNullOrEmpty(value))
		    {
			    return null;
		    }

		    return ApplyReplacement(value);
	    }

	    public override bool CanConvert(Type objectType)
	    {
		    return objectType == typeof(string);
	    }
    }
}
