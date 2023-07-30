using System.Collections;
using System.Collections.Generic;

public class DataProviderUtil
{
	public static List<object> GetDataProvier(IList pSourceList)
	{
		if (pSourceList == null) return null;

		List<object> pResList = new();

		int nCount = pSourceList.Count;

		for (int i = 0; i < nCount; i++)
		{
			pResList.Add(pSourceList[i]);
		}

		return pResList;
	}
}
