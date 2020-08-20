using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    private bool started = false;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void StartTutorial()
    {
        if (!started)
        {
            StartCoroutine(IStartTutorial());
            started = true;
        }
    }

    private IEnumerator IStartTutorial()
    {
        ButtonPopUp.instance.Show("Attack");
        while (ButtonPopUp.instance.IsShowing())
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        ButtonPopUp.instance.Show("Recall");

        while (ButtonPopUp.instance.IsShowing())
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        ButtonPopUp.instance.Show("Dash");

        while (ButtonPopUp.instance.IsShowing())
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        Debug.Log("Ended!");
    }
}
