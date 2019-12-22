using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class stacks : MonoBehaviour {

    int stack_length;
    GameObject[] go_stack;
    int stack_index;
    int count = 3;
    bool stack_receipt=false;
    bool x_axis_move;
    const float max_value = 5f;
    const float speed_value = 0.15f;
    float speed = speed_value;
    const float size = 3f;
    Vector2 stack_size = new Vector2(size, size);
    Vector3 camera_pos;
    Vector3 old_stack_pos;
    float precision;
    bool play = true;
    bool dead = false;
    float hatapayi = 0.2f;
    int combo = 0;
    public Color32 color1;
    public Color32 color2;
    public Color32 color3;
    public Color32 color4;
    public Color32 color5;
    Color32 color;
    Camera camera;
    int sayac = 0;
    public Text textt;
    int score = 0;
    public GameObject g_panel;
    public GameObject g_panel2;
    int high_score;
    public Text high_score_text;

    // Use this for initialization
    void Start () {
        high_score = PlayerPrefs.GetInt("HighScore");
        high_score_text.text = high_score.ToString();
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        camera.backgroundColor = color5;
        color = color1;
        stack_length = transform.childCount;
        go_stack = new GameObject[stack_length];
        for (int i = 0; i < stack_length; i++)
        {
            go_stack[i] = transform.GetChild(i).gameObject;
            go_stack[i].GetComponent<Renderer>().material.color = color;
        }
        stack_index = stack_length - 1;
        play = false;
    }
	void NoLongerTrack(Vector3 location, Vector3 scale,Color32 color)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localScale = scale;
        go.transform.position = location;
        go.GetComponent<Renderer>().material.color = color;
        go.AddComponent<Rigidbody>();
    }
	// Update is called once per frame
	void Update () {
        if (play)
        {
            
            if (!dead)
            {
                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Game();
                    }
                    Animate();
                    transform.position = Vector3.Lerp(transform.position, camera_pos, 0.1f);
                }
                else if (Application.platform== RuntimePlatform.Android)
                {
                    if (Input.touchCount>0 && Input.GetTouch(0).phase==TouchPhase.Began)
                    {
                        Game();
                    }
                    Animate();
                    transform.position = Vector3.Lerp(transform.position, camera_pos, 0.1f);
                }
            }
            }
        else
        {
        }
    }
    public void Game()
    {
        if (Stack_control())
        {
            Stack_Put();
            count++;
            score++;
            textt.text = score.ToString();
            if (score > high_score)
            {
                high_score = score;
            }
            color = new Color32((byte)(color.r + 5), (byte)(color.g + 5), (byte)(color.b + 5), (byte)(color.a));
            color2 = new Color32((byte)(color2.r + 5), (byte)(color2.g + 5), (byte)(color2.b + 5), (byte)(color2.a));
            if (sayac > 5)
            {
                sayac = 0;
                color1 = color2;
                color2 = color3;
                color3 = color4;
                color4 = color1;
                color = color1;
            }
            sayac++;
        }
        else
        {
            GameEnd();
        }
    }
    void Stack_Put()
    {
        old_stack_pos = go_stack[stack_index].transform.localPosition;
        if (stack_index <= 0)
        {
            stack_index = stack_length ;
        }
        stack_receipt = false;
        stack_index--;
        x_axis_move = !x_axis_move;
        camera_pos = new Vector3(0,-count+2,0);
        go_stack[stack_index].transform.localScale = new Vector3(stack_size.x, 1, stack_size.y);
        go_stack[stack_index].GetComponent<Renderer>().material.color = Color32.Lerp(go_stack[stack_index].GetComponent<Renderer>().material.color, color, 0.4f);

        camera.backgroundColor = Color32.Lerp(camera.backgroundColor,color2,0.03f);

    }
    void Animate()
    {
        if (x_axis_move)
        {
            if (!stack_receipt)
            {
                go_stack[stack_index].transform.localPosition = new Vector3(max_value, count, precision);
                stack_receipt = true;
            }
            if (go_stack[stack_index].transform.localPosition.x > max_value)
            {
                speed = speed_value * -1;
            }
            else if (go_stack[stack_index].transform.localPosition.x < -max_value)
            {
                speed = speed_value;
            }
            go_stack[stack_index].transform.localPosition += new Vector3(speed, 0, 0);
        }
        else
        {
            if (!stack_receipt)
            {
                go_stack[stack_index].transform.localPosition = new Vector3(precision, count, max_value);
                stack_receipt = true;
            }
            if (go_stack[stack_index].transform.localPosition.z > max_value)
            {
                speed = speed_value * -1;
            }
            else if (go_stack[stack_index].transform.localPosition.z < -max_value)
            {
                speed = speed_value;
            }
            go_stack[stack_index].transform.localPosition += new Vector3(0, 0, speed);
        }
    }
    bool Stack_control()
    {
        if (x_axis_move)
        {
            float diff = old_stack_pos.x - go_stack[stack_index].transform.localPosition.x;
            if (Mathf.Abs(diff) > hatapayi)
            {
                combo = 0;
                Vector3 loca;
                if (go_stack[stack_index].transform.localPosition.x > old_stack_pos.x)
                {
                    loca = new Vector3(go_stack[stack_index].transform.position.x + go_stack[stack_index].transform.localScale.x / 2, go_stack[stack_index].transform.position.y, go_stack[stack_index].transform.position.z);
                }
                else
                {
                    loca = new Vector3(go_stack[stack_index].transform.position.x - go_stack[stack_index].transform.localScale.x / 2, go_stack[stack_index].transform.position.y, go_stack[stack_index].transform.position.z);
                }
                Vector3 boyut = new Vector3(diff, 1, stack_size.y);
                stack_size.x -= Mathf.Abs(diff);
                if (stack_size.x < 0)
                {
                    return false;
                }
                go_stack[stack_index].transform.localScale = new Vector3(stack_size.x, 1, stack_size.y);
                float mid = go_stack[stack_index].transform.localPosition.x / 2 + old_stack_pos.x / 2;
                go_stack[stack_index].transform.localPosition = new Vector3(mid, count, old_stack_pos.z);
                precision = go_stack[stack_index].transform.localPosition.x;
                NoLongerTrack(loca, boyut, go_stack[stack_index].GetComponent<Renderer>().material.color);
            }
            else
            {
                combo++;
                if (combo > 2)
                {
                    stack_size.x += 0.3f;
                    if (stack_size.x > size)
                    {
                        stack_size.x = size;
                        
                    }
                    go_stack[stack_index].transform.localScale = new Vector3(stack_size.x, 1, stack_size.y);
                    go_stack[stack_index].transform.localPosition = new Vector3(old_stack_pos.x, count, old_stack_pos.z);

                }
                else
                {
                    go_stack[stack_index].transform.localPosition = new Vector3(old_stack_pos.x, count, old_stack_pos.z);
                }
                precision = go_stack[stack_index].transform.localPosition.x;
            }
        }
        else
        {
            float diff = old_stack_pos.z - go_stack[stack_index].transform.localPosition.z;
            if (Mathf.Abs(diff) > hatapayi)
            {
                combo = 0;
                Vector3 loca;
                if (go_stack[stack_index].transform.localPosition.z > old_stack_pos.z)
                {
                    loca = new Vector3(go_stack[stack_index].transform.position.x, go_stack[stack_index].transform.position.y, go_stack[stack_index].transform.position.z + go_stack[stack_index].transform.localScale.z / 2);
                }
                else
                {
                    loca = new Vector3(go_stack[stack_index].transform.position.x, go_stack[stack_index].transform.position.y, go_stack[stack_index].transform.position.z - go_stack[stack_index].transform.localScale.z / 2);
                }
                Vector3 boyut = new Vector3(stack_size.x, 1, diff);
                stack_size.y -= Mathf.Abs(diff);
                if (stack_size.y < 0)
                {
                    return false;
                }
                go_stack[stack_index].transform.localScale = new Vector3(stack_size.x, 1, stack_size.y);
                float mid = go_stack[stack_index].transform.localPosition.z / 2 + old_stack_pos.z / 2;
                go_stack[stack_index].transform.localPosition = new Vector3(old_stack_pos.x, count, mid);
                precision = go_stack[stack_index].transform.localPosition.z;
                NoLongerTrack(loca, boyut, go_stack[stack_index].GetComponent<Renderer>().material.color);
                combo++;
            }
            else
            {
                combo++;
                if (combo > 2)
                {
                    stack_size.y += 0.3f;
                    if (stack_size.y > size)
                    {
                        stack_size.y = size;
           
                    }
                    go_stack[stack_index].transform.localScale = new Vector3(stack_size.x, 1, stack_size.y);
                    go_stack[stack_index].transform.localPosition = new Vector3(old_stack_pos.x, count, old_stack_pos.z);

                }
                else
                {
                    
                    go_stack[stack_index].transform.localPosition = new Vector3(old_stack_pos.x, count, old_stack_pos.z);
                }
                precision = go_stack[stack_index].transform.localPosition.z;
            }
        }
        return true;
    }
    void GameEnd()
    {
        dead=true;
        go_stack[stack_index].AddComponent<Rigidbody>();
        g_panel.SetActive(true);
        high_score_text.text = high_score.ToString();
        PlayerPrefs.SetInt("HighScore",high_score);
    }
    public void New_Game()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
    public void PlayGame()
    {
        play = true;
        g_panel2.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
