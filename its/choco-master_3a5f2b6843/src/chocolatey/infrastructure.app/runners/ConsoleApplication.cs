﻿// Copyright © 2011 - Present RealDimensions Software, LLC
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// 
// You may obtain a copy of the License at
// 
// 	http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace chocolatey.infrastructure.app.runners
{
    using System;
    using System.Collections.Generic;
    using SimpleInjector;
    using configuration;
    using logging;

    /// <summary>
    ///   Console application responsible for running chocolatey
    /// </summary>
    public sealed class ConsoleApplication
    {
        public void run(string[] args, ChocolateyConfiguration config, Container container)
        {
            this.Log().Debug(() => "Command line: {0}".format_with(Environment.CommandLine));
            this.Log().Debug(() => "Received arguments: {0}".format_with(string.Join(" ", args)));

            IList<string> commandArgs = new List<string>();
            //shift the first arg off 
            int count = 0;
            foreach (var arg in args)
            {
                if (count == 0)
                {
                    count += 1;
                    continue;
                }

                commandArgs.Add(arg);
            }

            var runner = new GenericRunner();
            runner.run(config, container, isConsole: true, parseArgs: command =>
                {
                    ConfigurationOptions.parse_arguments_and_update_configuration(
                        commandArgs,
                        config,
                        (optionSet) => command.configure_argument_parser(optionSet, config),
                        (unparsedArgs) => {
                            // if debug is bundled with local options, it may not get picked up when global 
                            // options are parsed. Attempt to set it again once local options are set.
                            // This does mean some output from debug will be missed (but not much)
                            if (config.Debug) Log4NetAppenderConfiguration.set_logging_level_debug_when_debug(config.Debug);

                            command.handle_additional_argument_parsing(unparsedArgs, config);

                            // all options / switches should be parsed, 
                            //  so show help menu if there are any left
                            foreach (var unparsedArg in unparsedArgs.or_empty_list_if_null())
                            {
                                if (unparsedArg.StartsWith("-") || unparsedArg.StartsWith("/"))
                                {
                                    config.HelpRequested = true;
                                }
                            }
                        },
                        () => command.handle_validation(config),
                        () => command.help_message(config));
                });
        }
    }
}