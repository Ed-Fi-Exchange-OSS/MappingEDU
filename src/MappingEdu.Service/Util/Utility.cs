// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using MappingEdu.Core.Domain.System;
using MappingEdu.Service.Model.ElementList;

namespace MappingEdu.Service.Util
{
    public static class Utility
    {
        public static string GetFullItemPath(SystemItem item)
        {
            return string.Join(".", GetAllItemNames(item));
        }

        public static string GetItemPathSpaced(SystemItem item)
        {
            return string.Join(" ", GetItemNames(item));
        }

        public static string GetShortItemPath(SystemItem systemItem)
        {
            var pathItemCount = GetAllItems(systemItem, item => item.ParentSystemItem).Count();
            var firstPartCount = pathItemCount > 2 ? 2 : 1;
            var secondPartCount = 1;

            var pathFirstPart = string.Join(".", GetFirstItemNames(systemItem, firstPartCount));
            var pathSecondPart = string.Join(".", GetLastItemNames(systemItem, secondPartCount));
            var middle = pathItemCount > firstPartCount + secondPartCount ? ".." : ".";

            var path = string.Format("{0}{1}{2}", pathFirstPart, middle, pathSecondPart);

            if (path.Length < 80 && pathItemCount > 3)
            {
                secondPartCount = 2;
                pathSecondPart = string.Join(".", GetLastItemNames(systemItem, secondPartCount));
                middle = pathItemCount > firstPartCount + secondPartCount ? ".." : ".";
                var newPath = string.Format("{0}{1}{2}", pathFirstPart, middle, pathSecondPart);
                if (newPath.Length < 80) return newPath;
            }

            return path;
        }

        public static SystemItem GetDomain(SystemItem systemItem)
        {
            return GetAllItems(systemItem, x => x.ParentSystemItem).Reverse().ToArray()[0];
        }

        public static string GetDomainItemPath(SystemItem item)
        {
            return string.Join(".", GetItemNames(item));
        }

        private static IEnumerable<string> GetAllItemNames(SystemItem systemItem)
        {
            return new[] {systemItem.MappedSystem.SystemName + " " + systemItem.MappedSystem.SystemVersion}
                .Union(GetItemNames(systemItem));
        }

        public static IEnumerable<ElementListViewModel.ElementPathViewModel.PathSegment> GetAllItemSegments(
            SystemItem systemItem, bool returnDataStandard = true)
        {
            if (returnDataStandard)
            {
                return new[]
                {
                    new ElementListViewModel.ElementPathViewModel.PathSegment
                    {
                        Name = systemItem.MappedSystem.SystemName + " " + systemItem.MappedSystem.SystemVersion
                    }
                }.Union(GetItemSegments(systemItem));
            }

            return GetItemSegments(systemItem);
        }

        private static IEnumerable<string> GetItemNames(SystemItem systemItem)
        {
            return GetAllItems(systemItem, x => x.ParentSystemItem).Reverse().Select(item => item.ItemName);
        }

        private static IEnumerable<ElementListViewModel.ElementPathViewModel.PathSegment> GetItemSegments(SystemItem systemItem)
        {
            return
                GetAllItems(systemItem, x => x.ParentSystemItem)
                    .Reverse()
                    .Select(item => new ElementListViewModel.ElementPathViewModel.PathSegment
                    {
                        Name = item.ItemName,
                        Definition = item.Definition,
                        SystemItemId = item.SystemItemId,
                        IsExtended = item.IsExtended
                    });
        }

        private static IEnumerable<string> GetFirstItemNames(SystemItem systemItem, int number)
        {
            var items = GetAllItems(systemItem, x => x.ParentSystemItem).Reverse().ToArray();
            return items.Take(number).Select(item => item.ItemName);
        }

        private static IEnumerable<string> GetLastItemNames(SystemItem systemItem, int number)
        {
            var items = GetAllItems(systemItem, x => x.ParentSystemItem).Reverse().ToArray();
            return items.Skip(Math.Max(0, items.Length - number)).Select(item => item.ItemName);
        }

        private static IEnumerable<T> GetAllItems<T>(T item, Func<T, T> next) where T : class
        {
            while (null != item)
            {
                yield return item;
                item = next(item);
            }
        }

        public static Guid GetElementGroupSystemItemId(SystemItem systemItem)
        {
            return
                GetAllItems(systemItem, x => x.ParentSystemItem)
                    .Reverse()
                    .First()
                    .SystemItemId;
        }

        public static string RemoveStopWords(string value)
        {
            string[] stopWords = {"a", "about", "above", "above", "across", "after", "afterwards", "again", "against", "all", "almost", "alone", "along", "already", "also", "although", "always", "am", "among", "amongst", "amoungst", "amount", "an", "and", "another", "any", "anyhow", "anyone", "anything", "anyway", "anywhere", "are", "around", "as", "at", "back", "be", "became", "because", "become", "becomes", "becoming", "been", "before", "beforehand", "behind", "being", "below", "beside", "besides", "between", "beyond", "bill", "both", "bottom", "but", "by", "call", "can", "cannot", "cant", "co", "con", "could", "couldnt", "cry", "de", "describe", "detail", "do", "done", "down", "due", "during", "each", "eg", "eight", "either", "eleven", "else", "elsewhere", "empty", "enough", "etc", "even", "ever", "every", "everyone", "everything", "everywhere", "except", "few", "fifteen", "fify", "fill", "find", "fire", "first", "five", "for", "former", "formerly", "forty", "found", "four", "from", "front", "full", "further", "get", "give", "go", "had", "has", "hasnt", "have", "he", "hence", "her", "here", "hereafter", "hereby", "herein", "hereupon", "hers", "herself", "him", "himself", "his", "how", "however", "hundred", "ie", "if", "in", "inc", "indeed", "interest", "into", "is", "it", "its", "itself", "keep", "last", "latter", "latterly", "least", "less", "ltd", "made", "many", "may", "me", "meanwhile", "might", "mill", "mine", "more", "moreover", "most", "mostly", "move", "much", "must", "my", "myself", "name", "namely", "neither", "never", "nevertheless", "next", "nine", "no", "nobody", "none", "noone", "nor", "not", "nothing", "now", "nowhere", "of", "off", "often", "on", "once", "one", "only", "onto", "or", "other", "others", "otherwise", "our", "ours", "ourselves", "out", "over", "own", "part", "per", "perhaps", "please", "put", "rather", "re", "same", "see", "seem", "seemed", "seeming", "seems", "serious", "several", "she", "should", "show", "side", "since", "sincere", "six", "sixty", "so", "some", "somehow", "someone", "something", "sometime", "sometimes", "somewhere", "still", "such", "system", "take", "ten", "than", "that", "the", "their", "them", "themselves", "then", "thence", "there", "thereafter", "thereby", "therefore", "therein", "thereupon", "these", "they", "thickv", "thin", "third", "this", "those", "though", "three", "through", "throughout", "thru", "thus", "to", "together", "too", "top", "toward", "towards", "twelve", "twenty", "two", "un", "under", "until", "up", "upon", "us", "very", "via", "was", "we", "well", "were", "what", "whatever", "when", "whence", "whenever", "where", "whereafter", "whereas", "whereby", "wherein", "whereupon", "wherever", "whether", "which", "while", "whither", "who", "whoever", "whole", "whom", "whose", "why", "will", "with", "within", "without", "would", "yet", "you", "your", "yours", "yourself", "yourselves"};
            var valueWords = value.Split(' ');
            var result = string.Join(" ", valueWords.Where(w => !stopWords.Contains(w)));
            return result;
        }

        public static string RemoveInvalidCharacters(string worksheetName)
        {
            worksheetName = worksheetName.Replace(@"\", "");
            worksheetName = worksheetName.Replace("/", "");
            worksheetName = worksheetName.Replace("*", "");
            worksheetName = worksheetName.Replace("[", "");
            worksheetName = worksheetName.Replace("]", "");
            worksheetName = worksheetName.Replace(":", "");
            worksheetName = worksheetName.Replace("?", "");
            worksheetName = worksheetName.Replace("\"", "");
            worksheetName = worksheetName.Replace("<", "");
            worksheetName = worksheetName.Replace(">", "");
            worksheetName = worksheetName.Replace("|", "");
            return worksheetName;
        }


        public static string ReplaceCommonWords(string value)
        {
            //Underscores are replaced with spaces
            var thesaurus = new
            {
                _begin_ = new [] { " start ", " entry " },
                _course_ = new [] { " class " },
                _type_ = new [] { " category " },
                _id_ = new [] {" identification code ", " identifier "},
                _local_education_agency_ = new[] { " lea " },
                _electronic_mail_ = new [] {" email "}
            };
            //Adds a space at end for if any of these are the end
            value = value + " ";
            return thesaurus.GetType().GetProperties().Aggregate(value, (current1, prop) => ((string[]) prop.GetValue(thesaurus)).Aggregate(current1, (current, str) => current.Replace(str, prop.Name.Replace('_', ' '))));
        }
    }
}