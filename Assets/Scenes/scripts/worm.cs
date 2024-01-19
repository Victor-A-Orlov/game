using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class worm : entity
{
	[SerializeField] private int lives = 3;
	
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject == hero.Instance.gameObject)
		{
			hero.Instance.GetDamage(3);
			lives--;
			Debug.Log("у червяка " + lives);
		}
		
		if (lives < 1)
			Die();
	}
}
