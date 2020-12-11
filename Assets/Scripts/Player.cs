using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
	public Text text;
	public int moveSpeed;
	// 一方通行の床
	public GameObject oneWay;
	// 接地しているかどうか
	public bool isGround = false;

	// プレイヤーの状態を格納
	PlayerStatus status = PlayerStatus.Idle;
	// 前回のプレイヤーの状態を格納
	PlayerStatus preStatus;
	Rigidbody2D rb;
	Animator animator;
	// はしごに触れているかどうか
	bool isLadder = false;

	// プレイヤーの状態
	enum PlayerStatus
	{
		Idle,
		Run,
		Climb,
		Jump,
	}

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update()
	{
		text.text = "isLadder: " + isLadder + "\n"
			+ "status: " + status + "\n"
			+ "isGround: " + isGround + "\n";

		float horizontalKey = Input.GetAxisRaw("Horizontal");
		float verticalKey = Input.GetAxisRaw("Vertical");

		if (isLadder)
		{
			// はしごに触れている状態で[↑]または[↓]キー押下した場合
			if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
			{
				// はしご昇降中の状態にする
				ChangePlayerStatus(PlayerStatus.Climb);
			}
		}

		if (status == PlayerStatus.Climb)
		{
			// [↑]または[↓]キー押下の場合はアニメーションを再生する。押下してない場合ばアニメーションを一時停止させる。
			animator.SetFloat("Speed", verticalKey == 0 ? 0.0f : 1.0f);

			// ハシゴに触れていて昇降中の場合
			rb.velocity = new Vector2(horizontalKey * moveSpeed, verticalKey * moveSpeed);
			rb.gravityScale = 0;
			// 一方通行の床のColliderを無効にする
			oneWay.GetComponent<BoxCollider2D>().enabled = false;
		}
		else
		{
			// 昇降中でない場合
			rb.velocity = new Vector2(horizontalKey * moveSpeed, rb.velocity.y);
			rb.gravityScale = 4f;
			// 一方通行の床のColliderを有効にする
			oneWay.GetComponent<BoxCollider2D>().enabled = true;

			// 接地している場合
			if (isGround)
			{
				ChangePlayerStatus(horizontalKey == 0 ? PlayerStatus.Idle : PlayerStatus.Run);
				// ジャンプ
				float jumpPower = 10.0f;
				if (Input.GetKeyDown(KeyCode.Space))
				{
					rb.velocity = new Vector2(rb.velocity.x, jumpPower);
				}
			}
			// 空中に居る場合
			else
			{
				if (rb.velocity.y > 0)
				{
					ChangePlayerStatus(PlayerStatus.Jump);
				}
			}

			// スプライトの反転
			float x = horizontalKey >= 0 ? 1 : -1;
			transform.localScale = new Vector3(x, 1, 1);
		}

		// 移動範囲を補正
		Vector3 pos = transform.position;
		pos = new Vector3(
			Mathf.Clamp(pos.x, -7.5f, 7.5f),
			pos.y,
			pos.z);
		transform.position = pos;
	}

	void ChangePlayerStatus(PlayerStatus newStatus)
	{
		if(newStatus != status)
		{
			preStatus = status;
			status = newStatus;

			// 状態に応じてアニメーションを更新
			switch (status)
			{
				case PlayerStatus.Idle:
					animator.SetTrigger("Idle");
					break;
				case PlayerStatus.Run:
					animator.SetTrigger("Run");
					break;
				case PlayerStatus.Climb:
					// ハシゴ昇降中
					animator.SetTrigger("Climb");
					break;
				case PlayerStatus.Jump:
					animator.SetTrigger("Jump");
					break;
				default:
					break;
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Ladder"))
		{
			isLadder = true;
		}
	}
	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Ladder"))
		{
			isLadder = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Ladder"))
		{
			isLadder = false;
			ChangePlayerStatus(PlayerStatus.Idle);
			//status = PlayerStatus.Idle;
		}
	}
}
