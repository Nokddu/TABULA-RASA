Shader "Sprites/Sprite-StencilReadShow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,0.5) // 실루엣 색상 (기본: 반투명 흰색)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _StencilRef ("Stencil Reference Value", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Sprite"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            // ▼▼▼ 여기가 핵심! ▼▼▼
            Stencil
            {
                Ref [_StencilRef] // 장애물이 남긴 표시(1)를 참조해서
                Comp Equal      // 스텐실 버퍼의 값이 Ref와 '같을(Equal)' 때만 그림을 그려라!
                Pass Keep       // 테스트 통과해도 스텐실 버퍼는 건드리지 않음
            }
            // ▲▲▲ 여기까지 ▲▲▲

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            fixed4 _Color; // Properties에 선언한 _Color를 여기서 받음

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * float4(1,1,1,1); // 여기선 Tint 대신 _Color를 쓸 거라 기본값으로 둠
                return OUT;
            }

            sampler2D _MainTex;
            
            // ▼▼▼ 실루엣 색상을 만드는 핵심 로직 ▼▼▼
            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord);
                
                // 원래 스프라이트의 알파값(투명도)은 유지하면서
                // 색상은 우리가 머티리얼에서 지정한 _Color로 덮어씌운다.
                c.rgb = _Color.rgb; 
                c.a = c.a * _Color.a;
                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }
    }
}