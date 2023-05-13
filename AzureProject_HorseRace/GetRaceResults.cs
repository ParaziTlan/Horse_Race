using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PlayFab.ServerModels;
using System.Collections.Generic;

namespace HorseRace.Racing
{
    public class TitleAuthenticationContext
    {
        public string Id { get; set; }
        public string EntityToken { get; set; }
    }

    public class FunctionExecutionContext<T>
    {
        public PlayFab.ProfilesModels.EntityProfileBody CallerEntityProfile { get; set; }
        public TitleAuthenticationContext TitleAuthenticationContext { get; set; }
        public bool? GeneratePlayStreamEvent { get; set; }
        public T FunctionArgument { get; set; }
    }

    public static class GetRaceResults
    {
        [FunctionName("GetRaceResults")]
        public static async Task<dynamic> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());

            dynamic args = context.FunctionArgument;


            if (args.playerHorseId > args.opponentHorseId) // Player Has Better Horse , So we are increasing its win count on server
            {

                var apiSettings = new PlayFab.PlayFabApiSettings()
                {
                    TitleId = context.TitleAuthenticationContext.Id,
                    DeveloperSecretKey = "JZAEU4XB49TEXYWXYX1UFBGGJPWT9DZDT7M94P4ZOXZEUNN3DE"
                };

                PlayFab.PlayFabAuthenticationContext titleContext = new PlayFab.PlayFabAuthenticationContext();

                titleContext.EntityToken = context.TitleAuthenticationContext.EntityToken;
                var serverAPI = new PlayFab.PlayFabServerInstanceAPI(apiSettings, titleContext);

                //LeaderBoard!
                StatisticUpdate statisticUpdate = new StatisticUpdate();
                statisticUpdate.StatisticName = "Wins";
                statisticUpdate.Value = 1;

                List<StatisticUpdate> statisticsListUpdate = new List<StatisticUpdate>();
                statisticsListUpdate.Add(statisticUpdate);

                UpdatePlayerStatisticsRequest updatePlayerStatisticsRequest = new UpdatePlayerStatisticsRequest();
                updatePlayerStatisticsRequest.Statistics = statisticsListUpdate;
                updatePlayerStatisticsRequest.PlayFabId = context.CallerEntityProfile.Lineage.MasterPlayerAccountId;

                var uptaPlayerStatisticsResult = await serverAPI.UpdatePlayerStatisticsAsync(updatePlayerStatisticsRequest);

                if (uptaPlayerStatisticsResult.Error == null)
                {
                    return new { uptaPlayerStatisticsResult.Result, won = true };
                }
                else return uptaPlayerStatisticsResult.Error;

            }

            return new { won = false };



        }
    }
}
