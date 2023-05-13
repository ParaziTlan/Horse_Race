using UnityEngine;
using PlayFab;
using PlayFab.CloudScriptModels;
using System;
using Newtonsoft.Json;

namespace HorseRace.MultiPlayer
{
    public static class PlayfabCloudCall
    {
        private static Action<bool> callbackAfterCompleted;
        public static void CallCloudScript(Action<bool> callBack, int horseId, int otherHorseId)
        {
            ExecuteFunctionRequest cloudFunction = new ExecuteFunctionRequest()
            {
                FunctionName = "GetRaceResults",
                FunctionParameter = new { playerHorseId = horseId, opponentHorseId = otherHorseId },
                GeneratePlayStreamEvent = true
            };

            callbackAfterCompleted = callBack;
            PlayFabCloudScriptAPI.ExecuteFunction(cloudFunction, CloudFunctionCompleted, CloudFunctionFailed);
        }

        private static void CloudFunctionCompleted(ExecuteFunctionResult result)
        {
            dynamic deSerializedResult = JsonConvert.DeserializeObject<dynamic>(result.FunctionResult.ToString());

            callbackAfterCompleted((bool)deSerializedResult.won);
        }

        private static void CloudFunctionFailed(PlayFabError error)
        {
            Debug.LogError(error.ErrorMessage);
        }
    }
}
