using System;
using System.Diagnostics;

namespace VoteMyst
{
    /// <summary>
    /// Provides access to the semantic version of the project.
    /// </summary>
    public class SemVer
    {
        /// <summary>
        /// Returns the semantic version as a <see cref="System.Version"/> object.
        /// </summary>
        public Version SystemVersion => new Version(Major, Minor, Patch);
        /// <summary>
        /// Returns the semantic version as a version string in the format 'Major.Minor.Patch'.
        /// </summary>
        public string VersionString => $"{Major}.{Minor}.{Patch}";

        /// <summary>
        /// The major component of the version.
        /// </summary>
        public int Major { get; } = 0;
        /// <summary>
        /// The minor component of the version.
        /// </summary>
        public int Minor { get; } = 0;
        /// <summary>
        /// The patch component of the version.
        /// </summary>
        public int Patch { get; } = 0;

        /// <summary>
        /// Creates a new instance of the semantic version for the project, by analyzing the git history.
        /// </summary>
        public SemVer()
        {
            // Try to locate the latest tag from the git history.
            string baseVersion = CaptureOutputFromProcess("git", "describe --tags --abbrev=0").Trim('\n', ' ');

            // If an empty string was returned then a tag has not been set yet. Use a zero-based version instead.
            bool hasTagVersion = !string.IsNullOrEmpty(baseVersion);
            if (!hasTagVersion)
                baseVersion = "v0.0.0";

            // Split the version string to extract the version components. Skip the first character 'v'.
            string[] versionSegments = baseVersion.Substring(1).Split('.');
            if (hasTagVersion && versionSegments != null && versionSegments.Length > 0) 
            {
                if (versionSegments.Length >= 1 && int.TryParse(versionSegments[0], out int major))
                    Major = major;
                if (versionSegments.Length >= 2 && int.TryParse(versionSegments[1], out int minor))
                    Minor = minor;
                if (versionSegments.Length >= 3 && int.TryParse(versionSegments[2], out int patch))
                    Patch = patch;
            }

            // Iterate over the reversed commits, scanning for semver messages (::major, ::minor, ::patch).
            string[] commits = CaptureOutputFromProcess("git", $"log {(hasTagVersion ? baseVersion + "..HEAD " : "")}-i --oneline --reverse").Split('\n');
            foreach (string commit in commits)
            {
                if (commit.Contains("::patch"))
                {
                    Patch++;
                }
                else if (commit.Contains("::minor")) 
                {
                    Minor++;
                    Patch = 0;
                }
                else if (commit.Contains("::major"))
                {
                    Major++;
                    Minor = 0;
                    Patch = 0;
                }
            }
        }

        private string CaptureOutputFromProcess(string filename, params string[] args)
        {
            using Process p = new Process();

            // Populate the start information with the file to execute and the arguments.
            p.StartInfo.FileName = filename;
            p.StartInfo.Arguments = string.Join(' ', args);
            p.StartInfo.UseShellExecute = false;

            // Redirect the standard output so we can read the results ...
            p.StartInfo.RedirectStandardOutput = true;
            // ... and the standard error so we don't get process errors in our console.
            p.StartInfo.RedirectStandardError = true;

            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output;
        }
    }
}