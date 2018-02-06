using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName=("xoio/State Machine/State/Air"))]
public class StateAir : StateBase {

	public float speed = 1;

	public override void Init( StateMachineController con)
	{
		base.Init(con);

		StateRef.BeforeEntering = BeforeEntering;
		StateRef.Loop = Loop;
		StateRef.BeforeLeaving = BeforeLeaving;
	}

	void BeforeEntering()
	{
		Controller.M_Material.color = Color.yellow;
	}

	public bool Trigger1 { get; private set; } = false;

	void Loop()
	{
		Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))  * speed;

		Controller.M_Rigidbody.AddForce(input, ForceMode.Acceleration);

		// On Jump input a generic trigger is set
		// this could be reassigned to any state inside the editor
		if(Input.GetButtonDown("Jump"))
		{
			Trigger1 = true;
		}

	}

	void BeforeLeaving()
	{
		Trigger1 = false;
	}
}
