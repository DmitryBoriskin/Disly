using ImportOldInfo.Models;
using System;
using System.Linq;

namespace ImportOldInfo
{
    class Program
    {
        private static Repository repository = new Repository();
        private static ParamsHelper helper = new ParamsHelper();

        /// <summary>
        /// Точка входа
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //int[] orgsIds = repository.GetOrgsIds();
            int[] orgsIds = repository.GetSitesIds();

            Console.WriteLine("приложение для переноса инфы со старого med.cap.ru");
            Console.WriteLine("1 - перенос по одному");
            Console.WriteLine("2 - перенос всех сайтов");

            string variant = Console.ReadLine();

            switch (variant)
            {
                case "1":
                    ImportSingleSite(orgsIds);
                    break;
                case "2":
                    int count = 0;
                    foreach (int id in orgsIds)
                    {
                        count++;
                        ServiceLogger.Info("{work}", $"организация {count} из {orgsIds.Count()}");
                        Run(repository.GetOrg(id));
                    }
                    break;
            }

            Console.WriteLine("импорт завершён");
            Console.ReadLine();
        }

        /// <summary>
        /// Запуск переноса
        /// </summary>
        /// <param name="id"></param>
        private static void Run(Org org)
        {
            ServiceLogger.Info("{work}", $"сайт {org.Alias}");

            // перенос изображений
            PhotoImport.Execute(org, repository, helper);
            //PhotoImport.CreateAlbums(org, repository, helper);

            // перенос новостей
            repository.DeleteMaterials(org);
            repository.ImportMaterials(org);

            // перенос обратной связи
            repository.DeleteFeedbacks(org);
            repository.ImportFeedbacks(org);

            // перенос вакансий
            repository.DeleteVacancies(org);
            repository.ImportVacancies(org);
            
            // прикрепление фотогаллерей
            repository.UpdateContentWithGallery(org);
        }

        /// <summary>
        /// Перенос одного сайта
        /// </summary>
        private static void ImportSingleSite(int[] orgsIds)
        {
            int id = 0;
            do
            {
                Console.WriteLine("введите id организации: ");
                try
                {
                    id = int.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("такого id не существует");
                }
            } while (!orgsIds.Contains(id));
            
            Run(repository.GetOrg(id));
        }
    }
}
