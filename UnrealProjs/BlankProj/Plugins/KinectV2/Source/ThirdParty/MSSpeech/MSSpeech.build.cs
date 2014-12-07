

using UnrealBuildTool;
using System.IO;
public class MSSpeech : ModuleRules
{

    public MSSpeech(TargetInfo Target)
    {

        Type = ModuleType.External;
        string DefaultInstallStr = "C:\\Program Files";

        if (Target.Platform == UnrealTargetPlatform.Win32)
            DefaultInstallStr += " (x86)";

        DefaultInstallStr += "\\Microsoft SDKs\\Speech\\v11.0";


        if (Directory.Exists(DefaultInstallStr))
        {


           // Path DefaultInstallPath = DefaultInstallStr;

            PublicIncludePaths.Add(Path.Combine(DefaultInstallStr, "Include"));

            PublicLibraryPaths.Add(Path.Combine(DefaultInstallStr, "Lib"));
            // string BinPath = Path.Combine(ModulePath, "MSSpeech", "bin", (Target.Platform == UnrealTargetPlatform.Win64) ? "x64" : "x86");

            PublicAdditionalLibraries.Add("sapi.lib");
            // PublicDelayLoadDLLs.Add(Path.Combine(BinPath, "lwsreng.dll"));
            // PublicDelayLoadDLLs.Add(Path.Combine(BinPath, "Microsoft.Speech.dll"));
            //  PublicDelayLoadDLLs.Add(Path.Combine(BinPath, "mssps.dll"));
            // PublicDelayLoadDLLs.Add(Path.Combine(BinPath, "msttsengine.dll"));
            //  PublicDelayLoadDLLs.Add(Path.Combine(BinPath, "msttsloc.dll"));
            // PublicDelayLoadDLLs.Add(Path.Combine(BinPath, "spsreng.dll"));
            //  PublicDelayLoadDLLs.Add(Path.Combine(BinPath, "spsrx.dll"));
            //  PublicDelayLoadDLLs.Add(Path.Combine(BinPath, "srloc.dll"));

        }
    }
}
