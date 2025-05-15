using UnityEngine;
using TMPro; // TextMeshPro を使う場合

public class ScoreUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI scoreText; // ← Text を使うなら Text に変更


	private void Start()
	{
		GameManager.RefreshUI();
	}
	void Update()
	{
		scoreText.text = "Score: " + GameManager.score.ToString();
	}
}
