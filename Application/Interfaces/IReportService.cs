using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain;

namespace Application.Interfaces
{
    public interface IReportService
    {
        //public Task<List<Event>> GetFirstAndLastUseKey(int userId, DateTime startDate, DateTime endDate);

        public Task<Report> GetReportByReaders(DateTime startDate, DateTime endDate, List<int> inputReader, List<int> outputReader);
    }
}
