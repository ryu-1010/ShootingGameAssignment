using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;


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


	[Header("移動関係")]
	[SerializeField, Tooltip("敵の体力")] private float moveSpeed = 2f;
	[SerializeField, Tooltip("敵の体力")] private float moveRange = 2f;
	private Vector3 startPos;
	private float direction = 1f;

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
		float offset = Mathf.Sin(Time.time * moveSpeed) * moveRange;
		transform.position = startPos + new Vector3(0, offset, 0);
	}


	// 放射弾攻撃
	void FireRadialBullet()
	{
		
	}


	// 通常弾、特殊弾の生成
	void CreateBullet()
	{
		// 親オブジェクト生成
		poolParent = new GameObject("EnemyBulletPool");

		// 放射状に発射する弾を作成
		float angleStart = -45f;             // 上方向
		float angleEnd = -135f;              // 下方向
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
}
