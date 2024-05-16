Shader "Unlit/CurvedSurface"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SwerveX("�������", Range(-0.01,0.01)) = 0.0
        _SwerveY("��������", Range(-0.01,0.01)) = 0.0
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
                //��ȡģ�͵Ŀռ�����
                float3 WorldPos = mul(unity_ObjectToWorld, v.vertex);

                //----����������Ϊ��� ----
                //����Z������ƽ����ȡ�������ߣ�ԽԶ����������ԭ�㣬����Ч��Խ���ԡ�
                //��������������������򣬺�����ǿ��
                WorldPos.x += pow(WorldPos.z, 2) * _SwerveX;

                //������������ͬ���ı�Y�ᣬ���������Ч��
                WorldPos.y += pow(WorldPos.z, 2) * _SwerveY;

                //����ģ��λ�ã�WordPos ��������������Ŀռ�λ��
                WorldPos -= mul(unity_ObjectToWorld, float4(0, 0, 0, 1));

                //�޸����綥��ת�����������㡣
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
