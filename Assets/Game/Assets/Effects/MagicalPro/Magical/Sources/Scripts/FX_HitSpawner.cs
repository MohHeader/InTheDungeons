using UnityEngine;

namespace MagicalFX {
    public class FX_HitSpawner : MonoBehaviour {
        public bool DestoyOnHit = false;
        public bool FixRotation = false;


        public GameObject FXSpawn;
        public float LifeTime = 0;
        public float LifeTimeAfterHit = 1;

        private void Start() {
        }

        private void Spawn() {
            if (FXSpawn != null)
            {
                var rotate = transform.rotation;
                if (!FixRotation)
                    rotate = FXSpawn.transform.rotation;
                var fx = (GameObject) Instantiate(FXSpawn, transform.position, rotate);
                if (LifeTime > 0)
                    Destroy(fx.gameObject, LifeTime);
            }
            if (DestoyOnHit)
            {
                Destroy(gameObject, LifeTimeAfterHit);
                if (gameObject.GetComponent<Collider>())
                    gameObject.GetComponent<Collider>().enabled = false;
            }
        }

        private void OnTriggerEnter(Collider other) {
            Spawn();
        }

        private void OnCollisionEnter(Collision collision) {
            Spawn();
        }
    }
}