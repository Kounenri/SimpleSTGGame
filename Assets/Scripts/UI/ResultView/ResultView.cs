using TMPro;
using UnityEngine;

public class ResultView : BaseCanvas
{
	public static ResultView Create(LevelResultEnum pResult)
	{
		GameObject pPrefab = Resources.Load<GameObject>("UI/ResultView/ResultView");

		GameObject pInstance = Instantiate(pPrefab);
		ResultView pResultView = pInstance.GetComponent<ResultView>();
		pResultView.m_Result = pResult;

		return pResultView;
	}

	private TMP_Text m_Text;
	private GameObject m_RestartBtn;
	private GameObject m_ContinueBtn;
	private LevelResultEnum m_Result;

	protected override void Awake()
	{
		base.Awake();

		m_Text = this.Find<TMP_Text>("Panel/Text");
		m_RestartBtn = this.GetTGameObject("Panel/Panel/Button_Restart");
		m_ContinueBtn = this.GetTGameObject("Panel/Panel/Button_Continue");
	}

	protected override void Start()
	{
		base.Start();

		if (m_Result == LevelResultEnum.LevelClear)
		{
			m_Text.text = "The current level has been completed, go to the next level.";
			m_Text.color = Color.green;

			m_RestartBtn.SetActive(false);
			m_ContinueBtn.SetActive(true);
		}
		else if (m_Result == LevelResultEnum.AllLevelClear)
		{
			m_Text.text = "Congratulations! All levels have been completed.";
			m_Text.color = Color.yellow;

			m_RestartBtn.SetActive(false);
			m_ContinueBtn.SetActive(false);
		}
		else if (m_Result == LevelResultEnum.PlayerDead)
		{
			m_Text.text = "No luck, the character dies. TAT";
			m_Text.color = Color.red;

			if (LevelManager.GetInstance.RetryCount > 0)
			{
				m_Text.text += "\nRetryCount (" + LevelManager.GetInstance.RetryCount + ")";
			}

			m_RestartBtn.SetActive(true);
			m_ContinueBtn.SetActive(false);
		}
		else if (m_Result == LevelResultEnum.TimeOut)
		{
			m_Text.text = "Countdown is over, go faster next time. TAT";
			m_Text.color = Color.red;

			if (LevelManager.GetInstance.RetryCount > 0)
			{
				m_Text.text += "\nRetryCount (" + LevelManager.GetInstance.RetryCount + ")";
			}

			m_RestartBtn.SetActive(true);
			m_ContinueBtn.SetActive(false);
		}
	}

	public void OnRestartBtnClick()
	{
		LevelManager.GetInstance.ResetLevel();

		Destroy(gameObject);
	}

	public void OnContinueBtnClick()
	{
		LevelManager.GetInstance.NextLevel();

		Destroy(gameObject);
	}
}
