using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
    
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CalmStorm.Shared.Extensions;
using Microsoft.Web.Administration;

namespace CalmStorm.Core.Shared
{
    public class IisSettingsGetHelper
    {

        private string[] appPoolNamesToInclude = {"STORM","FscOnline", "Clr4IntegratedAppPool"};

        private IEnumerable<KeyValuePair<string, string>> GetAppPools()
        {
            List<KeyValuePair<string, string>> results = new List<KeyValuePair<string, string>>();
            try
            {
                using (ServerManager serverManager = new ServerManager())
                {
                    Configuration config = serverManager.GetApplicationHostConfiguration();
                    ConfigurationSection applicationPoolsSection = config.GetSection("system.applicationHost/applicationPools");

                    foreach (var appPoolConfigSection in applicationPoolsSection.GetCollection().Where(x => appPoolNamesToInclude.Contains((string)x.GetAttribute("Name").Value, StringComparer.OrdinalIgnoreCase)))
                    {
                        results.Add(new KeyValuePair<string, string>("-----", "------- " + appPoolConfigSection.GetAttribute("Name").Value + " GetApplicationHostConfiguration -----"));
                        results.AddRange(GetAllAttributesRecursively(appPoolConfigSection, "GetApplicationHostConfiguration"));
                    }
                    
                    //results.AddRange(GetAllAttributesRecursively(applicationPoolsSection, "GetApplicationHostConfiguration"));

                    foreach (ApplicationPool applicationPool in serverManager.ApplicationPools.Where(x => appPoolNamesToInclude.Contains(x.Name, StringComparer.OrdinalIgnoreCase)))
                    {
                        string prefix = "serverManagerApplicationPools";
                        results.Add(new KeyValuePair<string, string>("-----", "------- " + applicationPool.Name + string.Format(" {0} -----", prefix)));
                        results.AddRange(GetAllAttributesRecursively(applicationPool, prefix));
                        results.AddRange(GetAllAttributesRecursively(applicationPool.ProcessModel, prefix + "-ProcessModel"));
                        results.AddRange(GetAllAttributesRecursively(applicationPool.Recycling, prefix + "-Recycling"));
                        results.AddRange(GetAllAttributesRecursively(applicationPool.Cpu, prefix + "-Cpu"));
                        results.AddRange(GetAllAttributesRecursively(applicationPool.Failure, prefix + "-Failure"));
                    }
                   
                }
            }
            catch (Exception ex)
            {
                results.Add(new KeyValuePair<string, string>("GetAppPools-Exception:", ex.Message));
            }
            return results;
        }

        private IList<KeyValuePair<string, string>> GetIisVersion()
        {
            List<KeyValuePair<string, string>> results = new List<KeyValuePair<string, string>>();
            try
            {
                using (Process process = Process.GetCurrentProcess())
                {
                    using (ProcessModule mainModule = process.MainModule)
                    {
                        // main module would be w3wp
                        results.Add(new KeyValuePair<string, string>("IIS Version-process.MainModule-FileVersionInfo-FileMajorPart:", mainModule.FileVersionInfo.FileMajorPart.ToString()));
                        results.Add(new KeyValuePair<string, string>("IIS Version-process.MainModule-FileVersionInfo:", mainModule.FileVersionInfo.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {

                results.Add(new KeyValuePair<string, string>("GetIisVersion-Exception:", ex.Message));
            }
            return results;
        }

        private IList<KeyValuePair<string, string>> GetEnvironmentVariables()
        {
            List<KeyValuePair<string, string>> results = new List<KeyValuePair<string, string>>();
            try
            {
                results.Add(new KeyValuePair<string, string>("Environment-.net version: ", Environment.Version.ToString()));
                results.Add(new KeyValuePair<string, string>("Environment-UserName: ", Environment.UserName));
                results.Add(new KeyValuePair<string, string>("Environment-MachineName: ", Environment.MachineName));
                results.Add(new KeyValuePair<string, string>("Environment-SystemDirectory: ", Environment.SystemDirectory));
                results.Add(new KeyValuePair<string, string>("Environment-Is64BitProcess: ", Environment.Is64BitProcess.ToString()));
                results.Add(new KeyValuePair<string, string>("Environment-UserDomainName: ", Environment.UserDomainName));
                results.Add(new KeyValuePair<string, string>("Environment-CurrentDirectory: ", Environment.CurrentDirectory));
                var environmentVariables = Environment.GetEnvironmentVariables();
                foreach (DictionaryEntry pair in environmentVariables)
                {
                    results.Add(new KeyValuePair<string, string>("Environment-GetEnvironmentVariables_" + pair.Key+":", pair.Value.ToString()));
                }


                //results.Add(new KeyValuePair<string, string>("Environment-CurrentDirectory: ", Environment.CurrentDirectory));
                //results.Add(new KeyValuePair<string, string>("Environment-CurrentDirectory: ", Environment.CurrentDirectory));
                //results.Add(new KeyValuePair<string, string>("Environment-CurrentDirectory: ", Environment.CurrentDirectory));

                
            }
            catch (Exception ex)
            {
                results.Add(new KeyValuePair<string, string>("GetIisVersion-Exception:", ex.Message));
            }
            return results;

        }


        public IEnumerable<KeyValuePair<string, string>> GetSettingsWeCareAbout()
        {
            List<KeyValuePair<string, string>> results = new List<KeyValuePair<string, string>>();
            results.AddRange(GetBasicSiteInfo());
            results.AddRange(GetSiteBindings());
            results.AddRange(GetAuthenticationModes());
            results.AddRange(GetIisVersion());
            results.AddRange(GetEnvironmentVariables());
            results.AddRange(GetAppPools());

            results.AddRange(GetSiteBindingsViaConfig());
            results.AddRange(GetAppPoolsViaWebConfigManager());
            results.AddRange(GetSessionStateViaWebConfigManager());
            results.AddRange(GetAspSettingsViaConfig());
            results.AddRange(GetDefaultDocumentViaConfig());
            results.AddRange(GetCustomErrorsViaWebConfigManager());

            return results;
        }

        private IEnumerable<KeyValuePair<string, string>> GetBasicSiteInfo()
        {
            List<KeyValuePair<string, string>> results = new List<KeyValuePair<string, string>>();
            results.Add(new KeyValuePair<string, string>("HostingEnvironment:SiteName:", System.Web.Hosting.HostingEnvironment.SiteName));
            results.Add(new KeyValuePair<string, string>("HostingEnvironment:ApplicationID:", System.Web.Hosting.HostingEnvironment.ApplicationID));
            results.Add(new KeyValuePair<string, string>("HostingEnvironment:ApplicationPhysicalPath:", System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath));
            results.Add(new KeyValuePair<string, string>("HostingEnvironment:ApplicationVirtualPath:", System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath));
            return results;
        }

        private IEnumerable<KeyValuePair<string, string>> GetSiteBindings()
        {
            
            List<KeyValuePair<string, string>> results = new List<KeyValuePair<string, string>>();
            try
            {
                results.AddRange(GetSiteBindingsViaSites());
            }
            catch (Exception ex)
            {
                results.Add(new KeyValuePair<string, string>("GetSiteBindingsViaSites:Exception", ex.Message));
                //suppress
            }
            return results;
        }

        private List<KeyValuePair<string, string>> GetSiteBindingsViaConfig()
        {
            List<KeyValuePair<string, string>> results = new List<KeyValuePair<string, string>>();
            results.Add(new KeyValuePair<string, string>("Sites", "------------Sites-------------"));
            results.AddRange(GetCollectionViaWebConfigManager("system.applicationHost/sites"));
            results.AddRange(GetCollectionViaWebConfigManager("system.applicationHost/sites", "bindings"));
            results.AddRange(GetCollectionViaWebConfigManager("system.applicationHost/sites", "limits"));
            results.AddRange(GetCollectionViaWebConfigManager("system.applicationHost/sites", "logFile"));
            results.AddRange(GetCollectionViaWebConfigManager("system.applicationHost/sites", "traceFailedRequestsLogging"));
            return results;
        }

         private List<KeyValuePair<string, string>> GetAspSettingsViaConfig()
        {
            List<KeyValuePair<string, string>> results = new List<KeyValuePair<string, string>>();
            results.Add(new KeyValuePair<string, string>("asp", "------------asp-------------"));
            results.AddRange(GetCollectionViaWebConfigManager("system.webServer/asp"));
            results.AddRange(GetCollectionViaWebConfigManager("system.webServer/asp", "session"));
            results.AddRange(GetCollectionViaWebConfigManager("system.webServer/asp", "limits"));
            return results;
        }
        
        private List<KeyValuePair<string, string>> GetDefaultDocumentViaConfig()
        {
            List<KeyValuePair<string, string>> results = new List<KeyValuePair<string, string>>();
            results.Add(new KeyValuePair<string, string>("defaultDocument", "------------defaultDocument-------------"));
            results.AddRange(GetCollectionViaWebConfigManager("system.webServer/defaultDocument"));
            return results;
        }
       

        private List<KeyValuePair<string, string>> GetAppPoolsViaWebConfigManager()
        {
            List<KeyValuePair<string, string>> results = new List<KeyValuePair<string, string>>();
            results.Add(new KeyValuePair<string, string>("applicationPools", "------------applicationPools-------------"));
            results.AddRange(GetCollectionViaWebConfigManager("system.applicationHost/applicationPools"));
            results.AddRange(GetCollectionViaWebConfigManager("system.applicationHost/applicationPools", "processModel"));
            results.AddRange(GetCollectionViaWebConfigManager("system.applicationHost/applicationPools", "recycling"));
            return results;
        }

        private List<KeyValuePair<string, string>> GetSessionStateViaWebConfigManager()
        {
            List<KeyValuePair<string, string>> results = new List<KeyValuePair<string, string>>();
            results.Add(new KeyValuePair<string, string>("sessionState", "------------sessionState-------------"));
            results.AddRange(GetCollectionViaWebConfigManager("system.web/sessionState"));
            results.AddRange(GetCollectionViaWebConfigManager("system.web/sessionState", "providers"));
            
            return results;
        }
        

        private List<KeyValuePair<string, string>> GetCustomErrorsViaWebConfigManager()
        {
            List<KeyValuePair<string, string>> results = new List<KeyValuePair<string, string>>();
            results.Add(new KeyValuePair<string, string>("customErrors", "------------customErrors-------------"));
            results.AddRange(GetCollectionViaWebConfigManager("system.web/customErrors"));
            return results;
        }
        
        /// <summary>
        /// You can find the schema that has the subElement names at 
        /// C:\Windows\System32\inetsrv\config\schema\IIS_schema.xml
        /// </summary>
        /// <param name="rootCollectionString"></param>
        /// <param name="subElementName"></param>
        /// <returns></returns>
        private List<KeyValuePair<string, string>> GetCollectionViaWebConfigManager(string rootCollectionString, string subElementName = "")
        {
            List<KeyValuePair<string, string>> results = new List<KeyValuePair<string, string>>();
            try
            {
                string siteName = System.Web.Hosting.HostingEnvironment.SiteName;
                
                ConfigurationSection sitesSection = WebConfigurationManager.GetSection(null, null, rootCollectionString);

                Func<ConfigurationElement, bool> matchesSiteNamePredicate = x => String.Equals((string)x["name"], siteName, StringComparison.OrdinalIgnoreCase);
                Func<ConfigurationElement, bool> matchesAppPoolNamePredicate = x => String.Equals((string)x["name"], Environment.GetEnvironmentVariable("APP_POOL_ID"), StringComparison.OrdinalIgnoreCase);
                string preString = "WCM:" + rootCollectionString;

                if (sitesSection.GetCollection() != null)
                {
                    if (sitesSection.GetCollection().Any(matchesSiteNamePredicate))
                    {
                        var ourSitesSection = sitesSection.GetCollection().First(matchesSiteNamePredicate);
                        if (ourSitesSection != null)
                        {
                            if (string.IsNullOrEmpty(subElementName))
                            {
                                results.AddRange(GetAllAttributesRecursively(ourSitesSection, preString));
                            }
                            else
                            {
                                if (ourSitesSection.GetCollection(subElementName) != null)
                                {
                                    foreach (ConfigurationElement binding in ourSitesSection.GetCollection(subElementName))
                                    {
                                        results.AddRange(GetAllAttributesRecursively(binding, preString));
                                    }
                                }
                                var section = ourSitesSection.ChildElements.FirstOrDefault(x => string.Equals(x.Schema.Name, subElementName, StringComparison.OrdinalIgnoreCase));
                                if (section != null)
                                {
                                    results.AddRange(GetAllAttributesRecursively(section, preString));
                                }

                            }
                        }
                    }
                    else if (sitesSection.GetCollection().Any(matchesAppPoolNamePredicate))
                    {
                        var ourAppPoolSection = sitesSection.GetCollection().First(matchesAppPoolNamePredicate);
                        if (ourAppPoolSection != null)
                        {
                            if (string.IsNullOrEmpty(subElementName))
                            {
                                results.AddRange(GetAllAttributesRecursively(ourAppPoolSection, preString));
                            }
                            else
                            {
                                var childElement = ourAppPoolSection.GetChildElement(subElementName);
                                results.AddRange(GetAllAttributesRecursively(childElement, preString));
                            }
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(subElementName))
                    {
                        results.AddRange(GetAllAttributesRecursively(sitesSection, preString));
                    }
                    else
                    {
                        var childElement = sitesSection.GetChildElement(subElementName);
                        results.AddRange(GetAllAttributesRecursively(childElement, preString));
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                results.Add(new KeyValuePair<string, string>("Exception-GetCollectionViaWebConfigManager trying to get " + rootCollectionString + " - " + subElementName, ex.ToString()));
                return results;
            }
        }



       

        private static IEnumerable<KeyValuePair<string, string>> GetSiteBindingsViaSites( )
        {
            // Get the Site name  
            List<KeyValuePair<string, string>> results = new List<KeyValuePair<string, string>>();

            using (var server = new ServerManager())
            {
                var site2 = server.Sites.FirstOrDefault(s => string.Equals(s.Name, System.Web.Hosting.HostingEnvironment.SiteName, StringComparison.OrdinalIgnoreCase));
                if (site2 != null)
                {
                    foreach (Binding binding in site2.Bindings)
                    {
                        results.Add(new KeyValuePair<string, string>("ViaSites:"+binding.Schema.Name + ".EndPoint:", binding.EndPoint.ToString()));
                        results.Add(new KeyValuePair<string, string>("ViaSites:"+binding.Schema.Name + ".Host:", binding.Host));
                        results.Add(new KeyValuePair<string, string>("ViaSites:"+binding.Schema.Name + ".IsIPPortHostBinding:", binding.IsIPPortHostBinding.ToString()));
                        results.Add(new KeyValuePair<string, string>("ViaSites:"+binding.Schema.Name + ".Protocol:", binding.Protocol));
                        results.Add(new KeyValuePair<string, string>("ViaSites:"+binding.Schema.Name + ".ToString:", binding.ToString()));
                        results.Add(new KeyValuePair<string, string>("ViaSites:"+binding.Schema.Name + ".UseDsMapper:", binding.UseDsMapper.ToString()));
                        results.Add(new KeyValuePair<string, string>("ViaSites:"+binding.Schema.Name + ".BindingInformation:", binding.BindingInformation));
                        results.Add(new KeyValuePair<string, string>("ViaSites:"+binding.Schema.Name + ".Protocol:", binding.Protocol));
                    }
                }
            }
            return results;
        }


        private IList<KeyValuePair<string, string>> GetAllAttributesRecursively(ConfigurationElement configElement, string preString)
        {
            var results = new List<KeyValuePair<string, string>>();
            foreach (var attribute in configElement.Attributes)
            {
                results.Add(new KeyValuePair<string, string>(preString +"-"+ configElement.Schema.Name + "." + attribute.Name, GetSafeAttributeValue(attribute)));
            }

            try
            {
                foreach (var childElement in configElement.ChildElements)
                {
                    results.AddRange(GetAllAttributesRecursively(childElement, preString + "-" + configElement.Schema.Name));
                }
            }
            catch
            {
                //suppress - childelements is often not implemeted?
            }

            try
            {
                if (configElement.GetCollection() != null)
                {
                    foreach (var childElement in configElement.GetCollection())
                    {
                        results.AddRange(GetAllAttributesRecursively(childElement, preString + "-" + configElement.Schema.Name));
                    }
                }
            }
            catch
            {
                //suppress - childelements is often not implemeted?
            }

            return results;
        }


        private IList<KeyValuePair<string, string>> GetAllAttributes(ConfigurationElement binding, string preString)
        {
            var results = new List<KeyValuePair<string, string>>();
            foreach (var attribute in binding.Attributes)
            {
                results.Add(new KeyValuePair<string, string>(preString+binding.Schema.Name + "." + attribute.Name, GetSafeAttributeValue(attribute)));
            }

            return results;
        }

        private string GetSafeAttributeValue(ConfigurationAttribute attribute)
        {
            try
            {
                return attribute.Value != null ? attribute.Value.ToString() : "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        private IEnumerable<KeyValuePair<string, string>> GetAuthenticationModes()
        {
            var results = new List<KeyValuePair<string, string>>();
            string siteName = System.Web.Hosting.HostingEnvironment.SiteName;
            using (ServerManager serverManager = new ServerManager())
            {
                Configuration config = serverManager.GetApplicationHostConfiguration();
                string prefix = "GetApplicationHostConfiguration-Auth:";
                results.Add(new KeyValuePair<string, string>(prefix, ""));
                results.AddRange(GetSafeConfigSection(config, siteName, "system.webServer/security/authentication/anonymousAuthentication", prefix));
                results.AddRange(GetSafeConfigSection(config, siteName, "system.webServer/security/authentication/windowsAuthentication", prefix));
                results.AddRange(GetSafeConfigSection(config, siteName, "system.webServer/security/authentication/FormsAuthentication", prefix));
                results.AddRange(GetSafeConfigSection(config, siteName, "system.webServer/security/authentication/FormsAuthentication", prefix));

                results.AddRange(GetSafeConfigSection(config, siteName, "system.web/identity", prefix));
                results.AddRange(GetSafeConfigSection(config, siteName, "system.web/authentication", prefix));
                  

                Configuration configWeb = serverManager.GetWebConfiguration(siteName);
                string prefixWebConfig = "GetWebConfiguration:";
                results.Add(new KeyValuePair<string, string>(prefixWebConfig, ""));
                results.AddRange(GetSafeConfigSection(configWeb, siteName, "system.webServer/security/authentication/anonymousAuthentication", prefixWebConfig));
                results.AddRange(GetSafeConfigSection(configWeb, siteName, "system.webServer/security/authentication/windowsAuthentication", prefixWebConfig));
                results.AddRange(GetSafeConfigSection(configWeb, siteName, "system.webServer/security/authentication/FormsAuthentication", prefixWebConfig));
                //ConfigurationSection authenticationSection = config.RootSectionGroup.SectionGroups. GetEffectiveSectionGroup("system.webServer/security", siteName);

                results.AddRange(GetSafeConfigSection(configWeb, siteName, "system.web/identity", prefixWebConfig));
                results.AddRange(GetSafeConfigSection(configWeb, siteName, "system.web/authentication", prefixWebConfig));

                try
                {
                    var identity = WebConfigurationManager.GetSection(null, null, "system.web/identity");
                    results.AddRange(GetAllAttributesRecursively(identity, "WebConfigurationManager:"));
                    var auth = WebConfigurationManager.GetSection(null, null, "system.web/authentication");
                    results.AddRange(GetAllAttributesRecursively(auth, "WebConfigurationManager:"));
                }
                catch (Exception ex)
                {
                    results.Add(new KeyValuePair<string, string>("GetAuthenticationModes:Exception", ex.Message));
                    //suppress
                }
                

            }
            return results;
        }

        private IList<KeyValuePair<string, string>> GetSafeConfigSection(Configuration config, string siteName, string path,string preString )
        {

            try
            {
                ConfigurationSection anonymousAuthenticationSection = config.GetSection(path, siteName);
                return GetAllAttributesRecursively(anonymousAuthenticationSection, preString);
            }
            catch (Exception ex)
            {
                return new[] {new KeyValuePair<string, string>("Exception-GetSafeConfigSection-" + preString, ex.Message)};
            }
        }


        private void RestartIis()
        {
            using (var server = new ServerManager())
            {
                var site = server.Sites.FirstOrDefault(s => string.Equals(s.Name, System.Web.Hosting.HostingEnvironment.SiteName, StringComparison.OrdinalIgnoreCase));
                if (site != null)
                {
                    //stop the site...
                    site.Stop();
                    if (site.State == ObjectState.Stopped)
                    {
                        //do deployment tasks...
                    }
                    else
                    {
                        throw new InvalidOperationException("Could not stop website!");
                    }
                    //restart the site...
                    site.Start();
                }
                else
                {
                    throw new InvalidOperationException("Could not find website!");
                }
            }

        }

    }

}
