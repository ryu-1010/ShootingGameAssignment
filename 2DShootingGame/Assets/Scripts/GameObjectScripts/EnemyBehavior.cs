using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EnemyState { 
	State_Normal,
	State_Alert,
	State_Berserk,
}


public class EnemyBehavior : MonoBehaviour
{
	
	[Header("敵のパラメーター")]
	// 弾のオブジェクトプールの親オブジェ
	[SerializeField,ReadOnly] private GameObject poolParent;
	// 弾のプレハブ
	[SerializeField,Tooltip("弾のプレハブ")] private GameObject bulletPrefab;
	// 弾のオブジェクトプール
	[SerializeField] List<GameObject> bulletPool = new List<GameObject>();
	// 弾数
	[SerializeField, Tooltip("放射弾の最大数")] int radialBulletMaxNum = 10;
	// 敵のステート
	[SerializeField,Tooltip("敵の状態"),ReadOnly] private EnemyState enemyState = EnemyState.State_Normal;
	// 攻撃するか
	[SerializeField, Tooltip("攻撃するか")] private bool isAttack = false;


	[SerializeField,Tooltip("ボスのサイドボディ(0:右,1:左)")] private GameObject[] sideBodies = new GameObject[2];


	[Header("移動関係")]
	[SerializeField, Tooltip("敵の体力")] private float moveSpeed = 2f;
	[SerializeField, Tooltip("敵の体力")] private float moveRange = 2f;
	private Vector3 startPos;

	// 発射時間
	private float fireTimer = 0f;
	// 発射間隔
	[SerializeField, Tooltip("発射間隔")] private float fireInterval = 5f;

	[SerializeField,Tooltip("敵のHP管理SC")] private EnemyHealth[] healths = new EnemyHealth[3];
	[SerializeField, Tooltip("敵が生きているか")] private bool isAlive = true;


	// Start is called before the first frame update
	void Awake()
    {
		// 初期位置を記録
		startPos = transform.position;

		// 弾の生成
		CreateBullet();


	}

	// Update is called once per frame
	void Update()
    {
		SinMove();

		fireTimer += Time.deltaTime;

		if (fireTimer >= fireInterval && isAttack)
		{
			FireRadialBullet();
			fireTimer = 0f;
		}

		StateChange();

	}


	// 放射弾攻撃
	void FireRadialBullet()
	{
		for (int i = 0; i < radialBulletMaxNum; i++)
		{
			GameObject bullet = GetBulletFromPool();

			bullet.transform.position = transform.position;
			bullet.SetActive(true);

		}
	}


	// 通常弾、特殊弾の生成
	void CreateBullet()
	{
		// 親オブジェクト生成
		poolParent = new GameObject("EnemyBulletPool");

		// 放射状に発射する弾を作成
		float angleStart = 135;             // 上方向
		float angleEnd = 225f;              // 下方向
		float angleStep = (angleEnd - angleStart) / radialBulletMaxNum;
		float angle = angleStart;

		// 弾を事前に生成してプールに格納
		for (int i = 0; i < radialBulletMaxNum; i++)
		{
			GameObject bullet = Instantiate(bulletPrefab, poolParent.transform);
			bullet.SetActive(false);
			bulletPool.Add(bullet);
			// 角度を計算
			float dirX = Mathf.Cos(angle * Mathf.Deg2Rad);
			float dirY = Mathf.Sin(angle * Mathf.Deg2Rad);
			Vector2 dir = new Vector2(dirX, dirY).normalized;
			// 座標、回転をセット
			bullet.transform.position = transform.position;
			bullet.transform.rotation = Quaternion.identity;
			bullet.GetComponent<NormalBullet>().SetBulletDirection(dir);

			angle += angleStep;

		}

	}
	private GameObject GetBulletFromPool()
	{
		foreach (var bullet in bulletPool)
		{
			if (!bullet.activeInHierarchy)
				return bullet;
		}

		// プールに空きがない場合（必要なら拡張可）
		return null;
	}

	void SinMove()
	{
		float offset = Mathf.Sin(Time.time * moveSpeed) * moveRange;
		transform.position = startPos + new Vector3(0, offset, 0);

	}


	// ステートが変わるか確認
	void StateChange()
	{
		// 最大HPの合計と現在のHPを取得
		int totalHP = 0;
		int totalCurrentHP = 0;
		for(int i = 0; i < healths.Length;i++)
		{
			totalHP += healths[i].maxHealth;
			totalCurrentHP += healths[i].currentHealth;
		}


		// 最大HPの2/3なら
		if (totalHP / 3  * 2 > totalCurrentHP && enemyState != EnemyState.State_Alert)
		{
			sideBodies[0].GetComponent<EnemySideBody>().isAttack = true;
			enemyState = EnemyState.State_Alert;
		}
		// 最大HPの1/3なら
		else if (totalHP / 3 > totalCurrentHP && enemyState != EnemyState.State_Berserk)
		{
			isAttack = true;
			enemyState = EnemyState.State_Berserk;
		}


		if (healths[0].isDie && healths[1].isDie && healths[2].isDie)
		{
			isAlive = false;
		}


		if (!isAlive)
		{
			this.gameObject.SetActive(false);
			GameManager.GameClear();

			//Debug.Log("Enemy Die");
		}

	}
}
