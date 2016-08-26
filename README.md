# PlayFab Unity Editor Extensions
Welcome to the home of PlayFab's official Unity plugin, the best interface for viewing and configuring our Unity SDK. Our plugin (currently in beta) houses a new custom inspector serving as the remodeled "front door" for our Unity Developers. 

We will be adding to this platform as PlayFab's features grow.

![PlayFab Developer Login](https://github.com/PlayFab/UnityEditorExtensions/raw/master/_repoAssets/img/EdEx_CreateAccount.png "Users new to PlayFab can create an account.")

##Features:

  * **Automated SDK installation, upgrading & removal** - Now pain free!
  * **Toggle Admin, Server & Client APIs ON and OFF** - 100% less file-fiddling
  * **Direct links into the Game Manager** - Some tasks are still best done in the Game Manager
  * **Create New Developer Accounts** - Brand new front door for new developers
  * **Configure WebRequest settings** - Similar to before, except now with an easy to use interface.
  * **Easily switch between your studios and titles** - Automatically populates your settings based on the selected Title Id 
  * **Easy to follow help links** - Links to *Documentation*, *Ask Questions* and *View Service Availability*
  * **View, Edit & Save TitleData** - Edit your title data keys and values with a simple text editor within the inspector.

##Setup:
  
  1. Download [this Asset Package](https://api.playfab.com/sdks/download/unity-edex "PlayFabEditorExtensions.unitypackage") and import into a new or existing project.
  2. Open the Editor Extensions via the Unity menu: **Window > PlayFab > Editor Extensions** 
  3. Log in using an existing PlayFab developer account (or create a new account) to continue.
  4. Editor Extensions will automatically detect if you have a PlayFab SDK installed. 
    * If your SDK is a supported version(2.0+), you may use this plugin stay up-to-date with the latest SDK
    * If you have an older SDK, we recommend reading [this upgrade guide](https://github.com/PlayFab/UnitySDK/blob/master/UPGRADE.md) prior to upgrading.
    * If no SDK was detected, this plugin can download and install the latest SDK version from gitHub.
  5. After a supported SDK is installed, you will need to select a studio and title id.
  6. After a studio and title id are selected, you may now call APIs and use the Editor Extensions to configure the SDK settings from within the IDE.   

![EditorExtensions_SKDs](https://github.com/PlayFab/UnityEditorExtensions/raw/master/_repoAssets/img/EdEx_SDKs.png "View the current SDK and upgrade to the latest SDK.")

## How it works:
PlayFab Editor Extensions is a stand-alone Unity plug-in that streamlines getting started with PlayFab.  All of the Editor Extensions code lives in editor folders within your Unity Project. This prevents any of the editor code from being compiled into the game build. 

When a supported SDK is installed, additional service menus are available. These menus provide access to SDK configurations. These configurations settings are saved in a combination of places to ensure that the data persists throughout Unity compilations and deployments. 
 
Now that Admin & Server APIs and Models are all included in one single SDK, we require *#IFDEF*'s to [selectively ignore sets](https://docs.unity3d.com/Manual/PlatformDependentCompilation.html "Unity Scripting Define Symbols") that are not needed. In doing so, we also include and omit the **DeveloperSecretKey** to match the selected settings.  As always, never publish your developer secret key in any client facing code.

### Additional Systems & Features within the Editor Extension
  * Editor HTTP Service & Coroutine Manager
  * Isolated API set (independent from the PlayFab SDK)
  * Configured to fetch our [latest SDK](https://github.com/PlayFab/UnitySDK/blob/versioned/Packages/UnitySDK.unitypackage "GitHub Versioned Repo")
  * Saves Account and Environment settings via Unity's [EditorPrefs](https://docs.unity3d.com/ScriptReference/EditorPrefs.html "Unity3d Docs") and [ScripableObjects](https://docs.unity3d.com/ScriptReference/ScriptableObject.html "Unity3d Docs")
  * Saves configuration data to the SDK via reflecting on the installed assemblies. 
  * Setting the *STUDIO* to **_OVERRIDE_** will blank the *TITLE ID* and *DEVELOPER SECRET KEY* for manual input. Use this option when you need to connect to a studio to which you do not belong. Generally speaking, it is a good practice to only connect to titles to which you are a member; however, this mode can be useful when getting familiar with PlayFab. 
 
 
 ![EditorExtensions_Override](https://github.com/PlayFab/UnityEditorExtensions/raw/master/_repoAssets/img/EdEx_Override.png "Select _OVERRIDE_ to manually input your Title Id")
 

## Troubleshooting and Support:
This project is designed to work with Unity 5.4+. Using this plugin on earlier versions should work; however, If you choose to remove Editor Extensions make sure to close the Inspector window prior to deleting the files. 

Did you find an issue or is Editor Extensions giving you a bad experience? 

[**Tell us all about it**](https://github.com/PlayFab/UnityEditorExtensions/issues).

###Version History:
See [GitHub Releases](https://github.com/PlayFab/UnityEditorExtensions/releases "GitHub Versions") for the latest stable build and patch notes.  

When in the app, Our Editor Extension
 ![EditorExtensions_About](https://github.com/PlayFab/UnityEditorExtensions/raw/master/_repoAssets/img/EdEx_About.png "EdEx Details can be found under the Help Tab")

### Ongoing Development:
Editor Extensions is our attempt to make a first class PlayFab development experience within the Unity Editor. We consider this project a sandbox and are always open to feedback from our community developers. 

###Proposed Features (to be added in upcoming releases):

  * Download and install recipes and examples
  * Enable and disable additional add-on modules
  * View usage & modify the limits for a title
  * Configure additional title settings
  * Automatic plugin upgrades
  * Extensible code structure to support 3rd party tool development

Let us know what we are missing and we will do our best to accommodate.

### Known Issues:

  1. The EditorExtensions Tab is open and nothing is being drawn. 
    * This typically is caused when the PlayFabEditorExtensions directory has been renamed.
  2. Plugins and other packages must be installed / removed / upgraded separately 
  3. When toggling the API sets (Client, Server, Admin), sometimes the editor does not recompile and apply the updated definitions.
    *  This can be manually set via under  **Build Settings > Player Settings**. [More Information](https://docs.unity3d.com/Manual/PlatformDependentCompilation.html).

#####A word of caution:
You may move our plugin folder around; however, we discourage renaming the root(*PlayFabEditorExtensions*) folder. This may cause the relative links within the plugin to break.

A complete list of issues can be found [here](https://github.com/PlayFab/UnityEditorExtensions/issues)
   
#####Unsupported Build-Targets:
  * **Unity Web Player** - The editor Extensions will not work properly when Unity is set to output Web Player builds. Web player was removed in Unity 5.4, and is no longer a supported build target. 

##Copyright and Licensing Information:

  Apache License -- Version 2.0, January 2004 [http://www.apache.org/licenses/](http://www.apache.org/licenses/)

  License Details available in [LICENSE.txt](https://github.com/PlayFab/UnityEditorExtensions/blob/master/LICENSE "Apache 2.0 License")
  

  
