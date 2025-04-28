using System.Security.Cryptography;
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
	private static readonly byte[] SALT = [0x26, 0xdc, 0xff, 0xfe, 0x00, 0xad, 0x7a, 0xee, 0x3c, 0xaf, 0xc5, 0x07, 0xed, 0x4d, 0x08, 0x22];
	private static readonly SecureRandom RANDOM = new();

	public static byte[] DecryptData(byte[] buffer, string password)
	{
		using Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, SALT);
		return DecryptImpl(buffer, pdb.GetBytes(32));
	}
	public static byte[] EncryptData(byte[] buffer, string password)
	{
		using Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, SALT);
		return EncryptImpl(buffer, pdb.GetBytes(32));
	}

    private static byte[] DecryptImpl(byte[] encryptedMessage, byte[] key, int nonSecretPayloadLength = 0)
    {
        ValidateKeyImpl(key);

        if (encryptedMessage == null || encryptedMessage.Length == 0) throw new ArgumentException("Encrypted Message Required!", "encryptedMessage");

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
}
