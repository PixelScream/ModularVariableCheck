using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName=("xoio/State Machine/State/Dash"))]
public class StateDash : StateBase {

	public float speed = 1, duration = 0.5f;

	float exitTime;
	Vector3 dir;
	public bool Dashing { get; private set; } = false;

	public AnimationCurve velocityCurve;

	public override void Init( StateMachineController con)
	{
		base.Init(con);

		StateRef.BeforeEntering = BeforeEntering;
		StateRef.Loop = Loop;
	}

	void BeforeEntering()
	{
		Dashing = true;
		exitTime = Time.time + duration;
		dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))  * speed;
		Controller.M_Material.color = Color.green;
	}

	void Loop()
	{
		Controller.Velocity = velocityCurve.Evaluate( (exitTime - Time.time) / duration) * dir;

		if(Time.time > exitTime)
		{
			Dashing = false;
		}

	}
}
