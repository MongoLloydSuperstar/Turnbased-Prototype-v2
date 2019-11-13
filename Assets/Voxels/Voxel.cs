using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voxels
{
    [Serializable]
    public class Voxel
    {
        public Vector3 Position { get; }
        public Vector3Int AbstractPos { get; }        // Abstract position in the Chunk
        public bool State { get; }

        public Voxel(Vector3 position, Vector3Int abstractPos, bool state)
        {
            AbstractPos = abstractPos;
            Position = position;
            State = state;
        }

        public VoxelData GetVoxelData()
        {
            return new VoxelData(Position.ToString(), Position, State);
        }
        
        
        
    }
}