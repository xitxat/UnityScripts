using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  NOTES
// YT: Spontaneous Simulations: wave equation
// INCLUDE: BoyancyObject.cs for floaters



public class WaveManagerSS : MonoBehaviour
{

public Material waveMaterial;
public Texture2D waveTexture;
public bool reflectiveBoundry;

float []  [] waveN, waveNm1, waveNp1; // matrices containing wave STATE INFO for current, Previous, next states


float Lx = 10;                       // texture width and height
float Ly = 10;

[SerializeField] float dx = 0.1f;   // X Density Resolution
float dy {get => dx;}               // Y Density Resolution  dy = dx
int nx, ny;                         // Resolution

public float CFL = 0.5f;
public float c =1;                  // ? wave seed may intefere if Freq is same
float dt;                           // Time Step
float t;                            // Current Time
[SerializeField] float floatToColorMultiplier = 2f;      // Visual enhancement
[SerializeField] float pluseFequency = 1f;              //  Fequency
[SerializeField] float pluseMagnitude = 1f;             //  Amplitutide
[SerializeField] Vector2Int pulsePosition = new Vector2Int(50,50);             //  
[SerializeField] float elasticity = 0.98f;              // wave dissapation






    void Start()
    {
        nx = Mathf.FloorToInt(Lx / dx);
        ny = Mathf.FloorToInt(Ly / dy);
        waveTexture = new Texture2D(nx, ny, TextureFormat.RGBA32, false);

        // creates empty fields
        waveN = new float[nx][];    // iniate 2D matricies
        waveNm1 = new float [nx][];
        waveNp1 = new float [nx][];

        for (int i=0; i<nx; i++)    // iniate rows seperatly
            {
                waveN[i] = new float[ny];
                waveNp1[i] = new float[ny];
                waveNm1[i] = new float[ny];
            }
        
        // assign texture material, names from Shader
        waveMaterial.SetTexture("_MainTex", waveTexture);
        waveMaterial.SetTexture("_DisplacementTex", waveTexture);

    }


    void WaveStep()
    {
        // calc. Time Step
        // instead of Time.deltaT
        dt = CFL * dx / c;      // recalculate dt
        t += dx;                // increment time


        if(reflectiveBoundry)
            ApplyReflectiveBoundry();
        else ApplyAbsorbtiveBoundry();


        // MATRIX
        for (int i=0; i<nx; i++)
        {
            for (int j=0; j<ny; j++)
            {
                waveNm1[i][j] = waveN[i][j]; // copy state at N to state N-1. 
                waveN[i][j] = waveNp1[i][j]; // copy state at N+1 to state N
            }
        }

        // EFFECT DRIPPING
        // Current State
        // COS used to enable PULSE val 0. if COS = 0 , CoSin = 1
        //      Constant addition to wave equation. limit visual feedback
        waveN[pulsePosition.x][pulsePosition.y] = dt * dt* 20 * pluseMagnitude * Mathf.Cos(t*Mathf.Rad2Deg * pluseFequency);


        for (int i=1; i<nx-1; i++)          // do NOT ssprocess edges
        {
            for (int j=1; j<ny-1; j++)
            {
                        // use values from adjacent element
                        // store in local var for equation
                        // get val from previous step
                float n_ij = waveN[i][j];        
                float n_ip1j = waveN[i+1][j];        
                float n_im1j = waveN[i-1][j];    
                float n_ijp1 = waveN[i][j+1];        
                float n_ijm1 = waveN[i][j-1];  
                         // get val from previous step                     
                float nm1_ij = waveNm1[i][j];        

                        // WAVE EQUATION
                waveNp1[i][j] = 2f * n_ij - nm1_ij + CFL * CFL * (n_ijm1 + n_ijp1 + n_im1j + n_ip1j -4f * n_ij);
                        //  DISSAPATION
                waveNp1[i][j] *= elasticity;
            }
        }


    }// end WaveStep


    void  ApplyReflectiveBoundry()
    {
        for (int i=1; i<nx; i++)          // DO  Process Edges
        {
            waveN[i][0] = 0f;
            waveN[i][ny-1] = 0f;
        }

        for (int j=0; j<ny; j++)        // non square textures
            {
            waveN[0][j] = 0f;
            waveN[nx-1][j] = 0f;
            }
/*             for (int j=1; j<ny; j++) // ORIGINAL square tex only
            {
            waveN[0][j] = 0f;
            waveN[ny-1][j] = 0f;
            } */
        
    }


    void  ApplyAbsorbtiveBoundry()
    {
        float v = (CFL - 1f) / (CFL + 1f);

        for (int i=0; i<nx; i++)          // DO  Process Edges
        {
            waveNp1[i][0] = waveN[i][1] + v * (waveNp1[i][1] - waveN[i] [0]);
            waveNp1[i][ny-1] = waveN[i][ny-2] + v * (waveNp1[i][ny-2] - waveN[i] [ny-1]);
        }
            for (int j=0; j<ny; j++)
                        {
            waveNp1[0][j] = waveN[1][j] + v * (waveNp1[1][j] - waveN[0][j]);
            waveNp1[nx-1][j] = waveN[nx - 2][j] + v * (waveNp1[nx-2][j] - waveN[nx -1][j]);
            }

/*             {        // ORIGINAL square tex
            waveNp1[0][j] = waveN[1][j] + v * (waveNp1[1][j] - waveN[0][j]);
            waveNp1[ny-1][j] = waveN[ny - 2][j] + v * (waveNp1[ny-2][j] - waveN[ny -1][j]);
            } */
        
    }


                    // Apply values from current Matrice to Texture
                    // Added multiplier for enhancement
                    // floatToColor : directly change color
    void ApplyMatrixToTexture(float[][] state, ref Texture2D tex, float floatToColorMultiplier)      
    {
        // iterate thru all the elements and save as a greyscale colour
        // MATRIX
        for (int i=0; i<nx; i++)
        {
            for (int j=0; j<ny; j++)
            {
                float val = state[i][j] * floatToColorMultiplier;
                tex.SetPixel(i, j, new Color(val+0.5f,val+0.5f,val+0.5f, 1f)); // paint greys w. APLHA , 0.5 to get average about grey
            }
        }
        // wave texture applied at run time. Don't drop into waveManager script @ wave Tex
        tex.Apply();
    }      



                    // Cast Ray from Mouse to Screen
                    // If material has collider and is hit ...  
                    // Check CAMERA selcetion 
                    // only change dirVector only on hit                
void MousePositionOnTexture(ref Vector2Int pos) 
{
    RaycastHit hit;
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    if  (Physics.Raycast(ray, out hit))
    {
        pos = new Vector2Int((int)(hit.textureCoord.x * nx), (int)(hit.textureCoord.y * ny));
    }

}





                    // ~~~~~~~~~~~~~~Update is called once per frame
    void Update()
    {

        MousePositionOnTexture(ref pulsePosition);
        WaveStep();
        ApplyMatrixToTexture(waveN, ref waveTexture, floatToColorMultiplier);
    }
}
