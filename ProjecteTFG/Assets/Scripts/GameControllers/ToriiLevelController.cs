using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToriiLevelController : MonoBehaviour
{
    public static ToriiLevelController instance;

    public SwordPickUp sword;

    public Collider2D summonerGate;
    public Collider2D preserverGate;
    public Collider2D destroyerGate;

    public GameState currentGameState;

    public List<Transform> startingPoints;

    public Player player;

    public SoundController soundController;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        Globals.gameState = currentGameState;
        InitializeLevel();
    }

    public void OpenSummoner()
    {
        summonerGate.gameObject.SetActive(false);
    }

    public void OpenPreserver()
    {
        preserverGate.gameObject.SetActive(false);
    }

    public void OpenDestroyer()
    {
        destroyerGate.gameObject.SetActive(false);
    }

    public void InitializeLevel()
    {
        sword.gameObject.SetActive(false);

        if (Globals.gameState == GameState.Started)
        {
            sword.gameObject.SetActive(true);
            player.transform.position = startingPoints[0].position;
            StartCoroutine(ICinematicSword());
        }
        else if (Globals.gameState == GameState.SwordPicked)
        {
            player.transform.position = startingPoints[1].position;
        }
        else if (Globals.gameState == GameState.SummonerDefeated)
        {
            player.transform.position = startingPoints[2].position;
            StartCoroutine(ICinematicSummonerDefeated());
        }
        else if (Globals.gameState == GameState.PerserverDefeated)
        {
            player.transform.position = startingPoints[3].position;
            StartCoroutine(ICinematicPreserverDefeated());
        }
    }

    public void PickSword()
    {
        StartCoroutine(ICinematicPickSword());
    }

    private IEnumerator ICinematicSword()
    {

        GameManager.instance.BlockInputs(true);
        ScreenManager.instance.StartFadeShowScreen(4,2);
        yield return new WaitForSeconds(2f);

        soundController.PlaySound("enter_torii_level");
        yield return new WaitForSeconds(4f);

        CameraManager.instance.mainCamera.SetDestination(sword.transform.position,2f);
        yield return new WaitForSeconds(5f);
        CameraManager.instance.mainCamera.FollowPlayer(1f);
        yield return new WaitForSeconds(2f);
        GameManager.instance.BlockInputs(false);
    }

    private IEnumerator ICinematicPickSword()
    {
        GameManager.instance.BlockInputs(true);
        CameraManager.instance.mainCamera.SetDestination(player.transform.position + Vector3.up*2, 0.2f);
        yield return new WaitForSeconds(14f);

        CameraManager.instance.mainCamera.SetDestination(summonerGate.transform.position, 1f);
        yield return new WaitForSeconds(2f);
        OpenSummoner();
        yield return new WaitForSeconds(2f);
        CameraManager.instance.mainCamera.FollowPlayer(1f);
        yield return new WaitForSeconds(2f);
        GameManager.instance.BlockInputs(false);
    }

    private IEnumerator ICinematicSummonerDefeated()
    {
        GameManager.instance.BlockInputs(true);
        ScreenManager.instance.StartFadeShowScreen(4, 1);
        yield return new WaitForSeconds(4f);

        CameraManager.instance.mainCamera.SetDestination(preserverGate.transform.position, 1f);
        yield return new WaitForSeconds(2f);
        OpenPreserver();
        yield return new WaitForSeconds(2f);
        CameraManager.instance.mainCamera.FollowPlayer(1f);
        yield return new WaitForSeconds(2f);
        GameManager.instance.BlockInputs(false);
    }

    private IEnumerator ICinematicPreserverDefeated()
    {
        GameManager.instance.BlockInputs(true);
        ScreenManager.instance.StartFadeShowScreen(4, 1);
        yield return new WaitForSeconds(4f);

        CameraManager.instance.mainCamera.SetDestination(destroyerGate.transform.position, 1f);
        yield return new WaitForSeconds(2f);
        OpenDestroyer();
        yield return new WaitForSeconds(2f);
        CameraManager.instance.mainCamera.FollowPlayer(1f);
        yield return new WaitForSeconds(2f);
        GameManager.instance.BlockInputs(false);
    }
}
