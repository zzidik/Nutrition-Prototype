using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PuppetJump.Interactables;
using DG.Tweening;

public class PlayerStation : MonoBehaviour
{
    /*
    [Serializable]
    public class DisplayInfo
    {
        public Text question;
        public Image correctMark;
        public Image incorrectMark;
    }
    */

    private Tween question_Tween;
    private AudioSource question_Audio;
    private Color white = new Color(1f, 1f, 1f, 1f);

    private Tween correctMark_Tween;
    private AudioSource correctMark_Audio;
    private Color correctMark_StartColor;
    private Color green = new Color(0f, 1f, 0f, 1f);

    private Tween incorrectMark_Tween;
    private AudioSource incorrectMark_Audio;
    private Color incorrectMark_StartColor;
    private Color red = new Color(1f, 0f, 0f, 1f);

    private float tweenSpeed = 0.5f;
    private float tweenDelay = 0.5f;

    /// <summary>
    /// The PlayerStation ID number, assigned by the GameManager.
    /// </summary>
    public int IDNum = 0;
    /// <summary>
    /// Reference to the GameManager
    /// </summary>
    public GameManager gameManager;
    /// <summary>
    /// A Lane the PlayerStation is linked to.
    /// </summary>
    public Lane lane;

    public GameObject selectLevelMenu;
    public InGameDisplay inGameDisplay;

    /*
    /// <summary>
    /// Displays information.
    /// </summary>
    public DisplayInfo display;
    */


    /// <summary>
    /// A list of VolumeSelectors for the Player to choose from.
    /// </summary>
    public List<VolumeSelector> volumeSelectors;
    /// <summary>
    /// An array with four integers that tell the VolumeSectors
    /// which volume to display. -1 displays no volume.
    /// </summary>
    public int[] displayVolumes = new int[4];

    /// <summary>
    /// The quiz this PlayerStation is currently taking.
    /// </summary>
    public List<GameManager.QAVol> quiz = new List<GameManager.QAVol>();

    public int level = 0;

    /// <summary>
    /// The question number from a quiz this PlayerStation is on.
    /// </summary>
    public int question = 0;

    /// <summary>
    /// Initializes the displayVolumes.
    /// </summary>
    private void OnEnable()
    {
        displayVolumes[0] = -1;
        displayVolumes[1] = -1;
        displayVolumes[2] = -1;
        displayVolumes[3] = -1;
    }

    /// <summary>
    /// Intializes the VolumeSelectors to display no volume at the start.
    /// Deactivate all VolumeSlectors.
    /// </summary>
    private void Start()
    {
        SetVolumeSelectors(new int[] { -1, -1, -1, -1 });

        ActivateVolumeSelectors(false);

        question_Audio = inGameDisplay.question.GetComponent<AudioSource>();
        correctMark_Audio = inGameDisplay.correctMark.GetComponent<AudioSource>();
        incorrectMark_Audio = inGameDisplay.incorrectMark.GetComponent<AudioSource>();

        correctMark_StartColor = inGameDisplay.correctMark.color;
        incorrectMark_StartColor = inGameDisplay.incorrectMark.color;

    }

    public void SelectLevel(int lvl)
    {
        level = lvl;
    }

    public void StartLevel()
    {
        gameManager.LoadLevel(level);
        selectLevelMenu.SetActive(false);
        inGameDisplay.gameObject.SetActive(true);
        inGameDisplay.levelCompleteButtons.SetActive(false);
    }

    /// <summary>
    /// Displays the proper volume for the question.
    /// </summary>
    /// <param name="vols"></param>
    public void SetVolumeSelectors(int[] vols)
    {
        int numVolSelectors = volumeSelectors.Count;
        for (int vs = 0; vs < numVolSelectors; vs++)
        {
            volumeSelectors[vs].SetDisplayVolume(vols[vs]);
        }
    }

    /// <summary>
    /// Called by one of the VolumeSelectors when it is touched.
    /// </summary>
    /// <param name="whichVol">The index of the VolumeSelector the call came from.</param>
    public void VolumeTouched(int whichVol)
    {
        if (quiz[question].answer == whichVol)
        {
            // CORRECT ANSWER
            ActivateVolumeSelectors(false);

            lane.OpenNextWayPoint();

            inGameDisplay.question.text = "Correct";
            inGameDisplay.question.color = green;

            correctMark_Tween?.Kill();
            correctMark_Tween = inGameDisplay.correctMark.DOColor(green, tweenSpeed).OnComplete(() => CorrectAnswerSelected(whichVol));
            correctMark_Audio.Play();

            GetVolumeSelectorByDisplayVolume(whichVol).TweenPlateColor_Correct();
        }
        else
        {
            // WRONG ANSWER
            ActivateVolumeSelectors(false);

            if (whichVol < quiz[question].answer)
            {
                inGameDisplay.question.text = "Too Small";
            }
            else if (whichVol > quiz[question].answer)
            {
                inGameDisplay.question.text = "Too Large";
            }

            inGameDisplay.question.color = red;

            incorrectMark_Tween?.Kill();
            incorrectMark_Tween = inGameDisplay.incorrectMark.DOColor(red, tweenSpeed).OnComplete(() => WrongAnswerSelected(whichVol));
            incorrectMark_Audio.Play();

            GetVolumeSelectorByDisplayVolume(whichVol).TweenPlateColor_Incorrect();
        }
    }

    private void CorrectAnswerSelected(int whichVol)
    {
        correctMark_Tween?.Kill();
        question_Tween?.Kill(); 

        int numClues = quiz.Count;

        // if there is a next question
        if (question < numClues - 1)
        {
            correctMark_Tween = inGameDisplay.correctMark.DOColor(correctMark_StartColor, tweenSpeed).SetDelay(tweenDelay).OnComplete(() => LoadNextQuestion(whichVol));
        }
        else
        {
            // if there is no next question
            // the player has finished the level
            correctMark_Tween = inGameDisplay.correctMark.DOColor(correctMark_StartColor, tweenSpeed).SetDelay(tweenDelay).OnComplete(() => LevelComplete(whichVol));
        }
    }

    private void WrongAnswerSelected(int whichVol)
    {
        // take the question the player got wrong and add it to the end of the quiz
        quiz.Add(quiz[question]);

        incorrectMark_Tween?.Kill();
        
        incorrectMark_Tween = inGameDisplay.incorrectMark.DOColor(incorrectMark_StartColor, tweenSpeed).SetDelay(tweenDelay).OnComplete(() => LoadNextQuestion(whichVol));
    }

    /// <summary>
    /// Loads the next question of the quiz.
    /// </summary>
    public void LoadNextQuestion(int whichVol)
    {
        inGameDisplay.question.color = white;

        // load next question
        question++;
        SetVolumeSelectors(quiz[question].displayVols);
        inGameDisplay.question.text = quiz[question].question;

        // reactive the touch on VolumeSelectors
        ActivateVolumeSelectors(true);

        GetVolumeSelectorByDisplayVolume(whichVol).TweenPlateColor_Normal();
    }

    private void LevelComplete(int whichVol)
    {
        inGameDisplay.question.color = white;
        inGameDisplay.question.text = "Level Complete";
        question_Audio.Play();

        GetVolumeSelectorByDisplayVolume(whichVol).TweenPlateColor_Normal();

        inGameDisplay.levelCompleteButtons.SetActive(true);
    }

    public void OpenSelectLevelMenu()
    {
        selectLevelMenu.SetActive(true);
        inGameDisplay.gameObject.SetActive(false);
        inGameDisplay.levelCompleteButtons.SetActive(false);
    }

    /// <summary>
    /// Activates or deactivates the touch and point on the VolumeSelectors.
    /// </summary>
    /// <param name="active">True if the VolumeSelectors are touchable.</param>
    public void ActivateVolumeSelectors(bool active)
    {
        int numVolSelectors = volumeSelectors.Count;
        for(int vs = 0; vs < numVolSelectors; vs++)
        {
            if (active)
            {
                volumeSelectors[vs].GetComponent<Touchable>().isTouchable = true;
                volumeSelectors[vs].GetComponent<Touchable>().isPointable = true;
            }
            else
            {
                volumeSelectors[vs].GetComponent<Touchable>().isTouchable = false;
                volumeSelectors[vs].GetComponent<Touchable>().isPointable = false;
            }
        }
    }

    /// <summary>
    /// Gets a VolumeSelector for the list of volumeSelectors that matchs the selected volume.
    /// Need this because the order they are displayed in game mighht differe from the order
    /// in the PlayerStation list of volumeSelectors.
    /// </summary>
    /// <param name="whichVol"></param>
    /// <returns></returns>
    private VolumeSelector GetVolumeSelectorByDisplayVolume(int whichVol)
    {
        VolumeSelector volumeSelector = null;
        int numVolSel = volumeSelectors.Count;
        for (int vs = 0; vs < numVolSel; vs++)
        {
            if (volumeSelectors[vs].displayVolume == whichVol)
            {
                volumeSelector = volumeSelectors[vs];
            }
        }

        return volumeSelector;
    }
}
