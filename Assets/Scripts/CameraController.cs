using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform hoop;
    [SerializeField] private Transform player;
    [SerializeField] private float rotationSpeed;

    private float sceneWidth = 10;

    private void Update()
    {
        float unitsPerPixel = sceneWidth / Screen.width;

        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

        Camera.main.orthographicSize = desiredHalfHeight;
    }

    private void LateUpdate()
    {
        

        if (InputManager.Instance.isItScore)
        {
            Vector3 finalPos = hoop.position;
            finalPos.y = 7.5f;
            finalPos.z -= 10;

            Vector3 finalRot = new Vector3(10, 0, 0);

            transform.position = Vector3.Lerp(transform.position, finalPos, Time.deltaTime * rotationSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(finalRot), Time.deltaTime * rotationSpeed);
        }
        else
        {
            Vector3 A = new Vector3(player.position.x, 0, player.position.z);
            Vector3 B = new Vector3(hoop.position.x, 0, hoop.position.z);
            Vector3 dir = (B - A).normalized * 10;
            Vector3 camPos = player.position - dir;
            camPos.y = 7.5f;
            transform.position = Vector3.Lerp(transform.position, camPos, 0.125f);

            Vector3 rot = player.rotation.eulerAngles;
            rot.x = 20;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rot), 0.125f);

        }
    }
}
