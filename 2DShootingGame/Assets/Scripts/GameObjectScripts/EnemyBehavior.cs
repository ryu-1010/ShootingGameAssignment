using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;


public class EnemyBehavior : MonoBehaviour
{
	[Header("敵のパラメーター")]
	[NamedArrayAttribute(new string[] { "MainBody HP", "RightBody HP", "LeftBody HP"})]
	[SerializeField,Tooltip("敵の体力")] private float[] hitPoint = new float[3];
	private float[] maxHitPoint = new float[3];

	// 弾のオブジェクトプールの親オブジェ
	[SerializeField, ReadOnly] private GameObject poolParent;

	[NamedArrayAttribute(new string[] { "NormalBullet", "HomingBullet" })]
	[SerializeField,Tooltip("弾のプレハブ")] private GameObject[] bulletPrefabs = new GameObject[2];
	// 弾のオブジェクトプール
	[SerializeField] List<List<GameObject>> bulletPool = new List<List<GameObject>>();

	[SerializeField, Tooltip("通常弾の最大数")] int normalBulletMaxNum = 50;
	[SerializeField, Tooltip("特殊弾の最大数")] int uniqueBulletMaxNum = 10;

	// Start is called before the first frame update
	void Awake()
    {
		// 最大値を記録
		maxHitPoint = hitPoint;

		// 弾の生成
		CreateBullets();


	}

	// Update is called once per frame
	void Update()
    {
        
    }

	// 通常弾攻撃
	void FireNormalBullet()
	{
		
	}

	// 放射弾攻撃
	void FireRadialBullet()
	{
		
	}

	// ホーミング弾攻撃
	void FireHomingBullet()
	{

	}

	// 通常弾、特殊弾の生成
	void CreateBullets()
	{
		// 親オブジェクト生成
		poolParent = new GameObject("EnemyBulletPool");

		// 通常弾を作成
		List<GameObject> normalBulletPool = new List<GameObject>();
		for (int i = 0; i < normalBulletMaxNum; i++)
		{
			GameObject bullet = Instantiate(bulletPrefabs[0], poolParent.transform);
			bullet.SetActive(false);
			normalBulletPool.Add(bullet);
		}
		bulletPool.Add(normalBulletPool);


		// 放射状に発射する弾を作成
		List<GameObject> radialBulletPool = new List<GameObject>();
		float angleStart = -45f;             // 上方向
		float angleEnd = -135f;              // 下方向
		float angleStep = (angleEnd - angleStart) / uniqueBulletMaxNum;
		float angle = angleStart;

		// 弾を事前に生成してプールに格納
		for (int i = 0; i < uniqueBulletMaxNum; i++)
		{
			GameObject bullet = Instantiate(bulletPrefabs[0], poolParent.transform);
			bullet.SetActive(false);
			radialBulletPool.Add(bullet);
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
		bulletPool.Add(radialBulletPool);


		// ホーミング弾の生成
		List<GameObject> homingBulletPool = new List<GameObject>();

		Transform target = GameObject.Find("Player").transform;
		for (int i = 0; i < uniqueBulletMaxNum; i++)
		{
			GameObject bullet = Instantiate(bulletPrefabs[1], poolParent.transform);
			bullet.SetActive(false);
			bullet.GetComponent<HomingBullet>().SetTarget(target);
			normalBulletPool.Add(bullet);

		}
		bulletPool.Add(homingBulletPool);

	}
}
