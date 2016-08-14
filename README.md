# PlayFab Unity Editor Extensions
Welcome to the home of PlayFab's official Unity plugin, the best interface for viewing and configuring our latest Unity SDK. This plugin (currently in beta) houses a new custom inspector serving as the remodeled "front door" for our Unity Developers. 

We will continuing to add onto this platform as PlayFab's features grow.

![PlayFab Developer Login](https://github.com/PlayFab/UnityEditorExtensions/raw/master/_repoAssets/img/EdEx_CreateAccount.png "Users new to PlayFab can create an account.")

##Features:

  * Automated SDK installs & Removals
  * Toggle Admin / Server & Client APIs ON and OFF
  * Direct links into the Game Manager portal
  * New developer account signup
  * Configure WebRequest settings
  * Easily switch between your studios and titles
  * Easy to follow help links
  * View, Edit & Save TitleData from within the Unity IDE

##Setup:
  
  1. Download [this Asset Package](/Packages/ "PlayFabEditorExtensions.unitypackage") and import into a new or existing project.
  2. Open the Editor Extensions via the Unity menu: **Window > PlayFab > Editor Extensions** 
  3. Log in using an existing PlayFab developer account (or create a new account) to continue.
  4. Editor Extenstions will automatically detect if you have a PlayFab SDK installed. 
    * If your SDK is a recent version, you may use this plugin stay up-to-date with the latest SDK
    * If you have an older SDK, we recommend reading [this upgrade guide](https://github.com/PlayFab/UnitySDK/blob/master/UPGRADE.md) prior to upgrading.
    * If no SDK was detected, this plugin can download and install the latest SDK version from gitHub.
  5. After a supported SDK is installed, you may use the Editor Extensions to configure the SDK settings from within the IDE.   

![EditorExtensions_SKDs](https://github.com/PlayFab/UnityEditorExtensions/raw/master/_repoAssets/img/EdEx_SDKs.png "View the current SDK and upgrade to the latest SDK.")

## How it works:
PlayFab Editor Extensions is a stand-alone Unity plug-in that streamlines getting started with PlayFab.  All of the Editor Extensions code lives in editor folders within your Unity Project. This prevents any of the editor code from being compiled into the game build. 


## Troubleshooting and Support:

Did you find an issue or is Editor Extensions giving you a bad experience? 

[**Tell us all about it**](https://github.com/PlayFab/UnityEditorExtensions/issues).

###Version History:
See [GitHub Releases](https://github.com/PlayFab/UnityEditorExtensions/releases "GitHub Versions") for the latest stable build and patch notes.  

### Ongoing Development:
Editor Extensions is our attempt to make a first class PlayFab development experience within the Unity Editor. We consider this project a sandbox and are always open to feedback from our community developers. 

###Proposed Features (to be added in upcoming releases):

  * Download and install recipes and examples
  * Enable and disable additional add-on modules
  * View usage & modify the limits for a title
  * Configure additional title settings
  * Automatic plugin upgrades
  * Extensible code structure to support 3rd party tool development

Let us know what we are missing and we will do our best to accomodate.

### Known Issues:
  * Occasional Error thrown when logging in. Currently this does not seem to be impacting functionality.


##Copyright and Licensing Information:

  Apache License -- Version 2.0, January 2004 [http://www.apache.org/licenses/](http://www.apache.org/licenses/)

  License Details available in [LICENSE.txt](https://github.com/PlayFab/UnityEditorExtensions/blob/master/LICENSE "Apache 2.0 License")
  

  