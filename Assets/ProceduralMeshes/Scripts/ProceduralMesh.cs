using UnityEngine;
using UnityEngine.Rendering;
using ProceduralMeshes;
using ProceduralMeshes.Generators;
using ProceduralMeshes.Streams;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralMesh : MonoBehaviour
{
	static MeshJobScheduleDelegate[] jobs = {
		MeshJob<SquareGrid			, SingleStream>.ScheduleParallel,
		MeshJob<SharedSquareGrid	, SingleStream>.ScheduleParallel,
		MeshJob<SharedTriangleGrid	, SingleStream>.ScheduleParallel,
		MeshJob<PointyHexagonGrid	, SingleStream>.ScheduleParallel,
		MeshJob<FlatHexagonGrid		, SingleStream>.ScheduleParallel,
		MeshJob<CubeSphere			, SingleStream>.ScheduleParallel,
		MeshJob<UVSphere			, SingleStream>.ScheduleParallel,
	};
	public enum MeshType {
		SquareGrid, SharedSquareGrid, SharedTriangleGrid,
		PointyHexagonGrid, FlatHexagonGrid,
		CubeSphere, UVSphere,
	};
	[SerializeField] MeshType meshType;
	[SerializeField, Range(1, 50)] int resolution = 40;

	[System.Flags]
	public enum GizmoMode { Nothing = 0, Vertices = 1, Normals = 0b10, Tangents = 0b100 }
	[SerializeField] GizmoMode gizmos;

	public enum MaterialMode { Flat, Ripple, LatLonMap, CubeMap }
	[SerializeField] MaterialMode material;
	[SerializeField] Material[] materials;


	Mesh mesh;
	Vector3[] vertices, normals;
	Vector4[] tangents;

	private void Awake() {
		mesh = new Mesh {
			name = "Procedural Mesh"
		};
		GetComponent<MeshFilter>().mesh = mesh;
	}
	private void OnValidate()
	{
		enabled = true;
	}

	private void Update()
	{
		GenerateMesh();
		enabled = false;

		vertices = null;
		normals = null;
		tangents = null;

		GetComponent<MeshRenderer>().material = materials[(int)material];
	}

	private void OnDrawGizmos()
	{
		if (gizmos == GizmoMode.Nothing || mesh == null) { return; }

		bool drawVertices = gizmos.HasFlag(GizmoMode.Vertices);
		bool drawNormals  = gizmos.HasFlag(GizmoMode.Normals );
		bool drawTangents = gizmos.HasFlag(GizmoMode.Tangents);

		if (				vertices == null) { vertices = mesh.vertices; }
		if (drawNormals  && normals  == null) { normals  = mesh.normals ; }
		if (drawTangents && tangents == null) { tangents = mesh.tangents; }

		Transform t = transform;
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 position = t.TransformPoint(vertices[i]);
			if (drawVertices)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawSphere(position, 0.02f);
			}
			if (drawNormals)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawRay(position, t.TransformDirection(normals[i]) * 0.2f);
			}
			if (drawTangents)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawRay(position, t.TransformDirection(tangents[i]) * 0.2f);
			}
		}
	}

	void GenerateMesh()
	{
		Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
		var meshData = meshDataArray[0];

		jobs[(int)meshType](mesh, meshData, resolution, default).Complete();

		Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
	}
}
