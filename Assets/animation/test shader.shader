Shader "visualization_app_data"{
properties{}
SubShader{
	pass{
		CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members vertPos)
#pragma exclude_renderers d3d11

		#pragma vertex vert 
		#pragma fragment frag

		struct appdata{
			float4 vertPosition:POSITION; 
		};
		struct v2f{
			float4 vertPosition:SV_POSITION; 
			float4 vertPos;
			float4 textcoords: TEXTCOORD0;
			
		};

		v2f vert(appdata vInput):SV_POSITION {
			v2f vOut;
			vOut.vertPosition = UnityObjectToClipPos(vInput.vertPosition); 
			return vOut; 
		}

		float4 frag(void):COLOR {
			return float4(0.2,0.7,0.9,1.0); 
		}

		ENDCG
		}
	}
}