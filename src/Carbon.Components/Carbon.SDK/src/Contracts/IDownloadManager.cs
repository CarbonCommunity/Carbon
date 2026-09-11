using System;
using System.Threading;
using System.Threading.Tasks;

namespace API.Contracts;

public interface IDownloadManager
{
	/// <summary>
	///     Downloads the contents of the provided URL as an array of bytes.
	/// </summary>
	Task<byte[]> Download(string url);

	/// <summary>
	///     Downloads the contents of the provided URL as an array of bytes and
	///     allows cancellation of the request.
	/// </summary>
	Task<byte[]> Download(string url, CancellationToken token);

	/// <summary>
	///     Downloads the contents of the provided URL as an array of bytes and
	///     allows cancellation of the request with optional error suppression.
	/// </summary>
	Task<byte[]> Download(string url, CancellationToken token, bool suppressErrors);

	/// <summary>
	///     Adds a download request for the provided URL to the download queue and
	///     triggers the specified callback when the task completes.
	/// </summary>
	void DownloadAsync(string url, Action<string, byte[]> callback);
}
