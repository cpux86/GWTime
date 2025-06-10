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


    public class Group : IComparable<Group>
    {
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("users")]
        public List<Worker> Workers { get; set; } = new List<Worker>();


        public int CompareTo(Group? other)
        {
            //if (other is Group gp) return string.Compare(gp.Name, Name, StringComparison.Ordinal);
            //throw new ArgumentException("Некорректное значение параметра");
            if (other == null)
                throw new ArgumentNullException(nameof(other), "Сравниваемый объект не может быть null");
            return string.Compare(Name, other.Name, StringComparison.Ordinal);
        }
    }

    public class Worker 
    {
        private TimeSpan _total => TimeSpan.FromHours(WorkTimes.Sum(e => e.TotalDuration.TotalHours));
        
        public string Name { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        public string ContractInfo { get; set; } = string.Empty;

        [JsonIgnore]
        public Group? Group { get; set; }

        [JsonPropertyName("total")]
        public string TotalTime => $"{(int)_total.TotalHours:00}:{_total.Minutes:00}";

        [JsonPropertyName("details")]
        public List<WorkTime> WorkTimes { get; set; } = new List<WorkTime>();
    }

    public class WorkTime
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

        // Общее время работы
        [JsonIgnore]
        public TimeSpan TotalDuration => (ExitTime - EntryTime);

        [JsonPropertyName("time")]
        public string Tot => $"{(int)TotalDuration.TotalHours:00}:{TotalDuration.Minutes:00}:{TotalDuration.Seconds:00}";
        //public string Tot => $"{TotalDuration.TotalHours:F0}:{TotalDuration.Minutes:D2}";


        public WorkTime(DateTime dt1, DateTime dt2)
        {
            this.EntryTime = dt1;
            this.ExitTime = dt2;
        }

    }
}
