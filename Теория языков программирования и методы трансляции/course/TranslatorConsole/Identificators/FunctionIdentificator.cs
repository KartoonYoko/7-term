using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorConsole.Identificators
{
    public class FunctionIdentificator : Identificator
    {
        public string Name { get; set; }
        public string ReturnType { get; set; }
        /// <summary>
        /// TODO аргументы должны быть уникальными идентификаторами
        /// т.е. не пересекаться с внешними идентификаторами с теми же названиями
        /// </summary>
        public List<string> Arguments { get; set; } = new();


    }
}
