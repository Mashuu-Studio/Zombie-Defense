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
        cam = Camera.main;
    }

    public Camera Cam { get { return cam; } }
    private Camera cam;
    private const int CAM_ORTHOGRAPHIC_MINSIZE = 4;
    private const int CAM_ORTHOGRAPHIC_MAXSIZE = 7;
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

    public void MoveCamera(Vector3 target, Vector3 pointer)
    {
        // 카메라의 위치는 target과 마우스를 기준으로 사이값으로 이동. 6:1 비율로 맞추도록 함.
        // 이 때 줌인 유무에 다라 사이값이 달라져야함.
        // MAXSIZE에 가까울 때는 target쪽으로, MINSIZE에 가까울 때는 pointer쪽으로 이동해야 함.
        // target은 5를 유지하고, pointer는 5~1까지의 범위를 가지도록 함.
        // 따라서 pointer는 1~0을 range에 맞춘 뒤 4를 곱한 뒤 1을 더해주면 됨.
        float range = CAM_ORTHOGRAPHIC_MINSIZE - CAM_ORTHOGRAPHIC_MAXSIZE;
        float current = cam.orthographicSize - CAM_ORTHOGRAPHIC_MAXSIZE;
        float targetRatio = 5;
        float pointerRatio = 1 + (current / range) * 4;

        Vector3 pos = (target * targetRatio + pointer * pointerRatio) / (targetRatio + pointerRatio);
        cam.transform.position = new Vector3(
            Mathf.Clamp(pos.x, minX, maxX),
            Mathf.Clamp(pos.y, minY, maxY),
            cam.transform.position.z);
    }

    bool zoomin;
    IEnumerator zoomCoroutine;

    public void ZoomCamera(bool zoomin)
    {
        // 현재 줌인 상태랑 들어온 상태랑 다를 경우 함수 진행
        if (this.zoomin == zoomin) return;

        // 줌인이든 아웃이든 시작하는 순간 Player의 보정 취소
        Player.Instance.Zoom(false);
        // 줌인아웃을 진행중이라면 취소 후 새롭게 진행
        if (zoomCoroutine != null)
        {
            StopCoroutine(zoomCoroutine);
            zoomCoroutine = null;
        }
        this.zoomin = zoomin;
        zoomCoroutine = Zooming();
        StartCoroutine(zoomCoroutine);
    }

    private IEnumerator Zooming()
    {
        // n초의 시간동안 줌인아웃이 완료되도록 함.
        // 이 경우는 처음부터 끝까지의 경우고 중간에서 작동해도 끝으로 이동할 수 있도록 함.
        // 줌인아웃의 속도가 끝으로 갈수록 점점 느려지도록 함.
        // PI ~ 2PI까지의 코사인그래프 활용
        // MINSIZE가 PI의 값, MAXSIZE가 2PI의 값이 되도록 범위 조절

        // acosPI + b = MIN, acos2PI + b = MAX
        // b = MIN + a = MAX - a
        // 2a = MAX - MIN
        float a = (CAM_ORTHOGRAPHIC_MAXSIZE - CAM_ORTHOGRAPHIC_MINSIZE) / 2f;
        float b = CAM_ORTHOGRAPHIC_MINSIZE + a;
        // size = acost + b
        // cos-1((size - b) / a) = t
        // 현재 t 값을 구한 뒤 cos 함수를 다시 넣어 자연스럽게 이동하도록 함.
        float t = Mathf.Acos((cam.orthographicSize - b) / a);
        // t의 범위가 0~PI로 나오기 떄문에 PI~2PI로 보정
        if (t <= Mathf.PI) t = Mathf.PI * 2 - t;
        float time = .3f;
        // PI부터 2PI까지 time초안에 이동하도록 함.
        // Time.deltaTime * PI 만큼 t가 증가하고 yield return null로 변형시키면 1초후에는 PI만큼 이동해있음.
        do
        {
            t += Time.deltaTime * Mathf.PI * ((zoomin) ? -1 : 1) / time;
            float size = a * Mathf.Cos(t) + b;
            if (zoomin && t <= Mathf.PI) size = CAM_ORTHOGRAPHIC_MINSIZE;
            if (!zoomin && t >= Mathf.PI * 2) size = CAM_ORTHOGRAPHIC_MAXSIZE;
            yield return null;
            cam.orthographicSize = size;
            SetCamera(cam);
        } while (t > Mathf.PI && t < Mathf.PI * 2);
        // 줌인이 확실하게 끝난 상황일 경우에는 Player의 보정을 true로
        if (zoomin) Player.Instance.Zoom(true);
    }
}
