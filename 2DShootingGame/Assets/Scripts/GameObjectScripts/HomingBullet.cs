using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBullet : MonoBehaviour
{
	[Header("弾のパラメーター")]
	[SerializeField, Tooltip("弾の移動速度")] private float bulletSpeed = 5f;
	[SerializeField, Tooltip("回転速度（追尾の鋭さ）")] private float rotateSpeed = 200f;
	[SerializeField, Tooltip("弾のダメージ")] private int bulletDamage = 1;
	[SerializeField, Tooltip("弾の生存時間")] private float bulletLifeTime = 2f;
	[SerializeField, Tooltip("弾の生存可能時間"), ReadOnly] private float bulletLifeTimeCount = 0f;

	[SerializeField] private float maxHomingAngle = 60f; // 追尾を続けられる最大角度

	private bool isHoming = true;

	private Rigidbody2D rb;
	[SerializeField]private Transform target;
	[SerializeField]private Vector2 direction;

	public void SetTarget(Transform _target)
	{
		target = _target;
	}

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		// 生存時間チェック
		bulletLifeTimeCount += Time.deltaTime;
		if (bulletLifeTimeCount > bulletLifeTime)
		{
			gameObject.SetActive(false);
			bulletLifeTimeCount = 0f;
			return;
		}

		if (target == null || !isHoming)
		{
			// ターゲットが無い、または追尾中止中は直進
			transform.position += transform.up * bulletSpeed * Time.deltaTime;
			return;
		}

		Vector2 toTarget = (target.position - transform.position).normalized;
		Vector2 currentDir = transform.up;

		// 現在の向きとターゲット方向との角度を取得
		float angle = Vector2.Angle(currentDir, toTarget);

		if (angle > maxHomingAngle)
		{
			// 角度が制限を超えたら追尾やめる
			isHoming = false;
			return;
		}

		// 方向更新して追尾
		transform.up = toTarget;
		transform.position += (Vector3)(toTarget * bulletSpeed * Time.deltaTime);
	}
	private void OnTriggerEnter2D(Collider2D _other)
	{
		if (this.gameObject.CompareTag("EnemyBullet") && _other.gameObject.CompareTag("Bullet"))
		{
			GameManager.AddScore();
			_other.gameObject.SetActive(false);
		}
		else if (_other.gameObject.CompareTag("Wall"))
		{
			//gameObject.SetActive(false);
		}
		else if (_other.gameObject.CompareTag("Player"))
		{
			gameObject.SetActive(false);
			_other.gameObject.GetComponent<PlayerBehavior>().TakeDamage();
		}
	}
}
