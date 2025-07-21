using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;

namespace Carbon.Documentation;

public static partial class WebRCon
{
	private const string Ok = "ok";

	private static ItemContainer FindContainer(int id, BasePlayer player)
	{
		switch (id)
		{
			case 0:
				return player.inventory.containerMain;
			case 1:
				return player.inventory.containerBelt;
			case 2:
				return player.inventory.containerWear;
		}

		return null;
	}

	private static object ParseEntity(BaseEntity entity)
	{
		return new { NetId = entity.net.ID.Value, Name = entity.name, Flags = entity.flags };
	}

	private static object ParseItem(Item item)
	{
		return new
		{
			ItemId = item.info?.itemid,
			ShortName = item.info?.shortname,
			Position = item.position,
			Amount = item.amount,
			MaxCondition = item.maxCondition,
			Condition = item.condition,
			ConditionNormalized = item.conditionNormalized,
			HasCondition = item.hasCondition
		};
	}

	private static string CompressStringToBase64(string data)
	{
		if (string.IsNullOrEmpty(data))
			return string.Empty;

		byte[] bytes = Encoding.UTF8.GetBytes(data);
		using var output = new MemoryStream();
		using (var gzip = new GZipStream(output, CompressionMode.Compress))
		{
			gzip.Write(bytes, 0, bytes.Length);
		}

		return Convert.ToBase64String(output.ToArray());
	}

	/* javascript

	 async function compressStringToBase64(str) {
	       if (!str) return '';

	       const stream = new Blob([new TextEncoder().encode(str)]).stream();
	       const compressedStream = stream.pipeThrough(new CompressionStream('gzip'));
	       const compressedBuffer = await new Response(compressedStream).arrayBuffer();

	       // Convert to Base64 without binary string conversion
	       const bytes = new Uint8Array(compressedBuffer);
	       return btoa(String.fromCharCode(...bytes));
	   }
	 */

	private static string DecompressBase64ToString(string base64)
	{
		if (string.IsNullOrEmpty(base64))
			return string.Empty;

		byte[] compressed = Convert.FromBase64String(base64);
		using var input = new MemoryStream(compressed);
		using var gzip = new GZipStream(input, CompressionMode.Decompress);
		using var output = new MemoryStream();
		gzip.CopyTo(output);
		return Encoding.UTF8.GetString(output.ToArray());
	}


	/* javascript

	async function decompressBase64ToString(base64) {
	    if (!base64) return '';

	    const binary = atob(base64);
	    const bytes = Uint8Array.from(binary, m => m.codePointAt(0));

	    const stream = new Blob([bytes]).stream();
	    const decompressedStream = stream.pipeThrough(new DecompressionStream('gzip'));
	    const decompressedBuffer = await new Response(decompressedStream).arrayBuffer();

	    return new TextDecoder().decode(decompressedBuffer);
	}
	 */

	private struct RConError(RConErrorEnum code, string error)
	{
		[JsonProperty] public RConErrorEnum Code = code;
		[JsonProperty] public string Error = error;
	}

	private enum RConErrorEnum
	{
		InvalidArgs = 1,
		NoSuchFile,
	}
}
