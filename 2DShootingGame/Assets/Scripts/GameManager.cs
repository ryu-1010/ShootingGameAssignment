using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// ゲームの状態管理やスコア管理、シーン遷移を司る静的クラス。
/// </summary>
public static class GameManager
{
	// プレイヤーのスコア
	public static int score;

	// 遷移先のシーン名
	private static string pendingSceneName = null;

	// 表示するメッセージ（Game Overなど）
	private static string pendingMessage = null;

	// メッセージ表示に使用するUIテキスト（TextMeshPro）
	private static TextMeshProUGUI messageText = null;

	// 入力待ち状態かどうか
	private static bool isWaitingForInput = false;

	// スコアを加算する
	public static void AddScore(int _score)
	{
		score += _score;
	}

	// ゲームクリア時に呼び出す。メッセージを表示し、任意の入力でシーンを切り替える。
	public static void GameClear()
	{
		ShowMessageAndWaitForInput("Game Clear!!\nPush Space or X button", "TitleScene");
	}

	// ゲームオーバー時に呼び出す。メッセージを表示し、任意の入力でシーンを切り替える。
	public static void GameOver()
	{
		ShowMessageAndWaitForInput("Game Over\nPush Space or X button", "TitleScene");
	}

	// メッセージを表示し、任意の入力があれば指定のシーンへ遷移する処理を開始。
	private static void ShowMessageAndWaitForInput(string _message, string _sceneName)
	{
		pendingMessage = _message;
		pendingSceneName = _sceneName;

		ShowMessage(_message);

		// 任意入力待ち → シーン遷移コルーチン開始
		GameManagerHelper.Instance.StartCoroutine(WaitForAnyInputAndChangeScene());
	}

	// UIにメッセージを表示する
	private static void ShowMessage(string _message)
	{
		// TextMeshPro オブジェクトがまだ取得されていなければ探す
		if (messageText == null)
		{
			GameObject textObj = GameObject.Find("MessageText");
			if (textObj != null)
			{
				messageText = textObj.GetComponent<TextMeshProUGUI>();
			}
			else
			{
				Debug.LogWarning("MessageText が見つかりませんでした。");
				return;
			}
		}

		// メッセージの表示
		if (messageText != null)
		{
			messageText.text = _message;
			messageText.gameObject.SetActive(true);
		}
	}

	// 任意のボタン入力があるまで待機し、その後シーンを変更する。
	private static IEnumerator WaitForAnyInputAndChangeScene()
	{
		isWaitingForInput = true;

		while (isWaitingForInput)
		{
			yield return null;
		}

		if (!string.IsNullOrEmpty(pendingSceneName))
		{
			// 指定されたシーンへ遷移
			SceneManager.LoadScene(pendingSceneName);

			// データを初期化
			pendingSceneName = null;
			pendingMessage = null;
			messageText = null;
		}
	}

	// 入力があったことを通知する。外部から呼び出される。
	public static void NotifyInputReceived()
	{
		if (isWaitingForInput)
		{
			isWaitingForInput = false;
		}
	}

	// 指定されたシーンに即時遷移する

	public static void SceneChange(string _sceneName)
	{
		SceneManager.LoadScene(_sceneName);
	}

	// シーン遷移後にMessageTextを再取得する（シーン変更で失われるため）
	public static void RefreshUI()
	{
		GameObject textObj = GameObject.Find("MessageText");
		if (textObj != null)
		{
			messageText = textObj.GetComponent<TextMeshProUGUI>();
		}
	}


}
