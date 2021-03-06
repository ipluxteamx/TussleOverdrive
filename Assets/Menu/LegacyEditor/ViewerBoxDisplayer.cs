﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewerBoxDisplayer : MonoBehaviour
{
    public Rect boxRect;
    public BattleObject fighter;
    public Camera viewerCamera;
    public SubactionData createHitboxSubData;
    
    private SpriteHandler fighterSprite;
    [SerializeField] private Transform displayBox;
    private float depth;

    private BoxResizerHandle[] handles;
    private bool selected;
    public bool isResizing = false;

    public bool SelectedForEditing{
        get{
            return selected;
        }
        set {
            selected = value;
            toggleHandles(value);
            selectedBox = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        BroadcastMessage("SetCamera",viewerCamera);

        depth = transform.localPosition.z;
        fighterSprite = fighter.GetComponent<SpriteHandler>();
        handles = GetComponentsInChildren<BoxResizerHandle>();

        //We set the "real" position of the box to the "real" center of the sprite, then set the rect center to the "pixel space" center of the sprite
        displayBox.transform.position = fighterSprite.getCenterPoint();
        boxRect.center = displayBox.transform.localPosition;

        SelectedForEditing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isResizing){
            getDataFromSubaction();
        }

        SizeToOwner();
    }

    private void toggleHandles(bool visible){
        foreach(BoxResizerHandle handle in handles){
            handle.gameObject.SetActive(visible);
        }
    }

    private void getDataFromSubaction(){
        SubactionData subaction = LegacyEditorData.instance.currentSubaction;
        if (subaction.SubactionName == "CreateHitbox"){
            float centerX = float.Parse(subaction.arguments["centerX"].data);
            float centerY = float.Parse(subaction.arguments["centerY"].data);
            float width   = float.Parse(subaction.arguments["width"].data);
            float height  = float.Parse(subaction.arguments["height"].data);

            boxRect.center = new Vector2(centerX,centerY);
            boxRect.size = new Vector2(width,height);
        }
    }

    public void setSubactionFromData(){
        SubactionData subaction = LegacyEditorData.instance.currentSubaction;
        if (subaction.SubactionName == "CreateHitbox"){
            subaction.arguments["centerX"].data = boxRect.center.x.ToString();
            subaction.arguments["centerY"].data = boxRect.center.y.ToString();
            subaction.arguments["width"].data = boxRect.width.ToString();
            subaction.arguments["height"].data = boxRect.height.ToString();
            LegacyEditorData.ChangedSubaction();
        }
    }

    public void SizeToOwner()
    {
        float scale = fighter.GetFloatVar(TussleConstants.SpriteVariableNames.PIXELS_PER_UNIT);
        if (scale == 0){
            Debug.LogWarning("Scale is zero! Setting to 1 instead");
            scale = 1f;
        }
        transform.localScale = new Vector3(1 / scale, 1 / scale, 1.0f);
        
        //float scale = 50;
        Vector3 newPosition = new Vector3(boxRect.center.x,boxRect.center.y,displayBox.transform.localPosition.z);
        displayBox.transform.localPosition = newPosition;
        displayBox.transform.localScale = new Vector3(boxRect.width, boxRect.height, 0.1f);
    }

    public void toggleSelected(){
        if (selectedBox != null && selectedBox != this) selectedBox.SelectedForEditing = false;
        SelectedForEditing = !SelectedForEditing;
    }

    
    //Static methods for unselecting hitboxes as new ones are selected
    private static ViewerBoxDisplayer selectedBox = null;
}
