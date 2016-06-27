﻿using System;
using System.Collections.Generic;
using Cake.Core;
using Cake.Core.IO;

namespace Cake.Putty
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSettings"></typeparam>
    public class GenericPscpRunner<TSettings> : PscpTool<TSettings>
        where TSettings: AutoToolSettings, new()
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <param name="environment"></param>
        /// <param name="processRunner"></param>
        /// <param name="globber"></param>
        public GenericPscpRunner(IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner, IGlobber globber) 
            : base(fileSystem, environment, processRunner, globber)
        {
        }

        /// <summary>
        /// Runs given <paramref name="command"/> using given <paramref name=" settings"/> and <paramref name="additional"/>.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="settings"></param>
        /// <param name="additional"></param>
        public void Run(string command, TSettings settings, string[] additional)
        {
            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentNullException(nameof(command));
            }
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (additional == null || additional.Length == 0)
            {
                throw new ArgumentNullException(nameof(additional));
            }
            Run(settings, GetArguments(command, settings, additional));
        }
        /// <summary>
        /// Runs using given <paramref name=" settings"/> and <paramref name="additional"/>.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="additional"></param>
        public void Run(TSettings settings, IList<string> additional)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (additional == null || additional.Count == 0)
            {
                throw new ArgumentNullException(nameof(additional));
            }
            Run(settings, GetArguments(null, settings, additional));
        }

        private ProcessArgumentBuilder GetArguments(string command, TSettings settings, IList<string> additional)
        {
            var builder = new ProcessArgumentBuilder();
            builder.AppendAll(command, settings, additional);
            return builder;
        }

        /// <summary>
        /// Runs a command and returns a result based on processed output.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <param name="settings"></param>
        /// <param name="processOutput"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public T[] RunWithResult<T>(string command, TSettings settings, 
            Func<IEnumerable<string>, T[]> processOutput,
            params string[] arguments)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (processOutput == null)
            {
                throw new ArgumentNullException("processOutput");
            }
            T[] result = new T[0];
            Run(settings, GetArguments(command, settings, arguments), 
                new ProcessSettings { RedirectStandardOutput = true }, 
                proc => {
                    result = processOutput(proc.GetStandardOutput());
                });
            return result;
        }
    }
}
