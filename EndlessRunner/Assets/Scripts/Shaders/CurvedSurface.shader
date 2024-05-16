Shader "Unlit/CurvedSurface"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SwerveX("左右弯道", Range(-0.01,0.01)) = 0.0
        _SwerveY("上坡下破", Range(-0.01,0.01)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _SwerveX;
            float _SwerveY;
            //float4 curveIt(float4 v);

            v2f vert (appdata v)
            {
                //获取模型的空间坐标
                float3 WorldPos = mul(unity_ObjectToWorld, v.vertex);

                //----左右坐标作为弯道 ----
                //依据Z坐标求平方获取弯曲曲线，越远离世界坐标原点，弯曲效果越明显。
                //最后乘以左右弯道弯曲方向，和弯曲强度
                WorldPos.x += pow(WorldPos.z, 2) * _SwerveX;

                //方法与上面相同，改变Y轴，获得上下坡效果
                WorldPos.y += pow(WorldPos.z, 2) * _SwerveY;

                //修正模型位置，WordPos 不包含物体自身的空间位移
                WorldPos -= mul(unity_ObjectToWorld, float4(0, 0, 0, 1));

                //修改世界顶点转回物体自身顶点。
                v.vertex = mul(unity_WorldToObject, WorldPos);
                
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            //float4 curveIt(float4 v)
            //{
            // /*1*/float4 world = mul(unity_ObjectToWorld, v);
            // /*2*/float dist = length(world.xyz - _BendOrigin.xyz);
            // /*3*/dist = max(0, dist - _BendFallOff);

            // /*4*/dist = pow(dist, _BendFallOffStr);
            // /*5*/world.xyz += dist * _BendAmount;
            // /*6*/return mul(unity_WorldToObject, world);
            //}

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
