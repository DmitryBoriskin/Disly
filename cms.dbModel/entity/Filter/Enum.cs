using System;
using System.Web;

/// <summary>
/// Типы контента
/// </summary>
public enum ContentType
{
    UNDEFINED = 0,
    MATERIAL = 1,
    EVENT = 2,
    PHOTO = 3,
}
/// <summary>
/// Типы объектов, которые могут иметь свои сайты(домены)
/// </summary>
public enum ContentLinkType
{
    UNDEFINED = 0,
    EVENT = 1,
    ORG = 2,
    PERSON = 3
}
