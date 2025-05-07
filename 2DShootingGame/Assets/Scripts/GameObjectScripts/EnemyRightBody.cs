using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRightBody : MonoBehaviour
{
	[SerializeField] private Transform target;

	// Start is called before the first frame update
	void Awake()
	{
		target = GameObject.Find("Player").transform;

	}

	// Update is called once per frame
	void Update()
	{
		// 常にプレイヤーのほうを向く
		Vector2 toTarget = (target.position - transform.position).normalized;
		transform.up = toTarget;
	}
}
