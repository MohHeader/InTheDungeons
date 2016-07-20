﻿//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using System.Collections.Generic;
using Assets.Game.Scripts.Utility.Common;
using DungeonArchitect;
using DungeonArchitect.Utils;

public class NonViewBlockingPillarSelectorRule : NonViewBlockingSelectionRule {
	public override bool CanSelect(PropSocket socket, Matrix4x4 propTransform, DungeonModel model, FastRandom random) {
		var selected = base.CanSelect(socket, propTransform, model, random);
		if (!selected) return false;

		// Further filter near the door positions
		var cellId = socket.cellId;
		if (model is GridDungeonModel) {
			var gridModel = model as GridDungeonModel;
			foreach (var door in gridModel.Doors) {
				if (door.AdjacentCells.Length == 2) {
					if (door.AdjacentCells[0] == cellId || door.AdjacentCells[1] == cellId) {
						return false;
					}
				}
			}
			// Check if a door exists in this location
			//gridModel.DoorManager.ContainsDoorBetweenCells

			var cell = gridModel.GetCell(socket.cellId);
			if (cell == null) return false;
		}

		return true;
	}


}
