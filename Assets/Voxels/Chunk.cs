using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Random = System.Random;

namespace Voxels
{
    public struct VoxelData
    {
        public string name;
        public Vector3 position;
        public bool state;

        public VoxelData(string name, Vector3 position, bool state)
        {
            this.name = name;
            this.position = position;
            this.state = state;
        }
    }


    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Chunk : MonoBehaviour
    {
        private readonly int[][] _cubeTriangles =
        {
            new[] {5, 6, 4}, new[] {5, 7, 6}, // FRONT
            new[] {0, 3, 1}, new[] {0, 2, 3}, // BACK
            new[] {2, 7, 3}, new[] {2, 6, 7}, // TOP
            new[] {1, 4, 0}, new[] {1, 5, 4}, // BOTTOM
            new[] {1, 7, 5}, new[] {1, 3, 7}, // RIGHT
            new[] {4, 2, 0}, new[] {4, 6, 2}  // LEFT
        };

        private readonly int[] _cubeTrianglesNotGrouped =
        {
            5, 6, 4, 5, 7, 6, // FRONT
            0, 3, 1, 0, 2, 3, // BACK
            2, 7, 3, 2, 6, 7, // TOP
            1, 4, 0, 1, 5, 4, // BOTTOM
            1, 7, 5, 1, 3, 7, // RIGHT
            4, 2, 0, 4, 6, 2  // LEFT
        };

        private Vector3 _abstractPos;               // Abstract position in the World grid
        private int _resolution;
        private Voxel[] _voxels;

        private Mesh _mesh;
        private List<Vector3> _vertices;
        private List<Vector3> _realVertices;        // Real vertices are connected to an actual voxel in the Chunk
        private List<Vector3> _implicitVertices;    // Implicit are positions in that don't have a voxel in the current Chunk

        private Material _material;

        private bool isCubeDone;
        

        public void Initialize(Vector3 vPosition, int resolution, Material material)
        {
            _abstractPos = vPosition;
            _resolution = resolution;
            _voxels = new Voxel[_resolution * _resolution * _resolution];
            _realVertices = new List<Vector3>();
            _implicitVertices = new List<Vector3>();

            InitializeVoxels();

            GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
            GetComponent<MeshRenderer>().material = material;
            _mesh.name = "Chunk Mesh";
            
            // Puts implicit vertices after the real vertices 
            _vertices = _realVertices;
            _vertices.AddRange(_implicitVertices);
            _mesh.vertices = _vertices.ToArray();
            
            Triangulate();
        }

        private void InitializeVoxels()
        {
            Random rand = new Random();
            int i = 0;
            for (int z = 0; z < _resolution; z++)
            for (int y = 0; y < _resolution; y++)
            for (int x = 0; x < _resolution; x++, i++)
            {
                bool rndBool = rand.NextDouble() > 0.5;
                
                _voxels[i] = new Voxel(new Vector3(x, y, z), new Vector3Int(x, y, z), true);
                _realVertices.Add(_voxels[i].Position);
                
                if (x == _resolution - 1 || y == _resolution - 1 || z == _resolution - 1)
                    AddImplicitVoxel(_voxels[i].Position);
            }
        }

        private void AddImplicitVoxel(Vector3 vPos)
        {
            int ix = Mathf.RoundToInt(vPos.x);
            int iy = Mathf.RoundToInt(vPos.y);
            int iz = Mathf.RoundToInt(vPos.z);

            if (ix == _resolution - 1)
                ix++;
            if (iy == _resolution - 1)
                iy++;
            if (iz == _resolution - 1)
                iz++;

            _implicitVertices.Add(new Vector3(ix, iy, iz));
        }
        
        private void AddImplicitVoxelSorted(Vector3 vPos)
        {
            int ix = Mathf.RoundToInt(vPos.x);
            int iy = Mathf.RoundToInt(vPos.y);
            int iz = Mathf.RoundToInt(vPos.z);

            if (ix == _resolution - 1)
                ix++;
            if (iy == _resolution - 1)
                iy++;
            if (iz == _resolution - 1)
                iz++;

            // TODO: Sort positions in _implicitVertices
        }
        
        private void Triangulate()
        {
            List<int> chunkTriangles = new List<int>();

            int i = 0;
            for (int z = 0; z < _resolution; z++)
            for (int y = 0; y < _resolution; y++)
            for (int x = 0; x < _resolution; x++, i++)
            {
                if (x >= _resolution - 1 || y >= _resolution - 1 || z >= _resolution - 1) continue;
                if (!_voxels[i].State) continue;
                chunkTriangles.AddRange(TriangulateCell(i));
            }

            _mesh.triangles = chunkTriangles.ToArray();
        }

        private List<int> TriangulateCell(int vi)
        {
            List<int> ts = new List<int>();
            int[] offsets = new int[8];

            offsets[0] = vi;
            offsets[1] = vi + 1;
            offsets[2] = vi + _resolution;
            offsets[3] = vi + _resolution + 1;
            offsets[4] = vi + _resolution * _resolution;
            offsets[5] = vi + _resolution * _resolution + 1;
            offsets[6] = vi + _resolution * _resolution + _resolution;
            offsets[7] = vi + _resolution * _resolution + _resolution + 1;


            bool[] states = GetVoxelNeighbourStates(vi);

            for (var i = 0; i < _cubeTriangles.Length; i++)
            {
                if (states[i / 2]) continue;
                var triangles = _cubeTriangles[i];
                int[] offsetTriangles =
                {
                    offsets[triangles[0]],
                    offsets[triangles[1]],
                    offsets[triangles[2]]
                };

                ts.AddRange(offsetTriangles);
            }

            return ts;
        }

        private bool[] GetVoxelNeighbourStates(int voxelIndex)
        {
            
            bool[] states = {false, false, false, false, false, false};
            Voxel originVoxel = _voxels[voxelIndex];

            // Gets the state of neighbours except for the last voxels in all x, y, z
            // since they have no neighbouring voxels to which it connect itself
            if (originVoxel.Position.z < _resolution - 2)
                states[0] = _voxels[voxelIndex + _resolution * _resolution].State;

            if (originVoxel.Position.z > 0)
                states[1] = _voxels[voxelIndex - _resolution * _resolution].State;

            if (originVoxel.Position.y < _resolution - 2)
                states[2] = _voxels[voxelIndex + _resolution].State;

            if (originVoxel.Position.y > 0)
                states[3] = _voxels[voxelIndex - _resolution].State;

            if (originVoxel.Position.x < _resolution - 2)
                states[4] = _voxels[voxelIndex + 1].State;

            if (originVoxel.Position.x > 0)
                states[5] = _voxels[voxelIndex - 1].State;

            return states;
        }


        private Voxel GetStateInNeighbourX(int y, int z)
        {
            string nName = World.BuildChunkName(new Vector3(
                _abstractPos.x + 1,
                _abstractPos.y,
                _abstractPos.z));

            Chunk nChunk = World.GetChunk(nName);

            return nChunk._voxels[GetIndexFromVector(0, y, z)];
        }

        private Voxel GetStateInNeighbourY(int x, int z)
        {
            string nName = World.BuildChunkName(new Vector3(
                _abstractPos.x + 1,
                _abstractPos.y,
                _abstractPos.z));

            Chunk nChunk = World.GetChunk(nName);

            return nChunk._voxels[GetIndexFromVector(x, 0, z)];
        }

        private Voxel GetStateInNeighbourZ(int x, int y)
        {
            string nName = World.BuildChunkName(new Vector3(
                _abstractPos.x + 1,
                _abstractPos.y,
                _abstractPos.z));

            Chunk nChunk = World.GetChunk(nName);

            return nChunk._voxels[GetIndexFromVector(x, y, 0)];
        }


        private int GetIndexFromVector(int x, int y, int z)
        {
            return x + y * _resolution + z * _resolution * _resolution;
        }

        private int GetIndexFromVector(Vector3Int abstractPos)
        {
            return abstractPos.x + abstractPos.y * _resolution + abstractPos.z * _resolution * _resolution;
        }

        private Vector3 GetVectorFromIndex(int i)
        {
            // Not Tested!
            int z = i % _resolution;
            int y = (i - z * _resolution * _resolution) % _resolution;
            int x = i - z * _resolution * _resolution - y * _resolution;

            return new Vector3(x, y, z);
        }

        private VoxelData[] GetNeighboursData(int voxelIndex)
        {
            VoxelData[] states = new VoxelData[6];
            Voxel originVoxel = _voxels[voxelIndex];

            if (originVoxel.Position.z < _resolution - 2)
                states[0] = _voxels[voxelIndex + _resolution * _resolution].GetVoxelData();
            // TODO: Connection to neighbour Z
            if (originVoxel.Position.z > 0)
                states[1] = _voxels[voxelIndex - _resolution * _resolution].GetVoxelData();

            if (originVoxel.Position.y < _resolution - 2)
                states[2] = _voxels[voxelIndex + _resolution].GetVoxelData();
            // TODO: Connection to neighbour Y
            if (originVoxel.Position.y > 0)
                states[3] = _voxels[voxelIndex - _resolution].GetVoxelData();

            if (originVoxel.Position.x < _resolution - 2)
                states[4] = _voxels[voxelIndex + 1].GetVoxelData();
            // TODO: Connection to neighbour X
            if (originVoxel.Position.x > 0)
                states[5] = _voxels[voxelIndex - 1].GetVoxelData();

            return states;
        }


#region Enumerator Debug
// This part is mostly for debugging purposes to be able to draw one triangle on screen at a time
        
        public void InitializeEnumerator()
        {
            _resolution = 16;
            _voxels = new Voxel[_resolution * _resolution * _resolution];
            _vertices = new List<Vector3>();

            Random rand = new Random();
            int i = 0;
            for (int z = 0; z < _resolution; z++)
            for (int y = 0; y < _resolution; y++)
            for (int x = 0; x < _resolution; x++, i++)
            {
                bool rndBool = rand.NextDouble() > 0.5;
                _voxels[i] = new Voxel(new Vector3(x, y, z), new Vector3Int(x, y, z), rndBool);
                _vertices.Add(_voxels[i].Position);
            }

            GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
            _mesh.name = "Chunk Mesh";
            _mesh.vertices = _vertices.ToArray();
            StartCoroutine(TriangulateCellRowsEnumerator());
        }

        private IEnumerator TriangulateCellRowsEnumerator()
        {
            List<int> chunkTriangles = new List<int>();

            int i = 0;
            for (int z = 0; z < _resolution; z++)
            for (int y = 0; y < _resolution; y++)
            for (int x = 0; x < _resolution; x++, i++)
            {
                if (x >= _resolution - 1 || y >= _resolution - 1 || z >= _resolution - 1) continue;
                if (!_voxels[i].State) continue;

                isCubeDone = false;
                StartCoroutine(TriangulateCellEnumerator(i, chunkTriangles));
                yield return new WaitUntil(() => isCubeDone);
            }

            _mesh.triangles = chunkTriangles.ToArray();
        }

        private IEnumerator TriangulateCellEnumerator(int vi, List<int> ts)
        {
            int[] offsets = new int[8];

            offsets[0] = vi;
            offsets[1] = vi + 1;
            offsets[2] = vi + _resolution;
            offsets[3] = vi + _resolution + 1;
            offsets[4] = vi + _resolution * _resolution;
            offsets[5] = vi + _resolution * _resolution + 1;
            offsets[6] = vi + _resolution * _resolution + _resolution;
            offsets[7] = vi + _resolution * _resolution + _resolution + 1;


            VoxelData[] voxelData = GetNeighboursDataEnumerator(
                vi, offsets[1], offsets[2], offsets[4]);


            //bool[] states = GetNeighbours(vi);

            for (var i = 0; i < _cubeTriangles.Length; i++)
            {
                VoxelData vd = voxelData[i / 2];
                if (vd.state) continue;
                var triangles = _cubeTriangles[i];
                int[] offsetTriangles =
                {
                    offsets[triangles[0]],
                    offsets[triangles[1]],
                    offsets[triangles[2]]
                };

                ts.AddRange(offsetTriangles);
                _mesh.triangles = ts.ToArray();
                yield return null;
            }

            isCubeDone = true;
        }


        private VoxelData[] GetNeighboursDataEnumerator(int voxelIndex, int xOffset, int yOffset, int zOffset)
        {
            VoxelData[] states =
            {
                new VoxelData("Forward", Vector3.zero, false),
                new VoxelData("Back", Vector3.zero, false),
                new VoxelData("Right", Vector3.zero, false),
                new VoxelData("Left", Vector3.zero, false),
                new VoxelData("Up", Vector3.zero, false),
                new VoxelData("Down", Vector3.zero, false)
            };
            Voxel originVoxel = _voxels[voxelIndex];

            if (originVoxel.Position.z < _resolution - 2)
                states[0] = _voxels[voxelIndex + _resolution * _resolution].GetVoxelData();
            // TODO: Connection to neighbour Z
            if (originVoxel.Position.z > 0)
                states[1] = _voxels[voxelIndex - _resolution * _resolution].GetVoxelData();

            if (originVoxel.Position.y < _resolution - 2)
                states[2] = _voxels[voxelIndex + _resolution].GetVoxelData();
            // TODO: Connection to neighbour Y
            if (originVoxel.Position.y > 0)
                states[3] = _voxels[voxelIndex - _resolution].GetVoxelData();

            if (originVoxel.Position.x < _resolution - 2)
                states[4] = _voxels[voxelIndex + 1].GetVoxelData();
            // TODO: Connection to neighbour X
            if (originVoxel.Position.x > 0)
                states[5] = _voxels[voxelIndex - 1].GetVoxelData();

            return states;
        }

#endregion

#region JobsTesting
// Nothing here yet
        public struct TriangulateCellRowsJob : IJob
        {
            public void Execute()
            {
            }
        }

#endregion
    }
}