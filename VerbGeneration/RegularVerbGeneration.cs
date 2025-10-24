using System.Reflection;

namespace VerbGeneration
{
    internal class RegularVerbGeneration
    {
        public void GenerateRegularVerbForms()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "VerbGeneration.Resources.regularVerbs_noPhrasalVerbs.txt";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var writeStream = new FileStream("regularVerbList.csv", FileMode.Create))
            using (var writer = new StreamWriter(writeStream))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {                    
                    string baseForm = reader.ReadLine();
                    var newLine = $"{baseForm}, {GetPast(baseForm)}, {GetPastParticiple(baseForm)}, {GetGerund(baseForm)}";
                    writer.WriteLine(newLine);
                }
            }
        }

        public string GetPast(string baseForm)
        {
            switch (baseForm[baseForm.Length - 1])
            {
                case 'e':
                    return baseForm + "d";
                case 'y':
                    return baseForm.Substring(0, baseForm.Length - 1) + "ied";
                default:
                    return baseForm + "ed";
            }
        }

        public string GetPastParticiple(string baseForm)
        {
            return GetPast(baseForm);
        }

        // Gerund = "-ing form"
        public string GetGerund(string baseForm)
        {
            if (baseForm[baseForm.Length - 1] == 'e')
            {
                // unionize -> unionizing
                return baseForm.Substring(0, baseForm.Length - 1) + "ing";
            }

            return baseForm + "ing";
        }
    }
}
