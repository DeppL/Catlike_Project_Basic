
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using System.Runtime.CompilerServices;

namespace ProceduralMeshes.Streams
{
	public struct MultiStream : IMeshStreams
	{
		[NativeDisableContainerSafetyRestriction] NativeArray<float3> streams0, streams1;
		[NativeDisableContainerSafetyRestriction] NativeArray<float4> streams2;
		[NativeDisableContainerSafetyRestriction] NativeArray<float2> streams3;
		[NativeDisableContainerSafetyRestriction] NativeArray<TriangleUInt16> triangles;
		public void Setup(Mesh.MeshData meshData, Bounds bounds, int vertexCount, int indexCount)
		{
			var descriptor = new NativeArray<VertexAttributeDescriptor>(
				4, Allocator.Temp, NativeArrayOptions.UninitializedMemory
			);
			descriptor[0] = new VertexAttributeDescriptor(dimension: 3);
			descriptor[1] = new VertexAttributeDescriptor(
				VertexAttribute.Normal, dimension: 3, stream: 1
			);
			descriptor[2] = new VertexAttributeDescriptor(
				VertexAttribute.Tangent, dimension: 4, stream: 2
			);
			descriptor[3] = new VertexAttributeDescriptor(
				VertexAttribute.TexCoord0, dimension: 2, stream: 3
			);

			meshData.SetVertexBufferParams(vertexCount, descriptor);
			descriptor.Dispose();

			meshData.SetIndexBufferParams(indexCount, IndexFormat.UInt16);

			meshData.subMeshCount = 1;
			meshData.SetSubMesh(
				0, new SubMeshDescriptor(0, indexCount) {
					bounds = bounds,
					vertexCount = vertexCount
				},
				MeshUpdateFlags.DontRecalculateBounds |
				MeshUpdateFlags.DontValidateIndices
			);

			streams0 = meshData.GetVertexData<float3>();
			streams1 = meshData.GetVertexData<float3>(1);
			streams2 = meshData.GetVertexData<float4>(2);
			streams3 = meshData.GetVertexData<float2>(3);
			triangles = meshData.GetIndexData<ushort>().Reinterpret<TriangleUInt16>(2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetVertex(int index, Vertex data)
		{
			streams0[index] = data.position;
			streams1[index] = data.normal;
			streams2[index] = data.tangent;
			streams3[index] = data.texCoord0;
		}

		public void SetTriangle(int index, int3 triangle) => triangles[index] = triangle;
	}
}
