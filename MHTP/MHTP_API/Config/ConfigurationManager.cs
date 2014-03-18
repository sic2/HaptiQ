﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Phidgets;

using Input_API;

namespace MHTP_API
{
    /// <summary>
    /// Manage MHTPs configurations. 
    /// Configurations can either be loaded from file or added
    /// by a third party (i.e. via ConfigurationForm)
    /// </summary>
    public class ConfigurationManager
    {
        /// <summary>
        /// List of available and valid configurations
        /// </summary>
        public static List<Configuration> configurations = new List<Configuration>();

        /// <summary>
        /// Get all the valid configurations held in memory
        /// </summary>
        /// <returns></returns>
        public static List<Configuration> getConfigurations()
        {
            return configurations;
        }

        /// <summary>
        /// Add a given configuration to the ConfigurationManager
        /// </summary>
        /// <param name="configuration"></param>
        public static void addConfiguration(Configuration configuration)
        {
            configuration.serializableInputIdentifier = configuration.inputIdentifier.getSerializableInputIdentifier();
            configurations.Add(configuration);
            Helper.SerializeToXML(configuration, "test.xml");
        }

    }
}
