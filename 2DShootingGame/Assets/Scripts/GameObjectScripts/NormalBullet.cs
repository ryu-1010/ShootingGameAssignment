using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBullet : MonoBehaviour
{
	[Header("弾のパラメーター")]
	// 弾の移動速度
	[SerializeField, Tooltip("弾の移動速度")] private float bulletSpeed = 5f;
	// 弾の移動方向
	[SerializeField, Tooltip("弾の移動方向"), ReadOnly] private Vector2 bulletVec = Vector2.right;
	// 弾のダメージ
	[SerializeField, Tooltip("弾のダメージ")] private int bulletDamage = 1;
	// 弾の生存時間
	[SerializeField, Tooltip("弾の生存時間")] private float bulletLifeTime = 2f;
	// 弾の生存可能時間
	[SerializeField, Tooltip("弾の生存可能時間"), ReadOnly] private float bulletLifeTimeCount = 0f;


	// Start is called before the first frame update
	void Start()
	{

		SetBulletDirection(Vector2.right);

	}

	// Update is called once per frame
	void Update()
	{
		if (bulletLifeTimeCount < bulletLifeTime) bulletLifeTimeCount += Time.deltaTime;
		else
		{
			gameObject.SetActive(false); // 生存時間が経過したら弾を非アクティブにする
			bulletLifeTimeCount = 0f; // 生存時間カウントをリセット
		}

		// 弾の移動
		transform.Translate(bulletVec * bulletSpeed * Time.deltaTime);

	}

	// 弾の移動方向を設定するメソッド
	public void SetBulletDirection(Vector2 _direction)
	{
		// 弾の向きをセットする
		transform.up = bulletVec;
		// 弾の移動方向をセットする
		bulletVec = new Vector2(_direction.y, _direction.x);

	}



	private void OnTriggerEnter2D(Collider2D _other)
	{
		Debug.Log("弾が当たったオブジェクトのタグ: " + _other.gameObject.tag);
		// 弾が当たったオブジェクトのタグを確認
		if (_other.gameObject.CompareTag("Enemy"))
		{
			// 弾のダメージを敵に与える
			// _collision.gameObject.GetComponent<EnemyBehavior>().TakeDamage(bulletDamage);
			// 弾を非アクティブにする
			gameObject.SetActive(false);
		}
		if(_other.gameObject.CompareTag("Wall"))
		{
			// 壁に当たったら弾を非アクティブにする
			gameObject.SetActive(false);
		}
		else if(_other.gameObject.CompareTag("Player"))
		{
			// プレイヤーに当たったら弾を非アクティブにする
			gameObject.SetActive(false);
			// プレイヤーのライフを減らす
			_other.gameObject.GetComponent<PlayerBehavior>().TakeDamage();
		}
	}
}
