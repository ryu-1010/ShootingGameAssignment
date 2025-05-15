using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum BulletType { 
	Normal,
	Homing,

	None,
}

public class EnemySideBody : MonoBehaviour
{
	// 向く方角、弾のターゲット
	[SerializeField,Tooltip("ターゲット"),ReadOnly] private Transform target;
	// 弾のプレハブ
	[SerializeField, Tooltip("弾のプレハブ")] private GameObject bulletPrefab;
	// アタッチした弾の種類
	[SerializeField, Tooltip("弾の種類")] private BulletType bulletType;
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

	[SerializeField, Tooltip("攻撃をするか")] public bool isAttack = false;

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
			if (bulletType == BulletType.Normal) bullet.GetComponent<NormalBullet>().SetBulletDirection(toTarget);
			else if (bulletType == BulletType.Homing) bullet.GetComponent<HomingBullet>().SetTarget(target);
		}
	}

	IEnumerator FireBulletsWithDelay(int _count)
	{
		int fired = 0;

		// 攻撃可能でなければここで処理を切る
		if (!isAttack) yield break;
;

		foreach (GameObject bullet in bulletPool)
		{
			if (!bullet.activeSelf)
			{
				Vector3 toTarget = (target.position - transform.position).normalized;

				bullet.transform.position = transform.position + Vector3.Scale( fireOffsetPos, toTarget );
				
				if(bulletType == BulletType.Normal) bullet.GetComponent<NormalBullet>().SetBulletDirection(toTarget);
				else if(bulletType == BulletType.Homing) bullet.GetComponent<HomingBullet>().SetTarget(target);
				
				bullet.SetActive(true);
				fired++;

				if (fired >= _count)
					break;

				yield return new WaitForSeconds(delayBetweenBullets); // 弾の間隔
			}
		}
	}
}
