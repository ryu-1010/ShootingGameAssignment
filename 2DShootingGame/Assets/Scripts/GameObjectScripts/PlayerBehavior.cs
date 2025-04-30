using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class PlayerBehavior : MonoBehaviour
{
	[Header("プレイヤーのパラメーター")]
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
	
	void Awake()
	{
		// 無敵時間のカウントの初期化
		invincibleTimeCount = invincibleTime;

		// 弾オブジェクトをまとめる親オブジェクトを作成
		poolParent = new GameObject("PlayerBulletParent");

		// 弾を作成
		for (int i = 0; i < bulletMaxNum; i++)
		{
			GameObject bullet = Instantiate(bulletPrefab, poolParent.transform);
			bullet.SetActive(false);
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
			if (invincibleTime <= 0f)
			{
				isInvincible = false; // 無敵状態を解除
				invincibleTimeCount = invincibleTime; // 無敵時間をリセット
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

	// 弾発射の入力
	public void OnFire(InputAction.CallbackContext _context)
	{
		// オブジェクトプールから弾を取得
		foreach (GameObject bullet in bulletPool)
		{
			// 弾が使用中かどうか確認
			if (!bullet.activeSelf)
			{
				// 使用していない場合、玉を発射
				bullet.transform.position = transform.position;
				bullet.SetActive(true);
				break;
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
		}
		else { Debug.Log("ゲームオーバー"); }
	}
}
