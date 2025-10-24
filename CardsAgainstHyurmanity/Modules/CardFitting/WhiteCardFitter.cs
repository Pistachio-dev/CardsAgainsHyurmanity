using DalamudBasics.Configuration;
using DalamudBasics.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CardsAgainstHyurmanity.Modules.WhiteCardFitting;

// Changes white cards to better match the black card
public class WhiteCardFitter
{
    public WhiteCardFitter(ILogService log, IConfigurationService<Configuration> configService)
    {
        this.log = log;
        this.configService = configService;
    }
    private readonly Hashtable Present = new();
    private readonly Hashtable Past = new();
    private readonly Hashtable PastParticiple = new();
    private readonly Hashtable Gerund = new();

    record class FullForm (string present, string past, string pastParticiple, string infinitive);
    private readonly ILogService log;
    private readonly IConfigurationService<Configuration> configService;

    // Exceptions: we check the second word if the first one is one of these
    private readonly string[] exceptions = ["not", "limit", "accidentally", "single", "subtly", "french"];

    public List<string> AdaptWhiteCards(string blackCard, List<string> whiteCards)
    {
        if (!configService.GetConfiguration().MatchVerbsToBlackCards)
        {
            return whiteCards;
        }

        if (Present.Count == 0) LoadDictionaries();
        var verb = ExtractVerbFormInfo(blackCard);
        if (verb == VerbForm.NoChange) return whiteCards;
        return whiteCards.Select(wc => AdaptCard(wc, verb)).ToList();
    }

    private VerbForm ExtractVerbFormInfo(string blackCard)
    {
        var regex = new Regex("@([0-3])$");
        var match = regex.Match(blackCard);
        if (!match.Success)
        {
            return VerbForm.NoChange;
        }

        string formNumber = match.Groups[1].Value;
        return formNumber switch
        {
            "0" => VerbForm.Present,
            "1" => VerbForm.Past,
            "2" => VerbForm.PastParticiple,
            "3" => VerbForm.Gerund,
            _ => VerbForm.Gerund
        };
    }

    private string AdaptCard(string card, VerbForm formToChangeTo)
    {
        var tokenizedCard = card.Split(' ');
        var possibleVerb = exceptions.Contains(tokenizedCard[0].ToLower()) ? tokenizedCard[1] : tokenizedCard[0];
        var match = FindMatch(possibleVerb);
        if (match != null)
        {
            var word = GetForm(match, formToChangeTo);
            card = new Regex(Regex.Escape(possibleVerb), RegexOptions.IgnoreCase).Replace(card, word, 1);
        }

        return card;
    }

    private string GetForm(FullForm fullForm, VerbForm target)
    {
        return target switch
        {
            VerbForm.Present => fullForm.present,
            VerbForm.Past => fullForm.past,
            VerbForm.PastParticiple => fullForm.pastParticiple,
            VerbForm.Gerund => fullForm.infinitive,
            _ => throw new NotImplementedException()
        };
    }

    private FullForm? FindMatch(string word)
    {
        Hashtable[] collections = [Present, Past, PastParticiple, Gerund];
        var wordLower = word.ToLower();
        foreach (var collection in collections)
        {
            if (collection.Contains(wordLower)) return (FullForm?)(collection[wordLower]);
        }

        return null;
    }
    public void LoadDictionaries(string routeToResources = "CardsAgainstHyurmanity.Data.Language")
    {
        log.Info("Loading verbs from resource");
        LoadFromResource($"{routeToResources}.irregularVerbList.csv");
        LoadFromResource($"{routeToResources}.regularVerbList.csv");
        log.Info($"Loaded: Present({Present.Count} Past({Past.Count} " +
            $"PastParticiple({PastParticiple.Count} Infinitive({Gerund.Count} ");
    }

    private void LoadFromResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using (var stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
            {
                throw new EndOfStreamException($"Could not read the resource {resourceName}");
            }
            using (var reader = new StreamReader(stream))
            {
                int index = 0;
                while (!reader.EndOfStream)
                {
                    var baseForm = reader.ReadLine();
                    if (baseForm == null) throw new EndOfStreamException($"Could not read the resource {resourceName}");
                    var verbs = baseForm.Split(',').Select(x => x.Trim()).ToArray();
                    try
                    {
                        FullForm full = new FullForm(verbs[0].Split("/").First(),
                            verbs[1].Split("/").First(),
                            verbs[2].Split("/").First(),
                            verbs[3].Split("/").First());
                        var enumerator = verbs.GetEnumerator();
                        AddToCollection(Present, verbs[0], full);
                        AddToCollection(Past, verbs[1], full);
                        AddToCollection(PastParticiple, verbs[2], full);
                        AddToCollection(Gerund, verbs[3], full);
                    }
                    catch (ArgumentException)
                    {
                        // Word was already added
                        continue;
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex, "Could not read all 4 forms of a verb.");
                    }
                    finally
                    {
                        index++;
                    }                    
                }
            }
        }
    }
    private void AddToCollection(Hashtable collection, string conjugation, FullForm fullForm)
    {
        foreach (var variant in conjugation.Split("/"))
        {
            collection.Add(variant, fullForm);
        }
    }
}

