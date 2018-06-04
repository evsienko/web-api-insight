﻿using System;
using log4net;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace WebApiInsight.Agent
{
    public class AspNetCollector : BaseCollector
    {
        public AspNetCollector(ILog logger, IDbManager dbManager)
            : base(logger, dbManager)
        {
        }
        
        protected override void InitMetricRecords()
        {
            MetricsConfig = MetricsConfigManager.ReadMetricsConfig()
                .AspNetMetricsConfig
                .ToList();
        }

        public override void Start()
        {
            Logger.InfoFormat("Started asp.net metrics reading (for the pool {0})", Settings.PoolName);
            while (true)
            {
                try
                {
                    SaveMetrics();
                }
                catch (InvalidOperationException ex)
                {
                    Logger.Error("Presumably go to sleep. Detail: {0}", ex);
                }
                catch (Exception ex)
                {
                    Logger.Error("Error: {0}", ex);
                }
                Thread.Sleep(Settings.ReadingInterval);
            }
        }

        private void SaveMetrics()
        {
            InstanceName = ProcessHelper.GetInstanseName(Settings.AppName, Settings.PoolName);
            var counters = MetricsConfig
                .Select(c => new MeasurementData
                {
                    Measurement = c.Measurement,
                    Counter = new PerformanceCounter
                    {
                        CategoryName = c.CategoryName,
                        CounterName = c.CounterName,
                        InstanceName = InstanceName
                    }
                })
                .ToList();
            try
            {

                while (true)
                {
                    counters.ForEach(WriteRecord);
                    Thread.Sleep(Settings.ReadingInterval);
                }
            }
            finally
            {
                counters.ForEach(c =>
                {
                    if (c.Counter != null)
                        c.Counter.Dispose();
                });
            }
        }
    }
}