//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using System.Collections;
using Assets.Game.Scripts.Utility.Common;
using DungeonArchitect;

public class RoomSelectionRule : SelectorRule {
	public override bool CanSelect(PropSocket socket, Matrix4x4 propTransform, DungeonModel model, FastRandom random) {
		if (model is GridDungeonModel) {
			var gridModel = model as GridDungeonModel;
			var cell = gridModel.GetCell(socket.cellId);
			if (cell == null) return false;
			return cell.CellType == CellType.Room;
		} else {
			return false;
		}
	}
}
