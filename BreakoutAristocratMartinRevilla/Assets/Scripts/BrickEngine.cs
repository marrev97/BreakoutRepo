using UnityEngine;
using BrickNamespace;

public class BrickEngine : MonoBehaviour
{
    [Header("Layout Settings")]
    public int rows = 4;
    public int columns = 12;
    public float horizontalSpacing = 1.5f;
    public float verticalSpacing = 0.8f;
    public Vector2 startPosition = new Vector2(-8f, 2f);

    [Header("Brick Variants")]
    public GameObject blueBrickVariant;
    public GameObject greenBrickVariant;
    public GameObject yellowBrickVariant;
    public GameObject redBrickVariant;

    public Transform brickContainer;

    [SerializeField]
    public GameController GameController;

    [SerializeField]
    public BallEngine BallEngine;

    [SerializeField]
    public PowerUpEngine PowerUpEngine;

    [HideInInspector]
    public int activeBricks = 0;

    private ReadInLevelsLoader levelLoader;

    void Awake()
    {
        levelLoader = new ReadInLevelsLoader();
    }

    void Start()
    {
        activeBricks = transform.childCount;
        LoadLevel();
    }

    public void SpawnBricks()
    {
        // Clear existing bricks
        foreach (Transform child in brickContainer)
        {
            Destroy(child.gameObject);
        }

        // Try custom level first if enabled and available
        if ( levelLoader.LevelCount > 0)
        {
            if (levelLoader.TryGetLevel(GameController.currentLevel, out LevelData levelData))
            {
                GenerateLevelFromData(levelData);
                return;
            }
        }

        // to default generation
        GenerateDefaultLevel();
    }

    private void GenerateLevelFromData(LevelData levelData)
    {
        columns = levelData.columns;
        rows = levelData.rows;
        PowerUpEngine.powerUpDropChance = levelData.powerUpProbability / 100f;


        //clamp the columns and rows if they excteed 12 or 7
        if (columns > 12)
            columns = 12;
        if (rows > 7)
            rows = 7;

        for (int row = 0; row < rows; row++)
        {
            GameObject brickVariant = SelectBrickVariant(row);

            for (int col = 0; col < columns; col++)
            {
                Vector2 position = new Vector2(
                    startPosition.x + (col * horizontalSpacing),
                    startPosition.y - (row * verticalSpacing)
                );

                SpawnBrickAtPosition(position, brickVariant);
                activeBricks++;
            }
        }
    }

    private void GenerateDefaultLevel()
    {
        for (int row = 0; row < rows; row++)
        {
            GameObject brickVariant = SelectBrickVariant(row);

            for (int col = 0; col < columns; col++)
            {
                Vector2 position = new Vector2(
                    startPosition.x + (col * horizontalSpacing),
                    startPosition.y - (row * verticalSpacing)
                );

                SpawnBrickAtPosition(position, brickVariant);
                activeBricks++;
            }
        }
    }

    private GameObject SelectBrickVariant(int row)
    {
        switch (row % 4)
        {
            case 0: return blueBrickVariant;
            case 1: return greenBrickVariant;
            case 2: return yellowBrickVariant;
            case 3: return redBrickVariant;
            default: return blueBrickVariant;
        }
    }

    public void BrickDestroyed(GameObject brick)
    {
        activeBricks--;

        if (activeBricks <= 0)
        {
            LevelCompleted();
        }
    }

    private void LevelCompleted()
    {
        GameController.currentLevel++;

        // Despawn existing balls first
        BallEngine.DespawnAllBalls();

        // delay before respawning
        Invoke("LoadLevel", 1f);

        GameController.UpdateLevelText();

        PowerUpEngine.ResetForNewLevel();
    }

    public void LoadLevel()
    {
        // Spawn new bricks
        SpawnBricks();

        // Spawn new ball (in ready state)
        BallEngine.SpawnNewBall(true);
    }

    void SpawnBrickAtPosition(Vector2 position, GameObject brickPrefab)
    {
        GameObject brick = Instantiate(brickPrefab, position, Quaternion.identity, brickContainer);
        brick.GetComponent<Brick>().Initialize(GameController, PowerUpEngine, BallEngine, this);

        // Randomly make this a power-up brick
        if (PowerUpEngine.ShouldBecomePowerUpBrick())
        {
            brick.GetComponent<Brick>().isPowerUpBrick = true;
            PowerUpEngine.RegisterPowerUpSpawn();
        }
    }
}