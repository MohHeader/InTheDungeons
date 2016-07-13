using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.Helpers {
    public static class VectorHelpers {
        public static float GetPathLength(this Vector3[] path) {
            if (path == null || path.Length <= 1) return 0f;
            float length = 0f;
            for (int i = 0; i < path.Length - 1; i++) {
                length += Vector3.Distance(path[i], path[i + 1]);
            }
            return length;
        }

        public static float GetPathLength(this List<Vector3> path)
        {
            if (path == null || path.Count <= 1) return 0f;
            float length = 0f;
            for (int i = 0; i < path.Count - 1; i++)
            {
                length += Vector3.Distance(path[i], path[i + 1]);
            }
            return length;
        }
    }
}