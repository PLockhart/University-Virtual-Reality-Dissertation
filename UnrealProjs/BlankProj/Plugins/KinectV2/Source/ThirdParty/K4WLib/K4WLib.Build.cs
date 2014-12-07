// Copyright 1998-2014 Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;

public class K4WLib : ModuleRules
{
    public K4WLib(TargetInfo Target)
	{
		Type = ModuleType.External;

        string SDKDIR = Utils.ResolveEnvironmentVariable("%KINECTSDK20_DIR%");

        SDKDIR = SDKDIR.Replace("\\", "/");

		if ((Target.Platform == UnrealTargetPlatform.Win64) || (Target.Platform == UnrealTargetPlatform.Win32))
		{
            PublicIncludePaths.Add(SDKDIR+"inc/");

            string PlatformPath =  (Target.Platform == UnrealTargetPlatform.Win64) ? "x64/" : "x86/";

            string LibPath = SDKDIR+"Lib/"+PlatformPath;

            PublicLibraryPaths.Add(LibPath);
            PublicAdditionalLibraries.Add("Kinect20.lib");

            string redistPath = SDKDIR + "Redist/";

            PublicDelayLoadDLLs.AddRange(new string[] { redistPath+"VGB/"+PlatformPath+"Kinect20.VisualGestureBuilder.dll" });

		}
	}
}
