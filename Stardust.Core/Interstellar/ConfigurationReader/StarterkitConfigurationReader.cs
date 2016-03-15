using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using Stardust.Interstellar.Serializers;
using Stardust.Nucleus;
using Stardust.Nucleus.Extensions;
using Stardust.Particles;
using Stardust.Particles.Xml;

namespace Stardust.Interstellar.ConfigurationReader
{
    public abstract class NotificationHandlerBase
    {
        private readonly IConfigurationReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        protected NotificationHandlerBase(IConfigurationReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Override this to support customer readers not based on the StarterkitConfigurationReader.
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanRaiseEvent()
        {
            return reader.Implements<StarterkitConfigurationReader>();
        }

        /// <summary>
        /// Implement the cache monitor code here, eighter using signalR or any other signaling mechanism.
        /// </summary>
        protected abstract void Monitor();

        protected virtual void NotifyChange(string setName, string environmentName, ConfigurationSet configSet)
        {
            var oldSet = (ConfigurationSet)MemoryCache.Default.Get(StarterkitConfigurationReader.GetCacheKey(setName, environmentName));
            if (oldSet.ETag != configSet.ETag)
            {
                MemoryCache.Default.Remove(StarterkitConfigurationReader.GetCacheKey(setName, environmentName));
                MemoryCache.Default.Add(StarterkitConfigurationReader.GetCacheKey(setName, environmentName), configSet, new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 0, StarterkitConfigurationReader.GetCacheExpirationTime()) });
            }
            if (StarterkitConfigurationReader.changeHandler != null) StarterkitConfigurationReader.changeHandler(configSet);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class StarterkitConfigurationReader : IConfigurationReader
    {
        private readonly IAppPoolRecycler recycler;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        [Using]
        public StarterkitConfigurationReader(IAppPoolRecycler recycler)
        {
            this.recycler = recycler;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public StarterkitConfigurationReader()
        {
        }

        private static int? ExpireTime;

        internal static Action<ConfigurationSet> changeHandler;

        /// <summary>
        /// Reads the configSet from the store
        /// </summary>
        /// <param name="setName"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public ConfigurationSet GetConfiguration(string setName, string environment = null)
        {
            if (environment.IsNullOrWhiteSpace()) environment = Utilities.Utilities.GetEnvironment();////ConfigurationManagerHelper.GetValueOnKey("environment");
            ConfigurationSet item;
            var faultedCache = GetSettingsFromCache(setName, environment, out item);
            if (item.IsInstance()) return item;
            item = GetConfigurationSet(setName, environment);
            SaveConfigSetToCache(setName, environment, faultedCache, item);
            return item;
        }

        /// <summary>
        /// Saves a configSet to the cache
        /// </summary>
        /// <param name="setName">the set name</param>
        /// <param name="environment">the current environment</param>
        /// <param name="faultedMemCache">the state of the cache </param>
        /// <param name="item">the configSet to cache</param>
        protected virtual void SaveConfigSetToCache(string setName, string environment, bool faultedMemCache, ConfigurationSet item)
        {
            Logging.DebugMessage("[ConfigSet cache is not primed]");
            if (!faultedMemCache)
                MemoryCache.Default.Set(setName + environment, item,
                    new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 0, GetCacheExpirationTime()) });
        }


        /// <summary>
        /// Retreives the configSet from the cache, returns null if not found
        /// </summary>
        /// <param name="setName">the set name</param>
        /// <param name="environment">the current environment</param>
        /// <param name="item">the config set to return</param>
        /// <returns>The state of the cache.</returns>
        protected internal virtual bool GetSettingsFromCache(string setName, string environment, out ConfigurationSet item)
        {
            var faultedMemCache = ValidateCache();
            item = null;
            if (!faultedMemCache)
                item = (ConfigurationSet)MemoryCache.Default.Get(GetCacheKey(setName, environment));
            return faultedMemCache;
        }

        internal static string GetCacheKey(string setName, string environment)
        {
            return string.Format("{0}{1}", setName, environment);
        }

        private static bool ValidateCache()
        {
            try
            {
                bool faultedMemCache;
                faultedMemCache = validateMemCache();
                if (faultedMemCache)
                    Logging.DebugMessage("ConfigSet MemoryCache error.....");
                return faultedMemCache;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool validateMemCache()
        {
            var faultedMemCache = false;
            var threadDelegate = new ThreadStart(() => GetCacheCount());
            var thread_ = new Thread(threadDelegate);
            thread_.Start();
            var timer = Stopwatch.StartNew();
            while (thread_.IsAlive)
            {
                if (timer.ElapsedMilliseconds <= 20) continue;
                try
                {
                    thread_.Join();
                    thread_.Abort();

                    faultedMemCache = true;
                }
                catch (ThreadAbortException ex)
                {
                    faultedMemCache = true;
                    Logging.Exception(ex, "ThreadAborted outside");
                }
                catch (Exception)
                {
                    faultedMemCache = true;
                }
                finally
                {

                }
            }
            timer.Stop();
            return faultedMemCache;
        }

        private static int GetCacheCount()
        {
            try
            {
                return MemoryCache.Default.Count();
            }
            catch (ThreadAbortException ex)
            {
                Logging.Exception(ex, "ThreadAborted inside");
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
            finally
            {

            }
        }

        internal static int GetCacheExpirationTime()
        {
            if (ExpireTime != null) return ExpireTime.Value;
            var expTime = ConfigurationManagerHelper.GetValueOnKey("stardust.ConfigCacheExpity");
            int val;
            if (!int.TryParse(expTime, out val))
                val = 1000;
            ExpireTime = val;
            return ExpireTime.Value;
        }

        private ConfigurationSet GetConfigurationSet(string setName, string environment)
        {
            using (var wc = GetClient())
            {

                var url = string.Format("{0}/api/ConfigReader/{1}?env={2}&updKey{3}", GetConfigServiceUrl(), setName, environment, (int)(DateTime.Now - DateTime.MinValue).TotalMinutes);
                try
                {
                    wc.CachePolicy = new RequestCachePolicy(GetCahceLevel());
                    SetCredentials(wc);
                    var payload = wc.DownloadString(url);
                    return GetConfigurationFromString(payload);
                }
                catch (Exception ex)
                {
                    var message = string.Format("Unable to open {0}", url);
                    Logging.DebugMessage(message, additionalDebugInformation: GetType().Name + ".GetConfigurationSet");
                    throw new InvalidOperationException(message, ex);
                }
            }
        }

        protected virtual WebClient GetClient()
        {
            return new WebClient();
        }

        private static RequestCacheLevel GetCahceLevel()
        {
            var config = ConfigurationManagerHelper.GetValueOnKey("stardust.ConfigReaderCache");
            if (config.ContainsCharacters()) return config.ParseAsEnum(RequestCacheLevel.Revalidate);
            return RequestCacheLevel.NoCacheNoStore;
        }

        private string GetConfigServiceUrl()
        {
            return ConfigurationManagerHelper.GetValueOnKey("stardust.configLocation");
        }

        private static ConfigurationSet GetConfigurationFromString(string payload)
        {
            try
            {
                if (payload.IsNullOrWhiteSpace()) throw new ArgumentNullException("payload", "No configset raw data available");
                var parser = Resolver.Activate<IReplaceableSerializer>();
                if (parser.IsNull()) throw new NullReferenceException("No parser found");
                return parser.GetConfigurationFromString(payload);
            }
            catch (Exception ex)
            {
                ex.Log("Parser error");
                throw;
            }

        }

        private static void SetCredentials(WebClient wc)
        {
            if (ConfigurationManagerHelper.GetValueOnKey("stardust.useAccessToken") == "true")
            {
                wc.Headers.Add(HttpRequestHeader.Authorization, "Token " + Convert.ToBase64String(ConfigurationManagerHelper.GetValueOnKey("stardust.accessToken").GetByteArray()));
                if (GetTokenKey().ContainsCharacters())
                    wc.Headers.Add("key", GetTokenKey());
                return;
            }
            var userName = GetConfigServiceUser();
            var password = GetConfigServicePassword();
            if (userName.ContainsCharacters() && password.ContainsCharacters())
            {
                wc.Credentials = new NetworkCredential(userName, password, GetConfigServiceDomain());
            }
        }

        private static string GetTokenKey()
        {
            return ConfigurationManagerHelper.GetValueOnKey("stardust.accessTokenKey");
        }

        private static string GetConfigServiceDomain()
        {
            return ConfigurationManagerHelper.GetValueOnKey("stardust.configDomain");
        }

        private static string GetConfigServicePassword()
        {
            return ConfigurationManagerHelper.GetValueOnKey("stardust.configPassword");
        }

        private static string GetConfigServiceUser()
        {
            return ConfigurationManagerHelper.GetValueOnKey("stardust.configUser");
        }

        public void WriteConfigurationSet(ConfigurationSet configuration, string setName)
        {
            throw new NotImplementedException();
        }

        public T GetRawConfigData<T>(string setName)
        {
            throw new NotImplementedException();
        }

        public object GetRawConfigData(string setName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ConfigurationSet> GetAllSets()
        {
            throw new NotImplementedException();
        }

        public bool TryRegisterService(ServiceRegistrationServer.ServiceRegistrationMessage serviceMessage)
        {
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                SetCredentials(wc);
                var url = string.Format("{0}/Registration/TryAddService", GetConfigServiceUrl());
                var data = serviceMessage.Serialize();
                var payload = wc.UploadString(url, data);
                var ds = Deserializer<ServiceRegistrationServer.ServiceRegistrationMessage>.Deserialize(data);
                return payload == "\"OK\"";
            }
        }

        /// <summary>
        /// This will only be used if the Reader implementation supports change notification. not implemented in this, but if you change the cache by overriding this remember to hook up this.
        /// </summary>
        /// <param name="onCacheChanged"></param>
        public virtual void SetCacheInvalidatedHandler(Action<ConfigurationSet> onCacheChanged)
        {
            if (recycler != null) changeHandler = c =>
                  {
                      if (onCacheChanged != null)
                      {
                          onCacheChanged(c);
                      }
                      recycler.TryRecycleCurrent();
                  };
            else
                changeHandler = onCacheChanged;

        }
    }
}