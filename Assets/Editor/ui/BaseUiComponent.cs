
namespace PlayFab.Editor
{
using UnityEngine;
using UnityEditor;
using System.Collections;



public class BaseUiComponent : Editor {
    [System.Flags]
    public enum ComponentSettings 
    { 
        none = 0, 
        useScrollBar = 1 << 0, 
        fillHorizontal = 1 << 1, 
        fillVertical = 1 << 2  
    }

    public ComponentSettings settings = (ComponentSettings.none);

    public System.Action postDrawCall;

    public GUIStyle style;

    public Color fillColor = Color.gray;
    public Color borderColor = Color.cyan;
    public float opacity = 1.0f;

    public int borderSize = 0;
    public Rect bounds;

    public Rect parentBounds = new Rect(0,0,1,1);

    public enum AnchorPoints { Null, None, TopLeft, TopCenter, TopRight, CenterLeft, Center, CenterRight, BottomLeft, BottomCenter, BottomRight }
    public AnchorPoints anchor = AnchorPoints.Null;

    // states
    public bool isMouseOver = false;
    public bool isAnimating = false;
    private Vector2 scrollPos = Vector2.zero;

    private void Init()
    {
        if(this.anchor == AnchorPoints.None)
        {
            DropAnchor();
           
        }

        // TODO add condition here to make this optional
        PlayFabEditor.UpdateLoopTick += Update;
    }

    public void Init(Rect b, Rect p, Color fc)
    {
        this.bounds = b;
        this.parentBounds = p;
        this.fillColor = fc;
        this.Init();
    }

    public void Init(Rect b, Rect p, Color fc, GUIStyle st)
    {
        this.bounds = b;
        this.parentBounds = p;
        this.fillColor = fc;
        this.style = st;
        this.Init();
    }

//    public void OnEnable()
//    {
//            //PlayFabEditor.UpdateLoopTick += Update;
//            PlayFabEditor.UpdateLoopTick += Update;
//    }

    public void OnDisable()  // destructor
    {
        // cleanup statements...
        PlayFabEditor.UpdateLoopTick -= Update;
    }

    public void OnDestroy()
    {
        PlayFabEditor.UpdateLoopTick -= Update;
    }

    public void Draw()
    {
        this.isMouseOver = MouseHoverCheck();
        EditorGUILayout.BeginFadeGroup(opacity);
        bool fillHorizontal = (this.settings & ComponentSettings.fillHorizontal) != 0;
        bool fillVertical = (this.settings & ComponentSettings.fillVertical) !=  0;

         //
        if((this.settings & ComponentSettings.useScrollBar) == ComponentSettings.useScrollBar)
        {
                this.scrollPos = EditorGUILayout.BeginScrollView(scrollPos, this.style, GUILayout.ExpandHeight(fillVertical), GUILayout.MaxWidth(fillHorizontal ? EditorGUIUtility.currentViewWidth : this.bounds.width));
        }
        else
        {
            GUILayout.BeginArea(bounds, this.style);
        }



       // Positioning();
//        EditorGUILayout.BeginHorizontal();
//            //EditorGUI.DrawRect(this.bounds, this.fillColor);
//        EditorGUILayout.EndHorizontal();

//        GUILayout.TextArea(Event.current.mousePosition.ToString());
//
//        EditorGUILayout.TextArea(this.bounds.ToString());
//        EditorGUILayout.TextArea(this.parentBounds.ToString());
//        if(GUILayout.Button("Submit"))
//        {
//            BaseUiAnimationController.StartAlphaFade(1, 0, this);
//        }




        PostDraw();
    }

    public virtual void PostDraw() 
    {
        if(this.postDrawCall != null)
        {
            this.postDrawCall();
        }
        // override this for additional components, but either call base.PostDraw or manually call the following:
        if((this.settings & ComponentSettings.useScrollBar) == ComponentSettings.useScrollBar)
        {
            EditorGUILayout.EndScrollView();
        }
        else
        {
            GUILayout.EndArea();
        }
        EditorGUILayout.EndFadeGroup();

    }

    public virtual void Update()
    {
        //Debug.Log("Update()");
        Draw();
    }


    //TODO this is not the proper place to clean this up.


    public Rect GetInnerBounds()
    {
        //TODO update this after we can actually calculate this.
        throw new System.NotImplementedException();
    }

    private void Positioning(bool reAnchor = false)
    {
        

        if(reAnchor == true)
        {
            DropAnchor();
        }

        Vector2 rectAdjustment = Vector2.zero;
        if((this.settings & ComponentSettings.fillVertical) == ComponentSettings.fillVertical && isAnimating == false)
        {
            rectAdjustment.y = this.parentBounds.height - this.bounds.height;
        }

        if((this.settings & ComponentSettings.fillHorizontal) == ComponentSettings.fillHorizontal && isAnimating == false)
        {
            rectAdjustment.x = this.parentBounds.width - this.bounds.width;
        }

        if(rectAdjustment != Vector2.zero)
        {
            this.bounds = new Rect(this.bounds.x, this.bounds.y, this.bounds.width + rectAdjustment.x, this.bounds.height + rectAdjustment.y);
        }
    }

    //depends on the default way to make a component auto scale x or y

    private void DropAnchor()
    {
        float currW = EditorGUIUtility.currentViewWidth;

        switch(this.anchor)
        {
            case AnchorPoints.TopLeft:
                   this.bounds = new Rect(0,0, this.bounds.width, this.bounds.height);
            break; 

            case AnchorPoints.TopCenter:
                    this.bounds = new Rect(currW / 2 - this.bounds.width / 2 ,0, this.bounds.width, this.bounds.height);
            break; 

            case AnchorPoints.TopRight:
                    this.bounds = new Rect(currW - this.bounds.width, 0, this.bounds.width, this.bounds.height);
            break; 
        } 
    }

    private bool MouseHoverCheck()
    {   
        //Note: In Unity the screen space y coordinate varies from zero at the top edge of the window to a maximum at the bottom edge of the window. This is different from what you might expect.
        Vector2 currentPos = Event.current.mousePosition;

        if(this.bounds.Contains(currentPos))
        {
            return true;
        }
        else
        {
            
            return false;
        }
    }

    public BaseUiComponent()
    {
        
    }

    public BaseUiComponent(Rect b, Rect p, Color fc)
    {
        this.bounds = b;
        this.parentBounds = p;
        this.fillColor = fc;
        this.Init();
    }

    public BaseUiComponent(Rect b, Rect p, Color fc, GUIStyle st)
    {
        this.bounds = b;
        this.parentBounds = p;
        this.fillColor = fc;
        this.style = st;
        this.Init();
    }

}
}
