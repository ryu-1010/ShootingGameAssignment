using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ButtonSceneChange : MonoBehaviour
{
	[SerializeField] private string sceneName = "TitleScene"; // 遷移先のシーン名

	private bool hasTransitioned = false;


	public void OnButtonPressed(InputAction.CallbackContext _context)
	{
		if (hasTransitioned) return;

		hasTransitioned = true; // 多重呼び出し防止

		// 任意の処理：ここではシーン遷移
		SceneManager.LoadScene(sceneName);
	}

	public void OnGameEnd(InputAction.CallbackContext _context)
	{
		GameManager.NotifyInputReceived();
	}
}
