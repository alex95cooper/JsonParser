using System;
using System.Collections.Generic;
using System.Globalization;

namespace JSONParser
{
    internal class Lexer
    {
        private readonly string _input;

        public List<Lexem> _lexems;
        private int _counter;

        public Lexer(string input)
        {
            _lexems = new List<Lexem>();
            _input = input;
        }

        public List<Lexem> MoveNext()
        {
            if (_input == string.Empty)
            {
                return null;
            }

            while (_counter < _input.Length)
            {
                if (_input[_counter] == ' ')
                {
                    _counter++;
                }
                else if (_input[_counter] == '{' || _input[_counter] == '}' ||
                    _input[_counter] == '[' || _input[_counter] == ']' ||
                    _input[_counter] == ':' || _input[_counter] == ',')
                {
                    (int token, string symbol) = GetAnySymbol();
                    _lexems.Add(new Lexem(token, symbol));
                    _counter++;
                }
                else if (_input[_counter] == '"')
                {
                    (int token, string keyОrString) = GetKeyOrString();
                    _lexems.Add(new Lexem(token, keyОrString));
                }
                else if (Char.IsNumber(_input[_counter]))
                {
                    (int token, string intОrDouble) = GetIntOrDouble();
                    _lexems.Add(new Lexem(token, intОrDouble));
                }
                else if (Char.IsLetter(_input[_counter]))
                {
                    (int token, string intОrDouble) = GetBoolOrNull();
                    _lexems.Add(new Lexem(token, intОrDouble));
                }
                else
                {
                    return null;
                }
            }

            if (_lexems.Contains(null))
            {
                return null;
            }

            return _lexems;
        }

        private (int, string) GetAnySymbol()
        {
            if (_input[_counter] == '{')
            {
                return ((int)Tokens.OpenObjectBrace, "{ ");
            }
            else if (_input[_counter] == '}')
            {
                return ((int)Tokens.CloseObjectBrace, "}");
            }
            else if (_input[_counter] == '[')
            {
                return ((int)Tokens.OpenArrayBrace, "[ ");
            }
            else if (_input[_counter] == ']')
            {
                return ((int)Tokens.CloseArrayBrace, "]");
            }
            else if (_input[_counter] == ':')
            {
                return ((int)Tokens.Colon, ": ");
            }
            else
            {
                return ((int)Tokens.Comma, ", ");
            }
        }

        private (int, string) GetKeyOrString()
        {
            List<char> interimList = EnsureStringValid();
            if (interimList == null)
            {
                return ((int)Tokens.NotValidToken, null);
            }

            while (_counter < _input.Length)
            {
                _counter++;
                if (_input[_counter] != ' ')
                {
                    if (_input[_counter] == ':')
                    {
                        return ((int)Tokens.Key, new string(interimList.ToArray()));
                    }
                    else if (_input[_counter] == ',' || _input[_counter] == '}' || _input[_counter] == ']')
                    {
                        return ((int)Tokens.String, new string(interimList.ToArray()));
                    }
                    else
                    {
                        return ((int)Tokens.NotValidToken, null);
                    }
                }
            }

            return ((int)Tokens.NotValidToken, null);
        }

        private List<char> EnsureStringValid()
        {
            List<char> interimList = new();
            interimList.Add(_input[_counter]);

            while (_counter < _input.Length)
            {
                _counter++;
                interimList.Add(_input[_counter]);
                if (_input[_counter] == '\\')
                {
                    if (_input[_counter + 1] == '\\' || _input[_counter + 1] == '"')
                    {
                        interimList.Add(_input[_counter]);
                        interimList.Add(_input[_counter + 1]);
                        _counter++;
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (_input[_counter] == '"')
                {
                    return interimList;
                }
            }

            return null;
        }

        private (int, string) GetIntOrDouble()
        {
            List<char> interimList = GetAnyWordOrNumber();
            string interimString = new(interimList.ToArray());
            if (int.TryParse(interimString, out _))
            {
                return ((int)Tokens.Int, interimString);
            }
            else if (double.TryParse(interimString, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
            {
                return ((int)Tokens.Double, interimString);
            }
            else
            {
                return ((int)Tokens.NotValidToken, null);
            }
        }

        private (int, string) GetBoolOrNull()
        {
            List<char> interimList = GetAnyWordOrNumber();
            string interimString = new(interimList.ToArray());
            if (interimString == "true" || interimString == "true " ||
                interimString == "false" || interimString == "false " ||
                interimString == "null" || interimString == "null ")
            {
                return ((int)Tokens.BoolOrNull, interimString);
            }
            else
            {
                return ((int)Tokens.NotValidToken, null);
            }
        }

        private List<char> GetAnyWordOrNumber()
        {
            List<char> interimList = new();
            while (_counter < _input.Length)
            {
                if (_input[_counter] == ' ' || _input[_counter] == ']' || _input[_counter] == '}')
                {
                    interimList.Add(' ');
                    return interimList;
                }
                else if (_input[_counter] == ',')
                {
                    return interimList;
                }

                interimList.Add(_input[_counter]);
                _counter++;
            }

            return interimList;
        }
    }
}