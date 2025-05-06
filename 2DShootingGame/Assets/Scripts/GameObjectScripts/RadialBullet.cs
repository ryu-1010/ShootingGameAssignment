using System.Collections.Generic;
using UnityEngine;

public class RadialBullet : MonoBehaviour
{
	[Header("弾の設定")]
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private int bulletCount = 10;
	[SerializeField] private float fireInterval = 1.0f;

	private float fireTimer = 0f;
	[SerializeField] private List<GameObject> bulletPool = new List<GameObject>();
	

	void Awake()
	{
		float angleStart = -45f;             // 上方向
		float angleEnd = -135f;              // 下方向
		float angleStep = (angleEnd - angleStart) / bulletCount;
		float angle = angleStart;

		// 弾を事前に生成してプールに格納
		for (int i = 0; i < bulletCount; i++)
		{
			GameObject bullet = Instantiate(bulletPrefab, this.transform);
			bullet.SetActive(false);
			bulletPool.Add(bullet);

			float dirX = Mathf.Cos(angle * Mathf.Deg2Rad);
			float dirY = Mathf.Sin(angle * Mathf.Deg2Rad);
			Vector2 dir = new Vector2(dirX, dirY).normalized;

			bullet.transform.position = transform.position;
			bullet.transform.rotation = Quaternion.identity;
			bullet.GetComponent<NormalBullet>().SetBulletDirection(dir);

			angle += angleStep;

		}
	}

	void Update()
	{
		fireTimer += Time.deltaTime;

		if (fireTimer >= fireInterval)
		{
			FireRadialBullets();
			fireTimer = 0f;
		}
	}

	private void FireRadialBullets()
	{
		float angleStart = -45f;             // 上方向
		float angleEnd = -135f;              // 下方向
		float angleStep = (angleEnd - angleStart) / bulletCount;
		float angle = angleStart;

		for (int i = 0; i < bulletCount; i++)
		{
			GameObject bullet = GetBulletFromPool();
			//if (bullet == null) return;

			//float dirX = Mathf.Cos(angle * Mathf.Deg2Rad);
			//float dirY = Mathf.Sin(angle * Mathf.Deg2Rad);
			//Vector2 dir = new Vector2(dirX, dirY).normalized;

			bullet.transform.position = transform.position;
			//bullet.transform.rotation = Quaternion.identity;
			bullet.SetActive(true);
			//bullet.GetComponent<NormalBullet>().SetBulletDirection(dir);

			//Debug.Log("dirX : " + dirX + " ,dirY : " + dirY);

			//angle += angleStep;
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
}
