using System;
using System.Diagnostics;

namespace TaleWorlds.DotNet.AutoGenerated;

internal class SizeChecker
{
	[Conditional("_RGL_KEEP_ASSERTS")]
	private static void CheckTypeSizeAux(int managedSize, string name)
	{
		LibraryApplicationInterface.ILibrarySizeChecker.GetEngineStructSize(name);
	}

	[Conditional("_RGL_KEEP_ASSERTS")]
	private static void CheckTypeOffsetAux(IntPtr managedOffset, string className, string memberName)
	{
		LibraryApplicationInterface.ILibrarySizeChecker.GetEngineStructMemberOffset(className, memberName);
	}

	[Conditional("_RGL_KEEP_ASSERTS")]
	public static void CheckSharedStructureSizes()
	{
	}

	[Conditional("_RGL_KEEP_ASSERTS")]
	public static void CheckSharedStructureOffsets()
	{
	}
}
