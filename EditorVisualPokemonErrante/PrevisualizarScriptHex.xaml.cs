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
using Gabriel.Cat.Extension;
namespace EditorVisualPokemonErrante
{
    /// <summary>
    /// Lógica de interacción para PrevisualizarScriptHex.xaml
    /// </summary>
    public partial class PrevisualizarScriptHex : Window
    {
        public PrevisualizarScriptHex(ImageSource img, int pokemon, int vida, byte nivel, byte stat, bool isEsmeralda)
        {
            string scriptHex = new PrevisualizarScriptXSE(img, pokemon, vida, nivel, stat, isEsmeralda).txtScript.Text;
            InitializeComponent();
            if(img!=null)
            this.imgVersion.Source = img;
            scriptHex = scriptHex.Remove(0,PrevisualizarScriptXSE.CABEZERASCRIPT.Length).Replace("special","0x25").Replace("setvar", "0x16").Replace("end", "0xFF");
            this.txtScript.Text =scriptHex;
        }


    }
}
