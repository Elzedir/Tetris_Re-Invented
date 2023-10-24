using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level_Manager : MonoBehaviour
{
    public static Level_Manager Instance;
    [SerializeField] Vector3Int _levelDirection; public Vector3Int LevelDirection { get { return _levelDirection; } }
    [SerializeField] float _spawnTime; public float SpawnTime { get { return _spawnTime; } }
    List<Color> _levelColours = new(); public List<Color> LevelColours { get { return _levelColours; } }

    float _timer;
    bool _shapeInPlay;
    public Shape_Spawner Spawner { get; private set; }
    public Tilemap ShapeTilemap { get; private set; }
    public Tilemap BorderTilemap { get; private set; }

    Shape_Controller _currentController;

    void Start()
    {
        Spawner = null;
        _shapeInPlay = false;
        _timer = 0;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        if (GameObject.Find("Spawner").TryGetComponent<Shape_Spawner>(out Shape_Spawner spawner))
        {
            Spawner = spawner;
        }
        else
        {
            Debug.Log("Spawner not found");
        }

        if (GameObject.Find("Shapes").TryGetComponent<Tilemap>(out Tilemap shapeTilemap))
        {
            ShapeTilemap = shapeTilemap;
        }
        else
        {
            Debug.Log("Shape Tilemap not found");
        }

        if (GameObject.Find("Border").TryGetComponent<Tilemap>(out Tilemap borderTilemap))
        {
            BorderTilemap = borderTilemap;
        }
        else
        {
            Debug.Log("Shape Tilemap not found");
        }
    }

    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= SpawnTime && !_shapeInPlay)
        {
            if (!ShapeTilemap || LevelDirection == Vector3Int.zero)
            {
                Debug.Log($"Spawner: {ShapeTilemap} or LevelDirection: {LevelDirection} is zero");
                return;
            }

            _currentController = Spawner.SpawnShape(ShapeTilemap, LevelDirection);
            _shapeInPlay = true;

            _timer = 0;
        }

        if (_currentController != null && _currentController.ShapePlaced)
        {
            Destroy(_currentController.gameObject);
            _shapeInPlay = false;
        }
    }

    public void SetLevelColours(List<Color> newLevelColours)
    {
        LevelColours.Clear();
        _levelColours = newLevelColours;
    }

    public void SetLevelSpeed(float newSpawnTime)
    {
        _spawnTime = newSpawnTime;
    }

    public void SpawnShape()
    {

    }
}
