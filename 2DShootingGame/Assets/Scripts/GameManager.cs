using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq.Expressions;


/// <summary>
/// ゲーム管理用クラス
/// </summary>
public class GameManager : MonoBehaviour
{
	// 唯一のインスタンス
	private static GameManager instance;
	[Tooltip("スコア"),ReadOnly] public float score;

	// 起動時に
	void Awake()
	{
		// インスタンスがあるか
		if (instance == null)
		{
			// なければ変数にインスタンスを格納
			instance = this;
			// このスクリプトがアタッチされているオブジェクトがシーンをまたいでも消えないように
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject); // すでに存在している場合は破棄
		}
	}

}
