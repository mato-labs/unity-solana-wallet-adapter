using UnityEngine;

namespace SolanaWalletAdapter.Scripts
{
    public static class Utils
    {
        public static Texture2D Resize(Texture2D texture2D, int targetX, int targetY)
        {
            RenderTexture rt = new RenderTexture(targetX, targetY, 24);
            RenderTexture.active = rt;
            Graphics.Blit(texture2D, rt);
            Texture2D result = new Texture2D(targetX, targetY);
            result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
            result.Apply();
            
            rt.Release();
            
            return result;
        }
        
        public static Texture2D ScaleTexture(Texture2D src, int width, int height, FilterMode mode = FilterMode.Trilinear)
        {
            Rect texR = new Rect(0,0,width,height);
            _gpu_scale(src,width,height,mode);
		
            //Get rendered data back to a new texture
            Texture2D result = new Texture2D(width, height, TextureFormat.ARGB32, true);
            result.Resize(width, height);
            result.ReadPixels(texR,0,0,true);
            return result;			
        }
	
        /// <summary>
        /// Scales the texture data of the given texture.
        /// </summary>
        /// <param name="tex">Texure to scale</param>
        /// <param name="width">New width</param>
        /// <param name="height">New height</param>
        /// <param name="mode">Filtering mode</param>
        public static void Scale(this Texture2D tex, int width, int height, FilterMode mode = FilterMode.Trilinear)
        {
            Rect texR = new Rect(0,0,width,height);
            _gpu_scale(tex,width,height,mode);
		
            // Update new texture
            tex.Resize(width, height);
            tex.ReadPixels(texR,0,0,true);
            tex.Apply(true);	//Remove this if you hate us applying textures for you :)
        }
		
        // Internal unility that renders the source texture into the RTT - the scaling method itself.
        static void _gpu_scale(Texture2D src, int width, int height, FilterMode fmode)
        {
            //We need the source texture in VRAM because we render with it
            src.filterMode = fmode;
            src.Apply(true);	
				
            //Using RTT for best quality and performance. Thanks, Unity 5
            RenderTexture rtt = new RenderTexture(width, height, 32);
		
            //Set the RTT in order to render to it
            Graphics.SetRenderTarget(rtt);
		
            //Setup 2D matrix in range 0..1, so nobody needs to care about sized
            GL.LoadPixelMatrix(0,1,1,0);
		
            //Then clear & draw the texture to fill the entire RTT.
            GL.Clear(true,true,new Color(0,0,0,0));
            Graphics.DrawTexture(new Rect(0,0,1,1),src);
        }

        public static Sprite TexToSprite(Texture2D tex)
        {
            var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 1.0f);
            return sprite;
        }

    }
}