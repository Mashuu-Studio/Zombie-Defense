using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MeshGenerator : MonoBehaviour
{
    public SquareGrid grid;
    private List<Vector3> vertices;
    private List<int> triangles;
    public void GenerateMesh(int[,] map, float squareSize)
    {
        grid = new SquareGrid(map, squareSize);
        vertices = new List<Vector3>();
        triangles = new List<int>();

        for (int x = 0; x < grid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < grid.squares.GetLength(1); y++)
            {
                TriangulateSquare(grid.squares[x, y]);
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private void TriangulateSquare(Square square)
    {
        /* 강의에서는 switch문으로 구현하였으나 이렇게 되면 실수의 가능성도 있고 
         * 코드 자체의 가독성도 너무 떨어지게 됨.
         * 따라서 다른 방안을 제안.
         * 
         * 기본적으로 Mesh를 만들 때 방향이 중요함.
         * 따라서 bottomLeft부터 시작 반시계 방향으로 돌아가는 형태로 제작.
         * control Node의 on/off는 비트 연산을 통해 확인할 수 있음.
         * on/off 될 때마다 양 옆 node를 !b의 형태로 on/off 함.
         * node의 양 옆이 전부 켜진다면 꺼지게 되고 하나만 켜진다면 켜지게 됨.
         * 
         * 마지막에 반시계방향으로 순차적으로 Node를 넣어준 뒤 ToArray를 통해 변환시키면
         * 방향을 맞추며 모든 상황에 하나의 코드로 동일하게 작동시킬 수 있음.
         * 다만 이 방법이 더 나은 퍼포먼스를 보여준다고는 하지 못하겠음.
         * 이는 단지 코드 가독성을 높이며 실수를 줄일 수 있게 해줌.
         */

        List<Node> nodes = new List<Node>();
        bool[] control = new bool[4]; // 0: bottomLeft, 1: bottomRight, 2: topRight, 3: topLeft
        bool[] center = new bool[4]; // 0: left, 1: bottom, 2: right, 3: top
        short mask = 1;
        for (int i = 0; i < 4; i++)
        {
            control[i] = (square.configuration & mask) != 0;

            if (control[i])
            {
                // 양 옆 노드는 같은 index와 index + 1 을 해준 값임.
                // 이 때 마지막의 경우에는 0으로 돌아가야하므로 %4를 해 줌.
                int nextSide = (i + 1) % 4; 
                center[i] = !center[i];
                center[nextSide] = !center[nextSide];
            }

            mask <<= 1; // 비트연산을 통해 한 칸씩 옆으로 밀어서 마스킹해줌. 
        }

        // 이 후 추가는 하드코딩으로 추가.
        if (control[0]) nodes.Add(square.bottomLeft);
        if (center[1]) nodes.Add(square.bottom);

        if (control[1]) nodes.Add(square.bottomRight);
        if (center[2]) nodes.Add(square.right);

        if (control[2]) nodes.Add(square.topRight);
        if (center[3]) nodes.Add(square.top);

        if (control[3]) nodes.Add(square.topLeft);
        if (center[0]) nodes.Add(square.left);

        MeshFromPoints(nodes.ToArray());

        
        /*
        switch (square.configuration)
        {
            case 0: break;

            // 1 points
            case 1:
                MeshFromPoints(square.bottom, square.bottomLeft, square.left);
                break;
            case 2:
                MeshFromPoints(square.bottom, square.bottomRight, square.right);
                break;
            case 4:
                MeshFromPoints(square.top, square.topRight, square.right);
                break;
            case 8:
                MeshFromPoints(square.top, square.topLeft, square.left);
                break;

            // 2 points
            case 3:
                MeshFromPoints(square.left, square.bottomLeft, square.right, square.bottomRight);
                break;
            case 6:
                MeshFromPoints(square.top, square.topRight, square.bottom, square.bottomRight);
                break;
            case 9:
                MeshFromPoints(square.top, square.topLeft, square.bottom, square.bottomLeft);
                break;
            case 12:
                MeshFromPoints(square.left, square.topLeft, square.right, square.topRight);
                break;
            case 5:
                MeshFromPoints(square.left, square.bottomLeft, square.bottom, square.topRight, square.top, square.right);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.left, square.bottom, square.bottomRight, square.right, square.top);
                break;

            // 3 points
            case 7:
                MeshFromPoints(square.bottomLeft, square.bottomRight, square.topRight, square.left, square.top);
                break;
            case 11:
                MeshFromPoints(square.bottomLeft, square.bottomRight, square.topLeft, square.right, square.top);
                break;
            case 13:
                MeshFromPoints(square.bottomLeft, square.topRight, square.topLeft, square.right, square.bottom);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.bottomRight, square.topRight, square.right, square.bottom);
                break;

            // 4 points
            case 15:
                MeshFromPoints(square.bottomLeft, square.bottomRight, square.topRight, square.topLeft);
                break;
        }*/
    }

    private void MeshFromPoints(params Node[] points)
    {
        AssigneVertices(points);

        if (points.Length >= 3) CreateTriangle(points[0], points[1], points[2]);
        if (points.Length >= 4) CreateTriangle(points[0], points[2], points[3]);
        if (points.Length >= 5) CreateTriangle(points[0], points[3], points[4]);
        if (points.Length >= 6) CreateTriangle(points[0], points[4], points[5]);
    }

    private void AssigneVertices(Node[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].vertextIndex == -1)
            {
                points[i].vertextIndex = vertices.Count;
                vertices.Add(points[i].position);
            }
        }
    }

    private void CreateTriangle(Node a, Node b, Node c)
    {
        triangles.Add(a.vertextIndex);
        triangles.Add(b.vertextIndex);
        triangles.Add(c.vertextIndex);
    }

    private void OnDrawGizmos()
    {/*
        if (grid != null)
        {
            for (int x = 0; x < grid.squares.GetLength(0); x++)
            {
                for (int y = 0; y < grid.squares.GetLength(1); y++)
                {
                    Gizmos.color = (grid.squares[x, y].topLeft.active) ? new Color(.65f, .45f, .4f) : new Color(.2f, .5f, .2f);
                    Gizmos.DrawCube(grid.squares[x, y].topLeft.position, Vector3.one * .4f);

                    Gizmos.color = (grid.squares[x, y].topRight.active) ? new Color(.65f, .45f, .4f) : new Color(.2f, .5f, .2f);
                    Gizmos.DrawCube(grid.squares[x, y].topRight.position, Vector3.one * .4f);

                    Gizmos.color = (grid.squares[x, y].bottomRight.active) ? new Color(.65f, .45f, .4f) : new Color(.2f, .5f, .2f);
                    Gizmos.DrawCube(grid.squares[x, y].bottomRight.position, Vector3.one * .4f);

                    Gizmos.color = (grid.squares[x, y].bottomLeft.active) ? new Color(.65f, .45f, .4f) : new Color(.2f, .5f, .2f);
                    Gizmos.DrawCube(grid.squares[x, y].bottomLeft.position, Vector3.one * .4f);

                    Gizmos.color = new Color(.425f, .475f, .3f);
                    Gizmos.DrawCube(grid.squares[x, y].top.position, Vector3.one * .15f);
                    Gizmos.DrawCube(grid.squares[x, y].right.position, Vector3.one * .15f);
                    Gizmos.DrawCube(grid.squares[x, y].bottom.position, Vector3.one * .15f);
                    Gizmos.DrawCube(grid.squares[x, y].left.position, Vector3.one * .15f);
                }
            }
        }*/
    }

    public class SquareGrid
    {
        public Square[,] squares;

        public SquareGrid(int[,] map, float squareSize)
        {
            int countX = map.GetLength(0);
            int countY = map.GetLength(1);
            float width = countX * squareSize;
            float height = countY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[countX, countY];

            for (int x = 0; x < countX; x++)
            {
                for (int y = 0; y < countY; y++)
                {
                    Vector3 pos = new Vector3(x - countX / 2 + .5f, y - countY / 2 + .5f) * squareSize;
                    controlNodes[x, y] = new ControlNode(pos, map[x, y] != 1, squareSize);
                }
            }

            squares = new Square[countX - 1, countY - 1];

            for (int x = 0; x < countX - 1; x++)
            {
                for (int y = 0; y < countY - 1; y++)
                {
                    squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
                }
            }
        }
    }
    public class Square
    {
        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node top, right, bottom, left;
        public int configuration;

        public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomRight, ControlNode bottomLeft)
        {
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomRight = bottomRight;
            this.bottomLeft = bottomLeft;

            top = topLeft.right;
            right = bottomRight.above;
            bottom = bottomLeft.right;
            left = bottomLeft.above;

            if (topLeft.active) configuration += 8;
            if (topRight.active) configuration += 4;
            if (bottomRight.active) configuration += 2;
            if (bottomLeft.active) configuration += 1;
        }
    }
    public class Node
    {
        public Vector3 position;
        public int vertextIndex = -1;

        public Node(Vector3 pos)
        {
            position = pos;
        }
    }

    public class ControlNode : Node
    {
        public bool active;
        public Node above, right;

        public ControlNode(Vector3 pos, bool active, float sqaureSize) : base(pos)
        {
            this.active = active;
            above = new Node(pos + Vector3.up * sqaureSize / 2f);
            right = new Node(pos + Vector3.right * sqaureSize / 2f);
        }
    }
}
