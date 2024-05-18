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

        public Task<Report> GetReportByReaders(DateTime startDate, DateTime endDate, List<int> inputReader, List<int> outputReader, int messageId=2);
        /// <summary>
        /// Возвращает список сотрудников прошедших регистрацию в системе за определенный период 
        /// </summary>
        /// <param name="startDate">начало периода</param>
        /// <param name="endDate">окончание</param>
        /// <returns></returns>
        public Task<List<User>> GetUsersAsync(DateTime startDate, DateTime endDate);
        public Task<List<Event>> TrackingByUserIdAndDateAsync(int userId, DateTime dateTime);
        public Task<Event> GetLastUseKey(int userId);

        public List<DateTime> GetWorkingDaysByUserId(int userId, DateTime startDate);

        public Task<List<User>> GetWorkersTodayAsync();


    }
}
