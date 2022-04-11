Shader "Unlit/SimpleSky"
{
   Properties {
      _TopColor ("Top", Color) = (0.039, 0.741, 0.890, 1)
      _MiddleColor ("Middle", Color) = (0.180, 0.525, 0.870, 1)
      _BottomColor ("Bottom", Color) = (0.133, 0.184, 0.243, 1)
      _MinHeight ("Min", Range(0.001, 1)) = 1
      _MaxHeight ("Max", Range(0.001, 1)) = 1
   }

   SubShader {
      Tags { "Queue"="Background"  }

      Pass {
         ZWrite Off 
         Cull Off

         CGPROGRAM
         #pragma vertex vert
         #pragma fragment frag

         // User-specified uniforms
         fixed4 _BottomColor, _MiddleColor, _TopColor;
         fixed _MinHeight, _MaxHeight;

         struct vertexInput {
            float4 vertex : POSITION;
            float3 texcoord : TEXCOORD0;
         };

         struct vertexOutput {
            float4 vertex : SV_POSITION;
            float3 texcoord : TEXCOORD0;
         };

         vertexOutput vert(vertexInput input)
         {
            vertexOutput output;
            output.vertex = UnityObjectToClipPos(input.vertex);
            output.texcoord = input.texcoord;
            return output;
         }

        

         fixed4 frag (vertexOutput input) : COLOR
         {
            if (input.texcoord.y < 0){
                return lerp(_MiddleColor, _BottomColor,  saturate(-input.texcoord.y / _MinHeight));
            } 
            else  {
                return lerp(_MiddleColor, _TopColor, saturate(input.texcoord.y / _MaxHeight));
            }
         }
         ENDCG 
      }
   } 	
}