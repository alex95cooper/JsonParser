using System;
using System.Collections.Generic;

namespace JSONParser
{
    internal class Formatter
    {
        private const string _newInterval = "  ";

        private readonly List<Lexem> _tokenList;

        private List<string> _formattedJsonString;
        private List<bool> _tokenListIsValid;
        private string _indent;

        public Formatter(List<Lexem> tokenList)
        {
            _tokenList = tokenList;
            _indent = string.Empty;
            _tokenListIsValid = new();
            _formattedJsonString = new();
        }

        public string GetJsonString()
        {
            for (int count = 0; count < _tokenList.Count; count++)
            {
                if (_tokenList[count].Key == (int)Tokens.OpenObjectBrace || _tokenList[count].Key == (int)Tokens.OpenArrayBrace)
                {
                    if (count > 0 && _tokenList[count - 1].Key != (int)Tokens.Colon)
                    {
                        _formattedJsonString.Add(_indent);
                    }

                    _formattedJsonString.Add(_tokenList[count].Value + Environment.NewLine);
                    _indent += _newInterval;
                }
                else if (_tokenList[count].Key == (int)Tokens.String || _tokenList[count].Key == (int)Tokens.Int ||
                    _tokenList[count].Key == (int)Tokens.Double || _tokenList[count].Key == (int)Tokens.BoolOrNull)
                {
                    if (_tokenList[count - 1].Key == (int)Tokens.OpenArrayBrace || _tokenList[count - 1].Key == (int)Tokens.Comma)
                    {
                        _formattedJsonString.Add(_indent);
                    }

                    _formattedJsonString.Add(_tokenList[count].Value);

                    if (_tokenList[count + 1].Key == (int)Tokens.CloseArrayBrace || _tokenList[count + 1].Key == (int)Tokens.CloseObjectBrace)
                    {
                        _formattedJsonString.Add(Environment.NewLine);
                    }
                }
                else if (_tokenList[count].Key == (int)Tokens.Key)
                {
                    _formattedJsonString.Add(_indent + _tokenList[count].Value);
                }
                else if (_tokenList[count].Key == (int)Tokens.Comma)
                {
                    _formattedJsonString.Add(_tokenList[count].Value + Environment.NewLine);
                }
                else if (_tokenList[count].Key == (int)Tokens.CloseArrayBrace || _tokenList[count].Key == (int)Tokens.CloseObjectBrace)
                {
                    _indent = _indent.Remove(0, 2);
                    _formattedJsonString.Add(_indent + _tokenList[count].Value);

                    if (count < _tokenList.Count - 1 && _tokenList[count + 1].Key != (int)Tokens.Comma)
                    {
                        _formattedJsonString.Add(Environment.NewLine);
                    }
                }
                else
                {
                    _formattedJsonString.Add(_tokenList[count].Value);
                }
            }

            return string.Join("", _formattedJsonString.ToArray());
        }

        public bool CheckIfTokenListValid()
        {
            if (_tokenList == null)
            {
                return false;
            }

            int tokenListCounter = 0;

            if (_tokenList[0].Key != (int)Tokens.OpenObjectBrace || _tokenList[^1].Key != (int)Tokens.CloseObjectBrace)
            {
                return false;
            }

            tokenListCounter++;
            return CheckIfObjectValid(ref tokenListCounter);
        }

        private bool CheckIfObjectValid(ref int counter)
        {
            while (counter < _tokenList.Count)
            {
                if (_tokenListIsValid.Contains(false))
                {
                    return false;
                }

                if (_tokenList[counter].Key == (int)Tokens.Key)
                {
                    if (_tokenList[counter + 2].Key == (int)Tokens.OpenObjectBrace)
                    {
                        counter += 3;
                        _tokenListIsValid.Add(CheckIfObjectValid(ref counter));
                    }
                    else if (_tokenList[counter + 2].Key == (int)Tokens.OpenArrayBrace)
                    {
                        counter += 3;
                        _tokenListIsValid.Add(CheckIfArrayValid(ref counter));
                    }
                    else
                    {
                        _tokenListIsValid.Add(CheckIfKeyValueValid(ref counter));
                    }
                }
                else if (_tokenList[counter].Key == (int)Tokens.Comma && _tokenList[counter + 1].Key != (int)Tokens.CloseObjectBrace)
                {
                    counter++;
                }
                else if (_tokenList[counter].Key == (int)Tokens.CloseObjectBrace)
                {
                    counter++;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        private bool CheckIfArrayValid(ref int counter)
        {
            while (counter < _tokenList.Count)
            {
                if (_tokenListIsValid.Contains(false))
                {
                    return false;
                }

                if (_tokenList[counter].Key == (int)Tokens.String || _tokenList[counter].Key == (int)Tokens.Int ||
                    _tokenList[counter].Key == (int)Tokens.Double || _tokenList[counter].Key == (int)Tokens.BoolOrNull)
                {
                    if (_tokenList[counter + 1].Key == (int)Tokens.Comma || _tokenList[counter + 1].Key == (int)Tokens.CloseArrayBrace)
                    {
                        counter++;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (_tokenList[counter].Key == (int)Tokens.OpenObjectBrace)
                {
                    counter++;
                    _tokenListIsValid.Add(CheckIfObjectValid(ref counter));
                }
                else if (_tokenList[counter].Key == (int)Tokens.Comma && _tokenList[counter + 1].Key != (int)Tokens.CloseArrayBrace)
                {
                    counter++;
                }
                else if (_tokenList[counter].Key == (int)Tokens.CloseArrayBrace)
                {
                    if (_tokenList[counter + 1].Key == (int)Tokens.Comma || _tokenList[counter + 1].Key == (int)Tokens.CloseObjectBrace)
                    {
                        counter++;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        private bool CheckIfKeyValueValid(ref int keyValueCount)
        {
            List<int> keyValueTokens = new();
            for (int count = 0; count < 4; count++)
            {
                if (keyValueCount + count < _tokenList.Count)
                {
                    keyValueTokens.Add((int)_tokenList[keyValueCount + count].Key);
                }
            }

            int keyValueToken = int.Parse(string.Join("", keyValueTokens));
            if (Enum.IsDefined(typeof(KeyValueTokens), keyValueToken))
            {
                keyValueCount += 3;
                return true;
            }

            return false;
        }
    }
}
