using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHeart : MonoBehaviour
{
	public List<Image> hearts;

	public void SetLives(int _life)
	{
		for (int i = 0; i < hearts.Count; i++)
		{
			hearts[i].enabled = (i < _life);
		}
	}
}
