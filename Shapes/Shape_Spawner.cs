using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Shape_Spawner : MonoBehaviour
{
    List<Shape> _shapes = new();
    Tilemap _spawner;
    Transform _shapeParent;
    GameObject _shapePrefab;
    Sprite _nextShape;
    [SerializeField] Tile _blockTile;

    public void Start()
    {
        _shapeParent = GameObject.Find("ShapeParent").transform;
        _shapePrefab = Resources.Load<GameObject>("Prefabs/ShapePrefab");
        _spawner = GetComponent<Tilemap>();
        CreateShapes();
    }

    void CreateShapes()
    {
        List<Vector3Int> blockVectors = new List<Vector3Int>
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(1, 1, 0)
        };
        Shape Block = new Shape(ShapeName.Block, blockVectors);
        _shapes.Add(Block);

        List<Vector3Int> lVectors = new List<Vector3Int>
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, 2, 0),
            new Vector3Int(1, 2, 0)
        };
        Shape L = new Shape(ShapeName.L, lVectors);
        _shapes.Add(L);

        List<Vector3Int> lReverseVectors = new List<Vector3Int>
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(1, 2, 0),
            new Vector3Int(0, 2, 0)
        };
        Shape L_Reverse = new Shape(ShapeName.L_Reversed, lReverseVectors);
        _shapes.Add(L_Reverse);

        List<Vector3Int> lineVectors = new List<Vector3Int>
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, 2, 0),
            new Vector3Int(0, 3, 0)
        };
        Shape Line = new Shape(ShapeName.Line, lineVectors);
        _shapes.Add(Line);

        List<Vector3Int> tVectors = new List<Vector3Int>
        {
            new Vector3Int(0, 1, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(2, 1, 0),
            new Vector3Int(1, 0, 0)
        };
        Shape T = new Shape(ShapeName.T, tVectors);
        _shapes.Add(T);

        List<Vector3Int> zVectors = new List<Vector3Int>
        {
            new Vector3Int(0, 1, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(2, 0, 0)
        };
        Shape Z = new Shape(ShapeName.Z, zVectors);
        _shapes.Add(Z);

        List<Vector3Int> zReverseVectors = new List<Vector3Int>
        {
            new Vector3Int(2, 1, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 0, 0)
        };
        Shape Z_Reverse = new Shape(ShapeName.Z_Reverse, zReverseVectors);
        _shapes.Add(Z_Reverse);
    }

    public Shape_Controller SpawnShape(Tilemap tilemap, Vector3Int direction)
    {
        BoundsInt bounds = _spawner.cellBounds;
        TileBase[] allTiles = _spawner.GetTilesBlock(bounds);

        List<Vector3Int> validTilePositions = new List<Vector3Int>();

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x + bounds.x, y + bounds.y, bounds.z);
                TileBase tile = _spawner.GetTile(tilePosition);

                if (tile != null)
                {
                    validTilePositions.Add(tilePosition);
                }
            }
        }

        if (validTilePositions.Count > 0)
        {
            Vector3Int randomTilePosition = validTilePositions[UnityEngine.Random.Range(0, validTilePositions.Count)];
            Shape randomShape = _shapes[Random.Range(0, _shapes.Count)];

            GameObject shapeObject = new GameObject("ShapeController");
            Shape_Controller controller = shapeObject.AddComponent<Shape_Controller>();

            List<Vector3Int> initialShape = new List<Vector3Int>();

            foreach (Vector3Int relativePosition in randomShape.ShapeVectors)
            {
                Vector3Int actualPosition = randomTilePosition + relativePosition;
                tilemap.SetTile(actualPosition + direction, _blockTile);
                initialShape.Add(actualPosition);
            }

            controller.CreateShape(direction, tilemap, initialShape, _blockTile);
            return controller;
        }

        return null;
    }
}

public enum ShapeName
{
    Block,
    L,
    L_Reversed,
    Line,
    T,
    Z,
    Z_Reverse
}

public class Shape
{
    public ShapeName ShapeName;
    public List<Vector3Int> ShapeVectors = new();

    public Shape(ShapeName name, List<Vector3Int> shapeVectors)
    {
        ShapeName = name;
        ShapeVectors = shapeVectors;    
    }
}
