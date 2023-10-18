using System;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils.Editor;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.MetaQuestSupport;

static class VRSuiteProjectValidation
{
    const string k_DisplayName = "CPVRlab VR Suite Package";
    const string k_Category = "CPVRlab VR Suite Package";
    const string k_XRITKPackageName = "com.unity.xr.interaction.toolkit";
    const string k_StarterAssetsSampleName = "Starter Assets";
    const string k_HandsInteractionSampleName = "Hands Interaction Demo";
    const string k_HandsPackageName = "com.unity.xr.hands";
    const string k_HandVisualizerSampleName = "HandVisualizer";
    const string k_ShaderGraphPackageName = "com.unity.shadergraph";
    const string k_ProjectValidationSettingsPath = "Project/XR Plug-in Management/Project Validation";
    static readonly PackageVersion s_MinimumXRITKPackageVersion = new("2.5.2");
    static readonly PackageVersion s_MinimumXRHandsPackageVersion = new("1.3.0");

    static readonly BuildTargetGroup[] s_BuildTargetGroups =
            ((BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup))).Distinct().ToArray();

    static readonly List<BuildValidationRule> s_BuildValidationRules = new()
    {
        new BuildValidationRule
        {
            IsRuleEnabled = () => s_HandsPackageAddRequest == null || s_HandsPackageAddRequest.IsCompleted,
            Message = $"[{k_DisplayName}] XR Hands ({k_HandsPackageName}) package must be installed or updated to use this sample.",
            Category = k_Category,
            CheckPredicate = () => PackageVersionUtility.GetPackageVersion(k_HandsPackageName) >= s_MinimumXRHandsPackageVersion,
            FixIt = () =>
            {
                if (s_HandsPackageAddRequest == null || s_HandsPackageAddRequest.IsCompleted)
                    InstallOrUpdateHands();
            },
            FixItAutomatic = true,
            Error = true
        },
        new BuildValidationRule
        {
            IsRuleEnabled = () => s_HandsPackageAddRequest == null || s_HandsPackageAddRequest.IsCompleted,
            Message = $"[{k_DisplayName}] XR Hands ({k_HandsPackageName}) package must be at version {s_MinimumXRHandsPackageVersion} or higher to use the latest sample features.",
            Category = k_Category,
            CheckPredicate = () => PackageVersionUtility.GetPackageVersion(k_HandsPackageName) >= s_MinimumXRHandsPackageVersion,
            FixIt = () =>
            {
                if (s_HandsPackageAddRequest == null || s_HandsPackageAddRequest.IsCompleted)
                    InstallOrUpdateHands();
            },
            FixItAutomatic = true,
            Error = false
        },
        new BuildValidationRule
        {
            IsRuleEnabled = () => PackageVersionUtility.GetPackageVersion(k_HandsPackageName) >= s_MinimumXRHandsPackageVersion,
            Message = $"[{k_DisplayName}] {k_HandVisualizerSampleName} sample from XR Hands ({k_HandsPackageName}) package must be imported or updated to use this sample.",
            Category = k_Category,
            CheckPredicate = () => TryFindSample(k_HandsPackageName, string.Empty, k_HandVisualizerSampleName, out var sample) && sample.isImported,
            FixIt = () =>
            {
                if (TryFindSample(k_HandsPackageName, string.Empty, k_HandVisualizerSampleName, out var sample))
                {
                    sample.Import(Sample.ImportOptions.OverridePreviousImports);
                }
            },
            FixItAutomatic = true,
            Error = true
        },
        new BuildValidationRule
        {
            IsRuleEnabled = () => s_XRInteractionToolkitAddRequest == null || s_XRInteractionToolkitAddRequest.IsCompleted,
            Message = $"[{k_DisplayName}] XR Interaction Toolkit ({k_XRITKPackageName}) package must be installed or updated to use this sample.",
            Category = k_Category,
            CheckPredicate = () => PackageVersionUtility.GetPackageVersion(k_XRITKPackageName) >= s_MinimumXRITKPackageVersion,
            FixIt = () =>
            {
                if (s_XRInteractionToolkitAddRequest == null || s_XRInteractionToolkitAddRequest.IsCompleted)
                    InstallOrUpdateXRITK();
            },
            FixItAutomatic = true,
            Error = true
        },
        new BuildValidationRule
        {
            IsRuleEnabled = () => s_HandsPackageAddRequest == null || s_HandsPackageAddRequest.IsCompleted,
            Message = $"[{k_DisplayName}] XR Interaction Toolkit ({k_XRITKPackageName}) package must be at version {s_MinimumXRITKPackageVersion} or higher to use the latest sample features.",
            Category = k_Category,
            CheckPredicate = () => PackageVersionUtility.GetPackageVersion(k_XRITKPackageName) >= s_MinimumXRITKPackageVersion,
            FixIt = () =>
            {
                if (s_XRInteractionToolkitAddRequest == null || s_XRInteractionToolkitAddRequest.IsCompleted)
                    InstallOrUpdateXRITK();
            },
            FixItAutomatic = true,
            Error = false
        },
        new BuildValidationRule
        {
            Message = $"[{k_DisplayName}] {k_StarterAssetsSampleName} sample from XR Interaction Toolkit ({k_XRITKPackageName}) package must be imported or updated to use this sample.",
            Category = k_Category,
            CheckPredicate = () => TryFindSample(k_XRITKPackageName, string.Empty, k_StarterAssetsSampleName, out var sample) && sample.isImported,
            FixIt = () =>
            {
                if (TryFindSample(k_XRITKPackageName, string.Empty, k_StarterAssetsSampleName, out var sample))
                {
                    sample.Import(Sample.ImportOptions.OverridePreviousImports);
                }
            },
            FixItAutomatic = true,
            Error = true
        },
        new BuildValidationRule
        {
            Message = $"[{k_DisplayName}] {k_HandsInteractionSampleName} sample from XR Interaction Toolkit ({k_XRITKPackageName}) package must be imported or updated to use this sample.",
            Category = k_Category,
            CheckPredicate = () => TryFindSample(k_XRITKPackageName, string.Empty, k_HandsInteractionSampleName, out var sample) && sample.isImported,
            FixIt = () =>
            {
                if (TryFindSample(k_XRITKPackageName, string.Empty, k_HandsInteractionSampleName, out var sample))
                {
                    sample.Import(Sample.ImportOptions.OverridePreviousImports);
                }
            },
            FixItAutomatic = true,
            Error = true
        },
        new BuildValidationRule
        {
            IsRuleEnabled = () => s_ShaderGraphPackageAddRequest == null || s_ShaderGraphPackageAddRequest.IsCompleted,
            Message = $"[{k_DisplayName}] Shader Graph ({k_ShaderGraphPackageName}) package must be installed for materials used in this sample.",
            Category = k_Category,
            CheckPredicate = () => PackageVersionUtility.IsPackageInstalled(k_ShaderGraphPackageName),
            FixIt = () =>
            {
                s_ShaderGraphPackageAddRequest = Client.Add(k_ShaderGraphPackageName);
                if (s_ShaderGraphPackageAddRequest.Error != null)
                {
                    Debug.LogError($"Package installation error: {s_ShaderGraphPackageAddRequest.Error}: {s_ShaderGraphPackageAddRequest.Error.message}");
                }
            },
            FixItAutomatic = true,
            Error = false
        },
        new BuildValidationRule
        {
            IsRuleEnabled = () => EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android,
            Message = "[Project Settings > Player] 'Internet Access' needs to be set to 'Require' in order to send screenshots via email.",
            Category = k_Category,
            CheckPredicate = () => EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android || PlayerSettings.Android.forceInternetPermission == true,
            FixIt = () => PlayerSettings.Android.forceInternetPermission = true,
            Error = false
        }
    };

    static AddRequest s_XRInteractionToolkitAddRequest;
    static AddRequest s_HandsPackageAddRequest;
    static AddRequest s_ShaderGraphPackageAddRequest;

    [InitializeOnLoadMethod]
    static void RegisterProjectValidationRules()
    {
        foreach (var buildTargetGroup in s_BuildTargetGroups)
        {
            BuildValidator.AddRules(buildTargetGroup, s_BuildValidationRules);
        }

        // Delay evaluating conditions for issues to give time for Package Manager and UPM cache to fully initialize.
        EditorApplication.delayCall += ShowWindowIfIssuesExist;
    }

    static void ShowWindowIfIssuesExist()
    {
        foreach (var validation in s_BuildValidationRules)
        {
            if (validation.CheckPredicate == null || !validation.CheckPredicate.Invoke())
            {
                ShowWindow();
                return;
            }
        }
    }

    internal static void ShowWindow()
    {
        // Delay opening the window since sometimes other settings in the player settings provider redirect to the
        // project validation window causing serialized objects to be nullified.
        EditorApplication.delayCall += () =>
        {
            SettingsService.OpenProjectSettings(k_ProjectValidationSettingsPath);
        };
    }

    static bool TryFindSample(string packageName, string packageVersion, string sampleDisplayName, out Sample sample)
    {
        sample = default;

        if (!PackageVersionUtility.IsPackageInstalled(packageName))
            return false;

        IEnumerable<Sample> packageSamples;
        try
        {
            packageSamples = Sample.FindByPackage(packageName, packageVersion);
        }
        catch (Exception e)
        {
            Debug.LogError($"Couldn't find samples of the {ToString(packageName, packageVersion)} package; aborting project validation rule. Exception: {e}");
            return false;
        }

        if (packageSamples == null)
        {
            Debug.LogWarning($"Couldn't find samples of the {ToString(packageName, packageVersion)} package; aborting project validation rule.");
            return false;
        }

        foreach (var packageSample in packageSamples)
        {
            if (packageSample.displayName == sampleDisplayName)
            {
                sample = packageSample;
                return true;
            }
        }

        Debug.LogWarning($"Couldn't find {sampleDisplayName} sample in the {ToString(packageName, packageVersion)} package; aborting project validation rule.");
        return false;
    }

    static string ToString(string packageName, string packageVersion)
    {
        return string.IsNullOrEmpty(packageVersion) ? packageName : $"{packageName}@{packageVersion}";
    }

    static void InstallOrUpdateHands()
    {
        // Set a 3-second timeout for request to avoid editor lockup
        var currentTime = DateTime.Now;
        var endTime = currentTime + TimeSpan.FromSeconds(3);

        var request = Client.Search(k_HandsPackageName);
        if (request.Status == StatusCode.InProgress)
        {
            Debug.Log($"Searching for ({k_HandsPackageName}) in Unity Package Registry.");
            while (request.Status == StatusCode.InProgress && currentTime < endTime)
                currentTime = DateTime.Now;
        }

        var addRequest = k_HandsPackageName;
        if (request.Status == StatusCode.Success && request.Result.Length > 0)
        {
            var versions = request.Result[0].versions;
            var verifiedVersion = new PackageVersion(versions.recommended);
            var latestCompatible = new PackageVersion(versions.latestCompatible);
            if (verifiedVersion < s_MinimumXRHandsPackageVersion && s_MinimumXRHandsPackageVersion <= latestCompatible)
                addRequest = $"{k_HandsPackageName}@{s_MinimumXRHandsPackageVersion}";
        }

        s_HandsPackageAddRequest = Client.Add(addRequest);
        if (s_HandsPackageAddRequest.Error != null)
        {
            Debug.LogError($"Package installation error: {s_HandsPackageAddRequest.Error}: {s_HandsPackageAddRequest.Error.message}");
        }
    }

    static void InstallOrUpdateXRITK()
    {
        // Set a 3-second timeout for request to avoid editor lockup
        var currentTime = DateTime.Now;
        var endTime = currentTime + TimeSpan.FromSeconds(3);

        var request = Client.Search(k_XRITKPackageName);
        if (request.Status == StatusCode.InProgress)
        {
            Debug.Log($"Searching for ({k_XRITKPackageName}) in Unity Package Registry.");
            while (request.Status == StatusCode.InProgress && currentTime < endTime)
                currentTime = DateTime.Now;
        }

        var addRequest = k_XRITKPackageName;
        if (request.Status == StatusCode.Success && request.Result.Length > 0)
        {
            var versions = request.Result[0].versions;
            var verifiedVersion = new PackageVersion(versions.recommended);
            var latestCompatible = new PackageVersion(versions.latestCompatible);
            if (verifiedVersion < s_MinimumXRITKPackageVersion && s_MinimumXRITKPackageVersion <= latestCompatible)
                addRequest = $"{k_XRITKPackageName}@{s_MinimumXRITKPackageVersion}";
        }

        s_XRInteractionToolkitAddRequest = Client.Add(addRequest);
        if (s_XRInteractionToolkitAddRequest.Error != null)
        {
            Debug.LogError($"Package installation error: {s_XRInteractionToolkitAddRequest.Error}: {s_XRInteractionToolkitAddRequest.Error.message}");
        }
    }
}
