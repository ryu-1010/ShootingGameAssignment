using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class PlayerBehavior : MonoBehaviour
{
	[Header("プレイヤーのパラメーター")]
	// UIを管理するスクリプト
	[SerializeField, Tooltip("プレイヤーの残機を管理するスクリプト")] PlayerHeart playerHeart;
	// ライフ(残機)
	[SerializeField,Tooltip("ライフの数")] private int life = 3;
	// 無敵状態か(true:無敵、false:通常)
	[SerializeField, Tooltip("無敵状態"), ReadOnly] private bool isInvincible = false;
	// 無敵状態の時間
	[SerializeField, Tooltip("無敵時間")] private float invincibleTime = 2f;
	// 無敵時間のカウント
	private float invincibleTimeCount = 0f;
	// 移動速度
	[SerializeField, Tooltip("移動速度")] private float moveSpeed = 1f;
	// 移動方向
	[SerializeField, Tooltip("移動方向"), ReadOnly] private Vector2 moveVec = Vector2.zero;

	[Header("弾関係")]
	// 弾のプレハブ
	[SerializeField] private GameObject bulletPrefab;
	// 弾のオブジェクトプールの親オブジェクト
	[SerializeField, ReadOnly] private GameObject poolParent;
	// 弾管理変数 (オブジェクトプール)
	[SerializeField,ReadOnly] private List<GameObject> bulletPool = new List<GameObject>();
	// 弾の最大数
	[SerializeField] private int bulletMaxNum = 30;
	// 弾の発射間隔
	[SerializeField,Tooltip("弾のインターバル")] private float fireInterval = 0.2f; // 弾の発射間隔
	// 弾の発射位置のオフセット
	[SerializeField, Tooltip("弾のオフセット")] private Vector3 fireOffsetPos = new Vector3(0.5f, 0, 0);

	private Coroutine fireCoroutine;
	void Awake()
	{
		// コンポーネントを取得
		playerHeart = GetComponent<PlayerHeart>();

		// 無敵時間のカウントの初期化
		invincibleTimeCount = invincibleTime;

		// 弾オブジェクトをまとめる親オブジェクトを作成
		poolParent = new GameObject("PlayerBulletParent");

		// 弾を作成
		for (int i = 0; i < bulletMaxNum; i++)
		{
			GameObject bullet = Instantiate(bulletPrefab, poolParent.transform);
			bullet.SetActive(false);
			bullet.GetComponent<NormalBullet>().SetBulletDirection(Vector2.right);
			bulletPool.Add(bullet);
		}
	}

	// Start is called before the first frame update
	void Start()
    {


	}

    // Update is called once per frame
    void Update()
    {
		Move();

		// 無敵時間のカウント
		if (isInvincible)
		{
			invincibleTimeCount -= Time.deltaTime;
			if (invincibleTimeCount <= 0f)
			{
				isInvincible = false; // 無敵状態を解除
			}
		}

		if (Input.GetKeyDown(KeyCode.V))
		{
			TakeDamage();
		}
	}

	// 移動の入力
	public void OnMove(InputAction.CallbackContext _context)
	{           
		// 移動方向を取得
		if (_context.started)
		{
			// 押し始めた瞬間
			moveVec = _context.ReadValue<Vector2>();
		}
		else if (_context.performed)
		{
			// 押しっぱなし更新
			moveVec = _context.ReadValue<Vector2>();
		}
		else if (_context.canceled)
		{
			// 離した瞬間
			moveVec = Vector2.zero;
		}
	}

	private void Move()
	{
		// 移動方向の調整
		Vector2 tmpVec = new Vector2(-moveVec.y, moveVec.x);
		// 移動
		transform.Translate(tmpVec * moveSpeed * Time.deltaTime);

		// プレイヤーの位置を画面内に制限
		Vector3 pos = transform.position;
		Vector3 clampValue = transform.localScale / 2f; // プレイヤーの大きさの半分を取得

		// カメラのビューポートの0～1をワールド座標に変換
		Vector3 min = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane)) + clampValue;
		Vector3 max = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane)) - clampValue;

		// プレイヤーの位置を画面内に制限
		pos.x = Mathf.Clamp(pos.x, min.x, max.x);
		pos.y = Mathf.Clamp(pos.y, min.y, max.y);

		// プレイヤーの位置を更新
		transform.position = pos;
	}

	// 入力イベント（ボタンを押した・離した）に応じて処理する
	public void OnFire(InputAction.CallbackContext _context)
	{
		// ボタンが押された瞬間（初回のみ）
		if (_context.started)
		{
			// 発射処理を開始（Coroutineをスタート）
			fireCoroutine = StartCoroutine(FireContinuously());
		}
		// ボタンが離された瞬間
		else if (_context.canceled)
		{
			// 発射処理を停止（Coroutineを止める）
			if (fireCoroutine != null)
			{
				StopCoroutine(fireCoroutine);
				fireCoroutine = null;
			}
		}
	}

	// 一定間隔で弾を発射し続けるCoroutine
	private IEnumerator FireContinuously()
	{
		while (true)
		{
			FireBullet(); // 弾を1発発射
			yield return new WaitForSeconds(fireInterval); // 次の発射まで待機
		}
	}

	// 弾をプールから取得して発射
	private void FireBullet()
	{
		foreach (GameObject bullet in bulletPool)
		{
			// まだ使われていない弾を探す
			if (!bullet.activeSelf)
			{
				// 弾の位置をプレイヤーの位置にセット
				bullet.transform.position = transform.position + fireOffsetPos;

				// 弾をアクティブにして発射
				bullet.SetActive(true);
				break; // 1発だけ発射したら終了
			}
		}
	}
	// ダメージ処理
	public void TakeDamage()
	{
		if(isInvincible) return; // 無敵状態ならダメージを受けない

		if (life > 0) { 
			// ライフを減らす
			life--; 
			// 無敵状態に移行
			isInvincible = true;
			// タイマー
			invincibleTimeCount = invincibleTime;
			// UIを更新
			playerHeart.SetLives(life);
		}
		else { Debug.Log("ゲームオーバー"); }
	}
}
