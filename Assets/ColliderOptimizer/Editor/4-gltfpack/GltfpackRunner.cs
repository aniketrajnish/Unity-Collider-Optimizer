#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine;

namespace ColliderOptimizer.Gltfpack
{
    static class GltfpackRunner
    {
        static string FindBinary()
        {
#if UNITY_EDITOR_WIN
            string rel = "Assets/ColliderOptimizer/Editor/4-gltfpack/1-bin/1-win/gltfpack.exe";
            string full = Path.GetFullPath(rel);
            if (!File.Exists(full)) throw new FileNotFoundException("gltfpack not found at: " + full);
            return full;
#elif UNITY_EDITOR_OSX
            bool wantArm = RuntimeInformation.ProcessArchitecture == Architecture.Arm64;
            string arm = "Assets/ColliderOptimizer/Editor/4-gltfpack/1-bin/2-mac/1-arm64/gltfpack";
            string x64 = "Assets/ColliderOptimizer/Editor/4-gltfpack/1-bin/2-mac/2-x86_64/gltfpack";
            string rel = wantArm && File.Exists(Path.GetFullPath(arm))
                        ? arm
                        : (File.Exists(Path.GetFullPath(x64)) ? x64 : arm);
            string full = Path.GetFullPath(rel);
            if (!File.Exists(full)) throw new FileNotFoundException("gltfpack not found at: " + full);
            EnsureExecutableAndUnquarantined(full);
            return full;
#else
            string rel = "Assets/ColliderOptimizer/Editor/4-gltfpack/1-bin/3-linux/gltfpack";
            string full = Path.GetFullPath(rel);
            if (!File.Exists(full)) throw new FileNotFoundException("gltfpack not found at: " + full);
            EnsureExecutable(full);
            return full;
#endif
        }

#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
        static void EnsureExecutable(string full)
        {
            try { RunTool("/bin/chmod", $"+x \"{full}\""); } catch { }
        }
#endif

#if UNITY_EDITOR_OSX
        static void EnsureExecutableAndUnquarantined(string full)
        {
            EnsureExecutable(full);
            try { RunTool("/usr/bin/xattr", $"-dr com.apple.quarantine \"{full}\""); } catch { }
        }
#endif

        public static bool Run(string __inPath, string __outPath, float __keepRatio, bool __aggressive = false, bool __permissive = false)
        {
            string exe = FindBinary();
            string args = string.Format(
                CultureInfo.InvariantCulture,
                "-i \"{0}\" -o \"{1}\" -si {2:0.###}",
                __inPath, __outPath, Mathf.Clamp01(__keepRatio)
            );
            if (__aggressive) args += " -sa";
            if (__permissive) args += " -sp";

            if (TryRunOnce(exe, args, out int code, out string err)) return true;

#if UNITY_EDITOR_OSX
            string alt = exe.Contains("/1-arm64/")
                        ? exe.Replace("/1-arm64/", "/2-x86_64/")
                        : exe.Replace("/2-x86_64/", "/1-arm64/");
            if (!Path.GetFullPath(alt).Equals(Path.GetFullPath(exe)) && File.Exists(alt))
            {
                EnsureExecutableAndUnquarantined(alt);
                if (TryRunOnce(alt, args, out code, out err)) return true;
            }
#endif
            UnityEngine.Debug.LogError($"gltfpack failed.\nexe: {exe}\nargs: {args}\n(exit {code})\n{err}");
            return false;
        }

        static bool TryRunOnce(string __exe, string __args, out int __exitCode, out string __stderr)
        {
            var psi = new ProcessStartInfo(__exe, __args)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = false,
                WorkingDirectory = Directory.GetCurrentDirectory()
            };
            try
            {
                using var p = Process.Start(psi);
                __stderr = p.StandardError.ReadToEnd();
                p.WaitForExit();
                __exitCode = p.ExitCode;
                return __exitCode == 0;
            }
            catch (Exception e)
            {
                __exitCode = -1;
                __stderr = e.ToString();
                return false;
            }
        }

#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
        static void RunTool(string __fileName, string __args)
        {
            var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = __fileName,
                    Arguments = __args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            p.Start();
            p.WaitForExit();
        }
#endif
    }
}
#endif
