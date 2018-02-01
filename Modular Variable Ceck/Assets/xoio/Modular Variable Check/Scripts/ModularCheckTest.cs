using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularCheckTest : ModularCheckBase {

	public bool test = true;
	

	[ContextMenu("test")]
	public override  bool Check()
	{
		return test;
	}

	public override void Init(ScriptableObject so)
	{

	}
}
