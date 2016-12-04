
namespace NetGL.Constructor {
   internal static class Config {
       public const string PropertyOnlineHelpUrlFormat = "https://www.google.com/search?q={0}.{1}";
       public const string PropertyDetailedDescriptionApiUrlFormat = "http://cssodessa.com/NetGL/api/Help/GetDetailedDescription?class={0}&property={1}";
       public const int WebRequestTimeout = 5000;
       public const int PropertyMaxDetailedDescriptionLength = 1024;
    }
}