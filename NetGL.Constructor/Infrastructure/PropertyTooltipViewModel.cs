using NetGL.Core.Infrastructure;
using NetGL.SceneGraph.Components;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace NetGL.Constructor.Infrastructure {
    public class PropertyTooltipViewModel : NotifyPropertyChange {
        private bool _isLoading;
        private string _detaildedDescription;

        public PropertyInfo Property { get; private set; }
        public string DisplayName { get; private set; }
        public string PropertyName { get; private set; }
        public string PropertyType { get; private set; }
        public string Description { get; private set; }
        public string OnlineHelpURL { get; private set; }

        public string DetailedDescription {
            get {
                if (_detaildedDescription == null)
                    LoadDetails();

                return _detaildedDescription;
            }
            set {
                if (_detaildedDescription == value)
                    return;
                _detaildedDescription = value;
                OnPropertyChanged();
            }
        }
        public bool IsLoading {
            get { return _isLoading; }
            set {
                if (_isLoading == value)
                    return;

                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public PropertyTooltipViewModel(PropertyInfo prop) {
            Property = prop;

            DisplayName = DisplayAttribute.GetPropertyDisplayName(prop);
            PropertyName = "{0}.{1}".UseFormat(prop.DeclaringType.Name, prop.Name);
            PropertyType = prop.PropertyType.Name;

            Description = DisplayAttribute.GetPropertyDescription(prop);
            OnlineHelpURL = Config.PropertyOnlineHelpUrlFormat
                .UseFormat(prop.DeclaringType.FullName, prop.Name);
            DetailedDescription = null;
        }

        private async void LoadDetails() {
            if (_detaildedDescription != null)
                return;
            if (IsLoading)
                return;
            lock (this) {
                if (IsLoading)
                    return;

                IsLoading = true;
            }

            try {
                DetailedDescription = await Task.Run(new Func<string>(GetDetailedDescription));
            }
            catch (Exception ex) {
                Log.Exception(ex);
                DetailedDescription = "Error loading description..";
            }
            finally {
                IsLoading = false;
            }
        }
        private string GetDetailedDescription() {
            var url = Config.PropertyDetailedDescriptionApiUrlFormat
                .UseFormat(Property.DeclaringType.FullName, Property.Name);
            var request = HttpWebRequest.CreateHttp(url);
            request.ReadWriteTimeout = Config.WebRequestTimeout;
            request.Timeout = Config.WebRequestTimeout;
            request.MediaType = "text/html";

            string result = null;
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
                result = reader.ReadToEnd();

            if (result.Length > Config.PropertyMaxDetailedDescriptionLength)
                throw new InvalidDataException("Text too long.");

            return result;
        }
    }
}
