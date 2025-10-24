using CardsAgainstHyurmanity.Modules.WhiteCardFitting;
using DalamudBasics.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class CardFittingFixture
    {
        internal WhiteCardFitter fitter;
        public CardFittingFixture()
        {
            fitter = new WhiteCardFitter(new Mock<ILogService>().Object);
            fitter.LoadDictionaries();
        }

        public List<string> SourceCards = new() {
            "forgetting to update your portrait",
            "not doing something",
            "single being terrible",
            "limit cutting your veins",
            "accidentally sending an ERP chat to your FC",
            "subTly teSting this feature",
            "French kissing a Molbol",
            "perpetuating the cycle of abuse",
            "call the police",
            "not done anything",
            "tried to run away",
            "No verbs on this one"
            };
    public List<string> ExpectedPresent = new() {
            "forget to update your portrait",
            "not do something",
            "single be terrible",
            "limit cut your veins",
            "accidentally send an ERP chat to your FC",
            "subTly test this feature",
            "French kiss a Molbol",
            "perpetuate the cycle of abuse",
            "call the police",
            "not do anything",
            "try to run away",
            "No verbs on this one"
    };
    public List<string> ExpectedPast = new() {
            "forgot to update your portrait",
            "not did something",
            "single was terrible",
            "limit cut your veins",
            "accidentally sent an ERP chat to your FC",
            "subTly tested this feature",
            "French kissed a Molbol",
            "perpetuated the cycle of abuse",
            "called the police",
            "not did anything",
            "tried to run away",
            "No verbs on this one"
    };

    public List<string> ExpectedPastParticiple = new() {
            "forgotten to update your portrait",
            "not done something",
            "single been terrible",
            "limit cut your veins",
            "accidentally sent an ERP chat to your FC",
            "subTly tested this feature",
            "French kissed a Molbol",
            "perpetuated the cycle of abuse",
            "called the police",
            "not done anything",
            "tried to run away",
            "No verbs on this one"
    };
        public List<string> ExpectedInfinitive = new() {
            "forgetting to update your portrait",
            "not doing something",
            "single being terrible",
            "limit cutting your veins",
            "accidentally sending an ERP chat to your FC",
            "subTly testing this feature",
            "French kissing a Molbol",
            "perpetuating the cycle of abuse",
            "calling the police",
            "not doing anything",
            "trying to run away",
            "No verbs on this one"
        };        
    }
}
