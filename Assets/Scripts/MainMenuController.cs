using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Button[] availableButtons;
    [SerializeField] private SoundController soundController;
    [SerializeField] private MasterController masterController;
    [SerializeField] private AudioClip optionChangeSound;
    [SerializeField] private AudioClip optionSelectSound;

    private Button activeButton;
    private TMP_Text activeText;
    private int selectedIndex = 0;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SetActiveButton();
    }

    private void Update()
    {
        if(Input.GetKeyDown("up")) {
            if(selectedIndex > 0){
                selectedIndex -= 1;
            } else {
                selectedIndex = availableButtons.Length - 1;
            }
            masterController.soundController.PlaySound(optionChangeSound, 0.2f);
            SetActiveButton();
        } else if(Input.GetKeyDown("down")) {
            if(selectedIndex < (availableButtons.Length - 1)) {
                selectedIndex += 1;
            } else {
                selectedIndex = 0;
            }
            masterController.soundController.PlaySound(optionChangeSound, 0.2f);
            SetActiveButton();
        }

        if(Input.GetKeyDown("return")) {
            activeButton.onClick.Invoke();
            masterController.soundController.PlaySound(optionSelectSound, 0.2f);
        }
    }

    private void SetActiveButton()
    {
        activeButton = availableButtons[selectedIndex];
        activeText = activeButton.GetComponentInChildren<TMP_Text>();
        activeText.color = Color.white;

        foreach (Button optButton in availableButtons) {
            if(optButton != activeButton) {
                TMP_Text buttonText = optButton.GetComponentInChildren<TMP_Text>();
                buttonText.color = Color.grey;
            }
        }
    }

    public void PlayGame()
    {
        masterController.StartLevel();
    }

    public void ShowInstructions()
    {
        masterController.ShowInstructionsPanel();
    }

    public void ShowOptions()
    {
        masterController.ShowOptionsPanel();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Button overButton = eventData.pointerCurrentRaycast.gameObject.transform.parent.GetComponent<Button>();
        selectedIndex = System.Array.IndexOf(availableButtons,overButton);
        SetActiveButton();
    }
}
