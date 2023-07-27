using System.IO;
using UnityEngine;

internal static class PathUtil
{
	public static bool CheckFileExists(string strPath)
	{
		bool bExists = File.Exists(strPath);

		return bExists;
	}

	public static string ResourcePath
	{
		get
		{
			return Combine(Application.persistentDataPath,@"Resources");
		}
	}

	public static string GetPath(string strPath)
	{
		strPath = strPath.Replace(@"//",@"/");
		strPath = strPath.Replace(@"\\",@"\");

		if(Path.DirectorySeparatorChar == '\\')
		{
			return strPath.Replace(@"/",Path.DirectorySeparatorChar.ToString());
		}
		else
		{
			return strPath.Replace(@"\",Path.DirectorySeparatorChar.ToString());
		}
	}

	public static string Combine(string strPath1,string strPath2,string strExtension = @"")
	{
		strPath1 = GetPath(strPath1);
		strPath2 = GetPath(strPath2);

		if(!strPath1.EndsWith(Path.DirectorySeparatorChar.ToString()))
		{
			strPath1 += Path.DirectorySeparatorChar.ToString();
		}

		if(strPath2.StartsWith(Path.DirectorySeparatorChar.ToString()))
		{
			strPath2 = strPath2.Substring(1,strPath2.Length - 1);
		}

		if(!string.IsNullOrEmpty(strExtension))
		{
			if(strPath2.EndsWith(Path.DirectorySeparatorChar.ToString()))
			{
				strPath2 = strPath2.Substring(0,strPath2.Length - 1);
			}

			if(strPath2.IndexOf(@"." + strExtension) == -1 && strPath2.IndexOf(@".") == -1)
			{
				strPath2 += @"." + strExtension;
			}
		}

		return strPath1 + strPath2;
	}

	public static string CombineExtension(string strPath,string strExtension)
	{
		strPath = GetPath(strPath);

		if(strPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
		{
			strPath = strPath.Substring(0,strPath.Length - 1);
		}

		if(strPath.IndexOf(@"." + strExtension) == -1 && strPath.IndexOf(@".") == -1)
		{
			strPath += @"." + strExtension;
		}

		return strPath;
	}
}
