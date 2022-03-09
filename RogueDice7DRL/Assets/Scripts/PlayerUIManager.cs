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
    private Vector2 bigDiceSize;
    
    void Awake()
    {
        Instance = this;
        playerUi = GameObject.Find("PlayerUI");
        diceContainer = playerUi.transform.Find("DiceUI").gameObject;
        uiDices = new List<UiDice>();
        originalDiceSize = new Vector2(40, 40);  //dicePrefab.GetComponent<RectTransform>().sizeDelta;
        bigDiceSize = new Vector2(60,60);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RefreshSortOrder() { 
    
    }

    public void AddCard() { 
        
    }

    public void EnablePlayerUi() {
        playerUi.SetActive(true);
    }

    internal void DrawInitialCards(List<ActivatableDice> dices)
    {
        for (int i = 0; i < uiDices.Count; i++)
        {
            var curr = uiDices[i];
            Destroy(curr.diceGameObject);
            curr.diceGameObject = null;
            curr.diceRef = null;
        }

        uiDices.Clear();
        foreach (Transform child in diceContainer.transform)
        {
            Destroy(child.gameObject);
        }


        for (int i = 0; i < dices.Count; i++) {
            var curr = dices[i];

            var createdInstance = Instantiate(dicePrefab, Vector3.zero, Quaternion.identity, diceContainer.transform);
            GameObject image = createdInstance.transform.Find("Image").gameObject;
            Image imageComponent = image.GetComponent<Image>();
            if (curr.chosenSide.foregroundImage == null) {
                Debug.Log("Foreground image missing for dice" + curr.diceDataRef.diceName);
            }

            if (curr.chosenSide.backgroundImage == null)
            {
                Debug.Log("Background image missing for dice" + curr.diceDataRef.diceName);
            }

            imageComponent.sprite = curr.chosenSide.foregroundImage;

            UnityAction<BaseEventData> imageHoverHandler = (baseEventData) => {
                OnImageHover(image);
            };

            AddListener(image, imageHoverHandler, EventTriggerType.PointerEnter);

            UiDice uiDice = new UiDice() {
                diceGameObject = createdInstance,
                diceRef = curr
            };

            uiDices.Add(uiDice);

        }

        ResetSortingOrder();
    }

    private void ResetSortingOrder() {
        for (int i = 0; i < uiDices.Count; i++)
        {
            var curr = uiDices[i];
            curr.diceGameObject.GetComponent<Canvas>().sortingOrder = i + 1;
        }
    }

    private void AddListener(GameObject image, UnityAction<BaseEventData> call,EventTriggerType eventTriggerType)
    {
        EventTrigger eventTriggerComponent = image.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventTriggerType;
        entry.callback.AddListener(call);
        eventTriggerComponent.triggers.Add(entry);
    }


    private void RemoveListeners(GameObject image)
    {
        EventTrigger eventTriggerComponent = image.GetComponent<EventTrigger>();
        eventTriggerComponent.triggers.Clear();
    }



    public void OnImageHover(GameObject gameObject) {

        gameObject.transform.parent.GetComponent<Canvas>().sortingOrder = 100;
        var diceTransform = gameObject.GetComponent<RectTransform>();
        diceTransform.sizeDelta = bigDiceSize;
        RemoveListeners(gameObject);
        UnityAction<BaseEventData> imageHoverHandler = (baseEventData) => {
            OnImageExit(gameObject);
        };
        AddListener(gameObject, imageHoverHandler, EventTriggerType.PointerExit);
    }


    public void OnImageExit(GameObject gameObject)
    {
        gameObject.transform.parent.GetComponent<Canvas>().sortingOrder = 1;
        var diceTransform = gameObject.GetComponent<RectTransform>();
        diceTransform.sizeDelta = originalDiceSize;
        RemoveListeners(gameObject);
        UnityAction<BaseEventData> imageHoverHandler = (baseEventData) => {
            OnImageHover(gameObject);
        };
        AddListener(gameObject, imageHoverHandler, EventTriggerType.PointerEnter);
        ResetSortingOrder();
    }
}
