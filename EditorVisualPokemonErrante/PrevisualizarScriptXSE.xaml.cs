using Gabriel.Cat;
using Gabriel.Cat.Extension;
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
        public PrevisualizarScriptXSE()
        {
            InitializeComponent();
        }

        public PrevisualizarScriptXSE(int pokemon,int vida,byte nivel,byte stat):this()
        {
            string variableEspecialE= ToHex((int)VariablesEsmeralda.Special), variableEspecialR= ToHex((int)VariablesRojoFuego.Special);
            string variablePokemonE = ToHex((int)VariablesEsmeralda.Pokemon), variablePokemonR= ToHex((int)VariablesRojoFuego.Pokemon);
            string variableNivelYEstadoE=ToHex((int)VariablesEsmeralda.NivelYEstado) , variableNivelYEstadoR= ToHex((int)VariablesRojoFuego.NivelYEstado);
            string variableVitalidadE= ToHex((int)VariablesEsmeralda.Vitalidad) , variableVitalidadR= ToHex((int)VariablesRojoFuego.Vitalidad);
            string scriptE = CABEZERASCRIPT,scriptR=CABEZERASCRIPT;
            scriptE += "\r\n special " + variableEspecialE;
            scriptE += "\r\n setvar " + variablePokemonE + " " + ToHex(nivel);
            scriptE += "\r\n setvar " + variableVitalidadE + " " + ToHex(vida);
            scriptE += "\r\n setvar " + variableNivelYEstadoE + " 0x" + ((Hex)stat).ToString() + ((Hex)nivel).ToString();
            scriptE += "\r\n end";

            scriptR += "\r\n special " + variableEspecialR;
            scriptR += "\r\n setvar " + variablePokemonR + " " + ToHex(nivel);
            scriptR += "\r\n setvar " + variableVitalidadR + " " + ToHex(vida);
            scriptR += "\r\n setvar " + variableNivelYEstadoR + " 0x" + ((Hex)stat).ToString() + ((Hex)nivel).ToString();
            scriptR += "\r\n end";

            
            
            imgVersionR.SetImage(Resource1.FireRed);
            txtScriptR.Text = scriptR;
            imgVersionE.SetImage(Resource1.Emerald);
            txtScriptE.Text = scriptE;
        }
        private string ToHex(int num)
        {
            return "0x"+((Hex)num).ToString();
        }
    }
}
