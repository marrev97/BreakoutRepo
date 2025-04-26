using UnityEngine;

namespace BrickNamespace
{
    public class Brick : MonoBehaviour
    {
        public int points; 

        [HideInInspector]
        public bool isPowerUpBrick = false;

        [HideInInspector]
        public GameController controller;

        [HideInInspector]
        public PowerUpEngine powerUpEngine;

        [HideInInspector]
        public BallEngine ballEngine;

        [HideInInspector]
        public BrickEngine brickEngine;

        public void Initialize(GameController gc, PowerUpEngine pe, BallEngine be, BrickEngine brickEngine)
        {
            controller = gc;
            powerUpEngine = pe;
            ballEngine = be;
            this.brickEngine = brickEngine;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ball"))
            {
                DestroyBrick();
            }
        }

        void DestroyBrick()
        {
            controller.AddScore(points);
            brickEngine.BrickDestroyed(GetComponent<GameObject>());
            Destroy(gameObject);
        }

        void Start()
        {
            //activate the child game object which has the power up sprite
            if (isPowerUpBrick)
            {
                GetComponent<SpriteRenderer>().enabled = false;
                transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        void OnDestroy()
        {
            //Edge case checking
            if (isPowerUpBrick && gameObject.scene.isLoaded)
            {
                SpawnPowerUp();
            }
        }

        void SpawnPowerUp()
        {
            powerUpEngine.RegisterPowerUpSpawn();
            ballEngine.SpawnNewBall(false);
        }
    }
}