// Source: http://answers.unity3d.com/questions/482128/draw-grid-lines-in-game-view.html

using UnityEngine;

public class GridOverlay
{
    public bool showMain = true;
    public bool showSub = true;

    public Vector3 gridSize;

    public float largeStep;
    public float smallStep;

    public Vector3 startPosition;

    private Material lineMaterial;

    public Color mainColor = new Color(0f, 1f, 0f, 1f);
    public Color subColor = new Color(0f, 0.5f, 0f, 1f);

    void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            var shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    public void OnPostRender() 
	{        
		CreateLineMaterial();
		// set the current material
		lineMaterial.SetPass( 0 );
		
		GL.Begin( GL.LINES );
		if (smallStep == 0)
			Debug.Log("Step is 0, wont draw Grid. Step it to higher than 0.");

		if (largeStep == 0)
            Debug.Log("Step is 0, wont draw Grid. Step it to higher than 0.");

		if (showSub)
        {
            GL.Color(subColor);

            //Layers
            for (float j = 0; j <= gridSize.y; j += smallStep)
            {
                //X axis lines
                for (float i = 0; i <= gridSize.z; i += smallStep)
                {
                    GL.Vertex3(startPosition.x, startPosition.y + j , startPosition.z + i);
                    GL.Vertex3(startPosition.x + gridSize.x, startPosition.y + j , startPosition.z + i);
                }

                //Z axis lines
                for (float i = 0; i <= gridSize.x; i += smallStep)
                {
                    GL.Vertex3(startPosition.x + i, startPosition.y + j , startPosition.z);
                    GL.Vertex3(startPosition.x + i, startPosition.y + j , startPosition.z + gridSize.z);
                }
            }

            //Y axis lines
            for (float i = 0; i <= gridSize.z; i += smallStep)
            {
                for (float k = 0; k <= gridSize.x; k += smallStep)
                {
                    GL.Vertex3(startPosition.x + k, startPosition.y , startPosition.z + i);
                    GL.Vertex3(startPosition.x + k, startPosition.y + gridSize.y , startPosition.z + i);
                }
            }
        }

        if (showMain)
        {
            GL.Color(mainColor);

            //Layers
            for (float j = 0; j <= gridSize.y; j += largeStep)
            {
                //X axis lines
                for (float i = 0; i <= gridSize.z; i += largeStep)
                {
                    GL.Vertex3(startPosition.x, startPosition.y + j, startPosition.z + i);
                    GL.Vertex3(startPosition.x + gridSize.x, startPosition.y + j, startPosition.z + i);
                }

                //Z axis lines
                for (float i = 0; i <= gridSize.x; i += largeStep)
                {
                    GL.Vertex3(startPosition.x + i, startPosition.y + j, startPosition.z);
                    GL.Vertex3(startPosition.x + i, startPosition.y + j, startPosition.z + gridSize.z);
                }
            }

            //Y axis lines
            for (float i = 0; i <= gridSize.z; i += largeStep)
            {
                for (float k = 0; k <= gridSize.x; k += largeStep)
                {
                    GL.Vertex3(startPosition.x + k, startPosition.y, startPosition.z + i);
                    GL.Vertex3(startPosition.x + k, startPosition.y + gridSize.y, startPosition.z + i);
                }
            }
        }
		
		GL.End();
	}

    public Vector3 SnapToGrid(Vector3 position) {
        position.x = Mathf.Round(position.x * largeStep) * smallStep;
        position.y = Mathf.Round(position.y * largeStep) * smallStep;
        position.z = Mathf.Round(position.z * largeStep) * smallStep;
        return position; 
    } 
}