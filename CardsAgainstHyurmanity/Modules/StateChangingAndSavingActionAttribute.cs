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
        [IntroduceDependency]
        private readonly ISaveManager<CahGame> saveManager;
        [IntroduceDependency]
        private readonly ILogService logService;
        [IntroduceDependency]
        private readonly IFramework framework;

        public override dynamic? OverrideMethod()
        {
            Task? saveTask = null;
            var returnValue = meta.Proceed();
            try
            {
                saveTask = framework.RunOnFrameworkThread(() =>
                {
                    if (saveManager.GetCharacterSaveInMemory() == null)
                    {
                        throw new Exception("Attempting to write a null save");
                    }

                    saveManager.WriteCharacterSave();
                });
            }
            catch (Exception ex)
            {
                this.logService.Error(ex, "Error on " + nameof(StateChangingAndSavingActionAttribute));
                if (saveTask?.Exception != null)
                {
                    this.logService.Error(saveTask.Exception, "Error on framework thread");
                }                
            }

            return returnValue;
        }
    }
}
