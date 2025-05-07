using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyLeftBody : MonoBehaviour
{
	// 向く方角、弾のターゲット
	[SerializeField,Tooltip("ターゲット"),ReadOnly] private Transform target;
	// 弾のプレハブ
	[SerializeField, Tooltip("弾のプレハブ")] private GameObject bulletPrefab;
	// 弾のオブジェクトプールのリスト
	private List<GameObject> bulletPool = new List<GameObject>();
	// 弾数
	[SerializeField, Tooltip("弾の数")] private int bulletMaxNum = 30;
	// 弾の発射位置の調整値
	[SerializeField, Tooltip("弾の発射位置のオフセット")] Vector3 fireOffsetPos = Vector3.zero;


	// 弾のインターバル
	[SerializeField, Tooltip("弾の発射インターバル")] private float fireInterval = 5;
	// 弾のタイマー
	private float fireTimer = 3;
	// 一度に発射する弾の数
	[SerializeField,Tooltip("一度の発射数")] private int bulletsPerShot = 5;
	// 弾と弾の間の遅延
	[SerializeField,Tooltip("弾の間隔")] private float delayBetweenBullets = 0.5f; 

	void Start()
    {
		if(target == null) target = GameObject.Find("Player").transform;

		// 弾の生成
		CreateBullet();


	}

    // Update is called once per frame
    void Update()
    {
		// 常にプレイヤーのほうを向く
		Vector2 toTarget = (target.position - transform.position).normalized;
		transform.up = toTarget;

		fireTimer += Time.deltaTime;
		if (fireTimer >= fireInterval)
		{
			StartCoroutine(FireBulletsWithDelay(bulletsPerShot));
			fireTimer = 0f;
		}
	}

	void CreateBullet()
	{
		// 常にプレイヤーのほうを向く
		Vector2 toTarget = (target.position - transform.position).normalized;

		GameObject poolParent;
		poolParent = GameObject.Find("EnemyBulletPool");
		for (int i = 0; i < bulletMaxNum; i++)
		{
			GameObject bullet = Instantiate(bulletPrefab, poolParent.transform);
			bullet.SetActive(false);
			bulletPool.Add(bullet);
			bullet.GetComponent<NormalBullet>().SetBulletDirection(toTarget);
		}
	}

	IEnumerator FireBulletsWithDelay(int _count)
	{
		int fired = 0;
		Vector2 toTarget = (target.position - transform.position).normalized;

		Debug.Log(toTarget);

		foreach (GameObject bullet in bulletPool)
		{
			if (!bullet.activeSelf)
			{
				bullet.transform.position = transform.position;
				bullet.GetComponent<NormalBullet>().SetBulletDirection(toTarget);
				bullet.SetActive(true);
				fired++;

				if (fired >= _count)
					break;

				yield return new WaitForSeconds(delayBetweenBullets); // 弾の間隔
			}
		}
	}
}
