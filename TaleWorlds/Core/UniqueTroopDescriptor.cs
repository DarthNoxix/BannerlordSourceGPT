using System;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core;

public struct UniqueTroopDescriptor : IComparable<UniqueTroopDescriptor>, IEquatable<UniqueTroopDescriptor>, ISavedStruct
{
	private static readonly UniqueTroopDescriptor InvalidDescriptor;

	[SaveableProperty(1)]
	public int UniqueSeed { get; private set; }

	public static UniqueTroopDescriptor Invalid => InvalidDescriptor;

	public bool IsValid => UniqueSeed > 0;

	public static void AutoGeneratedStaticCollectObjectsUniqueTroopDescriptor(object o, List<object> collectedObjects)
	{
		((UniqueTroopDescriptor)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
	}

	private void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
	{
	}

	internal static object AutoGeneratedGetMemberValueUniqueSeed(object o)
	{
		return ((UniqueTroopDescriptor)o).UniqueSeed;
	}

	public UniqueTroopDescriptor(int uniqueSeed)
	{
		this = default(UniqueTroopDescriptor);
		UniqueSeed = uniqueSeed;
	}

	public int CompareTo(UniqueTroopDescriptor other)
	{
		return UniqueSeed.CompareTo(other.UniqueSeed);
	}

	public int CompareTo(int uniqueSeed)
	{
		return UniqueSeed.CompareTo(uniqueSeed);
	}

	public override string ToString()
	{
		return UniqueSeed.ToString();
	}

	public override bool Equals(object obj)
	{
		if (!(obj is UniqueTroopDescriptor uniqueTroopDescriptor))
		{
			return false;
		}
		return uniqueTroopDescriptor.UniqueSeed == UniqueSeed;
	}

	public bool Equals(UniqueTroopDescriptor other)
	{
		return other.UniqueSeed == UniqueSeed;
	}

	public override int GetHashCode()
	{
		return UniqueSeed;
	}

	bool ISavedStruct.IsDefault()
	{
		return UniqueSeed == 0;
	}

	public static bool operator ==(UniqueTroopDescriptor a, UniqueTroopDescriptor b)
	{
		return a.UniqueSeed == b.UniqueSeed;
	}

	public static bool operator !=(UniqueTroopDescriptor a, UniqueTroopDescriptor b)
	{
		return !(a == b);
	}
}
