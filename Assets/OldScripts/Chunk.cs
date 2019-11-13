using System.Collections.Generic;
using UnityEngine;

namespace OldScripts
{
    public class Chunk : MonoBehaviour
    {
        private int _resolution;
        private float _voxelSize, _gridSize;
        private Voxel[] _voxels;

        private Mesh _mesh;
        private List<Vector3> _vertices;
        private List<int> _triangles;


        public Voxel this[int x, int y, int z]
        {
            get => _voxels[VoxelRealIndex(x, y, z)];
            set => _voxels[VoxelRealIndex(x, y, z)] = value;
        }

        public void Initialize(int resolution, float size)
        {
            _resolution = resolution;
            _gridSize = size;
            _voxelSize = size / resolution;
            _voxels = new Voxel[resolution * resolution * resolution];
            _vertices = new List<Vector3>();
            _triangles = new List<int>();
            
            
            
            int i = 0;
            for (int z = 0; z < resolution; z++)
            for (int y = 0; y < resolution; y++)
            for (int x = 0; x < resolution; x++, i++)
            {
                CreateVoxel(x, y, z);
            }


            GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
            _mesh.name = "VoxelGrid Mesh";
            Refresh();
        }

        private void Refresh()
        {
            BuildMesh();
        }

        private void BuildMesh()
        {
            _vertices.Clear();
            _triangles.Clear();
            _mesh.Clear();
            
            int i = 0;
            for (int z = 0; z < _resolution; z++)
            for (int y = 0; y < _resolution; y++)
            for (int x = 0; x < _resolution; x++, i++)
            {
                _voxels[i].CreateCube(CubeConfig.FULL, _vertices, _triangles);
            }
            
            _mesh.vertices = _vertices.ToArray();
            _mesh.triangles = _triangles.ToArray();
        }

        private void CreateVoxel(int x, int y, int z)
        {
            Vector3 pos = new Vector3(x, y, z);
            Voxel voxel = new Voxel(pos, gameObject);
        }

        public int VoxelRealIndex(int x, int y, int z)
        {
            return (x * _resolution * _resolution) + (y * _resolution) + z;
        }
    }
}