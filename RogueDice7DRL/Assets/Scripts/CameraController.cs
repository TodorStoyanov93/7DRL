using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject cameraGameObject;
    Camera cameraComponent;
    private float defaultCameraZ;
    public static CameraController Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
        cameraGameObject = GameObject.Find("Main Camera");
        cameraComponent = cameraGameObject.GetComponent<Camera>();
        defaultCameraZ = cameraGameObject.transform.position.z;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SnapTo(GameObject target) {
        var targetPosition = target.transform.position;
        var cameraTarget = ToCameraPosition(targetPosition);
        Instance.cameraGameObject.transform.position = cameraTarget;
    }

    public IEnumerator FollowSmooth(GameObject unit, float duration)
    {
        float timeElapsed = 0f;
        Vector3 start = cameraGameObject.transform.position;
        while (timeElapsed <= duration)
        {

            var lerpedValue = Vector3.Lerp(start, unit.transform.position, timeElapsed / duration);

            cameraGameObject.transform.position = ToCameraPosition(lerpedValue);
            yield return null;
            timeElapsed += Time.deltaTime;
        }
        cameraGameObject.transform.position = ToCameraPosition(unit.transform.position);

        yield break;
    }

    public static Vector3 ToCameraPosition(Vector3 position) {
        return new Vector3(position.x, position.y, Instance.defaultCameraZ);
    }

}
