using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class InsetCamera : MonoBehaviour
{
    public Transform backButton;
    public Renderer backRenderer;
    public Texture2D button0;
    public Texture2D button1;
    public Transform center;
    public Color textColor;
    public GameObject moviePrefab;
    public Renderer background;
    public Transform particle;
    public Renderer glow;
    public Texture2D pTexture;
    public Texture2D redTex;
    public Texture2D blueTex;
    public Renderer eRenderer;
    public int pCharge;
    public float pSize;
    public float pSpeed;
    public Transform targetObject;
    public Vector3 target;
    public float guiDist;
    public float z;
    public int p;
    public int n;
    public int e;
    private float backButtonDistance;
    private GUIStyle guiStyle;
    private float scrX;
    private float scrY;
    private RenderTexture renderTex;
    public static int insetCams;
    private GameObject movieParent;
    public Camera camera1;
    public virtual void Start()
    {
        this.guiStyle = new GUIStyle();
        this.guiStyle.normal.background = (Texture2D)Resources.Load("Tray_TIF");
        this.guiStyle.border.top = this.guiStyle.border.bottom = this.guiStyle.border.left = this.guiStyle.border.right = 9;
        this.guiStyle.padding.top = 7;
        this.guiStyle.padding.left = 10;
        this.guiStyle.font = (Font)Resources.Load("MainFont");
        this.guiStyle.normal.textColor = this.textColor;
        this.backButton.gameObject.SetActive(this.p > 0);
        this.backButtonDistance = (this.backButton.position - this.center.position).magnitude;
        this.particle.localScale = Vector3.one * this.pSize;
        this.particle.GetComponent<Renderer>().material.mainTexture = this.pTexture;
        this.glow.enabled = this.pCharge == 0 ? false : true;
        this.glow.material.mainTexture = this.pCharge > 0 ? this.redTex : this.blueTex;
        ((InsetCameraMovie) this.movieParent.GetComponent(typeof(InsetCameraMovie))).speed = this.pSpeed * 3;
        this.eRenderer.enabled = this.e > 2;
        Vector3 p1 = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, -Camera.main.transform.position.z + this.z));
        Vector3 p2 = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, -Camera.main.transform.position.z + this.z));
        this.scrX = p2.x - p1.x;
        this.scrY = p2.y - p1.y;
    }

    public virtual void OnEnable()
    {
        //renderTex = new RenderTexture(64,64,16);
        //renderTex.isPowerOfTwo = true;
        this.movieParent = UnityEngine.Object.Instantiate(this.moviePrefab);
        this.camera1 = this.movieParent.transform.Find("Camera").GetComponent<Camera>();
        this.camera1.enabled = true;
        //camera1.targetTexture = renderTex;
        this.camera1.aspect = 1;
        //background.material.mainTexture = renderTex;
        this.movieParent.transform.position = new Vector3(0, InsetCamera.insetCams * 10, 0);
        //transform.DetachChildren(); // this causes a crash, awesome
        InsetCamera.insetCams++;
    }

    public virtual void OnDisable()
    {
        //Destroy(movieParent.parent.gameObject); // fuck
        UnityEngine.Object.Destroy(this.renderTex);
    }

    public virtual void Update()
    {
        if (this.targetObject)
        {
            this.target = this.targetObject.position - Camera.main.transform.position;
            this.target.z = 0;
            Vector3 screenSpace = Camera.main.WorldToViewportPoint(this.targetObject.position);
            if (screenSpace != new Vector3(Mathf.Clamp01(screenSpace.x), Mathf.Clamp01(screenSpace.y), screenSpace.z))
            {
                float thetaForY = Mathf.Atan2(this.target.y, this.target.x);
                float thetaForX = Mathf.Atan2(this.target.x, this.target.y);
                Vector3 x2 = new Vector3(Mathf.Tan(thetaForX) * this.scrY, this.scrY, 0) * (this.target.y > 0 ? 0.5f : -0.5f);
                Vector3 y2 = new Vector3(this.scrX, Mathf.Tan(thetaForY) * this.scrX, 0) * (this.target.x > 0 ? 0.5f : -0.5f);
                Vector3 result = (x2.sqrMagnitude < y2.sqrMagnitude ? x2 : y2) + Camera.main.transform.position;
                result.z = this.z;
                this.transform.position = result;
                this.transform.LookAt(new Vector3(this.targetObject.position.x, this.targetObject.position.y, this.z), -Vector3.forward);
            }
            else
            {
                this.transform.position = Vector3.up * 1000;
            }
        }
    }

    public virtual void LateUpdate()
    {
        if (this.camera1 && this.background)
        {
            this.background.transform.rotation = Quaternion.LookRotation(-Vector3.forward); // clear z rotation
            Vector3 min = Camera.main.WorldToViewportPoint(this.background.bounds.min);
            Vector3 max = Camera.main.WorldToViewportPoint(this.background.bounds.max);
            Vector3 avg = (min + max) * 0.5f;
            avg.x = avg.x - 0.5f;
            avg.y = avg.y - 0.5f;
            avg.y = avg.y * 0.5f;
            min = min + (avg * 0.02f);
            max = max + (avg * 0.02f);
            this.camera1.rect = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
            Vector3 blah = this.background.transform.parent.forward;
            this.camera1.transform.rotation = Quaternion.Euler(0, 0, (Mathf.Atan2(blah.y, -blah.x) * Mathf.Rad2Deg) + 180);
            this.particle.transform.position = this.camera1.transform.position + (this.camera1.transform.forward * 4);
            this.eRenderer.transform.position = this.camera1.transform.position + (this.camera1.transform.forward * 4.1f);
            this.glow.transform.position = this.camera1.transform.position + (this.camera1.transform.forward * 4.2f);
            this.particle.gameObject.layer = 9;
            this.eRenderer.gameObject.layer = 9;
            this.glow.gameObject.layer = 9;
        }
        this.backButton.transform.position = this.center.position + ((Quaternion.AngleAxis((this.target.x > 0 ? 1 : -1) * -158, Vector3.forward) * this.target.normalized) * this.backButtonDistance);
        this.backButton.transform.rotation = Quaternion.identity;
    }

    public virtual void OnMouseUp()
    {
        UserAtom.p = this.p;
        UserAtom.n = this.n;
        UserAtom.e = this.e;
        Application.LoadLevel(0);
    }

    public virtual void OnMouseOver()
    {
        this.backRenderer.material.mainTexture = this.button1;
    }

    public virtual void OnMouseExit()
    {
        this.backRenderer.material.mainTexture = this.button0;
    }

    public virtual void OnGUI()
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector3 guiPosition = this.center.position + ((Quaternion.AngleAxis((this.target.x > 0 ? 1 : -1) * 158, Vector3.forward) * this.target.normalized) * this.guiDist);
        float screenWidthFloat = Screen.width;
        Vector3 pos = Camera.main.WorldToScreenPoint(guiPosition) * (screenSize.x / screenWidthFloat);
        pos.y = screenSize.y - pos.y;
        Vector2 gsize = new Vector2(59, 62);
        GUI.Box(new Rect(pos.x - (gsize.x * 0.5f), pos.y - (gsize.y * 0.5f), gsize.x, gsize.y), (((("p: " + this.p) + "\nn: ") + this.n) + "\ne: ") + this.e, this.guiStyle);
    }

    public InsetCamera()
    {
        this.pSize = 0.1f;
        this.pSpeed = 1f;
        this.guiDist = 0.8f;
        this.z = -5f;
    }

}