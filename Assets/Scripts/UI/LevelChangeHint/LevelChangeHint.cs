using TMPro;
using UnityEngine;

public class LevelChangeHint : BaseCanvas
{
	public static LevelChangeHint Create(LevelVO pLevelVO)
	{
		GameObject pPrefab = Resources.Load<GameObject>("UI/LevelChangeHint/LevelChangeHint");

		GameObject pInstance = Instantiate(pPrefab);
		LevelChangeHint pLevelChangeHint = pInstance.GetComponent<LevelChangeHint>();
		pLevelChangeHint.m_LevelVO = pLevelVO;

		return pLevelChangeHint;
	}

	private TMP_Text m_Text;
	private LevelVO m_LevelVO;

	protected override void Awake()
	{
		base.Awake();

		m_Text = this.Find<TMP_Text>("Panel/Text");
	}

	protected override void Start()
	{
		base.Start();

		m_Text.text = m_LevelVO.Name + "\nStart!";
	}
}
