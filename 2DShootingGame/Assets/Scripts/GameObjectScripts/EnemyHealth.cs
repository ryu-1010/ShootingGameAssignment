using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
	// 最大体力
	[SerializeField] public int maxHealth;
	// 現在の体力
	[SerializeField,ReadOnly] public int currentHealth;
	// UIのスライダー
	//public Slider healthSlider;
	// 体力バーのプレハブ
	//public GameObject hpBarPrefab; 
	// 体力バーのトランスフォーム
	private Transform hpBarTransform;

	public bool isDie;

	// Start is called before the first frame update
	void Start()
    {
		currentHealth = maxHealth;
		//healthSlider.value = currentHealth;
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void TakeDamage(int _damage)
	{
		currentHealth -= _damage;
		//healthSlider.value = currentHealth;
		currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

		// HPが0以下になっていないか確認
		if(currentHealth <= 0)
		{
			isDie = true;
		}
	}
}
