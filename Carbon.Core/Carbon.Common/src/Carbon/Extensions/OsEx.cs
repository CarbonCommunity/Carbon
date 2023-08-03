/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Extensions;

public class OsEx
{
	public static class File
	{
		public static bool Exists(string file)
		{
			if (string.IsNullOrEmpty(file))
			{
				return false;
			}

			return System.IO.File.Exists(file);
		}
		public static void Create(string file, string content)
		{
			if (!string.IsNullOrEmpty(file))
			{
				System.IO.File.WriteAllText(file, content);
			}
		}
		public static void Create(string file, string[] contents)
		{
			if (!string.IsNullOrEmpty(file))
			{
				System.IO.File.WriteAllLines(file, contents);
			}
		}
		public static void Create(string file, byte[] contents)
		{
			if (!string.IsNullOrEmpty(file))
			{
				System.IO.File.WriteAllBytes(file, contents);
			}
		}
		public static void Delete(string file)
		{
			if (!string.IsNullOrEmpty(file))
			{
				if (System.IO.File.Exists(file))
				{
					System.IO.File.Delete(file);
				}
			}
		}

		public static string Copy(string file, string destination, bool overwrite = true)
		{
			if (!string.IsNullOrEmpty(file) && !string.IsNullOrEmpty(destination))
			{
				if (System.IO.File.Exists(file))
				{
					var destinationFolder = Path.GetDirectoryName(destination);
					if (!string.IsNullOrEmpty(destinationFolder))
					{
						if (!Directory.Exists(destinationFolder))
						{
							Directory.CreateDirectory(destinationFolder);
						}
					}

					System.IO.File.Copy(file, destination, overwrite);
				}
			}

			return destination;
		}
		public static string Move(string file, string destination, bool overwrite = true)
		{
			if (!string.IsNullOrEmpty(file) && !string.IsNullOrEmpty(destination))
			{
				if (System.IO.File.Exists(file))
				{
					Copy(file, destination);
					Delete(file);
				}
			}

			return destination;
		}
		public static string Find(string filter, string folder, SearchOption option = SearchOption.AllDirectories)
		{
			foreach (var f in Folder.GetFilesWithExtension(folder, "*", option))
			{
				if (f.Contains(filter))
				{
					return f;
				}
			}

			return null;
		}

		public static readonly string EMPTY_STRING = string.Empty;
		public static readonly string[] EMPTY_STRARRAY = new string[0];
		public static readonly byte[] EMPTY_BYTEARRAY = new byte[0];

		public static string ReadText(string file)
		{
			if (!string.IsNullOrEmpty(file))
			{
				if (System.IO.File.Exists(file))
				{
					return System.IO.File.ReadAllText(file);
				}
			}

			return EMPTY_STRING;
		}
		public static string[] ReadTextLines(string file)
		{
			if (!string.IsNullOrEmpty(file))
			{
				if (System.IO.File.Exists(file))
				{
					return System.IO.File.ReadAllLines(file);
				}
			}

			return EMPTY_STRARRAY;
		}
		public static byte[] ReadBytes(string file)
		{
			if (!string.IsNullOrEmpty(file))
			{
				if (System.IO.File.Exists(file))
				{
					return System.IO.File.ReadAllBytes(file);
				}
			}

			return EMPTY_BYTEARRAY;
		}
	}

	public static class Folder
	{
		public static bool Exists(string folder)
		{
			if (string.IsNullOrEmpty(folder))
			{
				return false;
			}

			return Directory.Exists(folder);
		}
		public static void Create(string folder, bool recreate = false)
		{
			if (!string.IsNullOrEmpty(folder))
			{
				if (recreate)
				{
					if (Directory.Exists(folder))
					{
						Directory.Delete(folder, true);
					}
				}

				if (!Directory.Exists(folder))
				{
					Directory.CreateDirectory(folder);
				}
			}
		}

		public static void Delete(string folder)
		{
			if (!string.IsNullOrEmpty(folder))
			{
				if (Directory.Exists(folder) && !string.IsNullOrEmpty(folder))
				{
					Directory.Delete(folder, true);
				}
			}
		}
		public static void DeleteFilesWithExtension(string folder, string extension, SearchOption option = SearchOption.AllDirectories)
		{
			if (!string.IsNullOrEmpty(folder))
			{
				if (Exists(folder))
				{
					foreach (var file in Directory.GetFiles(folder, string.Format("*.{0}", extension), option))
					{
						System.IO.File.Delete(file);
					}
				}
			}
		}
		public static void DeleteFilesWithExtension(string folder, string extension, string[] exceptions, SearchOption option = SearchOption.AllDirectories)
		{
			if (Directory.Exists(folder) && !string.IsNullOrEmpty(folder))
			{
				foreach (var file in Directory.GetFiles(folder, string.Format("*.{0}", extension), option))
				{
					if (exceptions.Any(x => x != Path.GetFileName(file)))
					{
						System.IO.File.Delete(file);
					}
				}
			}
		}
		public static void DeleteContents(string folder, string folderPattern = "*", string filePattern = "*", SearchOption option = SearchOption.AllDirectories)
		{
			if (Directory.Exists(folder) && !string.IsNullOrEmpty(folder))
			{
				foreach (var _file in Directory.GetFiles(folder, filePattern, option))
				{
					if (System.IO.File.Exists(_file))
					{
						System.IO.File.Delete(_file);
					}
				}

				foreach (var _folder in Directory.GetDirectories(folder, folderPattern, option))
				{
					if (Directory.Exists(_folder))
					{
						Directory.Delete(_folder, true);
					}
				}
			}
		}
		public static Dictionary<string, string> Copy(string folder, string destination, bool subdirectories = true, bool overwrite = true, SearchOption option = SearchOption.AllDirectories)
		{
			if (!string.IsNullOrEmpty(folder) && !string.IsNullOrEmpty(destination))
			{
				if (Directory.Exists(folder))
				{
					var folderInfo = new DirectoryInfo(folder);
					var retDictionary = new Dictionary<string, string>();

					var folders = folderInfo.GetDirectories();
					Create(destination);
					retDictionary.Add(folderInfo.FullName, destination);

					var files = folderInfo.GetFiles();
					foreach (var file in files)
					{
						var tempPath = Path.Combine(destination, file.Name);
						file.CopyTo(tempPath, overwrite);
						retDictionary.Add(file.FullName, tempPath);
					}

					foreach (var subDirectory in folders)
					{
						var tempPath = Path.Combine(destination, subDirectory.Name);
						Copy(subDirectory.FullName, tempPath, subdirectories, overwrite, option);
						retDictionary.Add(subDirectory.FullName, tempPath);
					}

					return retDictionary;
				}
			}

			return null;
		}
		public static List<string> Move(string folder, string destination, bool subdirectories = true, bool overwrite = true, SearchOption option = SearchOption.AllDirectories)
		{
			if (!string.IsNullOrEmpty(folder) && !string.IsNullOrEmpty(destination))
			{
				if (Directory.Exists(folder))
				{
					var folderInfo = new DirectoryInfo(folder);
					var retList = new List<string>();

					var folders = folderInfo.GetDirectories();
					Create(destination);
					retList.Add(destination);

					var files = folderInfo.GetFiles();
					foreach (var file in files)
					{
						var tempPath = Path.Combine(destination, file.Name);
						file.CopyTo(tempPath, overwrite);
						retList.Add(tempPath);
						file.Delete();
					}

					foreach (var subDirectory in folders)
					{
						var tempPath = Path.Combine(destination, subDirectory.Name);
						Copy(subDirectory.FullName, tempPath, subdirectories, overwrite, option);
						retList.Add(tempPath);
						subDirectory.Delete(subdirectories);
					}

					Delete(folder);

					return retList;
				}
			}

			return null;
		}
		public static string[] GetFilesWithExtension(string folder, string extension, SearchOption option = SearchOption.AllDirectories)
		{
			var files = new List<string>();

			if (Directory.Exists(folder) && !string.IsNullOrEmpty(folder))
			{
				foreach (var file in Directory.GetFiles(folder, string.Format("*.{0}", extension), option))
				{
					files.Add(file);
				}
			}

			return files.ToArray();
		}
		public static string[] GetFilesWithExtension(string folder, string extension, string[] exceptions, SearchOption option = SearchOption.AllDirectories)
		{
			var files = new List<string>();

			if (Directory.Exists(folder) && !string.IsNullOrEmpty(folder))
			{
				foreach (var file in Directory.GetFiles(folder, string.Format("*.{0}", extension), option))
				{
					if (exceptions.Any(x => x != Path.GetFileName(file)))
					{
						files.Add(file);
					}
				}
			}

			return files.ToArray();
		}
	}

	public static class Utils
	{
		public static string Copy(string fileOrFolder, string destination, bool subdirectories = true, bool overwrite = true)
		{
			if (!string.IsNullOrEmpty(fileOrFolder) && !string.IsNullOrEmpty(destination))
			{
				if (File.Exists(fileOrFolder))
				{
					File.Copy(fileOrFolder, destination, overwrite);
				}
				else if (Folder.Exists(fileOrFolder))
				{
					Folder.Copy(fileOrFolder, destination, subdirectories, overwrite);
				}

				return destination;
			}

			return fileOrFolder;
		}
		public static string Move(string fileOrFolder, string destination, bool subdirectories = true, bool overwrite = true)
		{
			if (!string.IsNullOrEmpty(fileOrFolder) && !string.IsNullOrEmpty(destination))
			{
				if (File.Exists(fileOrFolder))
				{
					File.Move(fileOrFolder, destination, overwrite);
				}
				else if (Folder.Exists(fileOrFolder))
				{
					Folder.Move(fileOrFolder, destination, subdirectories, overwrite);
				}

				return destination;
			}

			return fileOrFolder;
		}
		public static void Delete(string fileOrFolder)
		{
			if (!string.IsNullOrEmpty(fileOrFolder))
			{
				if (System.IO.File.Exists(fileOrFolder))
				{
					File.Delete(fileOrFolder);
				}
				else if (Directory.Exists(fileOrFolder))
				{
					Folder.Delete(fileOrFolder);
				}
			}
		}
	}
}
