using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ModularCheckBase : ScriptableObject{


	public abstract bool Check();
	public abstract void Init(ScriptableObject so);

}
