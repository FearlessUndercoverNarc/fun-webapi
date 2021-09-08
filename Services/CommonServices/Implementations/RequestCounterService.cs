using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Models.Misc;
using Services.CommonServices.Abstractions;

namespace Services.CommonServices.Implementations
{
    public class RequestCounterService : IRequestCounterService
    {
        private readonly ILogger<RequestCounterService> _logger;
        private readonly Dictionary<string, RequestData> _dictionary;

        private int _totalPhp = 0;
        private int _totalPhpMyAdmin = 0;

        private readonly Mutex _mutex;

        public RequestCounterService(ILogger<RequestCounterService> logger)
        {
            _logger = logger;
            _dictionary = new();
            _mutex = new();
        }

        public void Notice(string path)
        {
            var nowDateString = DateTime.Now.ToString("yy.MM.dd HH:mm:ss");
            
            // Use double check with a mutex 
            if (!_dictionary.ContainsKey(path))
            {
                _mutex.WaitOne();
                if (!_dictionary.ContainsKey(path))
                {
                    _logger.LogInformation("Adding a request map");
                    _dictionary.Add(path, new RequestData(nowDateString, 0));
                }
                _mutex.ReleaseMutex();
            }

            var requestData = _dictionary[path];

            requestData.Amount++;
            requestData.LastRequest = nowDateString;

            if (path.EndsWith(".php"))
            {
                _totalPhp++;
            }

            if (path.Contains("phpmyadmin"))
            {
                _totalPhpMyAdmin++;
            }
        }

        public (IDictionary<string, RequestData> stats, int totalPhp, int totalPhpMyAdmin) Get()
        {
            return (
                _dictionary
                    .OrderByDescending(r => r.Value.Amount)
                    .ToDictionary(x => x.Key, x => x.Value),
                _totalPhp,
                _totalPhpMyAdmin);
        }
    }
}