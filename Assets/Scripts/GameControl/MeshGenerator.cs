using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public SquareGrid grid;
    private List<Vector3> vertices;
    private List<int> triangles;
    private Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();
    private List<List<int>> outlines = new List<List<int>>();
    private HashSet<int> checkedVertices = new HashSet<int>();

    [SerializeField] private PolygonCollider2D mapCollider;

    public void GenerateMesh(int[,] map, float squareSize)
    {
        triangleDictionary.Clear();
        outlines.Clear();
        checkedVertices.Clear();

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
        CreateWallCollider();
    }

    private void CreateWallCollider()
    {
        CalculateMeshOutlines();
        mapCollider.pathCount = outlines.Count;

        for (int pathIndex = 0; pathIndex < outlines.Count; pathIndex++)
        {
            List<int> outline = outlines[pathIndex];
            Vector2[] points = new Vector2[outline.Count];
            for (int i = 0; i < outline.Count; i++)
                points[i] = vertices[outline[i]];

            mapCollider.SetPath(pathIndex, points);
        }
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
        if (square.configuration == 15)
        {
            checkedVertices.Add(square.topLeft.vertextIndex);
            checkedVertices.Add(square.topRight.vertextIndex);
            checkedVertices.Add(square.bottomRight.vertextIndex);
            checkedVertices.Add(square.bottomLeft.vertextIndex);
        }
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

        Triangle triangle = new Triangle(a.vertextIndex, b.vertextIndex, c.vertextIndex);
        AddTriangleDictinary(triangle.vertexIndexA, triangle);
        AddTriangleDictinary(triangle.vertexIndexB, triangle);
        AddTriangleDictinary(triangle.vertexIndexC, triangle);
    }

    private void AddTriangleDictinary(int vertexIndexKey, Triangle triangle)
    {
        if (triangleDictionary.ContainsKey(vertexIndexKey))
            triangleDictionary[vertexIndexKey].Add(triangle);
        else
        {
            triangleDictionary.Add(vertexIndexKey, new List<Triangle>());
            triangleDictionary[vertexIndexKey].Add(triangle);
        }
    }

    private void CalculateMeshOutlines()
    {
        for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++)
        {
            if (!checkedVertices.Contains(vertexIndex))
            {
                int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
                if (newOutlineVertex != -1)
                {
                    checkedVertices.Add(vertexIndex);

                    List<int> newOutline = new List<int>();
                    newOutline.Add(vertexIndex);
                    outlines.Add(newOutline);
                    FollowOutline(newOutlineVertex, outlines.Count - 1);
                    outlines[outlines.Count - 1].Add(vertexIndex);
                }
            }
        }
    }

    private void FollowOutline(int vertexIndex, int outlineIndex)
    {
        outlines[outlineIndex].Add(vertexIndex);
        checkedVertices.Add(vertexIndex);
        int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

        if (nextVertexIndex != -1)
        {
            FollowOutline(nextVertexIndex, outlineIndex);
        }
    }

    private int GetConnectedOutlineVertex(int vertexIndex)
    {
        List<Triangle> triangles = triangleDictionary[vertexIndex];

        for (int i = 0; i < triangles.Count; i++)
        {
            Triangle triangle = triangles[i];

            for (int j = 0; j < 3; j++)
            {
                int vertexB = triangle[j];
                if (vertexB == vertexIndex || checkedVertices.Contains(vertexB)) continue;
                if (IsOutlineEdge(vertexIndex, vertexB))
                {
                    return vertexB;
                }
            }
        }

        return -1;
    }

    private bool IsOutlineEdge(int vertexA, int vertexB)
    {
        List<Triangle> trianglesA = triangleDictionary[vertexA];
        int sharedTriCount = 0;

        for (int i = 0; i < trianglesA.Count; i++)
        {
            if (trianglesA[i].Contains(vertexB))
            {
                sharedTriCount++;
                if (sharedTriCount > 1) break;
            }
        }

        return sharedTriCount == 1;
    }

    struct Triangle
    {
        public int vertexIndexA;
        public int vertexIndexB;
        public int vertexIndexC;
        int[] vertices;

        public Triangle(int a, int b, int c)
        {
            vertexIndexA = a;
            vertexIndexB = b;
            vertexIndexC = c;

            vertices = new int[] { vertexIndexA, vertexIndexB, vertexIndexC };
        }

        public int this[int i] { get { return vertices[i]; } }

        public bool Contains(int vertexIndex)
        {
            return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
        }
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
