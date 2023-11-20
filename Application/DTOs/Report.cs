﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain;

namespace Application.DTOs
{
    public class Report
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<Worker> Workers{ get; set; }
    }

    public class Worker
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; }
        public string FullName { get; set; }
        // общее время 
        public string TotalTime { get; set; }
        public List<WorkTime> WorkTimes { get; set; }

        public void AddTime(WorkTime workTime)
        {

        }
    }

    public class WorkTime
    {
        // Время входа
        [JsonPropertyName("t1")]
        public DateTime EntryTime { get; set; }

        // время выхода
        [JsonPropertyName("t2")]
        public DateTime ExitTime { get; set; }
        // устройства входа
        [JsonPropertyName("r1")]
        public string FirstReader { get; set; }
        [JsonPropertyName("r2")]
        // устройство выхода
        public string LastReader { get; set; }
        [JsonIgnore]
        public TimeSpan Total { get; set; }

        public string Tot => $"{(int)Total.TotalHours:00}:{Total.Minutes:00}";

       
    }
}
