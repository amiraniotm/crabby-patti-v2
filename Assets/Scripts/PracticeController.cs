using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PracticeController : MonoBehaviour
{
    [SerializeField] private ToggleGroup enemiesToggle;
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private TMP_Text exitText;
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;

    private MasterController masterController;
    private float colorChangespeed = 1.0f;
    

    private void Awake()
    {
        masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        masterController.SetPracticeController(this);
        exitText.color = startColor;
        StartCoroutine(ChangeTextColorCoroutine());
    }

    private void Update()
    {
        pointsText.text = masterController.pointsCount.ToString("0");
    }

    public void OnToggleChange()
    {
        foreach(Toggle toggle in enemiesToggle.ActiveToggles()) {
            string enemyName = toggle.name.Replace("Toggle", "");
            masterController.currentLevel.SetPracticeEnemies(enemyName);
        }
    }

    private IEnumerator ChangeTextColorCoroutine()
    {
        float tick = 0f;

        if(exitText.color == startColor) {
            while (exitText.color != endColor) {
                tick += Time.deltaTime * colorChangespeed;
                exitText.color = Color.Lerp(startColor, endColor, tick);
                yield return null;
            }
            StartCoroutine(ChangeTextColorCoroutine());
        } else if(exitText.color == endColor) {
            while (exitText.color != startColor) {
                tick += Time.deltaTime * colorChangespeed;
                exitText.color = Color.Lerp(endColor, startColor, tick);
                yield return null;
            }
            StartCoroutine(ChangeTextColorCoroutine());
        }
    }
}
