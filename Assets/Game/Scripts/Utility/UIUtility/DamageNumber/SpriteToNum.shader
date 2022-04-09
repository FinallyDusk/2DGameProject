Shader "Custom/SpriteToNum"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_NumWidth ("Num Width", Range(0.01, 0.5)) = 0.05
		//_ShowNum ("Show Num", int) = 1
		_Size ("Size", float) = 1
    }
    SubShader
    {
		Tags{"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}

		Pass
		{
			Cull Off //要渲染背面保证效果正确
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#define	NUM_MAX_COUNT 20
            #include "UnityCG.cginc"


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float _NumWidth;
			float _Size;
			uniform float _ShowNum[NUM_MAX_COUNT];
			uniform vector _NumBorder[20];
			uniform fixed4 _FontColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				//todo,需要删除
				/*_NumBorder[0] = fixed4(0, 0.0365, 0.0357, 1);
				_NumBorder[1] = fixed4(0.087, 0.1376, 0, 1);
				_NumBorder[2] = fixed4(0.1769, 0.2247, 0, 1);
				_NumBorder[3] = fixed4(0.264, 0.3146, 0.0357, 1);*/
				/*_ShowNum[0] = 1;
				_ShowNum[1] = 1;
				_ShowNum[2] = 1;
				_ShowNum[3] = 1;
				_ShowNum[4] = 1;
				_ShowNum[5] = 1;
				_ShowNum[6] = 1;*/

				float x = i.uv.x;
				float y = i.uv.y;
				bool _Clip = true;
				int validCount = 0;
				for (int index = 0; index < NUM_MAX_COUNT; index++)
				{
					if (_ShowNum[index] == -1) break;
					validCount += 1;
				}
				float standard = (1.0 - validCount * _NumWidth) / 2 + validCount * _NumWidth;
				for (int j = 0; j < validCount; j++)
				{
					//获得余数
					int col = _ShowNum[j];
					//计算uv

					//计算行列
					float colUVWidth = _NumBorder[col].y - _NumBorder[col].x;
					float rowUVHeight = _NumBorder[col].w - _NumBorder[col].z;
					float minWidth = _NumBorder[col].x;
					//float maxWidth = colUVWidth * (col + 1);
					float minHeight = _NumBorder[col].z;
					//float maxHeight = rowUVWidth * (row + 1);
					//将(standard - (j * _NumWidth), standard - ((j + 1) * _NumWidth))的范围内，映射到第row行第col列的数字uv
					float xMin = standard - ((j + 1.0) * _NumWidth);
					float xMax = standard - (j * _NumWidth);
					if (xMin <= i.uv.x && i.uv.x <= xMax) {
						float xScale = _NumWidth / colUVWidth;
						x = (i.uv.x - xMin) / xScale;
						x += minWidth;
						float yScale = 1.0 / rowUVHeight;
						y = i.uv.y / yScale;
						y += minHeight;
						//return fixed4(j / 20.0, 0, 0, 1);
						//y = (y - 1) * -1;
						_Clip = false;
						break;
					}
				}
				if (_Clip) {
					return fixed4(0, 0, 0, 0);
				}
                fixed4 col = tex2D(_MainTex, float2(x, y)) * _FontColor;
				return col;//fixed4(colUVWidth, 0, 0, 1);
            }
            ENDCG
        }
    }
}
