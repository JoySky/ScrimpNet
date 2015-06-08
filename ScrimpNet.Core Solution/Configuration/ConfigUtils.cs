/**
/// ScrimpNet.Core Library
/// Copyright (c) 2005-2010
///
/// This module is Copyright (c) 2005-2010 Steve Powell
/// All rights reserved.
///
/// This library is free software; you can redistribute it and/or
/// modify it under the terms of the Microsoft Public License (Ms-PL)
/// 
/// This library is distributed in the hope that it will be
/// useful, but WITHOUT ANY WARRANTY; without even the implied
/// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
/// PURPOSE.  See theMicrosoft Public License (Ms-PL) License for more
/// details.
///
/// You should have received a copy of the Microsoft Public License (Ms-PL)
/// License along with this library; if not you may 
/// find it here: http://www.opensource.org/licenses/ms-pl.html
///
/// Steve Powell, spowell@scrimpnet.com
**/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using System.Text.RegularExpressions;
using ScrimpNet.Core.Diagnostics;
using ScrimpNet.Core;
using ScrimpNet.Core.Text;

namespace ScrimpNet.Core.Configuration
{
    /// <summary>
    /// Methods to retrieve values from connection strings.  Contains FSFH (fail-fast, fail-hard) logic and appropriate logging
    /// </summary>
    public class ConfigUtils
    {
        /// <summary>
        /// Get a required value from AppSettings section of the configuration pipeline
        /// </summary>
        /// <typeparam name="T">Type to cast result to</typeparam>
        /// <param name="key">Key in AppSettings section containing value</param>
        /// <returns>Converted type</returns>
        /// <exception cref="ConfigurationErrorsException">Thrown if key cannot be found or converted into requested type</exception>
        public static T AppSetting<T>(string key)
        {
            string configValue = ConfigurationManager.AppSettings[resolveValueSetting(key)];
            if (configValue == null)
            {
                string msg = string.Format("Unable to find required <AppSettings> key = \"{0}\" (resolved: {1})in .config file or configuration pipeline", key,resolveValueSetting(key));
                Log.LastChanceLog(msg);
                throw new ConfigurationErrorsException(msg);
            }
            try
            {
                configValue = resolveValueSetting(configValue);
                return configValue.ConvertTo<T>();
            }
            catch (Exception ex)
            {
                Log.LastChanceLog(ex, "Unable to convert '{0}' to type of {1}", configValue, typeof(T).FullName);
                throw;
            }
        }

        /// <summary>
        /// Get an optional value from AppSettings section of the configuration pipeline
        /// </summary>
        /// <typeparam name="T">Type to cast result to</typeparam>
        /// <param name="key">Key in AppSettings section containing value</param>
        /// <param name="defaultValue">default to use if key is not found in configuration file</param>
        /// <returns>Converted type</returns>
        public static T AppSetting<T>(string key, T defaultValue)
        {
            var expandedKey = resolveValueSetting(key);
            string configValue = ConfigurationManager.AppSettings[expandedKey];
            if (configValue == null)
            {
                string msg = string.Format("Unable to find <AppSettings> key=\"{0}\" in .config or configuration pipeline.  Using default value '{1}' instead",
                    expandedKey, defaultValue.ToString());
                Log.LastChanceLog(LogLevel.Warning, msg);
                return defaultValue;
            }
            try
            {
                configValue = resolveValueSetting(configValue);
                return configValue.ConvertTo<T>(defaultValue);
            }
            catch (Exception ex)
            {
                Log.LastChanceLog(ex, "Unable to convert '{0}' to type of {1}.  Using '2' instead", configValue, typeof(T).FullName, defaultValue.ToString());
                throw;
            }
        }

        /// <summary>
        /// Gets a connection string from .config or configuration pipeline
        /// </summary>
        /// <param name="key">Key in connection string to get</param>
        /// <returns>Connection string as found in .config file</returns>
        /// <exception cref="ConfigurationException">If key cannot be found in .config file</exception>
        public static string ConnectionString(string key)
        {
            string expandedKey = resolveValueSetting(key);
            ConnectionStringSettings configValue = ConfigurationManager.ConnectionStrings[expandedKey];
            if (configValue == null)
            {
                string msg = string.Format("Unable to find <ConnectionStrings> key = \"{0}\" in .config file or configuration pipeline",
                    expandedKey);
                Log.LastChanceLog(msg);
                throw new ConfigurationErrorsException(msg);
            }
            return resolveValueSetting(configValue.ConnectionString);
        }

        /// <summary>
        /// Method to expand tokens within a key
        /// </summary>
        /// <param name="tokenizedString">String with one of these tokens {env}, {app}, {machine}, {user} </param>
        /// <returns>A string with tokens expanded</returns>
        /// <remarks>
        /// <para>{env} - active environment</para>
        /// <para>{app} - application key</para>
        /// <para>{machine} - machine name</para>
        /// <para>{user} - user name</para>
        /// <para>{ts} - timestamp format tokens (</para>
        /// </remarks>
        internal static string ReplaceKeyTokens(string tokenizedString) //EXTEND
        {
            return tokenizedString.Replace("{machine}", Environment.MachineName).Replace("{env}", CoreConfig.ActiveEnvironment).Replace("{app}", CoreConfig.ApplicationKey).Replace("{user}", Environment.UserName);
        }

        /// <summary>
        /// '{%'
        /// </summary>
        private readonly static string START_TOKEN = settingWithDefault("Config.Expdander.StartToken","{%");
        /// <summary>
        /// '%}'
        /// </summary>
        private readonly static string STOP_TOKEN = settingWithDefault("Config.Expander.StopToken","%}");

        private static string settingWithDefault(string key, string defaultValue)
        {
            string configValue = ConfigurationManager.AppSettings[key];
            if (configValue == null)
            {
                return defaultValue;
            }
            return configValue;
        }
        private static string resolveValueSetting(string configValue)
        {
            //-------------------------------------------------------
            // key was not found or value was empty string
            //-------------------------------------------------------
            if (string.IsNullOrEmpty(configValue) == true)
            {
                return configValue;
            }
            configValue = ReplaceKeyTokens(configValue);

            //-------------------------------------------------------
            // look for any expansion indicators 
            //-------------------------------------------------------
            var tokenPattern = new Regex(START_TOKEN + @"([^"+START_TOKEN+"]+):([^"+STOP_TOKEN+"]+)" + STOP_TOKEN, RegexOptions.Compiled);
            var matches = tokenPattern.Matches(configValue);
            if (matches.Count == 0)  // no expansion needed
            {
                return configValue;
            }

            //-------------------------------------------------------
            // recursively expand all CONFIG_TOKEN elements
            // as defined by START_TOKEN and STOP_TOKEN
            //-------------------------------------------------------
            for (var x = matches.Count - 1; x >= 0; x--)
            {
                Match match = matches[x];
                var fullKey = match.Groups[0].Value;
                var sectionIdentifier = match.Groups[1].Value.ToLower();
                string configKey = match.Groups[2].Value;
                string resolvedValue = "";
                switch (sectionIdentifier)
                {
                    case "app":
                        resolvedValue = ConfigurationManager.AppSettings[ReplaceKeyTokens(configKey)];
                        resolvedValue = resolveValueSetting(resolvedValue);
                        break;
                    case "connection":
                        resolvedValue =
                            resolvedValue = ConfigurationManager.ConnectionStrings[ReplaceKeyTokens(configKey)].ConnectionString;
                        resolvedValue = resolveValueSetting(resolvedValue);
                        break;
                    default:
                        throw new ConfigurationErrorsException(TextUtils.StringFormat("Unable to resolve key: '{0}'", match.Groups[0].Value));
                }
                if (string.IsNullOrEmpty(resolvedValue)==false)
                {
                    resolvedValue = ReplaceKeyTokens(resolvedValue);
                }
                int tokenPosition = configValue.IndexOf(fullKey);  //replace CONFIG_TOKEN with actual resolved value
                configValue = configValue.Remove(tokenPosition, fullKey.Length);
                configValue = configValue.Insert(tokenPosition, resolvedValue);              
            }
            return configValue;
        }
    }
}
