using System;
using System.Collections.Generic;
using UnityEngine;

namespace OldScripts
{
    public enum CubeConfig
    {
        FULL,
        NO_Y,
        EMPTY
    };

    public class Voxel
    {
        private enum CubeSide
        {
            FRONT,
            BACK,
            LEFT,
            RIGHT,
            TOP,
            BOTTOM,
        };

        private CubeSide[][] cubeSides = new[]
        {
            new[]
            {
                CubeSide.FRONT, CubeSide.BACK,
                CubeSide.LEFT, CubeSide.RIGHT,
                CubeSide.TOP, CubeSide.BOTTOM
            },
            new[]
            {
                CubeSide.FRONT, CubeSide.BACK,
                CubeSide.LEFT, CubeSide.RIGHT,
            }
        };

        private Vector3 _position;
        private GameObject _parent;

        public Voxel(Vector3 pos, GameObject parent)
        {
            _position = pos;
            _parent = parent;
        }
        
        public void CreateCube(CubeConfig config, List<Vector3> vertices, List<int> triangles)
        {
            // All possible vertices
            Vector3 p0 = new Vector3(-0.5f, -0.5f, -0.5f);
            Vector3 p1 = new Vector3(0.5f, -0.5f, -0.5f);
            Vector3 p2 = new Vector3(0.5f, 0.5f, -0.5f);
            Vector3 p3 = new Vector3(-0.5f, 0.5f, -0.5f);
            Vector3 p4 = new Vector3(-0.5f, 0.5f, 0.5f);
            Vector3 p5 = new Vector3(0.5f, 0.5f, 0.5f);
            Vector3 p6 = new Vector3(0.5f, -0.5f, 0.5f);
            Vector3 p7 = new Vector3(-0.5f, -0.5f, 0.5f);

            foreach (CubeSide side in cubeSides[(int) config])
            {
                switch (side)
                {
                    case CubeSide.FRONT:
                        CreateTriangle(p0, p4, p5, vertices, triangles);
                        CreateTriangle(p0, p5, p1, vertices, triangles);
                        break;
                    case CubeSide.BACK:
                        CreateTriangle(p2, p6, p7, vertices, triangles);
                        CreateTriangle(p2, p7, p3, vertices, triangles);
                        break;
                    case CubeSide.LEFT:
                        CreateTriangle(p1, p6, p2, vertices, triangles);
                        CreateTriangle(p1, p5, p6, vertices, triangles);
                        break;
                    case CubeSide.RIGHT:
                        CreateTriangle(p3, p4, p0, vertices, triangles);
                        CreateTriangle(p3, p7, p4, vertices, triangles);
                        break;
                    case CubeSide.TOP:
                        CreateTriangle(p4, p6, p5, vertices, triangles);
                        CreateTriangle(p4, p7, p6, vertices, triangles);
                        break;
                    case CubeSide.BOTTOM:
                        CreateTriangle(p1, p2, p3, vertices, triangles);
                        CreateTriangle(p1, p3, p0, vertices, triangles);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(side), side, null);
                }
            }
        }

        private void CreateTriangle(Vector3 v1, Vector3 v2, Vector3 v3, List<Vector3> vertices, List<int> triangles)
        {
            int vertexIndex = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            triangles.Add(1);
            triangles.Add(2);
            triangles.Add(3);
        }


//        public BlockConfig blkConfig;
//
//
//        private bool _isSolid;
//
//        private Vector3 _position;
//        private GameObject _parent;
//
//        public Voxel(BlockConfig blockConfig, Vector3 pos, GameObject parent)
//        {
//            blkConfig = blockConfig;
//            _isSolid = blockConfig != BlockConfig.EMPTY;
//
//            _position = pos;
//            _parent = parent;
//        }
//
//        public MeshFilter[] CreateBlock()
//        {
//            if (blkConfig == BlockConfig.EMPTY) return null;
//
//            List<MeshFilter> meshFilters = new List<MeshFilter>();
//
//            int[] sides = new int[] { };
//            switch (blkConfig)
//            {
//                case BlockConfig.FULL:
//                    sides = new[] {0, 1, 2, 3, 4, 5};
//                    break;
//                case BlockConfig.SLOPE_TOP:
//                    sides = new[] {0, 5, 6, 7, 8};
//                    break;
//                case BlockConfig.EMPTY:
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException(nameof(blkConfig), blkConfig, null);
//            }
//
//            meshFilters.AddRange(sides.Select(side => CreateSide((CubeSide) side)));
//            return meshFilters.ToArray();
//        }
//
//        private MeshFilter CreateSide(CubeSide side)
//        {
//            Mesh mesh = new Mesh();
//
//            int[] oneTriangle = new[] {3, 1, 0};
//            int[] twoTriangles = new[] {3, 1, 0, 3, 2, 1};
//
//

//
//            switch (side)
//            {
//                case CubeSide.BOTTOM:
//                    mesh.vertices = new[] {p0, p1, p2, p3};
//                    mesh.normals = new[] {Vector3.down, Vector3.down, Vector3.down, Vector3.down};
//                    mesh.triangles = twoTriangles;
//                    break;
//                case CubeSide.TOP:
//                    mesh.vertices = new[] {p7, p6, p5, p4};
//                    mesh.normals = new[] {Vector3.up, Vector3.up, Vector3.up, Vector3.up};
//                    mesh.triangles = twoTriangles;
//                    break;
//                case CubeSide.LEFT:
//                    mesh.vertices = new[] {p7, p4, p0, p3};
//                    mesh.normals = new[] {Vector3.left, Vector3.left, Vector3.left, Vector3.left};
//                    mesh.triangles = twoTriangles;
//                    break;
//                case CubeSide.RIGHT:
//                    mesh.vertices = new[] {p5, p6, p2, p1};
//                    mesh.normals = new[] {Vector3.right, Vector3.right, Vector3.right, Vector3.right};
//                    mesh.triangles = twoTriangles;
//                    break;
//                case CubeSide.FRONT:
//                    mesh.vertices = new[] {p4, p5, p1, p0};
//                    mesh.normals = new[] {Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward};
//                    mesh.triangles = twoTriangles;
//                    break;
//                case CubeSide.BACK:
//                    mesh.vertices = new[] {p6, p7, p3, p2};
//                    mesh.normals = new[] {Vector3.back, Vector3.back, Vector3.back, Vector3.back};
//                    mesh.triangles = twoTriangles;
//                    break;
//
//                case CubeSide.HALF_LEFT:
//                    mesh.vertices = new[] {p7, p4, p0};
//                    mesh.normals = new[] {Vector3.left, Vector3.left, Vector3.left};
//                    mesh.triangles = oneTriangle;
//                    break;
//                case CubeSide.HALF_RIGHT:
//                    mesh.vertices = new[] {p6, p2, p1};
//                    mesh.normals = new[] {Vector3.right, Vector3.right, Vector3.right};
//                    mesh.triangles = oneTriangle;
//                    break;
//
//                case CubeSide.TOP_SLOPE:
//                    Vector3 n = Vector3.Normalize(Vector3.forward + Vector3.up);
//                    mesh.vertices = new[] {p7, p6, p1, p0};
//                    mesh.normals = new[] {n, n, n, n};
//                    mesh.triangles = twoTriangles;
//                    break;
//
//                default:
//                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
//            }
//
//            MeshFilter meshFilter = new MeshFilter();
//            meshFilter.mesh = mesh;
//            return meshFilter;
//        }
//
//
    }
}