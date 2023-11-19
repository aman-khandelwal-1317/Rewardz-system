using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddButtons : MonoBehaviour
{

  [SerializeField]  private Transform panel;
    [SerializeField] private GameObject btn;
    [SerializeField] private Sprite[] sprites;

   private void Awake()
    {
        for(int i = 0; i < 4; i++)
        {
            GameObject button = Instantiate(btn);
            button.name = i.ToString();
            button.transform.SetParent(panel,false);
            button.GetComponent<Image>().sprite = sprites[i];
        }
    }
}
