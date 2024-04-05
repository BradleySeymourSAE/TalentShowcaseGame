// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

#if UNITY_EDITOR

using UnityEngine;

namespace Weaver.Examples
{
    // An example of how to create a procedural asset.
    // This is a partial class with the actual destructible building code in the 05 Missile Command example.
    public partial class Building
    {
        /************************************************************************************************************************/

        /// <summary>[Editor-Only] The horizontal size of the building.</summary>
        private const float Width = 1.5f;

        /// <summary>[Editor-Only] The vertical size of the building.</summary>
        private const float Height = 2;

        /// <summary>[Editor-Only] The square size of the windows.</summary>
        private const float WindowSize = 0.4f;

        /// <summary>[Editor-Only] The number of windows in a horizontal row.</summary>
        private const int WindowCountX = 2;

        /// <summary>[Editor-Only] The number of windows in a vertical column.</summary>
        private const int WindowCountY = 3;

        /************************************************************************************************************************/

        /// <summary>[Editor-Only]
        /// The procedurally generated building prefab. Marked as EditorOnly because we don't actually reference the
        /// prefab in code, we just use it in scenes like a regular prefab.
        /// </summary>
        [AssetReference(EditorOnly = true)]
        [ProceduralAsset]
        private static readonly Building ProceduralOfficeBuilding;

        /// <summary>[Editor-Only]
        /// Creates and returns an object with a <see cref="Building"/> component for Weaver to save as the
        /// <see cref="ProceduralOfficeBuilding"/> prefab.
        /// </summary>
        private static Building GenerateProceduralOfficeBuilding()
        {
            var gameObject = new GameObject();

            var filter = gameObject.AddComponent<MeshFilter>();
            filter.sharedMesh = GenerateMesh();

            var renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.sharedMaterials = new Material[] { GenerateBaseMaterial(), GenerateWindowMaterial() };

            var collider = gameObject.AddComponent<BoxCollider>();
            collider.center = new Vector3(0, Height * 0.5f, 0);
            collider.size = new Vector3(Width, Height, 1);

            return gameObject.AddComponent<Building>();
        }

        /************************************************************************************************************************/

        /// <summary>[Editor-Only]
        /// Uses a <see cref="MeshBuilder"/> to generate a <see cref="Mesh"/> using sub-mesh 0 for the base and
        /// sub-mesh 1 for the windows.
        /// </summary>
        private static Mesh GenerateMesh()
        {
            var builder = new MeshBuilder(
                4 * (1 + WindowCountX * WindowCountY),
                6,
                6 * (WindowCountX * WindowCountY));

            // Base.
            builder.Index2Triangles(0);
            builder.Vertices.Add(new Vector3(Width * -0.5f, 0, 0));
            builder.Vertices.Add(new Vector3(Width * -0.5f, Height, 0));
            builder.Vertices.Add(new Vector3(Width * 0.5f, Height, 0));
            builder.Vertices.Add(new Vector3(Width * 0.5f, 0, 0));

            // Windows.
            const float OffsetX = (Width / WindowCountX - WindowSize) * 0.5f;
            const float OffsetY = (Height / WindowCountY - WindowSize) * 0.5f;
            for (int x = 0; x < WindowCountX; x++)
            {
                for (int y = 0; y < WindowCountY; y++)
                {
                    builder.Index2Triangles(1);

                    var point = new Vector3(
                        OffsetX + Width * (x / (float)WindowCountX - 0.5f),
                        OffsetY + Height * (y / (float)WindowCountY),
                        -0.1f);

                    builder.Vertices.Add(point);
                    point.y += WindowSize;
                    builder.Vertices.Add(point);
                    point.x += WindowSize;
                    builder.Vertices.Add(point);
                    point.y -= WindowSize;
                    builder.Vertices.Add(point);
                }
            }

            for (int i = 0; i < builder.VertexCount; i++)
            {
                builder.Normals.Add(Vector3.back);
            }

            return builder.Compile();
        }

        /************************************************************************************************************************/

        /// <summary>[Editor-Only]
        /// Creates a <see cref="Material"/> to use for the base sub-mesh.
        /// </summary>
        private static Material GenerateBaseMaterial()
        {
            return new Material(Shader.Find("Standard"))
            {
                name = "Base",
                color = new Color(0.5f, 0.5f, 0.3f),
            };
        }

        /************************************************************************************************************************/

        /// <summary>[Editor-Only]
        /// Creates a <see cref="Material"/> to use for the window sub-mesh.
        /// </summary>
        private static Material GenerateWindowMaterial()
        {
            return new Material(Shader.Find("Standard"))
            {
                name = "Window",
                color = new Color(0.65f, 0.65f, 1),
            };
        }

        /************************************************************************************************************************/
    }
}

#endif
