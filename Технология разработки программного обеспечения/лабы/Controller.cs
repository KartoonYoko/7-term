
namespace Lab{
    /// <summary>
    /// Класс для обработки HTTP запросов.
    /// </summary>
    class Controller{
        private VK VK;
        private ApplicationContext Context;
        private TelegramSevice Telegram;
        private WhatsAppSevice WhatsAppSevice;
        public Controller(ApplicationContext context, TelegramSevice telegramSevice, WhatsAppSevice whatsAppSevice){
            ApplicationContext = context;
            Telegram = telegramSevice;
            WhatsAppSevice = whatsAppSevice;
            VK = new();
        }
        /// <summary>
        /// Генерирует страницу диалога.
        /// </summary>
        /// <returns></returns>
        public IActionResult Conversation(){  }
        /// <summary>
        /// Генерирует страницу чатов.
        /// </summary>
        /// <returns></returns>
        public IActionResult Chats(){ }
    }
    /// <summary>
    /// Класс для работы с API ВК.
    /// </summary>
    class VK{
        /// <summary>
        /// Токен для работы с серверами ВК.
        /// </summary>
        /// <value></value>
        private string Token {get; set;}
        public VK(){
            Token = GetToken();
        }
        /// <summary>
        /// Получить токен.
        /// </summary>
        /// <returns></returns>  
        private string GetToken(){}
        /// <summary>
        /// Отправить сообщение.
        /// </summary>
        public void SendMessage(){}
        /// <summary>
        /// Получить информацию об аккаунте.
        /// </summary>
        public void GetAccInfo(){}

    }
    /// <summary>
    /// Контескт БД.
    /// </summary>
    class ApplicationContext{
        /// <summary>
        /// Таблица сообщений whatsapp.
        /// </summary>
        DbSet<MessageWhatsApp> MessagesWhatsApp;
        /// <summary>
        /// Таблица сообщений ВК.
        /// </summary>
        DbSet<MessageVK> MessagesVK;
        /// <summary>
        /// Таблица сообщений телеграма.
        /// </summary>
        DbSet<MessageTelegram> MessagesTelegram;
    }
    /// <summary>
    /// Telegram сервис.
    /// </summary>
    class TelegramSevice : IMessengerSrvice{
        /// <summary>
        /// Токен для работы с API Telegram.
        /// </summary>
        /// <value></value>  
        private string Token {get; set;}
        /// <summary>
        /// Отправить сообщение.
        /// </summary>
        public override void SendMessage(){}
        /// <summary>
        /// Получить список чатов.
        /// </summary>
        public void GetChats(){}
        /// <summary>
        /// Получить информацию о своем аккаунте.
        /// </summary>
        public void GetAccInfo(){}
    }
    /// <summary>
    /// WhatsApp сервис.
    /// </summary>
    class WhatsAppSevice : IMessengerSrvice{
        public override void SendMessage(){}
    }
    /// <summary>
    /// Интерфейс для мессенджеров-сеовисов.
    /// </summary>
    interface IMessengerSrvice{
        /// <summary>
        /// Отправка сообщения.
        /// </summary>
        void SendMessage(){}
    }
}