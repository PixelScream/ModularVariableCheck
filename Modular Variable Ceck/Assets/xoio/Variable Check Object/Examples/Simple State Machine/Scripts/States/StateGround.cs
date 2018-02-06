using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName=("xoio/State Machine/State/Ground"))]
public class StateGround : StateBase {

	public float speed = 1, jumpForce = 5;

	public override void Init( StateMachineController con)
	{
		base.Init(con);

		StateRef.BeforeEntering = BeforeEntering;
		StateRef.Loop = Loop;
	}

	void BeforeEntering()
	{
		Controller.M_Material.color = Color.cyan;
	}

	void Loop()
	{
		
		Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))  * speed;

		Controller.M_Rigidbody.AddForce(input, ForceMode.Acceleration);

		if(Input.GetButtonDown("Jump"))
		{
			Vector3 v = Controller.Velocity;
			v.y = jumpForce;
			Controller.Velocity = v;
		}

	}
}
