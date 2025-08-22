using CardsAgainstHyurmanity.Model.Game;
using DalamudBasics.Logging;
using DalamudBasics.SaveGames;
using Metalama.Extensions.DependencyInjection;
using Metalama.Framework.Aspects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstHyurmanity.Modules
{
    public class StateChangingAndSavingActionAttribute : OverrideMethodAspect
    {
        [IntroduceDependency]
        private readonly ISaveManager<CahGame> saveManager;
        [IntroduceDependency]
        private readonly CahGame game;
        [IntroduceDependency]
        private readonly ILogService logService;

        public override dynamic? OverrideMethod()
        {
            var returnValue = meta.Proceed();
            try
            {
                saveManager.WriteSave(game, true);
            }
            catch (Exception ex)
            {
                this.logService.Error(ex, "Error on " + nameof(StateChangingAndSavingActionAttribute));
            }

            return returnValue;
        }
    }
}
