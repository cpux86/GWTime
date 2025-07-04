﻿
using Domain;
using PRTelegramBot.Interfaces;
using PRTelegramBot.Models;

namespace Api.BotControllers.Dialog
{

    public class TrackingCache : ITelegramCache
    {
        public string FullName { get; set; }
        public int UserId { get; set; }
        public OptionMessage Options { get; set; } = new OptionMessage();
        public DateTime DateTime { get; set; }
        public bool ClearData()
        {
            this.FullName = string.Empty;
            return true;
        }
    }
}
