using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthOverTarget : MonoBehaviour
{
    public Transform target;
    //private string textToDisplay;
    private Canvas uican;
    public bool displayName = true;
    public bool displayTAG = false;
    private Text nametag;
    private Text healthtag;
    private UnitBehavior uni;
    //private Renderer rend;
    // Use this for initialization
    void Start()
    {
        target = gameObject.transform;

        uican = GameObject.Find("UICanvas").GetComponent<Canvas>();
        GameObject prefab = Resources.Load<GameObject>("CanvasTextObject");
        //(GameObject)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Resources/CanvasTextObject.prefab", typeof(GameObject));
        Vector3 pos = Camera.main.ScreenToViewportPoint(target.transform.position);
        nametag = Instantiate(prefab, pos, uican.transform.rotation).GetComponent<Text>();
        healthtag = Instantiate(prefab, pos, uican.transform.rotation).GetComponent<Text>();
        //Parent to the panel
        nametag.transform.SetParent(uican.transform, false);
        healthtag.transform.SetParent(uican.transform, false);
        nametag.transform.SetAsFirstSibling();
        healthtag.transform.SetAsFirstSibling();
        //Set the text box's text element font size and style:
        nametag.fontSize = 20;
        healthtag.fontSize = 14;



        nametag.text =target.name;
        nametag.transform.parent = uican.transform;
        uni = target.GetComponent<UnitBehavior>();
        healthtag.text = uni.GetHealth().x.ToString();
        //TextMesh tm = GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        if (uni != null)
        {
            nametag.text = uni.name;
            //nametag.transform.position = Input.mousePosition;
            Vector3 raw = Camera.main.WorldToScreenPoint(target.transform.position);
            nametag.transform.position = raw + new Vector3(0, -nametag.rectTransform.rect.height *1.0f, 0);

            Vector3 v = uni.GetHealth();
            healthtag.text = ((int)(v.x - v.y - v.z)).ToString() + " / " + (int)(v.x - v.y);
            healthtag.transform.position = raw + new Vector3(0, -nametag.rectTransform.rect.height * 1.0f -healthtag.rectTransform.rect.height * 1.0f, 0);
            //Debug.Log("pulled name from unitbehavior");
        }
        //nameDisplayer();
        //tagDisplayer();
    }

    void LateUpdate()
    {
        //Make the text allways face the camera
        //transform.rotation = Camera.main.transform.rotation;
    }

    private void OnDestroy()
    {
        destroytags();
    }

    /// <summary>
    /// destroy text objects we owned
    /// </summary>
    public void destroytags()
    {
        if (nametag != null)
            GameObject.Destroy(nametag.gameObject);
        if (healthtag != null)
            GameObject.Destroy(healthtag.gameObject);


    }
}