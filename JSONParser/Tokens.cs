using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONParser
{
    internal enum Tokens
    {
        NotValidToken,
        OpenObjectBrace,
        CloseObjectBrace,
        OpenArrayBrace,
        CloseArrayBrace,
        Colon,
        Comma,
        Key,
        String,
        Int,
        Double,
        BoolOrNull
    }
}
