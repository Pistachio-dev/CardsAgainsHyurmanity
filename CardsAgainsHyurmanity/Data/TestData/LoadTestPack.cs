namespace CardsAgainsHyurmanity.Data
{
    public class LoadTestPack : ICustomCahPack
    {
        public string Name => "Load test, don't use";

        public string Description => "Load test, don't use";

        public bool Official => false;

        public string[] Black => [
            "In the final raid, as per tradition, you will fight _ inside _",
            "_'s hunting log has several entries for _, obviously.",
            "Welcome to _, please put away your minions, weapons and _",
            "After several days with mods not working, mare users have resorted to _",
            "_ did nothing wrong, except _",
            "Your band cannot _ Valigarrrrmannda",            
        ];

        public string[] White => [
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec tincidunt sollicitudin lobortis. Duis at rhoncus sem. Cras malesuada ante eu arcu molestie, eu consectetur eros euismod. Quisque sit amet mattis nisl. Nunc vulputate eget nibh at eleifend. Maecenas ut lorem vel metus molestie vestibulum. Vivamus non hendrerit lectus. Fusce tempor turpis libero, eget tempor ante luctus eget. Vivamus ornare venenatis sa",
            "Not just very glib, extremely glib",
            "The same end, again and again",
            "Queer bubble",
            "Literally who?",
            "Queue pop!",            
        ];
    }
}
