using System;

public class EnumUtil
{
	public static uint GetEnumValue<T>(T TValue)
	{
		T TType = (T)Enum.Parse(typeof(T), TValue.ToString());

		uint uCmd = (uint)TType.GetHashCode();

		return uCmd;
	}

	public static T GetEnum<T>(string strValue)
	{
		T TType = (T)Enum.Parse(typeof(T), strValue, true);

		return TType;
	}
}
