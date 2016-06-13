using Acr.UserDialogs;
using FWMobile.Infrastructure;
using FWMobile.Infrastructure.Models;
using FWMobile.Infrastructure.Models.Weather;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FWMobile.Modules.Races
{
    [ImplementPropertyChanged]
    public class RacePageModel : FreshMvvm.FreshBasePageModel
    {
        private IDataService _dataService;
        private IWeatherService _weatherService;
        private IUserDialogs _userDialogs;

        public Race Race { get; set; }
        public DayForecast Forecast { get; set; }
        public string MinTemperature { get; set; }
        public string MaxTemperature { get; set; }
        public string ChancePrecipitation { get; set; }
        public string PrecipitationType { get; set; }
        public string PrecipitationIntensity { get; set; }
        public ICommand MakePicksCommand { get; set; }

        public RacePageModel(IDataService dataService, IWeatherService weatherService, IUserDialogs userDialogs)
        {
            _dataService = dataService;
            _weatherService = weatherService;
            _userDialogs = userDialogs;
        }

        public async override void Init(object initData)
        {
            base.Init(initData);

            MakePicksCommand = new Command(() =>
            {
                CoreMethods.PushPageModel<PicksPageModel>(Race);
            });
            _userDialogs.ShowLoading("Loading race data");
            if (initData is Race)
            {
                Race = initData as Race;
                IList<DayForecast> dayForecasts = null;
                if (Race.RaceDate != DateTime.MinValue)
                {
                    DateTimeOffset raceDate = new DateTimeOffset(Race.RaceDate);
                    dayForecasts = await _weatherService.GetForecast(Race.Latitude, Race.Longitude, raceDate);
                    if (dayForecasts != null && dayForecasts.Count > 0)
                    {
                        Forecast = dayForecasts[0];
                        MinTemperature = Forecast.MinTemperature.ToString("N1");
                        MaxTemperature = Forecast.MaxTemperature.ToString("N1");
                        ChancePrecipitation = Forecast.PrecipitationProbability.ToString("P1");
                        PrecipitationType = Forecast.PrecipitationType;
                        PrecipitationIntensity = Forecast.PrecipitationIntensity.ToString("N0");
                    }
                }
                else
                {
                    dayForecasts = await _weatherService.GetForecast(Race.Latitude, Race.Longitude);
                }
            }
            _userDialogs.HideLoading();
        }
    }
}
