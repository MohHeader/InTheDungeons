//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using System;
using System.Collections;
using Assets.Game.Scripts.Utility.Common;

/// <summary>
/// Extends System.Random with gamedev based utility functions
/// </summary>
public static class RandomExtensions {
	
	public static float NextFloat(this FastRandom random)
	{
		return (float)random.NextDouble();
	}

	public static Vector3 OnUnitSphere(this FastRandom random) {
		var z = (float)random.NextDouble() * 2 - 1;
		var rxy = Mathf.Sqrt(1 - z*z);
		var phi = (float)random.NextDouble() * 2 * Mathf.PI;
		var x = rxy * Mathf.Cos(phi);
		var y = rxy * Mathf.Sin(phi);
		return new Vector3(x, y, z);
	}
	
	public static float Range(this FastRandom random, float a, float b) {
		return a + NextFloat(random) * (b - a);
	}

	public static int Range(this FastRandom random, int a, int b) {
		return Mathf.RoundToInt(a + NextFloat(random) * (b - a));
	}

	public static float value(this FastRandom random) {
		return NextFloat(random);
	}
}
