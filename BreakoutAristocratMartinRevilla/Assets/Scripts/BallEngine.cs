using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallEngine : MonoBehaviour
{
    [Header("Ball Settings")]
    public GameObject ballPrefab;
    public Transform ballSpawnPoint;
    public float respawnDelay = 0.5f;
    public int maxBalls = 5;
    public float ballSpawnSpread = 2f;

    [Header("Launch Settings")]
    public float ballLaunchDelay = 1f;
    public bool waitingForLaunch = true;

    private List<Ball> activeBalls = new List<Ball>();

    private int bricksHitForSpeed = 0;
    public float speedIncreaseMultiplier = 1.1f;

    [SerializeField]
    public BrickEngine brickEngine;

    public void InitializeGame()
    {
        DespawnAllBalls();

        // First ball requires spacebar
        SpawnNewBall(true); 
    }

    public void DespawnAllBalls()
    {
        foreach (Ball ball in activeBalls)
        {
            if (ball != null) Destroy(ball.gameObject);
        }
        activeBalls.Clear();
    }

    public void SpawnNewBall(bool requireSpacebar = false)
    {
        //Fix edge case where last brick is special and it spawns a new one on the next level
        if (brickEngine.activeBricks == 0)
            return;

        StartCoroutine(SpawnBall(requireSpacebar));
    }

    private IEnumerator SpawnBall(bool requireSpacebar)
    {
        yield return new WaitForSeconds(respawnDelay);

        GameObject newBallObj = Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);
        newBallObj.GetComponent<Ball>().Initialize(this);
        Ball newBall = newBallObj.GetComponent<Ball>();

        activeBalls.Add(newBall);

        newBall.rb.isKinematic = true;
        newBall.SetVisualState(Ball.VisualState.Waiting);
        waitingForLaunch = true;

        if (!requireSpacebar)
        {
            LaunchBall(newBall, true);
        }

    }

    public void LaunchBall(Ball ball, bool launchImmediate)
    {
        ball.rb.isKinematic = false;
        ball.Launch(launchImmediate);
        ball.SetVisualState(Ball.VisualState.Normal);
        waitingForLaunch = false;
    }

    void Update()
    {
        if (waitingForLaunch && Input.GetKeyDown(KeyCode.Space))
        {
            // Find the waiting ball and launch it
            foreach (Ball ball in activeBalls)
            {
                if (ball.rb.isKinematic)
                {
                    LaunchBall(ball,false);
                    break;
                }
            }
        }
    }

    public void BallLost(Ball lostBall)
    {
        activeBalls.Remove(lostBall);

        if (activeBalls.Count == 0)
        {
            InitializeBall();
        }
    }

    private void InitializeBall()
    {
        SpawnNewBall(true);
    }
}