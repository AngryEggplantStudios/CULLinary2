using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Enable this to start the truck tutorial!
public class TruckTutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialUiBox;
    [SerializeField] private TextMeshProUGUI tutorialUiText;
    [SerializeField] private AudioSource taskCompleteAudio;
    [SerializeField] private string[] tasksText =
    {
        "Tap W once to move forward",
        "Press A or D to steer",
        "Hold S to brake",
        "While you are stationary, press S again to reverse",
        "Hold W to brake while reversing",
        "Press W again to switch to forward gear",
        "Try not to drive too fast as accidents may happen!",
        "And no, you cannot damage enemies by hitting them!",
        "If monsters attack, it is best to get out and deal with them!",
        "Good luck out there, chef!",
        "Good luck out there, chef!"
    };
    private Func<bool>[] taskChecks;
    private Action[] taskAfters;
    private int taskNum = 0;
    // Delay the checking of tasks
    private float delay = 0.0f;
    private bool pauseTutorial = true;

    void Awake()
    {
        taskChecks = new Func<bool>[]
        {
            CheckTutorialTask0,
            CheckTutorialTask1,
            CheckTutorialTask2,
            CheckTutorialTask3,
            CheckTutorialTask4,
            CheckTutorialTask5,
            CheckTutorialTaskAlwaysTrue,
            CheckTutorialTaskAlwaysTrue,
            CheckTutorialTaskAlwaysTrue,
            CheckTutorialTaskAlwaysTrue,
            CheckTutorialTaskAlwaysTrue
        };
        taskAfters = new Action[]
        {
            AfterTutorialTask0,
            AfterTutorialTask1,
            AfterTutorialTask2,
            AfterTutorialTask3,
            AfterTutorialTask4,
            AfterTutorialTask5,
            AfterTutorialTaskWaitFiveSeconds,
            AfterTutorialTaskWaitFiveSeconds,
            AfterTutorialTaskWaitFiveSeconds,
            AfterTutorialTask9,
            AfterTutorialTask10
        };
    }
    void OnEnable()
    {
        pauseTutorial = false;
        tutorialUiBox.SetActive(true);
        if (delay > 0.0f && taskNum > 0)
        {
            AfterTask(taskNum - 1);
        }
        else
        {
            RunAction(taskNum);
        }
    }

    void Update()
    {
        // Pause tutorial when tutorial is disabled
        if (pauseTutorial)
        {
            return;
        }

        if (delay > 0.0f)
        {
            delay -= Time.deltaTime;
        }
        else
        {
            RunAction(taskNum);
            if (CheckTask(taskNum))
            {                
                taskCompleteAudio.Play();
                AfterTask(taskNum);
                taskNum++;
            }
        }
    }

    void OnDisable()
    {
        pauseTutorial = true;
        tutorialUiBox.SetActive(false);
    }

    private void RunAction(int num)
    {
        Debug.Log(num);
        tutorialUiText.text = tasksText[num];
    }

    private bool CheckTask(int num)
    {
        return taskChecks[num]();
    }
    
    private void AfterTask(int num)
    {
        taskAfters[num]();
    }

    private bool CheckTutorialTask0()
    {
        return Input.GetKeyDown(KeyCode.W);
    }
    
    private bool CheckTutorialTask1()
    {
        return Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D);
    }
    
    private bool CheckTutorialTask2()
    {
        return Input.GetKeyDown(KeyCode.S);
    }
    
    private bool CheckTutorialTask3()
    {
        return DrivingManager.instance.IsTruckReversing();
    }
    private bool CheckTutorialTask4()
    {
        return Input.GetKeyDown(KeyCode.W);
    }

    private bool CheckTutorialTask5()
    {
        return !DrivingManager.instance.IsTruckReversing();
    }

    private bool CheckTutorialTaskAlwaysTrue()
    {
        return true;
    }

    private void AfterTutorialTask0()
    {
        tutorialUiText.text = "Good! Try not to hold down the W key, as you may lose control!";
        delay = 5.0f;
    }
    
    private void AfterTutorialTask1()
    {
        tutorialUiText.text = "Nice! Same for steering, you can tap the A and D keys.";
        delay = 5.0f;
    }
    
    private void AfterTutorialTask2()
    {
        tutorialUiText.text = "Terrific! Brake when you have to make a sharp turn!";
        delay = 5.0f;
    }
    
    private void AfterTutorialTask3()
    {
        tutorialUiText.text = "Reverse to get out of tight spots!";
        delay = 5.0f;
    }
    private void AfterTutorialTask4()
    {
        tutorialUiText.text = "Great job!";
        delay = 1.0f;
    }

    private void AfterTutorialTask5()
    {
        tutorialUiText.text = "You now know everything that you need to drive safely!";
        delay = 5.0f;
    }
    
    private void AfterTutorialTaskWaitFiveSeconds()
    {
        delay = 5.0f;
    }

    private void AfterTutorialTask9()
    {
        delay = 3.0f;
    }
    
    private void AfterTutorialTask10()
    {
        PlayerManager.instance.isTruckTutorialDone = true;
        gameObject.SetActive(false);
    }
}
