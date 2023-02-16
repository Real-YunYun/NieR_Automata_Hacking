using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constant : Enemy
{
	[Header("Constant Parameters")]
	public int random;

	protected override void Start()
	{
		base.Start();
		
		AddThread<PointerOrbital>();
	}
}
