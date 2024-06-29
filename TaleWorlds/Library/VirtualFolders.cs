using System;
using System.IO;
using System.Reflection;

namespace TaleWorlds.Library;

public class VirtualFolders
{
	[VirtualDirectory("..")]
	public class Win64_Shipping_Client
	{
		[VirtualDirectory("..")]
		public class bin
		{
			[VirtualDirectory("Parameters")]
			public class Parameters
			{
				[VirtualDirectory("ClientProfiles")]
				public class ClientProfiles
				{
					[VirtualDirectory("DigitalOcean.Discovery")]
					public class DigitalOceanDiscovery
					{
						[VirtualFile("LobbyClient.xml", "<Configuration>\t<SessionProvider Type=\"ThreadedRest\" />\t<Clients>\t\t<Client Type=\"LobbyClient\" />\t</Clients>\t<Parameters>\t\t<Parameter Name=\"LobbyClient.ServiceDiscovery.Address\" Value=\"https://bannerlord-service-discovery.bannerlord-services-3.net/\" />\t\t\t\t<Parameter Name=\"LobbyClient.Address\" Value=\"service://bannerlord.lobby/\" />\t\t<Parameter Name=\"LobbyClient.Port\" Value=\"443\" />\t\t<Parameter Name=\"LobbyClient.IsSecure\" Value=\"true\" />\t</Parameters></Configuration>")]
						public string LobbyClient;
					}
				}

				[VirtualFile("Environment", "bwa7JVwQ6hEmBuFHLrVnqKEjXI6q749vlJONzBSDYWtmSyECpQZEducl__Hl.TOcVX4vN7y2z4z1OWnpN7mIWmH1sQXbi5QexhpPpUdDeYpL_EBf7uL4m1toYGdbIv0cd6_MnUi4uIyBXyoiwev7_49IubPGbPf6fCaHsh1Oiws-")]
				public string Environment;

				[VirtualFile("Version.xml", "<Version>\t<Singleplayer Value=\"v1.2.10\" /></Version>")]
				public string Version;

				[VirtualFile("ClientProfile.xml", "<ClientProfile Value=\"DigitalOcean.Discovery\"/>")]
				public string ClientProfile;
			}
		}
	}

	private static readonly bool _useVirtualFolders = true;

	public static string GetFileContent(string filePath)
	{
		if (!_useVirtualFolders)
		{
			if (!File.Exists(filePath))
			{
				return "";
			}
			return File.ReadAllText(filePath);
		}
		return GetVirtualFileContent(filePath);
	}

	private static string GetVirtualFileContent(string filePath)
	{
		string fileName = Path.GetFileName(filePath);
		string[] array = Path.GetDirectoryName(filePath).Split(new char[1] { Path.DirectorySeparatorChar });
		Type type = typeof(VirtualFolders);
		int num = 0;
		while (type != null && num != array.Length)
		{
			if (!string.IsNullOrEmpty(array[num]))
			{
				type = GetNestedDirectory(array[num], type);
			}
			num++;
		}
		if (type != null)
		{
			FieldInfo[] fields = type.GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				VirtualFileAttribute[] array2 = (VirtualFileAttribute[])fields[i].GetCustomAttributesSafe(typeof(VirtualFileAttribute), inherit: false);
				if (array2[0].Name == fileName)
				{
					return array2[0].Content;
				}
			}
		}
		return "";
	}

	private static Type GetNestedDirectory(string name, Type type)
	{
		Type[] nestedTypes = type.GetNestedTypes();
		foreach (Type type2 in nestedTypes)
		{
			if (((VirtualDirectoryAttribute[])type2.GetCustomAttributesSafe(typeof(VirtualDirectoryAttribute), inherit: false))[0].Name == name)
			{
				return type2;
			}
		}
		return null;
	}
}
