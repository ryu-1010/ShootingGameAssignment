using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class Bomb : MonoBehaviour
{

	[SerializeField,Tooltip("発射向き")] Vector3 moveDir = Vector3.right;
	[SerializeField,Tooltip("速度")] float moveSpeed = 3;

	// 爆発までの経過時間
	[SerializeField,Tooltip("爆発するまでの時間")]private float bombTime = 1.5f;
	// 爆発までの現在時間
	private float bombCurrentTime;

	private void Start()
	{
		transform.up = moveDir;
	}

	void Update()
	{
		bombCurrentTime += Time.deltaTime;

		if (bombCurrentTime > bombTime)
		{
			Explode();
		}

		transform.position += moveDir * moveSpeed * Time.deltaTime;
	}

	// 爆発を起こす関数（例えばプレイヤーがBキーを押したときに呼ぶ）
	public void Explode()
	{
		// Tagが"EnemyBullet"のすべての弾を検索
		GameObject[] enemyBullets = GameObject.FindGameObjectsWithTag("EnemyBullet");

		// 全部削除
		foreach (GameObject bullet in enemyBullets)
		{
			// 弾のスコアを取得
			NormalBullet enemyBullet = bullet.GetComponent<NormalBullet>();
			if (enemyBullet != null)
			{
				// スコア加算処理（ScoreManagerなどに送る）
				GameManager.AddScore(enemyBullet.addScoreValue);
			}
			else
			{
				// ホーミング弾の場合
				HomingBullet enemyHomingBullet = bullet.GetComponent<HomingBullet>();
				if (enemyHomingBullet != null)
				{
					// スコア加算処理（ScoreManagerなどに送る）
					GameManager.AddScore(enemyHomingBullet.addScoreValue);
				}
			}

			bullet.SetActive(false);
		}

		// 爆弾自身を削除（使い捨ての場合）
		Destroy(gameObject);
	}
}
