using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorConsole.Identificators
{
    /// <summary>
    /// Идентификатор переменной.
    /// </summary>
    public class VariableIdentificator : Identificator
    {
        /// <summary>
        /// Название переменной.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// тип переменной
        /// </summary>
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
