using cms.dbModel.entity;
using System;

namespace cms.dbModel
{
    public abstract class abstract_AccountRepository
    {
        // ------------------------ Для CMS --------------------------
        #region Для CMS

        /// <summary>
        /// Работа с логами
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public abstract cmsLogModel[] getCmsUserLog(Guid UserId);
        public abstract cmsLogModel[] getCmsPageLog(Guid PageId);
        public abstract void insertLog(Guid PageId, Guid UserId, string Action, string IP);

        /// <summary>
        /// Получаем список пунктов cmsMenu
        /// </summary>
        /// <param name="ItemId">id для получения одной записи</param>
        /// <returns></returns>
        public abstract cmsMenuModel[] getCmsMenu(string SectionId, Guid UserId);
        public abstract cmsMenuModel getCmsMenu(Guid ItemId);
        public abstract bool updateCmsMenu(Guid id, cmsMenuModel Item);
        public abstract bool createCmsMenu(Guid id, cmsMenuModel Item);
        public abstract bool deleteCmsMenu(Guid id);
        public abstract bool permit_cmsMenu(Guid id, int num);

        /// <summary>
        /// Фильтр по секциям
        /// </summary>
        /// <param name="ItemId">id для получения одной записи</param>
        /// <returns></returns>
        public abstract SectionGroupModel[] getSectionsGroup(string SectionId);
        public abstract SectionGroupModel getSectionGroup(Guid ItemId);
        public abstract bool createSectionGroup(Guid id, SectionGroupModel Item);
        public abstract bool updateSectionGroup(string Alias, SectionGroupModel Item);
        public abstract bool deleteSectionGroup(string Alias);
        public abstract bool permit_SectionGroup(Guid Id, int num);

        /// <summary>
        /// Фильтр (разделы в секцииях)
        /// </summary>
        /// <param name="ItemId">id для получения одной записи</param>
        /// <returns></returns>
        public abstract SectionGroupItemsModel[] getSectionsGroupItem(string sectionId, string groupId);
        public abstract SectionGroupItemsModel getSectionGroupItem(Guid ItemId);
        public abstract bool createSectionGroupItem(Guid id, SectionGroupItemsModel Item);
        public abstract bool updateSectionGroupItem(Guid id, SectionGroupItemsModel Item);
        public abstract bool deleteSectionGroupItem(Guid id);
        public abstract bool permit_SectionGroupItem(Guid Id, int num);

        /// <summary>
        /// Карта сайта
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public abstract cmsSiteMapModel getCmsSiteMap(string SiteId, Guid Id);
        public abstract cmsSiteMapModel[] getCmsSiteMap(string SiteId, string Url);
        public abstract bool insCmsSiteMap(Guid id, string path, cmsSiteMapModel insert);
        public abstract bool setCmsSiteMap(Guid id, cmsSiteMapModel update);
        public abstract bool delCmsSiteMap(string SiteId, Guid id);
        public abstract cmsSiteMapTypeModel[] getSiteMapType();

        /// <summary>
        /// Новости
        /// </summary>
        /// <returns></returns>
        public abstract cmsMaterialsModel[] getSearchCmsMaterial(string SiteId, string SearсhLine, DateTime? Begin, DateTime? End, string category, string type);
        public abstract cmsMaterialsModel[] getMaterials(string SiteId);
        public abstract cmsMaterialsModel getMaterialsItem(string Path, string SiteId);
        public abstract cmsMaterialsModel getCmsMaterial(string SiteId, Guid id);
        public abstract bool insCmsMaterials(Guid id, cmsMaterialsModel insert);
        public abstract bool setCmsMaterials(Guid id, cmsMaterialsModel update);
        public abstract bool delCmsMaterials(string SiteId, Guid id);

        /// <summary>
        /// События (афиша)
        /// </summary>
        /// <returns></returns>
        public abstract PlaceCardModel[] getCmsPlaceCards(string SiteId);
        public abstract PlaceCardModel[] getPlaceCards(string SiteId);
        public abstract PlaceCardModel getPlaceCard(string SiteId, Guid id);
        public abstract bool createPlaceCard(Guid id, PlaceCardModel insert);
        public abstract bool updatePlaceCard(Guid id, PlaceCardModel update);
        public abstract bool deletePlaceCard(string SiteId, Guid id);

        /// <summary>
        /// Представления
        /// </summary>
        /// <returns></returns>
        public abstract cmsPageViewsModel[] getCmsPageViews();
        public abstract cmsSiteMapViewsModel[] getCmsPageViewsSelec();
        public abstract cmsPageViewsModel getCmsPageViews(Guid id);
        public abstract bool setCmsPageViews(Guid id, cmsPageViewsModel update);
        public abstract bool insCmsPageViews(Guid id, cmsPageViewsModel insert);
        public abstract bool delCmsPageViews(Guid id);


        /// <summary>
        /// Получаем информацию о пользователе
        /// </summary>
        /// <param name="Email">Email пользователя</param>
        /// <param name="Guid">Guid пользователя</param>
        /// <returns></returns>
        public abstract AccountModel getCmsAccount(string Email);
        public abstract AccountModel getCmsAccount(Guid Id);
        public abstract DomainList[] getUserDomains(Guid Id);


        /// <summary>
        /// Права пользователей
        /// </summary>
        /// <param name="_userId"></param>
        /// <returns></returns>
        public abstract UserResolutionModel getCmsUserResolutioInfo(Guid _userId, string _pageUrl);

        /// <summary>
        /// Информация о сайте
        /// </summary>
        /// <returns></returns>
        public abstract SettingsModel getCmsSettings();
        public abstract bool updateCmsSettings(SettingsModel settings);

        /// <summary>
        /// Страница пользователей
        /// </summary>
        /// <returns></returns>
        public abstract UsersModel[] getUserList(string domain);
        public abstract UsersModel[] getUsersList(string GroupName, string searchSurname, string searchName, string searchEmail, string domain);
        public abstract UsersModel[] getUserList(string GroupName, string domain);
        public abstract UsersModel[] getAllUserList(string group);
        public abstract UsersModel getUser(Guid? Id);
        public abstract UsersModel getUser(Guid? Id, string domain);
        public abstract UsersModel getUser(string email);
        public abstract bool isEmailFree(string email);
        public abstract bool createUser(Guid Id, UsersModel obj);
        public abstract bool createUser(Guid Id, string domain, UsersModel obj);
        public abstract bool deleteUser(Guid Id);
        public abstract bool deleteUser(Guid Id, string domain);
        public abstract bool updateUser(Guid Id, UsersModel obj);

        /// <summary>
        /// Сайты, связанные с пользователем (только для админов портала)
        /// </summary>
        /// <returns></returns>
        public abstract SitesModel[] getUserSiteLinks(Guid userId);
        public abstract bool createUserSiteLink(UserSiteLink obj);
        public abstract bool deleteUserSiteLink(Guid userId, string siteId);

        /// <summary>
        /// группа пользователей
        /// </summary>
        /// <returns></returns>
        public abstract UsersGroupModel[] getUserGroup();
        public abstract UsersGroupModel getUserGroup(string Alias);
        public abstract bool createUsersGroup(Guid id, string Alias, string GroupName);
        public abstract bool updateUsersGroup(string Alias, string GroupName);
        public abstract bool deleteUsersGroup(string Alias);
        
        /// <summary>
        /// Изменение пароля
        /// </summary>
        /// <param name="id"></param>
        /// <param name="NewSalt"></param>
        /// <param name="NewHash"></param>
        /// <returns></returns>
        public abstract bool changePasswordUser(Guid id, string NewSalt, string NewHash);

        /// <summary>
        /// права группы пользователей(просто отдельных пользователей)
        /// </summary>
        /// <param name="UserGroupId"></param>
        /// <returns></returns>
        public abstract ResolutionsTemplatesModel[] getResolutions(string UserGroupId);
        public abstract bool appointResolutionsTemplates(string user, Guid url, string action, int val);
        public abstract bool delResolutionsTemplates(string UserGroup);

        public abstract ResolutionsModel[] getResolutionsPerson(Guid UserId);
        public abstract bool appointResolutionsUser(Guid id, Guid url, string action, int val);


        /// <summary>
        /// Страница "Сайты"
        /// </summary>
        /// <returns></returns>
        public abstract SitesDomainModel getSiteId(string Domain);
        public abstract SitesDomainModel[] getSiteDomains(string siteId);
        public abstract bool createSiteDomain(SitesDomainModel obj);
        public abstract bool deleteSiteDomain(Guid id);
        public abstract SitesModel[] getSiteList(string SearchLine);
        public abstract SitesModel getSite(Guid? Id);
        public abstract SitesModel getSite(string domain);
        public abstract bool updateSite(Guid Id, SitesModel obj);
        public abstract bool createSite(Guid Id, SitesModel obj);
        public abstract bool deleteSite(Guid Id);


        /// <summary>
        /// Фотоальбомы
        /// </summary>
        /// <returns></returns>
        public abstract PhotoAlbumsModel[] getPhotoAlbums(string domain);
        public abstract PhotoAlbumsModel getPhotoAlbums(Guid id, string domain);
        public abstract bool insertPhotoAlbums(Guid id, PhotoAlbumsModel insert);
        public abstract bool updatePhotoAlbums(Guid id, PhotoAlbumsModel update);
        public abstract bool updatePhotoAlbumPreview(Guid id, string preview);
        public abstract bool deletePhotoAlbums(Guid id, string domain);


        /// <summary>
        /// Фотографии в фотоальбомах
        /// </summary>
        /// <returns></returns>
        public abstract PhotosModel[] getPhotos(Guid AlbumId);
        public abstract PhotosModel getPhoto(Guid Id);
        public abstract bool insertPhotos(Guid AlbumId, PhotosModel[] insert);
        public abstract bool deletePhotos(Guid AlbumId);
        public abstract bool deletePhoto(Guid Id);


        /// <summary>
        /// Видео
        /// </summary>
        /// <returns></returns>
        public abstract cmsVideoModel[] getVideo();
        public abstract cmsVideoModel getVideo(Guid id);

        /// <summary>
        /// Баннеры
        /// </summary>
        /// <returns></returns>
        //public abstract cmsBannersModel[] getBanners();
        //public abstract cmsBannersModel[] getBanners(string? Section, string? Type);
        public abstract BannersModel[] getBanners(string[] sections, string domain);
        public abstract cmsBannersModel[] getcmsBanners(string Section, string Type, string domain);

        public abstract cmsBannersModel getBanner(Guid id, string domain);
        public abstract bool insertBanners(Guid id, cmsBannersModel insert);
        public abstract bool updBanners(Guid id, cmsBannersModel update);
        public abstract bool delBanners(Guid id, string domain);

        
        /// <summary>
        /// Модули сайтов
        /// </summary>
        /// <returns></returns>
        public abstract SiteModulesModel[] getSiteModules();
        public abstract SiteModulesModel getSiteModule(Guid id);
        public abstract bool createSiteModule(Guid id, SiteModulesModel insert);
        public abstract bool updateSiteModule(Guid id, SiteModulesModel update);
        public abstract bool deleteSiteModule(Guid id);
        #endregion



        // ----------------------- Для сайта -------------------------
        #region Для сайта
        /// <summary>
        /// Получаем информацию о странице сайта
        /// </summary>
        /// <param name="Path">Путь к странице относительно корня сайта</param>
        /// <param name="Alias">Имя страницы</param>
        /// <returns></returns>
        public abstract siteMapModel getPageInfo(string Path, string Alias);
        public abstract siteMapModel getPageInfo(string Path, string Alias, string Domain);

        /// <summary>
        /// Получаем хлебные крошки
        /// </summary>
        /// <param name="Url">Путь к странице относительно корня сайта</param>
        /// <returns></returns>
        public abstract pagePathModel[] getPagePath(string Url, string Domain);

        public abstract siteMapModel[] getPageMenu(string Site);
        public abstract siteMapModel[] getPageChildElem(string Path, string Site);
        public abstract SitesModel[] getAllSitesInfo();
        #endregion
    }
}