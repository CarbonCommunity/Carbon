#if UNIX

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

public sealed class TarGzReader
{
    private readonly string _archivePath;
    private readonly List<TarEntry> _entries = new();

    public IReadOnlyList<TarEntry> Entries => _entries;

    public TarGzReader(string archivePath)
    {
        _archivePath = archivePath ?? throw new ArgumentNullException(nameof(archivePath));
        Load();
    }

    private void Load()
    {
        using var fileStream = new FileStream(_archivePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress, leaveOpen: false);

        var header = new byte[512];

        while (true)
        {
            int read = ReadExact(gzipStream, header, 0, 512);
            if (read == 0)
            {
                break;
            }

            if (header.All(b => b == 0))
            {
                break;
            }

            string name = ReadString(header, 0, 100).Trim('\0');
            if (string.IsNullOrEmpty(name))
            {
                break;
            }

            string sizeOctal = ReadString(header, 124, 12).Trim('\0', ' ');
            long size = 0;
            if (!string.IsNullOrWhiteSpace(sizeOctal))
            {
                size = Convert.ToInt64(sizeOctal, 8);
            }

            byte[] data = new byte[size];
            if (size > 0)
            {
                ReadExact(gzipStream, data, 0, (int)size);
            }

            long padding = (512 - (size % 512)) % 512;
            if (padding > 0)
            {
                Skip(gzipStream, padding);
            }
            bool isDirectory = name.EndsWith("/", StringComparison.Ordinal) || size == 0;
            if (!isDirectory)
            {
                _entries.Add(new TarEntry(name, size, data));
            }
        }
    }

    private static string ReadString(byte[] buffer, int offset, int length)
    {
        return Encoding.ASCII.GetString(buffer, offset, length);
    }

    private static void Skip(Stream stream, long bytes)
    {
        var buffer = new byte[8192];
        long remaining = bytes;

        while (remaining > 0)
        {
            int toRead = (int)Math.Min(remaining, buffer.Length);
            int read = stream.Read(buffer, 0, toRead);
            if (read <= 0)
            {
                break;
            }
            remaining -= read;
        }
    }

    private static int ReadExact(Stream stream, byte[] buffer, int offset, int count)
    {
        int totalRead = 0;
        while (totalRead < count)
        {
            int read = stream.Read(buffer, offset + totalRead, count - totalRead);
            if (read == 0)
            {
                break;
            }
            totalRead += read;
        }

        return totalRead;
    }

    public sealed class TarEntry
    {
        private readonly byte[] _data;

        internal TarEntry(string name, long size, byte[] data)
        {
            Name = name;
            Size = size;
            _data = data;
        }

        public string Name { get; }

        public long Size { get; }

        public Stream Open()
        {
            return new MemoryStream(_data, 0, _data.Length, writable: false, publiclyVisible: true);
        }

        public override string ToString() => $"{Name} ({Size} bytes)";
    }
}
#endif
