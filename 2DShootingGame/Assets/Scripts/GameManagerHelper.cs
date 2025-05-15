using UnityEngine;

/// <summary>
/// staticクラスでCoroutineを使うためのヘルパー（自動生成され、DontDestroy）
/// </summary>
public class GameManagerHelper : MonoBehaviour
{
	private static GameManagerHelper instance;
	public static GameManagerHelper Instance
	{
		get
		{
			if (instance == null)
			{
				var obj = new GameObject("GameManagerHelper");
				instance = obj.AddComponent<GameManagerHelper>();
				Object.DontDestroyOnLoad(obj);
			}
			return instance;
		}
	}
}
