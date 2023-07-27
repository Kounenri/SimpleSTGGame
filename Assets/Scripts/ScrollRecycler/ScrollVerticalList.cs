using UnityEngine;

namespace ScrollRecycler
{
	public class ScrollVerticalList : ScrollRecycleBase
	{
		public float Spread;
		private int rowSum;


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

				rowCount = Mathf.CeilToInt((scrollRectHeight + offsetY) / (nItemHeight + offsetY)) + 1;
				colCount = Mathf.FloorToInt((scrollRectWidth + offsetX) / (nItemWidth + offsetX));
			}
			else
			{
				scrollRectHeight = GetComponent<RectTransform>().rect.height - padding.top;
				rowCount = Mathf.CeilToInt((scrollRectHeight + offsetY) / (nItemHeight + offsetY)) + 1;

				nItemWidth = GetComponent<RectTransform>().rect.width - padding.right - padding.left;
				colCount = 1;
			}
			SetMargin((int)nItemWidth, (int)nItemHeight, colCount, rowCount, !AdapterWidthOrHeight);
		}

		protected override void SetVertWidOrHorHeightOnOffset()
		{
			rectWidth = (columnCount - 1) * (itemWidth + offsetX) + (padding.left + padding.right);
		}

		protected override void SetVertHeightOrHorWid(int count)
		{
			rowSum = count / columnCount + (count % columnCount > 0 ? 1 : 0); //计算有多少行，用于计算出总高度
			rectHeight = Mathf.Max(0, rowSum * itemHeight + (rowSum - 1) * offsetY) + (padding.top + padding.bottom);
		}

		protected override Vector2 GetPos(int index)
		{
			return new Vector2(index % columnCount * (itemWidth + offsetX) + padding.left,
				-index / columnCount * (itemHeight + offsetY) - Spread - padding.top);
		}

		protected override int GetStartIndex(Vector2 pos)
		{
			var value = pos.y;
			var _spreadHeight = 0;
			if (value <= itemHeight + _spreadHeight)
				return 0;
			var scrollHeight = gameObject.GetComponent<RectTransform>().sizeDelta.y;
			if (value >= itemParent.sizeDelta.y - scrollHeight - _spreadHeight) //拉到底部了
			{
				if (listCount <= createCount)
					return 0;
				return listCount - createCount;
			}

			return ((int)((value - _spreadHeight) / (itemHeight + offsetY)) +
					((value - _spreadHeight) % (itemHeight + offsetY) > 0 ? 1 : 0) - 1) * columnCount;
		}
	}
}