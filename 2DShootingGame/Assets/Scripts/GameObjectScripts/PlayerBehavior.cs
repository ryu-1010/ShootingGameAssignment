using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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
	[SerializeField, Tooltip("移動速度")] private float currentMoveSpeed = 5f;
	[SerializeField, Tooltip("移動速度")] private float slowMoveSpeed = 3f;
	[SerializeField, Tooltip("移動速度")] private float moveSpeed = 5f;
	// 移動方向
	[SerializeField, Tooltip("移動方向"), ReadOnly] private Vector2 moveVec = Vector2.zero;

	// 死んだか
	private bool isDied = false;

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

	[Header("爆弾関係")]
	[SerializeField, Tooltip("爆弾のプレハブ")] GameObject bombPrefab;
	[SerializeField,Tooltip("使用回数"),ReadOnly] const int bombMaxNum = 3;
	[SerializeField,Tooltip("使用したか"),ReadOnly] bool[] bombUsed = new bool[bombMaxNum];

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

		if(life <= 0 && !isDied)
		{
			isDied = true;
			GameManager.GameOver();
		}

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

	// 低速移動
	public void OnSlowMoveStarted(InputAction.CallbackContext _context)
	{

		if (_context.performed)
		{
			// 押しっぱなし更新
			currentMoveSpeed = slowMoveSpeed; // 低速移動の速度に切り替え
		}
		else if (_context.canceled)
		{
			// 離した瞬間
			currentMoveSpeed = moveSpeed; // 低速移動の速度に切り替え
		}
		
	}

	private void Move()
	{
		// 移動
		transform.position += (Vector3)moveVec * currentMoveSpeed * Time.deltaTime;

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

	// 入力イベント : 敵の弾をすべて破壊する爆弾の発射
	public void OnBombFire(InputAction.CallbackContext _context)
	{
		// ボタンが押された瞬間にのみ処理する
		if (_context.performed)
		{
			for (int i = 0; i < bombMaxNum; i++)
			{
				if (!bombUsed[i])
				{
					GameObject bomb = Instantiate(bombPrefab);
					bomb.transform.position = transform.position;
					bombUsed[i] = true;
					break;
				}
			}
		}
	}

	// 入力イベント（ボタンを押した・離した）に応じて処理する
	public void OnFire(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			// コルーチンが二重に始まらないようにチェック
			if (fireCoroutine == null)
			{
				fireCoroutine = StartCoroutine(FireContinuously());
			}
		}
		else if (context.canceled)
		{
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

	}
}
