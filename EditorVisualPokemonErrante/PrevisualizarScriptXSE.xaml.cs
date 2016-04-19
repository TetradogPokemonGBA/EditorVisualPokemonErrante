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
        public const string CABEZERASCRIPT = "#dynamic 0x800000\r\n#org @ScriptPokemonErrante";
        public PrevisualizarScriptXSE()
        {
            InitializeComponent();
        }

        public PrevisualizarScriptXSE(int pokemon,int vida,byte nivel,byte stat):this()
        {
            string scriptE = CABEZERASCRIPT,scriptR=CABEZERASCRIPT;
            scriptE += "\r\nspecial " +MainWindow.VariableEspecialE;
            scriptE += "\r\nsetvar " + MainWindow.VariablePokemonE + " " + ((Hex)pokemon).ByteString;
            scriptE += "\r\nsetvar " + MainWindow.VariableVitalidadE + " " + ((Hex)vida).ByteString;
            scriptE += "\r\nsetvar " + MainWindow.VariableNivelYEstadoE + " " +((Hex)stat).ByteString + ((Hex)nivel).Number;
            scriptE += "\r\nend";

            scriptR += "\r\nspecial " + MainWindow.VariableEspecialR;
            scriptR += "\r\nsetvar " + MainWindow.VariablePokemonR + " " +((Hex)pokemon).ByteString;
            scriptR += "\r\nsetvar " + MainWindow.VariableVitalidadR + " " + ((Hex)vida).ByteString;
            scriptR += "\r\nsetvar " + MainWindow.VariableNivelYEstadoR + " "+((Hex)stat).ByteString + ((Hex)nivel).Number;
            scriptR += "\r\nend";
             
            imgVersionR.SetImage(Resource1.FireRed);
            txtScriptR.Text = scriptR;
            imgVersionE.SetImage(Resource1.Emerald);
            txtScriptE.Text = scriptE;
        }

    }
}
