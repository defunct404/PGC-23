using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class puse : MonoBehaviour
{
    public float timer;
    public bool ispuse;
    public bool guipuse;
    public Save save; 
    void Update()
    {
        Time.timeScale = timer; if (Input.GetKeyDown(KeyCode.Escape) && ispuse == false) { ispuse = true; } else if (Input.GetKeyDown(KeyCode.Escape) && ispuse == true) { ispuse = false; }
        if (ispuse == true) { timer = 0; guipuse = true; }
        else if (ispuse == false)
        {
            timer = 1f; guipuse = false;
        }
    }
    public void OnGUI()
    {
        if (guipuse == true)
        {
            Cursor.visible = true;
            // �������� ����������� �������
            if (GUI.Button(new Rect((float)(Screen.width / 2), (float)(Screen.height / 2) - 150f, 150f, 45f), "����������"))
            { ispuse = false; timer = 0; }
            if (GUI.Button(new Rect((float)(Screen.width / 2), (float)(Screen.height / 2) - 100f, 150f, 45f), "���������"))
            {
                Save tmpsave = save.GetComponent<Save>();
                tmpsave.SaveGame();
            }
            if (GUI.Button(new Rect((float)(Screen.width / 2), (float)(Screen.height / 2) - 50f, 150f, 45f), "���������"))
            {
                ispuse = false; timer = 0;
                Save tmpsave = save.GetComponent<Save>();
                tmpsave.LoadGame();
            }
            if (GUI.Button(new Rect((float)(Screen.width / 2), (float)(Screen.height / 2), 150f, 45f), "� ����"))
            {
                ispuse = false; timer = 0; SceneManager.LoadScene("Menu");
                // ����� ��� ������� �� ������ ����������� ������ �����, �� ������ �������� �������� ����� �� ����
            }
        }
    }
}