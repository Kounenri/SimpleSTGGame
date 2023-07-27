using UnityEngine;
using UnityEngine.EventSystems;

namespace ScrollRecycler
{
	public class RecycleItemRender : MonoBehaviour, IPointerClickHandler
	{
		public int index;

		[SerializeField]
		protected bool m_UpdateSelectRender = false;
		/// <summary>
		/// 仅兼容性处理使用
		/// </summary>
		protected IRecycleData m_Data;

		public IRecycleData Data
		{
			get { return m_Data; }
		}

		public void Refresh(IRecycleData data, int index, bool isSelect)
		{
			m_Data = data;
			OnSetData();
			RefreshView();
			this.index = index;
			if (isSelect)
			{
				OnSelect();
			}
			else
			{
				OnDisSelect();
			}
		}

		/// <summary>
		/// 为了兼容 ARender
		/// </summary>
		protected virtual void RefreshView()
		{
			// _data as yours object.
		}

		/// <summary>
		/// 仅兼容性处理使用
		/// </summary>
		/// <param name="data"></param>
		protected virtual void OnSetData()
		{
		}

		public virtual void OnSelect()
		{
		}

		public virtual void OnDisSelect()
		{
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (!InputUtility.IsSingleTouch) return;
			OnClickRender(Vector3.zero);
			if (m_UpdateSelectRender)
			{
				GetComponentInParent<ScrollRecycleBase>().SelectGameobject = gameObject;
				GetComponentInParent<ScrollRecycleBase>().SelectIndex = index;
			}
		}

		public virtual void OnClickRender(Vector3 pClickPosition)
		{
#if UNITY_EDITOR
			Debug.Log(@"Render Click");

			if (m_Data != null)
			{
				Debug.Log(@"Data Type : " + m_Data.ToString());

				//	if (data is TDataVO)
				//	{
				//		Debug.Log(@"Data ID : " + (data as TDataVO).ID);
				//	}
			}
#endif
		}
	}


	public interface IRecycleData
	{
	}
}