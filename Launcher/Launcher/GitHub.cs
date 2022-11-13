using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher
{
    internal static class GitHub
    {
        /// <summary> Personal access token for read-only access to Ultraman's releases. </summary>
        private static readonly string Token = "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";

        /// <summary> UltraMan's repo ID. </summary>
        private static readonly long RepoId = 541292141;

        /// <summary> Gets GitHub client. </summary>
        /// <returns> GitHub client. </returns>
        private static GitHubClient GetClient()
        {
            return new GitHubClient(new ProductHeaderValue("UltraMan-Launcher-Client"))
            {
                Credentials = new Credentials(Token)
            };
        }

        public static Task<Release> GetLatestRelease()
        {
            return GetClient().Repository.Release.GetLatest(RepoId);
        }
    }
}
