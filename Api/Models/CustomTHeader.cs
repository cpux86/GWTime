using PRTelegramBot.Attributes;

namespace Api.Models
{
    /// <summary>
    /// Кастомные заголовки команд
    /// </summary>
    [InlineCommand]
    public enum CustomTHeader
    {
        Chronos = 100,
        Kurilka1,
        Kurilka2,
        Galvanika
    }
}
