using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TConfProxy<T, U> : TInstanceLite<T> where T : class where U : TDataVO
{
	//private string m_ConfigXML;
	//private Dictionary<int, U> m_DataVODictionary;
	protected Dictionary<int, U> m_DataVODictionary;
	private Thread m_ParseThread;
	private bool m_IsDone = false;

	protected int m_NodeCount;

	public bool IsDone
	{
		get
		{
			return m_IsDone && m_DataVODictionary != null;
		}
	}

	protected Dictionary<int, U> DataVODictionary
	{
		get
		{
			if (!m_IsDone) ParseConfig();

			return m_DataVODictionary;
		}
	}

	protected virtual bool AllowGetNullDataVO
	{
		get
		{
			return false;
		}
	}

	//protected virtual string BundleName
	//{
	//	get
	//	{
	//		return Configuration.CONF_PATH;
	//	}
	//}

	protected virtual string ConfName
	{
		get
		{
			return string.Empty;
		}
	}

	protected virtual string TabName
	{
		get
		{
			return string.Empty;
		}
	}

	protected virtual string NodeName
	{
		get
		{
			return string.Empty;
		}
	}

	protected virtual string ValuePath
	{
		get
		{
			return ">0>_text";
		}
	}

	public TConfProxy()
	{
		// To manage all conf proxy
		//Configuration.OnResetConf += OnResetConfig;
	}

	protected virtual void OnResetConfig(bool bIsALL)
	{
		//if (BundleName == Configuration.CONF_PATH || bIsALL)
		//{
		//	if (m_DataVODictionary != null)
		//	{
		//		m_DataVODictionary.Clear();
		//		m_DataVODictionary = null;
		//	}

		//	m_IsDone = false;
		//}
	}

	protected override void OnDispose()
	{
		if (m_ParseThread != null)
		{
			m_ParseThread.Abort();
			m_ParseThread = null;
		}

		if (m_DataVODictionary != null)
		{
			m_DataVODictionary.Clear();
			m_DataVODictionary = null;
		}

		base.OnDispose();
	}

	public void ParseConfigAync()
	{
		//m_ConfigXML = Configuration.LoadConfig(ConfName, BundleName);
		m_ParseThread = new Thread(new ThreadStart(ParseConfig)) { IsBackground = true };

		m_ParseThread.Start();
	}

	public virtual void ParseConfig()
	{
		m_DataVODictionary = new Dictionary<int, U>();
		m_NodeCount = 0;

		try
		{
			if (string.IsNullOrEmpty(TabName))
			{
				throw new Exception("TConfProxy Error - " + typeof(T).Name + ",TabName Unset!");
			}
			else
			{
				if (string.IsNullOrEmpty(NodeName))
				{
					throw new Exception("TConfProxy Error - " + typeof(T).Name + ",NodeName Unset!");
				}
				else
				{
					// Load from xml
					//if (string.IsNullOrEmpty(m_ConfigXML)) m_ConfigXML = Configuration.LoadConfig(ConfName, BundleName);

					//XMLParser pXMLParser = new XMLParser();
					//XMLNode pXMLRoot = pXMLParser.Parse(m_ConfigXML);
					//XMLNodeList pXMLList = pXMLRoot.GetNodeList(TabName + ">0>" + NodeName);

					//if (pXMLList != null && pXMLList.Count > 0)
					//{
					//	m_NodeCount = pXMLList.Count;

					//	for (int i = 0; i < m_NodeCount; i++)
					//	{
					//		U pVO = OnParserNode(pXMLList[i] as XMLNode);

					//		if (pVO != null) m_DataVODictionary[pVO.ID] = pVO;
					//	}
					//}
					//else
					//{
					//	throw new Exception("TConfProxy Error - " + typeof(T).Name + ",Load empty file!");
					//}

					//pXMLParser = null;
					//pXMLRoot = null;
					//pXMLList = null;
					//m_ConfigXML = string.Empty;
				}
			}
		}
		catch (Exception pException)
		{
			Debug.LogException(pException);
		}
		finally
		{
			SetSourceFinished();
		}
	}

	protected virtual void SetSourceFinished()
	{
		m_IsDone = true;
	}

	//protected virtual U OnParserNode(XMLNode pXMLNode)
	//{
	//	return null;
	//}

	public void ResetConfig()
	{
		if (m_IsDone) OnResetConfig(true);
	}

	public virtual U GetDataVO(int nID)
	{
		if (!m_IsDone) ParseConfig();

		if (m_DataVODictionary.ContainsKey(nID))
		{
			return m_DataVODictionary[nID];
		}
		else
		{
			if (AllowGetNullDataVO)
			{
				return null;
			}
			else
			{
				throw new Exception("TConfProxy Error - Ù–@Q“ž" + typeof(U).Name + ",ID : " + nID);
			}
		}
	}

	public virtual bool HasDataVO(int nID)
	{
		if (!m_IsDone) ParseConfig();

		return m_DataVODictionary.ContainsKey(nID);
	}

	public virtual List<U> GetDataVOList()
	{
		if (!m_IsDone) ParseConfig();

		return new List<U>(m_DataVODictionary.Values);
	}
}
