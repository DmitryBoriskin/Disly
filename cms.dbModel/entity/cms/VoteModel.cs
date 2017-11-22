using System;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
    /// <summary>
    /// Модель, описывающая список новостей
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
        public bool Disabled { get; set; }
        public bool HisAnswer { get; set; }
        /// <summary>
        /// начало опроса
        /// </summary>
        public DateTime DateStart { get; set; }
        /// <summary>
        /// завршение опроса
        /// </summary>
        public DateTime? DateEnd { get; set; }
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
        public Guid id { get; set; }
        public Guid VoteId { get; set; }
        public string Variant { get; set; }
        public int Sort { get; set; }
        public VoteStat Statistic { get; set; }
    }
    /// <summary>
    /// статистика ответов
    /// </summary>
    public class VoteUsers
    {
        public string Ip { get; set; }
        public Guid VoteId { get; set; }
        public Guid AnserId { get; set; }
        public DateTime Date { get; set; }
    }
    public class VoteStat
    {
        public int AllVoteCount { get; set; }
        public int ThisVoteCount { get; set; }
    }

}
