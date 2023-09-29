# CPVRLab-VR-Suite

## About

This is a package created for and by the CPVRLab of the BFH. It contains assets which are commonly used together in new VR projects by members of the CPVRLab.
Please refer to the Unity [documentation](https://docs.unity3d.com/Manual/CustomPackages.html) regarding the handling of a custom UPM package.

## Adding this package to a Unity project

1. Open the project settings and set up XR Plugin Management
2. Head to the package manager, add a package via git URL and paste the following URL: https://github.com/cpvrlab/cpvr-vr-suite.git
3. In the project settings, set OpenXR as Plugin provider and configure the feature groups as well as the interaction profiles for the desired platform
4. In order to use the overlay keyboard when using a Meta Quest 2 or Pro headset, a custom AndroidManifest.xml and folder structure has to be created.
   1. Create the following folder structure: **Assets/Plugins/Android**
   2. Create a file within the Android folder named **AndroidManifest.xml**
   3. Paste the following content into the newly created file:
   ```
   <?xml version="1.0" encoding="utf-8"?>
   <manifest
           xmlns:android="http://schemas.android.com/apk/res/android"
           package="com.unity3d.player"
           xmlns:tools="http://schemas.android.com/tools">
       <application>
           <activity android:name="com.unity3d.player.UnityPlayerActivity"
                     android:theme="@style/UnityThemeSelector">
               <intent-filter>
                   <action android:name="android.intent.action.MAIN" />
                   <category android:name="android.intent.category.LAUNCHER" />
               </intent-filter>
               <meta-data android:name="unityplayer.UnityActivity" android:value="true" />
           </activity>
       </application>
       <uses-feature android:name="oculus.software.overlay_keyboard" android:required="false"/>
   </manifest>
   ```
   4. Go to **Project Settings -> Player -> Publishing Settings** and make sure that the Checkbox for **Custom Main Manifest** is checked

### Scene setup

1. Place the **Persistent Objects** prefab in an empty scene and add it to the build index
2. Add your additional scenes to the build index without the **Persistent Object** prefab, camera or any other XR Rig in them

### Sending screenshots via Email

In order to send emails via the screenshotmenu a valid EmailLogin.json file must be provided and stored in 'Assets/Resources/Secrets'. It is forbidden to push this file to a repository as it contains the necessary credentials for the sending email account.

Additionally it must be verified that the following two settings are correctly set in the *Project Settings*.
- Player > Internet Acces: Required
- XR Plugin Management > OpenXR > Meta Quest Support Settings (Gear icon) > Manifest Settings > Force Remove Internet: false (unticked)

## Extending this package

If you wish to extend the functionality of this package, there are two basic ways to accomplish this.
1. If you are using a blank project, you can directly clone this repository into your 'Assets' folder. Open a Terminal in your project folder then use the following commands:
```
cd Assets
git clone https://gitlab.ti.bfh.ch/VR/cpvrlab-vr-suite.git
```
After the package has been cloned into your Assets folder, you can extend its functionality and push changes directly to the repository.

2. If you are using a project, which is already a git project, you can add this package as a submodule by using these commands:

```
cd existing_repo
cd Assets
git submodule add https://gitlab.ti.bfh.ch/VR/cpvrlab-vr-suite.git
```

Changes made to the package can be pushed from within the folder directly. Otherwise refer to the git [documentation](https://git-scm.com/book/en/v2/Git-Tools-Submodules).

Make sure to increment the version number in the **package.json** file.
