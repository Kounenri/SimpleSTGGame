using System;

public class TDataVO : IComparable
{
	public int ID
	{
		get; set;
	}

	public virtual int CompareTo(object pObject)
	{
		if (pObject == null)
		{
			return 1;
		}

		TDataVO pTDataVO = pObject as TDataVO;

		if (pTDataVO != null)
		{
			return ID.CompareTo(pTDataVO.ID);
		}
		else
		{
			throw new ArgumentException(@"Object is not a TDataVO");
		}
	}

	public override int GetHashCode()
	{
		return ToString().GetHashCode();
	}

	public override string ToString()
	{
		Type pType = GetType();

		return string.Format(@"{0}_{1}", pType.FullName, ID);
	}

	public override bool Equals(object pObject)
	{
		if (pObject is TDataVO)
		{
			TDataVO pTDataVO = pObject as TDataVO;

			return ID.Equals(pTDataVO.ID);
		}
		else
		{
			return base.Equals(pObject);
		}
	}

	public static bool operator ==(TDataVO lhs, TDataVO rhs)
	{
		if (Equals(lhs, null) || Equals(rhs, null))
		{
			return Equals(lhs, rhs);
		}
		else
		{
			return lhs.ID == rhs.ID;
		}
	}

	public static bool operator !=(TDataVO lhs, TDataVO rhs)
	{
		if (Equals(lhs, null) || Equals(rhs, null))
		{
			return !Equals(lhs, rhs);
		}
		else
		{
			return lhs.ID != rhs.ID;
		}
	}
}
