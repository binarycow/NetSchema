using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace NetSchema.Syntax
{
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    internal ref struct TokenEnumerator
    {
        private const int STATE_NOT_STARTED = 0;
        private const int STATE_IN_PROGRESS = 1;
        private const int STATE_FINISHED = 2;
        private readonly string text;
        private int state;
        private int index;
        private int tokenEnd;
        private YangToken token;
        public TokenEnumerator(string text)
        {
            this.text = text;
            this.state = STATE_NOT_STARTED;
            this.index = 0;
            this.token = default;
            this.tokenEnd = default;
        }


        public string GetCurrentString() => this.text.Substring(tokenEnd - Current.Length, Current.Length);
        
        public bool MoveNext()
        {
            switch (this.state)
            {
                case STATE_FINISHED:
                    return false;
                
                case { } when this.index >= this.text.Length:
                    this.state = STATE_FINISHED;
                    this.token = default;
                    return false;
                
                case STATE_NOT_STARTED:
                    this.index = 0;
                    this.state = STATE_IN_PROGRESS;
                    goto case STATE_IN_PROGRESS;
                
                case STATE_IN_PROGRESS:
                    if (TryToken())
                    {
                        tokenEnd += this.token.Length;
                        return true;
                    }
                    else if (this.index == this.text.Length)
                    {
                        this.state = STATE_FINISHED;
                        return false;
                    }
                    throw new NotImplementedException();
                
                default:
                    throw new InvalidOperationException();
            }
        }

        public YangToken Current => this.token;

        private bool TryToken()
        {
            if (index == text.Length)
                return false;
            if (TryGetSingleChar())
                return true;
            if (TryGetWhiteSpace())
                return true;
            if (TryGetComment())
                return true;
            if (TryGetUnquoted())
                return true;
            if (TryGetSingleQuote())
                return true;
            if (TryGetDoubleQuote())
                return true;
            throw new NotImplementedException();
        }

        private bool TryGetDoubleQuote()
        {            
            if (index < 0 || index >= text.Length || text[index] != '\"')
                return false;
            var end = index + 1;
            
            while (end < text.Length)
            {
                if (text[end] == '\\' && end + 1 < text.Length && text[end + 1] == '\"')
                {
                    end += 2;
                    continue;
                }
                if (text[end] == '\"')
                    break;
                ++end;
            }
            ++end;
            return MakeToken(YangTokenType.DoubleQuotedString, end - index);

        }

        private bool TryGetSingleQuote()
        {
            if (index < 0 || index >= text.Length || text[index] != '\'')
                return false;
            var end = index + 1;
            while (end < text.Length)
            {
                if (text[end] == '\'')
                    break;
                ++end;
            }
            ++end;
            return MakeToken(YangTokenType.SingleQuotedString, end - index);
        }

        private bool TryGetUnquoted()
        {
            if (index < 0 || index >= text.Length)
                return false;
            if (text[index] == '\'' || text[index] == '\"')
                return false;

            var end = index;
            var next = end + 1 < text.Length ? text[end + 1] : '\0';
            var cur = text[end];
            while (end < text.Length && IsValidChar(cur, next))
            {
                ++end;
                cur = next;
                next = end + 1 < text.Length ? text[end + 1] : '\0';
            }
            var len = end - index;
            return MakeToken(YangTokenType.UnquotedString, len);

            static bool IsValidChar(char cur, char next) => (cur, next) switch
            {
                (' ', _) => false,
                ('\t', _) => false,
                ('\r', _) => false,
                ('\n', _) => false,
                ('\'', _) => false,
                ('\"', _) => false,
                (';', _) => false,
                ('{', _) => false,
                ('}', _) => false,
                ('/', '/') => false,
                ('/', '*') => false,
                ('*', '/') => false,
                _ => true,
            };
        }


        private bool TryGetComment()
        {
            if (index < 0 || index + 1 >= text.Length || IsValidStart(text, index) == false)
                return false;

            throw new NotImplementedException();

            static bool IsValidStart(string text, int index) => (text[index], text[index + 1]) switch
            {
                ('/', '/') => true,
                ('/', '*') => true,
                _ => false,
            };
        }
        private bool TryGetWhiteSpace()
        {
            var end = index;
            while (end < text.Length && IsWhiteSpace(text[end]))
                ++end;
            var len = end - index;
            return MakeToken(YangTokenType.WhiteSpace, len);
        }

        private static bool IsWhiteSpace(char ch) => ch switch
        {
            ' ' => true,
            '\t' => true,
            '\r' => true,
            '\n' => true,
            _ => false
        };

        private bool TryGetSingleChar()
        {
            if (index < 0 || index >= text.Length)
                return false;
            var type = text[index] switch
            {
                '{' => YangTokenType.OpenBrace,
                '}' => YangTokenType.CloseBrace,
                ';' => YangTokenType.Semicolon,
                _ => YangTokenType.Unknown,
            };
            return MakeToken(type, 1);
        }

        private bool MakeToken(YangTokenType type, int length)
        {
            token = default;
            if (type == YangTokenType.Unknown || length == 0)
                return false;
            token = new (type, length);
            index += length;
            return true;
        }

    }
}