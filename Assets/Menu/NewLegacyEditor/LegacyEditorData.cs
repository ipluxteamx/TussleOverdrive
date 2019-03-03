﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// This is the model for the legacy editor. Each variable has properties that 
/// </summary>
public class LegacyEditorData : MonoBehaviour
{
    public static LegacyEditorData instance;
    public static ContextualPanelData contextualPanel;
    public static AnchorPositions anchors;

    [Header("Component Accessors")]
    [SerializeField]
    private PanelHider ShadowRealm;
    [SerializeField]
    private FileBrowser fileBrowser;

    [Header("Fighter Data")]
    public string FighterDirName;

    #region Loaded Fighter - the currently loaded fighter info
    [SerializeField]
    private FighterInfo _loadedFighter;
    public bool loadedFighterDirty { get; private set; }

    public FighterInfo loadedFighter
    {
        get { return _loadedFighter; }
        private set
        {
            _loadedFighter = value;
            loadedFighterDirty = true;
        }
    }
    #endregion
    #region Loaded Action File - the currently loaded action set
    [SerializeField]
    private ActionFile _loadedActionFile;
    public bool loadedActionFileDirty { get; private set; }

    public ActionFile loadedActionFile
    {
        get { return _loadedActionFile; }
        private set
        {
            _loadedActionFile = value;
            loadedActionFileDirty = true;
        }
    }
    #endregion
    #region Current Action - the action that is currently selected from the left panel
    [SerializeField]
    private DynamicAction _currentAction;
    public bool currentActionDirty { get; private set; }

    public DynamicAction currentAction
    {
        get { return _currentAction; }
        set
        {
            _currentAction = value;
            currentActionDirty = true;
        }
    }
    #endregion
    [Header("Navigation")]
    #region Left Dropdown - what is selected on the left dropdown menu
    [SerializeField]
    private string _leftDropdown;
    public bool leftDropdownDirty { get; private set; }

    public string leftDropdown
    {
        get { return _leftDropdown; }
        set
        {
            _leftDropdown = value;
            leftDropdownDirty = true;
        }
    }
    #endregion
    #region Right Dropdown - what is selected on the right dropdown menu
    [SerializeField]
    private string _rightDropdown;
    public bool rightDropdownDirty { get; private set; }

    public string rightDropdown
    {
        get { return _rightDropdown; }
        set
        {
            _rightDropdown = value;
            rightDropdownDirty = true;
        }
    }
    #endregion
    #region Subaction Group - The current selected Subaction Group
    [SerializeField]
    private string _subactionGroup;
    public bool subactionGroupDirty { get; private set; }

    public string subactionGroup
    {
        get {
            if (_subactionGroup == "Current Frame")
            {
                return SubactionGroup.ONFRAME(currentFrame);
            }
            return _subactionGroup;
        }
        set
        {
            _subactionGroup = value;
            subactionGroupDirty = true;
        }
    }
    #endregion
    #region Current Frame - the frame that is currently being shown in the viewer and right pane
    [SerializeField]
    private int _currentFrame;
    public bool currentFrameDirty { get; private set; }

    public int currentFrame
    {
        get { return _currentFrame; }
        set
        {
            _currentFrame = value;
            currentFrameDirty = true;
        }
    }
    #endregion
    #region Current Subaction - the subaction that is currently selected for editing
    private SubactionData _currentSubaction = null;
    public bool currentSubactionDirty { get; private set; }

    public SubactionData currentSubaction
    {
        get { return _currentSubaction; }
        set
        {
            Debug.Log("Changing Subaction");
            _currentSubaction = value;
            currentSubactionDirty = true;
        }
    }
    #endregion
    #region Contextual Panel Controller - the ContextualPanelData of the currently visible Contextual Panel
    [SerializeField]
    private ContextualPanelData _contextualPanelController;
    public bool contextualPanelControllerDirty { get; private set; }

    public ContextualPanelData contextualPanelController
    {
        get { return _contextualPanelController; }
        private set
        {
            _contextualPanelController = value;
            contextualPanelControllerDirty = true;
        }
    }
    #endregion

    //TODO
    public string contextFighterCategory;
    public string contextSubactionCategory;
    public string actionSearchText;
    public string spriteSearchText;
    public string sortOrder;
    
    /// <summary>
    /// Set the singleton instance at OnEnable time, the earliest we can
    /// </summary>
    private void OnEnable()
    {
        instance = this;
        anchors = GetComponent<AnchorPositions>();
    }

    /// <summary>
    /// Fire all the changes to things that were modified in the editor
    /// </summary>
    private void Start()
    {
        loadedFighter.LoadDirectory(FighterDirName);
        loadedFighterDirty = true;
        loadedActionFile = ActionFile.LoadActionsFromFile(FighterDirName, loadedFighter.action_file_path);
        loadedActionFileDirty = true;
        currentActionDirty = true;
        leftDropdownDirty = true;
        rightDropdownDirty = true;
        currentFrameDirty = true;
        currentSubactionDirty = true;
        currentAction = loadedActionFile.GetFirst();
        FireModelChange();
    }

    private void Update()
    {
        CheckKeyboardShortcuts();
        //TODO screen with hook here maybe? Or maybe it should be in the individual components...
        //Debug.Log(Screen.width);
    }

    public void LoadNewFighter(FighterInfo fInfo)
    {
        FighterDirName = fInfo.directory_name;

        loadedFighter = fInfo;
        loadedFighterDirty = true;
        loadedActionFile = ActionFile.LoadActionsFromFile(FighterDirName, fInfo.action_file_path);
        loadedActionFileDirty = true;
        currentActionDirty = true;
        leftDropdownDirty = true;
        rightDropdownDirty = true;
        currentFrameDirty = true;
        currentSubactionDirty = true;
        currentAction = loadedActionFile.GetFirst();
        FireModelChange();
    }
    /// <summary>
    /// Calls everything's OnModelChanged methods, then unsets the dirty bits for everything
    /// </summary>
    public void FireModelChange()
    {
        BroadcastMessage("OnModelChanged");
        
        //After the broadcast, clear all the "dirty" bits
        loadedFighterDirty = false;
        loadedActionFileDirty = false;
        currentActionDirty = false;
        leftDropdownDirty = false;
        rightDropdownDirty = false;
        currentFrameDirty = false;
        currentSubactionDirty = false;
    }

    private Stack<LegacyEditorAction> undoList = new Stack<LegacyEditorAction>();
    private Stack<LegacyEditorAction> redoList = new Stack<LegacyEditorAction>();
    
    public void Undo()
    {
        //If we have no history we don't have anything to undo and just quietly don't do anything
        if (undoList.Count > 0)
        {
            LegacyEditorAction act = undoList.Pop();
            act.undo();
            Debug.Log("Undoing Action: " + act);
            redoList.Push(act);
            FireModelChange();
        }
    }

    public void Redo()
    {
        //If we have nothing to redo then we just quietly don't do anything
        if (redoList.Count > 0)
        {
            LegacyEditorAction act = redoList.Pop();
            act.execute();
            undoList.Push(act);
            FireModelChange();
        }
    }

    public void DoAction(LegacyEditorAction act)
    {
        //Once we do a new thing, our redo list blows the hell up
        redoList.Clear();
        //Then actually do the thing
        act.execute();
        //This is a special tool that will help us later
        undoList.Push(act);
        FireModelChange();
    }

    private void CheckKeyboardShortcuts()
    {
        //Check for CTRL shortcuts. Since the editor keyboard shortcuts can't be disabled, if you're in editor, it'll activate without ctrl
        //TODO remove this for final build so testing is easier.
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Undo();
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                Redo();
            }
        }
    }

    public void LoadFighterClicked()
    {
        fileBrowser.BrowseForJSON(FileLoader.FighterDir, LoadFighterFromFile);
        fileBrowser.SetErrorText("File does not contain a fighter");
    }

    public void SaveFighterClicked()
    {
        loadedFighter.Save();
        string path = FileLoader.PathCombine(FileLoader.GetFighterPath(FighterDirName), loadedFighter.action_file_path);
        loadedActionFile.WriteJSON(path);
        Debug.Log("Saved Fighter: " + path);
    }

    private bool LoadFighterFromFile(FileInfo info)
    {
        FighterInfo fInfo = FighterInfo.LoadFighterInfoFile(info.DirectoryName, info.Name);
        if (fInfo != null)
        {
            LoadNewFighter(fInfo);
            return true;
        }
        return false;
    }

    #region static helper methods
    /// <summary>
    /// Banish a panel to the shadow realm, a place where panels no longer in use go.
    /// </summary>
    /// <param name="panelToBanish">The panel to banish. Can technically be any gameObject, but it's mostly used for panels.</param>
    public static void Banish(GameObject panelToBanish)
    {
        if (instance == null || instance.ShadowRealm == null)
        {
            FindObjectOfType<PanelHider>().Banish(panelToBanish);
        } else
        {
            instance.ShadowRealm.Banish(panelToBanish);
        }
    }

    /// <summary>
    /// Returns a panel to the place it was banished from intact.
    /// </summary>
    /// <param name="panelToBanish">An object in the shadow realm to be returned from whence it came</param>
    public static void Unbanish(GameObject panelToBanish)
    {
        if (instance == null || instance.ShadowRealm == null)
        {
            FindObjectOfType<PanelHider>().Unbanish(panelToBanish);
        }
        else
        {
            instance.ShadowRealm.Unbanish(panelToBanish);
        }
    }

    /// <summary>
    /// Returns a panel to the the given parent
    /// </summary>
    /// <param name="panelToBanish">An object in the shadow realm to be returned from whence it came</param>
    /// <param name="destinationPanel">The object to return this object to</param>
    public static void Unbanish(GameObject panelToBanish, GameObject destinationPanel)
    {
        if (instance == null || instance.ShadowRealm == null)
        {
            FindObjectOfType<PanelHider>().Unbanish(panelToBanish,destinationPanel);
        }
        else
        {
            instance.ShadowRealm.Unbanish(panelToBanish);
        }
    }

    /// <summary>
    /// This is a quick helper method that notifies the model that the FighterInfo has changed. It fires the reload method that will cause the view to update.
    /// Since the actions that modify the data directly access the FighterInfo field, it bypasses the setter that would normally do this. MAKE SURE TO CALL THIS WHENEVER THE FIGHTERINFO CHANGES
    /// </summary>
    public static void ChangedFighterData()
    {
        instance.loadedFighterDirty = true;
        instance.FireModelChange();
    }

    /// <summary>
    /// This is a quick helper method that notifies the model that the Current Action has changed. It fires the reload method that will cause the view to update.
    /// Since the actions that modify the data directly access the DynamicAction field, it bypasses the setter that would normally do this. MAKE SURE TO CALL THIS WHENEVER THE CURRENTACTION CHANGES
    /// </summary>
    public static void ChangedActionData()
    {
        instance.currentActionDirty = true;
        instance.loadedActionFileDirty = true; //? maybe don't need this? Revisit later
        instance.FireModelChange();
    }

    /// <summary>
    /// This is a quick helper method that notifies the model that the Action File has changed. It fires the reload method that will cause the view to update.
    /// Since the actions that modify the data directly access the ActionFile's fields, it bypasses the setter that would normally do this. MAKE SURE TO CALL THIS WHENEVER THE ACTIONFILE CHANGES
    /// </summary>
    public static void ChangedActionFile()
    {
        instance.loadedActionFileDirty = true;
        instance.FireModelChange();
    }

    /// <summary>
    /// This is a quick helper method that notifies the model that the Subaction has changed. It fires the reload method that will cause the view to update.
    /// Since the actions that modify the data directly access the Subaction's fields, it bypasses the setter that would normally do this. MAKE SURE TO CALL THIS WHENEVER THE SUBACTION CHANGES
    /// </summary>
    public static void ChangedSubaction()
    {
        instance.currentSubactionDirty = true;
        instance.subactionGroupDirty = true;
        instance.FireModelChange();
    }

    /// <summary>
    /// Returns true if the current action is a new action that is not currently in the ActionFile.
    /// If the action is in the ActionFile, it will return false.
    /// </summary>
    /// <returns></returns>
    public static bool CurrentActionIsNew()
    {
        DynamicAction action = instance.currentAction;
        ActionFile actionFile = instance.loadedActionFile;
        return !actionFile.actions.Contains(action);
    }
    #endregion
}