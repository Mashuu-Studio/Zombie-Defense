using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get { return instance; } }
    private static CameraController instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetCamera(Camera.main);
    }

    public Camera Cam { get { return cam; } }
    private Camera cam;
    private float minX, maxX, minY, maxY;

    public void SetCamera(Camera cam)
    {
        this.cam = cam;
        var halfHeight = cam.orthographicSize;
        var halfWidth = halfHeight * cam.aspect;

        var bound = MapGenerator.Instance.MapBounds;
        minX = bound.min.x + halfWidth;
        minY = bound.min.y + halfHeight;
        maxX = bound.max.x - halfWidth;
        maxY = bound.max.y - halfHeight;
    }

    public void MoveCamera(Vector3 target)
    {
        var diff = target - cam.transform.position;
        cam.transform.position = new Vector3(
            Mathf.Clamp(cam.transform.position.x + diff.x, minX, maxX),
            Mathf.Clamp(cam.transform.position.y + diff.y, minY, maxY),
            cam.transform.position.z);
    }
}
