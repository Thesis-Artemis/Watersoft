using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BottleController : MonoBehaviour
{
    public Color[] bottleCLs;
    public SpriteRenderer bottleMask;
    public SpriteRenderer[] wave;
    public AnimationCurve ScaleNRotation;
    public AnimationCurve FillTubeCurve;
    public AnimationCurve RotationSpeedMutiplier;
    public GameObject particalST;
    public AudioSource audioFinish;
    public AudioSource audioWaterPour;

    public float[] fillTubes;
    public float[] rotationVL;
    private int rotationIndex = 0;

    public GameController gameController;
    public BottleController bottleController;
    public bool justThisBottle = false;
    private int nbOfColorsToTransfer = 0;


    [Range(0,4)]
    public int nbOfColorsInBottle;

    public Color topCL;
    public int nbOfTopColorLayers;

    public int nbOfBottleDone;

    public Transform leftRotationPoint;
    public Transform rightRotationPoint;
    public Transform choosenRotationPoint;

    private float directionMultiplier = 1.0f;

    Vector3 originalPosition;
    Vector3 startPosition;
    Vector3 endPosition;

    public LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        bottleMask.material.SetFloat("_FillTube",fillTubes[nbOfColorsInBottle]);
        lineRenderer.enabled = false;
        originalPosition = transform.position;
        UpdateTopCLValue();
        UpdateCLOnShader();
              
    }

    // Update is called once per frame
    void Update()
    {
       
        
        if (Input.GetKeyUp(KeyCode.P)&& justThisBottle==true)
        {
            UpdateTopCLValue();

            if(bottleController.FillBottleCheck(topCL))
            {
                ChooseRotationPointAndDirection();
                nbOfColorsToTransfer = Mathf.Min(nbOfTopColorLayers, 4 - bottleController.nbOfColorsInBottle);
                for(int i = 0; i < nbOfColorsToTransfer; i++)
                {
                    bottleController.bottleCLs[bottleController.nbOfColorsInBottle + i] = topCL;
                }
                bottleController.UpdateCLOnShader();
            }
            calculaRotationIndex(4 - bottleController.nbOfColorsInBottle);
            StartCoroutine(RotateBottle());
        }    
    }
    public void StartColorTransfer()
    {
        ChooseRotationPointAndDirection();
        nbOfColorsToTransfer = Mathf.Min(nbOfTopColorLayers, 4 - bottleController.nbOfColorsInBottle);
        for (int i = 0; i < nbOfColorsToTransfer; i++)
        {
            if((bottleController.nbOfColorsInBottle+nbOfColorsToTransfer)<4)
            {
                bottleController.bottleCLs[bottleController.nbOfColorsInBottle + i] = topCL;
                bottleController.bottleCLs[bottleController.nbOfColorsInBottle + i+1] = topCL;
            }
            else
            {
                bottleController.bottleCLs[bottleController.nbOfColorsInBottle + i] = topCL;
            }
           
        }
        bottleController.UpdateCLOnShader();
        bottleController.UpdateTopCLValue();
        calculaRotationIndex(4 - bottleController.nbOfColorsInBottle);
        StartCoroutine(MoveBottle());
    }    


    IEnumerator MoveBottle()
    {
        startPosition = transform.position;
        if(choosenRotationPoint==leftRotationPoint)
        {
            endPosition = bottleController.rightRotationPoint.position;
        }
        else
        {
            endPosition = bottleController.leftRotationPoint.position;
        }

        float t = 0;
        while(t<=1)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition,t);
            t += Time.deltaTime * 2;
            
            
            yield return new WaitForEndOfFrame();
        }

        transform.position = endPosition;
        bottleController.particalST.SetActive(false);
        StartCoroutine(RotateBottle());
        

    }

    IEnumerator MoveBottleBack()
    {
        startPosition = transform.position;
        endPosition = originalPosition;
        

        float t = 0;
        while (t <= 1)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            t += Time.deltaTime * 2;
            yield return new WaitForEndOfFrame();
        }

        transform.position = endPosition;
        if (SceneManager.GetActiveScene().buildIndex < 3 && gameController.nbOfBottleDone.Equals(SceneManager.GetActiveScene().buildIndex+1) || SceneManager.GetActiveScene().buildIndex == 3 && gameController.nbOfBottleDone.Equals(SceneManager.GetActiveScene().buildIndex))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
    void UpdateCLOnShader()
    {
        bottleMask.material.SetColor("CL_Red",bottleCLs[0]);
        wave[0].color = bottleCLs[3];
        bottleMask.material.SetColor("CL_Blue", bottleCLs[1]);
        wave[1].color = bottleCLs[2];
        bottleMask.material.SetColor("CL_Yellow", bottleCLs[2]);
        wave[2].color = bottleCLs[1];
        bottleMask.material.SetColor("CL_Pink", bottleCLs[3]);
        wave[3].color = bottleCLs[0];


    }
    public float TimeToRotate = 1.0f;
    IEnumerator RotateBottle()
    {
        float t = 0;
        float lerpValue;
        float angleValue;
        float lastAngleValue = 0;

        while (t < TimeToRotate)
        {
            lerpValue = t / TimeToRotate;
            angleValue = Mathf.Lerp(0.0f,directionMultiplier* rotationVL[rotationIndex], lerpValue);
            wave[4 - nbOfColorsInBottle].transform.eulerAngles = new Vector3(0, 0, 0);

            //wave[1].
            //wave[2].gameObject.SetActive(true);
            //wave[3].gameObject.SetActive(true);

            //wave[1].
            //wave[2].transform.eulerAngles = new Vector3(0, 0, 0);
            //wave[3].transform.eulerAngles = new Vector3(0, 0, 0);
            //transform.eulerAngles = new Vector3(0, 0, angleValue);
            transform.RotateAround(choosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);
            bottleMask.material.SetFloat("_SARM", ScaleNRotation.Evaluate(angleValue));
            
            if(fillTubes[nbOfColorsInBottle]>FillTubeCurve.Evaluate(angleValue) )
            {

                if(lineRenderer.enabled==false)
                {
                    lineRenderer.startColor = topCL;
                    lineRenderer.endColor = topCL;

                    lineRenderer.SetPosition(0, choosenRotationPoint.position);
                    lineRenderer.SetPosition(1, choosenRotationPoint.position- Vector3.up*0.5f );
                    wave[4 - nbOfColorsInBottle].gameObject.SetActive(true);
                    lineRenderer.enabled = true;
                    audioWaterPour.Play();


                }    
                bottleMask.material.SetFloat("_FillTube", FillTubeCurve.Evaluate(angleValue));
                
                bottleController.FillUp(FillTubeCurve.Evaluate(lastAngleValue) - FillTubeCurve.Evaluate(angleValue));
                
                
            }    


            if (bottleMask.material.GetFloat("_FillTube") <= 0.07)
            {
                wave[0].gameObject.SetActive(false);
            }if (bottleMask.material.GetFloat("_FillTube") <= 0.0015)
            {
                wave[1].gameObject.SetActive(false);
            }
            if (bottleMask.material.GetFloat("_FillTube") <= -0.07)
            {
                wave[2].gameObject.SetActive(false);
            }
            if (bottleMask.material.GetFloat("_FillTube") <= -0.1)
            {
                wave[3].gameObject.SetActive(false);
                
            }
            

            t += Time.deltaTime * RotationSpeedMutiplier.Evaluate(angleValue);
            lastAngleValue = angleValue;
            yield return new WaitForEndOfFrame();


        }
        angleValue = directionMultiplier* rotationVL[rotationIndex];
        
        //transform.eulerAngles = new Vector3(0, 0, angleValue);
        bottleMask.material.SetFloat("_SARM", ScaleNRotation.Evaluate(angleValue));
        bottleMask.material.SetFloat("_FillTube", FillTubeCurve.Evaluate(angleValue));

        nbOfColorsInBottle -= nbOfColorsToTransfer;
        bottleController.nbOfColorsInBottle += nbOfColorsToTransfer;

        lineRenderer.enabled = false;
        if(CheckAnotherBottle())
        {
            gameController.nbOfBottleDone++;            
            bottleController.particalST.SetActive(true);
            audioFinish.Play();
            
        }    
        StartCoroutine(RotateBottleBack());
       



    }
    
    IEnumerator RotateBottleBack()
    {
        float t = 0;
        float lerpValue;
        float angleValue;
        float lastAngleValue=directionMultiplier*rotationVL[rotationIndex];

        wave[0].gameObject.SetActive(false);
        wave[1].gameObject.SetActive(false);
        wave[2].gameObject.SetActive(false);
        wave[3].gameObject.SetActive(false);

        while (t < TimeToRotate)
        {
            lerpValue = t / TimeToRotate;
            angleValue = Mathf.Lerp(directionMultiplier* rotationVL[rotationIndex], 0.0f, lerpValue);

            //transform.eulerAngles = new Vector3(0, 0, angleValue);
            transform.RotateAround(choosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);
            bottleMask.material.SetFloat("_SARM", ScaleNRotation.Evaluate(angleValue));
            lastAngleValue = angleValue;
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();


        }
        UpdateTopCLValue();
        
        
        angleValue = 0;

        transform.eulerAngles = new Vector3(0, 0, angleValue);
        bottleMask.material.SetFloat("_SARM", ScaleNRotation.Evaluate(angleValue));

        StartCoroutine(MoveBottleBack());
        
 
    }
    bool CheckAnotherBottle()
    {
        if(bottleController.nbOfColorsInBottle.Equals(4))
        {

            for (int i = 0; i < 3; i++)
            {
                if (!bottleController.bottleCLs[i].Equals(bottleController.bottleCLs[i + 1]))
                {
                    return false;
                }

            }
            return true;
        }
        return false;
    }   
    
    public void UpdateTopCLValue()
    {

        
        if (nbOfColorsInBottle != 0)
        {
            nbOfTopColorLayers = 1;
            topCL = bottleCLs[nbOfColorsInBottle - 1];

            if (nbOfColorsInBottle==4)
            {
                if (bottleCLs[3].Equals(bottleCLs[2]))
                {
                    nbOfTopColorLayers = 2;
                    if (bottleCLs[2].Equals(bottleCLs[1]))
                    {
                        nbOfTopColorLayers = 3;
                        if (bottleCLs[1].Equals(bottleCLs[0]))
                        {
                            nbOfTopColorLayers = 4;

                        }
                    }
                }
            }else if (nbOfColorsInBottle.Equals(3))
            {
                if (bottleCLs[2].Equals(bottleCLs[1]))
                {
                    nbOfTopColorLayers = 2;
                    if (bottleCLs[1].Equals(bottleCLs[0]))
                    {
                        nbOfTopColorLayers = 3;

                    }
                }
            }else if (nbOfColorsInBottle.Equals(2))
            {
                if (bottleCLs[1].Equals(bottleCLs[0]))
                {
                    nbOfTopColorLayers = 2;

                }
            }
            rotationIndex = 3 - (nbOfColorsInBottle - nbOfTopColorLayers);
        }  
    }
    public bool FillBottleCheck(Color clToCheck)
    {
        if(nbOfColorsInBottle==0)
        {
            return true;
        }
        else
        {
            if(nbOfColorsInBottle==4)
            {
                return false;
            } 
            else
            {
                if (topCL.Equals(clToCheck))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            
        }
    } 
    private void calculaRotationIndex(int nbOfEmptySpacesOn2Bottle)
    {
        rotationIndex = 3 - (nbOfColorsInBottle - Mathf.Min(nbOfEmptySpacesOn2Bottle, nbOfTopColorLayers));
    }

    private void  FillUp (float fillTubeToAdd)
    {   if(bottleMask.material.GetFloat("_FillTube") + fillTubeToAdd>0.15)
        {
            bottleMask.material.SetFloat("_FillTube", 0.15f);
        }
        else
        {
            bottleMask.material.SetFloat("_FillTube", bottleMask.material.GetFloat("_FillTube") + fillTubeToAdd);
        }   
        
    }
    private void ChooseRotationPointAndDirection()
    {
        if(transform.position.x>bottleController.transform.position.x)
        {
            choosenRotationPoint = leftRotationPoint;
            directionMultiplier = -1.0f;
        }
        else
        {
            choosenRotationPoint = rightRotationPoint;
            directionMultiplier = 1.0f;
        }
    }    
}
