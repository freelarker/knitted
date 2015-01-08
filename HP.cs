using UnityEngine;
using System.Collections;

public class health : MonoBehaviour {
public  Texture2D  bg_health_bar; //Фон health bar
public  Texture2D  bg_line_1; //Фон графы health
public  Texture2D  bg_line_2;//Фон графы armor
public  Texture2D  line_1;//Цвет графы heath
public  Texture2D  line_2;//Цвет графы armor
        
public float health_percent=100;        
public float armor_percent=100; 
        
public float x_pos_bg_health_bar=10;    
public float y_pos_bg_health_bar=10;    
public float x_size_bg_health_bar=300;  
public float y_size_bg_health_bar=100;
        
public float x_pos_bg_line_1=10;        
public float y_pos_bg_line_1=25;        
public float x_size_bg_line_1=220;      
public float y_size_bg_line_1=20;
                
public float x_pos_bg_line_2=10;        
public float y_pos_bg_line_2=64;        
public float x_size_bg_line_2=220;      
public float y_size_bg_line_2=20;
        
private float x_size_line_1=220;        
private float y_size_line_1=20;
private float x_size_line_2=220;        
private float y_size_line_2=20; 

        void Start () {
                   
        }
        
        void Update () {
        x_size_line_1=x_size_bg_line_1/100*health_percent;
        x_size_line_2=x_size_bg_line_2/100*armor_percent;
        if(health_percent>100){health_percent=100;}     
        }
        
        void OnGUI(){
        GUI.DrawTexture(new Rect(x_pos_bg_health_bar,y_pos_bg_health_bar,x_size_bg_health_bar,y_size_bg_health_bar),bg_health_bar);
        GUI.DrawTexture(new Rect(x_pos_bg_health_bar+x_pos_bg_line_1,y_pos_bg_health_bar+y_pos_bg_line_1,x_size_bg_line_1,y_size_bg_line_1),bg_line_1);
        GUI.DrawTexture(new Rect(x_pos_bg_health_bar+x_pos_bg_line_2,y_pos_bg_health_bar+y_pos_bg_line_2,x_size_bg_line_2,y_size_bg_line_2),bg_line_2);
    GUI.DrawTexture(new Rect(x_pos_bg_health_bar+x_pos_bg_line_1,y_pos_bg_health_bar+y_pos_bg_line_1,x_size_line_1,y_size_line_1),line_1);
        GUI.DrawTexture(new Rect(x_pos_bg_health_bar+x_pos_bg_line_2,y_pos_bg_health_bar+y_pos_bg_line_2,x_size_line_2,y_size_line_2),line_2);  
        GUI.Label(new Rect(x_pos_bg_health_bar+10,y_pos_bg_health_bar+3,300,50),"Health");
        GUI.Label(new Rect(x_pos_bg_health_bar+10,y_pos_bg_health_bar+y_pos_bg_line_1+20,300,50),"Armor");
        GUI.Label(new Rect(x_pos_bg_health_bar+x_pos_bg_line_1+x_size_bg_line_1+8,y_pos_bg_health_bar+y_pos_bg_line_1,300,50),health_percent+"/100");
        GUI.Label(new Rect(x_pos_bg_health_bar+x_pos_bg_line_2+x_size_bg_line_2+8,y_pos_bg_health_bar+y_pos_bg_line_2,300,50),armor_percent+"/100");
        }
}