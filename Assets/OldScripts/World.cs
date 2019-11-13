using UnityEngine;

namespace OldScripts
{
    public class World : MonoBehaviour
    {

        private void Awake()
        {
            
        }

//        Dictionary<string, Chunk> _voxels = new Dictionary<string, Chunk>();
//
//        private Voxel _voxel;
//        private GameObject _world;
//        public Material blockMaterial;
//        
//        private void Start()
//        {
//            _world = gameObject;
//            _voxel = new Voxel(BlockConfig.FULL, transform.position, gameObject);
//            CombineMesh(_voxel.CreateBlock());
//        }
//
//        private void CombineMesh(MeshFilter[] meshFilters)
//        {
//            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
//
//            int i = 0;
//            while (i < meshFilters.Length)
//            {
//                combine[i].mesh = meshFilters[i].sharedMesh;
//                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
//                i++;
//            }
//            
//            MeshFilter mf = (MeshFilter) _world.gameObject.AddComponent(typeof(MeshFilter));
//            mf.mesh = new Mesh();
//            
//            mf.mesh.CombineMeshes(combine);
//
//            MeshRenderer renderer = _world.gameObject.AddComponent<MeshRenderer>();
//            renderer.material = blockMaterial;
//            
//        }
//        
//        
//        private void VoxelLoop()
//        {
//            for (int z = 0; z < Chunk.GetSize; z++)
//            for (int y = 0; y < Chunk.GetSize; y++)
//            for (int x = 0; x < Chunk.GetSize; x++)
//            {
//                
//            }
//        }
    }
}
