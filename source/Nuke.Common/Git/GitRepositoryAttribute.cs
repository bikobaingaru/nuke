﻿// Copyright 2018 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Nuke.Common.BuildServers;
using Nuke.Common.Execution;
using Nuke.Common.Tools.Git;

namespace Nuke.Common.Git
{
    /// <inheritdoc/>
    /// <summary>
    /// Injects an instance of <see cref="GitRepository"/> based on the local repository.
    /// <para/>
    /// <inheritdoc/>
    /// </summary>
    [PublicAPI]
    [UsedImplicitly(ImplicitUseKindFlags.Default)]
    public class GitRepositoryAttribute : InjectionAttributeBase
    {
        [CanBeNull]
        public string Branch { get; set; }

        [CanBeNull]
        public string Remote { get; set; }

        public override object GetValue(MemberInfo member, Type buildType)
        {
            return ControlFlow.SuppressErrors(() =>
                GitRepository.FromLocalDirectory(
                    NukeBuild.RootDirectory,
                    Branch ?? GetBranch(),
                    Remote ?? "origin"));
        }

        private string GetBranch()
        {
            return
                AppVeyor.Instance?.RepositoryBranch ??
                Bitrise.Instance?.GitBranch ??
                GitLab.Instance?.CommitRefName ??
                Jenkins.Instance?.GitBranch ??
                TeamCity.Instance?.BranchName ??
                TeamServices.Instance?.SourceBranchName ??
                Travis.Instance?.Branch ??
                GitTasks.GitCurrentBranch();
        }
    }
}
