using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{

    public GameObject bottlePrefab;

    public BottleController[] bottles;

    public int nbOfBottleDone=0;



    // Start is called before the first frame update
    void Start()
    {
        //CreateBottle();
        
        
        
    }
    
    //void CreateBottle()
    //{
    //    Vector3 pos = new Vector3(-0.5f, 0);
    //    for (int i = 0; i < nbOfBottles; i++)
    //    {
    //        pos.x += 0.3f;
    //        Instantiate(bottlePrefab, pos, Quaternion.identity);

    //    }
    //}
    

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if(hit.collider!=null)
            {
                if(hit.collider.GetComponent<BottleController>()!=null)
                {
                    if(bottles[0]==null)
                    {
                        bottles[0] = hit.collider.GetComponent<BottleController>();
                        
                    } 
                    else
                    {
                        
                        if (bottles[0] == hit.collider.GetComponent<BottleController>())
                        {
                            bottles[0] = null;
                        }
                        else
                        {
                            
                            bottles[1] = hit.collider.GetComponent<BottleController>();
                           
                            bottles[0].bottleController = bottles[1];
                            

                            bottles[0].UpdateTopCLValue();
                            bottles[1].UpdateTopCLValue();

                            if(bottles[1].FillBottleCheck(bottles[0].topCL)==true)
                            {
                                
                                bottles[0].StartColorTransfer();
                                
                                bottles[0] = null;
                                bottles[1] = null;
                            }
                            else
                            {
                                bottles[0] = null;
                                bottles[1] = null;
                            }
                        }
                    }
                }    
            }
            
        }
        
        
        
    }
}
