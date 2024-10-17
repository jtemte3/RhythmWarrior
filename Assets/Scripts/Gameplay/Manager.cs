using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Manager : MonoBehaviour
{
    public enum Difficulty { easy, medium, hard }
    [Header("Difficulty")]
    public Difficulty difficulty = Difficulty.medium;
    [Header("Gameplay")]
    public float score;
    public bool isSongActive = false;
    public float menuTimer = 5.0f;
    private float showMenuTime;
    [Header("Song Setup")]
    public Playlist playlist;
    public AudioSource source;
    public Spawner spawner;
    [Header("UI")]
    public TMP_Text lbl_score;
    public GameObject mainMenuUI;
    [Header("Interactors")]
    public GameObject leftInteractor;
    public GameObject rightInteractor;
    public GameObject leftHandle;
    public GameObject rightHandle;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        lbl_score.text = string.Format(FormatDefinitions.scoreFormat, score);

        if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primaryButton, out bool AButton))
        {
            if (AButton)
            {
                SceneManager.LoadScene(0);
            }
        }

        if (isSongActive)
        {
            if (Time.time > showMenuTime)
            {
                mainMenuUI.SetActive(true);
                lbl_score.gameObject.SetActive(false);
                leftHandle.SetActive(false);
                rightHandle.SetActive(false);
                EnableInteractor(leftInteractor);
                EnableInteractor(rightInteractor);
                spawner.isSongActive = false;
                isSongActive = false;
            }
        }
    }

    public void IncreaseScore(float points)
    {
        score += points;
    }

    public void DecreaseScore(float points)
    {
        score -= points;
    }

    public void PickASongAndStart()
    {
        //Reset Score
        score = 0;

        //Pick a song
        SongProfile currentSong = playlist.songlist[Random.Range(0, playlist.songlist.Count)];

        if (difficulty == Difficulty.easy)
        {
            spawner.beat = (60 / currentSong.bpm) * currentSong.easySpeed;
        }
        if (difficulty == Difficulty.medium)
        {
            spawner.beat = (60 / currentSong.bpm) * currentSong.medSpeed;
        }
        if (difficulty == Difficulty.hard)
        {
            spawner.beat = (60 / currentSong.bpm) * currentSong.hardSpeed;
        }

        spawner.endTime = (Time.time + currentSong.song.length) - currentSong.stopSpawningTime;

        showMenuTime = (Time.time + currentSong.song.length) - currentSong.stopSpawningTime + menuTimer;

        source.clip = currentSong.song;

        //Disable UI Elements and Enable Saber handles
        mainMenuUI.SetActive(false);
        lbl_score.gameObject.SetActive(true);
        leftHandle.SetActive(true);
        rightHandle.SetActive(true);
        DisableInteractor(leftInteractor);
        DisableInteractor(rightInteractor);

        //Play the song
        source.Play();
        spawner.isSongActive = true;
        isSongActive = true;
    }

    private void DisableInteractor(GameObject interactor)
    {
        interactor.GetComponent<XRRayInteractor>().enabled = false;
        interactor.GetComponent<XRInteractorLineVisual>().enabled = false;
        interactor.GetComponent<LineRenderer>().enabled = false;
    }
    private void EnableInteractor(GameObject interactor)
    {
        interactor.GetComponent<XRRayInteractor>().enabled = true;
        interactor.GetComponent<XRInteractorLineVisual>().enabled = true;
        interactor.GetComponent<LineRenderer>().enabled = true;
    }
}
