using System;
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

        public List<Worker> Workers { get; set; } = new List<Worker>();
    }

    public class Worker
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Group { get; set; } = string.Empty;
        // общее время 
        public string TotalTime => WorkTimes.Sum(e => e.Total.TotalHours).ToString();
        public List<WorkTime> WorkTimes { get; set; } = new List<WorkTime>();

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

        // устройство выхода\
        [JsonPropertyName("r2")]
        public string LastReader { get; set; }

        public TimeSpan Total => ExitTime - EntryTime;

        public string Tot => $"{(int)Total.TotalHours:00}:{Total.Minutes:00}";

       
    }
}
