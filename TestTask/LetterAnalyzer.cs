using System.Linq;

namespace TestTask
{
    public static class LetterAnalyzer
    {
        private const string vowels = "аеёиоуыэюя";
        private const string consonants = "бвгджзйклмнпрстфхцчшщъь";

        public static bool IsVowel(char c) 
            => vowels.Contains(c);
        
        public static bool IsConsonant(char c) 
            => consonants.Contains(c);
    }
}
