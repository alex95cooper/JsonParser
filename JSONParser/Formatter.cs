using System;
using System.Collections.Generic;

namespace JSONParser
{
    internal class Formatter
    {
        private const string _newInterval = "    ";
        private const int _interval = 4;

        private readonly List<Lexem> _tokenList;
        private readonly List<bool> _tokenListIsValid;

        private string _indent;
        private int _counter;

        public Formatter(List<Lexem> tokenList)
        {
            _tokenList = tokenList;
            _indent = string.Empty;
            _tokenListIsValid = new();
        }

        public List<Lexem> GetFormattedTokenList()
        {
            for (int count = 0; count < _tokenList.Count; count++)
            {
                _tokenList[count].Value = _tokenList[count].Value.Trim();
                if (_tokenList[count].Key == (int)Tokens.OpenObjectBrace)
                {
                    if (count == 0)
                    {
                        _tokenList[count].Value = _tokenList[count].Value + Environment.NewLine + Environment.NewLine;
                    }
                    else if (count > 0 && _tokenList[count - 1].Key == (int)Tokens.Colon)
                    {
                        _tokenList[count].Value = _tokenList[count].Value + Environment.NewLine;
                    }
                    else
                    {
                        _tokenList[count].Value = _indent + _tokenList[count].Value + Environment.NewLine;
                    }

                    _indent += _newInterval;
                }
                else if (_tokenList[count].Key == (int)Tokens.OpenArrayBrace)
                {
                    _tokenList[count].Value = _tokenList[count].Value + Environment.NewLine;
                    _indent += _newInterval;
                }
                else if (_tokenList[count].Key == (int)Tokens.String || _tokenList[count].Key == (int)Tokens.IntOrDouble ||
                    _tokenList[count].Key == (int)Tokens.BoolOrNull)
                {
                    if (_tokenList[count - 1].Key == (int)Tokens.OpenArrayBrace || _tokenList[count - 1].Key == (int)Tokens.Comma)
                    {
                        _tokenList[count].Value = _indent + _tokenList[count].Value;
                    }

                    if (_tokenList[count + 1].Key == (int)Tokens.CloseArrayBrace || _tokenList[count + 1].Key == (int)Tokens.CloseObjectBrace)
                    {
                        _tokenList[count].Value = _tokenList[count].Value + Environment.NewLine;
                    }
                }
                else if (_tokenList[count].Key == (int)Tokens.Key)
                {
                    _tokenList[count].Value = _indent + _tokenList[count].Value;
                }
                else if (_tokenList[count].Key == (int)Tokens.Comma)
                {
                    _tokenList[count].Value = _tokenList[count].Value + Environment.NewLine;
                }
                else if (_tokenList[count].Key == (int)Tokens.Colon)
                {
                    _tokenList[count].Value = _tokenList[count].Value + " ";
                }
                else if (_tokenList[count].Key == (int)Tokens.CloseArrayBrace || _tokenList[count].Key == (int)Tokens.CloseObjectBrace)
                {
                    _indent = _indent.Remove(0, _interval);

                    _tokenList[count].Value = _indent + _tokenList[count].Value;
                    if (count < _tokenList.Count - 1 && _tokenList[count + 1].Key != (int)Tokens.Comma)
                    {
                        _tokenList[count].Value = _tokenList[count].Value + Environment.NewLine;
                    }
                }
            }

            return _tokenList;
        }

        public bool CheckIfTokenListValid()
        {
            _counter = 0;
            if (_tokenList == null)
            {
                return false;
            }

            if (_tokenList[0].Key != (int)Tokens.OpenObjectBrace || _tokenList[^1].Key != (int)Tokens.CloseObjectBrace)
            {
                return false;
            }

            _counter++;
            bool tokenListIsValid = CheckIfObjectValid();
            return (_counter == _tokenList.Count) && tokenListIsValid;
        }

        public List<Lexem> RemoveWhiteSpaces()
        {
            for (int count = 0; count < _tokenList.Count; count++)
            {
                _tokenList[count].Value = _tokenList[count].Value.Trim();
                if (_tokenList[count].Key == (int)Tokens.OpenObjectBrace || _tokenList[count].Key == (int)Tokens.OpenArrayBrace)
                {
                    _tokenList[count].Value = _tokenList[count].Value + " ";
                }
                else if (_tokenList[count].Key == (int)Tokens.String || _tokenList[count].Key == (int)Tokens.IntOrDouble ||
                    _tokenList[count].Key == (int)Tokens.BoolOrNull)
                {
                    if (_tokenList[count + 1].Key == (int)Tokens.CloseArrayBrace || _tokenList[count + 1].Key == (int)Tokens.CloseObjectBrace)
                    {
                        _tokenList[count].Value = _tokenList[count].Value + " ";
                    }
                }
                else if (_tokenList[count].Key == (int)Tokens.Comma || _tokenList[count].Key == (int)Tokens.Colon)
                {
                    _tokenList[count].Value = _tokenList[count].Value + " ";
                }
                else if (_tokenList[count].Key == (int)Tokens.CloseArrayBrace || _tokenList[count].Key == (int)Tokens.CloseObjectBrace)
                {
                    if (count < _tokenList.Count - 1 && _tokenList[count + 1].Key != (int)Tokens.Comma)
                    {
                        _tokenList[count].Value = _tokenList[count].Value + " ";
                    }
                }
            }

            return _tokenList;
        }

        private bool CheckIfObjectValid()
        {
            while (_counter < _tokenList.Count)
            {
                if (_tokenListIsValid.Contains(false))
                {
                    return false;
                }

                if (_tokenList[_counter].Key == (int)Tokens.Key)
                {
                    if (_tokenList[_counter + 2].Key == (int)Tokens.OpenObjectBrace)
                    {
                        _counter += 3;
                        _tokenListIsValid.Add(CheckIfObjectValid());
                    }
                    else if (_tokenList[_counter + 2].Key == (int)Tokens.OpenArrayBrace)
                    {
                        _counter += 3;
                        _tokenListIsValid.Add(CheckIfArrayValid());
                    }
                    else
                    {
                        _tokenListIsValid.Add(CheckIfKeyValueValid());
                    }
                }
                else if (_tokenList[_counter].Key == (int)Tokens.Comma && _tokenList[_counter + 1].Key != (int)Tokens.CloseObjectBrace)
                {
                    _counter++;
                }
                else if (_tokenList[_counter].Key == (int)Tokens.CloseObjectBrace)
                {
                    _counter++;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        private bool CheckIfArrayValid()
        {
            while (_counter < _tokenList.Count)
            {
                if (_tokenListIsValid.Contains(false))
                {
                    return false;
                }

                if (_tokenList[_counter].Key == (int)Tokens.String || _tokenList[_counter].Key == (int)Tokens.IntOrDouble ||
                    _tokenList[_counter].Key == (int)Tokens.BoolOrNull)
                {
                    if (_tokenList[_counter + 1].Key == (int)Tokens.Comma || _tokenList[_counter + 1].Key == (int)Tokens.CloseArrayBrace)
                    {
                        _counter++;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (_tokenList[_counter].Key == (int)Tokens.OpenObjectBrace)
                {
                    _counter++;
                    _tokenListIsValid.Add(CheckIfObjectValid());
                }
                else if (_tokenList[_counter].Key == (int)Tokens.Comma && _tokenList[_counter + 1].Key != (int)Tokens.CloseArrayBrace)
                {
                    _counter++;
                }
                else if (_tokenList[_counter].Key == (int)Tokens.CloseArrayBrace)
                {
                    if (_tokenList[_counter + 1].Key == (int)Tokens.Comma || _tokenList[_counter + 1].Key == (int)Tokens.CloseObjectBrace)
                    {
                        _counter++;
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

        private bool CheckIfKeyValueValid()
        {
            List<int> keyValueTokens = new();
            for (int count = 0; count < 4; count++)
            {
                if (_counter + count < _tokenList.Count)
                {
                    keyValueTokens.Add((int)_tokenList[_counter + count].Key);
                }
            }

            int keyValueToken = int.Parse(string.Join("", keyValueTokens));
            if (Enum.IsDefined(typeof(KeyValueTokens), keyValueToken))
            {
                _counter += 3;
                return true;
            }

            return false;
        }
    }
}
