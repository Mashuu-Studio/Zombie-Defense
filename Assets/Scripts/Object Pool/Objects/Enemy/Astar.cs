using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar : MonoBehaviour
{
    private AstarNode[,] info;
    private Vector2Int dest;
    private IEnumerator updatePathCoroutine;
    private Transform player;

    private static Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(-1,1),
        new Vector2Int(0,1),
        new Vector2Int(1,1),
        new Vector2Int(-1,0),
        new Vector2Int(1,0),
        new Vector2Int(-1,-1),
        new Vector2Int(0,-1),
        new Vector2Int(1,-1),
    };

    public void SetMap(int[,] map)
    {
        info = new AstarNode[map.GetLength(0), map.GetLength(1)];

        for (int x = 0; x < map.GetLength(0); x++)
            for (int y = 0; y < map.GetLength(1); y++)
                info[x, y] = new AstarNode(new Vector2Int(x, y), map[x, y] == MapGenerator.WALL);

        // Path 재설정을 위한 dest 변환
        dest = dest + Vector2Int.one;
    }

    // 이동은 다음 위치와 그 다음 위치만 있으면 충분함.
    public List<Vector2Int> FindPath(Vector2 start)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int startMapPos = MapGenerator.ConvertToMapPos(MapGenerator.RoundToInt(start));
        
        if (startMapPos.x < 0 || startMapPos.x >= info.GetLength(0) ||
            startMapPos.y < 0 || startMapPos.y >= info.GetLength(1)) return path;
        if (dest.x < 0 || dest.x >= info.GetLength(0) ||
            dest.y < 0 || dest.y >= info.GetLength(1)) return path;

        AstarNode startNode = info[startMapPos.x, startMapPos.y];
        AstarNode destNode = info[dest.x, dest.y];

        if (startNode.isWall || destNode.isWall) return path;

        AstarNode curNode = startNode;
        // 1. 현재 위치 추가
        path.Add(MapGenerator.ConvertToWorldPos(curNode.pos)); 
        // 그 다음 위치와 다음다음 위치까지만 추가
        for (int i = 0; i < 2 && curNode.parentNode != null; i++)
        {
            curNode = curNode.parentNode;
            Vector2Int pos = MapGenerator.ConvertToWorldPos(curNode.pos);
            if (path.Contains(pos)) break;
            path.Add(pos);
        }
        return path;
    }

    public void UpdateMapPath(Vector2 dest)
    {
        Vector2Int destMapPos = MapGenerator.ConvertToMapPos(MapGenerator.RoundToInt(dest));
        if (destMapPos.x < 0 || destMapPos.x >= info.GetLength(0) ||
            destMapPos.y < 0 || destMapPos.y >= info.GetLength(1)) return;
        if (this.dest == destMapPos) return;

        this.dest = destMapPos;
        if (updatePathCoroutine != null)
        {
            StopCoroutine(updatePathCoroutine);
            updatePathCoroutine = null;
        }
        updatePathCoroutine = UpdatePath();
        StartCoroutine(updatePathCoroutine);
    }

    IEnumerator UpdatePath()
    {
        AstarNode destNode = info[dest.x, dest.y];

        // 목적지를 openList에 넣어둔 뒤 모든 방향을 돌며 전체 노드를 업데이트 함.
        List<AstarNode> openList = new List<AstarNode>() { destNode };
        List<AstarNode> closedList = new List<AstarNode>();

        int count = 0;
        while (openList.Count > 0)
        {
            AstarNode curNode = openList[0];
            // 해당 조건문은 F가 작지만 H가 큰 특별한 상황이 있을 수는 있음.
            // 그러나 이 경우는 H 자체가 잘못 체크된 경우이며 2D에서는 고려할 필요가 없으므로
            // 해당 조건문을 그대로 활용.
            for (int i = 1; i < openList.Count; i++)
                if (curNode.F >= openList[i].F && curNode.H > openList[i].H)
                    curNode = openList[i];

            openList.Remove(curNode);
            closedList.Add(curNode);

            // 주변 노드를 openList에 추가하는 과정
            foreach (var dir in directions)
            {
                Vector2Int neighborPos = curNode.pos + dir;
                // 맵 밖이거나, 해당 방향이 벽이거나,
                // 이미 closedList에 있으면 스킵
                if (neighborPos.x < 0 || neighborPos.x >= info.GetLength(0) ||
                    neighborPos.y < 0 || neighborPos.y >= info.GetLength(1) ||
                    info[neighborPos.x, neighborPos.y].isWall ||
                    closedList.Contains(info[neighborPos.x, neighborPos.y])) continue;

                // 대각선 방향으로 이동할 때 수직 수평 방향에 벽이 있는지 체크
                // 수직수평 방향 이동의 경우 이미 위에서 걸러지기 때문에 상관X
                if (info[curNode.pos.x, neighborPos.y].isWall &&
                    info[neighborPos.x, curNode.pos.y].isWall) continue;

                AstarNode neighborNode = info[neighborPos.x, neighborPos.y];
                int moveCost = curNode.G + ((curNode.pos.x == neighborPos.x || curNode.pos.y == neighborPos.y) ? 10 : 14);

                if (moveCost < neighborNode.G || !openList.Contains(neighborNode))
                {
                    neighborNode.G = moveCost;
                    neighborNode.H = (Mathf.Abs(neighborPos.x - dest.x) + Mathf.Abs(neighborPos.y - dest.y)) * 10;
                    neighborNode.parentNode = curNode;

                    openList.Add(neighborNode);
                }
                count++;
                if (count > 400)
                {
                    count %= 400;
                    yield return null;
                }
            }
        }

    }
    /*
    public List<Vector2Int> FindPath(Vector2 start, Vector2 dest)
    {
        AstarNode[,] info = new AstarNode[this.info.GetLength(0), this.info.GetLength(1)];

        for (int x = 0; x < info.GetLength(0); x++)
            for (int y = 0; y < info.GetLength(1); y++)
                info[x, y] = new AstarNode(new Vector2Int(x, y), this.info[x, y].isWall);

        Vector2Int startMapPos = MapGenerator.ConvertToMapPos(Vector2Int.RoundToInt(start));
        Vector2Int destMapPos = MapGenerator.ConvertToMapPos(Vector2Int.RoundToInt(dest));

        AstarNode startNode = info[startMapPos.x, startMapPos.y];
        AstarNode destNode = info[destMapPos.x, destMapPos.y];

        List<AstarNode> openList = new List<AstarNode>() { startNode };
        List<AstarNode> closedList = new List<AstarNode>();
        List<Vector2Int> path = new List<Vector2Int>();

        while (openList.Count > 0)
        {
            AstarNode curNode = openList[0];
            // 해당 조건문은 F가 작지만 H가 큰 특별한 상황이 있을 수는 있음.
            // 그러나 이 경우는 H 자체가 잘못 체크된 경우이며 2D에서는 고려할 필요가 없으므로
            // 해당 조건문을 그대로 활용.
            for (int i = 1; i < openList.Count; i++)
                if (curNode.F >= openList[i].F && curNode.H > openList[i].H)
                    curNode = openList[i];

            openList.Remove(curNode);
            closedList.Add(curNode);
            if (curNode == destNode)
            {
                AstarNode targetCurNode = destNode;
                while (targetCurNode != startNode)
                {
                    path.Add(MapGenerator.ConvertToWorldPos(targetCurNode.pos));
                    targetCurNode = targetCurNode.parentNode;
                }
                path.Add(MapGenerator.ConvertToWorldPos(startNode.pos));
                path.Reverse();
                return path;
            }

            // 주변 노드를 openList에 추가하는 과정
            foreach (var dir in directions)
            {
                Vector2Int neighborPos = curNode.pos + dir;
                // 맵 밖이거나, 해당 방향이 벽이거나,
                // 이미 closedList에 있으면 스킵
                if (neighborPos.x < 0 || neighborPos.x >= info.GetLength(0) ||
                    neighborPos.y < 0 || neighborPos.y >= info.GetLength(1) ||
                    info[neighborPos.x, neighborPos.y].isWall ||
                    closedList.Contains(info[neighborPos.x, neighborPos.y])) continue;

                // 대각선 방향으로 이동할 때 수직 수평 방향에 벽이 있는지 체크
                // 수직수평 방향 이동의 경우 이미 위에서 걸러지기 때문에 상관X
                if (info[curNode.pos.x, neighborPos.y].isWall &&
                    info[neighborPos.x, curNode.pos.y].isWall) continue;

                AstarNode neighborNode = info[neighborPos.x, neighborPos.y];
                int moveCost = curNode.G + ((curNode.pos.x == neighborPos.x || curNode.pos.y == neighborPos.y) ? 10 : 14);

                if (moveCost < neighborNode.G || !openList.Contains(neighborNode))
                {
                    neighborNode.G = moveCost;
                    neighborNode.H = (Mathf.Abs(neighborPos.x - destMapPos.x) + Mathf.Abs(neighborPos.y - destMapPos.y)) * 10;
                    neighborNode.parentNode = curNode;

                    openList.Add(neighborNode);
                }
            }
        }
        return path;
    }*/
}

public class AstarNode
{
    public AstarNode parentNode;
    public int F { get { return G + H; } }
    public int G, H;
    public Vector2Int pos;
    public bool isWall;

    public AstarNode(Vector2Int pos, bool isWall)
    {
        this.pos = pos;
        this.isWall = isWall;
    }
}