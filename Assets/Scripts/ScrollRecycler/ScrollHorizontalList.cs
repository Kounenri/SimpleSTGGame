using UnityEngine;

namespace ScrollRecycler
{
	public class ScrollHorizontalList : ScrollRecycleBase
	{
		private int columnSum;

		protected override void RefreshAdapter()
		{
			if (!AdapterOnInit)
			{
				return;
			}

			if (_hasAdapter)
			{
				return;
			}

			_hasAdapter = true;

			float nItemWidth = 0;
			float nItemHeight = 0;

			if (usePrefabSize)
			{
				nItemWidth = PrefItem.GetComponent<RectTransform>().rect.width;
				nItemHeight = PrefItem.GetComponent<RectTransform>().rect.height;
			}
			else
			{
				nItemWidth = itemWidth * 1.0f;
				nItemHeight = itemHeight * 1.0f;
			}

			float scrollRectHeight = 0;
			float scrollRectWidth = 0;
			int colCount = 0;
			int rowCount = 0;

			if (!AdapterWidthOrHeight)
			{
				scrollRectHeight = GetComponent<RectTransform>().rect.height - padding.top;
				scrollRectWidth = GetComponent<RectTransform>().rect.width - padding.right - padding.left;

				colCount = Mathf.CeilToInt((scrollRectWidth + offsetX) / (nItemWidth + offsetX)) + 1;
				rowCount = Mathf.FloorToInt((scrollRectHeight + offsetY) / (nItemHeight + offsetY));
			}
			else
			{
				scrollRectWidth = GetComponent<RectTransform>().rect.width - padding.left;
				colCount = Mathf.CeilToInt((scrollRectWidth + offsetX) / (nItemWidth + offsetX)) + 1;

				nItemHeight = GetComponent<RectTransform>().rect.height - padding.top - padding.bottom;
				rowCount = 1;
			}

			SetMargin((int)nItemWidth, (int)nItemHeight, colCount, rowCount, !AdapterWidthOrHeight);
		}

		protected override void SetVertWidOrHorHeightOnOffset()
		{
			rectHeight = (rowCount - 1) * (itemHeight + offsetY);
		}

		protected override void SetVertHeightOrHorWid(int count)
		{
			columnSum = count / rowCount + (count % rowCount > 0 ? 1 : 0);
			rectWidth = Mathf.Max(0, columnSum * itemWidth + (columnSum - 1) * offsetX) + (padding.left + padding.right);
		}

		protected override Vector2 GetPos(int index)
		{
			var spread = 0;
			return new Vector2(index / rowCount * (itemWidth + offsetX) + padding.left,
				-index % rowCount * (itemHeight + offsetY) - spread - padding.top);
		}

		protected override int GetStartIndex(Vector2 pos)
		{
			var value = pos.x;
			value = -value;
			var _spreadWidth = 0;
			if (value <= itemWidth + _spreadWidth)
				return 0;
			var scrollWidth = gameObject.GetComponent<RectTransform>().sizeDelta.x;
			if (value >= itemParent.sizeDelta.x - scrollWidth - _spreadWidth)
			{
				if (listCount <= createCount)
					return 0;
				return listCount - createCount;
			}

			return ((int)((value - _spreadWidth) / (itemWidth + offsetX)) +
					((value - _spreadWidth) % (itemWidth + offsetX) > 0 ? 1 : 0) - 1) * rowCount;
		}
	}
}