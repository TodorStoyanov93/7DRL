using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{

    public static GameplayController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }


    public IEnumerator SmoothMove(GameObject gameObject,Vector2 desiredPosition, float duration)
    {
        float timeElapsed = 0f;
        Vector3 start = gameObject.transform.position;
        while (timeElapsed <= duration)
        {

            var lerpedValue = Vector3.Lerp(start, desiredPosition, timeElapsed / duration);

            gameObject.transform.position = lerpedValue;
            yield return null;
            timeElapsed += Time.deltaTime;
        }

        gameObject.transform.position = desiredPosition;

        yield break;
    }

}
