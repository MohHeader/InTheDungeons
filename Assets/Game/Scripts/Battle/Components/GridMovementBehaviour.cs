using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Game.Scripts.Battle.Presenter;
using Assets.Game.Scripts.Helpers;
using DG.Tweening;
using Pathfinding;
using UnityEngine;

namespace Assets.Game.Scripts.Battle.Components {
    public class GridMovementBehaviour : MonoBehaviour {
        private float _cellSize;
        private CharacterPresenter _character;
        private List<GraphNode> _constantPathNodes;
        private int _currentWaypoint;
        private GraphUpdateScene _graphUpdate;
        private GameObject _lastRenderer;

        private Seeker _seeker;
        private Animator _animator;
        protected Path Path;

        protected bool PathComplete;

        public Vector3 PathOffset = new Vector3(0f, 0.1f, 0f);
        //The waypoint we are currently moving towards
        public float RotationSpeed = 360;

        //The AI's speed per second
        public float Speed = 100;
        public Material SquareMat;

        public Seeker Seeker
        {
            get { return _seeker ?? (_seeker = GetComponent<Seeker>()); }
        }

        public void Initialize() {
            var gridGraph = AstarPath.active.astarData.gridGraph;
            _cellSize = gridGraph.nodeSize*1000;
            _character = GetComponent<CharacterPresenter>();
            _graphUpdate = GetComponent<GraphUpdateScene>();
            _animator = gameObject.FindAnimatorComponent();
        }

        public void ClearPrevious() {
            if (_lastRenderer != null) Destroy(_lastRenderer);
            _constantPathNodes = null;
        }

        protected void PathRangeCallback(Path p) {
            if (p.GetType() == typeof(ConstantPath)) {
                ClearPrevious();
                var constPath = p as ConstantPath;
                if (constPath != null) _constantPathNodes = constPath.allNodes;
            }
            PathComplete = true;
        }

        public void BuildPathMesh() {
            //The following code will build a mesh with a square for each node visited

            if (_constantPathNodes == null) return;

            var nodes = _constantPathNodes;
            var mesh = new Mesh();

            var verts = new List<Vector3>();

            var drawRaysInstead = false;

            //This will loop through the nodes from furthest away to nearest, not really necessary... but why not :D
            //Note that the reverse does not, as common sense would suggest, loop through from the closest to the furthest away
            //since is might contain duplicates and only the node duplicate placed at the highest index is guarenteed to be ordered correctly.
            // i >= 1 - для исключения ноды на которой стоит персонаж
            for (var i = nodes.Count - 1; i >= 1; i--) {
                var pos = (Vector3) nodes[i].position + PathOffset;
                if (verts.Count == 65000 && !drawRaysInstead) {
                    Debug.LogError("Too many nodes, rendering a mesh would throw 65K vertex error. Using Debug.DrawRay instead for the rest of the nodes");
                    drawRaysInstead = true;
                }

                if (drawRaysInstead) {
                    Debug.DrawRay(pos, Vector3.up, Color.blue);
                    continue;
                }

                //Add vertices in a square
                var gg = AstarData.GetGraph(nodes[i]) as GridGraph;
                var scale = 1F;

                if (gg != null) scale = gg.nodeSize;

                verts.Add(pos + new Vector3(-0.5F, 0, -0.5F)*scale);
                verts.Add(pos + new Vector3(0.5F, 0, -0.5F)*scale);
                verts.Add(pos + new Vector3(-0.5F, 0, 0.5F)*scale);
                verts.Add(pos + new Vector3(0.5F, 0, 0.5F)*scale);
            }

            //Build triangles for the squares
            var vs = verts.ToArray();
            var tris = new int[3*vs.Length/2];
            for (int i = 0, j = 0; i < vs.Length; j += 6, i += 4) {
                tris[j + 0] = i;
                tris[j + 1] = i + 1;
                tris[j + 2] = i + 2;

                tris[j + 3] = i + 1;
                tris[j + 4] = i + 3;
                tris[j + 5] = i + 2;
            }

            var uv = new Vector2[vs.Length];
            //Set up some basic UV
            for (var i = 0; i < uv.Length; i += 4) {
                uv[i] = new Vector2(0, 0);
                uv[i + 1] = new Vector2(1, 0);
                uv[i + 2] = new Vector2(0, 1);
                uv[i + 3] = new Vector2(1, 1);
            }

            mesh.vertices = vs;
            mesh.triangles = tris;
            mesh.uv = uv;
            mesh.RecalculateNormals();

            var pathMesh = new GameObject("PathMesh", typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider));
            pathMesh.layer = 31;
            var meshFilter = pathMesh.GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            var meshRenderer = pathMesh.GetComponent<MeshRenderer>();
            meshRenderer.material = SquareMat;
            var meshCollider = pathMesh.GetComponent<MeshCollider>();
            meshCollider.convex = true;
            meshCollider.sharedMesh = mesh;
            meshCollider.isTrigger = true;

            if (_lastRenderer != null) DestroyImmediate(_lastRenderer);

            _lastRenderer = pathMesh;
        }

        public IEnumerator CalculatePossiblePaths() {
            PathComplete = false;

            if (_graphUpdate != null) _graphUpdate.enabled = false;
            AstarPath.active.Scan();
            while (AstarPath.active.isScanning) {
                yield return new WaitForFixedUpdate();
            }

            var constPath = ConstantPath.Construct(_character.transform.position, (int) ((_character.CharacterData.MovementRange.Value + 1)*_cellSize), PathRangeCallback);

            Seeker.StartPath(constPath);
            yield return constPath.WaitForPath();
            if (_graphUpdate != null) _graphUpdate.enabled = true;

            while (!PathComplete) {
                yield return new WaitForEndOfFrame();
            }
            yield return null;
        }

        public bool CanMoveToGridNode(GraphNode node) {
            if (_constantPathNodes == null || !_constantPathNodes.Any()) return false;
            return _constantPathNodes.Contains(node);
        }

        public IEnumerator MoveToGridNode(GraphNode node) {
            IsMoving = true;
            yield return MoveTo((Vector3) node.position);
            while (IsMoving) {
                yield return new WaitForEndOfFrame();
            }
        }

        #region Movement methods

        protected bool IsMoving;

        public IEnumerator MoveTo(Vector3 position) {
            if (_graphUpdate != null) _graphUpdate.enabled = false;
            AstarPath.active.Scan();
            while (AstarPath.active.isScanning) {
                yield return new WaitForFixedUpdate();
            }

            var start = AstarPath.active.GetNearest(transform.position);
            var end = AstarPath.active.GetNearest(position);

            Seeker.StartPath((Vector3) start.node.position, (Vector3) end.node.position, DirectPathCallback);
        }

        public void DirectPathCallback(Path newPath) {
            if (!newPath.error) {
                if (_graphUpdate != null) _graphUpdate.enabled = true;
                _character.CharacterState.SetValueAndForceNotify(CharacterPresenter.CharacterStateEnum.Moving);

                //Reset the waypoint counter
                _currentWaypoint = -1;

                Path = newPath;
                _animator.SetBool("Moving", true);
                RotateCallback();
            }
            else {
                IsMoving = false;
            }
        }

        protected void MoveCallback() {
            var duration = Vector3.Distance(Path.vectorPath[_currentWaypoint], transform.position)/Speed;
            transform.DOMove(Path.vectorPath[_currentWaypoint], duration).OnComplete(RotateCallback);
        }

        protected void RotateCallback() {
            _currentWaypoint++;
            if (_currentWaypoint >= Path.vectorPath.Count) {
                MoveFinished();
                return;
            }
            if (Vector3.Distance(transform.position, Path.vectorPath[_currentWaypoint]) < 0.1f) {
                RotateCallback();
            }
            else {
                var sourceVector = new Vector3(gameObject.transform.position.x, 0f, gameObject.transform.position.z);
                var destinationVector = new Vector3(Path.vectorPath[_currentWaypoint].x, 0f, Path.vectorPath[_currentWaypoint].z);
                var relativePos = (destinationVector - sourceVector).normalized;
                var duration = Vector3.Angle(transform.forward, relativePos);
                if (duration < 0.2f) MoveCallback();
                else
                    transform.DOLookAt(Path.vectorPath[_currentWaypoint], duration/RotationSpeed)
                             .OnComplete(MoveCallback);
            }
        }

        protected void MoveFinished() {
            _animator.SetBool("Moving", false);
            AstarPath.active.Scan();

            _character.CharacterData.RemainingActionPoint.Value--;
            Path = null;
            IsMoving = false;
            _character.CharacterState.SetValueAndForceNotify(CharacterPresenter.CharacterStateEnum.Idle);
        }

        #endregion
    }
}