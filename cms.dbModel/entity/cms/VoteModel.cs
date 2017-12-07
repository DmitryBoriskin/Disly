using System;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
    /// <summary>
    /// Модель, описывающая список голосований
    /// </summary>
    public class VoteList
    {
        /// <summary>
        /// Список опросов
        /// </summary>
        public VoteModel[] Data { get; set; }

        /// <summary>
        /// Пейджер
        /// </summary>
        public Pager Pager { get; set; }
    }

    /// <summary>
    /// Модель, описывающая опрос
    /// </summary>
    public class VoteModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// false - еденичный выбор(радиокнопки), true - множественный выбор 
        /// </summary>
        public bool Type { get; set; }

        /// <summary>
        /// заголовок
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// описание
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Флаг запрещённости
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Пользовательский вариант ответа
        /// </summary>
        public bool HisAnswer { get; set; }

        /// <summary>
        /// начало опроса
        /// </summary>
        public DateTime DateStart { get; set; }
        
        /// <summary>
        /// завршение опроса
        /// </summary>
        public DateTime? DateEnd { get; set; }

        /// <summary>
        /// Список ответов
        /// </summary>
        public VoteAnswer[] Answer { get; set; }

        /// <summary>
        /// true  показываем статистику опроса
        /// </summary>
        public bool ShowStatistic { get; set; } 
    }

    /// <summary>
    /// Варианты ответов
    /// </summary>
    public class VoteAnswer
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid id { get; set; }

        /// <summary>
        /// Голосование
        /// </summary>
        public Guid VoteId { get; set; }

        /// <summary>
        /// Вариант
        /// </summary>
        public string Variant { get; set; }

        /// <summary>
        /// Сортировка
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// Статистика
        /// </summary>
        public VoteStat Statistic { get; set; }
    }

    /// <summary>
    /// Ответ пользователя
    /// </summary>
    public class VoteUsers
    {
        /// <summary>
        /// Ip-адрес
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Голосования
        /// </summary>
        public Guid VoteId { get; set; }

        /// <summary>
        /// Ответ
        /// </summary>
        public Guid AnserId { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }
    }

    /// <summary>
    /// Статистика ответа
    /// </summary>
    public class VoteStat
    {
        /// <summary>
        /// Общее кол-во ответов
        /// </summary>
        public int AllVoteCount { get; set; }

        /// <summary>
        /// Кол-во ответов на этот вопрос
        /// </summary>
        public int ThisVoteCount { get; set; }
    }

}
