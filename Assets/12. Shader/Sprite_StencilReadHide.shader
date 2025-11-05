Shader "Sprites/Sprite-StencilReadHide"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1) // 원래 캐릭터 색상
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _StencilRef ("Stencil Reference Value", Float) = 1
    }

    SubShader
    {
        // (Tags, Cull, Blend 등은 위 셰이더와 모두 동일)
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
            // ▼▼▼ 여기 딱 한 단어만 달라! ▼▼▼
            Stencil
            {
                Ref [_StencilRef]
                Comp NotEqual   // 스텐실 버퍼 값이 Ref와 '같지 않을(NotEqual)' 때만 그려라!
                Pass Keep
            }
            // ▲▲▲ 여기가 핵심 ▲▲▲

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
            
            fixed4 _Color;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color; // Sprite Renderer의 Tint 색상을 적용
                return OUT;
            }

            sampler2D _MainTex;
            
            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                c.rgb *= c.a; // 투명한 부분은 검게 처리 (기본 스프라이트 셰이더 동작)
                return c;
            }
            ENDCG
        }
    }
}