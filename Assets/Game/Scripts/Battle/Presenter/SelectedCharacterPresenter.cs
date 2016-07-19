using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = System.Diagnostics.Debug;

namespace Assets.Game.Scripts.Battle.Presenter {
    public class SelectedCharacterPresenter : PresenterBase<SquadPresenter> {
        private readonly CompositeDisposable _characterDisposables = new CompositeDisposable();
        private readonly List<GameObject> _lastRender = new List<GameObject>();

        protected Path LastPath;
        public Vector3 PathOffset = new Vector3(0f, 0.1f, 0f);
        protected CharacterPresenter SelectedCharacter;

        protected GameObject SelectionGameObject;
        public GameObject SelectionPrefab;
        public Material SquareMat;

        private float CellSize;

        protected override IPresenter[] Children
        {
            get { return EmptyChildren; }
        }

        protected override void BeforeInitialize(SquadPresenter argument) {
            var gridGraph = AstarPath.active.astarData.gridGraph;
            CellSize = gridGraph.nodeSize*1000;
        }

        private void SelectedCharacterChanged(CharacterPresenter characterPresenter) {
            _constantPathNodes = null;
            _characterDisposables.Clear();
            ClearPrevious();

            if (characterPresenter == null) {
                SelectedCharacter = null;
                if (SelectionGameObject != null) SelectionGameObject.SetActive(false);
                return;
            }
            if (SelectionGameObject == null) SelectionGameObject = Instantiate(SelectionPrefab);

            SelectedCharacter = characterPresenter;
            SelectedCharacter.CharacterState.Subscribe(CharacterStateChanged).AddTo(_characterDisposables);
            SelectionGameObject.transform.SetParent(characterPresenter.transform, false);
            SelectionGameObject.transform.localPosition = new Vector3(0f, 0.2f, 0f);
        }

        private void CharacterStateChanged(CharacterPresenter.CharacterStateEnum characterStateEnum) {
            switch (characterStateEnum) {
                case CharacterPresenter.CharacterStateEnum.Idle:
                    ShowPossibleMovements();
                    break;
                case CharacterPresenter.CharacterStateEnum.Moving:
                    ClearPrevious();
                    break;
                case CharacterPresenter.CharacterStateEnum.SelectingTarget:
                    ClearPrevious();
                    break;
                case CharacterPresenter.CharacterStateEnum.UsingSkill:
                    ClearPrevious();
                    break;
                case CharacterPresenter.CharacterStateEnum.Dead:
                    ClearPrevious();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("characterStateEnum", characterStateEnum, null);
            }
        }

        protected void ShowPossibleMovements() {
            StartCoroutine(CalculateConstantPath());
        }

        protected override void Initialize(SquadPresenter argument) {
            argument.SelectedCharacter.Subscribe(SelectedCharacterChanged);
        }

        public IEnumerator CalculateConstantPath() {
            SelectedCharacter.GetComponent<GraphUpdateScene>().enabled = false;
            AstarPath.active.Scan();
            while (AstarPath.active.isScanning)
            {
                yield return new WaitForFixedUpdate();
            }

            var constPath = ConstantPath.Construct(SelectedCharacter.transform.position, (int) ((SelectedCharacter.CharacterData.MovementRange.Value + 1) * CellSize), OnPathComplete);

            SelectedCharacter.Seeker.StartPath(constPath);
            LastPath = constPath;
            yield return constPath.WaitForPath();
            SelectedCharacter.GetComponent<GraphUpdateScene>().enabled = true;
        }

        private List<GraphNode> _constantPathNodes = null;

        protected void OnPathComplete(Path p) {
            if (p.GetType() == typeof(ConstantPath)) {
                ClearPrevious();
                //The following code will build a mesh with a square for each node visited

                var constPath = p as ConstantPath;
                Debug.Assert(constPath != null, "constPath != null");
                var nodes = constPath.allNodes;
                _constantPathNodes = nodes;

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
                        UnityEngine.Debug.LogError("Too many nodes, rendering a mesh would throw 65K vertex error. Using Debug.DrawRay instead for the rest of the nodes");
                        drawRaysInstead = true;
                    }

                    if (drawRaysInstead) {
                        UnityEngine.Debug.DrawRay(pos, Vector3.up, Color.blue);
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
                for (var i = 0; i < uv.Length; i += 4)
                {
                    uv[i] = new Vector2(0, 0);
                    uv[i + 1] = new Vector2(1, 0);
                    uv[i + 2] = new Vector2(0, 1);
                    uv[i + 3] = new Vector2(1, 1);
                }

                mesh.vertices = vs;
                mesh.triangles = tris;
                mesh.uv = uv;
                mesh.RecalculateNormals();

                var go = new GameObject("PathMesh", typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider));
                go.layer = 31;
                var fi = go.GetComponent<MeshFilter>();
                fi.mesh = mesh;
                var re = go.GetComponent<MeshRenderer>();
                re.material = SquareMat;
                var meshCollider = go.GetComponent<MeshCollider>();
                meshCollider.convex = true;
                meshCollider.sharedMesh = mesh;
                meshCollider.isTrigger = true;

                if (SelectedCharacter.CharacterState.Value == CharacterPresenter.CharacterStateEnum.Idle)
                    _lastRender.Add(go);
            }
        }

        private void ClearPrevious() {
            for (var i = 0; i < _lastRender.Count; i++) {
                Destroy(_lastRender[i]);
            }
            _lastRender.Clear();
            _constantPathNodes = null;
        }

        public void Update() {
            if (Input.GetMouseButtonDown(0) && SelectedCharacter != null && !EventSystem.current.IsPointerOverGameObject() && _constantPathNodes != null) {
                RaycastHit hit;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit) && hit.transform.name == "PathMesh")
                {
                    var closestNode = AstarPath.active.GetNearest(hit.point).node;
                    if (_constantPathNodes.Contains(closestNode))
                        StartCoroutine(SelectedCharacter.MoveTo(hit.point));
                }
            }
        }
    }
}