using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckpointsAndLaps : MonoBehaviour
{
    [Header("Points")]
    public GameObject start;
    public GameObject end;
    public GameObject[] checkpoints;

    [Header("Settings")]
    public int laps = 1;
    public LayerMask rayCastLayerMask;
    public GameObject FinishMenu;
    public GameObject PlayerUI;
    public GameObject MenuController;

    [Header("Information")]
    private float currentCheckpoint;
    private GameObject currentCheckpointObject = null;
    private int currentLapNumber = 0;
    private bool started;
    private bool finished;

    private float currentLapTime;
    private float bestLapTime;
    private float bestLap;

    private float totalRaceTime;

    [Header("UI (Multiple Allowed)")]
    public List<Text> currentLapTimeTexts = new List<Text>();
    public List<Text> bestLapTimeTexts = new List<Text>();
    public List<Text> currentLapTexts = new List<Text>();
    public List<Text> totalRaceTimeTexts = new List<Text>();

    private void Start()
    {
        currentCheckpoint = 0;
        currentLapNumber = 1;

        started = false;
        finished = false;

        currentLapTime = 0;
        bestLapTime = 0;
        bestLap = 0;
        totalRaceTime = 0;

        UpdateAllUI();
    }

    public void Respawn()
    {
        if (currentCheckpointObject != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(currentCheckpointObject.transform.position, Vector3.down, out hit, 100, rayCastLayerMask))
            {
                transform.position = hit.point;
                transform.rotation = currentCheckpointObject.transform.rotation;

                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }
    }

    private void Update()
    {
        if (started && !finished)
        {
            currentLapTime += Time.deltaTime;
            totalRaceTime += Time.deltaTime;
            UpdateAllUI();
        }

        if (Input.GetKey(KeyCode.R))
        {
            Respawn();
            Debug.Log("Respawned at current checkpoint");
        }
    }

    private void UpdateAllUI()
    {
        string currentTime = $"{Mathf.FloorToInt(currentLapTime / 60)}:{currentLapTime % 60:00.00}";
        string bestTime = (bestLapTime > 0)
            ? $"{Mathf.FloorToInt(bestLapTime / 60)}:{bestLapTime % 60:00.00}"
            : "0:00.00";
        string lapText = $"Lap {currentLapNumber}/{laps}";
        string totalTime = $"{Mathf.FloorToInt(totalRaceTime / 60)}:{totalRaceTime % 60:00.00}";

        foreach (var text in currentLapTimeTexts)
            if (text != null) text.text = currentTime;

        foreach (var text in bestLapTimeTexts)
            if (text != null) text.text = bestTime;

        foreach (var text in currentLapTexts)
            if (text != null) text.text = lapText;

        foreach (var text in totalRaceTimeTexts)
            if (text != null) text.text = totalTime;
    }

    public void MenuToggle()
    {
        FinishMenu.SetActive(!FinishMenu.activeSelf);
        PlayerUI.SetActive(!FinishMenu.activeSelf);
        Time.timeScale = FinishMenu.activeSelf ? 0f : 1f;
        Destroy(MenuController);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Checkpoint")) return;

        GameObject thisCheckpoint = other.gameObject;
        currentCheckpointObject = thisCheckpoint;

        if (thisCheckpoint == start && !started)
        {
            started = true;
            Debug.Log("Started");
        }
        else if (thisCheckpoint == end && started)
        {
            if (currentCheckpoint == checkpoints.Length)
            {
                // Update best lap time BEFORE resetting or finishing
                if (currentLapTime < bestLapTime || bestLapTime == 0)
                {
                    bestLap = currentLapNumber;
                    bestLapTime = currentLapTime;
                    UpdateAllUI();
                }

                if (currentLapNumber == laps)
                {
                    finished = true;
                    Debug.Log("Finished");
                    MenuToggle();
                }
                else
                {
                    currentLapNumber++;
                    currentCheckpoint = 0;
                    currentLapTime = 0;
                    Debug.Log($"Started lap {currentLapNumber}");
                }
            }
            else
            {
                Debug.Log("Did not go through all checkpoints");
            }
        }

        for (int i = 0; i < checkpoints.Length; i++)
        {
            if (finished) return;

            if (thisCheckpoint == checkpoints[i] && i + 1 == currentCheckpoint + 1)
            {
                currentCheckpoint++;
                Debug.Log($"Correct Checkpoint: {Mathf.FloorToInt(currentLapTime / 60)}:{currentLapTime % 60:00.00}");
            }
            else if (thisCheckpoint == checkpoints[i])
            {
                Debug.Log("Incorrect checkpoint");
            }
        }

        UpdateAllUI();
    }
}
