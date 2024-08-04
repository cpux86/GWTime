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
        public List<Worker> Workers { get; } = new List<Worker>();

        public void Item(Worker worker)
        {
            this.Workers.Add(worker);
        }

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
        private TimeSpan _total => TimeSpan.FromHours(WorkTimes.Sum(e => e.Total.TotalHours));
        public string Name { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        public string ContractInfo { get; set; } = string.Empty;

        [JsonIgnore]
        public Group? Group { get; set; }

        [JsonPropertyName("total")]
        public string TotalTime => $"{(int)_total.TotalHours:00}:{_total.Minutes:00}";

        [JsonPropertyName("details")]
        public List<WorkShift> WorkTimes { get; set; } = new List<WorkShift>();
    }

    public class WorkShift
    {
        // Время входа
        //[JsonPropertyName("t1")]
        [JsonPropertyName("dt_in")]
        public DateTime EntryTime { get; }

        // время выхода
        //[JsonPropertyName("t2")]
        [JsonPropertyName("dt_out")]
        public DateTime ExitTime { get; }

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

        public WorkShift(DateTime dt1, DateTime dt2)
        {
            this.EntryTime = dt1;
            this.ExitTime = dt2;
        }

    }
}
