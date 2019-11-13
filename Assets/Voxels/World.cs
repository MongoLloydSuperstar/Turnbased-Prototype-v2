using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Voxels
{
    public class World : MonoBehaviour
    {
        public int worldSize = 2;
        public int chunkResolution = 16;
        public Material material;

        private static Dictionary<string, Chunk> _chunks;

        private void Start()
        {
            _chunks = new Dictionary<string, Chunk>();

            for (int z = 0; z < worldSize; z++)
            for (int y = 0; y < worldSize; y++)
            for (int x = 0; x < worldSize; x++)
            {
                BuildChunkAt(new Vector3(x, y, z));
            }
        }

        private void BuildChunkAt(Vector3 abstractPos)
        {
            Vector3 worldPos = new Vector3(
                abstractPos.x * chunkResolution,
                abstractPos.y * chunkResolution,
                abstractPos.z * chunkResolution);
            
            
            GameObject o = new GameObject(BuildChunkName(abstractPos));
            o.transform.position = worldPos;
            o.transform.parent = transform;
            Chunk chunk = o.AddComponent<Chunk>();
            chunk.Initialize(abstractPos, chunkResolution, material);

            _chunks.Add(o.name, chunk);
        }

        public static string BuildChunkName(Vector3 v)
        {
            return v.x + ", " + v.y + ", " + v.z;
        }

        public static Chunk GetChunk(string chunkName)
        {
            if (_chunks.TryGetValue(chunkName, out var c))
            {
                return c;
            }

            return null;
        }
    }
}