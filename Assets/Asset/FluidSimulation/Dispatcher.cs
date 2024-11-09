using UnityEngine;

public class Dispatcher : MonoBehaviour
{
    private float[] ObstacleParam;
    private float[] VerticalParam;
    private float[] HorizonParam;
    private float[] MassParam;
    private float[] NewVerticalParam;
    private float[] NewHorizonParam;
    private float[] NewMassParam;
    private float[] PressParam;

    private float density;
    private float gridSize;

    private RenderTexture renderTexture;
    private const int resolution = 1024;

    private const float overRelaxation = 1.9f;

    private const float simHeight = 1.1f;
    private const float cScale = resolution / simHeight;
    private const float simWidth = resolution / cScale;

    /*
     * 	this.numX = numX + 2; 
	 *	this.numY = numY + 2;
     */


    #region Variables&Properties
    [SerializeField] private ComputeShader computeShader;
    private int KernelID;

    private struct CSPARAM
    {
        public const string KERNEL = "CSMain";
        public const string RESULT = "Result";
        public const int THREAD_NUMBER_X = 8;
        public const int THREAD_NUMBER_Y = 8;
    }

    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        Initialize();
    }
    private void Start()
    {
        transform.GetComponent<MeshRenderer>().material.mainTexture = renderTexture;

        int n = resolution;

        for (int w = 1; w < n; w++)
        {
            for (int h = 1; h < n; h++)
            {
                MassParam[w * n + h] = 1.0f;
            }
        }
    }
    private void Update()
    {
        return;

        Simulation();

        // Draw
        {
            float cellScale = 1.1f;

            float minP = PressParam[0];
            float maxP = PressParam[0];

            for (var i = 0; i < resolution; i++)
            {
                minP = Mathf.Min(minP, PressParam[i]);
                maxP = Mathf.Max(maxP, PressParam[i]);
            }

            Vector4 Color = new Vector4(255, 255, 255, 255);

            for (var i = 0; i < resolution; i++)
            {
                for (var j = 0; j < resolution; j++)
                {
                    //if (scene.showPressure)
                    {
                        float p = PressParam[i * resolution + j];
                        float s = MassParam[i * resolution + j];
                        // Color = getSciColor(p, minP, maxP);
                        {
                            p = Mathf.Min(Mathf.Max(p, minP), maxP - 0.0001f);
                            float d = maxP - minP;
                            p = d == 0.0f ? 0.5f : (p - minP) / d;
                            float m = 0.25f;
                            float num = Mathf.Floor(p / m);
                            float ns = (p - num * m) / m;

                            switch (num)
                            {
                                case 0: Color.x = 0.0f; Color.y = ns; Color.z = 1.0f; break;
                                case 1: Color.x = 0.0f; Color.y = 1.0f; Color.z = 1.0f - ns; break;
                                case 2: Color.x = ns; Color.y = 1.0f; Color.z = 0.0f; break;
                                case 3: Color.x = 1.0f; Color.y = 1.0f - ns; Color.z = 0.0f; break;
                            }
                        }

                        Color[0] = Mathf.Max(0.0f, Color[0] - 255 * s);
                        Color[1] = Mathf.Max(0.0f, Color[1] - 255 * s);
                        Color[2] = Mathf.Max(0.0f, Color[2] - 255 * s);
                    }
                    //else if (scene.showSmoke)
                    {
                        var s = MassParam[i * resolution + j];
                        Color[0] = 255 * s;
                        Color[1] = 255 * s;
                        Color[2] = 255 * s;
                        //if (scene.sceneNr == 2)
                        //    Color = getSciColor(s, 0.0, 1.0);
                    }
                    if (ObstacleParam[i * resolution + j] == 0.0)
                    {
                        Color[0] = 0;
                        Color[1] = 0;
                        Color[2] = 0;
                    }

                    /*
                      	function cX(x) {
		                    return x * cScale;
	                    }

	                    function cY(y) {
		                    return canvas.height - y * cScale;
	                    }
                     */

                    //var x = Mathf.Floor(cX(i * gridSize));
                    //var y = Mathf.Floor(cY((j + 1) * gridSize));
                    //var cx = Mathf.Floor(cScale * cellScale * gridSize) + 1;
                    //var cy = Mathf.Floor(cScale * cellScale * gridSize) + 1;

                    //for (var yi = y; yi < y + cy; yi++)
                    //{
                    //    var p = 4 * (yi * resolution + x);

                    //    for (var xi = 0; xi < cx; xi++)
                    //    {
                    //        id.data[p++] = Color[0];
                    //        id.data[p++] = Color[1];
                    //        id.data[p++] = Color[2];
                    //        id.data[p++] = 255;
                    //    }
                    //}
                }
            }
        }

        DispatchComputeShader();
    }
    #endregion


    #region ComputeShader
    private void Initialize()
    {
        KernelID = computeShader.FindKernel(CSPARAM.KERNEL);
        renderTexture = new RenderTexture(resolution, resolution, 24);

        renderTexture.enableRandomWrite = true;
        renderTexture.Create();
    }

    private void DispatchComputeShader()
    {
        computeShader.SetTexture(KernelID, CSPARAM.RESULT, renderTexture);
        computeShader.Dispatch(KernelID, resolution / CSPARAM.THREAD_NUMBER_X, resolution / CSPARAM.THREAD_NUMBER_Y, 1);
    }

    #endregion

    #region FluidSimulation
    void Simulation()
    {
        int n = resolution;

        // h(  )        normalization  
        float h1 = 1.0f / gridSize;
        float h2 = 0.5f * gridSize;

        for (int w = 1; w < n; w++)
        {
            for (int h = 1; h < n - 1; h++)
            {
                if (ObstacleParam[w * n + h] != 0.0f && ObstacleParam[w * n + h - 1] != 0.0f)
                {
                    VerticalParam[w * n + h] += Physics.gravity.magnitude * Time.deltaTime;
                }

                PressParam[w * n + h] = 0.0f;
            }
        }

        float cp = density * gridSize / Time.deltaTime;

        for (int iter = 0; iter < n; iter++)
        {
            for (int w = 1; w < n - 1; w++)
            {
                for (int h = 1; h < n - 1; h++)
                {
                    if (ObstacleParam[w * n + h] == 0.0)
                        continue;

                    float sx0 = ObstacleParam[(w - 1) * n + h];
                    float sx1 = ObstacleParam[(w + 1) * n + h];
                    float sy0 = ObstacleParam[w * n + h - 1];
                    float sy1 = ObstacleParam[w * n + h + 1];
                    float s = sx0 + sx1 + sy0 + sy1;
                    if (s == 0.0)
                        continue;

                    var div =
                        HorizonParam[(w + 1) * n + h] - HorizonParam[w * n + h] +
                        VerticalParam[w * n + h + 1] - VerticalParam[w * n + h];

                    var p = -div / s;
                    p *= overRelaxation;
                    PressParam[w * n + h] += cp * p;

                    HorizonParam[w * n + h] -= sx0 * p;
                    HorizonParam[(w + 1) * n + h] += sx1 * p;
                    VerticalParam[w * n + h] -= sy0 * p;
                    VerticalParam[w * n + h + 1] += sy1 * p;
                }
            }
        }

        for (int w = 0; w < n; w++)
        {
            HorizonParam[w * n + 0] = HorizonParam[w * n + 1];
            HorizonParam[w * n + n - 1] = HorizonParam[w * n + n - 2];
        }
        for (int h = 0; h < n; h++)
        {
            VerticalParam[0 * n + h] = VerticalParam[1 * n + h];
            VerticalParam[(n - 1) * n + h] = VerticalParam[(n - 2) * n + h];
        }

        // Deep Copy
        NewHorizonParam = HorizonParam;
        NewVerticalParam = VerticalParam;

        for (var i = 1; i < n; i++)
        {
            for (var j = 1; j < n; j++)
            {
                if (ObstacleParam[i * n + j] != 0.0 && ObstacleParam[(i - 1) * n + j] != 0.0 && j < n - 1)
                {
                    float x = i * gridSize;
                    float y = j * gridSize + 0.5f * gridSize;
                    float u = HorizonParam[i * n + j];
                    float v = (
                        VerticalParam[(i - 1) * n + j] +
                        VerticalParam[i * n + j] +
                        VerticalParam[(i - 1) * n + j + 1] +
                        VerticalParam[i * n + j + 1]
                    ) * 0.25f;

                    // make pivot to center
                    x = x - Time.deltaTime * u;
                    y = y - Time.deltaTime * v;

                    //        ġ    ̹               ʵ     е 
                    x = Mathf.Max(Mathf.Min(x, n * gridSize), gridSize);
                    y = Mathf.Max(Mathf.Min(y, n * gridSize), gridSize);

                    float dx = 0.0f;
                    float dy = 0.0f;

                    dy = h2;

                    // (Math.floor((x-dx)*h1) => Find Pivot then make to normalized with h
                    int x0 = (int)(Mathf.Min(Mathf.Floor((x - dx) * h1), n - 1));
                    // Different of delta X and normalized delta X
                    float tx = (float)(((x - dx) - x0 * gridSize) * h1);
                    // Prevate to over this.numX-1
                    int x1 = (int)(Mathf.Min(x0 + 1, n - 1));

                    int y0 = (int)(Mathf.Min(Mathf.Floor((y - dy) * h1), n - 1));
                    float ty = (float)(((y - dy) - y0 * gridSize) * h1);
                    int y1 = (int)(Mathf.Min(y0 + 1, n - 1));

                    float sx = 1.0f - tx;
                    float sy = 1.0f - ty;

                    // 이웃 셀과의 합 속도를 구하기 위해선 (전부의 합 / 4) 도 존재하지만
                    // xy(uv)의 위치가 중앙이 아닌 경우엔 아래와 같이 계산한다
                    // sx, sy, tx, ty를 곱해주면서 / 4를 하지 않아도 반에 반 토막이 나서 u는 Normalized 상태이다 
                    u = sx * sy * HorizonParam[x0 * n + y0] +
                        tx * sy * HorizonParam[x1 * n + y0] +
                        tx * ty * HorizonParam[x1 * n + y1] +
                        sx * ty * HorizonParam[x0 * n + y1];

                    NewHorizonParam[i * n + j] = u;
                }
                // v component
                if (ObstacleParam[i * n + j] != 0.0 && ObstacleParam[i * n + j - 1] != 0.0 && i < n - 1)
                {
                    float x = i * gridSize + h2;
                    float y = j * gridSize;
                    float u = (
                        HorizonParam[i * n + j - 1] +
                        HorizonParam[i * n + j] +
                        HorizonParam[(i + 1) * n + j - 1] +
                        HorizonParam[(i + 1) * n + j]
                    ) * 0.25f;

                    float v = VerticalParam[i * n + j];
                    x = x - Time.deltaTime * u;
                    y = y - Time.deltaTime * v;

                    //v = this.sampleField(x, y, V_FIELD);
                    {
                        x = Mathf.Max(Mathf.Min(x, n * gridSize), gridSize);
                        y = Mathf.Max(Mathf.Min(y, n * gridSize), gridSize);

                        float dx = 0.0f;
                        float dy = 0.0f;

                        dx = h2;

                        // (Math.floor((x-dx)*h1) => Find Pivot then make to normalized with h
                        int x0 = (int)(Mathf.Min(Mathf.Floor((x - dx) * h1), n - 1));
                        // Different of delta X and normalized delta X
                        float tx = (float)(((x - dx) - x0 * gridSize) * h1);
                        // Prevate to over this.numX-1
                        int x1 = (int)(Mathf.Min(x0 + 1, n - 1));

                        int y0 = (int)(Mathf.Min(Mathf.Floor((y - dy) * h1), n - 1));
                        float ty = (float)(((y - dy) - y0 * gridSize) * h1);
                        int y1 = (int)(Mathf.Min(y0 + 1, n - 1));

                        float sx = 1.0f - tx;
                        float sy = 1.0f - ty;

                        // 이웃 셀과의 합 속도를 구하기 위해선 (전부의 합 / 4) 도 존재하지만
                        // xy(uv)의 위치가 중앙이 아닌 경우엔 아래와 같이 계산한다
                        // sx, sy, tx, ty를 곱해주면서 / 4를 하지 않아도 반에 반 토막이 나서 u는 Normalized 상태이다 
                        v = sx * sy * VerticalParam[x0 * n + y0] +
                            tx * sy * VerticalParam[x1 * n + y0] +
                            tx * ty * VerticalParam[x1 * n + y1] +
                            sx * ty * VerticalParam[x0 * n + y1];
                    }

                    NewVerticalParam[i * n + j] = v;
                }
            }
        }

        // Deep Copy
        HorizonParam = NewHorizonParam;
        VerticalParam = NewVerticalParam;

        // Shallow Copy
        NewMassParam = MassParam;

        for (var i = 1; i < n - 1; i++)
        {
            for (var j = 1; j < n - 1; j++)
            {

                if (ObstacleParam[i * n + j] != 0.0)
                {
                    float u = (NewHorizonParam[i * n + j] + NewHorizonParam[(i + 1) * n + j]) * 0.5f;
                    float v = (NewVerticalParam[i * n + j] + NewVerticalParam[i * n + j + 1]) * 0.5f;
                    float x = i * gridSize + 0.5f * gridSize - Time.deltaTime * u;
                    float y = j * gridSize + 0.5f * gridSize - Time.deltaTime * v;

                    //NewMassParam[i*n + j] = this.sampleField(x,y, S_FIELD);
                    {
                        x = Mathf.Max(Mathf.Min(x, n * gridSize), gridSize);
                        y = Mathf.Max(Mathf.Min(y, n * gridSize), gridSize);

                        float dx = 0.0f;
                        float dy = 0.0f;

                        dx = h2;
                        dy = h2;

                        // (Math.floor((x-dx)*h1) => Find Pivot then make to normalized with h
                        int x0 = (int)(Mathf.Min(Mathf.Floor((x - dx) * h1), n - 1));
                        // Different of delta X and normalized delta X
                        float tx = (float)(((x - dx) - x0 * gridSize) * h1);
                        // Prevate to over this.numX-1
                        int x1 = (int)(Mathf.Min(x0 + 1, n - 1));

                        int y0 = (int)(Mathf.Min(Mathf.Floor((y - dy) * h1), n - 1));
                        float ty = (float)(((y - dy) - y0 * gridSize) * h1);
                        int y1 = (int)(Mathf.Min(y0 + 1, n - 1));

                        float sx = 1.0f - tx;
                        float sy = 1.0f - ty;

                        // 이웃 셀과의 합 속도를 구하기 위해선 (전부의 합 / 4) 도 존재하지만
                        // xy(uv)의 위치가 중앙이 아닌 경우엔 아래와 같이 계산한다
                        // sx, sy, tx, ty를 곱해주면서 / 4를 하지 않아도 반에 반 토막이 나서 u는 Normalized 상태이다 
                        NewMassParam[i * n + j] =
                            sx * sy * MassParam[x0 * n + y0] +
                            tx * sy * MassParam[x1 * n + y0] +
                            tx * ty * MassParam[x1 * n + y1] +
                            sx * ty * MassParam[x0 * n + y1];
                    }
                }
            }
        }

        MassParam = NewMassParam;
    }


    #endregion
}
