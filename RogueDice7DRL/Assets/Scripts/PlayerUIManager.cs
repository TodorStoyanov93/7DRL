using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public GameObject dicePrefab;

    public static PlayerUIManager Instance;

    private GameObject playerUi;
    private GameObject diceContainer;

    public List<UiDice> uiDices;
    private Vector2 originalDiceSize;
    private Vector2 originalDiceOffset;

    private Vector2 hoveredDiceSize;
    private Vector2 hoveredDiceOffset;

    private Vector2 clickedDiceSize;
    private Vector2 clickedDiceOffset;

    private UiDice currentClickedDice;

    void Awake()
    {
        Instance = this;
        playerUi = GameObject.Find("PlayerUI");
        diceContainer = playerUi.transform.Find("DiceUI").gameObject;
        uiDices = new List<UiDice>();
        originalDiceSize = dicePrefab.transform.Find("ImagesContainer").GetComponent<RectTransform>().sizeDelta;
        originalDiceOffset = dicePrefab.transform.Find("ImagesContainer").GetComponent<RectTransform>().anchoredPosition;

        hoveredDiceSize = new Vector2(60,60);
        hoveredDiceOffset = new Vector2(0, 10);

        clickedDiceSize = new Vector2(70, 70);
        clickedDiceOffset = new Vector2(0, 15);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EnablePlayerUi() {
        playerUi.SetActive(true);
    }

    internal void ResetCardView()
    {
        List<ActivatableDice> dices = BoardManager.Instance.playerUnit.dices;
        for (int i = 0; i < uiDices.Count; i++)
        {
            var curr = uiDices[i];
            Destroy(curr.diceGameObject);
            curr.diceGameObject = null;
            curr.activatableDice = null;
        }

        uiDices.Clear();
        foreach (Transform child in diceContainer.transform)
        {
            Destroy(child.gameObject);
        }


        for (int i = 0; i < dices.Count; i++) {
            var curr = dices[i];

            var createdInstance = Instantiate(dicePrefab, Vector3.zero, Quaternion.identity, diceContainer.transform);
            GameObject imageContainer = createdInstance.transform.Find("ImagesContainer").gameObject;
            
            if (curr.chosenSide.foregroundImage == null) {
                Debug.Log("Foreground image missing for dice" + curr.diceDataRef.diceName);
            }

            var foregroundImageGo = imageContainer.transform.Find("ForegroundImage");
            var foregroundImageImage = foregroundImageGo.GetComponent<Image>();
            foregroundImageImage.sprite = curr.chosenSide.foregroundImage;

            if (curr.chosenSide.backgroundImage == null)
            {
                Debug.Log("Background image missing for dice" + curr.diceDataRef.diceName);
            }

            var backgroundImageGo = imageContainer.transform.Find("BackgroundImage");
            var backgroundImageImage = backgroundImageGo.GetComponent<Image>();
            backgroundImageImage.sprite = curr.chosenSide.backgroundImage;

            var activeImageGo = imageContainer.transform.Find("ActiveImage");
            var activeImageImage = activeImageGo.GetComponent<Image>();
            if (curr.isActive) { 
                activeImageImage.color = Color.green;
            } else {
                activeImageImage.color = new Color(0, 0, 0, 0);
            }

            UiDice uiDice = new UiDice() {
                diceGameObject = createdInstance,
                activatableDice = curr,
                state = UiDiceState.Normal
            };

            uiDices.Add(uiDice);
            SetDiceByState(uiDice);
        }

        ResetSortingOrderForAllDice();
    }



    private void SetDiceByState(UiDice uiDice) {

        switch (uiDice.state)
        {
            case UiDiceState.Undefined:
                RemoveListeners(uiDice);
                break;
            case UiDiceState.Normal:
                { 
                    RemoveListeners(uiDice);
                    UnityAction<BaseEventData> imageHoverHandler = (baseEventData) => {
                        uiDice.state = UiDiceState.Hovered;
                        SetDiceByState(uiDice);
                    };
                    AddListener(uiDice, imageHoverHandler, EventTriggerType.PointerEnter);

                    var goRef = uiDice.diceGameObject;
                    var imageTransform = goRef.transform.Find("ImagesContainer").GetComponent<RectTransform>();
                    imageTransform.sizeDelta = originalDiceSize;
                    imageTransform.anchoredPosition = originalDiceOffset;
                    
                }
                break;
            case UiDiceState.Hovered:
                { 
                    RemoveListeners(uiDice);
                    UnityAction<BaseEventData> imageExitHandler = (baseEventData) => {
                        uiDice.state = UiDiceState.Normal;
                        SetDiceByState(uiDice);
                        ResetSortingOrderForAllDice();
                    };
                    AddListener(uiDice, imageExitHandler, EventTriggerType.PointerExit);
                    UnityAction<BaseEventData> imageClickHandler = (baseEventData) =>
                    {
                        PointerEventData pointerEventData = baseEventData as PointerEventData;
                        var isLeftClick = pointerEventData.button == PointerEventData.InputButton.Left;
                        if (isLeftClick) { 
                            if (uiDice.activatableDice.isActive)
                            {
                                SetAllDiceUnclicked();
                                uiDice.state = UiDiceState.Clicked;
                                SetDiceByState(uiDice);
                                (BoardManager.Instance.playerUnit as PlayerControlledUnit).DiceClicked(uiDice.activatableDice);
                            }
                            else
                            {
                                //Write msg
                                uiDice.state = UiDiceState.Clicked;
                                SetDiceByState(uiDice);
                            }
                        }
                    };
                    AddListener(uiDice, imageClickHandler, EventTriggerType.PointerClick);


                    var goRef = uiDice.diceGameObject;
                    var imageTransform = goRef.transform.Find("ImagesContainer").GetComponent<RectTransform>();
                    imageTransform.sizeDelta = hoveredDiceSize;
                    imageTransform.anchoredPosition = hoveredDiceOffset;

                    ResetSortingOrderForAllDice();
                    goRef.GetComponent<Canvas>().sortingOrder = 100;
                }
                break;
            case UiDiceState.Clicked:
                {
                    RemoveListeners(uiDice);

                    var goRef = uiDice.diceGameObject;
                    var imageTransform = goRef.transform.Find("ImagesContainer").GetComponent<RectTransform>();
                    imageTransform.sizeDelta = clickedDiceSize;
                    imageTransform.anchoredPosition = clickedDiceOffset;

                    ResetSortingOrderForAllDice();
                    goRef.GetComponent<Canvas>().sortingOrder = 100;

                    currentClickedDice = uiDice;
                }
                break;
            default:
                break;
        }
    }

    public void SetAllDiceUnclicked() {
        foreach (var dice in uiDices) {
            dice.state = UiDiceState.Normal;
            SetDiceByState(dice);
            ResetSortingOrderForAllDice();
        }
    }

    public void CancelCurrentDice() {
        currentClickedDice.state = UiDiceState.Normal;
        SetDiceByState(currentClickedDice);
        ResetSortingOrderForAllDice();
    }

    private void ResetSortingOrderForAllDice() {
        for (int i = 0; i < uiDices.Count; i++)
        {
            var curr = uiDices[i];
            curr.diceGameObject.GetComponent<Canvas>().sortingOrder = i + 1;
        }
    }

    private void AddListener(UiDice uiDice, UnityAction<BaseEventData> call,EventTriggerType eventTriggerType)
    {
        GameObject imageGo = uiDice.diceGameObject.transform.Find("ImagesContainer").gameObject;

        EventTrigger eventTriggerComponent = imageGo.GetComponent<EventTrigger>();

        EventTrigger.Entry entry = eventTriggerComponent.triggers.Find(e => e.eventID == eventTriggerType);
        if (entry == null)
        {
            entry = new EventTrigger.Entry { eventID = eventTriggerType };
            eventTriggerComponent.triggers.Add(entry);
        }

        entry.callback.AddListener(call);
        eventTriggerComponent.triggers.Add(entry);
    }


    private void RemoveListeners(UiDice uiDice)
    {
        GameObject image = uiDice.diceGameObject.transform.Find("ImagesContainer").gameObject;
        EventTrigger eventTriggerComponent = image.GetComponent<EventTrigger>();
        eventTriggerComponent.triggers.ForEach(i => i.callback.RemoveAllListeners());
    }
}
