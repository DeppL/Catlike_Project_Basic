// This shader fills the mesh shape with a color predefined in the code.
Shader "HexMap/URPUnlitShaderBasic"
{
    // The properties block of the Unity shader. In this example this block is empty
    // because the output color is predefined in the fragment shader code.
    Properties { }

    // The SubShader block containing the Shader code.
    SubShader
    {
        // SubShader Tags define when and under which conditions a SubShader block or
        // a pass is executed.
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            // The HLSL code block. Unity SRP uses the HLSL language.
            HLSLPROGRAM
            // This line defines the name of the vertex shader.
            #pragma vertex vert
            // This line defines the name of the fragment shader.
            #pragma fragment frag

            // The Core.hlsl file contains definitions of frequently used HLSL
            // macros and functions, and also contains #include references to other
            // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

						// fixed4 _Color;

            // The structure definition defines which variables it contains.
            // This example uses the Attributes structure as an input structure in
            // the vertex shader.
            struct Attributes
            {
                // The positionOS variable contains the vertex positions in object
                // space.
                float4 positionOS   : POSITION;
								float4 color 				: COLOR;
            };

            struct Varyings
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float4 positionHCS  : SV_POSITION;
								float4 color 				: COLOR;
            };

            // The vertex shader definition with properties defined in the Varyings
            // structure. The type of the vert function must match the type (struct)
            // that it returns.
            Varyings vert(Attributes IN)
            {
                // Declaring the output object (OUT) with the Varyings struct.
                Varyings OUT;
                // The TransformObjectToHClip function transforms vertex positions
                // from object space to homogenous clip space.
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);

								OUT.color = IN.color;
                // Returning the output.
                return OUT;
            }

            // The fragment shader definition.
            half4 frag(Varyings IN) : SV_Target
            {
                // Defining the color variable and returning it.
                // half4 customColor = IN.color;
                // return customColor;
								return IN.color;
            }
            ENDHLSL
        }
    }
}

// Shader "My Pipeline/Unlit" {

// 	Properties {
// 		// _Color ("Color", Color) = (1, 1, 1, 1)
// 	}

// 	SubShader {

// 		Pass {
// 			HLSLPROGRAM

// 			#pragma target 3.5

// 			#pragma multi_compile_instancing
// 			#pragma instancing_options assumeuniformscaling

// 			#pragma vertex UnlitPassVertex
// 			#pragma fragment UnlitPassFragment

// 			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
// 			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"

// 			CBUFFER_START(UnityPerFrame)
// 				float4x4 unity_MatrixVP;
// 			CBUFFER_END

// 			CBUFFER_START(UnityPerDraw)
// 				float4x4 unity_ObjectToWorld;
// 			CBUFFER_END

// 			#define UNITY_MATRIX_M unity_ObjectToWorld

// 			// UNITY_INSTANCING_BUFFER_START(PerInstance)
// 			// 	UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
// 			// UNITY_INSTANCING_BUFFER_END(PerInstance)

// 			struct VertexInput {
// 				float4 pos : POSITION;
// 				float4 color : COLOR;
// 				UNITY_VERTEX_INPUT_INSTANCE_ID
// 			};

// 			struct VertexOutput {
// 				float4 clipPos : SV_POSITION;
// 				float4 color : COLOR;
// 				UNITY_VERTEX_INPUT_INSTANCE_ID
// 			};

// 			VertexOutput UnlitPassVertex (VertexInput input) {
// 				VertexOutput output;
// 				UNITY_SETUP_INSTANCE_ID(input);
// 				UNITY_TRANSFER_INSTANCE_ID(input, output);
// 				float4 worldPos = mul(UNITY_MATRIX_M, float4(input.pos.xyz, 1.0));
// 				output.clipPos = mul(unity_MatrixVP, worldPos);
// 				output.color = input.color;
// 				return output;
// 			}

// 			float4 UnlitPassFragment (VertexOutput input) : SV_TARGET {
// 			// float4 UnlitPassFragment (VertexOutput input) : COLOR {
// 				UNITY_SETUP_INSTANCE_ID(input);
// 				// float4 color = UNITY_ACCESS_INSTANCED_PROP(PerInstance, _Color);
// 				// color = color.rgba * input.color;
// 				// return color;
// 				return input.color;
// 			}
// 			ENDHLSL
// 		}
// 	}
// }

