using Gabriel.Cat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EditorVisualPokemonErrante
{
    /// <summary>
    /// Lógica de interacción para ExportarXSE.xaml
    /// </summary>
    public partial class PrevisualizarScriptXSE : Window
    {
        public const string CABEZERASCRIPT = "#org scriptPokemonErrante";
        public PrevisualizarScriptXSE(ImageSource img,int pokemon,int vida,byte nivel,byte stat,bool isEsmeralda)
        {
            string variableEspecial= isEsmeralda ? ToHex((int)VariablesEsmeralda.Special) : ToHex((int)VariablesRojoFuego.Special);
            string variablePokemon = isEsmeralda ? ToHex((int)VariablesEsmeralda.Pokemon): ToHex((int)VariablesRojoFuego.Pokemon);
            string variableNivelYEstado= isEsmeralda ? ToHex((int)VariablesEsmeralda.NivelYEstado) : ToHex((int)VariablesRojoFuego.NivelYEstado);
            string variableVitalidad = isEsmeralda ? ToHex((int)VariablesEsmeralda.Vitalidad) : ToHex((int)VariablesRojoFuego.Vitalidad);
            string script = CABEZERASCRIPT;
            script += "\r\n special " + variableEspecial;
            script += "\r\n setvar " + variablePokemon + " " + ToHex(nivel);
            script += "\r\n setvar " + variableVitalidad + " " + ToHex(vida);
            script += "\r\n setvar " + variableNivelYEstado + " 0x" + ((Hex)stat).ToString() + ((Hex)nivel).ToString();
            script += "\r\n end";
            InitializeComponent();
            if (img != null)
                imgVersion.Source = img;
            txtScript.Text = script;
           
        }

        

        private string ToHex(int num)
        {
            return "0x"+((Hex)num).ToString();
        }
    }
}
