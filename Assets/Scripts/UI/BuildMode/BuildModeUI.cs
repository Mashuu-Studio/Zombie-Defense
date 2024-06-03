using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildModeUI : MonoBehaviour
{
    [SerializeField] private RectTransform buildingIconScrollRectTransform;
    [SerializeField] private BuildModeItemIcon buildingIconPrefab;
    private List<BuildModeItemIcon> buildingIcons = new List<BuildModeItemIcon>();

    [Space]
    [SerializeField] private BuildModeItemIcon[] companionIcons;

    private void Awake()
    {
        buildingIconPrefab.gameObject.SetActive(false);
    }

    public void Init()
    {
        buildingIcons.ForEach(buildingIcon => Destroy(buildingIcon.gameObject));
        buildingIcons.Clear();

        int count = 0;
        foreach (var building in BuildingManager.Buildings)
        {
            var buildingIcon = Instantiate(buildingIconPrefab, buildingIconScrollRectTransform);
            buildingIcon.Init(building.key);
            buildingIcon.gameObject.SetActive(true);
            buildingIcons.Add(buildingIcon);
            count++;
        }
    }

    public void BuildMode(bool b)
    {
        gameObject.SetActive(b);
        if (b)
        {
            selectedCompanionIndex = -1;
            UpdateCompanions();
        }
    }

    private int selectedCompanionIndex;
    private Vector2 companionPatrolStartPos;
    private Vector2 companionPatrolEndPos;

    private void Update()
    {
        if (GameController.Instance.GameStarted == false
            || GameController.Instance.Pause) return;

        float axisX = Input.GetAxis("Horizontal");
        float axisY = Input.GetAxis("Vertical");
        Vector3 movePos = CameraController.Instance.Cam.transform.position + new Vector3(axisX, axisY) * Time.deltaTime * 10;
        CameraController.Instance.MoveCamera(movePos, movePos);


        Vector3 mousePos = CameraController.Instance.Cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pos = MapGenerator.PosToGrid(MapGenerator.RoundToInt(mousePos));
        BuildingController.Instance.MoveBuildingPointer(pos);

        if (selectedCompanionIndex != -1)
        {
            if (Input.GetMouseButtonDown(0))
            {
                companionPatrolStartPos = pos;
            }

            if (Input.GetMouseButtonUp(0) && !UIController.PointOverUI())
            {
                companionPatrolEndPos = pos;
                CompanionController.Instance.SetCompanionPatrol(selectedCompanionIndex, new List<Vector2>() { companionPatrolStartPos, companionPatrolEndPos });
                selectedCompanionIndex = -1;
                BuildingController.Instance.SelectBuildingOnBuildMode("");
            }
        }
        // 패트롤 세팅 중에는 터렛 빌드가 안 되도록
        else
        {
            // 터렛 구매 및 빌드
            if (Input.GetMouseButton(0) && !UIController.PointOverUI())
            {
                BuildingController.Instance.Build(pos);
            }

            // 터렛 보관
            if (Input.GetMouseButton(1))
            {
                BuildingController.Instance.Store(pos);
            }

            // 마운트
            if (Input.GetKeyDown(KeyCode.Q) && BuildingController.Instance.SelectBuilding(pos))
            {
                // 마운트로 바로 넘어가는 게 아닌 Floating Dropdown을 띄움.
                UIController.Instance.ShowMountWeaponUI(true, pos);
            }
        }
    }

    public void SelectCompanion(BuildModeItemIcon icon)
    {
        selectedCompanionIndex = -1;
        for (int i = 0; i < companionIcons.Length; i++)
        {
            if (companionIcons[i] == icon)
            {
                selectedCompanionIndex = i;
                CompanionController.Instance.SetCompanionPatrol(selectedCompanionIndex, new List<Vector2>() { companionPatrolStartPos, companionPatrolEndPos });
                BuildingController.Instance.SelectBuildingOnBuildMode(CompanionController.Instance.Companions[selectedCompanionIndex].Key);
                // 빌드 아이콘 이미지 바꿈
                // 빌드 컨트롤러에서 변경해야함
                break;
            }
        }
    }

    public void UpdateCompanions()
    {
        foreach (var icon in companionIcons) icon.gameObject.SetActive(false);

        for (int i = 0; i < CompanionController.Instance.Companions.Count; i++)
        {
            var data = CompanionController.Instance.Companions[i];
            companionIcons[i].gameObject.SetActive(true);
            companionIcons[i].Init(data.Key);
        }
    }
}
