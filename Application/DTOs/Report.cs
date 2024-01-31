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
        [JsonPropertyName("dt_start")]
        public DateTime Start { get; set; }
        [JsonPropertyName("dt_end")]
        public DateTime End { get; set; }

        public List<Group> Groups { get; set; }

        [JsonIgnore]
        public List<Worker> Workers { get; set; } = new List<Worker>();
    }

    public class Group : IComparable
    {
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("users")]
        public List<Worker> Workers { get; set; } = new List<Worker>();

        public int CompareTo(object? obj)
        {
            if (obj is Group gp) return string.Compare(gp.Name, Name, StringComparison.Ordinal);
            throw new ArgumentException("Некорректное значение параметра");
        }
    }

    public class Worker 
    {
        double _total => WorkTimes.Sum(e => e.Total.TotalHours);

        
        public string Name { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        [JsonIgnore]
        public Group? Group { get; set; }

        //public string group { get; set; }

        [JsonPropertyName("total")]
        public string TotalTime => $"{(int)_total:00}:{(((_total - (int)_total)) * 60):00}";

        [JsonPropertyName("details")]
        public List<WorkTime> WorkTimes { get; set; } = new List<WorkTime>();
    }

    public class WorkTime
    {
        // Время входа
        //[JsonPropertyName("t1")]
        [JsonPropertyName("dt_in")]
        public DateTime EntryTime { get; set; }

        // время выхода
        //[JsonPropertyName("t2")]
        [JsonPropertyName("dt_out")]
        public DateTime ExitTime { get; set; }

        // устройства входа
        //[JsonPropertyName("r1")]
        [JsonPropertyName("reader_in")]
        public string FirstReader { get; set; }

        // устройство выхода
        //[JsonPropertyName("r2")]
        [JsonPropertyName("reader_out")]
        public string LastReader { get; set; }

        [JsonIgnore]
        public TimeSpan Total => (ExitTime - EntryTime);

        [JsonPropertyName("time")]
        public string Tot => $"{(int)Total.TotalHours:00}:{Total.Minutes:00}";
    }
}
