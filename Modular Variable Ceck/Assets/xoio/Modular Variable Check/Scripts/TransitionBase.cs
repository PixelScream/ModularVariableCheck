using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class TransitionBase : ScriptableObject
{
	public ScriptableObject tings;
	public abstract bool Check();
}
