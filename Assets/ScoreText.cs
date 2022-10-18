using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreText : MonoBehaviour
{

    
    public TextMeshProUGUI text;
    public int score =0 ;

    // Start is called before the first frame update
   

   void Start()
   {
    text.text="Current Score"+ score.ToString();
   }

    public void addScore()
    {
        score++;
        showScore();
    }

    public void showScore()
    {
    
        text.text="Current Score: "+score.ToString();
    }
}
