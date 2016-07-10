//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.Constraints.Grid
{
    //[Meta(displayText: "2x2 (Grid)")]
    [System.Serializable]
    public class SpatialConstraintGrid2x2 : SpatialConstraint
    {
        public SpatialConstraintGridCell[,] cells = new SpatialConstraintGridCell[2, 2];
        public override void OnEnable()
        {
            base.OnEnable();

            for (int x = 0; x < cells.GetLength(0); x++)
            {
                for (int y = 0; y < cells.GetLength(1); y++)
                {
                    if (cells[x, y] == null)
                    {
                        cells[x, y] = new SpatialConstraintGridCell();
                    }
                }
            }
        }
    }
}