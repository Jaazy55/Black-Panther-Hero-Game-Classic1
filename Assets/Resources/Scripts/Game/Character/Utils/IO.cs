using System;
using System.IO;

namespace Game.Character.Utils
{
	public static class IO
	{
		public static void CopyDirs(string sourceDirectory, string targetDirectory)
		{
			DirectoryInfo source = new DirectoryInfo(sourceDirectory);
			DirectoryInfo target = new DirectoryInfo(targetDirectory);
			IO.CopyAll(source, target);
		}

		private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
		{
			if (!Directory.Exists(target.FullName))
			{
				Directory.CreateDirectory(target.FullName);
			}
			foreach (FileInfo fileInfo in source.GetFiles())
			{
				fileInfo.CopyTo(Path.Combine(target.ToString(), fileInfo.Name), true);
			}
			foreach (DirectoryInfo directoryInfo in source.GetDirectories())
			{
				DirectoryInfo target2 = target.CreateSubdirectory(directoryInfo.Name);
				IO.CopyAll(directoryInfo, target2);
			}
		}

		public static string GetFileName(string absolutPath)
		{
			return Path.GetFileName(absolutPath);
		}

		public static string GetFileNameWithoutExtension(string absolutPath)
		{
			return Path.GetFileNameWithoutExtension(absolutPath);
		}

		public static string GetExtension(string absolutPath)
		{
			return Path.GetExtension(absolutPath).ToLower();
		}

		public static string ReadTextFile(string absolutPath)
		{
			if (File.Exists(absolutPath))
			{
				StreamReader streamReader = new StreamReader(absolutPath);
				string result = streamReader.ReadToEnd();
				streamReader.Close();
				return result;
			}
			return null;
		}

		public static void WriteTextFile(string absolutPath, string content)
		{
			StreamWriter streamWriter = new StreamWriter(absolutPath);
			streamWriter.Write(content);
			streamWriter.Close();
		}

		public static bool DeleteFile(string absolutPath)
		{
			if (File.Exists(absolutPath))
			{
				File.Delete(absolutPath);
				return true;
			}
			return false;
		}

		public static void DeleteDir(string absolutPath)
		{
			if (IO.DeleteFile(absolutPath))
			{
				return;
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(absolutPath);
			foreach (FileInfo fileInfo in directoryInfo.GetFiles())
			{
				fileInfo.Delete();
			}
			foreach (DirectoryInfo directoryInfo2 in directoryInfo.GetDirectories())
			{
				IO.DeleteDir(directoryInfo2.FullName);
				if (Directory.Exists(directoryInfo2.FullName))
				{
					directoryInfo2.Delete();
				}
			}
			Directory.Delete(absolutPath);
		}

		public static void RenameFile(string absolutPath, string newName)
		{
			string destFileName = Path.GetDirectoryName(absolutPath) + "/" + newName;
			File.Move(absolutPath, destFileName);
		}

		public static bool CopyFile(string src, string dst, bool overwrite)
		{
			if (File.Exists(src) && (!File.Exists(dst) || overwrite))
			{
				File.Copy(src, dst, overwrite);
				return true;
			}
			return false;
		}

		public static void WriteBinaryFile(string path, byte[] bytes)
		{
			FileStream fileStream = File.Open(path, FileMode.Create);
			BinaryWriter binaryWriter = new BinaryWriter(fileStream);
			binaryWriter.Write(bytes);
			fileStream.Close();
		}

		public static string ConvertFileSeparators(string path)
		{
			return path.Replace("\\", "/");
		}

		public static bool FileExists(string path)
		{
			return File.Exists(path);
		}
	}
}
