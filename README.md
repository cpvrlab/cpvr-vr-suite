# CPVRLab-VR-Suite

## About

This is a package created for and by the CPVRLab of the BFH. It contains assets which are commonly used together in new VR projects by members of the CPVRLab.
Please refer to the Unity [documentation](https://docs.unity3d.com/Manual/CustomPackages.html) regarding the handling of a custom UPM package.

## Adding this package to a Unity project

1. Open the project settings and set up XR Plugin Management
2. Head to the package manager, add a package via git URL and paste the following URL: https://gitlab.ti.bfh.ch/VR/cpvrlab-vr-suite.git
3. In the project settings, set OpenXR as Plugin provider and configure the feature groups as well as the interaction profiles for the desired platform

## Further developing this package

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
