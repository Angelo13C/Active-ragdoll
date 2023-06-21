using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct LineRenderer : IComponentData
{
    public float HalfLineWidth;
    public float LineWidth
    {
        get => HalfLineWidth * 2f;
        set => HalfLineWidth = value / 2f;
    }

    public float DisplayPercentage;

    public Mesh GenerateMesh(NativeArray<Point> points, float3 cameraPosition)
    {
        var mesh = new Mesh();

        var pointsCount = (int) math.ceil(points.Length * DisplayPercentage);
        if (pointsCount > 1)
        {
            var verticesCount = (pointsCount - 1) * 4;
            var indicesCount = (int) (verticesCount * 1.5f);
            var vertices = new NativeArray<float3>(verticesCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            var indices = new NativeArray<ushort>(indicesCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            var lineNormal = math.normalizesafe(cameraPosition - points[0].Position);
            var rightDirection = CalculateRightDirection(points[0].Position, points[1].Position, HalfLineWidth);

            for (var i = 0; i < pointsCount - 1; i++)
            {
                var vertexIndex = i * 4;
                GenerateLine(vertices.GetSubArray(vertexIndex, 4), points[i].Position, points[i + 1].Position, ref rightDirection, HalfLineWidth);
                var index = i * 6;
                indices[index + 0] = (ushort) (vertexIndex + 0);
                indices[index + 1] = (ushort) (vertexIndex + 1);
                indices[index + 2] = (ushort) (vertexIndex + 2);
                indices[index + 3] = (ushort) (vertexIndex + 1);
                indices[index + 4] = (ushort) (vertexIndex + 3);
                indices[index + 5] = (ushort) (vertexIndex + 2);
            }

            float3 CalculateRightDirection(float3 startPoint, float3 endPoint, float halfLineWidth)
            {
                var direction = math.normalizesafe(endPoint - startPoint);
                return halfLineWidth * math.cross(direction, lineNormal);
            }
            
            void GenerateLine(NativeArray<float3> vertices, float3 startPoint, float3 endPoint, ref float3 lastRightDirection, float halfLineWidth)
            {
                vertices[1] = startPoint - lastRightDirection;
                vertices[3] = startPoint + lastRightDirection;
                lastRightDirection = CalculateRightDirection(startPoint, endPoint, halfLineWidth);
                vertices[0] = endPoint - lastRightDirection;
                vertices[2] = endPoint + lastRightDirection;
                
            }
        
            mesh.SetVertices(vertices);
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
        }

        return mesh;
    }

    public struct Point : IBufferElementData
    {
        public float3 Position;

        public static implicit operator Point(float3 position) => new Point { Position = position };

        public static void SetPoints(DynamicBuffer<Point> pointsBuffer, NativeArray<float3> value)
        {
            pointsBuffer.ResizeUninitialized(value.Length);
            for (var i = 0; i < value.Length; i++)
                pointsBuffer.ElementAt(i) = value[i];
        }
    }
    
    // This is the Graphics part of the line renderer and cannot be burst compiled
    public class Graphics : IComponentData
    {
        public Material Material;

        public void Draw(LineRenderer lineRenderer, NativeArray<Point> points, float3 position, quaternion rotation, float3 cameraPosition)
        {
            var mesh = lineRenderer.GenerateMesh(points, cameraPosition);
            UnityEngine.Graphics.DrawMesh(mesh, position, rotation, Material, 0);
        }
    }
}