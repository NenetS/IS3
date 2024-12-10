using System;
using System.IO;

enum LogLevel { TRACE = 1, DEBUG, INFO, WARNING, ERROR, CRITICAL }

class Logger
{
    private StreamWriter logFile;
    private LogLevel currentLogLevel = LogLevel.TRACE; // Уровень по умолчанию
    private bool hasError = false;

    public Logger(string filename)
    {
        logFile = new StreamWriter(filename, true);
    }

    ~Logger()
    {
        logFile.Close();
    }

    public void SetLevel(LogLevel level)
    {
        currentLogLevel = level;
        Console.WriteLine($"Уровень логирования установлен на: {level}");

        // Сбрасываем флаг hasError при установке уровня выше ERROR
        if (currentLogLevel > LogLevel.ERROR)
        {
            hasError = false;
        }
    }

    public void Log(LogLevel level, string message)
    {
        // Проверяем, какие уровни сообщений разрешены в зависимости от установленного уровня
        if (!IsLogLevelAllowed(level)) return;

        // Обновляем флаги на основе уровня логирования
        if (level == LogLevel.ERROR)
            hasError = true;

        // Получаем текущий временной штамп
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Формируем запись лога с порядковым номером уровня
        string logEntry = $"[{timestamp}] {GetLogLevelNumber(level)} - {level}: {message}";

        // Выводим в консоль
        Console.WriteLine(logEntry);

        // Записываем в файл
        logFile.WriteLine(logEntry);
        logFile.Flush(); // Обеспечиваем немедленную запись в файл
    }

    // Метод для получения порядкового номера уровня логирования
    private int GetLogLevelNumber(LogLevel level)
    {
        return (int)level; // Преобразуем уровень в его порядковый номер
    }

    // Метод для проверки разрешенного уровня логирования
    public bool IsLogLevelAllowed(LogLevel level)
    {
        switch (currentLogLevel)
        {
            case LogLevel.TRACE:
                return true; // TRACE разрешает все уровни
            case LogLevel.DEBUG:
                return level == LogLevel.DEBUG || level == LogLevel.INFO; // DEBUG разрешает DEBUG и INFO
            case LogLevel.INFO:
                return level == LogLevel.INFO || level == LogLevel.ERROR; // INFO разрешает INFO и ERROR
            case LogLevel.WARNING:
                return level == LogLevel.WARNING || level == LogLevel.ERROR || level == LogLevel.CRITICAL; // WARNING разрешает WARNING, ERROR и CRITICAL
            case LogLevel.ERROR:
                return level == LogLevel.ERROR; // ERROR разрешает только ERROR
            case LogLevel.CRITICAL:
                return level == LogLevel.ERROR || level == LogLevel.CRITICAL; // CRITICAL разрешает ERROR и CRITICAL
            default:
                return false;
        }
    }
}

class Program
{
    static void Main()
    {
        Logger logger = new Logger("logfile.txt"); // Создаем экземпляр логгера

        while (true)
        {
            Console.WriteLine("Выберите уровень логирования (1 - TRACE, 2 - DEBUG, 3 - INFO, 4 - WARNING, 5 - ERROR, 6 - CRITICAL):");
            string input = Console.ReadLine();

            if (int.TryParse(input, out int level) && Enum.IsDefined(typeof(LogLevel), level))
            {
                logger.SetLevel((LogLevel)level);

                // Логируем сообщения в зависимости от выбранного уровня
                logger.Log(LogLevel.INFO, "Программа запущена.");
                logger.Log(LogLevel.DEBUG, "Отладочная информация.");
                logger.Log(LogLevel.ERROR, "Произошла ошибка.");
                logger.Log(LogLevel.WARNING, "Это предупреждение.");
                logger.Log(LogLevel.CRITICAL, "Критическая ситуация!");
            }
            else
            {
                Console.WriteLine("Некорректный ввод. Пожалуйста, попробуйте снова.");
                continue;
            }

            Console.WriteLine("Нажмите Enter для повторного выбора уровня или введите 'exit' для выхода.");
            string exitInput = Console.ReadLine();
            if (exitInput?.ToLower() == "exit")
                break;
        }

        Console.ReadLine(); // Ожидание ввода для предотвращения немедленного закрытия консоли
    }
}