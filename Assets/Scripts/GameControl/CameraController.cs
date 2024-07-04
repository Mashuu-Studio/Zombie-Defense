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
        cam.allowDynamicResolution = false;
    }

    public Camera Cam { get { return cam; } }
    private Camera cam;
    private const int CAM_ORTHOGRAPHIC_MINSIZE = 3;
    private const int CAM_ORTHOGRAPHIC_MAXSIZE = 4;
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
        // ī�޶��� ��ġ�� target�� ���콺�� �������� ���̰����� �̵�. 6:1 ������ ���ߵ��� ��.
        // �� �� ���� ������ �ٶ� ���̰��� �޶�������.
        // MAXSIZE�� ����� ���� target������, MINSIZE�� ����� ���� pointer������ �̵��ؾ� ��.
        // target�� 5�� �����ϰ�, pointer�� 5~1������ ������ �������� ��.
        // ���� pointer�� 1~0�� range�� ���� �� 4�� ���� �� 1�� �����ָ� ��.
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
        // ���� ���� ���¶� ���� ���¶� �ٸ� ��� �Լ� ����
        if (this.zoomin == zoomin) return;

        // �����̵� �ƿ��̵� �����ϴ� ���� Player�� ���� ���
        Player.Instance.Zoom(false);
        // ���ξƿ��� �������̶�� ��� �� ���Ӱ� ����
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
        // n���� �ð����� ���ξƿ��� �Ϸ�ǵ��� ��.
        // �� ���� ó������ �������� ���� �߰����� �۵��ص� ������ �̵��� �� �ֵ��� ��.
        // ���ξƿ��� �ӵ��� ������ ������ ���� ���������� ��.
        // PI ~ 2PI������ �ڻ��α׷��� Ȱ��
        // MINSIZE�� PI�� ��, MAXSIZE�� 2PI�� ���� �ǵ��� ���� ����

        // acosPI + b = MIN, acos2PI + b = MAX
        // b = MIN + a = MAX - a
        // 2a = MAX - MIN
        float a = (CAM_ORTHOGRAPHIC_MAXSIZE - CAM_ORTHOGRAPHIC_MINSIZE) / 2f;
        float b = CAM_ORTHOGRAPHIC_MINSIZE + a;
        // size = acost + b
        // cos-1((size - b) / a) = t
        // ���� t ���� ���� �� cos �Լ��� �ٽ� �־� �ڿ������� �̵��ϵ��� ��.
        float t = Mathf.Acos((cam.orthographicSize - b) / a);
        // t�� ������ 0~PI�� ������ ������ PI~2PI�� ����
        if (t <= Mathf.PI) t = Mathf.PI * 2 - t;
        float time = .3f;
        // PI���� 2PI���� time�ʾȿ� �̵��ϵ��� ��.
        // Time.deltaTime * PI ��ŭ t�� �����ϰ� yield return null�� ������Ű�� 1���Ŀ��� PI��ŭ �̵�������.
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
        // ������ Ȯ���ϰ� ���� ��Ȳ�� ��쿡�� Player�� ������ true��
        if (zoomin) Player.Instance.Zoom(true);
    }
}
