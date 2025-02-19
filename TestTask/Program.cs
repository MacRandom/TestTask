﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    public class Program
    {

        /// <summary>
        /// Программа принимает на входе 2 пути до файлов.
        /// Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
        /// Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
        /// По окончанию работы - выводит данную статистику на экран.
        /// </summary>
        /// <param name="args">Первый параметр - путь до первого файла.
        /// Второй параметр - путь до второго файла.</param>
        static void Main(string[] args)
        {
            Console.WriteLine("Подсчёт статистики вхождения символов/пар символов в заданных файлах.");
            IList<LetterStats> singleLetterStats;
            IList<LetterStats> doubleLetterStats;

            using (IReadOnlyStream inputStream1 = GetInputStream(args[0]))
            {
                singleLetterStats = FillSingleLetterStats(inputStream1);
            }

            using (IReadOnlyStream inputStream2 = GetInputStream(args[1]))
            {
                doubleLetterStats = FillDoubleLetterStats(inputStream2);
            }

            RemoveCharStatsByType(ref singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(ref doubleLetterStats, CharType.Consonants);

            Console.WriteLine(Environment.NewLine + $"Статистика вхождения символов в файле {args[0]}:");
            PrintStatistic(singleLetterStats);

            Console.WriteLine(Environment.NewLine + $"Статистика вхождения пар символов в файле {args[1]}:");
            PrintStatistic(doubleLetterStats);

            // TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
            Console.WriteLine(Environment.NewLine + "Для завершения работы нажмите любую клавишу.");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            return new ReadOnlyStream(fileFullPath);
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            var statsList = new List<LetterStats>();

            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.

                if (!statsList.Exists(item => item.Letter == c.ToString()))
                {
                    LetterStats letterStats = new LetterStats(c.ToString());

                    statsList.Add(letterStats);
                }

                IncStatistic(statsList.Find(item => item.Letter == c.ToString()));
            }

            return statsList;
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            var statsList = new List<LetterStats>();

            stream.ResetPositionToStart();

            string prev = stream.ReadNextChar().ToString().ToLower();

            while (!stream.IsEof)
            {
                string next = stream.ReadNextChar().ToString().ToLower();

                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.

                if (prev != next)
                {
                    prev = next;
                    continue;
                }

                string pair = string.Concat(next, prev);

                if (!statsList.Exists(item => item.Letter == pair))
                {
                    var stats = new LetterStats(pair);
                    statsList.Add(stats);
                }

                IncStatistic(statsList.Find(item => item.Letter == pair));
            }

            return statsList;
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(ref IList<LetterStats> letters, CharType charType)
        {
            // TODO : Удалить статистику по запрошенному типу букв.
            Func<char, bool> predicate = null;

            switch (charType)
            {
                case CharType.Consonants:
                    predicate = LetterAnalyzer.IsConsonant;
                    break;
                case CharType.Vowel:
                    predicate = LetterAnalyzer.IsVowel;
                    break;
            }

            if (predicate != null)
                letters = letters.Where(item => !predicate(item.Letter.ToLower().First())).ToList();
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
            letters = letters.OrderBy(item => item.Letter);

            foreach (var item in letters)
                Console.WriteLine($"{item.Letter} : {item.Count}");
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
        }
    }
}
