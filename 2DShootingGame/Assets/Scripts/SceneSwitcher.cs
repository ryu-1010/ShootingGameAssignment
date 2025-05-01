using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
	// シーンの切り替えを行うメソッド
	public void SwitchScene(string _sceneName)
	{
		SceneManager.LoadScene(_sceneName);
	}

	// 例: ボタンでシーンを切り替える
	public void OnButtonClick(string _sceneName)
	{
		SwitchScene(_sceneName); 
	}
}
