using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ToriiLevelController : MonoBehaviour
{
    public static ToriiLevelController instance;

    public SwordPickUp sword;

    public Collider2D summonerGate;
    public Collider2D preserverGate;
    public Collider2D destroyerGate;

    public Collider2D blockSummoner;
    public Collider2D blockPreserver;


    public List<Transform> startingPoints;

    public Player player;

    public SoundController soundController;
    public SoundController soundController2;

    public Light2D globalLight;
    public RainObject rainObject;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        InitializeLevel();
    }

    public void OpenSummoner()
    {
        soundController2.SetVolume(1);
        soundController2.PlaySound("seal_disappear",1f);
        summonerGate.GetComponentInChildren<Seal>().Fade(2);
        summonerGate.enabled = false;
    }

    public void OpenPreserver()
    {
        soundController2.SetVolume(1);
        soundController2.PlaySound("seal_disappear", 1f);
        preserverGate.GetComponentInChildren<Seal>().Fade(2);
        preserverGate.enabled = false;
    }

    public void OpenDestroyer()
    {
        soundController2.SetVolume(1);
        soundController2.PlaySound("seal_disappear", 1f);
        destroyerGate.GetComponentInChildren<Seal>().Fade(2);
        destroyerGate.enabled = false;
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
            rainObject.gameObject.SetActive(false);
            summonerGate.gameObject.SetActive(false); 
            globalLight.intensity = 1;
            player.transform.position = startingPoints[1].position;
            StartCoroutine(ISwordPicked());
        }
        else if (Globals.gameState == GameState.SummonerDefeated)
        {
            rainObject.gameObject.SetActive(false);
            blockSummoner.gameObject.SetActive(true);
            summonerGate.gameObject.SetActive(false);
            globalLight.intensity = 1;
            player.transform.position = startingPoints[2].position;
            StartCoroutine(ICinematicSummonerDefeated());
        }
        else if (Globals.gameState == GameState.PerserverDefeated)
        {
            rainObject.gameObject.SetActive(false);
            blockSummoner.gameObject.SetActive(true);
            blockPreserver.gameObject.SetActive(true);
            summonerGate.gameObject.SetActive(false);
            preserverGate.gameObject.SetActive(false);
            globalLight.intensity = 1;
            player.transform.position = startingPoints[3].position;
            StartCoroutine(ICinematicPreserverDefeated());
        }
        else
        {
            sword.gameObject.SetActive(true);
            player.transform.position = startingPoints[0].position;
            StartCoroutine(ICinematicSword());
        }
    }

    public void PickSword()
    {
        StartCoroutine(ICinematicPickSword());
    }

    private IEnumerator ICinematicSword()
    {
        player.MoveToDir(Vector3.up);
        GameManager.instance.BlockInputs(true);
        ScreenManager.instance.StartFadeShowScreen(4,2);
        yield return new WaitForSeconds(2f);

        yield return new WaitForSeconds(3f);
        player.StopMoving();
        yield return new WaitForSeconds(1f);
        soundController.PlaySound("enter_torii_level");
        CameraManager.instance.mainCamera.SetDestination(sword.transform.position,2f);
        yield return new WaitForSeconds(5f);
        CameraManager.instance.mainCamera.SetDestination(player.transform.position, 1f);
        yield return new WaitForSeconds(2f);
        CameraManager.instance.mainCamera.FollowPlayer();
        GameManager.instance.BlockInputs(false);

    }

    private IEnumerator ICinematicPickSword()
    {
        soundController2.PlaySound("eq");
        GameManager.instance.BlockInputs(true);
        CameraManager.instance.mainCamera.SetDestination(player.transform.position + Vector3.up*2, 0.2f);
        yield return new WaitForSeconds(14f);

        CameraManager.instance.mainCamera.SetDestination(summonerGate.transform.position, 1f);
        yield return new WaitForSeconds(2f);
        OpenSummoner();
        yield return new WaitForSeconds(2f);
        CameraManager.instance.mainCamera.SetDestination(player.transform.position, 1f);
        yield return new WaitForSeconds(3f);
        CameraManager.instance.mainCamera.FollowPlayer();
        GameManager.instance.BlockInputs(false);
        TutorialManager.instance.arrow.SetActive(true);
    }

    private IEnumerator ISwordPicked()
    {
        ScreenManager.instance.StartFadeShowScreen(2, 1);
        yield return new WaitForSeconds(3);
        GameManager.instance.BlockInputs(false);
       
    }

    private IEnumerator ICinematicSummonerDefeated()
    {
        GameManager.instance.BlockInputs(true);
        ScreenManager.instance.StartFadeShowScreen(2, 1);
        player.StartTeleportAppear(1, 2);
        yield return new WaitForSeconds(5f);
        soundController.PlaySound("enter_torii_level");
        CameraManager.instance.mainCamera.SetDestination(preserverGate.transform.position, 1f);
        yield return new WaitForSeconds(3f);
        OpenPreserver();
        yield return new WaitForSeconds(3f);
        CameraManager.instance.mainCamera.SetDestination(player.transform.position, 1f);
        yield return new WaitForSeconds(3f);
        CameraManager.instance.mainCamera.FollowPlayer();
        GameManager.instance.BlockInputs(false);
    }

    private IEnumerator ICinematicPreserverDefeated()
    {
        GameManager.instance.BlockInputs(true);
        ScreenManager.instance.StartFadeShowScreen(2, 1);
        player.StartTeleportAppear(1, 2);
        yield return new WaitForSeconds(5f);
        soundController.PlaySound("enter_torii_level");
        CameraManager.instance.mainCamera.SetDestination(destroyerGate.transform.position, 1f);
        yield return new WaitForSeconds(3f);
        OpenDestroyer();
        yield return new WaitForSeconds(3f);
        CameraManager.instance.mainCamera.SetDestination(player.transform.position, 1f);
        yield return new WaitForSeconds(3f);
        CameraManager.instance.mainCamera.FollowPlayer();
        GameManager.instance.BlockInputs(false);
    }
}
