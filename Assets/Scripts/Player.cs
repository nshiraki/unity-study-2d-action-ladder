using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
	public Text text;
	public int moveSpeed;
	// 一方通行の床
	public GameObject oneWay;
	Rigidbody2D rb;
	Animator animator;
	// はしごに触れているかどうか
	bool isLadder = false;

	// プレイヤーの状態
	enum PlayerStatus
	{
		Idle,
		Run,
		Climb
	}

	// プレイヤーの状態を格納
	PlayerStatus status = PlayerStatus.Idle;

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update()
	{
		text.text = "isLadder: " + isLadder + "\n" + "status: " + status + "\n";

		float horizontalKey = Input.GetAxisRaw("Horizontal");
		float verticalKey = Input.GetAxisRaw("Vertical");

		if (isLadder)
		{
			// はしごに触れている状態で[↑]または[↓]キー押下した場合
			if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
			{
				// はしご昇降中の状態にする
				status = PlayerStatus.Climb;
			}
		}

		if (status == PlayerStatus.Climb)
		{
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

			if (horizontalKey == 0)
			{
				status = PlayerStatus.Idle;
			}
			else
			{
				status = PlayerStatus.Run;
			}

			// スプライトの反転
			if (horizontalKey > 0)
			{
				transform.localScale = new Vector3(1, 1, 1);
			}
			if (horizontalKey < 0)
			{
				transform.localScale = new Vector3(-1, 1, 1);
			}
		}

		// 移動範囲を補正
		Vector3 pos = transform.position;
		pos = new Vector3(
			Mathf.Clamp(pos.x, -7.5f, 7.5f),
			pos.y,
			pos.z);
		transform.position = pos;

		// アニメーション
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
				animator.SetFloat("Speed", verticalKey == 0 ? 0.0f : 1.0f);
				break;
			default:
				break;
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
			status = PlayerStatus.Idle;
		}
	}
}
