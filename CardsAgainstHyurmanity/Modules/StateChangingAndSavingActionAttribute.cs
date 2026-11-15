using CardsAgainstHyurmanity.Model.Game;
using Dalamud.Plugin.Services;
using DalamudBasics.Logging;
using DalamudBasics.SaveGames;
using Metalama.Extensions.DependencyInjection;
using Metalama.Framework.Aspects;
using System;
using System.Threading.Tasks;

namespace CardsAgainstHyurmanity.Modules
{
    public class StateChangingAndSavingActionAttribute : OverrideMethodAspect
    {
        //[IntroduceDependency]
        //private readonly ISaveManager<CahGame> saveManager;
        //[IntroduceDependency]
        //private readonly ILogService logService;
        [IntroduceDependency]
        private readonly IFramework _framework;

        public override dynamic? OverrideMethod()
        {
            Task? saveTask = null;
            var returnValue = meta.Proceed();
            try
            {
                this._framework.RunOnFrameworkThread(() =>
                    {
                        if (meta.This.saveManager.GetCharacterSaveInMemory() == null)
                        {
                            throw new Exception("Attempting to write a null save");
                        }

                        meta.This.logService.Info(System.Text.Json.JsonSerializer.Serialize(meta.This.saveManager.GetCharacterSaveInMemory()));
                        meta.This.saveManager.WriteCharacterSave();
                    });
            }
            catch (Exception ex)
            {
                meta.This.logService.Error(ex, "Error on " + nameof(StateChangingAndSavingActionAttribute));
                if (saveTask?.Exception != null)
                {
                    meta.This.logService.Error(saveTask.Exception, "Error on framework thread");
                }
            }

            return returnValue;
        }

    }
}
