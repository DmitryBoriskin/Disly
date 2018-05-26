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
            Console.WriteLine("3 - фикс изображений одного сайта");
            Console.WriteLine("4 - фикс изображений всех сайтов");

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
                case "3":
                    FixImages(orgsIds);
                    break;
                case "4":
                    int c = 0;
                    foreach (int id in orgsIds)
                    {
                        c++;
                        var org = repository.GetOrg(id);
                        ServiceLogger.Info("{work}", $"организация: {org.Alias} --> {c} из {orgsIds.Count()}");
                        PhotoUpdater.Fix(org, repository, helper);
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

        /// <summary>
        /// Фикс изображение
        /// </summary>
        /// <param name="orgsIds"></param>
        private static void FixImages(int[] orgsIds)
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

            var org = repository.GetOrg(id);
            ServiceLogger.Info("{work}", $"организация: {org.Alias}");
            PhotoUpdater.Fix(org, repository, helper);
        }
    }
}
