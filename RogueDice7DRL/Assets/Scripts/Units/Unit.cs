using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit
{
    public GameObject gameObject { get; private set; }

    public void SetGameobject(GameObject gameObject) {
        this.gameObject = gameObject;
        if (this.gameObject.transform.childCount > 0)
        {

            var frame = this.gameObject.transform.GetChild(0).GetChild(0);
            var healthText = this.gameObject.transform.GetChild(0).GetChild(1);
            healthText.GetComponent<Renderer>().sortingLayerID = frame.GetComponent<Renderer>().sortingLayerID;
            healthText.GetComponent<TextMesh>().text = currentHealth.ToString() + "/" + maxHealth.ToString();
            var hpbarContainer = this.gameObject.transform.GetChild(0).GetChild(2);

            if (currentHealth == 0) {
                hpbarContainer.transform.localScale = new Vector3(0, 1, 1);
            } else {
                hpbarContainer.transform.localScale = new Vector3(1/(maxHealth/currentHealth),1,0);
            }
            var shieldText = this.gameObject.transform.GetChild(0).GetChild(3).GetChild(0);
            shieldText.GetComponent<Renderer>().sortingLayerID = frame.GetComponent<Renderer>().sortingLayerID;
            shieldText.GetComponent<Renderer>().sortingOrder = 5;

            if (shield > 0)
            {
                shieldText.GetComponent<TextMesh>().text = shield.ToString();
                shieldText.parent.gameObject.SetActive(true);
            } else {
                shieldText.parent.gameObject.SetActive(false);
            }
        }
    }
    public void SetHealth(int health)
    {
        currentHealth = health;
        if (gameObject != null && gameObject.transform.childCount > 0)
        {
            var healthText = this.gameObject.transform.GetChild(0).GetChild(1);
            healthText.GetComponent<TextMesh>().text = currentHealth.ToString() + "/" + maxHealth.ToString();
            var hpbarContainer = this.gameObject.transform.GetChild(0).GetChild(2);
            if (currentHealth == 0)
            {
                hpbarContainer.transform.localScale = new Vector3(0, 1, 1);
            }
            else
            {
                hpbarContainer.transform.localScale = new Vector3(1f / ((float)maxHealth / (float)currentHealth), 1, 0);
            }
        }
    }

    public void SetShield(int shield)
    {
        this.shield = shield;
        if (shield > 0)
        {
            var shieldText = this.gameObject.transform.GetChild(0).GetChild(3).GetChild(0);
            shieldText.GetComponent<TextMesh>().text = shield.ToString();
            shieldText.parent.gameObject.SetActive(true);
        }
        else {
            var shieldText = this.gameObject.transform.GetChild(0).GetChild(3).GetChild(0);
            shieldText.parent.gameObject.SetActive(false);
        }
    }

    public int maxHealth = 10;
    public int currentHealth { get; private set; }
    public int shield { get; private set; }

    

    public List<ActivatableDice> dices = new List<ActivatableDice>();
    public abstract void Turn();



    public Vector2Int GetVector2IntPosition() {
        return Helpers.RoundToVector2Int(gameObject.transform.position);
    }

}
