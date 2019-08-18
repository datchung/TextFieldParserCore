using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace TextFieldParserCore
{
    public class TextFieldParserCore : IDisposable
    {
        //private const string EndingQuote = "\"[{0}]*";
        private const RegexOptions DefaultRegexOptions = RegexOptions.CultureInvariant;

        //private readonly int[] whiteSpaceCodes = new [] {0x0009, 0x000B, 0x000C, 0x0020, 0x0085, 0x00A0, 0x1680, 0x2000, 0x2001, 0x2002, 0x2003, 0x2004, 0x2005, 0x2006, 0x2007, 0x2008, 0x2009, 0x200A, 0x200B, 0x2028, 0x2029, 0x3000, 0xFEFF };
        //private string WhitespaceCharacters {
        //    get
        //    {
        //        var s = "";
        //        foreach(var c in whiteSpaceCodes)
        //        {
        //            var spaceChar = (char)c;
        //            if (CharacterIsInDelimiter(spaceChar)) continue;
        //            s += spaceChar;
        //        }

        //        return s;
        //    }
        //}

        //private bool CharacterIsInDelimiter(char testCharacter)
        //{
        //    foreach(var delimiter in delimiters)
        //        if (delimiter.IndexOf(testCharacter) > -1) return true;

        //    return false;
        //}

        private readonly TextReader textReader;
        private string[] delimiters;
        private Regex delimiterRegex;
        //private Regex delimiterWithEndCharsRegex;

        //private string EndQuotePattern { get => string.Format(CultureInfo.InvariantCulture, EndingQuote, WhitespacePattern); }
        //private string WhitespacePattern {
        //    get 
        //    {
        //        var builder = "";
        //        foreach (var code in whiteSpaceCodes)
        //        {
        //            var spaceChar = (char)code;
        //            if (CharacterIsInDelimiter(spaceChar)) continue;
        //            builder += "\\u" + code.ToString("X4", CultureInfo.InvariantCulture);
        //        }

        //        return builder;
        //    } 
        //}

        public bool EndOfData
        {
            get
            {
                if (textReader == null) return true;
                if (textReader.Peek() != -1) return false;
                return true;
            }
        }
        public FieldType TextFieldType { get; set; }

        public TextFieldParserCore(TextReader textReader)
        {
            this.textReader = textReader;
        }

        private string[] ParseDelimitedLine()
        {
            var line = ReadNextLine();
            if (string.IsNullOrWhiteSpace(line)) return new string[0];

            var index = 0;
            var fields = new List<string>();
            var lineEndIndex = line.Length - 1;

            while (index <= lineEndIndex)
            {
                var delimiterMatch = delimiterRegex.Match(line, index);

                if (delimiterMatch.Success)
                {
                    var field = line.Substring(index, delimiterMatch.Index - index);

                    //            If m_TrimWhiteSpace Then
                    //                Field = Field.Trim()
                    //            End If

                    fields.Add(field);

                    index = delimiterMatch.Index + delimiterMatch.Length;
                }
                else
                {
                    var field = line.Substring(index).TrimEnd((char)13, (char)10);

                    //            If m_TrimWhiteSpace Then
                    //                Field = Field.Trim()
                    //            End If
                    fields.Add(field);
                    break;
                }
            }

            return fields.ToArray();
        }
        
        public string[] ReadFields()
        {
            ValidateReadyToRead();

            return ParseDelimitedLine();
        }

        private void ValidateReadyToRead()
        {
            ValidateAndEscapeDelimiters();
        }

        private void ValidateAndEscapeDelimiters()
        {
            if (delimiters == null || delimiters.Length == 0) throw new ArgumentException("", nameof(delimiters));

            if(delimiters.Length > 1) throw new ArgumentException("Only one delimiter is supported at the moment.", nameof(delimiters));

            if (string.IsNullOrWhiteSpace(delimiters[0])) return;

            var builder = "";
            //var quoteBuilder = "";
            
            //quoteBuilder += $"{EndQuotePattern}(";

            var escapedDelimiter = Regex.Escape(delimiters[0]);

            builder += $"{escapedDelimiter}|";
            //quoteBuilder += $"{escapedDelimiter}|";

            delimiterRegex = new Regex(builder.Substring(0, builder.Length - 1), DefaultRegexOptions);
            //builder += "\r|\n";
            //delimiterWithEndCharsRegex = new Regex(builder, DefaultRegexOptions);
        }

        private string ReadNextLine()
        {
            if (textReader == null) return null;

            return textReader.ReadLine();
        }

        public void SetDelimiters(params string[] delimiters)
        {
            this.delimiters = delimiters;
        }

        public void Dispose()
        {
            Close();
        }

        private void Close()
        {
            CloseReader();
        }

        private void CloseReader()
        {
            if (textReader == null) return;
            textReader.Dispose();
        }
    }
}
