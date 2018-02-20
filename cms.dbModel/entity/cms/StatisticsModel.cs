using System;

namespace cms.dbModel.entity
{   
    /// <summary>
    /// Статистика пресс-центр
    /// </summary>
    public class StatisticMaterial
    {
        public string Title { get; set; }
        public string Domain { get; set; }
        public int CountAll { get; set; }
        /// <summary>
        /// Количесвто анонсов
        /// </summary>
        public int CountAnnouncement { get; set; }
        /// <summary>
        /// Количесвто актуально
        /// </summary>
        public int CountActual{ get; set; }
        /// <summary>
        /// Количесвто новостей
        /// </summary>
        public int CountNews { get; set; }
        /// <summary>
        /// Количесвто сми о нас
        /// </summary>
        public int CountSmi { get; set; }
        /// <summary>
        /// Количесвто Мастер классы
        /// </summary>
        public int CountMasterClasses { get; set; }
        /// <summary>
        /// Количесвто наши гости
        /// </summary>
        public int CountGuests { get; set; }
        /// <summary>
        /// Количесвто мероприятия
        /// </summary>
        public int CountEvents { get; set; }
        /// <summary>
        /// Количесвто фото
        /// </summary>
        public int CountPhoto { get; set; }
        /// <summary>
        /// Количесвто видео
        /// </summary>
        public int CountVideo { get; set; }
        /// <summary>
        /// Количесвто новое в медицине
        /// </summary>
        public int CountNewInMedicin { get; set; }        
        
    }
 
    public class StaticticMaterialGroup
    {
        public Guid OrgId { get; set; }
        public string Rubric { get; set; }
        public int? Count { get; set; }
    }



    public class StatisticFeedBack
    {
        public string Title { get; set; }
        public string Domain { get; set; }
        /// <summary>
        /// количество отзывов
        /// </summary>
        public int RewiewCount { get; set; }
        /// <summary>
        /// количестов отзывов на которые дан ответ
        /// </summary>
        public int RewiewAnswerCount { get; set; }
        /// <summary>
        /// количестов отзывов на которые не дан ответ
        /// </summary>
        public int RewiewNoAnswerCount { get; set; }
        /// <summary>
        /// количество обращений
        /// </summary>        
        public int AppealCount { get; set; }
        /// <summary>
        /// количество опубликованных обращений 
        /// </summary>        
        public int AppealPublish { get; set; }
        /// <summary>
        /// количество неопубликованных обращений 
        /// </summary>        
        public int AppealNoPublish { get; set; }
    }  
}
