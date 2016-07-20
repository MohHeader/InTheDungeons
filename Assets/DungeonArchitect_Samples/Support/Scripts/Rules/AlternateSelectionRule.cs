//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using System.Collections;
using Assets.Game.Scripts.Utility.Common;
using DungeonArchitect;

public class AlternateSelectionRule : SelectorRule {
	public override bool CanSelect(PropSocket socket, Matrix4x4 propTransform, DungeonModel model, FastRandom random) {
		return (socket.gridPosition.x + socket.gridPosition.z) % 2 == 0;
	}
}
