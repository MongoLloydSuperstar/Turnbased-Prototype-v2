using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelEngine : MonoBehaviour
{
    private World _world = new World();
    private System.Random _random = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        var chunkGameObject = new GameObject("Chunk 0, 0, 0");
        chunkGameObject.transform.parent = transform.parent;

        var chunk = chunkGameObject.AddComponent<Chunk>();

        _world.Chunks.Add(new ChunkId(0,0,0), chunk);
    }

    // Update is called once per frame
    void Update()
    {
        var x = _random.Next(0, 16);
        var y = _random.Next(0, 16);
        var z = _random.Next(0, 16);
        var voxelType = (UInt16) _random.Next(0, 2);

        _world[x, y, z] = voxelType;
    }
}
