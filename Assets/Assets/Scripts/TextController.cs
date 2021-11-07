using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TextController : MonoBehaviour
{
    private enum State : int
    {
        Intro,
        
        House,
        ContinueWalking,
        
        TakeSword,
        TakeBow,
        
        ShootBear,
        KillBear,
        HandsBear,
        BearCatchesYou

    }

    public Text storyText;

    public GameObject[] options = new GameObject[2];
    private State[] statesBasedOnSelectedOption = new State[2];

    private List<State> choicesTaken= new List<State>();
    
    private State currentState = State.Intro;
    public GameObject selector;
    
    public AudioSource audioSource;
    public AudioClip[] sfx = new AudioClip[12];

    private int currentSelection = 0;

    private void ResetSelector()
    {
        currentSelection = 0;
        MoveSelector(0);
    }

    private void SetStory(string text)
    {
        storyText.text = text;
    }
    private void SetOption(string text, State nextState)
    {
        options[currentSelection].GetComponent<Text>().text = text;
        statesBasedOnSelectedOption[currentSelection] = nextState;
        currentSelection = (currentSelection + 1) % options.Length;
    }

    private void MoveSelector(int where)
    {
        selector.transform.position = new Vector2(selector.transform.position.x, options[where].transform.position.y);
    }

    private void Intro()
    {
        currentState = State.Intro;
        audioSource.PlayOneShot(sfx[0], 1);
        SetStory("It is raining heavily. You are walking alone in the woods. Suddenly, " +
                 "you see a house down the alley.");
        SetOption("You run to visit the house", State.House);
        SetOption("You continue walking in the rain", State.ContinueWalking);
    }

    
    IEnumerator WaitAndPlayDoor()
    {
        yield return new WaitForSeconds(3f);
        
        audioSource.Stop();
        audioSource.PlayOneShot(sfx[1], 1);
    }
    
    
    private void House()
    {
        SetStory("You arrive at the door. The door knob seems lose, you easily enter in the house. " +
                 "You see there is food cooking on the stove, probably the guy living is gone somewhere. " +
                 "What a fool, leaving his house open. Next to the stove you notice a sword and a bow.");
        SetOption("You take the sword and leave.", State.TakeSword);
        SetOption("You take the bow and leave.", State.TakeBow);

        StartCoroutine(WaitAndPlayDoor());
    }

    IEnumerator WaitAndPlayBear()
    {
        yield return new WaitForSeconds(6f);
        audioSource.PlayOneShot(sfx[2], 1);
    }
    
    private void AfterHouse()
    {
        audioSource.PlayOneShot(sfx[1], 1);
        audioSource.PlayOneShot(sfx[0], 1);
        
        string startText;
        State lastState = currentState;
        switch (lastState) {
            case State.TakeSword:
                startText = "You stole the sword and left the house. The food was delicious.";
                break;
            case State.TakeBow:
                startText = "You took the bow and left the house. The food was delicious.";
                break;
            case State.ContinueWalking:
                startText = "You walked right past the house. You notice that the chimney is smoking. " +
                            "You probably made the right decision to skip making contact with people living" +
                            "in the woods.";
                break;
            default:
                startText = "What?";
                break;
        }

        StartCoroutine(WaitAndPlayBear());
        SetStory(startText + " Walking down the alley, a wild bear appears.");
        
        switch (lastState) {
            case State.TakeSword:
                SetOption("You run towards the bear to kill it", State.KillBear);
                break;
            case State.TakeBow:
                SetOption("You take out your bow to shoot it", State.ShootBear);
                break;
            case State.ContinueWalking:
                SetOption("You might go and fight the bear with your hands", State.HandsBear);
                break;
        }
        SetOption("You start running in the opposite direction", State.BearCatchesYou);
    }
    
    private void KillBear()
    {
        SetStory("You ran towards the bear and slice it in half with your might sword. GG. " +
                 "To be continued");
        SetOption(":)", State.Intro);
        SetOption("Play Again", State.Intro);
    }
    
    private void ShootBear()
    {
        SetStory("You draw out your bow and take a steady position, but you have no arrows. " +
                 "You took only a bow from the house. The bear catches you and maims you. You died.");
        SetOption(";(", State.Intro);
        SetOption("Play again", State.Intro);
    }
    
    private void BearCatchesYou()
    {
        SetStory("Sadly, you didn't watch enough National Geographic and didn't know" +
                 "that bears are faster than you when you are running uphill. You died.");
        SetOption(";(", State.Intro);
        SetOption("Play again", State.Intro);
    }
    private void Start()
    {
        ResetSelector();
        Intro();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) {
            currentSelection = (currentSelection + 1) % options.Length;
        }
        else if (Input.GetKeyDown(KeyCode.W)) {
            currentSelection = Math.Abs(currentSelection - 1) % options.Length;
        }
        
        MoveSelector(currentSelection);
        
        if (Input.GetKeyDown(KeyCode.Space)) {
            GoToState();
            ResetSelector();
        }
    }

    private void GoToState()
    {
        choicesTaken.Add(currentState);
        currentState = statesBasedOnSelectedOption[currentSelection];

        switch (currentState) {
            case State.Intro:
                Intro();
                break;
            case State.House:
                House();
                break;
            case State.ContinueWalking:
            case State.TakeBow:
            case State.TakeSword:
                AfterHouse();
                break;
            case State.KillBear:
                KillBear();
                break;
            case State.ShootBear:
                ShootBear();
                break;
            case State.BearCatchesYou:
                BearCatchesYou();
                break;
        }
    }
}
