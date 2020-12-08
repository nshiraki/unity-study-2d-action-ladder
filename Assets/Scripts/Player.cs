using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
	public Text text;
	public Tilemap tilemap;
	public int moveSpeed;
	// 一方通行の床
	public GameObject oneWay;
	Rigidbody2D rb;
	Animator animator;
	// はしごに触れているかどうか
	bool isLadder = false;
	// はしごを昇降中かどうか
	bool isClimb = false;

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update()
	{
		text.text = "isLadder: " + isLadder + "\n" + "isClimb: " + isClimb + "\n";

		float horizontalKey = Input.GetAxisRaw("Horizontal");
		float verticalKey = Input.GetAxisRaw("Vertical");
		// アニメーション
		if (horizontalKey == 0)
		{
			animator.SetBool("isRun", false);
		}
		else
		{
			animator.SetBool("isRun", true);
		}

		if (horizontalKey > 0)
		{
			transform.localScale = new Vector3(1, 1, 1);
		}
		if (horizontalKey < 0)
		{
			transform.localScale = new Vector3(-1, 1, 1);
		}

		if (isLadder)
		{
			// はしごに触れている状態で[↑]または[↓]キー押下した場合
			if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
			{
				// はしご昇降中のフラグを立てる
				isClimb = true;
			}
		}

		if (isClimb)
		{
			rb.velocity = new Vector2(horizontalKey * moveSpeed, verticalKey * moveSpeed);
			rb.gravityScale = 0;
			oneWay.GetComponent<BoxCollider2D>().enabled = false;
		}
		else
		{
			rb.velocity = new Vector2(horizontalKey * moveSpeed, rb.velocity.y);
			rb.gravityScale = 4f;
			oneWay.GetComponent<BoxCollider2D>().enabled = true;
		}

		// はしごに触れていてかつ昇降中の場合
		// 一方通行の床のColliderを無効にする
		if (isLadder && isClimb)
		{
			oneWay.GetComponent<BoxCollider2D>().enabled = false;
		}
		else
		{
			oneWay.GetComponent<BoxCollider2D>().enabled = true;
		}

		// 移動範囲を補正
		Vector3 pos = transform.position;
		pos = new Vector3(
			Mathf.Clamp(pos.x, -7.5f, 7.5f),
			pos.y,
			pos.z);
		transform.position = pos;
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
			isClimb = false;
		}
	}
}
