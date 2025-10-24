using System.Reflection;

namespace VerbGeneration
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new RegularVerbGeneration().GenerateRegularVerbForms();
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "VerbGeneration.Resources.regularVerbs_noPhrasalVerbs.txt";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var writeStream = new FileStream("regularVerbList.csv", FileMode.Create))
            using (var writer = new StreamWriter(writeStream))
            using (var reader = new StreamReader(stream))
            {
                var gen = new RegularVerbGeneration();
                while (!reader.EndOfStream)
                {
                    string baseForm = reader.ReadLine();
                    if (baseForm == null)
                    {
                        throw new Exception("Could not read the base form of a verb");
                    }
                    var newLine = $"{baseForm}, {gen.GetPast(baseForm)}, {gen.GetPastParticiple(baseForm)}, {gen.GetGerund(baseForm)}";
                    writer.WriteLine(newLine);
                }
            }
        }
    }
}
