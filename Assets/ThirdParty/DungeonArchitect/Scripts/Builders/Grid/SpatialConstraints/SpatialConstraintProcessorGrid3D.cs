//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using System.Collections.Generic;

namespace DungeonArchitect.Constraints
{
    public class SpatialConstraintProcessorGrid3D : SpatialConstraintProcessor
    {
        public override bool ProcessSpatialConstraint(SpatialConstraint constraint, PropSocket socket, DungeonModel model, List<PropSocket> levelSockets)
        {
            return false;
        }
    }
}