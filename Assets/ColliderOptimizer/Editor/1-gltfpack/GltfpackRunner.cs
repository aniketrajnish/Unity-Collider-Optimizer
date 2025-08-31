#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace ColliderOptimizer.Gltfpack
{
    static class GltfpackRunner
    {
        static string FindBinary()
        {
#if UNITY_EDITOR_WIN
            string rel = "Assets/ColliderOptimizer/Editor/1-gltfpack/1-bin/1-win/gltfpack.exe";
            string full = Path.GetFullPath(rel);
            if (!File.Exists(full)) throw new FileNotFoundException("gltfpack not found at: " + full);
            return full;
#elif UNITY_EDITOR_OSX
            bool wantArm = RuntimeInformation.ProcessArchitecture == Architecture.Arm64;
            string arm = "Assets/ColliderOptimizer/Editor/1-gltfpack/1-bin/2-mac/1-arm64/gltfpack";
            string x64 = "Assets/ColliderOptimizer/Editor/1-gltfpack/1-bin/2-mac/2-x86_64/gltfpack";
            string rel = wantArm && File.Exists(Path.GetFullPath(arm))
                        ? arm
                        : (File.Exists(Path.GetFullPath(x64)) ? x64 : arm);
            string full = Path.GetFullPath(rel);
            if (!File.Exists(full)) throw new FileNotFoundException("gltfpack not found at: " + full);
            TryChmodX(full);
            return full;
#else
            string rel = "Assets/ColliderOptimizer/Editor/1-gltfpack/1-bin/3-linux/gltfpack";
            string full = Path.GetFullPath(rel);
            if (!File.Exists(full)) throw new FileNotFoundException("gltfpack not found at: " + full);
            TryChmodX(full);
            return full;
#endif
        }

#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
        static void TryChmodX(string full)
        {
            try
            {
                var p = new Process();
                p.StartInfo = new ProcessStartInfo("chmod", $"+x \"{full}\"") { UseShellExecute = false };
                p.Start(); p.WaitForExit();
            }
            catch {  }
        }
#endif

        public static bool Run(string inPath, string outPath, float keepRatio, bool aggressive = false, bool permissive = false)
        {
            string exe = FindBinary();
            string args = $"-i \"{inPath}\" -o \"{outPath}\" -si {Mathf.Clamp01(keepRatio):0.###}";
            if (aggressive) args += " -sa";
            if (permissive) args += " -sp";
            if (TryRunOnce(exe, args, out int code, out string err)) return true;

#if UNITY_EDITOR_OSX
            string alt = exe.Contains("/1-arm64/") 
                        ? exe.Replace("/1-arm64/", "/2-x86_64/")
                        : exe.Replace("/2-x86_64/", "/1-arm64/");
            if (!Path.GetFullPath(alt).Equals(Path.GetFullPath(exe)) && File.Exists(alt))
            {
                TryChmodX(alt);
                if (TryRunOnce(alt, args, out code, out err)) return true;
            }
#endif
            UnityEngine.Debug.LogError($"gltfpack failed.\nexe: {exe}\nargs: {args}\n(exit {code})\n{err}");
            return false;
        }

        static bool TryRunOnce(string exe, string args, out int exitCode, out string stderr)
        {
            var psi = new ProcessStartInfo(exe, args)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = Directory.GetCurrentDirectory()
            };
            try
            {
                using var p = Process.Start(psi);
                p.WaitForExit();
                exitCode = p.ExitCode;
                stderr = p.StandardError.ReadToEnd();
                return exitCode == 0;
            }
            catch (Exception e)
            {
                exitCode = -1;
                stderr = e.ToString();
                return false;
            }
        }
    }
}
#endif
